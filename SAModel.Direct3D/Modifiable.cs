namespace SonicRetro.SAModel.Direct3D
{
	public class Modifiable<T>
	{
		public bool Modified { get; private set; }

		public Modifiable()
		{
			current = default;
		}

		public Modifiable(T initialValue)
		{
			current = initialValue;
		}

		T current;

		public T Value
		{
			get => current;
			set
			{
				Modified = Modified || !current.Equals(value);
				current  = value;
			}
		}

		public void Clear()
		{
			Modified = false;
		}

		public static explicit operator T(Modifiable<T> value)
		{
			return value.Value;
		}
	}
}
