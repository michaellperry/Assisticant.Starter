using System;

namespace Assisticant
{
	public interface IValueConverter<TDisplay, TData>
	{
		TDisplay ConvertOutput(TData data);
		TData ConvertInput(TDisplay display);
	}
}

