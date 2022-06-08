using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Mew.AppLogAndEventHelper
{
    /// <summary>
    ///     Тип события
    /// </summary>
    public enum EventType
    {
        PopUp = 100,
        Message = 70,
        Result = 50,
        HalfResult = 40,
        Error = 60,
        Warning = 30,
        Info = 10,
        DebugInfo = 02,
        ThreadInfo = 01
    }


    /// <summary>
    ///     Событие содержащее место в коде, время, тип и комментарий
    /// </summary>
    public struct Event
    {
        public readonly object[] Comments;
        public readonly string Place;
        public readonly DateTime Time;
        public readonly EventType Type;
        public readonly Thread Thread;

        public Event(EventType et, object[] oa, string code_place = null)
        {
            this.Type = et;

            while (oa.Length == 1 && oa[0] is object[])
            {
                oa = (object[]) oa[0];
            }

            for (var i = 0; i < oa.Length; i++)
            {
                oa[i] = oa[i] ?? "param[" + i + "]=null";
            }

            this.Comments = oa;
            this.Time = DateTime.Now;

            this.Place = "";
            if (code_place != null)
            {
                this.Place = code_place;
            }
            else
            {
#if DEBUG
                var method = (new StackFrame(4)).GetMethod();
#else
                var method = (new StackFrame(2)).GetMethod();
#endif

                if (method?.DeclaringType != null)
                {
                    var classes = method.DeclaringType.ToString().Split('.');
                    this.Place = classes.Last() + " ." + method.Name;
                }
            }

            this.Thread = Thread.CurrentThread;
        }
    }
}