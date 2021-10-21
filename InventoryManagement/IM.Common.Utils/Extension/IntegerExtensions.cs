namespace IM.Common.Utils.Exception
{
    public static class IntegerExtensions
    {
        public static bool IsPositive(this int n)
        {
            return n >= 0;
        }

        public static bool IsNegative(this int n)
        {
            return n < 0;
        }

        public static bool IsGreaterThanZero(this int n)
        {
            return n > 0;
        }

        public static bool IsEqualToZero(this int n)
        {
            return n == 0;
        }
    }
}