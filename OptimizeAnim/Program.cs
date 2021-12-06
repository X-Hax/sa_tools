using SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace OptimizeAnim
{
	static class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Thread t = new Thread((ThreadStart)(() =>
				{
					using (OpenFileDialog ofd = new OpenFileDialog()
					{
						Filter = "Sonic Adventure Animation|*.saanim|All Files|*.*",
						Multiselect = true
					})
					{
						if (ofd.ShowDialog() == DialogResult.OK)
						{
							Convert(ofd.FileNames);
						}
					}
				}));
				t.SetApartmentState(ApartmentState.STA);
				t.Start();
			}
			else
			{
				Convert(args);
			}
		}

		static int removedFrames;

		private static void Convert(string[] args)
		{
			foreach (string filename in args)
			{
				Console.WriteLine(filename);
				removedFrames = 0;
				NJS_MOTION mtn = NJS_MOTION.Load(filename);
				foreach (var node in mtn.Models)
				{
					CheckValues(node.Value.Position);
					CheckValues(node.Value.Rotation);
					CheckValues(node.Value.Scale);
					CheckValues(node.Value.Vector);
					CheckValues(node.Value.Vertex);
					CheckValues(node.Value.Normal);
					CheckValues(node.Value.Target);
					CheckValues(node.Value.Roll);
					CheckValues(node.Value.Angle);
					CheckValues(node.Value.Color);
					CheckValues(node.Value.Intensity);
					CheckValues(node.Value.Spot);
					CheckValues(node.Value.Point);
				}
				if (removedFrames > 0)
				{
					Console.WriteLine("Removed {0} frame{1}.", removedFrames, removedFrames == 1 ? "" : "s");
					mtn.Save(filename);
				}
			}
		}

		private static void CheckValues<T>(Dictionary<int, T> position)
			where T : IEquatable<T>
		{
			if (position?.Count > 2)
			{
				var nodelist = position.ToArray();
				T lastFrame = nodelist[0].Value;
				for (int i = 1; i < nodelist.Length - 1; i++)
				{
					if (nodelist[i].Value.Equals(lastFrame) && nodelist[i].Value.Equals(nodelist[i + 1].Value))
					{
						position.Remove(nodelist[i].Key);
						++removedFrames;
					}
					else
						lastFrame = nodelist[i].Value;
				}
			}
		}

		private static void CheckValues<T>(Dictionary<int, T[]> position)
			where T : IEquatable<T>
		{
			if (position?.Count > 2)
			{
				var nodelist = position.ToArray();
				T[] lastFrame = nodelist[0].Value;
				for (int i = 1; i < nodelist.Length - 1;)
				{
					if (nodelist[i].Value.SequenceEqual(lastFrame) && nodelist[i].Value.SequenceEqual(nodelist[i + 1].Value))
					{
						position.Remove(nodelist[i].Key);
						++removedFrames;
					}
					else
						lastFrame = nodelist[i++].Value;
				}
			}
		}
	}
}
