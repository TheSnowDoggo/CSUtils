using System.Diagnostics.CodeAnalysis;
namespace CSUtils
{
    public static class Utils
    {
        #region String

        public static string Copy(char c, int copies)
        {
            var arr = new char[copies];
            for (int i = 0; i < copies; ++i)
                arr[i] = c;
            return new(arr);
        }

        public static string Copy(string str, int copies)
        {
            var arr = new char[str.Length * copies];
            for (int i = 0; i < arr.Length; i += str.Length)
            {
                for (int j = 0; j < str.Length; ++j)
                    arr[i + j] = str[j];
            }
            return new(arr);
        }

        public static string FTL(string s, int length, char fill = ' ', FitMode mode = FitMode.Post)
        {
            int dif = length - s.Length;
            if (dif > 0)
            {
                switch (mode)
                {
                    case FitMode.Post: return s + Copy(fill, dif);
                    case FitMode.Pre: return Copy(fill, dif) + s;
                    case FitMode.CenterL:
                    case FitMode.CenterR:
                        {
                            int first = mode == FitMode.CenterL ? dif / 2 : DivUp(dif, 2);
                            return string.Join(Copy(fill, first), s, Copy(fill, dif - first));
                        }
                }
            }
            if (dif < 0)
                return s[..length];
            return s;
        }

        public static string Reverse(string str)
        {
            var arr = new char[str.Length];
            for (int i = 0; i < str.Length; ++i)
                arr[i] = str[str.Length - i - 1];
            return new(arr);
        }

        public static bool IsPalindrome(string str)
        {
            for (int i = 0; i < str.Length / 2; ++i)
            {
                if (str[i] != str[str.Length - i - 1])
                    return false;
            }
            return true;
        }

        public static bool IsPrime(int num)
        {
            if (num == 2)
                return true;
            if (num <= 1 || num % 2 == 0)
                return false;
            int sqrt = CeilSqrt(num);
            for (int i = 3; i <= sqrt; i += 2)
            {
                if (num % i == 0)
                    return false;
            }
            return true;
        }

        public static bool Match(string str, string search, int index)
        {
            for (int i = 0; i < search.Length && i + index < str.Length; i++)
            {
                if (str[i + index] != search[i])
                    return false;
            }
            return true;
        }

        public static string TakeBetween(string str, char value, int startIndex = 0)
        {
            int f = str.IndexOf(value, startIndex) + 1;
            return str[f..str.IndexOf(value, f)];
        }

        public static string Replace(string str, string replacement, int start, int count)
        {
            return string.Join("", str[..start], replacement, str[(start + count)..]);
        }

        public static string Replace(string str, string replacement, int count)
        {
            return replacement + str[count..];
        }

        public static string Replace(string str, string replacement, int[] range)
        {
            return range.Length == 2 ? Replace(str, replacement, range[0], range[1]) : str;
        }

        public static string Substring(this string self, int[] r)
        {
            return r.Length == 2 ? self.Substring(r[0], r[1]) : "";
        }

        #endregion

        #region Input

        public static T? Read<T>()
        {
            string str = Console.ReadLine() ?? "";
            var res = Convert.ChangeType(str, typeof(T));
            return (T?)res;
        }

        public static bool TryRead<T>([MaybeNullWhen(false)] out T result)
        {
            result = Read<T>();
            return result != null;
        }

        public static void ContinueAny(bool clear = false, string msg = "Press any key to continue...")
        {
            if (msg != string.Empty)
                Console.WriteLine(msg);
            Console.ReadKey(true);
            if (clear)
                Console.Clear();
        }

        #endregion

        #region Math

        public static int DivUp(int a, int b)
        {
            return a % b == 0 ? a / b : (a / b) + 1;
        }

        private static int Sqrt(int num, bool roundUp)
        {
            int start = 0, end = num, mid = 0, lastMid = 0;
            while (start <= end)
            {
                lastMid = mid;
                mid = (start + end) >> 1;
                switch (((long)mid * mid) - num)
                {
                    case 0: return mid;
                    case > 0: end = mid - 1; break;
                    case < 0: start = mid + 1; break;
                }
            }
            return roundUp ? Math.Max(mid, lastMid) : Math.Min(mid, lastMid);
        }

        public static int CeilSqrt(int num)
        {
            return Sqrt(num, true);
        }

        public static int FloorSqrt(int num)
        {
            return Sqrt(num, false);
        }

        public static int Mod(int a, int b)
        {
            int mod = a % b;
            return a >= 0 ? mod : mod + b;
        }

        #endregion
    }
}