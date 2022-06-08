using System;
using System.IO;
using System.Text;

namespace Mew.AppLogAndEventHelper
{
    internal class RotatingLog : IDisposable
    {
        private readonly DirectoryInfo logDir_;
        private DateTime date_ = DateTime.MinValue;
        private Log log_;
        private FileInfo logFile_;

        public RotatingLog(DirectoryInfo log_dir)
        {
            this.logDir_ = log_dir;
            this.CheckLog();
            AppLogAndEventHelper.Instance.AddReciever(this.Write);
        }

        public void Dispose() => this.log_?.Dispose();

        private void CheckLog()
        {
            if (this.date_.Date == DateTime.Today) return;

            try
            {
                this.date_ = DateTime.Now;
                var path = Path.Combine(this.logDir_.FullName, $"{this.date_:yy-MM-dd}.log");
                this.logFile_ = new FileInfo(path);
                this.log_?.Dispose();
                this.log_ = new Log(this.logFile_.FullName, "", true);
            }
            catch(Exception e)
            {
                AppLogAndEventHelper.Instance.RemoveReciever(this.Write);
                AppLogAndEventHelper.Instance.RaiseError($"{nameof(RotatingLog)} was turned off becouse of some problems");
                AppLogAndEventHelper.Instance.RaiseError(e);
            }
        }

        private void Write(Event e)
        {
            this.CheckLog();
            // todo решить что сюда писать и что не писать
            //if (new[] {EventType.DebugInfo, EventType.ThreadInfo}.Contains(e.Type)) return;

            var sb = new StringBuilder();
            foreach (var c in e.Comments)
            {
                sb.Append(c == null ? "null" : c.Fo() + " ");
            }

            this.log_.Write($"{e.Time:HH:mm:ss} {e.Type.ToString().PadRight(10)} {e.Thread.ManagedThreadId:D3}: [{e.Place}] {sb}");
        }
    }
}