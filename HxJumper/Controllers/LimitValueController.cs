using HxJumper.Interface;
using HxJumper.Lib;
using HxJumper.Models;
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
    public class LimitValueController: Controller
    {
        List<string> path = new List<string> { };
        private UnitOfWork unitOfWork = new UnitOfWork();
        public string ViewPath1 = "~/Views/";
        public string ViewPath = "LimitValue";
        public string ViewPathBase = "LimitValue";
        public string ViewPath2 = "/";
        public LimitValueController() 
        {
            path.Add("测试管理");
            path.Add("极限值");
            ViewBag.path = path;
            ViewBag.Name = "极限值";
            ViewBag.Title = "极限值";
        }
        public virtual ActionResult Index(int page = 1, string filter = null)
        {
            ViewBag.RV = new RouteValueDictionary { { "tickTime", DateTime.Now.ToLongTimeString() }, { "returnRoot", "Index" }, { "actionAjax", "Get" }, { "page", page }, { "filter", filter } };
            return View();
        }

        public virtual PartialViewResult Get(string returnRoot, string actionAjax = "", int page = 1, string filter = null)
        {
            var results = Common<LimitValue>.GetQuery(unitOfWork, filter);

            results = results.OrderByDescending(a => a.Id);

            var rv = new RouteValueDictionary { { "tickTime", DateTime.Now.ToLongTimeString() }, { "returnRoot", returnRoot }, { "actionAjax", actionAjax }, { "page", page }, { "filter", filter } };
            return PartialView(ViewPath1 + ViewPath + ViewPath2 + "Get.cshtml", Common<LimitValue>.Page(this, rv, results));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create(string returnUrl = "Index") 
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(ViewPath1 + ViewPath + ViewPath2 + "Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreateSave(LimitValue model, string returnUrl = "Index") 
        {
            if(ModelState.IsValid) {
                try {
                    unitOfWork.LimitValueRepository.Insert(model);
                    unitOfWork.DbSaveChanges();
                    CommonMsg.RMOk(this, "记录新建成功!");
                    return Redirect(Url.Content(returnUrl));
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException.InnerException.Message.Contains("Cannot insert duplicate key row"))
                    {
                        ModelState.AddModelError(string.Empty, "相同的记录已存在,保存失败!");
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
            return View(ViewPath1 + ViewPath + ViewPath2 + "Create.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(int id = 0, string returnUrl = "Index") 
        {
            var result = Common<LimitValue>.GetQuery(unitOfWork).Where(a => a.Id == id).SingleOrDefault();

            if(result == null) {
                CommonMsg.RMError(this);
                return Redirect(Url.Content(returnUrl));
            }

            ViewBag.ReturnUrl = returnUrl;

            return View(ViewPath1 + ViewPath + ViewPath2 + "Edit.cshtml", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditSave(LimitValue model, string returnUrl = "Index") {
            var result = Common<LimitValue>.GetQuery(unitOfWork).Where(a => a.Id == model.Id).SingleOrDefault();

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
                    unitOfWork.DbSaveChanges();
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

            return View(ViewPath1 + ViewPath + ViewPath2 + "Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Restore(int id = 0, string returnUrl = "Index")
        {
            //检查记录在权限范围内
            var result = Common<LimitValue>.GetQuery(unitOfWork).Where(a => a.Id == id && a.IsDeleted == true).SingleOrDefault();
            if (result == null)
            {
                CommonMsg.RMError(this);
                return Redirect(Url.Content(returnUrl));
            }
            //end 检查记录在权限范围内

            ViewBag.ReturnUrl = returnUrl;

            return View(ViewPath1 + ViewPath + ViewPath2 + "Restore.cshtml", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult RestoreSave(LimitValue model, string returnUrl = "Index")
        {
            var result = Common<LimitValue>.GetQuery(unitOfWork).Where(a => a.Id == model.Id && a.IsDeleted == true).SingleOrDefault();
            if (result == null)
            {
                CommonMsg.RMError(this);
                return Redirect(Url.Content(returnUrl));
            }

            try
            {
                result.IsDeleted = false;
                unitOfWork.DbSaveChanges();
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
            var result = Common<LimitValue>.GetQuery(unitOfWork).Where(a => a.Id == id).SingleOrDefault();
            if (result == null)
            {
                CommonMsg.RMError(this);
                return Redirect(Url.Content(returnUrl));
            }
            //end 检查记录在权限范围内

            ViewBag.ReturnUrl = returnUrl;

            return View(ViewPath1 + ViewPath + ViewPath2 + "Delete.cshtml", result);
        }

        //
        // POST: /Model/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteSave(int id, string returnUrl = "Index")
        {
            //检查记录在权限范围内
            var result = Common<LimitValue>.GetQuery(unitOfWork).Where(a => a.Id == id).SingleOrDefault();
            if (result == null)
            {
                CommonMsg.RMError(this);
                return Redirect(Url.Content(returnUrl));
            }
            //end 检查记录在权限范围内
            var removeName = result.ToString();
            try
            {
                unitOfWork.LimitValueRepository.Delete(result);
                unitOfWork.JumperSaveChanges();
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
            var result = unitOfWork.LimitValueRepository.GetByID(id);
            return PartialView(ViewPath1 + ViewPath + ViewPath2 + "Abstract.cshtml", result);
        }

        [ChildActionOnly]
        public virtual PartialViewResult AbstractEdit(int id)
        {
            var result = unitOfWork.LimitValueRepository.GetByID(id);
            return PartialView(ViewPath1 + ViewPathBase + ViewPath2 + "AbstractEdit.cshtml", result);
        }
	}
}