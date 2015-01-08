using HxJumper.Interface;
using HxJumper.Lib;
using HxJumper.Models.Base;
using HxJumper.Models.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HxJumper.Controllers
{
    public class BaseModelController<Model> : Controller where Model : BaseModel, IEditable<Model>
    {
        public UnitOfWork UW;
        public GenericRepository<Model> GR;
        public string ViewPathStart = "~/Views/";
        public string ViewPath = "BaseModel";
        public string ViewPathBase = "BaseModel";
        public string ViewPathEnd = "/";
        public BaseModelController() 
        {
            UW = new UnitOfWork();
            GR = (GenericRepository<Model>)(typeof(UnitOfWork).GetProperty(typeof(Model).Name + "Repository").GetValue(UW));
        }
        public virtual ActionResult Index(int page = 1, string filter = null)
        {
            ViewBag.RV = new RouteValueDictionary { { "tickTime", DateTime.Now.ToLongTimeString() }, { "returnRoot", "Index" }, { "actionAjax", "Get" }, { "page", page }, { "filter", filter } };
            return View(ViewPathStart + ViewPath + ViewPathEnd + "Index.cshtml");
        }

        public virtual PartialViewResult Get(string returnRoot, string actionAjax = "", int page = 1, string filter = null)
        {
            var results = Common<Model>.GetQuery(UW, filter);

            results = results.OrderByDescending(a => a.Name);

            var rv = new RouteValueDictionary { { "tickTime", DateTime.Now.ToLongTimeString() }, { "returnRoot", returnRoot }, { "actionAjax", actionAjax }, { "page", page }, { "filter", filter } };
            return PartialView(ViewPathStart + ViewPath + ViewPathEnd + "Get.cshtml", Common<Model>.Page(this, rv, results));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create(string returnUrl = "Index") 
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(ViewPathStart + ViewPath + ViewPathEnd + "Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreateSave(Model model, string returnUrl = "Index") 
        {
            if(ModelState.IsValid) {
                try {
                    GR.Insert(model);
                    UW.DbSaveChanges();
                    CommonMsg.RMOk(this, "记录:" + model.Name + "新建成功!");
                    return Redirect(Url.Content(returnUrl));
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException.InnerException.Message.Contains("Cannot insert duplicate key row"))
                    {
                        ModelState.AddModelError(string.Empty, "相同名称的记录已存在,保存失败!");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "新建记录失败!" + e.ToString());
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, "新建记录失败!" + e.ToString());
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(ViewPathStart + ViewPath + ViewPathEnd + "Create.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(int id = 0, string returnUrl = "Index") 
        {
            var result = Common<Model>.GetQuery(UW).Where(a => a.Id == id).SingleOrDefault();

            if(result == null) {
                CommonMsg.RMError(this);
                return Redirect(Url.Content(returnUrl));
            }

            ViewBag.ReturnUrl = returnUrl;

            return View(ViewPathStart + ViewPath + ViewPathEnd + "Edit.cshtml", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditSave(Model model, string returnUrl = "Index") {
            var result = Common<Model>.GetQuery(UW).Where(a => a.Id == model.Id).SingleOrDefault();

            if (result == null)
            {
                CommonMsg.RMError(this);
                return Redirect(Url.Content(returnUrl));
            }
            if (ModelState.IsValid)
            {
                try
                {
                    result.Edit(model);
                    UW.DbSaveChanges();
                    CommonMsg.RMOk(this, "记录:" + result + "保存成功!");
                    return Redirect(Url.Content(returnUrl));
                }
                catch (UpdateException e)
                {
                    if (e.InnerException.Message.Contains("Cannot insert duplicate key row"))
                    {
                        ModelState.AddModelError(string.Empty, "相同名称的记录已存在,保存失败!");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "编辑记录失败!" + e.ToString());
                    }
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException.InnerException.Message.Contains("Cannot insert duplicate key row"))
                    {
                        ModelState.AddModelError(string.Empty, "相同名称的记录已存在,保存失败!");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "新建记录失败!" + e.ToString());
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, "编辑记录失败!" + e.ToString());
                }
            }
            ViewBag.ReturnUrl = returnUrl;

            return View(ViewPathStart + ViewPath + ViewPathEnd + "Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Restore(int id = 0, string returnUrl = "Index")
        {
            //检查记录在权限范围内
            var result = Common<Model>.GetQuery(UW).Where(a => a.Id == id && a.IsDeleted == true).SingleOrDefault();
            if (result == null)
            {
                CommonMsg.RMError(this);
                return Redirect(Url.Content(returnUrl));
            }
            //end 检查记录在权限范围内

            ViewBag.ReturnUrl = returnUrl;

            return View(ViewPathStart + ViewPath + ViewPathEnd + "Restore.cshtml", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult RestoreSave(Model model, string returnUrl = "Index")
        {
            var result = Common<Model>.GetQuery(UW).Where(a => a.Id == model.Id && a.IsDeleted == true).SingleOrDefault();
            if (result == null)
            {
                CommonMsg.RMError(this);
                return Redirect(Url.Content(returnUrl));
            }

            try
            {
                result.IsDeleted = false;
                UW.DbSaveChanges();
                CommonMsg.RMOk(this, "记录:" + result + "恢复成功!");
                return Redirect(Url.Content(returnUrl));
            }
            catch (Exception e)
            {
                CommonMsg.RMOk(this, "记录" + result + "恢复失败!" + e.ToString());
            }
            return Redirect(Url.Content(returnUrl));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Delete(int id = 0, string returnUrl = "Index")
        {
            //检查记录在权限范围内
            var result = Common<Model>.GetQuery(UW).Where(a => a.Id == id).SingleOrDefault();
            if (result == null)
            {
                CommonMsg.RMError(this);
                return Redirect(Url.Content(returnUrl));
            }
            //end 检查记录在权限范围内

            ViewBag.ReturnUrl = returnUrl;

            return View(ViewPathStart + ViewPath + ViewPathEnd + "Delete.cshtml", result);
        }

        //
        // POST: /Model/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteSave(int id, string returnUrl = "Index")
        {
            //检查记录在权限范围内
            var result = Common<Model>.GetQuery(UW).Where(a => a.Id == id).SingleOrDefault();
            if (result == null)
            {
                CommonMsg.RMError(this);
                return Redirect(Url.Content(returnUrl));
            }
            //end 检查记录在权限范围内
            var removeName = result.ToString();
            try
            {
                GR.Delete(result);
                UW.JumperSaveChanges();
                CommonMsg.RMOk(this, "记录:" + removeName + "删除成功!");
                return Redirect(Url.Content(returnUrl));
            }
            catch (Exception e)
            {
                CommonMsg.RMError(this, "记录" + removeName + "删除失败!" + e.ToString());
            }
            return Redirect(Url.Content(returnUrl));
        }

        [ChildActionOnly]
        public virtual PartialViewResult Abstract(int id)
        {
            var result = GR.GetByID(id);
            return PartialView(ViewPathStart + ViewPath + ViewPathEnd + "Abstract.cshtml", result);
        }

        [ChildActionOnly]
        public virtual PartialViewResult AbstractEdit(int id)
        {
            var result = GR.GetByID(id);
            return PartialView(ViewPathStart + ViewPathBase + ViewPathEnd + "AbstractEdit.cshtml", result);
        }
	}
}