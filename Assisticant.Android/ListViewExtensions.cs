using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace Assisticant.Binding
{
    /// <summary>
    /// ListViewExtensions
    /// </summary>
    public static class ListViewExtensions
    {
        class BindingArrayAdapter<T> : ArrayAdapter<T>
        {
            private int _resourceId;
            private Action<View, T, BindingManager> _bind;

            public BindingArrayAdapter(
                Context context, 
                int resourceId, 
                Action<View, T, BindingManager> bind)
                : base(context, resourceId, new List<T>())
            {
                _resourceId = resourceId;
                _bind = bind;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                var inflater = (LayoutInflater)Context.GetSystemService(
                    Context.LayoutInflaterService);
                var row = inflater.Inflate(_resourceId, parent, attachToRoot: false);
                var bindings = new BindingManager();
                _bind(row, GetItem(position), bindings);
                return row;
            }

            public void UpdateItems(IEnumerable<T> items)
            {
                Clear();
                foreach (var item in items)
                    Add(item);
            }
        }

        /// <summary>
        /// Bind the items of a ListView to a collection.
        /// </summary>
        /// <typeparam name="T">The type of item in the collection.</typeparam>
        /// <param name="bindings">The binding manager.</param>
        /// <param name="control">The ListView to bind.</param>
        /// <param name="output">The collection to which to bind.</param>
        /// <param name="layoutId">The ID of the layout resource to use for the items of the ListView.</param>
        /// <param name="bind">The delegate that binds each item of the collection.</param>
        public static void BindItems<T>(
            this BindingManager bindings,
            ListView control,
            Func<IEnumerable<T>> output,
            int layoutId,
            Action<View, T, BindingManager> bind)
        {
            var adapter = new BindingArrayAdapter<T>(control.Context, layoutId, bind);
            control.Adapter = adapter;
            bindings.Bind(output, items => adapter.UpdateItems(items));
        }
    }
}