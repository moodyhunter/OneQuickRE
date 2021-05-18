using System;

namespace OneQuick.Config
{
    public static class WheelOperationExt
    {
        public static string Translate(this WheelOperation wheelOperation)
        {
            return wheelOperation.ToString();
        }

        public static WheelOperation Parse(object obj)
        {
            return (WheelOperation)Enum.Parse(typeof(WheelOperation), obj.ToString());
        }
    }
}
