namespace SAModel
{
	public class VertexWeight
	{
		public NJS_OBJECT Node { get; set; }
		public int Vertex { get; set; }
		public float Weight { get; set; }

		public VertexWeight() { }

		public VertexWeight(NJS_OBJECT node, int vertex, float weight)
		{
			Node = node;
			Vertex = vertex;
			Weight = weight;
		}
	}
}
