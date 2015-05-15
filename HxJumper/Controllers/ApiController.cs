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
using System.Web.Helpers;
using iTextSharp.text.pdf;
using System.Drawing.Imaging;
using HxJumper.Lib;
using HxJumper.Models.Constant;
using System.Text.RegularExpressions;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace HxJumper.Controllers
{
    public class ApiController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult ClientLogin(string jobNumber = null, string passWord = null, string serialNumber = null)
        {
            string userCheckResult = CheckUser(jobNumber, passWord, serialNumber);
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
            List<RemarkMessage> remarkMessages = unitOfWork.RemarkMessageRepository.Get(a => a.IsDeleted == false).ToList();
            List<LimitValue> limitValues = unitOfWork.LimitValueRepository.Get(a => a.IsDeleted == false).ToList();
            
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
            foreach (var remarkMessage in remarkMessages)
            {
                RemarkMessageXml remarkMessageXml = new RemarkMessageXml
                {
                    Id = remarkMessage.Id,
                    Name = remarkMessage.Name
                };
                loginReturnXml.remarkMessageXmls.remarkMessageXml.Add(remarkMessageXml);
            }
            foreach (var limitValue in limitValues)
            {
                LimitValueXml limitValueXml = new LimitValueXml 
                {
                    Id = limitValue.Id,
                    MinVal = limitValue.MinVal == null ? "NAN" : limitValue.MinVal.ToString(),
                    MaxVal = limitValue.MaxVal == null ? "NAN" : limitValue.MaxVal.ToString()
                };
                loginReturnXml.limitValueXmls.limitValueXml.Add(limitValueXml);
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
            string uploadPath = AppDomain.CurrentDomain.BaseDirectory + "/UploadedFolder/VNA/" + uploadTime;
            string savePath;
            string saveFolderPath;

            if (file == null || file.ContentLength <= 0)
            {
                result.Message = "file can not be null";
                return new XmlResult<SingleResultXml>() { Data = result };
            }

            fileFullName = System.IO.Path.GetFileName(file.FileName);
            fileEx = System.IO.Path.GetExtension(fileFullName);
            fileNameWithoutEx = System.IO.Path.GetFileNameWithoutExtension(fileFullName);
            if (fileEx.ToLower() != ".zip")
            {
                result.Message = "incorrect file type";
                return new XmlResult<SingleResultXml>() { Data = result };
            }
            var fileNameSplit = fileFullName.Split('_');
            if (fileNameSplit[0] == "" || fileNameSplit[1] == "")
            {
                result.Message = "incorrect file name";
                return new XmlResult<SingleResultXml>() { Data = result };
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

            ZipFile zip = ZipFile.Read(savePath, new ReadOptions { Encoding = Encoding.Default });
            try
            {
                //unzip file
                zip.AlternateEncoding = Encoding.Default;
                zip.ExtractAll(uploadPath, ExtractExistingFileAction.OverwriteSilently);
            }
            catch (Exception /*e*/)
            {
                result.Message = "can not unzip file";
                zip.Dispose();
                return new XmlResult<SingleResultXml>() { Data = result };
            }
            zip.Dispose();

            using (var scope = new TransactionScope())
            {
                saveFolderPath = uploadPath + slash + fileNameWithoutEx;
                //read general.csv
                string generalCsvPath = saveFolderPath + slash + "General.csv";
                //read all csv file, include General.csv
                string[] labelTitleFiles = Directory.GetFiles(saveFolderPath, "*.csv");
                string imagePath = saveFolderPath + slash + "result.png";
                if (!System.IO.File.Exists(generalCsvPath))
                {
                    result.Message = "can not find general.csv";
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
                string remarkMessageIdStr;
                string notStaticStr;
                string line = string.Empty;
                string[] lineArr = null;
                int i = 0;
                //only read csv file's second line
                while (i <= 1)
                {
                    line = srGeneralCsv.ReadLine();
                    i++;
                    if (i == 1)
                    {
                        continue;
                    }
                    if (line == null)
                    {
                        srGeneralCsv.Close();
                        result.Message = "general.csv test result is null";
                        return new XmlResult<SingleResultXml>() { Data = result };
                    }
                    lineArr = line.Split(',');
                    if (lineArr.Count() != 9)
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
                    remarkMessageIdStr = lineArr[7];
                    notStaticStr = lineArr[8];
                    srGeneralCsv.Close();
                    //convert testTimeStr to testTime
                    DateTime testTime;
                    if (!DateTime.TryParseExact(testTimeStr, "yyyyMMdd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out testTime))
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
                            TestClassNumber testClassNumberAdd = new TestClassNumber { Name = testClassNumberStr };
                            unitOfWork.TestClassNumberRepository.Insert(testClassNumberAdd);
                            unitOfWork.DbSaveChanges();
                            testClassNumberId = testClassNumberAdd.Id;
                        }
                        catch (Exception /*e*/)
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
                    if (testResultStr.ToUpper() == "FAIL")
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
                    //get RemarkMessage
                    int remarkMessageId = 0;
                    if (String.IsNullOrEmpty(remarkMessageIdStr) || remarkMessageIdStr == "")
                    {
                        //do nothing
                    }
                    else
                    {
                        if (!int.TryParse(remarkMessageIdStr, out remarkMessageId))
                        {
                            RemarkMessage remarkMessageDb = unitOfWork.RemarkMessageRepository.Get(a => a.Name == remarkMessageIdStr && a.IsDeleted == false).SingleOrDefault();
                            if (remarkMessageDb == null)
                            {
                                try
                                {
                                    RemarkMessage remarkMessageAdd = new RemarkMessage { Name = remarkMessageIdStr };
                                    unitOfWork.RemarkMessageRepository.Insert(remarkMessageAdd);
                                    unitOfWork.DbSaveChanges();
                                    remarkMessageId = remarkMessageAdd.Id;
                                }
                                catch (Exception /*e*/)
                                {
                                    result.Message = "Insert RemarkMessage failed";
                                    return new XmlResult<SingleResultXml>() { Data = result };
                                }
                            }
                            else 
                            {
                                remarkMessageId = remarkMessageDb.Id;
                            }
                        }
                        else
                        {
                            RemarkMessage remarkMessageDb = unitOfWork.RemarkMessageRepository.Get(a => a.Id == remarkMessageId && a.IsDeleted == false).SingleOrDefault();
                            if (remarkMessageDb == null)
                            {
                                result.Message = "general.csv RemarkMessage can not find";
                                return new XmlResult<SingleResultXml>() { Data = result };
                            }
                        }
                    }
                    //get NotStatistic
                    bool notStatistic = false;
                    int notStatisticInt = 0;//0 is Statistic, false; 1 is not Statistic
                    if (!int.TryParse(notStaticStr, out notStatisticInt))
                    {
                        result.Message = "general.csv NotStatistic  parse failed";
                        return new XmlResult<SingleResultXml>() { Data = result };
                    }
                    else
                    {
                        if (notStatisticInt == 1)
                        {
                            notStatistic = true;
                        }
                    }
                    //justify if IsLatest
                    var testResultDbs = unitOfWork.TestResultRepository.Get(a => a.TestCode == testCode).ToList();
                    if (testResultDbs != null)
                    {
                        foreach (var testResultDb in testResultDbs)
                        {
                            try
                            {
                                testResultDb.IsLatest = false;
                                unitOfWork.DbSaveChanges();
                            }
                            catch (Exception /*e*/)
                            {
                                result.Message = "update old TestResult record IsLatest field failed";
                                return new XmlResult<SingleResultXml>() { Data = result };
                            }
                        }
                    }
                    //TestResult Object
                    TestResult testResultAdd;
                    //if remarkMessageId == 0
                    if (remarkMessageId == 0)
                    {
                        testResultAdd = new TestResult
                        {
                            TestTime = testTime,
                            ProductTypeId = productTypeId,
                            TestClassNumberId = testClassNumberId,
                            TestCode = testCode,
                            TestImg = imagePath,
                            Result = testResult,
                            LineNumberId = lineNumberId,
                            JumperUserId = jumperUserId,
                            NotStatistic = notStatistic
                        };
                    }
                    else
                    {
                        testResultAdd = new TestResult
                        {
                            TestTime = testTime,
                            ProductTypeId = productTypeId,
                            TestClassNumberId = testClassNumberId,
                            TestCode = testCode,
                            TestImg = imagePath,
                            Result = testResult,
                            LineNumberId = lineNumberId,
                            JumperUserId = jumperUserId,
                            RemarkMessageId = remarkMessageId,
                            NotStatistic = notStatistic
                        };
                    }
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
                //read every label title csv
                foreach(var labelTitleFile in labelTitleFiles)
                {
                    //skip General.csv
                    if (!labelTitleFile.ToUpper().Contains("GENERAL")) 
                    {
                        //get label title and label title result
                        int labelTitleId = 0;
                        var sublabelTitleFile = labelTitleFile.Substring(labelTitleFile.LastIndexOf("\\") + 1);
                        var labelTitleFileArr = sublabelTitleFile.Split(new char[] { '_', '.' });
                        var labeTitleName = labelTitleFileArr[0].Substring(labelTitleFileArr[0].LastIndexOf('\\') + 1);
                        var labeTitleDb = unitOfWork.TestItemRepository.Get(a => a.Name == labeTitleName && a.IsDeleted == false).SingleOrDefault();
                        if (labeTitleDb == null) //if label title no exists
                        {
                            try
                            {
                                TestItem labelTitleAdd = new TestItem { Name = labeTitleName };
                                unitOfWork.TestItemRepository.Insert(labelTitleAdd);
                                unitOfWork.DbSaveChanges();
                                labelTitleId = labelTitleAdd.Id;
                            }
                            catch (Exception /*e*/)
                            {
                                result.Message = "Insert Label Title failed";
                                return new XmlResult<SingleResultXml>() { Data = result };
                            }
                        }
                        else
                        {
                            labelTitleId = labeTitleDb.Id;
                        }
                        //label title result
                        bool labelTitleResult = false;
                        if(labelTitleFileArr[1].ToUpper() == "PASS")
                        {
                            labelTitleResult = true;
                        }
                        int testResultItemId;
                        //insert TestResultItem
                        try 
                        {
                            TestResultItem testResultItem = new TestResultItem 
                            {
                                TestResultId = testResultId,
                                TestItemId = labelTitleId,
                                TestResultItemResult = labelTitleResult,
                            };
                            unitOfWork.TestResultItemRepository.Insert(testResultItem);
                            unitOfWork.DbSaveChanges();
                            testResultItemId = testResultItem.Id;
                        }
                        catch(Exception /*e*/)
                        {
                            result.Message = "Insert TestResultItem failed";
                            return new XmlResult<SingleResultXml>() { Data = result };
                        }
                        //read current Label Title csv file
                        try 
                        {
                            StreamReader srLabelTitleCsv = new StreamReader(labelTitleFile);
                            string perItemLine = string.Empty;
                            string[] perItemLineArr = null;
                            int lineNumResult = 0;
                            while ((perItemLine = srLabelTitleCsv.ReadLine()) != null)
                            {
                                lineNumResult++;
                                if (lineNumResult == 1)//skip first title line
                                {
                                    continue;
                                }
                                perItemLineArr = perItemLine.Split(',');
                                //TraceNumberStr
                                string traceNumberStr = perItemLineArr[0];
                                int traceNumber = 0;
                                if (!int.TryParse(traceNumberStr, out traceNumber))
                                {
                                    srLabelTitleCsv.Close();
                                    result.Message = sublabelTitleFile + " line " + lineNumResult + " TraceNumber parse failed";
                                    return new XmlResult<SingleResultXml>() { Data = result };
                                }
                                //MarkNumberStr
                                string markNumberStr = perItemLineArr[1];
                                int markNumber = 0;
                                if (!int.TryParse(markNumberStr, out markNumber))
                                {
                                    srLabelTitleCsv.Close();
                                    result.Message = sublabelTitleFile + " line " + lineNumResult + " MarkNumber parse failed";
                                    return new XmlResult<SingleResultXml>() { Data = result };
                                }
                                //XValue
                                string xValueStr = perItemLineArr[2];
                                decimal xValue = 0M;
                                Unit xValueUnit = new Unit();
                                if (!Decimal.TryParse(xValueStr, out xValue))
                                {
                                    srLabelTitleCsv.Close();
                                    result.Message = sublabelTitleFile + " line " + lineNumResult + " XValue: " + xValueStr + " parse failed";
                                    return new XmlResult<SingleResultXml>()
                                    {
                                        Data = result
                                    };
                                }
                                DecimalFormart.FormartDecimalUnit(xValue, xValueUnit, out xValue, out xValueUnit);
                                //YValue
                                string yValueStr = perItemLineArr[3];
                                decimal yValue = 0M;
                                if (!Decimal.TryParse(yValueStr, out yValue))
                                {
                                    srLabelTitleCsv.Close();
                                    result.Message = sublabelTitleFile + " line " + lineNumResult + " YValue: " + yValueStr + " parse failed";
                                    return new XmlResult<SingleResultXml>()
                                    {
                                        Data = result
                                    };
                                }
                                //ParameterStr
                                string parameterStr = perItemLineArr[4];
                                //FormartStr
                                string formartStr = perItemLineArr[5];
                                //insert into TestResultValue
                                try 
                                {
                                    TestResultValue testResultValue = new TestResultValue 
                                    {
                                       TestResultItemId = testResultItemId,
                                       TraceNumber = traceNumber,
                                       MarkNumber = markNumber,
                                       XValue = xValue,
                                       XValueUnit = xValueUnit,
                                       MarkValue = yValue,
                                       Formart = formartStr,
                                       Paremeter = parameterStr
                                    };
                                    unitOfWork.TestResultValueRepository.Insert(testResultValue);
                                    unitOfWork.DbSaveChanges();
                                }
                                catch(Exception /*e*/)
                                {
                                    srLabelTitleCsv.Close();
                                    result.Message = sublabelTitleFile + " line " + lineNumResult + "insert into TestResultValue failed";
                                    return new XmlResult<SingleResultXml>()
                                    {
                                        Data = result
                                    };
                                }
                            }
                            srLabelTitleCsv.Close();
                        }
                        catch(Exception /*e*/)
                        {
                            result.Message = "File labelTitleFile open failed";
                            return new XmlResult<SingleResultXml>() { Data = result };
                        }
                    }
                }
                scope.Complete();
            }

            return new XmlResult<SingleResultXml>() { Data = result};
        }
        public ActionResult UploadPimFileRonsenberger()
        {
            SingleResultXml result = new SingleResultXml() { Message = "true" };
            HttpPostedFileBase file = Request.Files["file"];
            string fileFullName;
            string fileEx;
            string fileNameWithoutEx;
            string slash = "/";
            string uploadTime = DateTime.Now.ToString("yyyyMMdd");
            string uploadPath = AppDomain.CurrentDomain.BaseDirectory + "/UploadedFolder/PIM/Ronsenberger/" + uploadTime;
            string savePath;
            string saveFolderPath;

            if (file == null || file.ContentLength <= 0)
            {
                result.Message = "file can not be null";
                return new XmlResult<SingleResultXml>() { Data = result };
            }

            fileFullName = System.IO.Path.GetFileName(file.FileName);
            fileEx = System.IO.Path.GetExtension(fileFullName);
            fileNameWithoutEx = System.IO.Path.GetFileNameWithoutExtension(fileFullName);
            if (fileEx.ToLower() != ".zip")
            {
                result.Message = "incorrect file type";
                return new XmlResult<SingleResultXml>() { Data = result };
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

            ZipFile zip = ZipFile.Read(savePath, new ReadOptions { Encoding = Encoding.Default });
            try
            {
                //unzip file
                zip.AlternateEncoding = Encoding.Default;
                zip.ExtractAll(uploadPath + slash + fileNameWithoutEx, ExtractExistingFileAction.OverwriteSilently);
            }
            catch (Exception /*e*/)
            {
                result.Message = "can not unzip file";
                zip.Dispose();
                return new XmlResult<SingleResultXml>() { Data = result };
            }
            zip.Dispose();

            using (var scope = new TransactionScope())
            {
                saveFolderPath = uploadPath + slash + fileNameWithoutEx;
                //read xls File
                string serialNumber = "";
                string testImage = "";
                string excelFilePath = "";
                string txtFilePath = "";
                string pdfFilePath = "";
                string[] xlsFiles = Directory.GetFiles(saveFolderPath, "*.xls");
                if (xlsFiles.Count() != 1)
                {
                    result.Message = "The number of excel incorrect";
                    return new XmlResult<SingleResultXml>() { Data = result };
                }
                else 
                {
                    //get serial number, excel file without extension
                    serialNumber = System.IO.Path.GetFileNameWithoutExtension(xlsFiles[0]);
                    excelFilePath = saveFolderPath + slash + serialNumber + ".xls";
                    txtFilePath = saveFolderPath + slash + serialNumber + ".txt";
                    pdfFilePath = saveFolderPath + slash + serialNumber + ".pdf";
                }
                if (!System.IO.File.Exists(pdfFilePath)) 
                {
                    result.Message = "can not find " + serialNumber + ".pdf file";
                    return new XmlResult<SingleResultXml>() { Data = result };
                }
                if (!System.IO.File.Exists(txtFilePath))
                {
                    result.Message = "can not find " + serialNumber + ".txt file";
                    return new XmlResult<SingleResultXml>() { Data = result };
                }

                try 
                {
                    PdfHandler.ExtractImagesFromPDF(pdfFilePath, saveFolderPath);
                    testImage = "/Ronsenberger/" + uploadTime + slash + fileNameWithoutEx + slash + "result.jpg";
                }
                catch(Exception /*e*/)
                {
                    result.Message = "can not generate image from " + serialNumber + ".pdf file";
                    return new XmlResult<SingleResultXml>() { Data = result };
                }
                
                //get TestResultPim records in DB
                var testResultPimDbs = unitOfWork.TestResultPimRepository.Get(a => a.SerialNumber == serialNumber).ToList();
                if(testResultPimDbs.Count != 0)
                {
                    foreach(var testResultPimDb in testResultPimDbs)
                    {
                        try
                        {
                            testResultPimDb.IsLatest = false;
                            unitOfWork.DbSaveChanges();
                        }
                        catch(Exception /*e*/)
                        {
                            result.Message = "update TestResultPim Old record failed";
                            return new XmlResult<SingleResultXml>() { Data = result };
                        }
                    }
                }
                //initialize new TestResultPim
                TestResultPim testResultPim = new TestResultPim();
                testResultPim.SerialNumber = serialNumber;
                testResultPim.TestImage = testImage;

                //read txt file
                StreamReader srTxt = new StreamReader(txtFilePath);
                string txtLine = string.Empty;
                string[] txtLineArr = null;
                int txtLineNumber = 0;
                while (txtLineNumber < 25)//maxLineNumber = 25
                {
                    txtLine = srTxt.ReadLine();
                    txtLineNumber++;
                    if (txtLineNumber <= 2)//first two line, TestResultPim.Carrier
                    {
                        txtLineArr = txtLine.Trim().Split(' ');
                        //SetFreq
                        string setFreqStr = txtLineArr[1];
                        decimal setFreq;
                        if (!Decimal.TryParse(setFreqStr, out setFreq))
                        {
                            srTxt.Close();
                            result.Message = "line " + txtLineNumber + " SetFreq: " + setFreqStr + " parse failed";
                            return new XmlResult<SingleResultXml>()
                            {
                                Data = result
                            };
                        }
                        //StartFreq
                        string startFreqStr = txtLineArr[4];
                        decimal startFreq;
                        if (!Decimal.TryParse(startFreqStr, out startFreq))
                        {
                            srTxt.Close();
                            result.Message = "line " + txtLineNumber + " StartFreq: " + startFreqStr + " parse failed";
                            return new XmlResult<SingleResultXml>()
                            {
                                Data = result
                            };
                        }
                        //StopFreq
                        string stopFreqStr = txtLineArr[7];
                        decimal stopFreq;
                        if (!Decimal.TryParse(stopFreqStr, out stopFreq))
                        {
                            srTxt.Close();
                            result.Message = "line " + txtLineNumber + " StopFreq: " + stopFreqStr + " parse failed";
                            return new XmlResult<SingleResultXml>()
                            {
                                Data = result
                            };
                        }
                        //Freq unit
                        string freqUnitStr = txtLineArr[2];
                        Unit freqUnit;
                        if (freqUnitStr.ToUpper() == "MHZ")
                        {
                            freqUnit = Unit.M;
                        }
                        else 
                        {
                            srTxt.Close();
                            result.Message = "line " + txtLineNumber + " Freq unit: " + freqUnitStr + " parse failed";
                            return new XmlResult<SingleResultXml>()
                            {
                                Data = result
                            };
                        }
                        //Power
                        string powerStr = txtLineArr[10];
                        decimal power;
                        if (!Decimal.TryParse(powerStr, out power))
                        {
                            srTxt.Close();
                            result.Message = "line " + txtLineNumber + " Power: " + powerStr + " parse failed";
                            return new XmlResult<SingleResultXml>()
                            {
                                Data = result
                            };
                        }
                        //Power Unit
                        string imUnitStr = txtLineArr[11];
                        ImUnit imUnit;
                        if (imUnitStr.ToUpper() == "DBC")
                        {
                            imUnit = ImUnit.dBc;
                        }
                        else if (imUnitStr.ToUpper() == "DBM")
                        {
                            imUnit = ImUnit.dBm;
                        }
                        else
                        {
                            srTxt.Close();
                            result.Message = "line " + txtLineNumber + " Power unit: " + imUnitStr + " parse failed";
                            return new XmlResult<SingleResultXml>()
                            {
                                Data = result
                            };
                        }
                        Carrier carrier = new Carrier() 
                        {
                            SetFreq = setFreq,
                            StartFreq = startFreq,
                            StopFreq = stopFreq,
                            Power = power,
                            FreqUnit = freqUnit,
                            ImUnit = imUnit
                        };
                        var carrierDb = unitOfWork.CarrierRepository.Get(a => a.SetFreq == setFreq && a.StartFreq == startFreq && a.StopFreq == stopFreq && a.Power == power && a.FreqUnit == freqUnit && a.ImUnit == imUnit).SingleOrDefault();
                        if (carrierDb == null)
                        {
                            try
                            {
                                unitOfWork.CarrierRepository.Insert(carrier);
                                unitOfWork.DbSaveChanges();
                                testResultPim.Carriers.Add(carrier);
                            }
                            catch (Exception /*e*/)
                            {
                                srTxt.Close();
                                result.Message = "insert Carrier failed";
                                return new XmlResult<SingleResultXml>()
                                {
                                    Data = result
                                };
                            }
                        }
                        else 
                        {
                            testResultPim.Carriers.Add(carrierDb);
                        }
                    }
                    else if (txtLineNumber == 3)//ImOrder, TestResultPim.TestMeas in line 3
                    {
                        txtLineArr = txtLine.Trim().Split(' ');
                        //ImOrder.OrderNumber
                        string orderNumberStr = txtLineArr[1];
                        int orderNumber;
                        if (!int.TryParse(orderNumberStr.Substring(orderNumberStr.Length - 1), out orderNumber))
                        {
                            srTxt.Close();
                            result.Message = "line " + txtLineNumber + " " + orderNumberStr  + " parse failed";
                            return new XmlResult<SingleResultXml>() { Data = result };
                        }
                        //ImOrder.ImUnit
                        string imUnitStr = txtLineArr[3];
                        ImUnit imUnit;
                        if (imUnitStr.ToUpper() == "DBC")
                        {
                            imUnit = ImUnit.dBc;
                        }
                        else if (imUnitStr.ToUpper() == "DBM")
                        {
                            imUnit = ImUnit.dBm;
                        }
                        else
                        {
                            srTxt.Close();
                            result.Message = "line " + txtLineNumber + " Power unit: " + imUnitStr + " parse failed";
                            return new XmlResult<SingleResultXml>()
                            {
                                Data = result
                            };
                        }
                        //set TestResultPim.TestMeans
                        string testMeansStr = txtLineArr[4];
                        TestMeans testMeans;
                        if (testMeansStr.ToUpper() == "SWEEP")
                        {
                            testMeans = TestMeans.Sweep;
                        }
                        else if (testMeansStr.ToUpper() == "SINGLE")
                        {
                            testMeans = TestMeans.Single;
                        }
                        else 
                        {
                            srTxt.Close();
                            result.Message = "line " + txtLineNumber + "  Meas: " + testMeansStr + " parse failed";
                            return new XmlResult<SingleResultXml>()
                            {
                                Data = result
                            };
                        }
                        //set TestResultPim.TestMeans
                        testResultPim.TestMeans = testMeans;
                        //set TestResultPim.ImOrderId
                        ImOrder imOrder = new ImOrder() { OrderNumber = orderNumber, ImUnit = imUnit };
                        //get ImOrder from db
                        var imOrderDb = unitOfWork.ImOrderRepository.Get(a => a.OrderNumber == orderNumber && a.ImUnit == imUnit).SingleOrDefault();
                        if (imOrderDb == null)
                        {
                            try 
                            {
                                unitOfWork.ImOrderRepository.Insert(imOrder);
                                unitOfWork.DbSaveChanges();
                                testResultPim.ImOrderId = imOrder.Id;
                            }
                            catch(Exception /*e*/)
                            {
                                srTxt.Close();
                                result.Message = "line " + txtLineNumber + ", Insert IMOrder failed";
                                return new XmlResult<SingleResultXml>()
                                {
                                    Data = result
                                };
                            }
                        }
                        else 
                        {
                            testResultPim.ImOrderId = imOrderDb.Id;
                        }
                    }
                    else if (txtLineNumber == 4)//TestEquipment in line 4
                    {
                        txtLineArr = txtLine.Trim().Split(' ');
                        string name = txtLineArr[2];
                        string serailNumber = txtLineArr[txtLineArr.Count() - 1];
                        //set TestResultPim.TestEquipmentId
                        TestEquipment testEquipment = new TestEquipment() { Name = name.Substring(0, name.Length -1), SerialNumber = serailNumber, isVna = false, IsDeleted = false};
                        var testEquipmentDb = unitOfWork.TestEquipmentRepository.Get(a => a.Name == name.Substring(0, name.Length - 1) && a.SerialNumber == serailNumber).First();
                        if (testEquipmentDb == null)
                        {
                            try 
                            {
                                unitOfWork.TestEquipmentRepository.Insert(testEquipment);
                                unitOfWork.DbSaveChanges();
                                testResultPim.TestEquipmentId = testEquipment.Id;
                            }
                            catch(Exception /*e*/)
                            {
                                srTxt.Close();
                                result.Message = "line " + txtLineNumber + ", Insert TestEquipment failed";
                                return new XmlResult<SingleResultXml>() { Data = result };
                            }
                        }
                        else
                        {
                            testResultPim.TestEquipmentId = testEquipmentDb.Id;
                        }
                    }
                    else if (txtLineNumber == 5)//TestTime in line 5
                    {
                        string testTimeStr = txtLine.Trim().Replace(":", "") + DateTime.Now.ToString("ss");//add ss to Test Time string 
                        DateTime testTime;
                        if (!DateTime.TryParseExact(testTimeStr, "yyyyMMdd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out testTime))
                        {
                            srTxt.Close();
                            result.Message = "line " + txtLineNumber + " Test Time : " + testTimeStr + " parse failed";
                            return new XmlResult<SingleResultXml>() { Data = result };
                        }
                        //set TestResultPim.TestTime
                        testResultPim.TestTime = testTime;
                    }
                    else if (txtLineNumber == 23)//TestResultPim.LimitLine
                    {
                        txtLineArr = txtLine.Trim().Split(' ');
                        string limitLineStr = txtLineArr[txtLineArr.Count() - 1];
                        decimal limitLine;
                        if (!Decimal.TryParse(limitLineStr, out limitLine))
                        {
                            srTxt.Close();
                            result.Message = "line " + txtLineNumber + " Limitline: " + limitLineStr + " parse failed";
                            return new XmlResult<SingleResultXml>() { Data = result };
                        }
                        //set TestResultPim.LimitLine
                        testResultPim.LimitLine = limitLine;
                    }
                    else
                    {
                        //other line, useless ignore
                    }
                }
                //close txt file
                srTxt.Close();

                //read excel file
                HSSFWorkbook workbook = new HSSFWorkbook();
                using (FileStream fsExcelFile = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
                {
                    workbook = new HSSFWorkbook(fsExcelFile);
                }
                ISheet sheet = workbook.GetSheetAt(0);
                List<TestResultPimPoint> testResultPimPoints = new List<TestResultPimPoint> { };
                for (int row = 0; row <= sheet.LastRowNum; row++)
                {
                    if (row == 0)
                    {
                        //skip line 1, title line
                    }
                    else 
                    {
                        if (sheet.GetRow(row) != null)
                        {
                            string carrierOneFreqStr = sheet.GetRow(row).GetCell(0).StringCellValue;
                            decimal carrierOneFreq;
                            if (!Decimal.TryParse(carrierOneFreqStr, out carrierOneFreq))
                            {
                                result.Message = "excel line " + (row + 1) + " Carrier 1 Freq " + carrierOneFreqStr + " parse failed";
                                return new XmlResult<SingleResultXml>() { Data = result };
                            }
                            string carrierTwoFreqStr = sheet.GetRow(row).GetCell(1).StringCellValue;
                            decimal carrierTwoFreq;
                            if (!Decimal.TryParse(carrierTwoFreqStr, out carrierTwoFreq))
                            {
                                result.Message = "excel line " + (row + 1) + " Carrier 2 Freq " + carrierOneFreqStr + " parse failed";
                                return new XmlResult<SingleResultXml>() { Data = result };
                            }
                            string imFreqStr = sheet.GetRow(row).GetCell(6).StringCellValue;
                            decimal imFreq;
                            if (!Decimal.TryParse(imFreqStr, out imFreq))
                            {
                                result.Message = "excel line " + (row + 1) + " IM Freq " + imFreqStr + " parse failed";
                                return new XmlResult<SingleResultXml>() { Data = result };
                            }
                            string imPowerStr = sheet.GetRow(row).GetCell(7).StringCellValue;
                            decimal imPower;
                            if (!Decimal.TryParse(imPowerStr, out imPower))
                            {
                                result.Message = "excel line " + (row + 1) + " IM Power " + imFreqStr + " parse failed";
                                return new XmlResult<SingleResultXml>()
                                {
                                    Data = result
                                };
                            }
                            TestResultPimPoint testResultPimPoint = new TestResultPimPoint 
                            {
                                CarrierOneFreq = carrierOneFreq,
                                CarrierTwoFreq = carrierTwoFreq,
                                ImFreq = imFreq,
                                ImPower = imPower
                            };
                            testResultPimPoints.Add(testResultPimPoint);
                        }
                    }
                }

                var maxImPower = testResultPimPoints.Max(a => a.ImPower);
                var maxImPowerPoint = testResultPimPoints.Where(a => a.ImPower == maxImPower).First();
                maxImPowerPoint.isWorst = true;
                //set TestResultPim.TestResultPimPoints
                testResultPim.TestResultPimPoints = testResultPimPoints;
                //set TestResultPim.TestResult
                if (maxImPower < testResultPim.LimitLine)
                {
                    testResultPim.TestResult = true;
                }

                try 
                {
                    unitOfWork.TestResultPimRepository.Insert(testResultPim);
                    unitOfWork.DbSaveChanges();
                }
                catch(Exception /*e*/)
                {
                    result.Message = "Insert TestResultPim failed";
                    return new XmlResult<SingleResultXml>()
                    {
                        Data = result
                    };
                }
                scope.Complete();                
            }
            
            return new XmlResult<SingleResultXml>() { Data = result };
        }

        private string CheckUser(string jobNumber = null, string passWord = null, string serialNumber = null) 
        {
            string result = "true";
            if (String.IsNullOrWhiteSpace(jobNumber) || String.IsNullOrWhiteSpace(passWord) || String.IsNullOrWhiteSpace(serialNumber))
            {
                result = "jobnumer or password or serialNumber can not be null.";
            }
            else
            {
                try
                {
                    var user = unitOfWork.JumperUserRepository.Get(a => a.JobNumber.ToUpper() == (jobNumber.ToUpper()) && a.JumperRole.Name == "测试员" && a.IsDeleted == false).SingleOrDefault();
                    if (user == null)
                    {
                        result = "jobnumber or password incorrect.";
                        return result;
                    }
                    else
                    {
                        var userName = user.UserName;
                        user = unitOfWork.JumperUserRepository.context.UserManager.Find(userName, passWord);
                        if (user == null)
                        {
                            result = "jobnumber or password incorrect.";
                            return result;
                        }
                    }

                    var equipment = unitOfWork.TestEquipmentRepository.Get(a => a.SerialNumber.ToUpper() == (serialNumber.ToUpper()) && a.isVna == true && a.IsDeleted == false).SingleOrDefault();
                    if (equipment == null) {
                        result = "can not find such equipment, may be you forgot add it to server";
                        return result;
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