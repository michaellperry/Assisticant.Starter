using System;
using System.Collections.Generic;
using Assisticant.Fields;

namespace Assisticant.Binding
{
	public class BindingSubscriptions
	{
		struct SubscriptionPair
		{
			public ComputedSubscription Output;
			public IInputSubscription Input;
		}

		private List<SubscriptionPair> _subscriptions = new List<SubscriptionPair>();

		public void Bind<T>(Func<T> function, Action<T> action)
		{
			_subscriptions.Add (new SubscriptionPair
			{
				Output = new Computed<T> (function).Subscribe (action)
			});
		}

		public void Bind<T>(Func<T> function, Action<T> action, IInputSubscription input)
		{
			input.Subscribe ();
			_subscriptions.Add (new SubscriptionPair
			{
				Output = new Computed<T> (function).Subscribe (action),
				Input = input
			});
		}

		public void Unsubscribe()
		{
			foreach (var subscription in _subscriptions) {
				subscription.Output.Unsubscribe ();
				if (subscription.Input != null)
					subscription.Input.Unsubscribe ();
			}
		}
	}
}

