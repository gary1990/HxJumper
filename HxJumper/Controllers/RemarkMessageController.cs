using HxJumper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HxJumper.Controllers
{
    public class RemarkMessageController : BaseModelController<RemarkMessage>
    {
        List<string> path = new List<string>();
        public RemarkMessageController() 
        {
            path.Add("测试管理");
            path.Add("失败原因");
            ViewBag.path = path;
            ViewBag.Name = "失败原因";
            ViewBag.Title = "失败原因";
            ViewBag.Controller = "RemarkMessage";
        }
	}
}