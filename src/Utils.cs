using System.Diagnostics.CodeAnalysis;
using System.Text;
namespace CSUtils
{
    public enum FillType
    {
        Left,
        Right,
        CenterLB,
        CenterRB,
    }

    public static class Utils
    {
        #region String

        public static string Copy(char c, int copies)
        {
            return new(Copy<char>(c, copies));
        }

        public static string Copy(string s, int copies)
        {
            return new(Copy(s.ToArray(), copies));
        }

        public static string InsertEscapeCharacters(string s, string prematch = @"\")
        {
            StringBuilder sb = new(s.Length);

            for (int i = 0; i < s.Length; ++i)
            {
                if (!MatchAll(s, prematch, i))
                {
                    sb.Append(s[i]);
                }
                else
                {
                    switch (s[i + prematch.Length])
                    {
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        case 'a':
                            sb.Append('\a');
                            break;
                        case 'b':
                            sb.Append('\b');
                            break;
                        default:
                            sb.Append(prematch);
                            continue;
                    }

                    i += prematch.Length;
                }
            }

            return sb.ToString();
        }

        public static IEnumerable<string> ColToStr<T>(IEnumerable<T> collection)
        {
            return from obj in collection
                   select obj.ToString();
        }

        public static (int, int) ReadDimensions<T>(T[,] data, bool rowMajor = true)
        {
            return rowMajor ? (data.GetLength(0), data.GetLength(1)) : (data.GetLength(1), data.GetLength(0));
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

        public static string[] OverflowSplitLines(string str, int maxLength = -1, int maxLines = -1)
        {
            StringBuilder sb = new();
            List<string> lines = new();

            for (int i = 0; i < str.Length && (maxLines < 0 || lines.Count < maxLines); ++i)
            {
                bool newLine = str[i] == '\n';
                if (!newLine)
                {
                    sb.Append(str[i]);
                }
                if (newLine || i == str.Length - 1 || (sb.Length == maxLength && str[i + 1] != '\n'))
                {
                    lines.Add(sb.ToString());
                    sb.Clear();
                }
            }

            return lines.ToArray();
        }

        public static string[] SplitLines(string str, int maxLength = -1, int maxLines = -1)
        {
            StringBuilder sb = new();
            List<string> lines = new();

            for (int i = 0; i < str.Length && (maxLines < 0 || lines.Count < maxLines); ++i)
            {
                if (str[i] == '\n')
                {
                    var s = sb.ToString();
                    lines.Add(maxLength >= 0 ? Shorten(s, maxLength) : s);
                    sb.Clear();

                }
                else
                {
                    sb.Append(str[i]);
                }
            }

            if (maxLines < 0 || lines.Count < maxLines)
            {
                lines.Add(sb.ToString());
            }

            return lines.ToArray();
        }

        #endregion

        #region StringExtensions

        public static string Substring(this string self, int[] range)
        {
            return range.Length == 2 ? self.Substring(range[0], range[1]) : "";
        }

        public static string Capitalize(this string self)
        {
            if (self.Length == 0)
            {
                return self;
            }

            var arr = new char[self.Length];
            arr[0] = char.ToUpper(self[0]);

            for (int i = 1; i < arr.Length; ++i)
            {
                arr[i] = char.ToLower(self[i]);
            }

            return new(arr);
        }

        public static string Reverse(this string self)
        {
            var arr = new char[self.Length];

            for (int i = 0; i < self.Length; ++i)
            {
                arr[i] = self[self.Length - i - 1];
            }

            return new(arr);
        }

        public static string TakeBetween(this string self, char value, int startIndex = 0)
        {
            int f = self.IndexOf(value, startIndex) + 1;
            return self[f..self.IndexOf(value, f)];
        }

        public static bool IsPalindrome(this string self)
        {
            for (int i = 0; i < self.Length / 2; ++i)
            {
                if (self[i] != self[self.Length - i - 1])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsNullOrEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }

        public static bool IsNullOrWhitespace(this string self)
        {
            return string.IsNullOrWhiteSpace(self);
        }

        #region RunLengthEncoding

        public static string RLCompress(this string self, char seperator = '§')
        {
            if (self.Length == 0)
            {
                return self;
            }

            StringBuilder sb = new(self.Length);
            char last = self[0];
            int count = 1;

            for (int i = 1; i <= self.Length; ++i)
            {
                char chr = i < self.Length ? self[i] : (char)(self[^1] + 1);

                if (chr == last)
                {
                    ++count;
                }
                else
                {
                    if (count > 1)
                    {
                        sb.Append(count);
                    }

                    sb.Append(char.IsDigit(last) ? $"{seperator}{last}{seperator}" : last.ToString());

                    last = chr;
                    count = 1;
                }
            }

            return sb.ToString();
        }

        public static string RLDecompress(this string self, char seperator = '§')
        {
            StringBuilder sb = new(self.Length);
            StringBuilder dsb = new();

            bool inDigit = false;

            for (int i = 0; i < self.Length; ++i)
            {
                if (self[i] == seperator)
                {
                    inDigit = !inDigit;
                    continue;
                }

                if (!inDigit && char.IsDigit(self[i]))
                {
                    dsb.Append(self[i]);
                }
                else if (dsb.Length > 0)
                {
                    sb.Append(Copy(self[i], Convert.ToInt32(dsb.ToString())));
                    dsb.Clear();
                }
                else
                {
                    sb.Append(self[i]);
                }
            }

            return sb.ToString();
        }

        #endregion

        #region Match

        public static bool MatchUntil(this string self, string search, int index = 0)
        {
            if (index < 0)
            {
                return false;
            }

            for (int i = 0; i < search.Length && i + index < self.Length; ++i)
            {
                if (self[i + index] != search[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool MatchAll(this string self, string search, int index = 0)
        {
            if (index < 0 || index + search.Length > self.Length)
            {
                return false;
            }

            for (int i = 0; i < search.Length; i++)
            {
                if (self[i + index] != search[i])
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region LengthManip

        public static string FTL(this string self, int length, char fill = ' ', FillType mode = FillType.Right)
        {
            int dif = length - self.Length;

            switch (mode)
            {
                case FillType.Left:
                    return Copy(fill, dif) + self;
                case FillType.Right:
                    return self + Copy(fill, dif);
                case FillType.CenterLB:
                case FillType.CenterRB:
                    int first = mode == FillType.CenterRB ? dif / 2 : DivUp(dif, 2);
                    return string.Join("", Copy(fill, first), self, Copy(fill, dif - first));
            }

            if (dif < 0)
            {
                return self[..length];
            }

            return self;
        }

        public static string FTL(this string self, int length, FillType mode)
        {
            return FTL(self, length, ' ', mode);
        }

        public static string Shorten(this string self, int length)
        {
            return self.Length <= length ? self : self[..length];
        }

        #endregion

        #region Replace

        public static string ReplaceRange(this string self, string replacement, int start, int count)
        {
            return string.Join("", self[..start], replacement, self[(start + count)..]);
        }

        public static string ReplaceRange(this string self, string replacement, int count)
        {
            return replacement + self[count..];
        }

        public static string ReplaceRange(this string self, string replacement, int[] range)
        {
            return range.Length == 2 ? ReplaceRange(self, replacement, range[0], range[1]) : self;
        }

        #endregion

        #endregion

        #region Array

        public static T[] Copy<T>(T value, int copies)
        {
            var arr = new T[copies];

            for (int i = 0; i < copies; ++i)
            {
                arr[i] = value;
            }

            return arr;
        }

        public static T[] Copy<T>(T[] value, int copies)
        {
            var arr = new T[copies * value.Length];

            for (int i = 0; i < arr.Length; i+=value.Length)
            {
                for (int j = 0; j < value.Length; ++j)
                {
                    arr[i + j] = value[j];
                }
            }

            return arr;
        }

        public static T[] TrimFromStart<T>(T[] arr, int trim)
        {
            if (trim >= arr.Length)
            {
                return Array.Empty<T>();
            }

            var newArr = new T[arr.Length - trim];

            for (int i = 0; i < newArr.Length; ++i)
            {
                newArr[i] = arr[i + trim];
            }

            return newArr; 
        }

        public static T[] TrimFirst<T>(T[] arr)
        {
            return TrimFromStart(arr, 1);
        }

        /// <summary>
        /// Checks whether the two arrays are equal value by value.
        /// </summary>
        public static bool ArraysMatch<T>(T[] a1, T[] a2)
            where T : IEquatable<T>
        {
            if (a1.Length != a2.Length)
            {
                return false;
            }

            for (int i = 0; i < a1.Length; ++i)
            {
                if (!a1[i].Equals(a2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static void PPrint<T>(IEnumerable<T> arr)
        {
            Console.Write(string.Join(',', arr));
        }

        public static void PPrintL<T>(IEnumerable<T> arr)
        {
            Console.WriteLine(string.Join(',', arr));
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
            {
                arr[i] = Read<T>();
            }

            return arr;
        }

        public static string ReadValid(Predicate<string> predicate)
        {
            while (true)
            {
                var str = Console.ReadLine() ?? "";

                if (predicate.Invoke(str))
                {
                    return str;
                }
            }
        }

        public static void ContinueAny(bool clear = true)
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);

            if (clear)
            {
                Console.Clear();
            }
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
                        {
                            Console.Clear();
                        }
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
            return (a % b == 0) ? (a / b) : (a / b) + 1;
        }

        public static int ClosestHigherMultiple(int a, int b)
        {
            var m = a % b;
            return m == 0 ? a : b - m + a;
        }

        public static bool IsPrime(int num)
        {
            if (num == 2)
            {
                return true;
            }

            if (num <= 1 || num % 2 == 0)
            {
                return false;
            }

            int end = CeilSqrt(num);

            for (int i = 3; i <= end; i += 2)
            {
                if (num % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        #region Sqrt

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

        #endregion

        #region Long

        public static int Mod(int a, int b)
        {
            var m = a % b;
            return m >= 0 ? m : m + b;
        }
        
        public static long Mod(long a, long b)
        {
            var m = a % b;
            return m >= 0 ? m : m + b;
        }

        #endregion

        #region Lerp

        public static float Lerp(float t, float min, float max)
        {
            return min + (max - min) * t;
        }

        public static double Lerp(double t, double min, double max)
        {
            return min + (max - min) * t;
        }

        public static float Delerp(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }

        public static double Delerp(double value, double min, double max)
        {
            return (value - min) / (max - min);
        }

        #endregion

        #region Angles

        public const float RAD_DEG_FACTOR = 180 / MathF.PI;

        public static float RadToDeg(float radians)
        {
            return radians * RAD_DEG_FACTOR;
        }

        public static float DegToRad(float degrees)
        {
            return degrees / RAD_DEG_FACTOR;
        }

        #endregion

        #region MaxMin

        public static T Max<T>(T a, T b)
            where T : IComparable<T>
        {
            return (a.CompareTo(b) >= 0) ? a : b;
        }

        public static T Min<T>(T a, T b)
            where T : IComparable<T>
        {
            return (a.CompareTo(b) <= 0) ? a : b;
        }

        public static T Clamp<T>(T value, T min, T max)
            where T : IComparable<T>
        {
            return Min(Max(value, min), max);
        }

        #endregion

        #region Middle

        /// <summary>
        /// Returns the middle number of the given values
        /// </summary>
        public static T Middle<T>(T a, T b, T c)
            where T : IComparable<T>
        {
            if (a.CompareTo(b) > 0)
            {
                if (a.CompareTo(c) < 0)
                {
                    return a;
                }
                return b.CompareTo(c) < 0 ? c : b;
            }
            if (b.CompareTo(c) < 0)
            {
                return b;
            }
            return a.CompareTo(c) < 0 ? c : a;
        }

        /// <summary>
        /// Reterns whether the given value is within the bounds.
        /// </summary>
        public static bool InMiddle<T>(T lower, T value, T upper)
            where T : IComparable<T>
        {
            if (lower.CompareTo(upper) > 0)
            {
                throw new ArgumentException("Lower bound cannot exceed upper bound.");
            }
            return lower.CompareTo(value) < 0 && upper.CompareTo(value) > 0;
        }

        #endregion

        #endregion

        #region Misc

        public static void Swap<T>(ref T a, ref T b)
        {
            (a, b) = (b, a);
        }

        #endregion
    }
}