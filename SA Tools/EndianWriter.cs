using SonicRetro.SAModel;
using System;
using System.IO;
using System.Text;

namespace SA_Tools
{
    public class EndianWriter : BinaryWriter
    {
        public EndianWriter()
            : base() { }

        public EndianWriter(Stream output)
            : base(output) { }

        public EndianWriter(Stream output, Encoding encoding)
            : base(output, encoding) { }

        public override void Write(decimal value)
        {
            throw new NotSupportedException();
        }

        public override void Write(double value)
        {
            Write(ByteConverter.GetBytes(value));
        }

        public override void Write(short value)
        {
            Write(ByteConverter.GetBytes(value));
        }

        public override void Write(int value)
        {
            Write(ByteConverter.GetBytes(value));
        }

        public override void Write(long value)
        {
            Write(ByteConverter.GetBytes(value));
        }

        public override void Write(float value)
        {
            Write(ByteConverter.GetBytes(value));
        }

        public override void Write(ushort value)
        {
            Write(ByteConverter.GetBytes(value));
        }

        public override void Write(uint value)
        {
            Write(ByteConverter.GetBytes(value));
        }

        public override void Write(ulong value)
        {
            Write(ByteConverter.GetBytes(value));
        }
    }
}
