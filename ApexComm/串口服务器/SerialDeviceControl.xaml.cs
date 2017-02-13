using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic;
using MyHelper;

namespace ApexComm.串口服务器
{
    /// <summary>
    /// SerialDeviceControl.xaml 的交互逻辑
    /// </summary>
    public partial class SerialDeviceControl : UserControl
    {
        /// <summary>
        /// 上面传下的设备对象
        /// </summary>
        public SerialDevice MyDevice;

        /// <summary>
        /// 主窗体对象
        /// </summary>
        public MainWindow mainWindow;

        /// <summary>
        /// 主窗口显示配置界面的函数
        /// </summary>
        public Action<SerialDevice> ShowCfg;

        public MyUDPClient udpMessage;

        /// <summary>
        /// 等待出现的命令
        /// </summary>
        private byte[] WaitingCMD;

        public SerialDeviceControl()
        {
            InitializeComponent();
        }

        public void SetDevice(SerialDevice d)
        {
            MyDevice = d;
            if (udpMessage != null)
            {
                udpMessage.Close();
                udpMessage = null;
            }

            udpMessage = new MyUDPClient(MyDevice.PC_Endpoint, MyDevice.Client_Endpoint);

            udpMessage.Event_ReceiveMsg = Evnet_ReceiveMsg;
            udpMessage.Event_SendAndWaitProcSucess = Evnet_SendAndWaitProcSucess;
            udpMessage.Event_SendAndWaitProcTimeout = Evnet_SendAndWaitProcTimeout;
            sp_msglist.Children.Clear();
            //便签占用的长度
            const int lbllength = -5;
            sp_msglist.Children.Add(new TextBlock() { Text = $"{"SN",lbllength}: {MyDevice.SN}" });
            sp_msglist.Children.Add(new TextBlock() { Text = $"{"名字:",lbllength}: {MyDevice.Name}" });
            sp_msglist.Children.Add(new TextBlock() { Text = $"{"装置类型",lbllength}: {MyDevice.TypeName}" });
            sp_msglist.Children.Add(new TextBlock() { Text = $"{"程序版本",lbllength}: {MyDevice.PVer}" });
            sp_msglist.Children.Add(new TextBlock() { Text = $"{"设置版本",lbllength}: {MyDevice.SVer}" });
            sp_msglist.Children.Add(new TextBlock() { Text = $"{"网口A地址",lbllength}: {MyDevice.Net1_IP}" });
            sp_msglist.Children.Add(new TextBlock() { Text = $"{"网口A子网掩码",lbllength}: {MyDevice.Net1_Mask}" });
            sp_msglist.Children.Add(new TextBlock() { Text = $"{"网口B地址",lbllength}: {MyDevice.Net2_IP}" });
            sp_msglist.Children.Add(new TextBlock() { Text = $"{"网口B子网掩码",lbllength}: {MyDevice.Net2_Mask}" });
        }

        public void Evnet_ReceiveMsg(byte[] msg)
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

                    if (BytesHelper.Equalbybyte(WaitingCMD, cmdcode))
                    {
                        udpMessage.IsFinish = true;
                    }
                }
                catch (Exception ex)
                {
                    LogMsg.log_error("解析失败:" + ex.ToString());
                    MessageBox.Show("解析失败:" + ex.ToString());
                }
            }
        }

        private void Evnet_SendAndWaitProcSucess()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                mainWindow.busywait.IsBusy = false;
            }));
        }

        private void Evnet_SendAndWaitProcTimeout()
        {
            MessageBox.Show("命令执行失败!!!");
            Dispatcher.Invoke(new Action(() =>
            {
                mainWindow.busywait.IsBusy = false;
            }));
        }

        private void button_set_Click(object sender, RoutedEventArgs e)
        {
            ShowCfg(MyDevice);
        }

        private void button_start_Click(object sender, RoutedEventArgs e)
        {
            //  22. 激活指定装置（下行）, 对于双网口, 则本节点IP指本网口的IP地址
            //      20       82H        报文命令码
            //      21       82H
            //      22       02H        报文正文长度
            //      23       00
            //      24       TimeL      闪烁时间（秒）
            //      25       TimeH

            string xx = Interaction.InputBox("请输入PowerPort的ALM指示LED闪烁的时间(0～32760)，单位：秒", "时间设定", "30");
            int num = 0;
            if (int.TryParse(xx, out num))
            {
                byte[] msg = CMDFactory.MakeCMDBytes(CMDCode.激活装置.CMD_Send, BytesHelper.intToBytes(num, 2), MyDevice.SN.ConvertStrTobytes());
                mainWindow.busywait.IsBusy = true;
                WaitingCMD = CMDCode.激活装置.CMD_Receive;
                udpMessage.SendAndWait(msg, true);
            }
        }

        private void button_reset_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定复位装置吗?", "警告", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                byte[] msg = CMDFactory.MakeCMDBytes(CMDCode.复位CPU.CMD_Send, CMDCode.SuperPWD, MyDevice.SN.ConvertStrTobytes());
                mainWindow.busywait.IsBusy = true;
                WaitingCMD = CMDCode.搜索设备.CMD_Receive;
                udpMessage.SendAndWait(msg, true);
            }
        }

        private void button_monitor_Click(object sender, RoutedEventArgs e)
        {
            Monitor mm = new Monitor();
            mm.MyDevice = MyDevice;
            mainWindow.Hide();
            mm.ShowDialog();
            mainWindow.Show();
        }
    }
}