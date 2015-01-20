using HxJumper.Models.Constant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HxJumper.Models
{
    public class TestResultValue
    {
        public int Id { get; set; }
        public int TestResultItemId { get; set; }
        public virtual TestResultItem TestResultItem { get; set; }
        [DisplayName("Trace Number")]
        public int TraceNumber { get; set; }
        [DisplayName("Mark Number")]
        public int MarkNumber { get; set; }
        [DisplayName("Channel值")]
        public decimal XValue { get; set; }
        public Unit XValueUnit { get; set; }
        [DisplayName("Mark值")]
        public decimal MarkValue { get; set; }
        public string Paremeter { get; set; }
        public string Formart { get; set; }
    }
}