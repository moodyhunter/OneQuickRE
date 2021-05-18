using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows;

namespace OneQuick
{
    internal static class BoolExt
    {
        public static Visibility ToVisibility(this bool bln)
        {
            return bln ? Visibility.Visible : Visibility.Collapsed;
        }
    }
    public static class Helper
    {
        public static IEnumerable<List<T>> SplitList<T>(IEnumerable<T> List, int nSize)
        {
            List<T> bigList = List.ToList();
            for (int i = 0; i < bigList.Count; i += nSize)
            {
                yield return bigList.GetRange(i, Math.Min(nSize, bigList.Count - i));
            }
            yield break;
        }

        public static string FolderReturn(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return folder;
        }

        public static bool PathEquals(string path1, string path2)
        {
            return Path.GetFullPath(path1).Equals(Path.GetFullPath(path2));
        }

        public static void CopyWholeFolder(string SourcePath, string DestinationPath, bool overWrite)
        {
            string[] array = Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories);
            for (int i = 0; i < array.Length; i++)
            {
                Directory.CreateDirectory(array[i].Replace(SourcePath, DestinationPath));
            }
            foreach (string text in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(text, text.Replace(SourcePath, DestinationPath), overWrite);
            }
        }

        public static string PercentageToText(int Percentage, int length = 10, char left = '.', char right = ' ')
        {
            int num = Percentage * length / 100;
            int count = length - num;
            return new string(left, num) + new string(right, count);
        }

        public static string GetExceptionContent(Exception e)
        {
            string text = e.Message + "\r\n" + e.StackTrace;
            if (e.InnerException != null)
            {
                return GetExceptionContent(e.InnerException) + "\r\n--- InnerException ---\r\n" + text;
            }
            return text;
        }

        public static string GetExceptionContentShort(Exception e)
        {
            if (e.InnerException != null)
            {
                return GetExceptionContentShort(e.InnerException);
            }
            return e.Message + "\r\n" + e.StackTrace;
        }

        public static string ProcessFatalUnhandledException(Exception e)
        {
            string text = GetExceptionContent(e) + Log.StackTraceLead + Environment.StackTrace;
            Log.Fatal(false, new string[] { text });
            return text;
        }

        public static string RandomString(int length)
        {
            return new string((from s in Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length) select s[random.Next(s.Length)]).ToArray());
        }

        public static string DictToString(Dictionary<string, string> dict, bool listShow = false)
        {
            string text;
            if (listShow)
            {
                text = dict.Aggregate("", (string sum, KeyValuePair<string, string> next) => string.Concat(new string[]
                {
                    sum,
                    "\n",
                    next.Key,
                    " = ",
                    next.Value
                }));
            }
            else
            {
                text = dict.Aggregate("", (string sum, KeyValuePair<string, string> next) => string.Concat(new string[]
                {
                    sum,
                    "&",
                    HttpUtility.UrlEncode(next.Key),
                    "=",
                    HttpUtility.UrlEncode(next.Value)
                }));
            }
            if (text.Length > 1)
            {
                text = text.Substring(1);
            }
            return text;
        }

        private static readonly Random random = new Random();
    }
}
