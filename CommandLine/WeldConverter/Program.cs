﻿using SAModel;
using SplitTools;
using System;
using System.Collections.Generic;
using System.Linq;

string mdlfn;
if (args.Length > 0)
	mdlfn = args[0];
else
{
	Console.Write("Model: ");
	mdlfn = Console.ReadLine().Trim('"');
}

string weldfn;
if (args.Length > 1)
	weldfn = args[1];
else
{
	Console.Write("Welds: ");
	weldfn = Console.ReadLine().Trim('"');
}

var model = new ModelFile(mdlfn);
var welds = WeldList.Load(weldfn);
var nodes = model.Model.GetObjects();
foreach (var node in nodes.Where(a => a.Attach != null))
	node.Attach.VertexWeights = null;
var nodedict = nodes.ToDictionary(a => a.Name);

foreach (var weld in welds)
{
	NJS_OBJECT objA, objB;
	switch (weld.WeldType)
	{
		case WeldType.Average:
			if (nodedict.TryGetValue(weld.ModelA, out objA) && nodedict.TryGetValue(weld.ModelB, out objB))
			{
				if (objA.Attach.VertexWeights == null)
					objA.Attach.VertexWeights = new Dictionary<int, List<VertexWeight>>();
				if (objB.Attach.VertexWeights == null)
					objB.Attach.VertexWeights = new Dictionary<int, List<VertexWeight>>();
				for (int i = 0; i < weld.VertIndexes.Count; i += 2)
				{
					objA.Attach.VertexWeights.Add(weld.VertIndexes[i], new List<VertexWeight>() { new VertexWeight(objA, weld.VertIndexes[i], 0.5f), new VertexWeight(objB, weld.VertIndexes[i + 1], 0.5f) });
					objB.Attach.VertexWeights.Add(weld.VertIndexes[i + 1], new List<VertexWeight>() { new VertexWeight(objA, weld.VertIndexes[i], 1) });
				}
			}
			break;
		case WeldType.Source:
			if (nodedict.TryGetValue(weld.ModelA, out objA) && nodedict.TryGetValue(weld.ModelB, out objB))
			{
				if (objB.Attach.VertexWeights == null)
					objB.Attach.VertexWeights = new Dictionary<int, List<VertexWeight>>();
				for (int i = 0; i < weld.VertIndexes.Count; i += 2)
					objB.Attach.VertexWeights.Add(weld.VertIndexes[i + 1], new List<VertexWeight>() { new VertexWeight(objA, weld.VertIndexes[i], 1) });
			}
			break;
		case WeldType.Destination:
			if (nodedict.TryGetValue(weld.ModelA, out objA) && nodedict.TryGetValue(weld.ModelB, out objB))
			{
				if (objA.Attach.VertexWeights == null)
					objA.Attach.VertexWeights = new Dictionary<int, List<VertexWeight>>();
				for (int i = 0; i < weld.VertIndexes.Count; i += 2)
					objA.Attach.VertexWeights.Add(weld.VertIndexes[i], new List<VertexWeight>() { new VertexWeight(objB, weld.VertIndexes[i + 1], 1) });
			}
			break;
	}
}
model.SaveToFile(mdlfn);
