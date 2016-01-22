using HxJumper.Models.Constant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HxJumper.Models
{
    public class TestResultPim
    {
        public TestResultPim()
        {
            TestResult = false;
            IsLatest = true;
            Carriers = new List<Carrier> { };
            TestResultPimPoints = new List<TestResultPimPoint> { };
            TestImages = new List<TestImage> { };
        }
        public int Id { get; set; }
        [DisplayName("测试时间")]
        public DateTime TestTime { get; set; }
        [DisplayName("序列号")]
        public string SerialNumber { get; set; }
        [DisplayName("测试员")]
        public string JumperUserId { get; set; }
        public virtual JumperUser JumperUser { get; set; }
        [DisplayName("测试设备")]
        public int? TestEquipmentId { get; set; }
        public virtual TestEquipment TestEquipment { get; set; }
        [DisplayName("阶数")]
        public int ImOrderId { get; set; }
        public virtual ImOrder ImOrder { get; set; }
        [DisplayName("测试方式")]
        public TestMeans TestMeans { get; set; }
        [DisplayName("描述")]
        public string TestDescription { get; set; }
        [DisplayName("测试结果")]
        public bool TestResult { get; set; }
        public bool IsLatest { get; set; }
        [DisplayName("极限值")]
        public decimal LimitLine { get; set; }

        public virtual ICollection<Carrier> Carriers { get; set; }
        public virtual ICollection<TestResultPimPoint> TestResultPimPoints { get; set; }
        public virtual ICollection<TestImage> TestImages { get; set; }
    }
}