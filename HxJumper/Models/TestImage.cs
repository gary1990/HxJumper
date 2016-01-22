using HxJumper.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HxJumper.Models
{
    public class TestImage : IEditable<TestImage>
    {
        public int Id { get; set; }
        public string path { get; set; }
        public int TestResultPimId { get; set; }
        public virtual TestResultPim TestResultPim { get; set; }

        public void Edit(TestImage model)
        {
            this.path = model.path;
        }
    }
}