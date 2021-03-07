using System.IO;

namespace BattleSimulator.Extensions
{
    public static class BinaryWriterExtensions 
    {
        public static void WriteFourCC(this BinaryWriter writer, byte a, byte b, byte c, byte d)
        {
            writer.Write(a);
            writer.Write(b);
            writer.Write(c);
            writer.Write(d);
        }

        public static void WriteFourCC(this BinaryWriter writer, char a, char b, char c, char d)
        {
            writer.Write((byte)a);
            writer.Write((byte)b);
            writer.Write((byte)c);
            writer.Write((byte)d);
        }
    }
}
