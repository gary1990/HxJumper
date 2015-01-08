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
        public int TestResultId { get; set; }
        public virtual TestResult TestResult { get; set; }
        [DisplayName("Mark值")]
        public decimal MarkValue { get; set; }
    }
}