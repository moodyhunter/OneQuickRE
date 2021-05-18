namespace OneQuick.Core
{
    public static class Internal
    {
        public static uint ComputeStringHash(string s)
        {
            uint num = 0;
            if (s != null)
            {
                num = 2166136261u;
                for (int i = 0; i < s.Length; i++)
                {
                    num = (s[i] ^ num) * 16777619u;
                }
            }
            return num;
        }
    }
}
