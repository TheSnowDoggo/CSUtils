namespace CSUtils
{
    public static class TagUtils
    {
        #region CharTags

        public static int SIndexOf(string str, char tag, int index = 0)
        {
            if (index < 0)
                return -1;
            for (int i = index; i < str.Length - 2; ++i)
            {
                if (str[i] == '<' && str[i + 1] == tag && str[i + 2] == '>')
                    return i;
            }
            return -1;
        }

        public static int EIndexOf(string str, char tag, int index = 0)
        {
            if (index < 0)
                return -1;
            for (int i = index; i < str.Length - 3; ++i)
            {
                if (str[i] == '<' && (str[i + 1] == '/' || str[i + 1] == '\\') 
                    && str[i + 2] == tag && str[i + 3] == '>')
                {
                    return i;
                }
            }
            return -1;
        }

        public static int FindIndex(string str, char tag, int count, int index = 0)
        {
            int start = SIndexOf(str, tag, index);
            for (int i = 1; i < count && start != -1; ++i)
                start = SIndexOf(str, tag, start + 3);
            return start;
        }

        public static int[] Range(string str, char tag, int index = 0)
        {
            int start = SIndexOf(str, tag, index);
            if (start != -1)
            {
                start += 3;
                int end = EIndexOf(str, tag, start);
                if (end != -1)
                    return new[] { start, end - start };
            }
            return Array.Empty<int>();
        }

        public static int[] LongRange(string str, char tag, int index = 0)
        {
            int start = SIndexOf(str, tag, index);
            if (start != -1)
            {
                int end = EIndexOf(str, tag, start + 3);
                if (end != -1)
                    return new[] { start, end + 4 - start };
            }
            return Array.Empty<int>();
        }

        public static int[] ExpandRange(int[] range)
        {
            if (range.Length != 2)
                return Array.Empty<int>();
            return new[] { range[0] - 3, range[1] + 7 };
        }

        public static string ReadTag(string str, char tag, int index = 0)
        {
            var r = Range(str, tag, index);
            if (r.Length == 0)
                return "";
            return str.Substring(r[0], r[1]);
        }

        public static string WriteTag(string str, char tag, string write, int index = 0)
        {
            var r = Range(str, tag, index);
            if (r.Length == 0)
                return str;
            return string.Join(str[..r[0]], write, str[(r[0] + r[1])..]);
        }

        public static IEnumerable<int[]> EnumerateTagRanges(string str, char tag, int index = 0)
        {
            var r = Range(str, tag, index);
            while (r.Length != 0)
            {
                yield return r;
                r = Range(str, tag, r[0]);
            }
        }

        public static int[][] ReadTagsRanges(string str, char tag, int index = 0)
        {
            return EnumerateTagRanges(str, tag, index).ToArray();
        }

        public static IEnumerable<string> EnumerateTags(string str, char tag, int index = 0)
        {
            foreach (var r in EnumerateTagRanges(str, tag, index))
                yield return str.Substring(r[0], r[1]);
        }

        public static string[] ReadTags(string str, char tag, int index = 0)
        {
            return EnumerateTags(str, tag, index).ToArray();
        }

        #endregion

        #region StringTags

        public static int SIndexOf(string str, string tag, int index = 0)
        {
            if (index < 0)
                return -1;
            int end = str.Length - tag.Length - 2;
            for (int i = index; i < end; ++i)
            {
                if (str[i] == '<' && str[i + tag.Length + 1] == '>' &&
                    Utils.MatchUntil(str, tag, i + 1))
                {
                    return i;
                }
            }
            return -1;
        }

        public static int EIndexOf(string str, string tag, int index = 0)
        {
            if (index < 0)
                return -1;
            int end = str.Length - tag.Length - 2;
            for (int i = index; i < end; ++i)
            {
                if (str[i] == '<' && (str[i + 1] == '/' || str[i + 1] == '\\')
                    && str[i + tag.Length + 2] == '>' && Utils.MatchUntil(str, tag, i + 2))
                {
                    return i;
                }
            }
            return -1;
        }

        public static int FindIndex(string str, string tag, int count, int index = 0)
        {
            int start = SIndexOf(str, tag, index);
            for (int i = 1; i < count && start != -1; ++i)
                start = SIndexOf(str, tag, start + tag.Length + 2);
            return start;
        }

        public static int[] Range(string str, string tag, int index = 0)
        {
            int start = SIndexOf(str, tag, index);
            if (start != -1)
            {
                start += tag.Length + 2;
                int end = EIndexOf(str, tag, start);
                if (end != -1)
                    return new[] { start, end - start };
            }
            return Array.Empty<int>();
        }

        public static int[] LongRange(string str, string tag, int index = 0)
        {
            int start = SIndexOf(str, tag, index);
            if (start != -1)
            {
                int end = EIndexOf(str, tag, start + tag.Length + 2);
                if (end != -1)
                    return new[] { start, end + tag.Length + 3 - start };
            }
            return Array.Empty<int>();
        }

        public static int[] ExpandTagRange(int[] range, int tagLength)
        {
            if (range.Length != 2)
                return Array.Empty<int>();
            return new[] { range[0] - tagLength - 2, range[1] + tagLength * 2 + 5 };
        }

        public static string ReadTag(string str, string tag, int index = 0)
        {
            var r = Range(str, tag, index);
            if (r.Length == 0)
                return "";
            return str.Substring(r[0], r[1]);
        }

        public static string WriteTag(string str, string tag, string write, int index = 0)
        {
            var r = Range(str, tag, index);
            if (r.Length == 0)
                return str;
            return string.Join("", str[..r[0]], write, str[(r[0] + r[1])..]);
        }

        public static IEnumerable<int[]> EnumerateTagRanges(string str, string tag, int index = 0)
        {
            var r = Range(str, tag, index);
            while (r.Length != 0)
            {
                yield return r;
                r = Range(str, tag, r[0]);
            }
        }

        public static int[][] ReadTagsRanges(string str, string tag, int index = 0)
        {
            return EnumerateTagRanges(str, tag, index).ToArray();
        }

        public static IEnumerable<string> EnumerateTags(string str, string tag, int index = 0)
        {
            foreach (var r in EnumerateTagRanges(str, tag, index))
                yield return str.Substring(r[0], r[1]);
        }

        public static string[] ReadTags(string str, string tag, int index = 0)
        {
            return EnumerateTags(str, tag, index).ToArray();
        }

        #endregion
    }
}