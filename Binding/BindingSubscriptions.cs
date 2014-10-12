using System;
using System.Collections.Generic;
using Assisticant.Fields;

namespace Assisticant
{
	public class BindingSubscriptions
	{
		private List<ComputedSubscription> _subscriptions = new List<ComputedSubscription>();

		public void Bind<T>(Func<T> function, Action<T> action)
		{
			_subscriptions.Add (new Computed<T> (function).Subscribe (action));
		}

		public void Unsubscribe()
		{
			foreach (var subscription in _subscriptions)
				subscription.Unsubscribe ();
		}
	}
}

