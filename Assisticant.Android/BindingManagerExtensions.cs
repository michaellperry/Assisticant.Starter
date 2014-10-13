using Android.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Assisticant.Android
{
    /// <summary>
    /// Binding manager extensions.
    /// </summary>
    public class BindingManagerExtensions
    {
        /// <summary>
        /// Initialize the binding manager for an activity.
        /// </summary>
        /// <param name="activity">The activity that owns the binding manager.</param>
        public static void Initialize(Activity activity)
        {
            UpdateScheduler.Initialize(action =>
            {
                ThreadPool.QueueUserWorkItem(delegate(Object obj)
                {
                    activity.RunOnUiThread(action);
                });
            });
        }
    }
}
