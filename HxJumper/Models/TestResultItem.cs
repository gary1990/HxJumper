using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HxJumper.Models
{
    public class TestResultItem
    {
        public TestResultItem() 
        {
            this.TestResultItemResult = false;
            TestResultValues = new List<TestResultValue> { };
        }
        public int Id { get; set; }
        [DisplayName("Label Tiltle")]
        public int TestItemId { get; set; }
        public int TestResultId { get; set; }
        public bool TestResultItemResult { get; set; }//false is not pass, 0; true is pass, 1;
        public virtual TestItem TestItem { get; set; }
        public virtual TestResult TestResult { get; set; }
        public virtual ICollection<TestResultValue> TestResultValues { get; set; }
    }
}