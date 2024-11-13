using SAModel;
using SplitTools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WeldConverter
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			string modelFile;
			if (args.Length > 0)
			{
				modelFile = args[0];
			}
			else
			{
				Console.Write("Model: ");
				modelFile = Console.ReadLine().Trim('"');
			}

			string weldFile;
			if (args.Length > 1)
			{
				weldFile = args[1];
			}
			else
			{
				Console.Write("Welds: ");
				weldFile = Console.ReadLine().Trim('"');
			}

			var model = new ModelFile(modelFile);
			var welds = WeldList.Load(weldFile);

			var nodes = model.Model.GetObjects();

			foreach (var node in nodes.Where(a => a.Attach != null))
			{
				node.Attach.VertexWeights = null;
			}

			var nodeDict = nodes.ToDictionary(a => a.Name);

			foreach (var weld in welds)
			{
				NJS_OBJECT objA, objB;
				switch (weld.WeldType)
				{
					case WeldType.Average:
					{
						if (nodeDict.TryGetValue(weld.ModelA, out objA) && nodeDict.TryGetValue(weld.ModelB, out objB))
						{
							if (objA.Attach.VertexWeights == null)
							{
								objA.Attach.VertexWeights = new Dictionary<int, List<VertexWeight>>();
							}

							if (objB.Attach.VertexWeights == null)
							{
								objB.Attach.VertexWeights = new Dictionary<int, List<VertexWeight>>();
							}

							for (var i = 0; i < weld.VertIndexes.Count; i += 2)
							{
								objA.Attach.VertexWeights.Add(weld.VertIndexes[i], new List<VertexWeight>
								{
									new(objA, weld.VertIndexes[i], 0.5f),
									new(objB, weld.VertIndexes[i + 1], 0.5f)
								});

								objB.Attach.VertexWeights.Add(weld.VertIndexes[i + 1], new List<VertexWeight>
								{
									new(objA, weld.VertIndexes[i], 1)
								});
							}
						}

						break;
					}
					case WeldType.Source:
					{
						if (nodeDict.TryGetValue(weld.ModelA, out objA) && nodeDict.TryGetValue(weld.ModelB, out objB))
						{
							if (objB.Attach.VertexWeights == null)
							{
								objB.Attach.VertexWeights = new Dictionary<int, List<VertexWeight>>();
							}

							for (var i = 0; i < weld.VertIndexes.Count; i += 2)
							{
								objB.Attach.VertexWeights.Add(weld.VertIndexes[i + 1], new List<VertexWeight>
								{
									new(objA, weld.VertIndexes[i], 1)
								});
							}
						}

						break;
					}
					case WeldType.Destination:
					{
						if (nodeDict.TryGetValue(weld.ModelA, out objA) && nodeDict.TryGetValue(weld.ModelB, out objB))
						{
							if (objA.Attach.VertexWeights == null)
							{
								objA.Attach.VertexWeights = new Dictionary<int, List<VertexWeight>>();
							}

							for (var i = 0; i < weld.VertIndexes.Count; i += 2)
							{
								objA.Attach.VertexWeights.Add(weld.VertIndexes[i], new List<VertexWeight>
								{
									new(objB, weld.VertIndexes[i + 1], 1)
								});
							}
						}

						break;
					}
				}
			}

			model.SaveToFile(modelFile);
		}
	}
}
