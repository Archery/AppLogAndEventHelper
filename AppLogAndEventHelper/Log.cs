using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mew {
    /// <summary>
    ///     Класс для работы с текстовыми файлами
    /// </summary>
    public class Log : IDisposable {
        private readonly string delimiter_;
        private readonly StreamWriter sw_;
        public readonly FileInfo Path;
        public int LinesCount { get; private set; }

        public Log(string path, string delimiter = " ", bool append = false, Encoding encoding = null) {
            this.LinesCount = 0;
            try {
                encoding ??= Encoding.Unicode;
                this.Path = new FileInfo(System.IO.Path.GetFullPath(path));

                var file_stream = File.Open(this.Path.FullName, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                this.sw_ = new StreamWriter(file_stream, encoding) {
                        AutoFlush = true
                    };
            }
            catch (Exception ex) {
                AppLogAndEventHelper.Instance.RaiseEvent(EventType.Error, ex);
                throw;
            }
        }

        public void Dispose() {
            if (this.sw_ == null) return;
            lock (this.sw_) {
                this.sw_.Close();
                this.sw_.Dispose();
            }
        }

        private void WriteListOfObjects(params object[] list) {
            if (this.sw_?.BaseStream == null) return;
            try {
                this.SortOutObjectList(list);
            }
            catch (Exception ex) {
                AppLogAndEventHelper.Instance.RaiseEvent(EventType.Error, ex);
            }
        }

        private void SortOutObjectList(IEnumerable<object> list) {
            foreach (var child in list) {
                var objects = child as object[];
                if (objects != null)
                    this.SortOutObjectList(objects);
                else
                    this.sw_.Write(child.Fo() + this.delimiter_);
            }
        }

        public void Write(params object[] list) {
            if (this.sw_?.BaseStream == null) return;
            

            lock (this.sw_) {
                this.WriteListOfObjects(list);
                this.sw_.WriteLine();
            }
        }
    }
}