using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyHelper;
using System.Windows;
using System.Net;

namespace ApexComm
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 对收到的数据进行处理
        /// </summary>
        /// <param name="msg"> 收到的数据包</param>
        /// <param name="pcip">本地终结点</param>
        /// <param name="clientendip">客户端地址信息</param>
        public static void ReceiveMsg(byte[] msg, IPEndPoint pcip, IPEndPoint clientendip)
        {
            //检测包
            if (CMDFactory.CheckReceiveMsg(msg))
            {
                //获取回复的命令码
                byte[] cmdcode = msg.CloneRange(20, 2);
                string sn = CMDFactory.ConvertbyteToStr(msg, 4, 16);
                MyDevice tdevice = null;
                try
                {
                    tdevice = DeviceCollection.First((D) => { return D.SN.Equals(sn) == true; });
                }
                catch
                {
                }

                //if(tdevice == null)
                //{
                //    tdevice = new MyDevice();
                //}

                if (BytesHelper.Equalbybyte(cmdcode, ApexComm.串口服务器.CMDCode.搜索设备.CMD_Receive))
                {
                    if (tdevice == null)
                    {
                        tdevice = new SerialDevice();
                        DeviceCollection.Add(tdevice);
                    }
                    SerialDevice device = tdevice as SerialDevice;
                    device.PC_Endpoint = pcip;
                    //此设备非广播包不回复
                    device.Client_Endpoint = new System.Net.IPEndPoint(IPAddress.Parse("255.255.255.255"), 61199); ;
                    device.SN = sn;
                    device.PVer = "Ver " + msg[25].ToString("X2") + "." + msg[24].ToString("X2");
                    device.SVer = "Ver " + msg[27].ToString("X2") + "." + msg[26].ToString("X2");
                    device.Net1_IP = BytesHelper.ToHexString_10(msg.CloneRange(28, 4), ".");
                    device.Net1_Mask = BytesHelper.ToHexString_10(msg.CloneRange(32, 4), ".");
                    device.Net2_IP = BytesHelper.ToHexString_10(msg.CloneRange(36, 4), ".");
                    device.Net2_Mask = BytesHelper.ToHexString_10(msg.CloneRange(40, 4), ".");
                    device.Name = CMDFactory.ConvertbyteToStr(msg, 44, 40);
                    device.TypeName = CMDFactory.ConvertbyteToStr(msg, 84, 20).Trim();
                }
            }
        }
    }
}