using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace OneQuick
{
    public static class XmlSerialization
    {
        public static string ToXmlText<T>(this T o, bool FailThrow = true) where T : new()
        {
            string result;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                using (StringWriter stringWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(stringWriter, o);
                    result = stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, new string[]
                {
                    "XmlSerialization.ToXmlText"
                });
                if (FailThrow)
                {
                    throw ex;
                }
                result = Helper.GetExceptionContent(ex);
            }
            return result;
        }

        public static void SaveToFile<T>(T o, string path, bool FailThrow = true) where T : new()
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                using (TextWriter textWriter = new StreamWriter(path))
                {
                    xmlSerializer.Serialize(textWriter, o);
                }
                Log.Info(new string[]
                {
                    "Write " + path
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, new string[]
                {
                    "XmlSerialization.ToXmlFile"
                });
                if (FailThrow)
                {
                    throw ex;
                }
            }
        }

        public static T FromXmlText<T>(string xml, bool FailCreateNew = false) where T : new()
        {
            T result;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                using (StringReader stringReader = new StringReader(xml))
                {
                    result = (T)xmlSerializer.Deserialize(stringReader);
                }
            }
            catch (Exception ex)
            {
                if (!FailCreateNew)
                {
                    Log.Error(ex, new string[]
                    {
                        "XmlSerialization.FromXmlText"
                    });
                    throw ex;
                }
                Log.Info(new string[]
                {
                    "FromXmlText: Parse Error, return new()"
                });
                result = Activator.CreateInstance<T>();
            }
            return result;
        }

        public static T FromXmlFile<T>(string path, bool FailCreateNew = false) where T : new()
        {
            T result;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                using (StreamReader streamReader = new StreamReader(path))
                {
                    XmlReader xmlReader = XmlReader.Create(streamReader);
                    T t = (T)xmlSerializer.Deserialize(xmlReader);
                    Log.Info(new string[]
                    {
                        "Load " + path
                    });
                    result = t;
                }
            }
            catch (Exception ex)
            {
                if (!FailCreateNew)
                {
                    Log.Error(ex, new string[]
                    {
                        "XmlSerialization.FromXmlFile"
                    });
                    throw ex;
                }
                Log.Info(new string[]
                {
                    "Cannot find xml " + path + ", return new()"
                });
                result = Activator.CreateInstance<T>();
            }
            return result;
        }
    }
}
