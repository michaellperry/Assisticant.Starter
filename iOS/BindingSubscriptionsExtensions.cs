using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Assisticant.Binding
{
	public static class BindingSubscriptionsExtensions
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

		public static void BindText(this BindingSubscriptions bindings, UITextField control, Func<string> output, Action<string> input)
		{
			bindings.Bind (output, s => control.Text = s);
			new TextBinding (control, input).Subscribe ();
		}

		public static void BindText(this BindingSubscriptions bindings, UILabel control, Func<string> output)
		{
			bindings.Bind (output, s => control.Text = s);
		}
	}
}

