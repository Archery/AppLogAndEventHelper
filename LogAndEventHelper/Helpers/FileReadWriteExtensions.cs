using System;
using System.IO;
using System.Text;

namespace Mew.Helpers {
    public static class FileReadWriteExtensions {
        public static FileInfo WriteToFile(this string text, FileInfo goal_file = null, Encoding encoding = null) {
            encoding ??= PackagingExtensions.DefaultEncoding;
            goal_file ??= new FileInfo(Path.GetTempFileName());
            if (goal_file.Exists)
                goal_file.Delete();

            using (var sw = new StreamWriter(goal_file.FullName, false, encoding)) {
                sw.Write(text);
                sw.Flush();
            }

            return goal_file;
        }

        public static FileInfo WriteToFile2(this string text, FileInfo goal_file = null, Encoding encoding = null) {
            encoding ??= PackagingExtensions.DefaultEncoding;
            goal_file ??= new FileInfo(Path.GetTempFileName());
            if (goal_file.Exists)
                goal_file.Delete();

            File.WriteAllText(goal_file.FullName, text, encoding);
            //using (var sw = new StreamWriter(goal_file.FullName, false, encoding))
            //{
            //    sw.Write(text);
            //    sw.Flush();
            //}
            return goal_file;
        }

        public static string ReadFile(this FileInfo file_info, Encoding encoding = null, bool detect_encoding = false) {
            if (encoding == null && detect_encoding) {
                using var sr = new StreamReader(file_info.FullName, true);
                return sr.ReadToEnd();
            }

            encoding ??= PackagingExtensions.DefaultEncoding;
            using (var sr = new StreamReader(file_info.FullName, encoding, detect_encoding)) {
                return sr.ReadToEnd();
            }
        }

        public static byte[] ReadBytesFromFile(this FileInfo file_info) {
            using var fs = new FileStream(file_info.FullName, FileMode.Open, FileAccess.Read);
            // Read the source file into a byte array.
            var bytes = new byte[fs.Length];
            var num_bytes_to_read = (int) fs.Length;
            var num_bytes_read = 0;
            while (num_bytes_to_read > 0) {
                // Read may return anything from 0 to numBytesToRead.
                var n = fs.Read(bytes, num_bytes_read, num_bytes_to_read);

                // Break when the end of the file is reached.
                if (n == 0)
                    break;

                num_bytes_read += n;
                num_bytes_to_read -= n;
            }

            return bytes;
        }

        public static FileInfo WriteBytesToFile(this byte[] bytes, FileInfo goal_file) {
            using (var fs = new FileStream(goal_file.FullName, FileMode.Create, FileAccess.Write)) {
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
            }

            return goal_file;
        }

        public static FileInfo WriteBytesToFile(this FileInfo goal_file, byte[] bytes) => WriteBytesToFile(bytes, goal_file);

        public static FileInfo GetFile(this DirectoryInfo dir, string file_name) => new FileInfo("{dir.FullName}{file_name}");
    }

    public static class TextComparer {
        /// <summary>
        /// To compare 2 strings or words using "Jaro distance" metric
        /// distance = 1 − similarity
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns>0 - the same, 1 - nothing similar</returns>
        public static double JaroSimilarity(string s1, string s2) {
            if (s1 == s2) return 1;
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s1)) return 0;

            // theoretical distance
            var distance = (int) Math.Floor((Math.Max(s1.Length, s2.Length)-1) / 2.0);

            // get common characters
            var commons1 = GetCommonCharacters(s1, s2, distance);
            var commons2 = GetCommonCharacters(s2, s1, distance);
            //AppLogAndEventHelper.Instance.RaiseDebugInfo(commons1, commons2);
            if (string.IsNullOrEmpty(commons1) || string.IsNullOrEmpty(commons2)) return 0;

            // calculate transpositions
            var transpositions = 0.0;
            for (var i = 0; i < Math.Min(commons1.Length, commons2.Length); i++)
                if (commons1[i] != commons2[i])
                    transpositions++;
            transpositions /= 2.0;
            //AppLogAndEventHelper.Instance.RaiseDebugInfo(nameof(transpositions),transpositions);

            // return the Jaro distance
            return (1 + (double)commons1.Length / s1.Length + (double)commons2.Length / s2.Length - transpositions / commons1.Length) / 3.0;
        }

        private static string GetCommonCharacters(string s1, string s2, int allowed_distance) {
            var c2 = s2.ToCharArray();
            var commonCharacters = new StringBuilder();

            for (var i = 0; i < s1.Length; i++) {
                // compare if char does match inside given allowedDistance
                // and if it does add it to commonCharacters
                for (var j = Math.Max(0, i - allowed_distance); j < Math.Min(i + allowed_distance + 1, s2.Length); j++) {
                    if (c2[j] != s1[i]) continue;
                    commonCharacters.Append(s1[i]);
                    c2[j] = ' ';
                    break;
                }
            }

            return commonCharacters.ToString();
        }

        private static int GetPrefixLength(string s1, string s2, int min_prefix_length = 4) {
            var n = Math.Min(Math.Min(min_prefix_length, s1.Length), s2.Length);

            for (var i = 0; i < n; i++) {
                if (s1[i] != s2[i]) // return index of first occurrence of different characters 
                    return i;
            }

            // first n characters are the same   
            return n;
        }

        public static double JaroWinkler(string s1, string s2, double prefix_scale = 0.1) {
            var jaro_distance = JaroSimilarity(s1, s2);
            var prefix_length = GetPrefixLength(s1, s2);
            AppLogAndEventHelper.Instance.RaiseDebugInfo(nameof(prefix_length),prefix_length);

            return jaro_distance + prefix_length * prefix_scale * (1.0 - jaro_distance);
        }
    }
}