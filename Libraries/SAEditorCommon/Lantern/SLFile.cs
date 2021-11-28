using SAModel.Direct3D;
using SharpDX;
using System.Collections.Generic;
using Color = System.Drawing.Color;

namespace SAModel.SAEditorCommon
{
	public class SLFile
	{
        public int RotationY;
        public int RotationZ;
        // Stuff below is not used for palettes
        public Color EnvRGB;
        public float EnvSpecularMultiplier;
        public float EnvDiffuseMultiplier;
        public float EnvAmbientMultiplier;
        public float FreeSlaveRD;
        public int FreeSlaveRT;
        public Color FreeSlaveRGB;

        public SLFile(byte[] data, bool bigendian = false)
        {
            bool bigendianbk = ByteConverter.BigEndian;
            ByteConverter.BigEndian = bigendian;
            int start = 0x5A0;
            // Direction    
            RotationY = ByteConverter.ToInt32(data, start);
            RotationZ = ByteConverter.ToInt32(data, start + 4);
            // Env RGB
            float r = 255 * ByteConverter.ToSingle(data, start + 8);
            float g = 255 * ByteConverter.ToSingle(data, start + 12);
            float b = 255 * ByteConverter.ToSingle(data, start + 16);
            EnvRGB = Color.FromArgb((int)r, (int)g, (int)b);
            // Multipliers
            EnvSpecularMultiplier = ByteConverter.ToSingle(data, start + 20);
            EnvDiffuseMultiplier = ByteConverter.ToSingle(data, start + 24);
            EnvAmbientMultiplier = ByteConverter.ToSingle(data, start + 28);
            // Free/Slave Data
            FreeSlaveRD = ByteConverter.ToSingle(data, start + 32);
            FreeSlaveRT = (int)(255 * ByteConverter.ToSingle(data, start + 36));
            //System.Windows.Forms.MessageBox.Show(EnvRGB.ToString());
            float sr = 255 * ByteConverter.ToSingle(data, start + 40);
            float sg = 255 * ByteConverter.ToSingle(data, start + 44);
            float sb = 255 * ByteConverter.ToSingle(data, start + 48);
            FreeSlaveRGB = Color.FromArgb((int)sr, (int)sg, (int)sb);
            ByteConverter.BigEndian = bigendianbk;
        }

        public SLFile() { }

        public Vector3 GetLightDirection()
        {
            MatrixStack matrixStack = new MatrixStack();
            matrixStack.NJRotateY(RotationY);
            matrixStack.NJRotateZ(RotationZ);
            return Vector3.TransformCoordinate(Vector3.Down, matrixStack.Top);
        }

        public Vector3 GetLightDirection(int rot_y, int rot_z)
        {
            MatrixStack matrixStack = new MatrixStack();
            matrixStack.NJRotateY(rot_y);
            matrixStack.NJRotateZ(rot_z);
            return Vector3.TransformCoordinate(Vector3.Down, matrixStack.Top);
        }

        public byte[] GetBytes(bool bigendian = false)
        {
            bool bigendianbk = ByteConverter.BigEndian;
            ByteConverter.BigEndian = bigendian;
            List<byte> result = new List<byte>();
            for (int i = 0; i < 0x5A0; i++)
                result.Add(0);
            result.AddRange(ByteConverter.GetBytes(RotationY));
            result.AddRange(ByteConverter.GetBytes(RotationZ));
            result.AddRange(ByteConverter.GetBytes((float)EnvRGB.R / 255.0f)); 
            result.AddRange(ByteConverter.GetBytes((float)EnvRGB.G / 255.0f));
            result.AddRange(ByteConverter.GetBytes((float)EnvRGB.B / 255.0f));
            result.AddRange(ByteConverter.GetBytes(EnvSpecularMultiplier));
            result.AddRange(ByteConverter.GetBytes(EnvDiffuseMultiplier));
            result.AddRange(ByteConverter.GetBytes(EnvAmbientMultiplier));
            result.AddRange(ByteConverter.GetBytes(FreeSlaveRD));
            result.AddRange(ByteConverter.GetBytes((float)FreeSlaveRT / 255.0f));
            result.AddRange(ByteConverter.GetBytes((float)FreeSlaveRGB.R / 255.0f));
            result.AddRange(ByteConverter.GetBytes((float)FreeSlaveRGB.G / 255.0f));
            result.AddRange(ByteConverter.GetBytes((float)FreeSlaveRGB.B / 255.0f));
            ByteConverter.BigEndian = bigendianbk;
            return result.ToArray();
        }
    }
}
