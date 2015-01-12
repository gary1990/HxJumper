using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace HxJumper.Common
{
    [XmlRoot("Result")]
    public class SingleResultXml
    {
        public string Message { get; set; }
    }

    public class ProductTypeXml 
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class ProductTypeXmls 
    {
        public ProductTypeXmls() 
        {
            productTypeXml = new List<ProductTypeXml> { };
        }
        [XmlElement("ProductType")]
        public List<ProductTypeXml> productTypeXml { get; set; }
    }
    public class TestClassNumberXml 
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class TestClassNumberXmls 
    {
        public TestClassNumberXmls() 
        {
            testClassNumberXml = new List<TestClassNumberXml> { };
        }
        [XmlElement("TestClassNumber")]
        public List<TestClassNumberXml> testClassNumberXml { get; set; }
    }
    public class LineNumberXml
    {
        public int Id { get; set;}
        public string Name { get; set;}
    }
    public class LineNumberXmls 
    {
        public LineNumberXmls() 
        {
            lineNumberXml = new List<LineNumberXml> { };
        }
        [XmlElement("LineNumber")]
        public List<LineNumberXml> lineNumberXml { get; set; }
    }
    public class RemarkMessageXml
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class RemarkMessageXmls
    {
        public RemarkMessageXmls()
        {
            remarkMessageXml = new List<RemarkMessageXml> { };
        }
        [XmlElement("RemarkMessage")]
        public List<RemarkMessageXml> remarkMessageXml { get; set; }
    }
    //login success returned xml's formart
    [XmlRoot("Result")]
    public class LoginReturnXml 
    {
        public LoginReturnXml() 
        {
            Message = "true";
            productTypeXmls = new ProductTypeXmls();
            testClassNumberXmls = new TestClassNumberXmls();
            lineNumberXmls = new LineNumberXmls();
            remarkMessageXmls = new RemarkMessageXmls();
        }
        public string Message { get; set; }
        //return ProductList xml
        [XmlElement("ProductTypes")]
        public ProductTypeXmls productTypeXmls { get; set; }
        //return TestClassNumber xml
        [XmlElement("TestClassNumbers")]
        public TestClassNumberXmls testClassNumberXmls { get; set; }
        //return LineNumber xml
        [XmlElement("LineNumbers")]
        public LineNumberXmls lineNumberXmls { get; set; }
        //return RemarkMessage xml
        [XmlElement("RemarkMessages")]
        public RemarkMessageXmls remarkMessageXmls { get; set; }
    }
}