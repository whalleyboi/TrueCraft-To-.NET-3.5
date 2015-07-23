using System;
using System.Xml;

/// <summary>
/// Created by Whalleyboi to read .csporj files to add / remove references +more
/// </summary>
namespace TrueCraft.NET35
{
    class Reference
    {
        public string Status = "";
        public string csproj = "";

        XmlDocument xmldoc = new XmlDocument();
        XmlNamespaceManager mgr;

        public Reference(string csproj)
        {
            this.csproj = csproj;
            mgr = new XmlNamespaceManager(xmldoc.NameTable);
            xmldoc.Load(csproj);
            mgr.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");
        }

        /// <summary>
        /// Downgrade the .NET Framework to Unity3D compaitable 3.5
        /// </summary>
        public void ChangeFramework()
        {
            var item = xmldoc.SelectNodes("//x:PropertyGroup//x:TargetFrameworkVersion", mgr)[0];
            string name2 = item.InnerText.ToString();
            if (name2 == "v4.5" || name2 == "v4")
            {
                item.InnerText = "v3.5";
                xmldoc.Save(csproj);
                Status += "Updated framework version to .NET v3.5" + Environment.NewLine;
            }
        }

        public void CreateReference(string name, string path)
        {
            var item = xmldoc.SelectNodes("//x:ItemGroup", mgr)[0];

            XmlElement elem = xmldoc.CreateElement("Reference", "http://schemas.microsoft.com/developer/msbuild/2003");
            XmlAttribute attr = xmldoc.CreateAttribute("Include");
            XmlElement hint = xmldoc.CreateElement("HintPath", "http://schemas.microsoft.com/developer/msbuild/2003");
            attr.InnerText = name;
            elem.Attributes.Append(attr);
            elem.AppendChild(hint).InnerText = path;

            item.AppendChild(elem);

            xmldoc.Save(csproj);
            Status += "Added " + name + " reference." + Environment.NewLine;
        }

        public void RemoveReference(string name)
        {
            foreach (XmlNode item2 in xmldoc.SelectNodes("//x:ItemGroup//x:Reference", mgr))
            {
                foreach (XmlAttribute attrs in item2.Attributes)
                {
                    if (attrs.InnerText == name)
                    {
                        item2.ParentNode.RemoveChild(item2);
                        Status += "Removed " + name + " reference to avoid conflict." + Environment.NewLine;
                    }
                }
            }
        }

        public void RemoveOriginal(string name)
        {
            foreach (XmlNode item3 in xmldoc.SelectNodes("//x:ItemGroup//x:ProjectReference//x:Name", mgr))
            {
                string name2 = item3.InnerText.ToString();
                // Remove // Add 
                if (name2 == name)
                {
                    item3.ParentNode.ParentNode.RemoveChild(item3.ParentNode);
                    Status += "Removed original " + name + " reference." + Environment.NewLine;
                }
            }
        }
    }
}
