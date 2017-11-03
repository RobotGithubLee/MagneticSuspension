using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace MagneticSuspension
{
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    struct U_sendData
    {
        [FieldOffset(0)]
        public byte b0;
        [FieldOffset(1)]
        public byte b1;
        [FieldOffset(2)]
        public byte b2;
        [FieldOffset(3)]
        public byte b3;

        [FieldOffset(0)]
        public float f;
    }


    [StructLayout(LayoutKind.Explicit, Size = 2)]
    struct U_Revise
    {
        [FieldOffset(0)]
        public byte b0;
        [FieldOffset(1)]
        public byte b1;

        [FieldOffset(0)]
        public short s;
    }





    [StructLayoutAttribute(LayoutKind.Explicit, Pack = 1)]
    public unsafe struct U_rcvData
    {
        [FieldOffset(0)]
        public fixed byte RevHead[2];

        [FieldOffset(2)]
        public fixed short W[6];

        [FieldOffset(14)]
        public fixed short I[6];

        [FieldOffset(26)]
        public fixed short duty[6];

        [FieldOffset(38)]
        public int speed;


        [FieldOffset(0)]
        public fixed byte rcvBuf[42];
    };


    class structClass
    {
    }
}
