using System.Diagnostics.CodeAnalysis;
using System.Text;
namespace CSUtils
{
    public static class Utils
    {
        public enum FMode
        {
            Left = 1,
            Right = 2,
            Center = 4,
        }

        #region String

        public static string Copy(char c, int copies)
        {
            return new(CopyArr(c, copies));
        }

        public static string Copy(string s, int copies)
        {
            var arr = new char[s.Length * copies];
            for (int i = 0; i < arr.Length; i += s.Length)
                for (int j = 0; j < s.Length; ++j)
                    arr[i + j] = s[j];
            return new(arr);
        }

        public static string FTL(string s, int length, char fill = ' ', FMode mode = FMode.Right)
        {
            int dif = length - s.Length;
            if (dif < 0)
                return s[..length];
            if (dif == 0)
                return s;
            bool right = (mode | FMode.Right) == FMode.Right;
            if ((mode | FMode.Center) == FMode.Center)
            {
                int first = !right ? dif / 2 : DivUp(dif, 2);
                return string.Join(Copy(fill, first), s, Copy(fill, dif - first));
            }
            else
            {
                return right ? s + Copy(fill, dif) : Copy(fill, dif) + s;
            }
        }

        public static string Shorten(string s, int length)
        {
            return s.Length <= length ? s : s[..length];
        }

        public static string Reverse(string s)
        {
            var arr = new char[s.Length];
            for (int i = 0; i < s.Length; ++i)
                arr[i] = s[s.Length - i - 1];
            return new(arr);
        }

        public static bool IsPalindrome(string s)
        {
            for (int i = 0; i < s.Length / 2; ++i)
                if (s[i] != s[s.Length - i - 1])
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

        public static bool MatchUntil(string s, string search, int index = 0)
        {
            if (index < 0)
                return false;
            for (int i = 0; i < search.Length && i + index < s.Length; ++i)
                if (s[i + index] != search[i])
                    return false;
            return true;
        }
        
        public static bool MatchAll(string s, string search, int index = 0)
        {
            if (index < 0 || index + search.Length > s.Length)
                return false;
            for (int i = 0; i < search.Length; i++)
                if (s[i + index] != search[i])
                    return false;
            return true;
        }

        public static string TakeBetween(string s, char value, int startIndex = 0)
        {
            int f = s.IndexOf(value, startIndex) + 1;
            return s[f..s.IndexOf(value, f)];
        }

        public static string Replace(string s, string replacement, int start, int count)
        {
            return string.Join("", s[..start], replacement, s[(start + count)..]);
        }

        public static string Replace(string s, string replacement, int count)
        {
            return replacement + s[count..];
        }

        public static string Replace(string s, string replacement, int[] range)
        {
            return range.Length == 2 ? Replace(s, replacement, range[0], range[1]) : s;
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

        public static string Infill<T>(IEnumerable<T> collection, string infill)
        {
            return Infill(ColToStr(collection), infill);
        }

        public static void PPrint<T>(IEnumerable<T> arr)
        {
            Console.WriteLine(Infill(arr, ","));
        }

        public static string Build(IEnumerable<string> collection)
        {
            StringBuilder sb = new();
            foreach (var item in collection)
                sb.Append(item);
            return sb.ToString();
        }

        public static string Build<T>(IEnumerable<T> collection)
        {
            return Build(ColToStr(collection));
        }

        public static (int, int) ReadDimensions<T>(T[,] data, bool rowMajor = true)
        {
            var d = (data.GetLength(0), data.GetLength(1));
            if (!rowMajor)
                (d.Item1, d.Item2) = (d.Item2, d.Item1);
            return d;
        }

        public static string BuildGridFlat<T>(T[,] data, bool rowMajor = true)
        {
            (int rows, int columns) = ReadDimensions(data, rowMajor);
            StringBuilder sb = new("[ { ");
            for (int row = 0; row < rows; ++row)
            {
                if (row != 0)
                    sb.Append(" }, { ");
                for (int col = 0; col < columns; ++col)
                {
                    if (col != 0)
                        sb.Append(", ");
                    sb.Append(rowMajor ? data[row, col] : data[col, row]);
                }
            }
            sb.Append(" } ]");
            return sb.ToString();
        }

        public static string BuildGrid2D<T>(T[,] data, bool rowMajor = true)
        {
            (int rows, int columns) = ReadDimensions(data, rowMajor);
            StringBuilder sb = new();
            for (int row = 0; row < rows; ++row)
            {
                if (row != 0)
                    sb.AppendLine();
                sb.Append("{ ");
                for (int col = 0; col < columns; ++col)
                {
                    if (col != 0)
                        sb.Append(", ");
                    sb.Append(rowMajor ? data[row, col] : data[col, row]);
                }
                sb.Append(" }");
            }
            return sb.ToString();
        }

        public static IEnumerable<string> ColToStr<T>(IEnumerable<T> collection)
        {
            return from obj in collection
                   select obj.ToString();
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

        public static string InsertEscapeCharacters(string s, string prematch = @"\")
        {
            StringBuilder sb = new(s.Length);
            for (int i = 0; i < s.Length; ++i)
            {
                if (!MatchAll(s, prematch, i))
                    sb.Append(s[i]);
                else
                {
                    switch (s[i + prematch.Length])
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
                        default: sb.Append(prematch);
                            continue;
                    }
                    i+=prematch.Length;
                }
            }
            return sb.ToString();
        }

        #endregion

        #region RunLengthEncoding

        public static string RLCompress(string s, char seperator = '§')
        {
            if (s.Length == 0)
                return s;

            StringBuilder sb = new(s.Length);
            char last = s[0];
            int count = 1;
            for (int i = 1; i <= s.Length; ++i)
            {
                char chr = i < s.Length ? s[i] : (char)(s[^1] + 1);
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

        public static string RLDecompress(string s, char seperator = '§')
        {
            StringBuilder sb = new(s.Length);
            StringBuilder dsb = new();
            bool inDigit = false;
            for (int i = 0; i < s.Length; ++i)
            {
                if (s[i] == seperator)
                {
                    inDigit = !inDigit;
                    continue;
                }

                if (!inDigit && char.IsDigit(s[i]))
                    dsb.Append(s[i]);
                else if (dsb.Length > 0)
                {
                    sb.Append(Copy(s[i], Convert.ToInt32(dsb.ToString())));
                    dsb.Clear();
                }
                else
                {
                    sb.Append(s[i]);
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
            var s = Console.ReadLine() ?? "";
            var res = Convert.ChangeType(s, typeof(T));
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

        public static void ContinueAny(bool clear = true)
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            if (clear)
                Console.Clear();
        }

        public static bool BoolPrompt()
        {
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

        public static bool BoolPrompt(string msg)
        {
            Console.WriteLine(msg);
            return BoolPrompt();
        }

        public static int SelectionPrompt(int count, Func<int, string> render, bool clear = true)
        {
            Console.CursorVisible = false;

            var (Left, Top) = Console.GetCursorPosition();

            int sel = 0;
            while (true)
            {
                Console.SetCursorPosition(Left, Top);
                for (int i = 0; i < count; ++i)
                {
                    var s = render.Invoke(i);
                    if (i == sel)
                    {
                        SwapConsoleColor();
                        Console.WriteLine(s);
                        SwapConsoleColor();
                    }
                    else
                    {
                        Console.WriteLine(s);
                    }
                }

                var cki = Console.ReadKey(true);

                switch (cki.Key)
                {
                    case ConsoleKey.UpArrow:
                        sel = Mod(sel - 1, count);
                        break;
                    case ConsoleKey.DownArrow:
                        sel = Mod(sel + 1, count);
                        break;
                    case ConsoleKey.Enter:
                        if (clear)
                            Console.Clear();
                        Console.CursorVisible = true;
                        return sel;
                }
            }
        }

        public static int SelectionPrompt(string[] options, bool clear = true)
        {
            int longest = options.Max(x => x.Length);
            return SelectionPrompt(options.Length, i => FTL(options[i], longest), clear);
        }

        public static void SwapConsoleColor()
        {
            (Console.ForegroundColor, Console.BackgroundColor)
                = (Console.BackgroundColor, Console.ForegroundColor);
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