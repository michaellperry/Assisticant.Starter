using System;
using MonoTouch.UIKit;

namespace Assisticant.Binding
{
	public static class ButtonBindingExtensions
	{
		public class ButtonClickSubscription : IInputSubscription
		{
			private UIButton _control;
			private Action _action;

			public ButtonClickSubscription(UIButton control, Action action)
			{
				_control = control;
				_action = action;
			}

			public void Subscribe()
			{
				_control.TouchUpInside += ButtonTouchUpInside;
			}

			public void Unsubscribe()
			{
				_control.TouchUpInside -= ButtonTouchUpInside;
			}

			private void ButtonTouchUpInside(object sender, EventArgs e)
			{
				_action ();
			}
		}

		public static void BindCommand(this BindingManager bindings, UIButton control, Action action)
		{
			bindings.Bind (new ButtonClickSubscription (control, action));
		}

		public static void BindCommand(this BindingManager bindings, UIButton control, Action action, Func<bool> condition)
		{
			bindings.Bind (condition, b => control.Enabled = b, new ButtonClickSubscription (control, action));
		}
	}
}

