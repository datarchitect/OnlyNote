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
    }

    [CollectionDataContract]
    class TODOItemList : List<TODOItem>
    {
        private string filename = "TODOList.json";

        public TODOItemList()
        {
            Populate();
        }

        private bool Populate()
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(TODOItemList));
            FileStream stream = new FileStream("TODOList.json", FileMode.OpenOrCreate);
            stream.Position = 0;
            TODOItemList newList = (TODOItemList)ser.ReadObject(stream);
            this.AddRange(newList);

            //CreateDummyTODO();

            return true;
        }

        private bool Save()
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(TODOItemList));
            FileStream stream = new FileStream("TODOList.json", FileMode.OpenOrCreate);
            ser.WriteObject(stream, this);

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
    }
}
