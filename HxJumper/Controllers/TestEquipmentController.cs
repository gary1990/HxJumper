using HxJumper.Lib;
using HxJumper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HxJumper.Controllers
{
    public class TestEquipmentController : BaseModelController<TestEquipment>
    {
        List<string> path = new List<string>();
        
        public TestEquipmentController() 
        {
            path.Add("测试管理");
            path.Add("测试设备");
            ViewBag.path = path;
            ViewBag.Name = "测试设备";
            ViewBag.Title = "测试设备";
            ViewBag.Controller = "TestEquipment";

            this.ViewPathStart = "~/Views/";
            this.ViewPath = "TestEquipment";
            this.ViewPathBase = "TestEquipment";
            this.ViewPathEnd = "/";
        }

        public override PartialViewResult Get(string returnRoot, string actionAjax = "", int page = 1, string filter = null)
        {
            var results = Common<TestEquipment>.GetQuery(UW, filter);
            results = results.Where(a => a.isVna == true);
            results = results.OrderByDescending(a => a.Id);

            var rv = new RouteValueDictionary { { "tickTime", DateTime.Now.ToLongTimeString() }, { "returnRoot", returnRoot }, { "actionAjax", actionAjax }, { "page", page }, { "filter", filter } };
            return PartialView(ViewPathStart + ViewPath + ViewPathEnd + "Get.cshtml", Common<TestEquipment>.Page(this, rv, results));
        }
	}
}