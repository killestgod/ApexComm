using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ApexComm
{
    public class UDPMessage
    {
        public static byte[] SendAndWait(MyDevice device, byte[] bytes)
        {
            byte[] datas = null;
            try
            {
                UdpClient tempudp = new UdpClient(device.PC_Endpoint);
                tempudp.Client.ReceiveTimeout = 3000;
                tempudp.Client.SendTimeout = 3000;
                tempudp.Send(bytes, bytes.Length, device.Client_Endpoint);

                //读取信息
                datas = tempudp.Receive(ref device.Client_Endpoint);
                tempudp.Close();
            }
            catch
            {
            }
            return datas;
        }
    }
}