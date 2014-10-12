using System;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace Assisticant.Binding
{
	/// <summary>
	/// Table binding extensions.
	/// </summary>
	public static class TableBindingExtensions
	{
		public static void BindItems<T>(this BindingManager bindings, UITableView control, Func<IEnumerable<T>> items)
		{
			bindings.Bind (items, c => {
			});
		}
	}
}

