using System;
using System.Collections.Generic;
using System.Linq;

namespace TextureLib
{
	/// <summary>
	/// Vector quantization algorithm.
	/// </summary>
	public class VectorQuantization
	{
		#region helper types

		private struct VQDataPoint
		{
			public byte[] values;
			public int count;

			public readonly byte[] ValueCopy => (byte[])values.Clone();

			public VQDataPoint(byte[] values, int count)
			{
				this.values = values;
				this.count = count;
			}
		}

		private struct DistanceInfo
		{
			public int index;
			public int count;
			public int distance;
			public int distanceIndex;

			public DistanceInfo(int index, int count, int distance, int distanceIndex)
			{
				this.index = index;
				this.count = count;
				this.distance = distance;
				this.distanceIndex = distanceIndex;
			}

			public override readonly string ToString()
			{
				return $"{index}[{count}] -> {distance} [{distanceIndex}]";
			}
		}


		#endregion

		private int Dimensions { get; }
		private int ClusterLimit { get; }
		private int MaxIterations { get; }
		private int ChangeAndMergeThreshold { get; }

		private VQDataPoint[] Data { get; }

		private List<byte[]> Clusters { get; }

		private VectorQuantization(int dimensions, int clusterLimit, int maxIterations, int changeAndMergeThreshold, VQDataPoint[] data)
		{
			Dimensions = dimensions;
			ClusterLimit = clusterLimit;
			MaxIterations = maxIterations;
			ChangeAndMergeThreshold = changeAndMergeThreshold;
			Data = data;
			Clusters = [];
		}

		private bool CheckDistinct()
		{
			if(Data.Length > ClusterLimit)
			{
				return false;
			}

			Clusters.AddRange(Data.Select(x => (byte[])x.values.Clone()));
			return true;
		}

		private int CalcMagnitude(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
		{
			int magnitude = 0;
			for(int i = 0; i < Dimensions; i++)
			{
				int dif = a[i] - b[i];
				magnitude += dif * dif;
			}

			return magnitude;
		}

		private int GetClosestClusterIndex(ReadOnlySpan<byte> value, out int distance)
		{
			int closest = 0;
			distance = int.MaxValue;

			for(int i = 0; i < Clusters.Count; i++)
			{
				int clusterDistance = CalcMagnitude(value, Clusters[i]);

				if(clusterDistance < distance)
				{
					closest = i;
					distance = clusterDistance;
					if(distance == 0)
					{
						break;
					}
				}
			}

			return closest;
		}

		private bool Init(ReadOnlySpan<byte> rawData)
		{
			if(CheckDistinct())
			{
				return true;
			}

			// use evenly spaced clusters for the start
			float index = 0;
			float step = 1f / (ClusterLimit - 1);
			for(int i = 0; i < ClusterLimit; i++)
			{
				int realIndex = (int)MathF.Round(index * ((rawData.Length / Dimensions) - 1));

				byte[] cluster = new byte[Dimensions];
				for(int j = 0; j < Dimensions; j++)
				{
					cluster[j] = rawData[realIndex + j];
				}

				Clusters.Add(cluster);
				index += step;
			}

			return false;
		}

		private void Fill()
		{
			while(Clusters.Count < ClusterLimit)
			{
				int[] childCount = new int[Clusters.Count];
				int[] distances = new int[Clusters.Count];
				int[] distancePoints = new int[Clusters.Count];

				for(int j = 0; j < Data.Length; j++)
				{
					VQDataPoint point = Data[j];
					int clusterIndex = GetClosestClusterIndex(point.values, out int distance);
					childCount[clusterIndex] += Data[j].count;

					if(distance > distances[clusterIndex])
					{
						distances[clusterIndex] = distance;
						distancePoints[clusterIndex] = j;
					}
				}

				DistanceInfo[] mappedChildCount = new DistanceInfo[Clusters.Count];
				for(int i = 0; i < mappedChildCount.Length; i++)
				{
					mappedChildCount[i] = new(i, childCount[i], distances[i], distancePoints[i]);
				}

				Array.Sort(mappedChildCount, (a, b) => -a.count.CompareTo(b.count));

				foreach(DistanceInfo distanceInfo in mappedChildCount)
				{
					if(distanceInfo.count < 2)
					{
						break;
					}

					if(distanceInfo.distance <= ChangeAndMergeThreshold)
					{
						continue;
					}

					Clusters.Add(Data[distanceInfo.distanceIndex].ValueCopy);

					if(Clusters.Count == ClusterLimit)
					{
						break;
					}
				}
			}
		}

		private void MergeClusters()
		{
			byte[][] copy = Clusters.Select(x => (byte[])x.Clone()).ToArray();
			Clusters.Clear();
			foreach(byte[] cluster in copy)
			{
				for(int i = 0; i < Clusters.Count; i++)
				{
					byte[] newCluster = Clusters[i];
					int distance = CalcMagnitude(cluster, newCluster);
					if(distance == 0)
					{
						goto skip;
					}
					else if(distance <= ChangeAndMergeThreshold)
					{
						for(int j = 0; j < Dimensions; j++)
						{
							newCluster[j] = (byte)((newCluster[j] + cluster[j]) / 2);
						}

						goto skip;
					}
				}

				Clusters.Add(cluster);
				skip:
				;
			}
		}

		private void Calculate()
		{
			int[,] newClusters = new int[Clusters.Count, Dimensions];
			int[] childCount = new int[Clusters.Count];

			for(int i = 0; i < MaxIterations; i++)
			{
				int distance = 0;

				for(int j = 0; j < Data.Length; j++)
				{
					VQDataPoint point = Data[j];
					int closest = GetClosestClusterIndex(point.values, out _);
					for(int k = 0; k < Dimensions; k++)
					{
						newClusters[closest, k] += point.values[k] * point.count;
					}

					childCount[closest] += point.count;
				}

				for(int j = Clusters.Count - 1; j >= 0; j--)
				{
					float divide = childCount[j];
					if(divide == 0)
					{
						Clusters.RemoveAt(j);
						continue;
					}

					byte[] newCluster = Clusters[j];
					int newDistance = 0;
					for(int k = 0; k < Dimensions; k++)
					{
						float component = newClusters[j, k] / divide;
						byte byteComponent = component % 1f > 0.5f ? (byte)(component + 1) : (byte)component;
						newDistance += (byteComponent - newCluster[k]) * (byteComponent - newCluster[k]);
						newCluster[k] = byteComponent;
					}

					distance = Math.Max(distance, newDistance);
				}

				if(Clusters.Count < ClusterLimit)
				{
					Fill();
				}
				else if(distance < ChangeAndMergeThreshold)
				{
					break;
				}

				Array.Clear(newClusters);
				Array.Clear(childCount);
			}
		}

		private int[] GetIndexMap(ReadOnlySpan<byte> data)
		{
			if(data.Length % Dimensions != 0)
			{
				throw new ArgumentException($"Data has invalid size! must be a multiple of {Dimensions}");
			}

			int[] indices = new int[data.Length / Dimensions];

			for(int i = 0; i < indices.Length; i++)
			{
				indices[i] = GetClosestClusterIndex(data[(i * Dimensions)..], out _);
			}

			return indices;
		}

		/// <summary>
		/// Vector quantizes byte data.
		/// </summary>
		/// <param name="data">Data to quantize.</param>
		/// <param name="dimensions">Number of dimensions per cluster to quantize to.</param>
		/// <param name="clusterLimit">Maximum number of clusters to calculate.</param>
		/// <param name="maxIterations">Maximum number of iterations. Higher number of iterations may result in more accurate results, but will take longer to process.</param>
		/// <param name="changeAndMergeThreshold">Value difference threshold below which values should be recalculated/merged.</param>
		/// <returns>Clusters and corresponding index mapping.</returns>
		/// <exception cref="ArgumentException"></exception>
		public static (int[] indices, byte[] clusters) QuantizeByteData(ReadOnlySpan<byte> data, int dimensions, int clusterLimit, int maxIterations = 10, int changeAndMergeThreshold = 31)
		{
			if(data.Length % dimensions != 0)
			{
				throw new ArgumentException($"Data has invalid size! must be a multiple of {dimensions}");
			}

			Dictionary<string, VQDataPoint> datapoints = [];
			for(int i = 0; i < data.Length; i += dimensions)
			{
				byte[] values = new byte[dimensions];
				for(int k = 0; k < dimensions; k++)
				{
					values[k] = data[i + k];
				}

				string hash = new(values.Select(x => (char)x).ToArray());

				if(datapoints.TryGetValue(hash, out VQDataPoint distinctCheck))
				{
					distinctCheck.count++;
					datapoints[hash] = distinctCheck;
				}
				else
				{
					datapoints.Add(hash, new(values, 1));
				}
			}

			VectorQuantization result = new(dimensions, clusterLimit, maxIterations, changeAndMergeThreshold, datapoints.Values.ToArray());

			if(!result.Init(data))
			{
				result.MergeClusters();
				result.Fill();
				result.Calculate();
			}

			int[] indices = result.GetIndexMap(data);
			byte[] clusters = result.Clusters.SelectMany(x => x).ToArray();

			return (indices, clusters);
		}
	}
}