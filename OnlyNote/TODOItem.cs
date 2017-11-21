using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace OnlyNote
{
    class TODOItem
    {
        private string _Id;
        private string _category;
        private string _task;
        private string _notes;

        public string ID { get => _Id; set => _Id = value; }
        public string Category { get => _category; set => _category = value; }
        public string Task { get => _task; set => _task = value; }
        public string Notes { get => _notes; set => _notes = value; }

        public TODOItem() { }
        public TODOItem(string category, string task, string notes)
        {
            this.ID = Guid.NewGuid().ToString();
            this.Category = category;
            this.Task = task;
            this.Notes = notes;
        }
    }

    class TODOItemList : List<TODOItem>
    {
        public bool Populate()
        {
            //            string s1 = @"[TODOItem:{'ID':1,'category':'A','task':'B','notes':'C'},TODOItem:{'ID':2,'category':'D','task':'E','notes':'F'}]";
            //           JavaScriptSerializer sr = new JavaScriptSerializer();
            //            TODOItem todo = sr.Deserialize<TODOItem>(s2);

            CreateDummyTODO();
            //List<TODOItem> newTasks = new List<TODOItem>();
            ////TODO: read JSON in newTasks
            //this.InsertRange(this.IndexOf(this.Last()), newTasks);

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
            this.Add(new TODOItem(category, string.Empty, string.Empty));
        }
    }
}
