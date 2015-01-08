using HxJumper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HxJumper.Controllers
{
    public class LineNumberController : BaseModelController<LineNumber>
    {
        List<string> path = new List<string>();
        public LineNumberController() 
        {
            path.Add("测试管理");
            path.Add("产线编号");
            ViewBag.path = path;
            ViewBag.Name = "产线编号";
            ViewBag.Title = "产线编号";
            ViewBag.Controller = "LineNumber";
        }
	}
}