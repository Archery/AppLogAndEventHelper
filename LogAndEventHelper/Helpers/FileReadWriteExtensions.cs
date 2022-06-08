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

        public static FileInfo GetFile(this DirectoryInfo dir, string file_name) => new FileInfo($"{dir.FullName}{file_name}");
    }
}