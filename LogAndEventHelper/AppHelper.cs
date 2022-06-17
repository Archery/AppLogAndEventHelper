using System;
using System.IO;

namespace Mew {
    /// <summary>
    ///     Синглтон класс для записи журнала событий приложения в файл
    /// </summary>
    public class AppHelper : IDisposable {
        private static volatile AppHelper instance_;
        private static readonly object syncRoot_ = new object();
        private readonly EventCatcher eventCatcher_ = new EventCatcher();

        private RotatingLog rotatingLog_;
        private AppLog appLog_;

        public static AppHelper Instance {
            get {
                if (instance_ != null)
                    return instance_;

                lock (syncRoot_) {
                    if (instance_ == null) instance_ = new AppHelper();
                }

                return instance_;
            }
        }

        private AppHelper() {
            this.AddReceiver(Receivers.DebugConsoleWrite);
        }

        public void Dispose() {
            this.appLog_?.Dispose();

            this.rotatingLog_?.Dispose();
        }

        public void AddRotatingLog(DirectoryInfo dir) {
            this.rotatingLog_ = new RotatingLog(dir);
        }

        public void AddReceiver(EventCatcher.InfoEventHandler f) {
            this.eventCatcher_.ReceivedInfo += f;
        }

        public void RemoveReceiver(EventCatcher.InfoEventHandler f) {
            this.eventCatcher_.ReceivedInfo -= f;
        }
        
        private void RaiseEvent_Sender(EventType type, object[] data_list) {
            this.eventCatcher_.RaiseEventByPlace(type, null, data_list);
        }

        public void RaiseEvent(EventType type, params object[] data_list) {
            this.RaiseEvent_Sender(type, data_list);
        }

        public void RaiseInfo(params object[] data_list) {
            this.RaiseEvent_Sender(EventType.Info, data_list);
        }

        public void RaiseDebugInfo(params object[] data_list) {
            this.RaiseEvent_Sender(EventType.DebugInfo, data_list);
        }

        public void RaiseError(params object[] data_list) {
            this.RaiseEvent_Sender(EventType.Error, data_list);
        }

        public void RaisePopUp(params object[] data_list) {
            this.RaiseEvent_Sender(EventType.PopUp, data_list);
        }

        public void RaiseEventByPlace(EventType type, string code_place, params object[] data_list) {
            this.eventCatcher_.RaiseEventByPlace(type, code_place, data_list);
        }
    }
}