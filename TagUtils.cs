namespace CSUtils
{
    public static class TagUtils
    {
        #region CharTags

        public static int SIndexOf(string s, char tag, int index = 0)
        {
            if (index < 0)
                return -1;
            for (int i = index; i < s.Length - 2; ++i)
            {
                if (s[i] == '<' && s[i + 1] == tag && s[i + 2] == '>')
                    return i;
            }
            return -1;
        }

        public static int EIndexOf(string s, char tag, int index = 0)
        {
            if (index < 0)
                return -1;
            for (int i = index; i < s.Length - 3; ++i)
            {
                if (s[i] == '<' && (s[i + 1] == '/' || s[i + 1] == '\\') 
                    && s[i + 2] == tag && s[i + 3] == '>')
                {
                    return i;
                }
            }
            return -1;
        }

        public static int FindIndex(string s, char tag, int count, int index = 0)
        {
            int start = SIndexOf(s, tag, index);
            for (int i = 1; i < count && start != -1; ++i)
                start = SIndexOf(s, tag, start + 3);
            return start;
        }

        public static int[] Range(string s, char tag, int index = 0)
        {
            int start = SIndexOf(s, tag, index);
            if (start != -1)
            {
                start += 3;
                int end = EIndexOf(s, tag, start);
                if (end != -1)
                    return new[] { start, end - start };
            }
            return Array.Empty<int>();
        }

        public static int[] LongRange(string s, char tag, int index = 0)
        {
            int start = SIndexOf(s, tag, index);
            if (start != -1)
            {
                int end = EIndexOf(s, tag, start + 3);
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

        public static string ReadTag(string s, char tag, int index = 0)
        {
            var r = Range(s, tag, index);
            if (r.Length == 0)
                return "";
            return s.Substring(r[0], r[1]);
        }

        public static string WriteTag(string s, char tag, string write, int index = 0)
        {
            var r = Range(s, tag, index);
            if (r.Length == 0)
                return s;
            return string.Join(s[..r[0]], write, s[(r[0] + r[1])..]);
        }

        public static IEnumerable<int[]> EnumerateTagRanges(string s, char tag, int index = 0)
        {
            var r = Range(s, tag, index);
            while (r.Length != 0)
            {
                yield return r;
                r = Range(s, tag, r[0]);
            }
        }

        public static int[][] ReadTagsRanges(string s, char tag, int index = 0)
        {
            return EnumerateTagRanges(s, tag, index).ToArray();
        }

        public static IEnumerable<string> EnumerateTags(string s, char tag, int index = 0)
        {
            foreach (var r in EnumerateTagRanges(s, tag, index))
                yield return s.Substring(r[0], r[1]);
        }

        public static string[] ReadTags(string s, char tag, int index = 0)
        {
            return EnumerateTags(s, tag, index).ToArray();
        }

        #endregion

        #region StringTags

        public static int SIndexOf(string s, string tag, int index = 0)
        {
            if (index < 0)
                return -1;
            int end = s.Length - tag.Length - 2;
            for (int i = index; i < end; ++i)
            {
                if (s[i] == '<' && s[i + tag.Length + 1] == '>' &&
                    Utils.MatchUntil(s, tag, i + 1))
                {
                    return i;
                }
            }
            return -1;
        }

        public static int EIndexOf(string s, string tag, int index = 0)
        {
            if (index < 0)
                return -1;
            int end = s.Length - tag.Length - 2;
            for (int i = index; i < end; ++i)
            {
                if (s[i] == '<' && (s[i + 1] == '/' || s[i + 1] == '\\')
                    && s[i + tag.Length + 2] == '>' && Utils.MatchUntil(s, tag, i + 2))
                {
                    return i;
                }
            }
            return -1;
        }

        public static int FindIndex(string s, string tag, int count, int index = 0)
        {
            int start = SIndexOf(s, tag, index);
            for (int i = 1; i < count && start != -1; ++i)
                start = SIndexOf(s, tag, start + tag.Length + 2);
            return start;
        }

        public static int[] Range(string s, string tag, int index = 0)
        {
            int start = SIndexOf(s, tag, index);
            if (start != -1)
            {
                start += tag.Length + 2;
                int end = EIndexOf(s, tag, start);
                if (end != -1)
                    return new[] { start, end - start };
            }
            return Array.Empty<int>();
        }

        public static int[] LongRange(string s, string tag, int index = 0)
        {
            int start = SIndexOf(s, tag, index);
            if (start != -1)
            {
                int end = EIndexOf(s, tag, start + tag.Length + 2);
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

        public static string ReadTag(string s, string tag, int index = 0)
        {
            var r = Range(s, tag, index);
            if (r.Length == 0)
                return "";
            return s.Substring(r[0], r[1]);
        }

        public static string WriteTag(string s, string tag, string write, int index = 0)
        {
            var r = Range(s, tag, index);
            if (r.Length == 0)
                return s;
            return string.Join("", s[..r[0]], write, s[(r[0] + r[1])..]);
        }

        public static IEnumerable<int[]> EnumerateTagRanges(string s, string tag, int index = 0)
        {
            var r = Range(s, tag, index);
            while (r.Length != 0)
            {
                yield return r;
                r = Range(s, tag, r[0]);
            }
        }

        public static int[][] ReadTagsRanges(string s, string tag, int index = 0)
        {
            return EnumerateTagRanges(s, tag, index).ToArray();
        }

        public static IEnumerable<string> EnumerateTags(string s, string tag, int index = 0)
        {
            foreach (var r in EnumerateTagRanges(s, tag, index))
                yield return s.Substring(r[0], r[1]);
        }

        public static string[] ReadTags(string s, string tag, int index = 0)
        {
            return EnumerateTags(s, tag, index).ToArray();
        }

        #endregion
    }
}