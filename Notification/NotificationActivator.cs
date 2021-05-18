using System;
using System.Runtime.InteropServices;

namespace OneQuick.Notification
{
    public abstract class NotificationActivator : NotificationActivator.INotificationActivationCallback
    {
        public void Activate(string appUserModelId, string invokedArgs, NOTIFICATION_USER_INPUT_DATA[] data, uint dataCount)
        {
            OnActivated(invokedArgs, new NotificationUserInput(data), appUserModelId);
        }

        public abstract void OnActivated(string arguments, NotificationUserInput userInput, string appUserModelId);

        [Serializable]
        public struct NOTIFICATION_USER_INPUT_DATA
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Key;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string Value;
        }

        [Guid("53E31837-6600-4A81-9395-75CFFE746F94")]
        [ComVisible(true)]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport]
        public interface INotificationActivationCallback
        {
            void Activate([MarshalAs(UnmanagedType.LPWStr)][In] string appUserModelId, [MarshalAs(UnmanagedType.LPWStr)][In] string invokedArgs, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)][In] NOTIFICATION_USER_INPUT_DATA[] data, [MarshalAs(UnmanagedType.U4)][In] uint dataCount);
        }
    }
}
