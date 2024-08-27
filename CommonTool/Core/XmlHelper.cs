#region USING

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

#endregion

namespace CommonTool.Core
{
    /// <summary>
    /// xml帮助类
    /// </summary>
    public class XmlHelper
    {
        private string mFileName;
        private bool mHasFile;
        private XmlDocument mXmlDoc;

        public XmlHelper()
        {
            mHasFile = false;
            mXmlDoc = new XmlDocument();
        }

        /// <summary>
        /// 添加一个节点
        /// </summary>
        /// <param name="ParentNode">节点 格式：中间加.分割 例:Group.Name</param>
        /// <param name="NodeName">子节点名称</param>
        /// <returns></returns>
        public bool AddNode(string ParentNode,string NodeName)
        {
            XmlElement XmlEle;

            if(!mHasFile)
            {
                return false;
            }

            try
            {
                XmlNode node;
                string[] NodeNames = ParentNode.Split('.');
                node = mXmlDoc.SelectSingleNode(NodeNames[0]);
                for(int i = 1;i < NodeNames.Length;i++)
                {
                    node = node.SelectSingleNode(NodeNames[i]);
                }

                XmlEle = mXmlDoc.CreateElement(NodeName);
                node.AppendChild(XmlEle);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }

        /// <summary>
        /// 添加一个节点并赋值
        /// </summary>
        /// <param name="ParentNode">节点 格式：中间加.分割 例:Group.Name</param>
        /// <param name="NodeName">要添加的节点</param>
        /// <param name="Value">值</param>
        /// <returns></returns>
        public bool AddNode(string ParentNode,string NodeName,string Value)
        {
            XmlElement XmlEle;

            if(!mHasFile)
            {
                return false;
            }

            try
            {
                XmlNode node;
                string[] NodeNames = ParentNode.Split('.');
                node = mXmlDoc.SelectSingleNode(NodeNames[0]);
                for(int i = 1;i < NodeNames.Length;i++)
                {
                    node = node.SelectSingleNode(NodeNames[i]);
                }

                XmlEle = mXmlDoc.CreateElement(NodeName);
                XmlEle.InnerText = Value;
                node.AppendChild(XmlEle);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }

        /// <summary>
        /// 添加一个节点以及值，节点的Attribute和Attribute的值
        /// </summary>
        /// <param name="ParentNode">节点 格式：中间加.分割 例:Group.Name</param>
        /// <param name="NodeName">需要添加的节点</param>
        /// <param name="Value">节点值</param>
        /// <param name="AttribName">节点的Attribute</param>
        /// <param name="Attrib">Attribute的值</param>
        /// <returns></returns>
        public bool AddNode(string ParentNode,string NodeName,string Value,string AttribName,string Attrib)
        {
            XmlElement XmlEle;

            if(!mHasFile)
            {
                return false;
            }

            try
            {
                XmlNode node;
                string[] NodeNames = ParentNode.Split('.');
                node = mXmlDoc.SelectSingleNode(NodeNames[0]);
                for(int i = 1;i < NodeNames.Length;i++)
                {
                    node = node.SelectSingleNode(NodeNames[i]);
                }

                XmlEle = mXmlDoc.CreateElement(NodeName);
                XmlEle.InnerText = Value;
                XmlEle.SetAttribute(AttribName,Attrib);
                node.AppendChild(XmlEle);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }

        /// <summary>
        /// 创建xml文件（包括一个根节点和描述）
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <param name="RootName">xml root node</param>
        /// <returns></returns>
        public bool CreateXml(string FileName,string RootName)
        {
            try
            {
                if(File.Exists(FileName))
                {
                    File.Delete(FileName);
                }

                XmlElement root = mXmlDoc.CreateElement(RootName);
                var dec = mXmlDoc.CreateXmlDeclaration("1.0","UTF-8","");
                mXmlDoc.AppendChild(dec);
                mXmlDoc.AppendChild(root);
                mXmlDoc.Save(FileName);

                mFileName = FileName;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }

        /// <summary>
        /// 删除指定节点
        /// </summary>
        /// <param name="ParentNode">父节点 格式：中间加.分割 例:Group.Name</param>
        /// <param name="NodeName">删除的节点</param>
        /// <returns></returns>
        public bool DeleteNode(string ParentNode,string NodeName)
        {
            if(!mHasFile)
            {
                return false;
            }

            try
            {
                XmlNode node;
                string[] NodeNames = ParentNode.Split('.');
                node = mXmlDoc.SelectSingleNode(NodeNames[0]);
                for(int i = 1;i < NodeNames.Length;i++)
                {
                    node = node.SelectSingleNode(NodeNames[i]);
                }

                XmlNodeList ChildNodes = node.ChildNodes;
                foreach(XmlNode ele in ChildNodes)
                {
                    if(ele.Name == NodeName)
                        node.RemoveChild(ele);
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }

        /// <summary>
        /// 获取父节点下的所有子节点
        /// </summary>
        /// <param name="ParentNodeName">父节点 格式：中间加.分割 例:Group.Name</param>
        /// <returns>所有子节点的名称的集合</returns>
        public List<string> GetListNode(string ParentNodeName)
        {
            List<string> ListNodeName = new List<string>();

            if(!mHasFile)
            {
                return null;
            }

            try
            {
                XmlNode Node;
                string[] NodeNames = ParentNodeName.Split('.');
                Node = mXmlDoc.SelectSingleNode(NodeNames[0]);
                for(int i = 1;i < NodeNames.Length;i++)
                {
                    Node = Node.SelectSingleNode(NodeNames[i]);
                }

                XmlNodeList ChildNodes = Node.ChildNodes;
                if(ChildNodes.Count > 0)
                {
                    foreach(XmlNode ele in ChildNodes)
                    {
                        ListNodeName.Add(ele.Name);
                    }
                }

                return ListNodeName;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取节点的Attribute值
        /// </summary>
        /// <param name="NodeName">节点 格式：中间加.分割 例:Group.Name</param>
        /// <param name="AttribName">Attribute的名称</param>
        /// <returns>节点的Attribute的值</returns>
        public string GetNodeAttribValue(string NodeName,string AttribName)
        {
            string Value = null;

            if(!mHasFile)
            {
                return null;
            }

            try
            {
                XmlNode Node;
                string[] NodeNames = NodeName.Split('.');
                Node = mXmlDoc.SelectSingleNode(NodeNames[0]);
                for(int i = 1;i < NodeNames.Length;i++)
                {
                    Node = Node.SelectSingleNode(NodeNames[i]);
                }

                if(Node != null)
                {
                    XmlElement ele = (XmlElement)Node;
                    Value = ele.Attributes[AttribName].Value;
                }

                return Value;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取父节点下所有相同子节点的值
        /// </summary>
        /// <param name="ParentNodeName">父节点 格式：中间加.分割 例:Group.Name</param>
        /// <param name="NodeName">子节点</param>
        /// <returns></returns>
        public List<string> GetNodeListValue(string ParentNodeName,string NodeName)
        {
            List<string> ValueList = new List<string>();
            if(!mHasFile)
            {
                return null;
            }

            try
            {
                XmlNode Node;
                string[] NodeNames = ParentNodeName.Split('.');
                Node = mXmlDoc.SelectSingleNode(NodeNames[0]);
                for(int i = 1;i < NodeNames.Length;i++)
                {
                    Node = Node.SelectSingleNode(NodeNames[i]);
                }

                XmlNodeList ChildNodes = Node.ChildNodes;
                if(ChildNodes.Count > 0)
                {
                    foreach(XmlNode ele in ChildNodes)
                    {
                        if(ele.Name == NodeName)
                        {
                            ValueList.Add(ele.InnerText);
                        }
                    }
                }

                return ValueList;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取节点的值
        /// </summary>
        /// <param name="NodeName">节点 格式：中间加.分割 例:Group.Name</param>
        /// <returns>节点的值。 如果出现异常，结果返回null</returns>
        public string GetNodeValue(string NodeName)
        {
            string Value = null;

            if(!mHasFile)
            {
                {
                    return null;
                }
            }

            try
            {
                XmlNode Node;
                string[] NodeNames = NodeName.Split('.');
                Node = mXmlDoc.SelectSingleNode(NodeNames[0]);
                for(int i = 1;i < NodeNames.Length;i++)
                {
                    Node = Node.SelectSingleNode(NodeNames[i]);
                }

                Value = Node.InnerText;
                return Value;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据xml文件的路径打开xml
        /// </summary>
        /// <param name="FileName">File name include path</param>
        /// <returns></returns>
        public bool OpenXml(string FileName)
        {
            try
            {
                if(!File.Exists(FileName))
                {
                    return false;
                }
                else
                {
                    mXmlDoc.Load(FileName);
                    mHasFile = true;
                }

                mFileName = FileName;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }

        /// <summary>
        /// 保存xml
        /// </summary>
        /// <returns></returns>
        public bool SaveXmlFile()
        {
            if(!mHasFile)
            {
                return false;
            }

            mXmlDoc.Save(mFileName);

            return true;
        }

        /// <summary>
        /// 更新节点值
        /// </summary>
        /// <param name="NodeName">节点 格式：中间加.分割 例:Group.Name</param>
        /// <param name="Value">节点值</param>
        /// <returns></returns>
        public bool UpdateNode(string NodeName,string Value)
        {
            if(!mHasFile)
            {
                return false;
            }

            try
            {
                XmlNode Node;
                string[] NodeNames = NodeName.Split('.');
                Node = mXmlDoc.SelectSingleNode(NodeNames[0]);
                for(int i = 1;i < NodeNames.Length;i++)
                {
                    Node = Node.SelectSingleNode(NodeNames[i]);
                }

                Node.InnerText = Value;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }

        /// <summary>
        /// 更新节点的Attribute值
        /// </summary>
        /// <param name="NodeName">节点 格式：中间加.分割 例:Group.Name</param>
        /// <param name="AttribName">节点的Attribute</param>
        /// <param name="Attrib">attribute的值</param>
        /// <returns></returns>
        public bool UpdateNodeAttribute(string NodeName,string AttribName,string Attrib)
        {
            if(!mHasFile)
            {
                return false;
            }

            try
            {
                XmlNode Node;
                string[] NodeNames = NodeName.Split('.');
                Node = mXmlDoc.SelectSingleNode(NodeNames[0]);
                for(int i = 1;i < NodeNames.Length;i++)
                {
                    Node = Node.SelectSingleNode(NodeNames[i]);
                }

                XmlElement ele = (XmlElement)Node;
                ele.SetAttribute(AttribName,Attrib);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }
    }
}