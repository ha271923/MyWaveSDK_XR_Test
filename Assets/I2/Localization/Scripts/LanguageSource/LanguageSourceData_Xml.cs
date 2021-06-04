using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace I2.Loc
{
	public partial class LanguageSourceData
	{
        private Dictionary<string, XmlDocument> mDict = new Dictionary<string, XmlDocument>();
        private List<string> mLangList = new List<string>();
        private Dictionary<string, string> mLangDict = new Dictionary<string, string>()
        {
            { "en-US", "" },
        };

        private void AddDataInRoot(XmlDocument xmlDoc, string key, string value, string comment)
        {
            XmlNode rootNode = xmlDoc.SelectSingleNode("root");

            XmlElement dataElement = xmlDoc.CreateElement("data");
            dataElement.SetAttribute("name", key);

            XmlElement valueElement = xmlDoc.CreateElement("value");
            valueElement.InnerText = value;
            dataElement.AppendChild(valueElement);

            if (comment != null)
            {
                XmlElement commentElement = xmlDoc.CreateElement("comment");
                commentElement.InnerText = string.Format("(en: {0})", comment);
                dataElement.AppendChild(commentElement);
            }

            rootNode.AppendChild(dataElement);
        }

        private string GetValue(XmlDocument xmlDoc, string key, string defaultValue)
        {
            var value = defaultValue;

            XmlNodeList nodeList = xmlDoc.SelectSingleNode("root").ChildNodes;
            foreach (XmlNode childNode in nodeList)
            {
                if (childNode is XmlElement childElement && childElement.GetAttribute("name") == key)
                {
                    XmlNodeList grandsonNodes = childElement.ChildNodes;
                    foreach (XmlNode grandsonNode in grandsonNodes)
                    {
                        if (grandsonNode is XmlElement grandsonElement && grandsonElement.Name == "value")
                        {
                            value = grandsonElement.InnerText;
                            break;
                        }
                    }
                    break;
                }
            }

            return value;
        }

        private XmlDocument CreateXmlDocument()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null));

            XmlElement xmlelem = xmlDoc.CreateElement("root");
            xmlDoc.AppendChild(xmlelem);

            return xmlDoc;
        }

        private void LoadXml()
        {
            mDict.Clear();
            mLangList.Clear();
            foreach (LanguageData langData in mLanguages)
            {
                var targetPath = Application.dataPath + $"/Resources/Localization/strings{MapLang(langData.Code)}.xml";
                if (!File.Exists(targetPath))
                    targetPath = Application.dataPath + $"/Resources/Localization/strings.xml";

                var xmlDoc = new XmlDocument();
                xmlDoc.Load(targetPath);
                mDict.Add(langData.Code, xmlDoc);
                mLangList.Add(langData.Code);
            }
        }

        private void CreateXmlDocuments()
        {
            mDict.Clear();
            mLangList.Clear();
            foreach (LanguageData langData in mLanguages)
            {
                var xmlDoc = CreateXmlDocument();
                mDict.Add(langData.Code, xmlDoc);
                mLangList.Add(langData.Code);
            }
        }

        private void SaveXml()
        {
            for (int i = 0; i < mLangList.Count; i++)
            {
                var lang = mLangList[i];
                var xmlDoc = mDict[lang];

                if (lang.Equals("en-US")) // commenting out this line can generate all xml files
                    xmlDoc.Save(Application.dataPath + $"/Resources/Localization/strings{MapLang(lang)}.xml");
            }
        }

        private string MapLang(string lang)
        {
            string ret = lang;

            if (mLangDict.TryGetValue(lang, out string value))
                ret = value;

            if (!string.IsNullOrEmpty(ret))
                ret = "_" + ret;

            return ret;
        }
    }
}