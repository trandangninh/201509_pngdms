using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Entities.Domain;

namespace Service.Common
{
    public class XmlService : IXmlService
    {
        public List<ProductionPlanningColor> GetAllProductionPlanningColors()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "XmlFolder/ProductPlanningColor.xml";
            var listProductionPlanningColor = new List<ProductionPlanningColor>();
            XElement root = XElement.Load(path);            
            IEnumerable<XElement> address = from el in root.Elements("ProducPlanningColor")
                                            select el;

            foreach (XElement elm in address)
            {
                var item = new ProductionPlanningColor()
                           {
                               ProductionName = elm.Element("ProductName").Value,
                               Color = elm.Element("Color").Value,
                           };
                listProductionPlanningColor.Add(item);

            }
            return listProductionPlanningColor;
        }
        public List<String> GetAllComplaintTrackingColor()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "XmlFolder/ComplaintTrackingColorList.xml";
            var colors = new List<string>();
            XElement root = XElement.Load(path);
            IEnumerable<XElement> xColors = from el in root.Elements("Color")
                                            select el;

            foreach (XElement elm in xColors)
            {
                colors.Add(elm.Value);
            }
            return colors;
        }
        public List<String> GetAllNumberComplaintTrackingColor()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "XmlFolder/NumberComplaintTrackingColorList.xml";
            var colors = new List<string>();
            XElement root = XElement.Load(path);
            IEnumerable<XElement> xColors = from el in root.Elements("Color")
                                            select el;

            foreach (XElement elm in xColors)
            {
                colors.Add(elm.Value);
            }
            return colors;
        }

        public int GetQA_MarkLimit()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "XmlFolder/Config.xml";
            XElement root = XElement.Load(path);
            int markLimit = Convert.ToInt32(root.Elements("QAMarkLimit").First().Value);
            return markLimit;
        }

        public int GetQA_SeverityLimit()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "XmlFolder/Config.xml";            
            XElement root = XElement.Load(path);
            int severityLimit = Convert.ToInt32(root.Elements("QASeverityLimit").First().Value);
            return severityLimit;
        }

        public string GetQA_EmailWarning()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "XmlFolder/Config.xml";
            XElement root = XElement.Load(path);
            string email = root.Elements("Email").First().Value;
            return email;
        }
    }
}
