using System;
using System.IO;
using System.Reflection;

namespace Mew {
    public class AppLog:IDisposable {
        private Log log_;
        
        public FileInfo AddLog(string app_path, bool append = false) {
            var path = new FileInfo(app_path);
            if (path.Extension.ToLower() != ".log")
                path = new FileInfo(path.FullName.Replace(path.Extension, ".log"));

            if (this.log_ != null) {
                AppHelper.Instance.RaiseError($"{MethodBase.GetCurrentMethod()?.DeclaringType}.{MethodBase.GetCurrentMethod()?.Name}: Лог для приложения уже существует {this.log_}");
                return null;
            }

            try {
                this.log_ = new Log(path.FullName, append, " ");
                AppHelper.Instance.AddReceiver(this.WriteEvent);
                this.log_.Write(DateTime.Now, "Begin");
            }
            catch (Exception e) {
                app_path = $"{path.FullName.Replace(".log", "")}_{DateTime.Now:yyMMdd_HHmmss}.log";
                path = new FileInfo(app_path);
                this.log_ = new Log(path.FullName, append, " ");
                AppHelper.Instance.AddReceiver(this.WriteEvent);
                this.log_.Write(DateTime.Now, "Begin from Catch");
            }

            return this.log_.Path;
        }

        private void WriteEvent(Event e) {
            this.log_?.Write(
                e.Type.ToString().PadRight(10),
                e.Time.ToString("dd.MM.yy hh:mm:ss"),
                e.Thread.ManagedThreadId.ToString("D3"),
                e.Place,
                e.Comments
            );
        }

        public void Dispose() {
            this.log_?.Write(DateTime.Now, "End");
            this.log_?.Dispose();
        }
    }
}