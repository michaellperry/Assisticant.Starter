/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2011 Michael L Perry
 * MIT License
 * 
 * This class based on a contribution by David Piepgrass.
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/
using System;
using System.ComponentModel;

namespace Assisticant.Fields
{
	/// <summary>
	/// A model property that participates in dependency tracking.
	/// </summary>
	public class Observable<T> : Observable
    {
		private T _value;

		/// <summary>
		/// Initializes a new instance of the Observable class.
		/// </summary>
		public Observable() { }

		/// <summary>
		/// Initializes a new instance of the Observable class with an initial value.
		/// </summary>
		/// <param name="value">Value.</param>
		public Observable(T value) { _value = value; }

		/// <summary>
		/// Gets or sets the value of the property.
		/// </summary>
		/// <value>The value of the property.</value>
		public T Value
		{
			get { base.OnGet(); return _value; }
			set {
				if (_value == null ? value != null : !_value.Equals(value))
				{
					base.OnSet();
					_value = value;
				}
			}
		}

		/// <param name="observable">Observable.</param>
		public static implicit operator T(Observable<T> observable)
		{
			return observable.Value;
		}

		/// <summary>
		/// Gets the observable sentry.
		/// </summary>
		/// <value>The observable sentry.</value>
		[Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
		public Observable ObservableSentry
		{
			get { return this; }
		}
    }
}