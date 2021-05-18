using System;
using System.IO;
using System.Windows;

namespace OneQuick
{
    internal static class Log
    {
        public static string StackTraceLead => "\r\n--- StackTrace ---\r\n";

        public static event Action<string> Listener;

        public static bool InfoSwitch { get; set; } = true;

        public static bool ErrorSwitch { get; set; } = true;

        public static bool VerboseSwitch { get; set; } = false;

        public static bool ThrowException { get; set; } = false;

        public static bool WriteLogToConsoleInDebug { get; set; } = false;

        public static bool TraceEventsSwitch => G.LogTraceEvents;

        static Log()
        {
            Application.Current.Exit += Current_Exit;
        }

        private static void Current_Exit(object sender, ExitEventArgs e)
        {
            _write("----------------------------------------------");
        }

        private static string JoinContents(params string[] contents)
        {
            return string.Join(", ", contents);
        }

        private static void WriteLog(string type, params string[] contents)
        {
            string text = string.Format("[0x{0}][{1}][{2}] {3}", new object[]
            {
                ModuleHandle,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                type,
                JoinContents(contents)
            });
            Listener?.Invoke(text);
            _write(text);
        }

        private static void _write(string str)
        {
            if (FirstRun)
            {
                FirstRun = false;
                _write("==============================================");
            }
            try
            {
                File.AppendAllText(G.LogFilePath, str + "\r\n");
            }
            catch (Exception)
            {
            }
        }

        public static bool Trace(params string[] contents)
        {
            if (TraceEventsSwitch)
            {
                string obj = JoinContents(contents);
                Listener?.Invoke(obj);
                return true;
            }
            return false;
        }

        public static bool Verbose(params string[] contents)
        {
            if (VerboseSwitch)
            {
                WriteLog("VERBOSE", contents);
                return true;
            }
            return false;
        }

        public static bool Info(params string[] contents)
        {
            if (InfoSwitch)
            {
                WriteLog("INFO", contents);
                return true;
            }
            return false;
        }

        public static bool Error(params string[] contents)
        {
            if (ErrorSwitch)
            {
                string text = JoinContents(contents) + StackTraceLead + Environment.StackTrace;
                WriteLog("ERROR", new string[]
                {
                    text
                });
                return true;
            }
            return false;
        }

        public static bool Error(Exception ex, params string[] contents)
        {
            string text = string.Concat(new string[]
            {
                JoinContents(contents),
                "\r\n",
                Helper.GetExceptionContent(ex),
                StackTraceLead,
                Environment.StackTrace
            });
            if (ErrorSwitch)
            {
                WriteLog("ERROR", new string[]
                {
                    text
                });
            }
            return ErrorSwitch;
        }

        public static void Fatal(bool throwExp, params string[] contents)
        {
            WriteLog("Fatal", contents);
            if (throwExp)
            {
                throw new Exception(JoinContents(contents));
            }
        }

        public static bool Debug(Exception ex, params string[] contents)
        {
            return false;
        }

        public static bool Debug(params string[] contents)
        {
            return false;
        }

        public static void CheckLogFileSize()
        {
            if (File.Exists(G.LogFilePath) && new FileInfo(G.LogFilePath).Length > 1048576L)
            {
                try
                {
                    string text = File.ReadAllText(G.LogFilePath);
                    int num = text.Length - 524288;
                    num = Math.Min(text.Length, num);
                    File.WriteAllText(G.LogFilePath, text.Substring(num) + "\n<LogFileTruncate>\n");
                }
                catch (Exception e)
                {
                    Helper.GetExceptionContent(e);
                }
            }
        }

        private static readonly string ModuleHandle = G.ModuleHandle.ToString("x");

        private static bool FirstRun = true;
    }
}
