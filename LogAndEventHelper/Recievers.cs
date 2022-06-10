using System;
using System.Diagnostics;
using System.Text;

namespace Mew {
    public class Recievers {
        //public static void ShowMyDialogBox(Event e)
        //{
        //    Form2 testDialog = new Form2();

        //    // Show testDialog as a modal dialog and determine if DialogResult = OK.
        //    if (testDialog.ShowDialog(this) == DialogResult.OK)
        //    {
        //        // Read the contents of testDialog's TextBox.
        //        this.txtResult.Text = testDialog.TextBox1.Text;
        //    }
        //    else
        //    {
        //        this.txtResult.Text = "Cancelled";
        //    }
        //    testDialog.Dispose();
        //}

        public static void DebugConsoleWrite(Event e) {
            if (e.Type != EventType.Error)
                return;

            var sb = new StringBuilder();
            foreach (var c in e.Comments) sb.Append(c == null ? "null" : c.Fo() + " ");

            Debug.WriteLine($"{e.Time:HH:mm:ss} {e.Type.ToString().PadRight(10)} {e.Thread.ManagedThreadId:D3}: [{e.Place}] {sb}");
        }

        public static void WriteEventToConsole(Event e) {
            if (e.Type < EventType.Info)
                return;

            Console.Write($"{e.Type.ToString().PadRight(5)} {e.Time:hh:mm:ss}: {e.CommentsToString()}");
            Console.WriteLine();
        }
    }
}