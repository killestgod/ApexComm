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
using System.Resources;

namespace ApexComm.串口服务器
{
    /// <summary>
    /// SerialDeviceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SerialDeviceWindow : Window
    {
        /// <summary>
        /// 上面传下的设备对象
        /// </summary>
        public SerialDevice MyDevice;

        /// <summary>
        /// 写入后 收到的回复信息
        /// </summary>
        private Dictionary<byte, string> WriteResult = new Dictionary<byte, string>() { };

        public SerialDeviceWindow()
        {
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
            bool isdebug = false;
            if (MyDevice == null)
            {
                isdebug = true;
                MyDevice = new SerialDevice();
                MyDevice.PC_Endpoint = new System.Net.IPEndPoint(IPAddress.Parse("192.168.1.71"), 61199);
                MyDevice.Client_Endpoint = new System.Net.IPEndPoint(IPAddress.Parse("255.255.255.255"), 61199);
                MyDevice.SN = "P2084ECT16122003";
                string hex = "EB 90 EB 90 32 50 38 30 45 34 54 43 36 31 32 31 30 32 33 30 AA AA D2 04 00 00 50 41 58 45 2D 04 08 02 0E 11 37 1E 2D 04 06 03 12 0F 0A 1E 32 4E 30 53 2D 38 38 34 43 35 45 2D 00 54 00 00 00 00 00 00 32 50 38 30 45 34 54 43 36 31 32 31 30 32 33 30 00 00 00 00 01 00 01 00 0A 00 00 8C 07 00 00 08 05 00 00 46 03 00 00 84 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 00 00 80 25 07 08 05 06 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 01 00 03 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 02 00 00 00 02 00 01 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 02 00 00 00 00 00 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 0A 00 FF FF FF FF FF FF FF FF A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0C 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 A8 C0 0D 00 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 ED 13 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 A8 C0 02 00 A8 C0 03 00 FF FF 00 FF FF FF 00 FF A8 C0 01 00 A8 C0 01 00 ED 13 ED 13 00 00 00 00 C0 00 A8 C0 02 00 C0 00 A8 C0 03 00 FF FF 00 00 01 00 00 00 FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 AC 26 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 73 55 72 65 31 31 31 31 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 32 31 34 33 36 35 38 37 50 41 58 45 00 00 79 F4 64 BB 03 03";
                hex2 = Properties.Settings.Default.HEX;
                byte[] msg = BytesHelper.StrToHexBytes(hex, " ");
                WaitingCMD = new byte[] { 0x11, 0x11 };
                ReceiveMsg(msg);
            }
            else
            {
            }
            watermarkPasswordBox_pwd.Password = Properties.Settings.Default.PWD;

            //初始化udp通讯
            Udpclient = new UdpClient(MyDevice.PC_Endpoint);
            Udpclient.BeginReceive(ReceiveCallback, Udpclient);
            if (!isdebug)
            {
                SendAndWait(SerialDeviceProc.ReadAllCfg(MyDevice), CMDCode.获取所有配置.CMD_Receive);
            }
            //
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
                    if (BytesHelper.Equalbybyte(WaitingCMD, new byte[] { 0x00, 0x00 }))
                    {
                        return;//无用的命令 可能是其他机器请求的
                    }
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
                        LogMsg.log("读取配置完成");
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
                    }
                    else if (BytesHelper.Equalbybyte(cmdcode, ApexComm.串口服务器.CMDCode.保存所有配置.CMD_Receive))
                    {
                        byte[] xx = CMDFactory.GetBody(msg);
                        if (xx[0] != 0x06)
                        {
                            string resultstr = WriteResult[xx[0]];
                            MessageBox.Show(resultstr);
                        }
                    }
                    else if (BytesHelper.Equalbybyte(cmdcode, ApexComm.串口服务器.CMDCode.复位CPU.CMD_Receive))
                    {
                        LogMsg.log("复位CPU完成");
                    }
                    else if (BytesHelper.Equalbybyte(cmdcode, ApexComm.串口服务器.CMDCode.搜索设备.CMD_Receive))
                    {
                        LogMsg.log("搜索设备完成");

                        //重新获取配置信息
                        SendAndWait(SerialDeviceProc.ReadAllCfg(MyDevice), CMDCode.获取所有配置.CMD_Receive);
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
            sp_state.Children.Add(new TextBlock() { Text = $"{"程序版本pVer",lbllength}: {ss.pVer.Swap01To10().ToHexString(".")}" });
            sp_state.Children.Add(new TextBlock() { Text = $"{"编译版本日期",lbllength}: { CMDCode.DateTimeFromBytes(ss.pDate) }" });
            sp_state.Children.Add(new TextBlock() { Text = $"{"设置版本Ver",lbllength}: {ss.sVer.Swap01To10().ToHexString(".")}" });
            sp_state.Children.Add(new TextBlock() { Text = $"{"设置版本日期",lbllength}: { CMDCode.DateTimeFromBytes(ss.sDate)}" });
            sd.TypeName = ss.typename.ConvertbyteToStr();
            sp_state.Children.Add(new TextBlock() { Text = $"{"装置类型",lbllength}: {  sd.TypeName}" });
            //备用模式
            comboBox_backmode.SelectedIndex = (int)ss.backmode;
            //名称
            textBox_Name.Text = sd.Name;
            //密码
            //textBox_password.Text = ss.DevicePass.ConvertbyteToStr();
            //APEX有效设置标识2
            sp_state.Children.Add(new TextBlock() { Text = $"{"有效设置标识",lbllength}: {  ss.ApexSetFlag.ConvertbyteToStr()}" });
            //

            mySerialCfg.SetDevice(MyDevice);
            netcfgA.NetNum = 0;
            netcfgA.SDWindow = this;
            netcfgB.NetNum = 1;
            netcfgB.SDWindow = this;
            netcfgA.SetDevice(MyDevice);
            netcfgB.SetDevice(MyDevice);
            int netnum = int.Parse(MyDevice.TypeName.Substring(1, 1));

            netcfgB.Visibility = netnum == 1 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string pwd = watermarkPasswordBox_pwd.Password;
            if (string.IsNullOrEmpty(pwd))
            {
                pwd = "12345678";
            }

            if (!pwd.Equals(MyDevice.Struct_SS.DevicePass.ConvertbyteToStr()))
            {
                MessageBox.Show("密码输入错误!!!");
                watermarkPasswordBox_pwd.Focus();
                return;
            }

            //结构体正文部分的xor校验
            byte[] ssbytes = StructWithBytes.StructToBytes(MyDevice.Struct_SS);
            byte[] xorbytes = CMDFactory.Xor(ssbytes, 26, 1230);
            ssbytes.Replace(xorbytes, ssbytes.Length - 6);
            List<byte> msg = new List<byte>();
            msg.AddRange(ssbytes.CloneRange(0, 20));
            //命令码
            msg.AddRange(CMDCode.保存所有配置.CMD_Send);
            //正文长度
            //int a =  "D8 04"
            msg.AddRange(BytesHelper.StrToHexBytes("D8 04"));
            msg.AddRange(CMDCode.SuperPWD);//超级密码
            msg.AddRange(ssbytes.CloneRange(24 + 2, 1232));
            //从 报文命令码  开始的包括所有正文内容的字方式XOR和   XOR L XOR H
            msg.AddRange(CMDFactory.Xor(msg, 4, msg.Count - 4));
            //结束标志
            msg.AddRange(new byte[] { 0x03, 0x00 });
            //1240-6
            Console.WriteLine(BytesHelper.ToHexString(msg.ToArray()));

            //byte[] tok = BytesHelper.StrToHexBytes(hex2);
            //byte[] ttt = msg.ToArray();

            //for (int i = 0; i < tok.Length; i++)
            //{
            //    if (tok[i] != ttt[i])
            //    {
            //        Console.WriteLine(i + ":  对-" + tok[i].ToString("X2") + " 新-" + ttt[i].ToString("X2"));
            //    }
            //}
            //Console.WriteLine();

            SendAndWait(msg.ToArray(), CMDCode.获取所有配置.CMD_Receive);
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
        /// 等待要出现的命令
        /// </summary>
        private byte[] WaitingCMD = new byte[] { 0x00, 0x00 };

        /// <summary>
        /// 发送并等待命令
        /// </summary>
        /// <param name="msg">正文</param>
        /// <param name="cmd">等待出现的命令码</param>
        private void SendAndWait(byte[] msg, byte[] thiscmd)
        {
            WaitingCMD = thiscmd;
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
                    if (BytesHelper.Equalbybyte(LastCMD, WaitingCMD))
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
                LastCMD = new byte[] { 0x00, 0x00 };
                WaitingCMD = new byte[] { 0x00, 0x00 };
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

        private void comboBox_backmode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyDevice.Struct_SS.backmode = (ushort)comboBox_backmode.SelectedIndex;
            MyDevice.Struct_SS.backmode_q = (ushort)comboBox_backmode.SelectedIndex;
        }

        private void textBox_Name_LostFocus(object sender, RoutedEventArgs e)
        {
            MyDevice.Struct_SS.DeviceName = CMDFactory.ConvertStrTobytes(textBox_Name.Text.Trim(), 40);
        }

        /// <summary>
        /// 重设密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watermarkPasswordBox_newpwdre_LostFocus(object sender, RoutedEventArgs e)
        {
            string pwd1 = watermarkPasswordBox_newpwd.Password;
            string pwd2 = watermarkPasswordBox_newpwdre.Password;

            if (!string.IsNullOrEmpty(pwd1) && pwd1.Equals(pwd2))
            {
                MyDevice.Struct_SS.DevicePass = CMDFactory.ConvertStrTobytes(pwd1, 8);
            }
            else
            {
                MessageBox.Show("重复输入的密码错误");
                watermarkPasswordBox_newpwd.Password = "";
                watermarkPasswordBox_newpwdre.Password = "";
            }
        }

        private void watermarkPasswordBox_pwd_LostFocus(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.PWD = watermarkPasswordBox_pwd.Password;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// 同步隐藏或显示tcp的一些选项
        /// </summary>
        public void UI_tcpudp()
        {
            {
                mySerialCfg.net0cfg.IsEnabled = true;
                if (MyDevice.Struct_SS.Net0IsUdpProtocol.bytesToInt(0, 2) == 0 ? true : false)
                {
                    int selectmode = MyDevice.Struct_SS.Net0TcpMode.bytesToInt(0, 2);
                    // tcp
                    if (selectmode == 0)
                    {
                        mySerialCfg.net0cfg.IsEnabled = false;
                    }
                }
                else
                {
                    //udp
                    mySerialCfg.net0cfg.IsEnabled = false;
                }
            }

            {
                mySerialCfg.net1cfg.IsEnabled = true;
                if (MyDevice.Struct_SS.Net1IsUdpProtocol.bytesToInt(0, 2) == 0 ? true : false)
                {
                    int selectmode = MyDevice.Struct_SS.Net1TcpMode.bytesToInt(0, 2);
                    // tcp
                    if (selectmode == 0)
                    {
                        mySerialCfg.net1cfg.IsEnabled = false;
                    }
                }
                else
                {
                    //udp
                    mySerialCfg.net1cfg.IsEnabled = false;
                }
            }
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            //系统路径-涉及不到的可以去掉
            string strPath = PathHelper.GetCurrentDirectory();
            Microsoft.Win32.SaveFileDialog dialogOpenFile = new Microsoft.Win32.SaveFileDialog();
            dialogOpenFile.DefaultExt = ".cfg";//默认扩展名
            dialogOpenFile.AddExtension = true;//是否自动添加扩展名
            dialogOpenFile.Filter = "配置文件|*.cfg";
            dialogOpenFile.OverwritePrompt = true;//文件已存在是否提示覆盖
            dialogOpenFile.FileName = "";//默认文件名
            dialogOpenFile.CheckPathExists = true;//提示输入的文件名无效
            dialogOpenFile.Title = "保存...";
            dialogOpenFile.InitialDirectory = strPath;

            //显示对话框
            bool? b = dialogOpenFile.ShowDialog();
            //Nullable<bool> b = ... 方式二
            if (b == true)//点击保存
            {
                byte[] ssbytes = StructWithBytes.StructToBytes(MyDevice.Struct_SS);

                //结构体正文部分的xor校验
                byte[] xorbytes = CMDFactory.Xor(ssbytes, 26, 1230);
                ssbytes.Replace(xorbytes, ssbytes.Length - 6);

                FileHelper.WriteBytesToFile(dialogOpenFile.FileName, ssbytes);
            }
        }

        private void button_init_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("是否恢复出厂设置?", "警告", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                byte[] bb = CMDFactory.MakeCMDBytes(CMDCode.恢复出厂.CMD_Send, CMDCode.SuperPWD
                     , MyDevice.SN.ConvertStrTobytes());
                SendAndWait(bb, CMDCode.恢复出厂.CMD_Receive);
            }
        }

        private void button_read_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
            //设置标题
            openFileDialog1.Title = "打开文件对话框";
            //该值指示对话框在关闭前是否还原当前目录,Environment.CurrentDirectory 的值不会被改变
            openFileDialog1.RestoreDirectory = true;

            //设置目录
            //string dir = @"c:\";
            //初始打开的对话框
            openFileDialog1.InitialDirectory = PathHelper.GetCurrentDirectory();
            //初始化打开我的文档
            //openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //打开的如果是快捷方式 打开的是指向的文件
            openFileDialog1.DereferenceLinks = false;
            //设置过滤器
            openFileDialog1.Filter = "配置文件|*.cfg";
            //设置默认选择那个过滤器
            openFileDialog1.FilterIndex = 0;
            //指定打开对话框弹出时选定的
            openFileDialog1.FileName = "";
            //显示"以只读方式打开"选项
            openFileDialog1.ShowReadOnly = true;
            //检查用户指示的路径是否存在
            openFileDialog1.CheckPathExists = true;
            //检查用户指示的文件是否存在
            openFileDialog1.CheckFileExists = true;

            if (openFileDialog1.ShowDialog() == true)
            {
                ReceiveMsg(FileHelper.ReadBytesFromFile(openFileDialog1.FileName));
            }
        }

        private void button_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}