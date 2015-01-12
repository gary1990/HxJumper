using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using HxJumper.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HxJumper.Controllers
{
    public class QualityManageHomeController : Controller
    {
        List<string> path = new List<string>();
        private UnitOfWork unitOfWork = new UnitOfWork();
        public QualityManageHomeController() 
        {
            path.Add("质量管理");
        }
        public ActionResult Index()
        {
            ViewBag.path = path;

            var todayStart = DateTime.Now.Date;
            var todayEnd = todayStart.AddDays(1);

            var totalRecord = unitOfWork.TestResultRepository
                .Get(a => a.TestTime <= todayEnd && a.TestTime >= todayStart && a.NotStatistic == false && a.IsLatest == true)
                .GroupBy(a => a.TestTime.Hour).Select(b => new { hour = b.Key, count = b.Count() }).ToList();
            var passRecord = unitOfWork.TestResultRepository
                .Get(a => a.TestTime <= todayEnd && a.TestTime >= todayStart && a.NotStatistic == false && a.IsLatest == true && a.Result == true)
                .GroupBy(a => a.TestTime.Hour).Select(b => new { hour = b.Key, count = b.Count() }).ToList();
            
            //total record
            var totalNum = new object[24];
            //passPercent record
            var passPercent = new object[24];
            for (int i = 0; i < 24; i++)
            {
                totalNum[i] = 0;
                passPercent[i] = 0;
            }
            foreach (var totalItem in totalRecord)
            {
                totalNum[totalItem.hour] = totalItem.count;
            }

            foreach (var passItem in passRecord)
            {
                var curTotal = totalNum[passItem.hour];
                passPercent[passItem.hour] = (Convert.ToDecimal(passItem.count) / Convert.ToDecimal(curTotal)) * 100;
            }

            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts("chart")
            .SetTitle(new Title { Text = "今日合格率" })
            .SetXAxis(new XAxis
            {
                Categories = new[] { "00", "01", "02", "03", "04", "05", "06", "07",
                                     "08", "09", "10", "11", "12", "13", "14", "15",
                                     "16", "17", "18", "19", "20", "21", "22", "23"
                                    }
            })
            .SetYAxis(new YAxis
            {
                Title = new YAxisTitle { Text = "合格率" },
                Min = Number.GetNumber(0),
                Max = Number.GetNumber(100),
            })
            .SetSeries(new Series
            {
                Name = "时间(24小时)",
                Type = ChartTypes.Column,
                Data = new Data(passPercent)
            })
            .SetTooltip(new Tooltip
            {
                Formatter = @"function(){return '<b>合格率</b>:' + this.y + '%<br/>' + '<b>时间点</b>: ' + (this.x) + '点'}"
            });

            DotNet.Highcharts.Highcharts chartTotal = new DotNet.Highcharts.Highcharts("chartTotal")
            .SetTitle(new Title { Text = "今日产量" })
            .SetXAxis(new XAxis
            {
                Categories = new[] { "00", "01", "02", "03", "04", "05", "06", "07",
                                     "08", "09", "10", "11", "12", "13", "14", "15",
                                     "16", "17", "18", "19", "20", "21", "22", "23"
                                    }
            })
            .SetYAxis(new YAxis
            {
                Title = new YAxisTitle { Text = "产量" }
            })
            .SetSeries(new Series
            {
                Name = "时间(24小时)",
                Type = ChartTypes.Column,
                Data = new Data(totalNum)
            })
            .SetTooltip(new Tooltip
            {
                Formatter = @"function(){return '<b>产量</b>:' + this.y + '<br/>' + '<b>时间点</b>: ' + (this.x) + '点'}"
            });
            List<Highcharts> chartList = new List<Highcharts> { };
            chartList.Add(chartTotal);
            chartList.Add(chart);
            return View(chartList);
        }
	}
}