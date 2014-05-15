using System;
using System.IO;
using System.Text;

namespace SA_Tools
{
    public class EndianReader : BinaryReader
    {
        public EndianReader(Stream input)
            : base(input) { }

        public EndianReader(Stream input, Encoding encoding)
            : base(input, encoding) { }

        public override decimal ReadDecimal()
        {
            throw new NotImplementedException();
        }

        public override double ReadDouble()
        {
            return ByteConverter.ToDouble(ReadBytes(sizeof(double)), 0);
        }

        public override short ReadInt16()
        {
            return ByteConverter.ToInt16(ReadBytes(sizeof(short)), 0);
        }

        public override int ReadInt32()
        {
            return ByteConverter.ToInt32(ReadBytes(sizeof(int)), 0);
        }

        public override long ReadInt64()
        {
            return ByteConverter.ToInt64(ReadBytes(sizeof(long)), 0);
        }

        public override float ReadSingle()
        {
            return ByteConverter.ToSingle(ReadBytes(sizeof(float)), 0);
        }

        public override ushort ReadUInt16()
        {
            return ByteConverter.ToUInt16(ReadBytes(sizeof(ushort)), 0);
        }

        public override uint ReadUInt32()
        {
            return ByteConverter.ToUInt32(ReadBytes(sizeof(uint)), 0);
        }

        public override ulong ReadUInt64()
        {
            return ByteConverter.ToUInt64(ReadBytes(sizeof(ulong)), 0);
        }
    }
}
