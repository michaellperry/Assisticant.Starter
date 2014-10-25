using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;

namespace Assisticant.Binding
{
	public static class CollectionViewBindingExtensions
	{
		class ItemContainer<T> : IDisposable
		{
			private readonly T _item;
			private readonly CollectionBinding<T> _binding;
			private BindingManager _bindings;
			private UICollectionViewCell _cell;

			public ItemContainer(T item, CollectionBinding<T> binding)
			{
				_item = item;
				_binding = binding;
			}

			public T Item
			{
				get { return _item; }
			}

			public BindingManager Bindings
			{
				get { return _bindings; }
			}

			public UICollectionViewCell Cell
			{
				get { return _cell; }
			}

			public void EnsureInCollection(int index)
			{
				if (_bindings == null)
				{
					_bindings = new BindingManager();
					_cell = _binding.Insert(this, index);
				}
				else if (_binding.GetCell(index) != _cell)
				{
					_binding.Move(this, index);
				}
			}

			public void Dispose()
			{
				if (_bindings != null)
				{
					_bindings.Unbind();
					_binding.Remove(this);
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

		class CollectionBinding<T> : IInputSubscription
		{
			private UICollectionView _collectionView;
			private Func<T, BindingManager, UIView> _bind;
			private List<ItemContainer<T>> _itemContainers = new List<ItemContainer<T>>();

			public CollectionBinding(
				UICollectionView collectionView,
				Func<T, BindingManager, UIView> bind)
			{
				_collectionView = collectionView;
				_bind = bind;
			}

			public object UpdateItems (IEnumerable<T> items)
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

			public UICollectionViewCell GetCell (int index)
			{
				return _collectionView.CellForItem (NSIndexPath.FromIndex (index));
			}

			public UICollectionViewCell Insert (ItemContainer<T> itemContainer, int index)
			{
				var view = _bind (itemContainer.Item, itemContainer.Bindings);
				_collectionView.InsertSubview (view, index);
				return _collectionView.CellForItem(NSIndexPath.FromIndex(index));
			}

			public void Move (ItemContainer<T> itemContainer, int index)
			{
				_collectionView.MoveItem (_collectionView.IndexPathForCell (itemContainer.Cell), NSIndexPath.FromIndex (index));
			}

			public void Remove (ItemContainer<T> itemContainer)
			{
				_collectionView.DeleteItems(_collectionView.IndexPathForCell(itemContainer.Cell));
			}

			public void Subscribe ()
			{
			}

			public void Unsubscribe ()
			{
				foreach (var itemContainer in _itemContainers)
				{
					itemContainer.Bindings.Unbind();
				}
			}
		}

		/// <summary>
		/// Bind the items of a UICollectionView to a collection.
		/// </summary>
		/// <typeparam name="T">The type of item in the collection.</typeparam>
		/// <param name="bindings">The binding manager.</param>
		/// <param name="control">The ListView to bind.</param>
		/// <param name="output">The collection to which to bind.</param>
		/// <param name="bind">The delegate that binds each item of the collection.</param>
		public static void BindItems<T>(
			this BindingManager bindings,
			UICollectionView control,
			Func<IEnumerable<T>> output,
			Func<T, BindingManager, UIView> bind)
		{
			var binding = new CollectionBinding<T>(bind);
			bindings.Bind(() => output().ToList(), items => binding.UpdateItems(items), binding);
		}
	}
}

