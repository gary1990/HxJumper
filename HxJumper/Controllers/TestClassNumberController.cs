using HxJumper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HxJumper.Controllers
{
    public class TestClassNumberController : BaseModelController<TestClassNumber>
    {
        List<string> path = new List<string>();
        public TestClassNumberController() 
        {
            path.Add("测试管理");
            path.Add("测试班号");
            ViewBag.path = path;
            ViewBag.Name = "测试班号";
            ViewBag.Title = "测试班号";
            ViewBag.Controller = "TestClassNumber";
        }
	}
}