namespace SAModel
{
	public class VertexWeight
	{
		public NJS_OBJECT Node { get; }
		public int Vertex { get; }
		public float Weight { get; }

		public VertexWeight(NJS_OBJECT node, int vertex, float weight)
		{
			Node = node;
			Vertex = vertex;
			Weight = weight;
		}
	}
}
