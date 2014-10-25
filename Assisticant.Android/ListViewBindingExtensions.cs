using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assisticant.Binding
{
    /// <summary>
    /// ListViewExtensions
    /// </summary>
    public static class ListViewBindingExtensions
    {
        class ItemContainer<T> : IDisposable
        {
            private readonly T _item;
            private readonly BindingArrayAdapter<T> _adapter;
            private BindingManager _bindings;

            public ItemContainer(T item, BindingArrayAdapter<T> adapter)
            {
                _item = item;
                _adapter = adapter;
            }

            public T Item
            {
                get { return _item; }
            }

            public BindingManager Bindings
            {
                get { return _bindings; }
            }

            public View View { get; set; }

            public void EnsureInCollection(int index)
            {
                if (_bindings == null)
                {
                    _adapter.Insert(this, index);
                    _bindings = new BindingManager();
                }
                else if (_adapter.GetItem(index) != this)
                {
                    _adapter.Remove(this);
                    _adapter.Insert(this, index);
                }
            }

            public void Dispose()
            {
                if (_bindings != null)
                {
                    _bindings.Unbind();
                    _adapter.Remove(this);
                }
            }

            public override bool Equals(object obj)
            {
                if (obj == this)
                    return true;
                var that = obj as ItemContainer<T>;
                if (that == null)
                    return false;
                return Object.Equals(_item, that._item);
            }

            public override int GetHashCode()
            {
                return _item.GetHashCode();
            }
        }

        class BindingArrayAdapter<T> : ArrayAdapter<ItemContainer<T>>, IInputSubscription
        {
            private int _resourceId;
            private Action<View, T, BindingManager> _bind;
            private List<ItemContainer<T>> _itemContainers = new List<ItemContainer<T>>();

            public BindingArrayAdapter(
                Context context, 
                int resourceId, 
                Action<View, T, BindingManager> bind)
                : base(context, resourceId, new List<ItemContainer<T>>())
            {
                _resourceId = resourceId;
                _bind = bind;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                var itemContainer = GetItem(position);
                if (itemContainer.View != null)
                    return itemContainer.View;

                var inflater = (LayoutInflater)Context.GetSystemService(
                    Context.LayoutInflaterService);
                var row = inflater.Inflate(_resourceId, parent, attachToRoot: false);
                var scheduler = UpdateScheduler.Begin();
                _bind(row, itemContainer.Item, itemContainer.Bindings);
                foreach (var update in scheduler.End())
                    update();
                itemContainer.View = row;
                return row;
            }

            public void UpdateItems(IEnumerable<T> items)
            {
                using (var bin = new RecycleBin<ItemContainer<T>>(_itemContainers))
                {
                    _itemContainers.Clear();
                    foreach (var item in items)
                    {
                        var itemContainer = bin.Extract(new ItemContainer<T>(item, this));
                        _itemContainers.Add(itemContainer);
                    }
                }

                int index = 0;
                foreach (var itemContainer in _itemContainers)
                {
                    itemContainer.EnsureInCollection(index);
                    index++;
                }
            }

            public void Subscribe()
            {
            }

            public void Unsubscribe()
            {
                foreach (var itemContainer in _itemContainers)
                {
                    itemContainer.Bindings.Unbind();
                }
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
            bindings.Bind(() => output().ToList(), items => adapter.UpdateItems(items), adapter);
        }
    }
}