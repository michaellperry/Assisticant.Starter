using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Assisticant.Binding
{
	public static class StepperBindingExtensions
	{
		public class ValueBinding<TData> : IInputSubscription
		{
			private UIStepper _control;
			private Action<TData> _input;
			private IValueConverter<double, TData> _converter;

			public ValueBinding(UIStepper control, Action<TData> input, IValueConverter<double, TData> converter)
			{
				_control = control;
				_input = input;
				_converter = converter;
			}

			public void Subscribe()
			{
				_control.ValueChanged += StepperValueChanged;
			}

			public void Unsubscribe()
			{
				_control.ValueChanged -= StepperValueChanged;
			}

			private void StepperValueChanged (object sender, EventArgs e)
			{
				_input(_converter.ConvertInput(_control.Value));
			}
		}

		public class Identity : IValueConverter<double, double>
		{
			public static Identity Instance = new Identity();

			public double ConvertOutput (double data)
			{
				return data;
			}

			public double ConvertInput (double display)
			{
				return display;
			}
		}

		public class ConvertInt : IValueConverter<double, int>
		{
			public static ConvertInt Instance = new ConvertInt();

			public double ConvertOutput (int data)
			{
				return data;
			}

			public int ConvertInput (double display)
			{
				return (int)display;
			}
		}

		public static void BindValue<TData>(this BindingManager bindings, UIStepper control, Func<TData> output, Action<TData> input, IValueConverter<double, TData> converter)
		{
			bindings.Bind (output, s => control.Value = converter.ConvertOutput(s), new ValueBinding<TData>(control, input, converter));
		}

		public static void BindValue(this BindingManager bindings, UIStepper control, Func<double> output, Action<double> input)
		{
			BindValue (bindings, control, output, input, Identity.Instance);
		}

		public static void BindValue(this BindingManager bindings, UIStepper control, Func<int> output, Action<int> input)
		{
			BindValue (bindings, control, output, input, ConvertInt.Instance);
		}
	}
}

