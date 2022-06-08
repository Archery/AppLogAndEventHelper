using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

// ReSharper disable PossibleNullReferenceException


namespace Mew {
    /// <summary>
    ///     Класс для улавливания событий и рассылки их подписчикам
    /// </summary>
    public class EventCatcher {
        //Todo Ignore classes	   (Add code place class)
        public delegate void InfoEventHandler(Event e);


        private static readonly object lock_ = new object();
        private readonly Queue<Event> events_ = new Queue<Event>();
        public event InfoEventHandler ReceivedInfo;

        public void RaiseEventByPlace(EventType type, string code_place, params object[] data_list) {
            lock (lock_) {
                this.events_.Enqueue(new Event(type, data_list, code_place));

                foreach (var to in data_list.OfType<Exception>()) {
                    var s = new StringBuilder();
                    var ex = to;
                    while (ex != null) {
                        s.AppendLine($"Exception: {ex.GetType().Name}");
                        s.AppendLine($"Message: {ex.Message}");
                        s.AppendLine($"StackTrace: {ex.StackTrace}");
                        ex = ex.InnerException;
                    }

                    this.events_.Enqueue(
                        new Event(
                            EventType.DebugInfo,
                            new object[] {s.ToString()},
                            this.events_.Peek().Place));
                }

                var t = new Thread(this.SendInfo) {
                    Name = "EventCatcher_SendInfo"
                };
                t.Start();
                t.Join(10000);
            }
        }

        private void SendInfo() {
            if (this.ReceivedInfo == null) return;

            while (this.events_.Count > 0) this.ReceivedInfo(this.events_.Dequeue());
        }
    }
}