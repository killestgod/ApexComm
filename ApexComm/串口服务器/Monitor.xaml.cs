using MyHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ApexComm.串口服务器
{
    /// <summary>
    /// Monitor.xaml 的交互逻辑
    /// </summary>
    public partial class Monitor : Window
    {
        /// <summary>
        /// 上面传下的设备对象
        /// </summary>
        public SerialDevice MyDevice;

        public MyUDPClient udpMessage;        /// <summary>

        /// 等待出现的命令
        /// </summary>
        private byte[] WaitingCMD;

        public Monitor()
        {
            InitializeComponent();
            int[] nums = new int[30];
            for (int i = 0; i < 30; i++)
            {
                nums[i] = i + 1;
            }
            comboBox_time.ItemsSource = nums;

            Dictionary<string, byte> zero = new Dictionary<string, byte>();

            zero.Add("所有计数器", 0x88);

            zero.Add("所有接收计数器", 0x08);
            zero.Add("所有发送计数器", 0x80);
            zero.Add("串口接收计数器", 0x01);
            zero.Add("串口发送计数器", 0x10);

            zero.Add("网络A接收计数器", 0x02);
            zero.Add("网络A发送计数器", 0x20);

            zero.Add("网络B接收计数器", 0x04);
            zero.Add("网络B发送计数器", 0x40);

            comboBox_counter.ItemsSource = zero;

            comboBox_counter.SelectedValuePath = "Value";
            comboBox_counter.DisplayMemberPath = "Key";
            comboBox_counter.SelectedIndex = 0;
            WindowHelper.ReSetWindowsSize(this);
        }

        private int NetNum;
        private int ComNum;
        public PortControl[] controls_com;
        private DispatcherTimer tmr;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // viewbox.Stretch = Stretch.UniformToFill;
            if (MyDevice == null)
            {
                MyDevice = new SerialDevice();
                MyDevice.PC_Endpoint = new System.Net.IPEndPoint(IPAddress.Parse("192.168.1.71"), 61199);
                MyDevice.Client_Endpoint = new System.Net.IPEndPoint(IPAddress.Parse("255.255.255.255"), 61199);
                MyDevice.SN = "P2084ECT16122003";
                MyDevice.TypeName = "N2S32-485C-ET";
            }
            NetNum = int.Parse(MyDevice.TypeName.Substring(1, 1));
            ComNum = int.Parse(MyDevice.TypeName.Substring(3, 2));
            controls_com = new PortControl[ComNum];
            for (int i = 0; i < ComNum; i++)
            {
                controls_com[i] = new PortControl();
                controls_com[i].Margin = new Thickness(5);
                controls_com[i].Width = 140;

                controls_com[i].IsNet2OK = NetNum == 2 ? true : false;
                controlList.Children.Add(controls_com[i]);
            }

            udpMessage = new MyUDPClient(MyDevice.PC_Endpoint, MyDevice.Client_Endpoint);
            udpMessage.Event_ReceiveMsg = ReceiveMsg;
            TimerOnTick(sender, e);
            tmr = new DispatcherTimer();
            comboBox_time.SelectedIndex = Properties.Settings.Default.Getcycle;
            tmr.Interval = TimeSpan.FromSeconds(comboBox_time.SelectedIndex + 1);
            tmr.Tick += TimerOnTick;
            tmr.Start();
        }

        private void ReceiveMsg(byte[] msg)
        {
            //检测包
            try
            {
                if (CMDFactory.CheckReceiveMsg(msg))
                {
                    try
                    {
                        //获取回复的命令码
                        byte[] cmdcode = msg.CloneRange(20, 2);
                        //工作状态回传
                        if (BytesHelper.Equalbybyte(CMDCode.工作状态.CMD_Receive, cmdcode))
                        {
                            Console.WriteLine("收到回复");
                            ShowUI(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMsg.log_error("解析失败:" + ex.ToString());
                        MessageBox.Show("解析失败:" + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("处理出错:" + ex.ToString());
            }
        }

        private void ShowUI(byte[] msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (isclosing)
                {
                    return;
                }
                SerialDevice_Monitor_Struct ss = StructWithBytes.ByteToStruct<SerialDevice_Monitor_Struct>(msg);
                StringBuilder sb = new StringBuilder();
                if ((ss.StatusFlag[0] & 0x1) != 0)
                {
                    sb.AppendLine("网络1外部物理连接断开");
                }
                if ((ss.StatusFlag[0] & 0x2) != 0)
                {
                    sb.AppendLine("网络2外部物理连接断开");
                }
                if ((ss.StatusFlag[0] & 0x4) != 0)
                {
                    sb.AppendLine("网络1芯片异常（重发缓冲溢出）");
                }
                if ((ss.StatusFlag[0] & 0x8) != 0)
                {
                    sb.AppendLine("网络2芯片异常（重发缓冲溢出）");
                }

                if ((ss.StatusFlag[1] & 0x2) != 0)
                {
                    sb.AppendLine("选择了人工复位（仅限于调试）");
                }
                if ((ss.StatusFlag[1] & 0x4) != 0)
                {
                    sb.AppendLine("CPLD/FPGA逻辑错误");
                }
                if ((ss.StatusFlag[1] & 0x8) != 0)
                {
                    sb.AppendLine("DSP内部RAM读/写错误");
                }
                if ((ss.StatusFlag[1] & 0x10) != 0)
                {
                    sb.AppendLine("NVRAM异常");
                }
                if ((ss.StatusFlag[1] & 0x20) != 0)
                {
                    sb.AppendLine("NVRAM读异常");
                }
                if ((ss.StatusFlag[1] & 0x40) != 0)
                {
                    sb.AppendLine("NVRAM读异常");
                }
                if ((ss.StatusFlag[1] & 0x80) != 0)
                {
                    sb.AppendLine("NVRAM中的设置异常（CRC、装置类型或设置版本错误）");
                }
                status.Content = sb.ToString().Trim();
                for (int i = 0; i < ComNum; i++)
                {
                    //// //A网口 COM的TCP链接状态
                    //controls_net0[i].TCPStatus = GetTCPStatus(BytesHelper.bytesToInt(ss.Net0TCPStatus, i * 2, 2));
                    //controls_net1[i].TCPStatus = GetTCPStatus(BytesHelper.bytesToInt(ss.Net1TCPStatus, i * 2, 2));
                    ////A B网口 COM的TCP链接状态
                    controls_com[i].PortName = "串口" + (i + 1);
                    controls_com[i].Net0TCPStatus = GetTCPStatus(BytesHelper.bytesToInt(ss.Net0TCPStatus, i * 2, 2));
                    controls_com[i].Net1TCPStatus = GetTCPStatus(BytesHelper.bytesToInt(ss.Net1TCPStatus, i * 2, 2));
                    controls_com[i].COMTotalSend = (uint)BytesHelper.bytesToInt(ss.COMTotalSend, i * 2);
                    controls_com[i].COMTotalReceive = (uint)BytesHelper.bytesToInt(ss.COMTotalReceive, i * 2);
                    controls_com[i].Net0TotalSend = (uint)BytesHelper.bytesToInt(ss.Net0TotalSend, i * 2);
                    controls_com[i].Net1TotalSend = (uint)BytesHelper.bytesToInt(ss.Net1TotalSend, i * 2);
                    controls_com[i].Net0TotalReceive = (uint)BytesHelper.bytesToInt(ss.Net0TotalReceive, i * 2);
                    controls_com[i].Net1TotalReceive = (uint)BytesHelper.bytesToInt(ss.Net1TotalReceive, i * 2);
                    controls_com[i].ReShow();
                }
                label_time.Content = "最后数据时间:" + DateTime.Now.ToLongTimeString();
            }));
        }

        /// <summary>
        /// com的tcp状态
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private string GetTCPStatus(int n)
        {
            string str = "";
            switch (n)
            {
                case 0:
                    str = "监听";
                    break;

                case 1:
                    str = "接收到同步信号";
                    break;

                case 2:
                    str = "发送同步信号";
                    break;

                case 3:
                    str = "链接已经建立";
                    break;

                case 4:
                    str = "从容关闭等待状态1";
                    break;

                case 5:
                    str = "从容关闭等待状态2";
                    break;

                case 6:
                    str = "正在关闭";
                    break;

                case 7:
                    str = "等待关闭";
                    break;

                case 8:
                    str = "关闭前的最后一个确认报文";
                    break;

                case 9:
                    str = "已经关闭";
                    break;

                case 10:
                    str = "执行关闭后的等待对方确认关闭报文";
                    break;

                case 255:
                    str = "自CPU复位以来还未链接过";
                    break;
            }
            return str;
        }

        private void TimerOnTick(object sender, EventArgs args)
        {
            /**
            '   7. 查看装置工作状态（下行）
            '      20   56H    报文命令码L
            '      21   56H    报文命令码H
            '      22    2     报文正文长度L
            '      23    0     报文正文长度H
            '      24   CtrlL  控制码L
            '      25   CtrlH  控制码H
            '
            '          控制码:  0x0000-正常查询              // 0000 0000 0000 0000
            '                  0x0101-清零串口接收计数器      // 0000 0001 0000 0001
            '                  0x0202-清零网络A接收计数器     // 0000 0010 0000 0010
            '                  0x0404-清零网络B接收计数器     // 0000 0100 0000 0100
            '                  0x0808-清零所有接收计数器      // 0000 1000 0000 1000
            '                  0x1010-清零串口发送计数器      // 0001 0000 0001 0000
            '                  0x2020-清零网络A发送计数器     // 0010 0000 0010 0000
            '                  0x4040-清零网络B发送计数器     // 0100 0000 0100 0000
            '                  0x8080-清零所有发送计数器      // 1000 0000 1000 0000
            '                  0x8888-清零所有接收和发送计数器 // 1000 1000 1000 1000
            '
            '           说明: 当控制码不为零时, 本次报文无响应。
            **/
            byte[] cmd = CMDFactory.MakeCMDBytes(CMDCode.工作状态.CMD_Send, new byte[] { 0x00, 0x00 }, MyDevice.SN.ConvertStrTobytes());
            udpMessage.Send(cmd);
            Console.WriteLine("请求数据");
        }

        private void comboBox_time_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tmr.Interval = TimeSpan.FromSeconds(comboBox_time.SelectedIndex + 1);
            Properties.Settings.Default.Getcycle = comboBox_time.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void button_send_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确认清零" + comboBox_counter.Text, "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                byte[] cmd = CMDFactory.MakeCMDBytes(CMDCode.工作状态.CMD_Send, new byte[] { (byte)comboBox_counter.SelectedValue, (byte)comboBox_counter.SelectedValue }, MyDevice.SN.ConvertStrTobytes());
                udpMessage.Send(cmd);
            }
        }

        //窗口是否正在关闭
        private bool isclosing = false;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isclosing = true;
            tmr.Stop();
            udpMessage.Close();
        }
    }
}