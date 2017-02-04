using MyHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ApexComm;

using System.Threading;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace ApexComm.串口服务器
{
    /// <summary>
    /// SerialDeviceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SerialDeviceWindow : Window
    {
        public SerialDevice MyDevice = new SerialDevice();

        /// <summary>
        /// 写入后 收到的回复信息
        /// </summary>
        private Dictionary<byte, string> WriteResult = new Dictionary<byte, string>() { };

        public SerialDeviceWindow()
        {
            MyDevice.PC_Endpoint = new System.Net.IPEndPoint(IPAddress.Parse("192.168.1.71"), 61199);
            MyDevice.Client_Endpoint = new System.Net.IPEndPoint(IPAddress.Parse("255.255.255.255"), 61199);
            MyDevice.SN = "P2084ECT16122003";
            InitializeComponent();
            List<string> netmodes = new List<string>()
            {
                "冷备用-不可同时访问,A网口抢占式优先",
                "热备用-可同时接收数据,一个网口发送有效,A网口抢占式优先",
                "自由切换-不可同时访问,后建立链接者抢占式优先",
                "等同备用(默认)-可同时访问,既能接收,又能发送",
            };
            comboBox_backmode.ItemsSource = netmodes;

            WriteResult.Add(0x6, "设置读取/修改成功");
            WriteResult.Add(0x80, "设置长度错误（针对所有设置内容）");
            WriteResult.Add(0x81, " 设置CRC错误（针对所有设置内容）");
            WriteResult.Add(0x82, " 设置版本错误（针对所有设置内容）");
            WriteResult.Add(0x83, " 装置类型错误（针对所有设置内容）");
            WriteResult.Add(0x84, " 工作模式错误");
            WriteResult.Add(0x85, " 网络参数错误");
            WriteResult.Add(0x86, " 串口参数错误");
            WriteResult.Add(0x87, " 外部驱动接口类型错误");
            WriteResult.Add(0x88, " 主动链接参数错误");
            WriteResult.Add(0x89, " 链接保持参数错误");
            WriteResult.Add(0x8A, " TCP链接切换模式错误");
            WriteResult.Add(0x8B, " 装置名称无效");
            WriteResult.Add(0x8C, " 维护口令无效");
            WriteResult.Add(0x8D, " 装置ID错误");
        }

        private void btn_read_Click(object sender, RoutedEventArgs e)
        {
            SendAndWait(SerialDeviceProc.ReadAllCfg(MyDevice), CMDCode.获取所有配置.CMD_Receive);
        }

        private string hex2;
        private UdpClient Udpclient;

        /// <summary>
        /// 信息框要显示的内容
        /// </summary>
        private string Msgstr;

        /// <summary>
        /// 最近收到的命令码
        /// </summary>
        private byte[] LastCMD = new byte[] { 0x00, 0x00 };

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //textBox_Name.Text = MyDevice.Name;
            // string hex = "eb:90:eb:90:32:50:36:31:45:34:54:43:36:31:31:31:38:32:36:30:aa:aa:d2:04:00:00:50:41:58:45:2d:04:08:02:0e:11:37:1e:2d:04:06:03:12:0f:0a:1e:32:4e:31:53:2d:36:38:34:43:35:45:2d:00:54:00:00:00:00:00:00:32:50:36:31:45:34:54:43:36:31:31:31:38:32:36:30:00:00:00:00:01:00:01:00:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:ff:ff:ff:ff:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:ff:ff:ff:ff:ff:ff:ff:ff:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:ed:13:ee:13:ef:13:f0:13:f1:13:f2:13:f3:13:f4:13:f5:13:f6:13:f7:13:f8:13:f9:13:fa:13:fb:13:fc:13:fd:13:fe:13:ff:13:00:14:01:14:02:14:03:14:04:14:05:14:06:14:07:14:08:14:09:14:0a:14:0b:14:0c:14:ed:13:ee:13:ef:13:f0:13:f1:13:f2:13:f3:13:f4:13:f5:13:f6:13:f7:13:f8:13:f9:13:fa:13:fb:13:fc:13:fd:13:fe:13:ff:13:00:14:01:14:02:14:03:14:04:14:05:14:06:14:07:14:08:14:09:14:0a:14:0b:14:0c:14:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:a8:c0:02:00:a8:c0:03:00:ff:ff:00:ff:ff:ff:00:ff:a8:c0:01:00:a8:c0:01:00:ed:13:ed:13:00:00:00:00:c0:00:a8:c0:02:00:c0:00:a8:c0:03:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:73:55:72:65:32:31:61:4e:65:6d:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:32:31:34:33:36:35:38:37:50:41:58:45:00:00:1e:7a:64:ba:03:03";
            string hex = "EB 90 EB 90 32 50 38 30 45 34 54 43 36 31 32 31 30 32 33 30 AA AA D2 04 00 00 50 41 58 45 2D 04 08 02 0E 11 37 1E 2D 04 06 03 12 0F 0A 1E 32 4E 30 53 2D 38 38 34 43 35 45 2D 00 54 00 00 00 00 00 00 32 50 38 30 45 34 54 43 36 31 32 31 30 32 33 30 00 00 00 00 01 00 01 00 0A 00 00 8C 07 00 00 08 05 00 00 46 03 00 00 84 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 07 08 05 06 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 01 00 03 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 02 00 00 00 02 00 01 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 00 00 00 00 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 FF FF FF FF FF FF FF FF A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 A8 C0 02 00 A8 C0 03 00 FF FF 00 FF FF FF 00 FF A8 C0 01 00 A8 C0 01 00 ED 13 ED 13 00 00 00 00 C0 00 A8 C0 02 00 C0 00 A8 C0 03 00 FF FF 00 00 01 00 00 00 FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 AC 26 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 73 55 72 65 31 31 31 31 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 32 31 34 33 36 35 38 37 50 41 58 45 00 00 79 F4 64 BB 03 03";
            //写入
            hex2 = "EB 90 EB 90 32 50 38 30 45 34 54 43 36 31 32 31 30 32 33 30 5B 5B D8 04 81 90 85 98 94 85 83 88 50 41 58 45 2D 04 08 02 0E 11 37 1E 2D 04 06 03 12 0F 0A 1E 32 4E 30 53 2D 38 38 34 43 35 45 2D 00 54 00 00 00 00 00 00 32 50 38 30 45 34 54 43 36 31 32 31 30 32 33 30 00 00 00 00 01 00 01 00 0A 00 00 8C 07 00 00 08 05 00 00 46 03 00 00 84 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 07 08 05 06 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 01 00 03 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 02 00 00 00 02 00 01 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 00 00 00 00 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 FF FF FF FF FF FF FF FF A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 A8 C0 02 00 A8 C0 03 00 FF FF 00 FF FF FF 00 FF A8 C0 01 00 A8 C0 01 00 ED 13 ED 13 00 00 00 00 C0 00 A8 C0 02 00 C0 00 A8 C0 03 00 FF FF 00 00 01 00 00 00 FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 AC 26 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 73 55 72 65 31 31 31 31 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 32 31 34 33 36 35 38 37 50 41 58 45 00 00 79 F4 8C 4F 03 00";
            byte[] msg = BytesHelper.StrToHexBytes(hex, " ");

            //初始化udp通讯
            Udpclient = new UdpClient(MyDevice.PC_Endpoint);
            Udpclient.BeginReceive(ReceiveCallback, Udpclient);
            ReceiveMsg(msg);
        }

        /// <summary>
        /// UDP数据接收处理
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient udpClient = ar.AsyncState as UdpClient;
            try
            {
                //客户端ip
                IPEndPoint clientendip = new IPEndPoint(IPAddress.Any, 61199);
                //接收数据，并保存到Byte数组里 
                byte[] ReceiveByte = udpClient.EndReceive(ar, ref clientendip);
                udpClient.BeginReceive(ReceiveCallback, ar.AsyncState);//重新开始监听

                IPEndPoint localendpoint = udpClient.Client.LocalEndPoint as IPEndPoint;

                // Console.WriteLine("clientendip:" + clientendip + "     localendpoint" + localendpoint);
                if (localendpoint.Address.ToString() == clientendip.Address.ToString())
                {
                    return;
                }

                Dispatcher.Invoke(new Action<byte[]>(ReceiveMsg), ReceiveByte);
            }
            catch (ObjectDisposedException ex)
            {
            }
            catch (Exception ex)
            {
                LogMsg.log_error("通讯失败:" + ex.ToString());
            }
        }

        /// <summary>
        /// 对收到的数据进行处理
        /// </summary>
        /// <param name="msg"> 收到的数据包</param>
        /// <param name="pcip">本地终结点</param>
        /// <param name="clientendip">客户端地址信息</param>
        public void ReceiveMsg(byte[] msg)
        {
            //检测包
            if (CMDFactory.CheckReceiveMsg(msg))
            {
                try
                {
                    //获取回复的命令码
                    byte[] cmdcode = msg.CloneRange(20, 2);
                    LastCMD = cmdcode;

                    string sn = CMDFactory.ConvertbyteToStr(msg, 4, 16);
                    if (sn != MyDevice.SN)
                    {
                        return;
                    }

                    //读取上来的配置
                    if (BytesHelper.Equalbybyte(cmdcode, ApexComm.串口服务器.CMDCode.获取所有配置.CMD_Receive))
                    {
                        sp_state.Children.Clear();
                        if (msg[26] != 0x50 || msg[27] != 0x41)
                        {
                            MessageBox.Show("设置内容的标识不正确,读取的设置无效");
                            return;
                        }

                        // MyDevice.PVer = "Ver " + msg[39].ToString("X2") + "." + msg[38].ToString("X2");
                        SerialDevice_Struct ss = StructWithBytes.ByteToStruct<SerialDevice_Struct>(msg);
                        MyDevice.Struct_SS = ss;

                        ReShowUI(MyDevice);
                        SerialCfg.SetDevice(MyDevice);
                        netcfgA.NetNum = 0;
                        netcfgB.NetNum = 1;
                        netcfgA.SetDevice(MyDevice);
                        netcfgB.SetDevice(MyDevice);
                    }
                    else if (BytesHelper.Equalbybyte(cmdcode, ApexComm.串口服务器.CMDCode.保存所有配置.CMD_Receive))
                    {
                        byte[] xx = CMDFactory.GetBody(msg);
                        string resultstr = WriteResult[xx[0]];
                        //MessageBox.Show(resultstr);
                    }
                    else if (BytesHelper.Equalbybyte(cmdcode, ApexComm.串口服务器.CMDCode.复位CPU.CMD_Receive))
                    {
                        LogMsg.log("复位CPU完成");
                    }
                    else if (BytesHelper.Equalbybyte(cmdcode, ApexComm.串口服务器.CMDCode.搜索设备.CMD_Receive))
                    {
                        LogMsg.log("搜索设备完成");
                    }
                    else
                    {
                        Console.WriteLine("未知命令码  " + LastCMD.ToHexString());
                    }
                }
                catch (Exception ex)
                {
                    LogMsg.log_error("解析失败:" + ex.ToString());
                    MessageBox.Show("解析失败:" + ex.ToString());
                }
            }
        }

        /// <summary>
        /// 根据结构体信息 更新UI
        /// </summary>
        /// <param name="ss"></param>
        private void ReShowUI(SerialDevice sd)
        {
            SerialDevice_Struct ss = sd.Struct_SS;
            sp_state.Children.Clear();
            sd.Name = ss.DeviceName.ConvertbyteToStr();
            //便签占用的长度
            const int lbllength = -5;
            sd.SN = ss.sn.ConvertbyteToStr();
            sp_state.Children.Add(new TextBlock() { Text = $"{"SN",lbllength}: {sd.SN}" });
            sp_state.Children.Add(new TextBlock() { Text = $"{"名字:",lbllength}: {sd.Name}" });
            sp_state.Children.Add(new TextBlock() { Text = $"{"当前数据获取方式",lbllength}: {ss.cmdtype.ToHexString()}" });
            sp_state.Children.Add(new TextBlock() { Text = $"{"有效设置标识",lbllength}: {ss.setFlagA_B.ConvertbyteToStr()}" });
            sp_state.Children.Add(new TextBlock() { Text = $"{"程序版本pVer",lbllength}: {ss.pVer.ToHexString()}" });
            sp_state.Children.Add(new TextBlock() { Text = $"{"编译版本日期",lbllength}: {ss.pDate.ToHexString_10()}" });
            sp_state.Children.Add(new TextBlock() { Text = $"{"设置版本Ver",lbllength}: {ss.sVer.ToHexString()}" });
            sp_state.Children.Add(new TextBlock() { Text = $"{"设置版本日期",lbllength}: {ss.sDate.ToHexString_10()}" });
            sd.TypeName = ss.typename.ConvertbyteToStr();
            sp_state.Children.Add(new TextBlock() { Text = $"{"装置类型",lbllength}: {  sd.TypeName}" });
            //备用模式
            comboBox_backmode.SelectedIndex = (int)ss.backmode;
            //名称
            textBox_Name.Text = sd.Name;
            //密码
            textBox_password.Text = ss.DevicePass.ConvertbyteToStr();
            //APEX有效设置标识2
            sp_state.Children.Add(new TextBlock() { Text = $"{"有效设置标识",lbllength}: {  ss.ApexSetFlag.ConvertbyteToStr()}" });
            //
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            byte[] ssbytes = StructWithBytes.StructToBytes(MyDevice.Struct_SS);
            List<byte> msg = new List<byte>();
            msg.AddRange(ssbytes.CloneRange(0, 20));
            //命令码
            msg.AddRange(CMDCode.保存所有配置.CMD_Send);
            //正文长度
            //int a =  "D8 04"
            msg.AddRange(BytesHelper.StrToHexBytes("D8 04"));
            msg.AddRange(CMDCode.SuperPWD);
            msg.AddRange(ssbytes.CloneRange(24 + 2, 1232));
            //从 报文命令码  开始的包括所有正文内容的字方式XOR和   XOR L XOR H
            msg.AddRange(CMDFactory.Xor(msg, 4, msg.Count - 4));
            //结束标志
            msg.AddRange(new byte[] { 0x03, 0x00 });
            //1240-6
            Console.WriteLine(BytesHelper.ToHexString(msg.ToArray()));

            byte[] tok = BytesHelper.StrToHexBytes(hex2);
            byte[] ttt = msg.ToArray();

            for (int i = 0; i < tok.Length; i++)
            {
                if (tok[i] != ttt[i])
                {
                    Console.Write(tok[i].ToString("X2") + " ");
                }
            }
            Console.WriteLine();
            for (int i = 0; i < tok.Length; i++)
            {
                if (tok[i] != ttt[i])
                {
                    Console.Write(ttt[i].ToString("X2") + " ");
                }
            }
            SendAndWait(msg.ToArray(), CMDCode.搜索设备.CMD_Receive);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Udpclient.Close();
            }
            catch
            {
            }
        }

        private Stopwatch usedTime = new Stopwatch();

        /// <summary>
        /// 发送并等待命令
        /// </summary>
        /// <param name="msg">正文</param>
        /// <param name="cmd">等待出现的命令码</param>
        private void SendAndWait(byte[] msg, byte[] thiscmd)
        {
            LastCMD = new byte[] { 0x00, 0x00 };
            mybusy.IsBusy = true;
            Msgstr = "命令执行完成";
            Udpclient.Send(msg, msg.Length, MyDevice.Client_Endpoint);
            Task<bool> t = new Task<bool>(new Func<bool>(() =>
            {
                usedTime.Restart();
                int timeout = 0;
                //3秒超时
                while (timeout < 50)
                {
                    if (BytesHelper.Equalbybyte(LastCMD, thiscmd))
                    {
                        return true;
                    }
                    else
                    {
                        Thread.Sleep(100);
                        timeout++;
                    }
                }
                Msgstr = "命令执行失败";
                return false;
            }));
            t.Start();
            t.ContinueWith((tt) =>
            {
                long a = 500 - usedTime.ElapsedMilliseconds;
                if (a > 0)
                {
                    Thread.Sleep((int)a);
                }

                Dispatcher.Invoke(new Action(() =>
                {
                    mybusy.IsBusy = false;
                    if (tt.Result == false)
                    {
                        MessageBox.Show(this, Msgstr);
                    }
                }));
            });
        }
    }
}