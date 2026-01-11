namespace TextureLib
{
	internal class TwiddleMap
	{
		private readonly int[] _twiddleMap;

		public TwiddleMap(int size)
		{
			_twiddleMap = new int[size];

			for(int i = 0; i < size; i++)
			{
				_twiddleMap[i] = 0;

				for(int j = 0, k = 1; k <= i; j++, k <<= 1)
				{
					_twiddleMap[i] |= (i & k) << j;
				}
			}
		}

		public int this[int x, int y]
			=> (_twiddleMap[x] << 1) | _twiddleMap[y];
	}
}
