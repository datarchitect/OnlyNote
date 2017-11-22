using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;

namespace OnlyNote
{
    [DataContract]
    class TODOItem
    {
        private string _Id;
        private string _category;
        private string _task;
        private string _notes;

        [DataMember]
        public string ID { get => _Id; set => _Id = value; }
        [DataMember]
        public string Category { get => _category; set => _category = value; }
        [DataMember]
        public string Task { get => _task; set => _task = value; }
        [DataMember]
        public string Notes { get => _notes; set => _notes = value; }

        public TODOItem() { }
        public TODOItem(string category, string task, string notes)
        {
            this.ID = Guid.NewGuid().ToString();
            this.Category = category;
            this.Task = task;
            this.Notes = notes;
        }

        internal void Copy(TODOItem dest)
        {
            this.Category = dest.Category;
            this.Task = dest.Task;
            this.Notes = dest.Notes;
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

        public void AddNotes(string taskID, string Notes)
        {
            TODOItem task = this.Where(t => t.ID == taskID).FirstOrDefault();
            task.Notes = Notes;
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
    }
}
