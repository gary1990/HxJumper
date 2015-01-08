using HxJumper.Common;
using HxJumper.Models.DAL;
using HxJumper.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using HxJumper.Models;
using Ionic.Zip;
using System.Text;
using System.Transactions;
using System.IO;
using System.Globalization;

namespace HxJumper.Controllers
{
    public class ApiController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult ClientLogin(string jobNumber = null, string passWord = null)
        {
            string userCheckResult = CheckUser(jobNumber, passWord);
            if (userCheckResult != "true") 
            {
                SingleResultXml result = new SingleResultXml()
                {
                    Message = userCheckResult
                };
                return new XmlResult<SingleResultXml>()
                {
                    Data = result
                };
            }

            List<ProductType> productTypes = unitOfWork.ProductTypeRepository.Get(a => a.IsDeleted == false).ToList();
            List<TestClassNumber> testClassNumbers = unitOfWork.TestClassNumberRepository.Get(a => a.IsDeleted == false).ToList();
            List<LineNumber> lineNumbers = unitOfWork.LineNumberRepository.Get(a => a.IsDeleted == false).ToList();
            
            LoginReturnXml loginReturnXml = new LoginReturnXml();
            foreach (var productType in productTypes) 
            {
                ProductTypeXml productTypeXml = new ProductTypeXml 
                {
                    Id = productType.Id,
                    Name = productType.Name
                };
                loginReturnXml.productTypeXmls.productTypeXml.Add(productTypeXml);
            }
            foreach (var testClassNumber in testClassNumbers)
            {
                TestClassNumberXml testClassNumberXml = new TestClassNumberXml
                {
                    Id = testClassNumber.Id,
                    Name = testClassNumber.Name
                };
                loginReturnXml.testClassNumberXmls.testClassNumberXml.Add(testClassNumberXml);
            }
            foreach (var lineNumber in lineNumbers)
            {
                LineNumberXml lineNumberXml = new LineNumberXml
                {
                    Id = lineNumber.Id,
                    Name = lineNumber.Name
                };
                loginReturnXml.lineNumberXmls.lineNumberXml.Add(lineNumberXml);
            }

            return new XmlResult<LoginReturnXml>()
            {
                Data = loginReturnXml
            };
        }
        public ActionResult UploadFile() 
        {
            SingleResultXml result = new SingleResultXml() { Message = "true"};
            HttpPostedFileBase file = Request.Files["file"];
            string fileFullName;
            string fileEx;
            string fileNameWithoutEx;
            string slash = "/";
            string uploadTime = DateTime.Now.ToString("yyyyMMdd");
            string uploadPath = AppDomain.CurrentDomain.BaseDirectory + "/UploadedFolder/" + uploadTime;
            string savePath;
            string saveFolderPath;

            if(file == null || file.ContentLength <= 0)
            {
                result.Message = "file can not be null";
                return new XmlResult<SingleResultXml>() { Data = result};
            }

            fileFullName = System.IO.Path.GetFileName(file.FileName);
            fileEx = System.IO.Path.GetExtension(fileFullName);
            fileNameWithoutEx = System.IO.Path.GetFileNameWithoutExtension(fileFullName);
            if(fileEx.ToLower() != ".zip")
            {
                result.Message = "incorrect file type";
                return new XmlResult<SingleResultXml>() { Data = result};
            }
            var fileNameSplit = fileFullName.Split('_');
            if(fileNameSplit[0] == "" || fileNameSplit[1] == "")
            {
                result.Message = "incorrect file name";
                return new XmlResult<SingleResultXml>() { Data = result};
            }
            if (!System.IO.Directory.Exists(uploadPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(uploadPath);
                }
                catch (Exception /*e*/)
                {
                    result.Message = "can not create upalod directory";
                    return new XmlResult<SingleResultXml>() { Data = result };
                }
            }
            savePath = System.IO.Path.Combine(uploadPath, fileFullName);
            try
            {
                file.SaveAs(savePath);
            }
            catch (Exception /*e*/)
            {
                result.Message = "can not save uploaded file";
                return new XmlResult<SingleResultXml>() { Data = result };
            }

            ZipFile zip = ZipFile.Read(savePath, new ReadOptions { Encoding = Encoding.Default});
            try 
            {
                //unzip file
                zip.AlternateEncoding = Encoding.Default;
                zip.ExtractAll(uploadPath, ExtractExistingFileAction.OverwriteSilently);
            }
            catch(Exception /*e*/)
            {
                result.Message = "can not unzip file";
                zip.Dispose();
                return new XmlResult<SingleResultXml>() { Data = result};
            }
            zip.Dispose();

            using (var scope = new TransactionScope())
            {
                saveFolderPath = uploadPath + slash + fileNameWithoutEx;
                //read general.csv
                string generalCsvPath = saveFolderPath + slash + "General.csv";
                string resultCsvPath = saveFolderPath + slash + "result.csv";
                string imagePath = saveFolderPath + slash + "result.png";
                if (!System.IO.File.Exists(generalCsvPath))
                {
                    result.Message = "can not find general.csv";
                    return new XmlResult<SingleResultXml>() { Data = result };
                }
                if (!System.IO.File.Exists(resultCsvPath))
                {
                    result.Message = "can not find result.csv";
                    return new XmlResult<SingleResultXml>() { Data = result };
                }
                if (!System.IO.File.Exists(imagePath))
                {
                    result.Message = "can not find result.png";
                    return new XmlResult<SingleResultXml>() { Data = result };
                }
                //imagePathAdd
                imagePath = uploadTime + slash + fileNameWithoutEx + slash + "result.png";
                int testResultId = 0;
                StreamReader srGeneralCsv = new StreamReader(generalCsvPath);
                string testTimeStr;
                string productTypeStr;
                string testClassNumberStr;
                string testCode;
                string testResultStr;
                string lineNumberStr;
                string testerJobNumberStr;
                string line = string.Empty;
                string[] lineArr = null;
                int i = 0;
                //only read csv file's second line
                while(i <= 1 )
                {
                    line = srGeneralCsv.ReadLine();
                    i++;
                    if(i == 1)
                    {
                        continue;
                    }
                    if(line == null)
                    {
                        srGeneralCsv.Close();
                        result.Message = "general.csv test result is null";
                        return new XmlResult<SingleResultXml>() { Data = result };
                    }
                    lineArr = line.Split(',');
                    if(lineArr.Count() != 7)
                    {
                        srGeneralCsv.Close();
                        result.Message = "general.csv test result content error";
                        return new XmlResult<SingleResultXml>() { Data = result };
                    }
                    testTimeStr = lineArr[0];
                    productTypeStr = lineArr[1];
                    testClassNumberStr = lineArr[2];
                    testCode = lineArr[3];
                    testResultStr = lineArr[4];
                    lineNumberStr = lineArr[5];
                    testerJobNumberStr = lineArr[6];
                    srGeneralCsv.Close();
                    //convert testTimeStr to testTime
                    DateTime testTime;
                    if(!DateTime.TryParseExact(testTimeStr, "yyyyMMdd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out testTime))
                    {
                        result.Message = "general.csv testtime convert failed";
                        return new XmlResult<SingleResultXml>() { Data = result };
                    }
                    //get ProductType
                    ProductType productTypeDb = unitOfWork.ProductTypeRepository.Get(a => a.Name == productTypeStr).SingleOrDefault();
                    int productTypeId;
                    if (productTypeDb == null)
                    {
                        //insert producttype to db
                        try
                        {
                            ProductType producttypeAdd = new ProductType { Name = productTypeStr };
                            unitOfWork.ProductTypeRepository.Insert(producttypeAdd);
                            unitOfWork.DbSaveChanges();
                            productTypeId = producttypeAdd.Id;
                        }
                        catch (Exception /*e*/)
                        {
                            result.Message = "Insert ProductType failed";
                            return new XmlResult<SingleResultXml>() { Data = result };
                        }
                    }
                    else 
                    {
                        productTypeId = productTypeDb.Id;
                    }
                    //get TestClassNumber
                    TestClassNumber testClassNumberDb = unitOfWork.TestClassNumberRepository.Get(a => a.Name == testClassNumberStr).SingleOrDefault();
                    int testClassNumberId;
                    if (testClassNumberDb == null)
                    {
                        try 
                        {
                            TestClassNumber testClassNumberAdd = new TestClassNumber { Name = testClassNumberStr};
                            unitOfWork.TestClassNumberRepository.Insert(testClassNumberAdd);
                            unitOfWork.DbSaveChanges();
                            testClassNumberId = testClassNumberAdd.Id;
                        }
                        catch(Exception /*e*/)
                        {
                            result.Message = "Insert TestClassNumber failed";
                            return new XmlResult<SingleResultXml>() { Data = result };
                        }
                    }
                    else 
                    {
                        testClassNumberId = testClassNumberDb.Id;
                    }
                    //convert testResult
                    bool testResult = false;
                    if(testResultStr.ToUpper() == "FAIL")
                    {
                        testResult = false;
                    }
                    else if (testResultStr.ToUpper() == "PASS")
                    {
                        testResult = true;
                    }
                    else 
                    {
                        result.Message = "general.csv TestResult column wrong";
                        return new XmlResult<SingleResultXml>() { Data = result };
                    }
                    //get LineNumber
                    int lineNumberId;
                    LineNumber lineNumberDb = unitOfWork.LineNumberRepository.Get(a => a.Name == lineNumberStr && a.IsDeleted == false).SingleOrDefault();
                    if (lineNumberDb == null)
                    {
                        result.Message = "general.csv LineNumber not found";
                        return new XmlResult<SingleResultXml>() { Data = result };
                    }
                    else 
                    {
                        lineNumberId = lineNumberDb.Id;
                    }
                    //get tester(JumperUser)
                    string jumperUserId;
                    JumperUser testerDb = unitOfWork.JumperUserRepository.Get(a => a.JobNumber.ToUpper() == testerJobNumberStr.ToUpper()).SingleOrDefault();
                    if (testerDb == null)
                    {
                        result.Message = "general.csv tester can not find";
                        return new XmlResult<SingleResultXml>() { Data = result };
                    }
                    else 
                    {
                        jumperUserId = testerDb.Id;
                    }
                    TestResult testResultAdd = new TestResult 
                    {
                        TestTime = testTime,
                        ProductTypeId = productTypeId,
                        TestClassNumberId = testClassNumberId,
                        TestCode = testCode,
                        TestImg = imagePath,
                        Result = testResult,
                        LineNumberId = lineNumberId,
                        JumperUserId = jumperUserId
                    };
                    try
                    {
                        unitOfWork.TestResultRepository.Insert(testResultAdd);
                        unitOfWork.DbSaveChanges();
                        testResultId = testResultAdd.Id;
                    }
                    catch (Exception /*e*/)
                    {
                        result.Message = "insert TestResult failed";
                        return new XmlResult<SingleResultXml>() { Data = result };
                    }
                }
                //read result.csv
                StreamReader srResultCsv = new StreamReader(resultCsvPath);
                string lineResult = string.Empty;
                int lineNumResult = 0;
                while((lineResult = srResultCsv.ReadLine()) != null)
                {
                    lineNumResult++;
                    if (lineNumResult == 1)
                    {
                        continue;
                    }
                    string markValueStr = lineResult;
                    decimal markValue;
                    if (!Decimal.TryParse(markValueStr, out markValue))
                    {
                        srResultCsv.Close();
                        result.Message = "can not Parse " + markValue + "  to decimal in result.csv in line " + lineNumResult;
                        return new XmlResult<SingleResultXml>() { Data = result };
                    }
                    TestResultValue restResultValue = new TestResultValue 
                    {
                        TestResultId = testResultId,
                        MarkValue = markValue
                    };
                    try 
                    {
                        unitOfWork.TestResultValueRepository.Insert(restResultValue);
                        unitOfWork.DbSaveChanges();
                    }
                    catch (Exception /*e*/) 
                    {
                        srResultCsv.Close();
                        result.Message = "Insert TestResultValue Failed";
                        return new XmlResult<SingleResultXml>() { Data = result };
                    }
                }
                srResultCsv.Close();
                scope.Complete();
            }

            return new XmlResult<SingleResultXml>() { Data = result};
        }
        private string CheckUser(string jobNumber = null, string passWord = null) 
        {
            string result = "true";
            var s = String.IsNullOrWhiteSpace(jobNumber);
            var s1 = String.IsNullOrWhiteSpace(passWord);
            if (String.IsNullOrWhiteSpace(jobNumber) || String.IsNullOrWhiteSpace(passWord))
            {
                result = "jobnumer or password can not be null.";
            }
            else
            {
                try
                {
                    var user = unitOfWork.JumperUserRepository.Get(a => a.JobNumber.ToUpper() == (jobNumber.ToUpper()) && a.JumperRole.Name == "测试员" && a.IsDeleted == false).SingleOrDefault();
                    if (user == null)
                    {
                        result = "jobnumber or password incorrect.";
                    }
                    else
                    {
                        var userName = user.UserName;
                        user = unitOfWork.JumperUserRepository.context.UserManager.Find(userName, passWord);
                        if (user == null)
                        {
                            result = "jobnumber or password incorrect.";
                        }
                    }
                }
                catch (Exception e)
                {
                    result = e.Message;
                }
            }

            return result;
        }
	}
}