using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Assisticant.Binding
{
	public static class BindingManagerExtensions
	{
		public static void Initialize (this BindingManager bindings, UIViewController controller)
		{
			UpdateScheduler.Initialize (a =>
				controller.BeginInvokeOnMainThread (new NSAction(a)));
		}
	}
}

