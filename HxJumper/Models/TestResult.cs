using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HxJumper.Models
{
    public class TestResult
    {
        public TestResult() 
        {
            Result = false;
            IsLatest = true;
            NotStatistic = false;
            TestResultItems = new List<TestResultItem>();
        }
        public int Id { get; set; }
        [Required]
        [DisplayName("测试时间")]
        public DateTime TestTime { get; set; }
        [DisplayName("产品类型")]
        public int ProductTypeId { get; set; }
        [DisplayName("测试班号")]
        public int TestClassNumberId { get; set; }
        [DisplayName("序列号")]
        public string TestCode { get; set; }
        [DisplayName("失败原因")]
        public int? RemarkMessageId { get; set; }
        [DisplayName("测试图像")]
        public string TestImg { get; set; }
        [DisplayName("测试结果")]
        public bool Result { get; set; }//default is false, 0 in db ,pass is true, 1 in db
        [DisplayName("产线编号")]
        public int LineNumberId { get; set; }
        [DisplayName("测试员")]
        public string JumperUserId { get; set; }
        [DisplayName("最新记录")]//equals to is not retest
        public bool IsLatest { get; set; }
        [DisplayName("不统计")]//default false, 0; if true, 1, do not static
        public bool NotStatistic { get; set; }
        public virtual JumperUser JumperUser { get; set; }
        public virtual LineNumber LineNumber { get; set; }
        public virtual RemarkMessage RemarkMessage { get; set; }
        public virtual TestClassNumber TestClassNumber { get; set; }
        public virtual ProductType ProductType { get; set; }
        public virtual ICollection<TestResultItem> TestResultItems { get; set; }
    }
}