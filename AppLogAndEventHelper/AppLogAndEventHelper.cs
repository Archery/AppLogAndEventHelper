using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Mew.AppLogAndEventHelper
{
    /// <summary>
    ///     Синглтон класс для записи журнала событий приложения в файл
    /// </summary>
    public class AppLogAndEventHelper : IDisposable
    {
        public const string EndOfLine = "\r\n";

        private static volatile AppLogAndEventHelper instance_;
        private static readonly object syncRoot_ = new object();
        private readonly EventCatcher eventCatcher_ = new EventCatcher();

        private Log log_;
        private RotatingLog rotatingLog_;

        public static AppLogAndEventHelper Instance
        {
            get
            {
                if (instance_ != null)
                    return instance_;

                lock (syncRoot_)
                {
                    if (instance_ == null)
                    {
                        instance_ = new AppLogAndEventHelper();
                    }
                }
                return instance_;
            }
        }

        private AppLogAndEventHelper()
        {
            this.AddReciever(DebugConsoleWrite);
        }

        public void Dispose()
        {
            this.log_?.Write(DateTime.Now, "End");
            this.log_?.Dispose();

            this.rotatingLog_?.Dispose();
        }

        public FileInfo AddLog(string app_path, string delimiter = " ", bool append = false)
        {
            var path = new FileInfo(app_path);
            if (path.Extension.ToLower() != ".log")
                path = new FileInfo(path.FullName.Replace(path.Extension, ".log"));

            if (this.log_ != null)
            {
                this.RaiseError($"{MethodBase.GetCurrentMethod().DeclaringType}.{MethodBase.GetCurrentMethod().Name}: Лог для приложения уже существует {this.log_}");
                return null;
            }

            try
            {
                this.log_ = new Log(path.FullName, delimiter, append);
                this.AddReciever(this.WriteEvent);
                this.log_.Write(DateTime.Now, "Begin");
            }
            catch(Exception e)
            {
                app_path = $"{path.FullName.Replace(".log", "")}_{DateTime.Now:yyMMdd_HHmmss}.log";
                path = new FileInfo(app_path);
                this.log_ = new Log(path.FullName, delimiter, append);
                this.AddReciever(this.WriteEvent);
                this.log_.Write(DateTime.Now, "Begin from Catch");
            }
            return this.log_.Path;
        }

        public void AddRotatingLog(DirectoryInfo dir)
        {
            this.rotatingLog_ = new RotatingLog(dir);
        }

        public void AddReciever(EventCatcher.InfoEventHandler f)
        {
            this.eventCatcher_.RecievedInfo += f;
        }

        public void RemoveReciever(EventCatcher.InfoEventHandler f)
        {
            this.eventCatcher_.RecievedInfo -= f;
        }

        private void WriteEvent(Event e)
        {
            this.log_.Write(
                e.Type.ToString().PadRight(10),
                e.Time.ToString("dd.MM.yy hh:mm:ss"),
                e.Thread.ManagedThreadId.ToString("D3"),
                e.Place,
                e.Comments
            );
        }

        private static void DebugConsoleWrite(Event e)
        {
            if (e.Type == EventType.Error)
                return;

            var sb = new StringBuilder();
            foreach (var c in e.Comments)
            {
                sb.Append(c == null ? "null" : c.Fo() + " ");
            }

            Debug.WriteLine($"{e.Time:HH:mm:ss} {e.Type.ToString().PadRight(10)} {e.Thread.ManagedThreadId:D3}: [{e.Place}] {sb}");
        }

        private void RaiseEvent_Sender(EventType type, object[] data_list)
        {
            this.eventCatcher_.RaiseEventByPlace(type, null, data_list);
        }

        public void RaiseEvent(EventType type, params object[] data_list)
        {
            this.RaiseEvent_Sender(type, data_list);
        }

        public void RaiseInfo(params object[] data_list)
        {
            this.RaiseEvent_Sender(EventType.Info, data_list);
        }

        public void RaiseDebugInfo(params object[] data_list)
        {
            this.RaiseEvent_Sender(EventType.DebugInfo, data_list);
        }

        public void RaiseError(params object[] data_list)
        {
            this.RaiseEvent_Sender(EventType.Error, data_list);
        }

        public void RaisePopUp(params object[] data_list)
        {
            this.RaiseEvent_Sender(EventType.PopUp, data_list);
        }

        public void RaiseEventByPlace(EventType type, string code_place, params object[] data_list)
        {
            this.eventCatcher_.RaiseEventByPlace(type, code_place, data_list);
        }
    }
}