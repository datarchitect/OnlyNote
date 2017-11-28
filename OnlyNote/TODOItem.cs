using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows.Media;

namespace OnlyNote
{

    struct TaskState
    {
        private string _state;
        private int _duration;
        private Color _stateColor;

        public string state { get => _state;}
        public int duration { get => _duration;}
        public Color stateColor { get => _stateColor; }

        public TaskState(string state, int duration, Color stateColor)
        {
            this._state = state;
            this._duration = duration;
            this._stateColor = stateColor;
        }
    }

    struct TaskSummary
    {
        private int _number;
        private Color _color;

        public int number { get => _number; }
        public Color color { get => _color; }

        public TaskSummary(int number, Color color)
        {
            this._number = number;
            this._color = color;
        }
    }

    class TaskStatus: List<TaskState>
    {
        public TaskStatus()
        {
            this.Add(new TaskState("Dormant", 14, Color.FromRgb(67,0,0)));
            this.Add(new TaskState("Untouched", 7, Color.FromRgb(0,20,20)));
            this.Add(new TaskState("Others", 0, Color.FromRgb(255,255,200)));
        }
    }

    [DataContract]
    class TODOItem
    {
        private string _Id;
        private string _category;
        private string _task;
        private string _notes;
        private DateTime _dateCreated;
        private DateTime _dateModified;
        private string _createdBy;
        private string _modifiedBy;

        [DataMember]
        public string ID { get => _Id; set => _Id = value; }
        [DataMember]
        public string Category { get => _category; set => _category = value; }
        [DataMember]
        public string Task { get => _task; set => _task = value; }
        [DataMember]
        public string Notes { get => _notes; set => _notes = value; }
        [DataMember]
        public DateTime DateCreated { get => _dateCreated; set => _dateCreated = value; }
        [DataMember]
        public DateTime DateModified { get => _dateModified; set => _dateModified = value; }
        [DataMember]
        public string CreatedBy { get => _createdBy; set => _createdBy = value; }
        [DataMember]
        public string ModifiedBy { get => _modifiedBy; set => _modifiedBy = value; }

        public TODOItem() { }

        public TODOItem(string category, string task, string notes)
        {
            this._Id = Guid.NewGuid().ToString();
            this._category = category;
            this._task = task;
            this._notes = notes;
            this._dateCreated = DateTime.Now;
            this._dateModified = DateTime.Now;
            this._createdBy = UserProfile.UserName;
            this._modifiedBy = UserProfile.UserName;
        }

        internal void Copy(TODOItem dest)
        {
            this._category = dest.Category;
            this._task = dest.Task;
            this._notes = dest.Notes;
            this._dateModified = DateTime.Now;
            this._modifiedBy = UserProfile.UserName;
        }

        internal void UpdateTask(string category, string task, string newNotes)
        {
            this._category = category;
            this._task = task;
            this._notes = newNotes;
            this._dateModified = DateTime.Now;
            this._modifiedBy = UserProfile.UserName;
        }
    }

    [CollectionDataContract]
    class TODOItemList : List<TODOItem>
    {
        private string filename = "TODOList.json";
        private string backupFilename = "TODOList.backup";

        public TODOItemList()
        {
//            Populate();
        }

        public bool Populate()
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(TODOItemList));
            FileStream stream = new FileStream(filename, FileMode.OpenOrCreate);
            stream.Position = 0;
            TODOItemList newList = (TODOItemList)ser.ReadObject(stream);
            this.AddRange(newList);
            stream.Close();
            //CreateDummyTODO();

            return true;
        }

        private bool Save()
        {
            File.Delete(backupFilename);
            File.Move(filename, backupFilename);
            
            FileStream stream = new FileStream(filename, FileMode.OpenOrCreate);            

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(TODOItemList));
            ser.WriteObject(stream, this);
            stream.Flush();
            stream.Close();
            return true;
        }

        public void CreateDummyTODO()
        {
            this.Add(new TODOItem("Category 1", "Task 1", "Notes 1"));
            this.Add(new TODOItem("Category 2", "TAsk 2", "Notes 2"));
            this.Add(new TODOItem("Category 2", "TAsk 3", "Notes 3"));
            this.Add(new TODOItem("Category 3", "TAsk 4", "Notes 4"));
            this.Add(new TODOItem("Category 1", "TAsk 5", "Notes 5"));
        }

        public TODOItemList FilterByCategory(string category)
        {
            TODOItemList result = new TODOItemList();

            var filtered =  this.Where(t => t.Category == category);
            result.AddRange(filtered);

            return result;
        }

        public List<string> GetAllCategories()
        {
            List<string> result = new List<string>();

            var filtered = this.Select(t => t.Category).Distinct();
            result.AddRange(filtered);

            return result;
        }

        public void AddNewCategory(string category)
        {
            this.Add(new TODOItem(category, "Dummy Task", string.Empty));
            Save();
        }

        public void AddTask(string category, string newTask)
        {
            this.Add(new TODOItem(category, newTask, string.Empty));
            Save();
        }

        public void AddNotes(string taskID, string newNotes)
        {
            TODOItem task = this.Where(t => t.ID == taskID).FirstOrDefault();
            task.UpdateTask(task.Category, task.Task, newNotes);
            Save();
        }

        internal void ImportTasks(string thisCategory, string newValues)
        {
            newValues = newValues.Replace('\r', ' ');
            string[] tasks = newValues.Split(new char[] { '\n' });
            foreach(string task in tasks)
            {
                this.AddTask(thisCategory, task);
            }
        }

        internal void DeleteTask(TODOItem selectedTaskItem)
        {
            this.Remove(selectedTaskItem);
            Save();
        }

        internal void SwapTask(TODOItem source, TODOItem dest)
        {
            TODOItem temp = new TODOItem();

            temp.Copy(source);
            source.Copy(dest);
            dest.Copy(temp);

            Save();
        }

        internal List<TaskSummary> GetTaskSummary(string category)
        {
            List<TaskSummary> result = new List<TaskSummary>();
            TaskStatus taskStatus = new TaskStatus();

            double lastLimit = 1000;
            foreach (TaskState taskState in taskStatus.OrderByDescending(t => t.duration))
            {
                int count = this.Where(t => ((t.Category == category) && (DateTime.Now - t.DateModified).TotalDays >= taskState.duration) && ((DateTime.Now - t.DateModified).TotalDays < lastLimit)).Count();
                result.Add(new TaskSummary(count, taskState.stateColor));

                lastLimit = taskState.duration;
            }

            return result;
        }

        internal void RenameTask(TODOItem task, string newValue)
        {
            task.UpdateTask(task.Category, newValue, task.Notes);
        }
    }
}
