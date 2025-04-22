using System;
using System.Collections.Generic;
using System.IO;

namespace SAModel
{
	// NJS_CAMERA
	public class NinjaCamera
	{
		public string Name; // NJS_CAMERA name
		public string ActionName { get; set; } // NJS_CACTION name
		public string MotionName { get; set; } // NJS_MOTION name
		public Vertex Position { get; set; } // Camera position
		public Vertex Vector { get; set; } // Camera vector in unit direction[Local Z axis]
		public int Roll { get; set; } // Camera roll
		public int Angle { get; set; } // Camera angle
		public float NearClip { get; set; } // Near clip 
		public float FarClip { get; set; } // Far clip
		public Vertex LocalX { get; set; } // Camera local X axis
		public Vertex LocalY { get; set; } // Camera local Y axis

		public NinjaCamera(byte[] file, int address, Dictionary<int, string> labels = null)
		{
			if (labels != null && labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "camera_" + address.ToString("X8");
			Position = new Vertex(file, address);
			Vector = new Vertex(file, address + 12);
			Roll = ByteConverter.ToInt32(file, address + 24);
			Angle = ByteConverter.ToInt32(file, address + 28);
			NearClip = ByteConverter.ToSingle(file, address + 32);
			FarClip = ByteConverter.ToSingle(file, address + 36);
			LocalX = new Vertex(file, address + 40);
			LocalY = new Vertex(file, address + 52);
		}

		public NinjaCamera() { }

		public static int Size { get { return 0x40; } }

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size);
			result.AddRange(Position.GetBytes());
			result.AddRange(Vector.GetBytes());
			result.AddRange(ByteConverter.GetBytes(Roll));
			result.AddRange(ByteConverter.GetBytes(Angle));
			result.AddRange(ByteConverter.GetBytes(NearClip));
			result.AddRange(ByteConverter.GetBytes(FarClip));
			result.AddRange(LocalX.GetBytes());
			result.AddRange(LocalY.GetBytes());
			result.Align(0x40);
			return result.ToArray();
		}

		public void ToNJA(TextWriter writer, List<string> labels = null)
		{
			writer.WriteLine("CAMERA    " + Name + "[]");
			writer.WriteLine("CAMERA_START");
			writer.WriteLine("CPosition    " + Position.ToNJA() + ",");
			writer.WriteLine("CInterest    " + Vector.ToNJA() + ",");
			writer.WriteLine("CRoll        " + "( " + (Roll / 182.044f).ToNJA() + " ),");
			writer.WriteLine("CAngle       " + "( " + (Angle / 182.044f).ToNJA() + " ),");
			writer.WriteLine("CNearClip    " + "( " + NearClip.ToNJA() + " ),");
			writer.WriteLine("CFarClip     " + "( " + FarClip.ToNJA() + " ),");
			writer.WriteLine("CAMERA_END");
			if (!string.IsNullOrEmpty(ActionName) && !string.IsNullOrEmpty(MotionName))
			{
				writer.WriteLine("\nCAMERA_ACTION {0}[]", ActionName.MakeIdentifier());
				writer.WriteLine("START");
				writer.WriteLine("CameraObj       {0},", Name.MakeIdentifier());
				writer.WriteLine("Motion          " + MotionName.MakeIdentifier());
				writer.Write("END");
			}
		}
	}

	// NJS_CACTION
	public class NinjaCameraAction
	{
		public string Name;
		public NinjaCamera Camera;
		public NJS_MOTION Motion;

		public NinjaCameraAction(byte[] file, int address, uint imageBase, Dictionary<int, string> labels)
		{
			if (labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "caction_" + address.ToString("X8");
			uint pointer_camera = ByteConverter.ToUInt32(file, address);
			uint pointer_motion = ByteConverter.ToUInt32(file, address + 0x4);
			Camera = new NinjaCamera(file, (int)(pointer_camera - imageBase), labels);
			Motion = new NJS_MOTION(file, (int)(pointer_motion - imageBase), imageBase, 1, labels);
		}

		public void ToNJA(TextWriter writer, List<string> labels = null)
		{
			Camera.ActionName = Name;
			Camera.MotionName = Motion.Name;
			Motion.ToNJA(writer, labels, camera: true, exportDefaults: false);
			writer.WriteLine();
			Camera.ToNJA(writer, labels);
		}
	}
}