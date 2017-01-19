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

using MyHelper;

using System.Threading;

namespace ApexComm.串口服务器
{
    /// <summary>
    /// SerialDeviceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SerialDeviceWindow : Window
    {
        public SerialDevice MyDevice = new SerialDevice();

        public SerialDeviceWindow()
        {
            InitializeComponent();
            List<string> netmodes = new List<string>()
            {
                "冷备用-不可同时访问,A网口抢占式优先",
                "热备用-可同时接收数据,一个网口发送有效,A网口抢占式优先",
                "自由切换-不可同时访问,黄埔店里链接者抢占式优先",
                "等同备用(默认)-可同时访问,既能接收,又能发送",
            };
            comboBox_backmode.ItemsSource = netmodes;
        }

        private void btn_read_Click(object sender, RoutedEventArgs e)
        {
            mybusy.IsBusy = true;
            byte[] cmd = SerialDeviceProc.ReadAllCfg(MyDevice);
            Task<byte[]> task = new Task<byte[]>(() =>
            {
                return UDPMessage.SendAndWait(MyDevice, cmd);
            });
            task.Start();
            task.ContinueWith((_task) =>
            {
                Thread.Sleep(500);
                Dispatcher.Invoke(new Action(() =>
                {
                    mybusy.IsBusy = false;
                    ReceiveMsg(_task.Result);
                }));
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //textBox_Name.Text = MyDevice.Name;
            // string hex = "eb:90:eb:90:32:50:36:31:45:34:54:43:36:31:31:31:38:32:36:30:aa:aa:d2:04:00:00:50:41:58:45:2d:04:08:02:0e:11:37:1e:2d:04:06:03:12:0f:0a:1e:32:4e:31:53:2d:36:38:34:43:35:45:2d:00:54:00:00:00:00:00:00:32:50:36:31:45:34:54:43:36:31:31:31:38:32:36:30:00:00:00:00:01:00:01:00:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:00:00:80:25:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:08:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:02:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:02:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:ff:ff:ff:ff:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:0a:00:ff:ff:ff:ff:ff:ff:ff:ff:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0c:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:a8:c0:0d:00:ed:13:ee:13:ef:13:f0:13:f1:13:f2:13:f3:13:f4:13:f5:13:f6:13:f7:13:f8:13:f9:13:fa:13:fb:13:fc:13:fd:13:fe:13:ff:13:00:14:01:14:02:14:03:14:04:14:05:14:06:14:07:14:08:14:09:14:0a:14:0b:14:0c:14:ed:13:ee:13:ef:13:f0:13:f1:13:f2:13:f3:13:f4:13:f5:13:f6:13:f7:13:f8:13:f9:13:fa:13:fb:13:fc:13:fd:13:fe:13:ff:13:00:14:01:14:02:14:03:14:04:14:05:14:06:14:07:14:08:14:09:14:0a:14:0b:14:0c:14:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:a8:c0:02:00:a8:c0:03:00:ff:ff:00:ff:ff:ff:00:ff:a8:c0:01:00:a8:c0:01:00:ed:13:ed:13:00:00:00:00:c0:00:a8:c0:02:00:c0:00:a8:c0:03:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:73:55:72:65:32:31:61:4e:65:6d:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:32:31:34:33:36:35:38:37:50:41:58:45:00:00:1e:7a:64:ba:03:03";
            string hex = "EB 90 EB 90 32 50 38 30 45 34 54 43 36 31 32 31 30 32 33 30 AA AA D2 04 00 00 50 41 58 45 2D 04 08 02 0E 11 37 1E 2D 04 06 03 12 0F 0A 1E 32 4E 30 53 2D 38 38 34 43 35 45 2D 00 54 00 00 00 00 00 00 32 50 38 30 45 34 54 43 36 31 32 31 30 32 33 30 00 00 00 00 01 00 01 00 0A 00 00 8C 07 00 00 08 05 00 00 46 03 00 00 84 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 07 08 05 06 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 01 00 03 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 02 00 00 00 02 00 01 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 00 00 00 00 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 FF FF FF FF FF FF FF FF A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 A8 C0 02 00 A8 C0 03 00 FF FF 00 FF FF FF 00 FF A8 C0 01 00 A8 C0 01 00 ED 13 ED 13 00 00 00 00 C0 00 A8 C0 02 00 C0 00 A8 C0 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 73 55 72 65 61 4E 65 6D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 32 31 34 33 36 35 38 37 50 41 58 45 00 00 2F 0E 64 BB 03 03";
            byte[] msg = BytesHelper.StrToHexBytes(hex, " ");
            ReceiveMsg(msg);
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
                    string sn = CMDFactory.ConvertbyteToStr(msg, 4, 16);
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
                        //MyDevice.SN_bytes = msg.CloneRange(4, 16);
                        //MyDevice.PVer = "Ver " + msg[25].ToString("X2") + "." + msg[24].ToString("X2");
                        //MyDevice.SVer = "Ver " + msg[27].ToString("X2") + "." + msg[26].ToString("X2");
                        //MyDevice.Net1_IP = BytesHelper.ToHexString_10(msg.CloneRange(28, 4), ".");
                        //MyDevice.Net1_Mask = BytesHelper.ToHexString_10(msg.CloneRange(32, 4), ".");
                        //MyDevice.Net2_IP = BytesHelper.ToHexString_10(msg.CloneRange(36, 4), ".");
                        //MyDevice.Net2_Mask = BytesHelper.ToHexString_10(msg.CloneRange(40, 4), ".");
                        //MyDevice.Name = CMDFactory.StrConvertSwapByteArrayToString(msg, 44, 40);
                        //MyDevice.TypeName = CMDFactory.StrConvertSwapByteArrayToString(msg, 84, 20).Trim();
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
            //便签占用的长度
            const int lbllength = -10;
            sd.SN = ss.sn.ConvertbyteToStr();
            sp_state.Children.Add(new TextBlock() { Text = $"{"SN",lbllength}: {sd.SN}" });

            sp_state.Children.Add(new TextBlock() { Text = $"{"当前数据获取方式",lbllength}: {ss.cmdtype.ToHexString()}" });
            sp_state.Children.Add(new TextBlock() { Text = $"{"有效设置标识",lbllength}: {ss.setFlagA_B.ConvertbyteToStr()}" });
            sp_state.Children.Add(new TextBlock() { Text = $"{"程序版本pVer",lbllength}: {ss.pVer.ToHexString()}" });
            sp_state.Children.Add(new TextBlock() { Text = $"{"编译版本日期",lbllength}: {ss.pVer.ToHexString_10()}" });
            sp_state.Children.Add(new TextBlock() { Text = $"{"设置版本Ver",lbllength}: {ss.sVer.ToHexString()}" });
            sp_state.Children.Add(new TextBlock() { Text = $"{"设置版本日期",lbllength}: {ss.sDate.ToHexString_10()}" });
            sd.TypeName = ss.typename.ConvertbyteToStr();
            sp_state.Children.Add(new TextBlock() { Text = $"{"装置类型",lbllength}: {  sd.TypeName}" });
            //备用模式
            comboBox_backmode.SelectedIndex = (int)ss.backmode;
            //波特率
        }
    }
}