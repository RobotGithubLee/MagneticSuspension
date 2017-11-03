using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MagneticSuspension
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PVCI_INIT_CONFIG
    {
        public uint AccCode;
        public uint AccMask;
        public uint Reserved;
        public byte Filter;
        public byte kCanBaud;
        public byte Timing0;
        public byte Timing1;
        public byte Mode;
        public byte CanRx_IER;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct PVCI_CAN_OBJ
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]//声明数组大小为4
        public byte[] ID; //4
        public UInt32 TimeStamp;//C++的UINT对应32位
        public byte TimeFlag;
        public byte SendType;
        public byte RemoteFlag; //=0
        public byte ExternFlag; //=1
        public byte DataLen; //=8
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]//声明数组大小为8
        public byte[] Data; //8
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]//声明数组大小为3
        public byte[] Reserved;//3
    }

    public class CANClass
    {


        [DllImport("VCI_CAN.dll", EntryPoint = "VCI_OpenDevice")]
        public static extern int VCI_OpenDevice(uint Devtype, uint Devindex, uint Reserved);
        [DllImport("VCI_CAN.dll", EntryPoint = "VCI_StartCAN")]
        public static extern int VCI_StartCAN(uint Devtype, uint Devindex, uint CANInd);
        [DllImport("VCI_CAN.dll", EntryPoint = "VCI_CloseDevice")]
        public static extern int VCI_CloseDevice(int DeviceType, int DeviceInd);
        [DllImport("VCI_CAN.dll", EntryPoint = "VCI_SetReference")]
        public static extern int VCI_SetReference(uint DeviceType, uint DeviceInd, uint CANInd, uint RefType, ref byte pData);
        [DllImport("VCI_CAN.dll", EntryPoint = "VCI_InitCAN")]
        public static extern int VCI_InitCAN(uint DevType, uint DevIndex, uint CANIndex, ref PVCI_INIT_CONFIG pInitConfig);
        [DllImport("VCI_CAN.dll", EntryPoint = "VCI_Transmit")]
        public static extern int VCI_Transmit(uint DevType, uint DevIndex, uint CANIndex, ref PVCI_CAN_OBJ pSend);
        [DllImport("VCI_CAN.dll", EntryPoint = "VCI_Receive")]
        public static extern int VCI_Receive(uint DevType, uint DevIndex, uint CANIndex, [Out]/*声明参数为输出*/ PVCI_CAN_OBJ[] pReceive);


        static uint DeviceType = 3;
        //  static uint DevIndex = 0;
        //  static uint CANIndex = 1;

        public static int sendMotorBuf(uint DevIndex, uint CANIndex,byte code, byte iref_int77, byte iref_int88)
        {
            PVCI_CAN_OBJ sendbuf = new PVCI_CAN_OBJ();
            sendbuf.ID = new byte[4];
            sendbuf.Data = new byte[8];
            sendbuf.Reserved = new byte[3];

            sendbuf.ExternFlag = 0;
            sendbuf.DataLen = 8;
            sendbuf.RemoteFlag = 0;
            sendbuf.ID[0] = 0x00;// SJA1000 mode
            sendbuf.ID[1] = 0x60;// ID=3
            sendbuf.ID[2] = 0x00;
            sendbuf.ID[3] = 0x00;
            sendbuf.Data[0] = 0x2;// ID=1
            sendbuf.Data[1] = 0x0;
            sendbuf.Data[2] = 0x0;
            sendbuf.Data[3] = 0x0;
            sendbuf.Data[4] = iref_int77;
            sendbuf.Data[5] = iref_int88;
            sendbuf.Data[6] = code;
            sendbuf.Data[7] = 0x0;

            return VCI_Transmit(DeviceType, DevIndex, CANIndex, ref sendbuf);//CAN DATA SEND;

        }

        public static int sendMagnBuf(uint DevIndex, uint CANIndex,byte code, byte iref_int77, byte iref_int88)
        {
            PVCI_CAN_OBJ sendbuf = new PVCI_CAN_OBJ();
            sendbuf.ID = new byte[4];
            sendbuf.Data = new byte[8];
            sendbuf.Reserved = new byte[3];

            sendbuf.ExternFlag = 0;
            sendbuf.DataLen = 8;
            sendbuf.RemoteFlag = 0;
            sendbuf.ID[0] = 0x00;// SJA1000 mode
            sendbuf.ID[1] = 0x20;// ID=3
            sendbuf.ID[2] = 0x00;
            sendbuf.ID[3] = 0x00;
            sendbuf.Data[0] = 0x3;// ID=3
            sendbuf.Data[1] = 0x0;
            sendbuf.Data[2] = code;
            sendbuf.Data[3] = 0x0;
            sendbuf.Data[4] = iref_int77;
            sendbuf.Data[5] = iref_int88;
            sendbuf.Data[6] = 0x0;
            sendbuf.Data[7] = 0x0;

            return VCI_Transmit(DeviceType, DevIndex, CANIndex, ref sendbuf);//CAN DATA SEND;

        }




        public static int rcvBuf(uint DevIndex, uint CANIndex, out PVCI_CAN_OBJ[] pCanObj)
        {
            pCanObj = new PVCI_CAN_OBJ[300];

            return VCI_Receive(DeviceType, DevIndex, CANIndex, pCanObj);
        }

        public static int closeCAN(uint DevIndex)
        {
            int count = CANClass.VCI_CloseDevice((int)DeviceType, 0);
            if (count == 1)
            {
                return 0;
            }
            else if (count == 0)
            {
                MessageBox.Show("close fail");
                return -1;
            }
            else
            {
                MessageBox.Show("device not open");
            }
            return 0;
        }

        public static int initCAN(uint DevIndex, uint CANIndex)
        {
            if (VCI_OpenDevice(DeviceType, DevIndex, 0) != 1)
            {
                MessageBox.Show("Open fail 0");
                return -1;
            }
            else
            {
                PVCI_INIT_CONFIG[] config = new PVCI_INIT_CONFIG[1];
                config[0].AccCode = 0x80000008;
                config[0].AccMask = 0xFFFFFFFF;
                config[0].Reserved = 204;
                config[0].Filter = 0;
                config[0].kCanBaud = 15;
                config[0].Timing0 = 0x00;
                config[0].Timing1 = 0x14;
                config[0].CanRx_IER = 1;
                config[0].Mode = 0;


                if (VCI_InitCAN(DeviceType, DevIndex, CANIndex, ref config[0]) != 1)
                {
                    //MessageBox.Show("Init fail");
                    //return -1;
                }


                if (VCI_StartCAN(DeviceType, DevIndex, CANIndex) != 1)
                {
                    MessageBox.Show("Start fail");
                    return -1;
                }

            }

            return 0;
        }

        public static int initSendBuffCAN(uint DevIndex, uint CANIndex)
        {

            PVCI_CAN_OBJ sendbuf = new PVCI_CAN_OBJ();

            sendbuf.ID = new byte[4];
            sendbuf.Data = new byte[8];
            sendbuf.Reserved = new byte[3];
            string[] str5 = new string[32];
            byte[] buf = new byte[50];
            byte[] SendID = new byte[10];

            sendbuf.ExternFlag = 0;
            sendbuf.DataLen = 8;
            sendbuf.RemoteFlag = 0;
            sendbuf.ID[0] = 0x00;// SJA1000 mode
            sendbuf.ID[1] = 0x60;// ID=3
            sendbuf.ID[2] = 0x00;
            sendbuf.ID[3] = 0x00;
            sendbuf.Data[0] = 0x2;
            sendbuf.Data[1] = 0x0;
            sendbuf.Data[2] = 0x0;
            sendbuf.Data[3] = 0x0;
            sendbuf.Data[4] = 0x1;
            sendbuf.Data[5] = 0x0;
            sendbuf.Data[6] = 0x0;
            sendbuf.Data[7] = 0x0;
            int flag = VCI_Transmit(DeviceType, DevIndex, CANIndex, ref sendbuf);



            if (flag != 1)
            {
                MessageBox.Show("开启发送数据失败");
                return -1;
            }


            sendbuf.Data[6] = 0x9;

            flag = VCI_Transmit(DeviceType, DevIndex, CANIndex, ref sendbuf);


            if (flag != 1)
            {
                MessageBox.Show("开启接受数据失败");
                return -1;
            }

            return 0;
        }

    }
}
