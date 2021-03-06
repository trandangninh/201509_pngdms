using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Routing;
using System.Xml;
using RepositoryPattern.Infrastructure;
using Service.Security;
using Web.Models;

namespace Web
{
    public class XmlSiteMap
    {
        public XmlSiteMap()
        {
            RootNode = new SiteMapNode();
        }

        public SiteMapNode RootNode { get; set; }

        public virtual void LoadFrom(string physicalPath)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            physicalPath = physicalPath.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            physicalPath = Path.Combine(baseDirectory, physicalPath);

            string content = File.ReadAllText(physicalPath);

            if (!string.IsNullOrEmpty(content))
            {
                using (var sr = new StringReader(content))
                {
                    using (var xr = XmlReader.Create(sr,
                            new XmlReaderSettings
                            {
                                CloseInput = true,
                                IgnoreWhitespace = true,
                                IgnoreComments = true,
                                IgnoreProcessingInstructions = true
                            }))
                    {
                        var doc = new XmlDocument();
                        doc.Load(xr);

                        if ((doc.DocumentElement != null) && doc.HasChildNodes)
                        {
                            XmlNode xmlRootNode = doc.DocumentElement.FirstChild;
                            Iterate(RootNode, xmlRootNode);
                        }
                    }
                }
            }
        }

        private static void Iterate(SiteMapNode siteMapNode, XmlNode xmlNode)
        {
            PopulateNode(siteMapNode, xmlNode);

            foreach (XmlNode xmlChildNode in xmlNode.ChildNodes)
            {
                if (xmlChildNode.LocalName.Equals("siteMapNode", StringComparison.InvariantCultureIgnoreCase))
                {
                    var siteMapChildNode = new SiteMapNode();
                    siteMapNode.ChildNodes.Add(siteMapChildNode);

                    Iterate(siteMapChildNode, xmlChildNode);
                }
            }
        }

        private static void PopulateNode(SiteMapNode siteMapNode, XmlNode xmlNode)
        {
            //title
            var title = GetStringValueFromAttribute(xmlNode, "title");
            if (!string.IsNullOrEmpty(title))
            {
                siteMapNode.Title = title;
            }
            else
            {
                siteMapNode.Title = GetStringValueFromAttribute(xmlNode, "title");
            }

            //routes, url
            string controllerName = GetStringValueFromAttribute(xmlNode, "controller");
            string actionName = GetStringValueFromAttribute(xmlNode, "action");
            string url = GetStringValueFromAttribute(xmlNode, "url");
            if (!string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(actionName))
            {
                siteMapNode.ControllerName = controllerName;
                siteMapNode.ActionName = actionName;

                //apply admin area as described here - http://www.nopcommerce.com/boards/t/20478/broken-menus-in-admin-area-whilst-trying-to-make-a-plugin-admin-page.aspx
                var id = GetStringValueFromAttribute(xmlNode, "id");
                if (!string.IsNullOrEmpty(id))
                    siteMapNode.RouteValues = new RouteValueDictionary {{"id", id}};
            }
            else if (!string.IsNullOrEmpty(url))
            {
                siteMapNode.Url = url;
            }

            //image URL
            siteMapNode.Icon = GetStringValueFromAttribute(xmlNode, "Icon");

            //permission name
            var permissionNames = GetStringValueFromAttribute(xmlNode, "PermissionNames");
            if (!string.IsNullOrEmpty(permissionNames))
            {
                var permissionService = EngineContext.Current.Resolve<IPermissionService>();
                siteMapNode.Visible = permissionNames.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                   .Any(permissionName => permissionService.Authorize(permissionName.Trim()));
            }
            else
            {
                siteMapNode.Visible = true;
            }
        }

        private static string GetStringValueFromAttribute(XmlNode node, string attributeName)
        {
            string value = null;

            if (node.Attributes != null && node.Attributes.Count > 0)
            {
                XmlAttribute attribute = node.Attributes[attributeName];

                if (attribute != null)
                {
                    value = attribute.Value;
                }
            }

            return value;
        }
    }
}