using HxJumper.Lib;
using HxJumper.Models;
using HxJumper.Models.DAL;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HxJumper.Controllers
{
    public class TestResultController : Controller
    {
        List<string> path = new List<string> { };
        private UnitOfWork unitOfWork = new UnitOfWork();
        public string ViewPath1 = "~/Views/";
        public string ViewPath = "TestResult";
        public string ViewPathBase = "TestResult";
        public string ViewPath2 = "/";

        public TestResultController()
        {
            path.Add("质量管理");
            path.Add("测试记录");
            ViewBag.path = path;
            ViewBag.Name = "测试记录";
            ViewBag.Title = "测试记录";
        }
        public ActionResult Index(int page = 1, string filter = null)
        {
            ViewBag.RV = new RouteValueDictionary { { "tickTime", DateTime.Now.ToLongTimeString() }, { "returnRoot", "Index" }, { "actionAjax", "Get" }, { "page", page }, { "filter", filter } };
            return View();
        }

        public ActionResult Get(string returnRoot, string actionAjax = "", int page = 1, string filter = null, bool export = false)
        {
            var results = Common<TestResult>.GetQuery(unitOfWork, filter)
                .Where(a => a.IsLatest == true && a.NotStatistic == false);

            results = results.OrderByDescending(a => a.TestTime);

            var totalResultCount = results.Count();
            if (totalResultCount != 0)
            {
                var passResultCount = results.Where(a => a.Result == true).Count();
                var passPercent = ((decimal)passResultCount / (decimal)totalResultCount) * 100;
                ViewBag.TotalResultCount = totalResultCount;
                ViewBag.PassResultCount = passResultCount;
                ViewBag.PassPercent = passPercent;
            }

            //not export
            if (!export)
            {
                var rv = new RouteValueDictionary { { "tickTime", DateTime.Now.ToLongTimeString() }, { "returnRoot", returnRoot }, { "actionAjax", actionAjax }, { "page", page }, { "filter", filter } };
                return PartialView(ViewPath1 + ViewPath + ViewPath2 + "Get.cshtml", Common<TestResult>.Page(this, rv, results));
            }
            else 
            {
                //initailize excel name
                string excelName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                if (results.Count() > 0)
                {
                    var maxVals = results.Max(a => a.TestResultValues.Count);
                    MemoryStream stream = new MemoryStream();
                    HSSFWorkbook workbook = new HSSFWorkbook();
                    workbook.CreateSheet("sheet1");
                    ISheet worksheet = workbook.GetSheet("sheet1");
                    IRow titleRow = worksheet.CreateRow(0);
                    titleRow.CreateCell(0).SetCellValue("测试时间");
                    titleRow.CreateCell(1).SetCellValue("产品型号");
                    titleRow.CreateCell(2).SetCellValue("产线编号");
                    titleRow.CreateCell(3).SetCellValue("测试班号");
                    titleRow.CreateCell(4).SetCellValue("测试结果");
                    titleRow.CreateCell(5).SetCellValue("序列号");
                    //value start from 7
                    int valTitleStart = 6;
                    for (int i = 1; i <= maxVals; i++ ) 
                    {
                        titleRow.CreateCell(valTitleStart).SetCellValue("Value" + i);
                        valTitleStart++;
                    }
                    titleRow.CreateCell(valTitleStart).SetCellValue("失败原因");
                    //value row start from 1
                    int valueRowStart = 1;
                    //red backgroud style
                    var redBgStyle = workbook.CreateCellStyle();
                    redBgStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;
                    redBgStyle.FillPattern = FillPattern.SolidForeground;
                    //green backgroud style
                    var greenBgStyle = workbook.CreateCellStyle();
                    greenBgStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Green.Index;
                    greenBgStyle.FillPattern = FillPattern.SolidForeground;
                    // font color
                    //HSSFFont font1 = hssfworkbook.CreateFont();
                    //font1.Color = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
                    //style1.SetFont(font1);
                    foreach(var item in results)
                    {
                        IRow valRow = worksheet.CreateRow(valueRowStart);
                        valRow.CreateCell(0).SetCellValue(item.TestTime.ToString());
                        valRow.CreateCell(1).SetCellValue(item.ProductType.Name);
                        valRow.CreateCell(2).SetCellValue(item.LineNumber.Name);
                        valRow.CreateCell(3).SetCellValue(item.TestClassNumber.Name);
                        if (item.Result)
                        {
                            var passCell = valRow.CreateCell(4);
                            passCell.CellStyle = greenBgStyle;
                            passCell.SetCellValue("合格");
                        }
                        else 
                        {
                            var failCell = valRow.CreateCell(4);
                            failCell.CellStyle = redBgStyle;
                            failCell.SetCellValue("不合格");
                        }
                        valRow.CreateCell(5).SetCellValue(item.TestCode);
                        //value row start from 6 in per record
                        int valValRow = 6;
                        foreach(var valItem in item.TestResultValues)
                        {
                            valRow.CreateCell(valValRow).SetCellValue(valItem.MarkValue.ToString());
                            valValRow++;
                        }
                        valRow.CreateCell(valValRow).SetCellValue((item.RemarkMessage == null) ? "" : item.RemarkMessage.Name);
                        valueRowStart++;
                    }
                    if (!workbook.IsWriteProtected)
                    {
                        workbook.Write(stream);
                    }
                    return File(stream.ToArray(), "application/vnd.ms-excel", excelName);
                }
                else 
                {
                    MemoryStream stream = new MemoryStream();
                    HSSFWorkbook workbook = new HSSFWorkbook();
                    workbook.CreateSheet("sheet1");
                    ISheet worksheet = workbook.GetSheet("sheet1");
                    IRow firstRow = worksheet.CreateRow(0);
                    ICell firstCell = firstRow.CreateCell(0);
                    firstCell.SetCellValue("查询记录为空");
                    if (!workbook.IsWriteProtected)
                    {
                        workbook.Write(stream);
                    }
                    return File(stream.ToArray(), "application/vnd.ms-excel", excelName);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Details(int Id = 0, string returnUrl = "Index") 
        {
            var result = unitOfWork.TestResultRepository.Get(a => a.Id == Id, null, "TestResultValues,JumperUser,LineNumber,TestClassNumber,ProductType").SingleOrDefault();
            if (result == null)
            {
                CommonMsg.RMError(this);
                return Redirect(Url.Content(returnUrl));
            }

            ViewBag.ReturnUrl = returnUrl;

            return View(ViewPath1 + ViewPath + ViewPath2 + "Details.cshtml", result);
        }
	}
}