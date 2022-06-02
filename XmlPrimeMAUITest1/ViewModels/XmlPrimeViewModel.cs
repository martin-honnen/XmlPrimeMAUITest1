using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using XmlPrime;

namespace XmlPrimeMAUITest1.ViewModels
{
    public class XmlPrimeViewModel : INotifyPropertyChanged
    {
        public XmlPrimeViewModel()
        {
            TestCallTemplateTransformationResult = new Command(() => { XdmTransformationResult = RunTestCallTemplate(); });
        }

        private NameTable nameTable = new NameTable();

        public event PropertyChangedEventHandler PropertyChanged;


        public Command TestCurrentDateTimeValue { get; }
        public Command TestCallTemplateTransformationResult { get; }

        private string xdmTransformationResult = "Not tested!";
        public string XdmTransformationResult
        {
            get
            {
                return xdmTransformationResult;
            }
            set
            {
                if (xdmTransformationResult != value)
                {
                    xdmTransformationResult = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(XdmTransformationResult)));
                }
            }
        }

        private string RunTestCallTemplate()
        {

            XdmDocument document = new XdmDocument(new StringReader(@"<root>
  <foo>bar</foo>
</root>"), XmlSpace.Preserve);


            string xsltCode = @"<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='3.0'>
  <xsl:template name='xsl:initial-template' match='/'>
    <Test>Run with <xsl:value-of select='system-property(&apos;xsl:product-name&apos;), system-property(&apos;xsl:product-version&apos;)'/> at <xsl:value-of select='current-dateTime()'/></Test>
  </xsl:template>
</xsl:stylesheet>";

            XsltSettings xsltSettings = new XsltSettings(nameTable);
            xsltSettings.ContextItemType = XdmType.Node;
            xsltSettings.XsltVersion = XsltVersion.Xslt30;

            Xslt xslt = Xslt.Compile(new StringReader(xsltCode), xsltSettings);

            XPathNavigator contextItem = document.CreateNavigator();

            using (var resultWriter = new StringWriter())
            {
                xslt.ApplyTemplates(contextItem, resultWriter);
                return resultWriter.ToString();
            }
  
        }
    }
}
