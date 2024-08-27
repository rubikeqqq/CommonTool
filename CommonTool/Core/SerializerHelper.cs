#region USING

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

#endregion

namespace CommonTool.Core
{
    /// <summary>
    /// 提供了3种方式的简单序列化方案
    /// </summary>
    /// <remarks>
    /// <list type="number">
    /// <item>Binary</item>
    /// <item>XML</item>
    /// <item>Json</item>
    /// </list>
    /// </remarks>
    public class SerializerHelper
    {
        #region 二进制序列化

        /// <summary>
        /// 从文件反序列化
        /// </summary>
        /// <remarks>
        /// <list type="table">
        /// <item>当文件不存在时，抛出文件不存在的异常</item>
        /// <item>当反序列化失败时，抛出异常原因</item>
        /// </list>
        /// </remarks>
        /// <exception cref="Exception"></exception>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="filePath">文件路径</param>
        /// <returns>具体类</returns>
        public static T DeSerializeFromBinary<T>(string filePath)
        {
            if(!File.Exists(filePath))
            {
                throw new Exception("序列化文件不存在,请检查！");
            }

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                T value;
                using(Stream destream = new FileStream(filePath,FileMode.Open,FileAccess.Read,FileShare.None))
                {
                    value = (T)formatter.Deserialize(destream);
                }

                return value;
            }
            catch(Exception ex)
            {
                throw new Exception("binary反序列化失败!\r\n" + ex.Message);
            }
        }

        /// <summary>
        /// 获取对象序列化的二进制版本
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="item">对象实体</param>
        /// <returns>二进制文件 如果对象实体为Null,则结果返回Null</returns>
        public static byte[] GetBytes<T>(T item)
        {
            if(item == null) return null;
            try
            {
                using(MemoryStream serializationStream = new MemoryStream())
                {
                    new BinaryFormatter().Serialize(serializationStream,item);
                    serializationStream.Position = 0L;
                    byte[] buffer = new byte[serializationStream.Length];
                    serializationStream.Read(buffer,0,buffer.Length);
                    serializationStream.Close();
                    return buffer;
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 从已序列化数据中(Byte[])获取对象实体
        /// </summary>
        /// <typeparam name="T">要返回的数据类型</typeparam>
        /// <param name="binData">二进制数据</param>
        /// <returns>对象实体</returns>
        public static T GetObject<T>(byte[] binData)
        {
            if(binData == null) return default(T);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using(MemoryStream serializationStream = new MemoryStream(binData))
                {
                    return (T)formatter.Deserialize(serializationStream);
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 二进制序列化---必须先将序列化的类标记为可序列化[serializable]
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="o">类型参数</param>
        /// <param name="filePath">路径</param>
        public static bool SerializeToBinary<T>(T type,string filePath)
        {
            if(!File.Exists(filePath))
            {
                FileHelper.CreateFile(filePath);
            }

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using(Stream stream = new FileStream
                    (filePath,
                    FileMode.OpenOrCreate,
                    FileAccess.Write,
                    FileShare.None))
                {
                    formatter.Serialize(stream,type);
                }
            }
            catch(Exception ex)
            {
                throw new Exception("binary序列化失败!" + ex.Message);
            }

            return true;
        }

        /// <summary>
        /// 深度拷贝对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepClone<T>(T obj)
        {
            using(Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream,obj);
                objectStream.Seek(0,SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }

        #endregion 二进制序列化

        #region XML序列化

        /******************************************************************************************
         *
         *       序列化数据只包含数据本身和类的结构。 类型标识和程序集信息不包括在内。
         *
         *       只能序列化公共属性和字段。 属性必须具有公共访问器（get 和 set 方法）。
         *       如果必须序列化非公共数据，请使用 DataContractSerializer 类而不使用 XML 序列化。
         *
         *       类必须具有无参数构造函数才能被 XmlSerializer 序列化。
         *
         *
         *******************************************************************************************/

        /// <summary>
        /// 从XML文件反序列化为类
        /// </summary>
        /// <remarks>
        /// <list type="table">
        /// <item>当文件不存在时，抛出文件不存在的异常</item>
        /// <item>当反序列化失败时，抛出异常原因</item>
        /// </list>
        /// </remarks>
        /// <exception cref="Exception"></exception>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="filePath">xml路径</param>
        /// <returns>具体类</returns>
        public static T DeSerializeFromXml<T>(string filePath)
        {
            if(!File.Exists(filePath))
            {
                throw new Exception("序列化文件不存在,请检查！");
            }

            try
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                T value;
                using(StreamReader sr = new StreamReader(filePath))
                {
                    value = (T)formatter.Deserialize(sr);
                }
                return value;
            }
            catch(Exception ex)
            {
                throw new Exception("xml反序列化失败!" + ex.Message);
            }
        }

        /// <summary>
        /// 文本XML反序列化
        /// </summary>
        /// <typeparam name="T">反序列化类型</typeparam>
        /// <param name="str">字符串序列</param>
        /// <returns>类型对象</returns>
        public static T FromXml<T>(string str)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using(XmlReader reader = new XmlTextReader(new StringReader(str)))
                {
                    return (T)serializer.Deserialize(reader);
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 从已序列化数据(XmlDocument)中获取对象实体
        /// </summary>
        /// <typeparam name="T">要返回的数据类型</typeparam>
        /// <param name="xmlDoc">已序列化的文档对象</param>
        /// <returns>对象实体</returns>
        public static T GetObject<T>(XmlDocument xmlDoc)
        {
            if(xmlDoc == null)
            {
                return default(T);
            }

            try
            {
                XmlNodeReader xmlReader = new XmlNodeReader(xmlDoc.DocumentElement);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(xmlReader);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取对象序列化的XmlDocument版本
        /// </summary>
        /// <param name="pObj">对象实体</param>
        /// <returns>如果对象实体为Null，则结果返回Null</returns>
        public static XmlDocument GetXmlDoc(object pObj)
        {
            if(pObj == null)
            {
                return null;
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(pObj.GetType());
                StringBuilder sb = new StringBuilder();
                StringWriter writer = new StringWriter(sb);
                serializer.Serialize((TextWriter)writer,pObj);
                XmlDocument document = new XmlDocument();
                document.LoadXml(sb.ToString());
                writer.Close();
                return document;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 序列化到XML文件
        /// </summary>
        /// <remarks>xml序列化时泛型类T的访问级别有限制</remarks>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="o">类型参数</param>
        /// <param name="filePath">路径</param>
        public static bool SerializeToXml<T>(T o,string filePath)
        {
            if(!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            try
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                using(StreamWriter sw = new StreamWriter(filePath,false))
                {
                    formatter.Serialize(sw,o);
                }
            }
            catch(Exception ex)
            {
                throw new Exception("xml序列化失败!" + ex.Message);
            }

            return true;
        }

        /// <summary>
        /// 文本化XML序列化
        /// </summary>
        /// <typeparam name="T">序列化的类型</typeparam>
        /// <param name="item">序列化对象</param>
        /// <returns>序列化得到的文本</returns>
        public static string ToXml<T>(T item)
        {
            XmlSerializer serializer = new XmlSerializer(item.GetType());
            StringBuilder sb = new StringBuilder();
            try
            {
                using(XmlWriter writer = XmlWriter.Create(sb))
                {
                    serializer.Serialize(writer,item);
                    return sb.ToString();
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion XML序列化

        #region Json序列化

        /// <summary>
        /// 从Json文件中反序列化到泛型类
        /// </summary>
        /// <remarks>
        /// <list type="table">
        /// <item>当文件不存在时，抛出文件不存在的异常</item>
        /// <item>当反序列化失败时，抛出异常原因</item>
        /// </list>
        /// </remarks>
        /// <typeparam name="T">泛型类T</typeparam>
        /// <param name="file">Json文件</param>
        /// <returns>泛型类</returns>
        public static T DeSerializeFromJson<T>(string filePath)
        {
            if(!File.Exists(filePath))
            {
                throw new Exception("序列化文件不存在,请检查！");
            }

            try
            {
                string str;
                using(TextReader reader = new StreamReader(filePath))
                {
                    str = reader.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<T>(str);
            }
            catch(Exception ex)
            {
                throw new Exception("json反序列化失败!" + ex.Message);
            }
        }

        /// <summary>
        /// 序列化泛型类到指定文件路径
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="path">保存的文件路径</param>
        /// <param name="t">指定序列化的类</param>
        public static bool SerializeToJson<T>(T t,string filePath)
        {
            if(!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            try
            {
                string str = JsonConvert.SerializeObject(t);
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                using(FileStream fs = new FileStream(filePath,FileMode.OpenOrCreate,FileAccess.Write))
                {
                    fs.Write(bytes,0,bytes.Length);
                }
            }
            catch(Exception ex)
            {
                throw new Exception("json序列化失败!" + ex.Message);
            }

            return true;
        }

        #endregion Json序列化
    }
}