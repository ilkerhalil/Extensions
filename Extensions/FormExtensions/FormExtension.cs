using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Extensions.FormExtensions
{
    public static class FormExtension
    {
        public static IEnumerable<T> AllControls<T>(this Control startingPoint) where T : Control
        {
            bool hit = startingPoint is T;
            if (hit)
            {
                yield return startingPoint as T;
            }
            foreach (var child in startingPoint.Controls.Cast<Control>())
            {
                foreach (var item in AllControls<T>(child))
                {
                    yield return item;
                }
            }
        }

        public static void Invoke(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
                return;
            }

            action();
        }

        public static T Invoke<T>(this Control control, Func<T> action)
        {
            if (control.InvokeRequired)
                return (T)control.Invoke(action);

            return action();
        }

    }
}
