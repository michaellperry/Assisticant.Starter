using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Assisticant.Binding
{
	public static class BindingManagerExtensions
	{
		public class TextBinding : IInputSubscription
		{
			private UITextField _control;
			private Action<string> _input;

			public TextBinding(UITextField control, Action<string> input)
			{
				_control = control;
				_input = input;
			}

			public void Subscribe()
			{
				_control.EditingChanged += TextEditingChanged;
			}

			public void Unsubscribe()
			{
				_control.EditingChanged -= TextEditingChanged;
			}

			private void TextEditingChanged (object sender, EventArgs e)
			{
				_input(_control.Text);
			}
		}

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

		public static void Initialize (this BindingManager bindings, UIViewController controller)
		{
			UpdateScheduler.Initialize (a =>
				controller.BeginInvokeOnMainThread (new NSAction(a)));
		}

		public static void BindText(this BindingManager bindings, UITextField control, Func<string> output, Action<string> input)
		{
			bindings.Bind (output, s => control.Text = s, new TextBinding(control, input));
		}

		public static void BindText(this BindingManager bindings, UILabel control, Func<string> output)
		{
			bindings.Bind (output, s => control.Text = s);
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

