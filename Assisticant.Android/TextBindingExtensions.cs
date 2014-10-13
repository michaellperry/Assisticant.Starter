using System;
using System.Collections.Generic;
using Android.Widget;
using Android.Views;

namespace Assisticant.Binding
{
	/// <summary>
	/// Text binding extensions.
	/// </summary>
	public static class TextBindingExtensions
	{
		class TextBinding<TData> : IInputSubscription
		{
			private TextView _control;
            private Func<TData> _output;
			private Action<TData> _input;
			private IValueConverter<string, TData> _converter;

			public TextBinding(TextView control, Func<TData> output, Action<TData> input, IValueConverter<string, TData> converter)
			{
				_control = control;
                _output = output;
				_input = input;
				_converter = converter;
			}

			public void Subscribe()
			{
				_control.TextChanged += TextViewTextChanged;
                _control.FocusChange += TextViewFocusChanged;
			}

			public void Unsubscribe()
			{
                _control.TextChanged -= TextViewTextChanged;
                _control.FocusChange -= TextViewFocusChanged;
            }

			private void TextViewTextChanged (object sender, EventArgs e)
			{
				_input(_converter.ConvertInput(_control.Text));
			}

            private void TextViewFocusChanged(object sender, View.FocusChangeEventArgs e)
            {
                _control.Text = _converter.ConvertOutput(_output());
            }
		}

		class Identity : IValueConverter<string, string>
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

		class ConvertInt : IValueConverter<string, int>
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

		/// <summary>
		/// Bind the text of a text view to a property using a value converter.
		/// </summary>
		/// <param name="bindings">The binding manager.</param>
		/// <param name="control">The text field.</param>
		/// <param name="output">A function that gets the property.</param>
		/// <param name="input">A function that sets the property.</param>
		/// <param name="converter">A custom value converter to string.</param>
		/// <typeparam name="TData">The type of the property.</typeparam>
        public static void BindText<TData>(this BindingManager bindings, TextView control, Func<TData> output, Action<TData> input, IValueConverter<string, TData> converter)
        {
            bindings.Bind(
                output, 
                data => UpdateTextView<TData>(control, data, converter), 
                new TextBinding<TData>(control, output, input, converter));
        }

		/// <summary>
		/// Bind the text of a text view to a read-only property using a value converter.
		/// </summary>
		/// <param name="bindings">The binding manager.</param>
		/// <param name="control">The label.</param>
		/// <param name="output">A function that gets the property.</param>
		/// <param name="converter">A custom value converter to string.</param>
		/// <typeparam name="TData">The type of the property.</typeparam>
		public static void BindText<TData>(this BindingManager bindings, TextView control, Func<TData> output, IValueConverter<string, TData> converter)
		{
            bindings.Bind(
                output,
                data => UpdateTextView<TData>(control, data, converter));
		}

		/// <summary>
		/// Bind the text of a text view to a string property.
		/// </summary>
		/// <param name="bindings">The binding manager.</param>
		/// <param name="control">The text field.</param>
		/// <param name="output">A function that gets the property.</param>
		/// <param name="input">A function that sets the property.</param>
		public static void BindText(this BindingManager bindings, TextView control, Func<string> output, Action<string> input)
		{
			BindText(bindings, control, output, input, Identity.Instance);
		}

		/// <summary>
		/// Bind the text of a text view to a read-only string property.
		/// </summary>
		/// <param name="bindings">The binding manager.</param>
		/// <param name="control">The label.</param>
		/// <param name="output">A function that gets the property.</param>
		public static void BindText(this BindingManager bindings, TextView control, Func<string> output)
		{
			BindText(bindings, control, output, Identity.Instance);
		}

		/// <summary>
		/// Bind the text of a text view to an int property.
		/// </summary>
		/// <param name="bindings">The binding manager.</param>
		/// <param name="control">The text field.</param>
		/// <param name="output">A function that gets the property.</param>
		/// <param name="input">A function that sets the property.</param>
		public static void BindText(this BindingManager bindings, TextView control, Func<int> output, Action<int> input)
		{
			BindText(bindings, control, output, input, ConvertInt.Instance);
		}

		/// <summary>
		/// Bind the text of a text view to a read-only int property.
		/// </summary>
		/// <param name="bindings">The binding manager.</param>
		/// <param name="control">The label.</param>
		/// <param name="output">A function that gets the property.</param>
		public static void BindText(this BindingManager bindings, TextView control, Func<int> output)
		{
			BindText(bindings, control, output, ConvertInt.Instance);
		}

        private static void UpdateTextView<TData>(TextView control, TData data, IValueConverter<string, TData> converter)
        {
            if (!control.HasFocus)
                control.Text = converter.ConvertOutput(data);
        }
    }
}

