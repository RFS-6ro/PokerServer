using System;

namespace UniCastCommonData.Observable
{
	[Serializable]
	public class ObservableVariable<T>
	{
		private T _value;

		public ObservableVariable(T value)
		{
			_value = value;
		}

		public event Action<T, T> OnValueChanged;

		public T Value
		{
			get => _value;
			set
			{
				T previous = _value;
				_value = value;
				OnValueChanged?.Invoke(previous, _value);
			}
		}

		public static implicit operator T(ObservableVariable<T> observableValue)
		{
			return observableValue.Value;
		}
	}
}
