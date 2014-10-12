using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace Assisticant.Binding
{
	public static class TextBindingExtensions
	{
		public class TextBinding<TData> : IInputSubscription
		{
			private UITextField _control;
			private Action<TData> _input;
			private IValueConverter<string, TData> _converter;

			public TextBinding(UITextField control, Action<TData> input, IValueConverter<string, TData> converter)
			{
				_control = control;
				_input = input;
				_converter = converter;
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
				_input(_converter.ConvertInput(_control.Text));
			}
		}

		public class Identity : IValueConverter<string, string>
		{
			public static Identity Instance = new Identity();

			public string ConvertOutput (string data)
			{
				return data;
			}

			public string ConvertInput (string display)
			{
				return display;
			}
		}

		public class ConvertInt : IValueConverter<string, int>
		{
			public static ConvertInt Instance = new ConvertInt();

			public string ConvertOutput (int data)
			{
				return data.ToString ();
			}

			public int ConvertInput (string display)
			{
				int data = 0;
				if (int.TryParse (display, out data))
					return data;
				else
					return 0;
			}
		}

		public static void BindText<TData>(this BindingManager bindings, UITextField control, Func<TData> output, Action<TData> input, IValueConverter<string, TData> converter)
		{
			bindings.Bind (output, s => control.Text = converter.ConvertOutput(s), new TextBinding<TData>(control, input, converter));
		}

		public static void BindText<TData>(this BindingManager bindings, UILabel control, Func<TData> output, IValueConverter<string, TData> converter)
		{
			bindings.Bind (output, s => control.Text = converter.ConvertOutput(s));
		}

		public static void BindText(this BindingManager bindings, UITextField control, Func<string> output, Action<string> input)
		{
			BindText (bindings, control, output, input, Identity.Instance);
		}

		public static void BindText(this BindingManager bindings, UILabel control, Func<string> output)
		{
			BindText (bindings, control, output, Identity.Instance);
		}

		public static void BindText(this BindingManager bindings, UITextField control, Func<int> output, Action<int> input)
		{
			BindText (bindings, control, output, input, ConvertInt.Instance);
		}

		public static void BindText(this BindingManager bindings, UILabel control, Func<int> output)
		{
			BindText (bindings, control, output, ConvertInt.Instance);
		}
	}
}

