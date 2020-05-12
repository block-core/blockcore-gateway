using System.Runtime.InteropServices;

namespace Blockcore.Platform.Networking
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HeaderInfo
    {
        public int Size;
        public ushort Command;
    }
}
