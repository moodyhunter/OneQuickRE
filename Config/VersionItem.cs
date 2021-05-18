using System;
using System.Xml.Serialization;

namespace OneQuick.Config
{
    public class VersionItem
    {
        public string Tag { get; set; }

        public string LauncherName { get; set; }

        public bool IsPrerelease { get; set; }

        [XmlIgnore]
        public Version VersionObj { get; set; }

        public string Version
        {
            get => VersionObj.ToString();
            set => VersionObj = System.Version.Parse(value);
        }

        public int PushPercentage { get; set; }

        public string X86Url { get; set; }

        public string X64Url { get; set; }

        public string ChangeLog { get; set; }

        public static VersionItem GenerateCurrent()
        {
            return new VersionItem
            {
                VersionObj = G.MainWindow.VersionObj,
                IsPrerelease = G.MainWindow.Preference.CheckPrerelease,
                PushPercentage = G.MainWindow.Preference.PushNumber
            };
        }

        public bool MatchAndNewer(VersionItem current)
        {
            return (!IsPrerelease || current.IsPrerelease) && PushPercentage >= current.PushPercentage && VersionObj > current.VersionObj;
        }
    }
}
