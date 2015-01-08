﻿using HxJumper.Models.Constant;
using HxJumper.Models.DAL;
using HxJumper.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HxJumper.Lib
{
    public class Common<Model> where Model : class
    {
        public static IQueryable<Model> GetQuery(UnitOfWork db, string filter = null)
        {
            IQueryable<Model> result;

            var rep = (GenericRepository<Model>)(typeof(UnitOfWork).GetProperty(typeof(Model).Name + "Repository").GetValue(db));

            result = rep.Get();

            //filter
            if (filter != null)
            {
                Dictionary<string, string> filterDic = new Dictionary<string, string>();
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    //reFormart filter if contains Time filter
                    filter = TimeFormat.TimeFilterConvert("TestTime", "TestTimeStartHour", ">=", filter);
                    filter = TimeFormat.TimeFilterConvert("TestTime", "TestTimeStopHour", "<=", filter);
                    var conditions = filter.Substring(0, filter.Length - 1).Split(';');
                    foreach (var item in conditions)
                    {
                        var tmp = item.Split(':');
                        if (!string.IsNullOrWhiteSpace(tmp[1]))
                        {
                            filterDic.Add(tmp[0], tmp[1]);
                        }
                    }
                }

                foreach (var item in filterDic)
                {
                    result = Common<Model>.DynamicFilter(result, item.Key, item.Value);
                }
            }
            //end filter
            return result;
        }

        public static List<Model> GetList(string filter = null)
        {
            using (var db = new UnitOfWork())
            {
                return GetQuery(db, filter).ToList();
            }
        }

        public static IQueryable<Model> DynamicContains<TProperty>(
            IQueryable<Model> query,
            string property,
            IEnumerable<TProperty> items)
        {
            var pe = Expression.Parameter(typeof(Model));
            var me = Expression.Property(pe, property);

            var nullExpression = Expression.Constant(null);
            var call1 = Expression.Equal(me, nullExpression);

            var ce = Expression.Constant(items);
            var call2 = Expression.Call(typeof(Enumerable), "Contains", new[] { me.Type }, ce, me);
            var call = Expression.OrElse(call1, call2);

            var lambda = Expression.Lambda<Func<Model, bool>>(call, pe);
            return query.Where(lambda);
        }

        public static IQueryable<Model> DynamicFilter(
            IQueryable<Model> query,
            string property,
            string target)
        {
            var pe = Expression.Parameter(typeof(Model), "pe");

            var tmp = property.Split('@');

            if (tmp[1] == "%")//like match
            {
                var words = tmp[0].Split('|');

                MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                var wordsExps = new List<Expression>();
                foreach (var item in words)
                {
                    var propertyNames = item.Split('.');
                    Expression left = pe;

                    foreach (var prop in propertyNames)
                    {
                        left = Expression.PropertyOrField(left, prop);
                    }
                    Expression upperLeft = Expression.Call(left, typeof(string).GetMethod("ToUpper", System.Type.EmptyTypes));

                    var upperRight = Expression.Constant(target.ToUpper());

                    wordsExps.Add(Expression.Call(upperLeft, method, upperRight));
                }

                Expression finalExp = wordsExps[0];
                for (int i = 1; i < wordsExps.Count; i++)
                {
                    finalExp = Expression.OrElse(finalExp, wordsExps[i]);
                }
                var lambda = Expression.Lambda<Func<Model, bool>>(finalExp, pe);
                return query.Where(lambda);
            }
            else
            {
                var propertyNames = tmp[0].Split('.');

                Expression left = pe;
                Expression right = null;
                foreach (var prop in propertyNames)
                {
                    left = Expression.PropertyOrField(left, prop);

                    var type = left.Type.Name;
                    var typeFullName = left.Type.FullName;

                    if (type == "Int32")
                    {
                        right = Expression.Constant(Int32.Parse(target));
                    }
                    else if (type == "DateTime")
                    {
                        DateTime targetParse;
                        if (!DateTime.TryParseExact(target, "yyyyMMdd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out targetParse))
                        {
                            targetParse = DateTime.Now;
                        }
                        right = Expression.Constant(targetParse);
                    }
                    else if (type == "Boolean")
                    {
                        Boolean targetBoolean = Convert.ToBoolean(target);
                        right = Expression.Constant(targetBoolean);
                    }
                    else if (type == "Nullable`1" && typeFullName.Contains("System.Int32"))
                    {
                        right = Expression.Constant(Int32.Parse(target));
                    }
                    else
                    {
                        right = Expression.Constant(target);
                    }
                }

                BinaryExpression call = null;
                if (tmp[1] == "=")
                {
                    call = MyEqual(left, right);
                }
                else if (tmp[1] == ">")
                {
                    call = Expression.GreaterThan(left, right);
                }
                else if (tmp[1] == ">=")
                {
                    call = Expression.GreaterThanOrEqual(left, right);
                }
                else if (tmp[1] == "<")
                {
                    call = Expression.LessThan(left, right);
                }
                else if (tmp[1] == "<=")
                {
                    call = Expression.LessThanOrEqual(left, right);
                }

                var lambda = Expression.Lambda<Func<Model, bool>>(call, pe);
                return query.Where(lambda);
            }
        }

        static BinaryExpression MyEqual(Expression e1, Expression e2)
        {
            if (IsNullableType(e1.Type) && !IsNullableType(e2.Type))
                e2 = Expression.Convert(e2, e1.Type);
            else if (!IsNullableType(e1.Type) && IsNullableType(e2.Type))
                e1 = Expression.Convert(e1, e2.Type);
            return Expression.Equal(e1, e2);
        }

        static bool IsNullableType(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static IQueryable<Model> Page(Controller c, RouteValueDictionary rv, IQueryable<Model> q, int size = 20)
        {
            var tmpPage = rv.Where(a => a.Key == "page").Select(a => a.Value).SingleOrDefault();
            int page = int.Parse(tmpPage.ToString());
            var tmpTotalPage = (int)Math.Ceiling(((decimal)(q.Count()) / size));
            page = page > tmpTotalPage ? tmpTotalPage : page;
            page = page == 0 ? 1 : page;
            rv.Add("totalPage", tmpTotalPage);
            rv["page"] = page;

            c.ViewBag.RV = rv;
            return q.Skip(((tmpTotalPage > 0 ? page : 1) - 1) * size).Take(size);
        }
    }

    public class CommonMsg
    {
        //带消息提示的返回索引页面
        public static void RMError(Controller controller, string msg = "没有找到对应记录")
        {
            Msg message = new Msg { MsgType = MsgType.ERROR, Content = msg };
            controller.TempData["msg"] = message;
        }

        public static void RMOk(Controller controller, string msg = "操作成功!")
        {
            Msg message = new Msg { MsgType = MsgType.OK, Content = msg };
            controller.TempData["msg"] = message;
        }

        public static void RMWarn(Controller controller, string msg)
        {
            Msg message = new Msg { MsgType = MsgType.WARN, Content = msg };
            controller.TempData["msg"] = message;
        }
    }

    public class TimeFormat
    {
        public static string TimeFilterConvert(string timeDateMark, string timeHourMark, string equalMark, string filterStr)
        {
            //Target Time String yyyyMMdd HHmmss formarted
            var timeTarget = "";
            //Matched Time Date String
            var regDateStr = "";
            //Matched Time Hour String
            var regHourStr = "";
            //match Time Date
            Regex regDate = new Regex(@"(" + timeDateMark + @"\@" + equalMark + @"\:)([-\d]+)(;)");
            //match Time Hour
            Regex regHour = new Regex(@"(" + timeHourMark + @"\@" + equalMark + @"\:)([\d]+)(;)");
            //Time Date Match result
            bool regDateStartMatch = regDate.IsMatch(filterStr);
            //Time Hour Match result
            bool regHourStartMatch = regHour.IsMatch(filterStr);
            if (regDateStartMatch)
            {
                regDateStr = regDate.Match(filterStr).Value;
                //replace matched Time Date with space
                filterStr = regDate.Replace(filterStr, "");
                if (regHourStartMatch)
                {
                    regHourStr = regHour.Match(filterStr).Value;
                    //replace matched Time Hour with space
                    filterStr = regHour.Replace(filterStr, "");
                    //colon position in regHourStr
                    int colonPostion = regHourStr.IndexOf(':');
                    //generate Target Time String yyyyMMdd HHmmss formarted
                    timeTarget = regDateStr.Substring(0, regDateStr.Length - 1).Replace("-", "") + " " + regHourStr.Substring(colonPostion + 1, regHourStr.Length - colonPostion - 2) + "0000" + ";";
                }
                else
                {
                    //generate Target Time String yyyyMMdd HHmmss formarted
                    timeTarget = regDateStr.Substring(0, regDateStr.Length - 1).Replace("-", "") + " " + "000000" + ";";
                }
            }
            else
            {
                if (regHourStartMatch)
                {
                    regHourStr = regHour.Match(filterStr).Value;
                    //replace matched Time Hour with space
                    filterStr = regHour.Replace(filterStr, "");
                    //get current date String yyyyMMdd formated
                    string currentDateStr = DateTime.Now.ToString("yyyyMMdd");
                    //colon position in regHourStartStr
                    int colonPostion = regHourStr.IndexOf(':');
                    //generate Target Time String yyyyMMdd HHmmss formarted
                    timeTarget = timeDateMark + @"@" + equalMark + ":" + currentDateStr + " " + regHourStr.Substring(colonPostion + 1, regHourStr.Length - colonPostion - 2) + "0000" + ";";
                }
                else
                {
                    //do noting
                }
            }
            //add Target Time String to filter String
            filterStr += timeTarget;
            return filterStr;
        }
    }
}