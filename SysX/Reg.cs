using Microsoft.Win32;

namespace OneQuick.SysX
{
    internal static class Reg
    {
        public static bool IsAutorun(string ProductName, string PathFullName, bool SetIfHasProductName = false)
        {
            object value = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run").GetValue(ProductName);
            if (value == null)
            {
                return false;
            }
            if ((string)value == PathFullName)
            {
                return true;
            }
            if (SetIfHasProductName)
            {
                SetAutorun(ProductName, PathFullName, true);
                return true;
            }
            return false;
        }

        public static void SetAutorun(string ProductName, string PathFullName, bool Autorun)
        {
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (Autorun)
                {
                    registryKey.SetValue(ProductName, PathFullName);
                }
                else
                {
                    registryKey.DeleteValue(ProductName);
                }
            }
            catch
            {
            }
        }
    }
}
