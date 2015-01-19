﻿using HxJumper.Models.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace System.Web.Mvc.Html
{
    public static class HtmlHelperExtensions
    {
        public static object IndexPageInit(this HtmlHelper htmlHelper)
        {
            htmlHelper.ViewBag.Action = (((RouteValueDictionary)(htmlHelper.ViewBag.RV)))["actionAjax"].ToString();
            htmlHelper.ViewBag.ReturnRoot = (((RouteValueDictionary)(htmlHelper.ViewBag.RV))["returnRoot"]).ToString();
            var filter = ((RouteValueDictionary)(htmlHelper.ViewBag.RV))["filter"];
            if (filter != null && filter != "")
            {
                var filterStr = filter.ToString();
                var conditions = filterStr.Substring(0, filterStr.Length - 1).Split(';');
                foreach (var item in conditions)
                {
                    var tmp = item.Split(':');
                    htmlHelper.ViewData.Add(tmp[0], tmp[1]);
                }
            }

            var wvp = (WebViewPage)htmlHelper.ViewDataContainer;

            htmlHelper.ViewBag.AjaxOpts = new AjaxOptions
            {
                UpdateTargetId = "AjaxBody",
                Url = wvp.Url.Action(htmlHelper.ViewBag.Action),
                OnSuccess = "syncSuccess",
                OnFailure = "synFail"
            };
            return null;
        }

        public static List<SelectListItem> GetHoursList(this HtmlHelper htmlHelper)
        {
            List<SelectListItem> hours = new List<SelectListItem>();
            hours.Add(new SelectListItem { Text = "00", Value = "00" });
            hours.Add(new SelectListItem { Text = "01", Value = "01" });
            hours.Add(new SelectListItem { Text = "02", Value = "02" });
            hours.Add(new SelectListItem { Text = "03", Value = "03" });
            hours.Add(new SelectListItem { Text = "04", Value = "04" });
            hours.Add(new SelectListItem { Text = "05", Value = "05" });
            hours.Add(new SelectListItem { Text = "06", Value = "06" });
            hours.Add(new SelectListItem { Text = "07", Value = "07" });
            hours.Add(new SelectListItem { Text = "08", Value = "08" });
            hours.Add(new SelectListItem { Text = "09", Value = "09" });
            for (int i = 10; i <= 23; i++)
            {
                hours.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
            return hours;
        }

        public static string GetUnitStr(this HtmlHelper htmlHelper, Unit unit) 
        {
            var unitStr = "";
            switch (unit)
            {
                case Unit.G:
                    unitStr = "G";
                    break;
                case Unit.M:
                    unitStr = "M";
                    break;
                case Unit.k:
                    unitStr = "k";
                    break;
                case Unit.h:
                    unitStr = "百";
                    break;
                case Unit.da:
                    unitStr = "";
                    break;
                case Unit.d:
                    unitStr = "10^1";
                    break;
                case Unit.c:
                    unitStr = "10^-2";
                    break;
                case Unit.m:
                    unitStr = "m";
                    break;
                case Unit.μ:
                    unitStr = "μ";
                    break;
                case Unit.n:
                    unitStr = "n";
                    break;
                default:
                    break;
            }
            return unitStr;
        }
        public static string GetImUnitStr(this HtmlHelper htmlHelper, ImUnit imUnit)
        {
            var imUnitStr = "";
            switch (imUnit)
            {
                case ImUnit.dBc:
                    imUnitStr = "dBc";
                    break;
                case ImUnit.dBm:
                    imUnitStr = "dBm";
                    break;
                default:
                    break;
            }
            return imUnitStr;
        }

        public static string GetMeasStr(this HtmlHelper htmlHelper, TestMeans testMeas)
        {
            var testMeasStr = "";
            switch (testMeas)
            {
                case TestMeans.Sweep:
                    testMeasStr = "SWEEP";
                    break;
                case TestMeans.Single:
                    testMeasStr = "SINGLE";
                    break;
                default:
                    break;
            }
            return testMeasStr;
        }
    }

    public static class AuthorizeActionLinkExtention
    {
        public static MvcHtmlString AuthorizeActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName)
        {
            if (HasActionPermission(helper, actionName, controllerName))
                return helper.ActionLink(linkText, actionName, controllerName);

            return MvcHtmlString.Empty;
        }

        static bool HasActionPermission(this HtmlHelper htmlHelper, string actionName, string controllerName = null)
        {
            controllerName = string.IsNullOrEmpty(controllerName) ? htmlHelper.ViewContext.Controller.GetType().Name : controllerName;
            if (controllerName.IndexOf("Controller") > 0)
            {
                controllerName = controllerName.Substring(0, controllerName.IndexOf("Controller"));
            }
            string controllerActionName = controllerName + "_" + actionName;
            var item = HttpContext.Current.Session["PermissionList"];
            //return (((List<string>)HttpContext.Current.Session["PermissionList"]).Contains(controllerActionName));
            return true;//current no permission limit in system, return true
        }
    }
}