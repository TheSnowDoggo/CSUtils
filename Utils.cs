using System.Diagnostics.CodeAnalysis;
using System.Text;
namespace CSUtils
{
    public static class Utils
    {
        public enum FMode
        {
            Post,
            Pre,
            CenterL,
            CenterR
        }

        public enum GBOrigin
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
        }

        #region String

        public static string Copy(char c, int copies)
        {
            return new(CopyArr(c, copies));
        }

        public static string Copy(string str, int copies)
        {
            var arr = new char[str.Length * copies];
            for (int i = 0; i < arr.Length; i += str.Length)
                for (int j = 0; j < str.Length; ++j)
                    arr[i + j] = str[j];
            return new(arr);
        }

        public static string FTL(string s, int length, char fill = ' ', FMode mode = FMode.Post)
        {
            int dif = length - s.Length;
            if (dif > 0)
            {
                switch (mode)
                {
                    case FMode.Post: return s + Copy(fill, dif);
                    case FMode.Pre: return Copy(fill, dif) + s;
                    case FMode.CenterL:
                    case FMode.CenterR:
                        {
                            int first = mode == FMode.CenterL ? dif / 2 : DivUp(dif, 2);
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
                if (str[i] != str[str.Length - i - 1])
                    return false;
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
                if (num % i == 0)
                    return false;
            return true;
        }

        public static bool MatchUntil(string str, string search, int index = 0)
        {
            if (index < 0)
                return false;
            for (int i = 0; i < search.Length && i + index < str.Length; ++i)
                if (str[i + index] != search[i])
                    return false;
            return true;
        }

        public static bool MatchAll(string str, string search, int index = 0)
        {
            if (index < 0 || index + search.Length > str.Length)
                return false;
            for (int i = 0; i < search.Length; i++)
                if (str[i + index] != search[i])
                    return false;
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

        public static string BuildGrid(char[,] grid, GBOrigin origin = GBOrigin.TopLeft)
        {
            int xEnd = grid.GetLength(0) - 1;
            int yEnd = grid.GetLength(1) - 1;
            StringBuilder sb = new(xEnd * yEnd + yEnd);
            for (int y = 0; y <= yEnd; ++y)
            {
                for (int x = 0; x <= xEnd; ++x)
                {
                    sb.Append(origin switch
                    {
                        GBOrigin.TopLeft =>     grid[x       , y       ],
                        GBOrigin.TopRight =>    grid[xEnd - x, y       ],
                        GBOrigin.BottomLeft =>  grid[x       , yEnd - y],
                        GBOrigin.BottomRight => grid[xEnd - x, yEnd - y],
                        _ => throw new NotImplementedException("Unknown Origin?")
                    });
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static string Infill(IEnumerable<string> collection, string infill)
        {
            StringBuilder sb = new();
            bool first = true;
            foreach (var item in collection)
            {
                if (!first)
                    sb.Append(infill);
                else
                    first = false;
                sb.Append(item);
            }
            return sb.ToString();
        }

        public static string Build(IEnumerable<string> collection)
        {
            StringBuilder sb = new();
            foreach (var item in collection)
                sb.Append(item);
            return sb.ToString();
        }

        public static int[] GetIntegerRange(string str, int index)
        {
            if (index < 0)
                return Array.Empty<int>();
            int start = 0;
            bool found = false;
            for (int i = index; i < str.Length; ++i)
            {
                bool digit = char.IsDigit(str[i]);
                if (!digit && found)
                    return new[] { start, i };
                if (digit && !found)
                {
                    start = i;
                    found = false;
                }
            }
            return Array.Empty<int>();
        }

        public static int[] GetDecimalRange(string str, int index)
        {
            if (index < 0)
                return Array.Empty<int>();
            int start = 0;
            bool found = false, point = false;
            for (int i = index; i < str.Length; ++i)
            {
                bool digit = false;
                if (char.IsDigit(str[i]))
                    digit = true;
                else if (!point && str[i] == '.')
                {
                    digit = true;
                    point = true;
                }
                if (!digit && found)
                    return new[] { start, i };
                if (digit && !found)
                {
                    start = i;
                    found = false;
                }
            }
            return Array.Empty<int>();
        }

        public static string Substring(this string self, int[] range)
        {
            return range.Length == 2 ? self.Substring(range[0], range[1]) : "";
        }

        public static string Capitalize(this string self)
        {
            if (self.Length == 0)
                return self;
            var arr = new char[self.Length];
            arr[0] = char.ToUpper(self[0]);
            for (int i = 1; i < arr.Length; ++i)
                arr[i] = char.ToLower(self[i]);
            return new(arr);
        }

        public static string InsertEscapeCharacters(string str)
        {
            StringBuilder sb = new(str.Length);
            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] != '\\' || i == str.Length - 1)
                    sb.Append(str[i]);
                else
                {
                    switch (str[i + 1])
                    {
                        case 'n': sb.Append('\n');
                            break;
                        case 'r': sb.Append('\r');
                            break;
                        case 't': sb.Append('\t');
                            break;
                        case 'a': sb.Append('\a');
                            break;
                        case 'b': sb.Append('\b');
                            break;
                        default: sb.Append('\\');
                            continue;
                    }
                    ++i;
                }
            }
            return sb.ToString();
        }

        #endregion

        #region RunLengthEncoding

        public static string RLCompress(string str, char seperator = '§')
        {
            if (str.Length == 0)
                return str;

            StringBuilder sb = new(str.Length);
            char last = str[0];
            int count = 1;
            for (int i = 1; i <= str.Length; ++i)
            {
                char chr = i < str.Length ? str[i] : (char)(str[^1] + 1);
                if (chr == last)
                    ++count;
                else
                {
                    if (count > 1)
                        sb.Append(count);
                    sb.Append(char.IsDigit(last) ? $"{seperator}{last}{seperator}" : last.ToString());

                    last = chr;
                    count = 1;
                }
            }
            return sb.ToString();
        }

        public static string RLDecompress(string str, char seperator = '§')
        {
            StringBuilder sb = new(str.Length);
            StringBuilder dsb = new();
            bool inDigit = false;
            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] == seperator)
                {
                    inDigit = !inDigit;
                    continue;
                }

                if (!inDigit && char.IsDigit(str[i]))
                    dsb.Append(str[i]);
                else if (dsb.Length > 0)
                {
                    sb.Append(Copy(str[i], Convert.ToInt32(dsb.ToString())));
                    dsb.Clear();
                }
                else
                {
                    sb.Append(str[i]);
                }
            }
            return sb.ToString();
        }

        #endregion

        #region Array

        public static T[] CopyArr<T>(T value, int copies)
        {
            var arr = new T[copies];
            for (int i = 0; i < copies; ++i)
                arr[i] = value;
            return arr;
        }

        public static T[] CopyArr<T>(T[] value, int copies)
        {
            var arr = new T[copies * value.Length];
            for (int i = 0; i < arr.Length; i+=value.Length)
                for (int j = 0; j < value.Length; ++j)
                    arr[i + j] = value[j];
            return arr;
        }

        public static T[] TrimFromStart<T>(T[] arr, int trim)
        {
            if (trim >= arr.Length)
                return Array.Empty<T>();
            var newArr = new T[arr.Length - trim];
            for (int i = 0; i < newArr.Length; ++i)
                newArr[i] = arr[i + trim];
            return newArr; 
        }

        public static T[] TrimFirst<T>(T[] arr)
        {
            return TrimFromStart(arr, 1);
        }

        #endregion

        #region Input

        public static T? Read<T>()
        {
            var str = Console.ReadLine() ?? "";
            var res = Convert.ChangeType(str, typeof(T));
            return (T?)res;
        }

        public static T?[] Read<T>(int count)
        {
            var arr = new T?[count];
            for (int i = 0; i < count; ++i)
                arr[i] = Read<T>();
            return arr;
        }
 
        public static bool TryRead<T>([MaybeNullWhen(false)] out T result)
        {
            result = Read<T>();
            return result != null;
        }

        public static T ReadValid<T>(string? msg = null)
        {
            T? res;
            do
            {
                Console.Write(msg);
            } while (!TryRead<T>(out res));
            return res;
        }

        public static void ContinueAny(bool clear = false, string msg = "Press any key to continue...")
        {
            if (msg.Length != 0)
                Console.WriteLine(msg);
            Console.ReadKey(true);
            if (clear)
                Console.Clear();
        }

        public static bool IBoolPrompt(bool display = false, 
            ConsoleKey tKey = ConsoleKey.Y, 
            ConsoleKey fKey = ConsoleKey.N)
        {
            while (true)
            {
                var cki = Console.ReadKey(true);
                if (cki.Key == tKey || cki.Key == fKey)
                {
                    if (display)
                        Console.WriteLine(cki.KeyChar);
                    return cki.Key == tKey;
                }
            }
        }

        public static bool BoolPrompt(string? msg = null)
        {
            if (msg != null)
                Console.Write(msg);
            while (true)
            {
                var inp = Console.ReadLine() ?? "";
                switch (inp.ToLower())
                {
                    case "y":
                    case "yes":
                    case "true":
                    case "1":
                        return true;
                    case "n":
                    case "no":
                    case "false":
                    case "0":
                        return false;
                }
            }
        }

        public static int Prompt(string msg, string[] options)
        {
            StringBuilder sb = new();
            if (msg != null)
                sb.AppendLine(msg);
            for (int i = 0; i < options.Length; ++i)
                sb.AppendLine($">[{i}] {options[i]}");
            Console.Write(sb.ToString());
            while (true)
            {
                var inp = Console.ReadLine() ?? "";
                if (int.TryParse(inp, out var num) 
                    && num >= 0 && num < options.Length)
                {
                    return num;
                }
            }
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
            var mod = a % b;
            return mod >= 0 ? mod : mod + b;
        }

        #endregion
    }
}