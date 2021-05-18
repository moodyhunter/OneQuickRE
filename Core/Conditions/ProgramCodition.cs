using OneQuick.SysX;
using System;
using System.IO;

namespace OneQuick.Core.Conditions
{
    public class ProgramCodition : Condition
    {
        public string FileName { get; set; }

        public string FullPath { get; set; }

        public string Title { get; set; }

        public string ClassName { get; set; }

        public ProgramCodition()
        {
        }

        public ProgramCodition(string filename)
        {
            FileName = filename;
        }

        protected override bool IsFit()
        {
            IntPtr foregroundWindow = Win.GetForegroundWindow();
            return FitbyHwnd(foregroundWindow);
        }

        public bool FitbyHwnd(IntPtr handle)
        {
            string text = Win.GetWindowProcFileName(new IntPtr?(handle)).ToLower();
            string b = Path.GetFileName(text).ToLower();
            return (FileName == null || !(FileName.ToLower() != b)) && (FullPath == null || !(FullPath.ToLower() != text));
        }

        public override string ToString()
        {
            if (Title != null)
            {
                return Title;
            }
            if (FileName != null)
            {
                return FileName;
            }
            if (FullPath != null)
            {
                return FullPath;
            }
            if (ClassName != null)
            {
                return ClassName;
            }
            return "<Program:Empty>";
        }
    }
}
