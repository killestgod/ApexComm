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

namespace ApexComm.串口服务器
{
    /// <summary>
    /// PortControl.xaml 的交互逻辑
    /// </summary>
    public partial class PortControl : UserControl
    {
        public bool IsNet2OK = false;

        public string PortName;
        public string Net0TCPStatus;
        public string Net1TCPStatus;
        public UInt32 COMTotalReceive;
        public UInt32 COMTotalSend;
        public UInt32 Net0TotalReceive;
        public UInt32 Net1TotalReceive;
        public UInt32 Net0TotalSend;
        public UInt32 Net1TotalSend;

        public PortControl()
        {
            InitializeComponent();
        }

        public void ReShow()
        {
            label_name.Content = PortName;
            label_COMTotalSend.Content = "总发送:" + COMTotalSend;
            label_COMTotalReceive.Content = "总接收:" + COMTotalReceive;

            label_Net0TCPStatus.Content = Net0TCPStatus;
            label_Net0TotalReceive.Content = "总接收:" + Net0TotalReceive;
            label_Net0TotalSend.Content = "总发送:" + Net0TotalSend;
            if (!IsNet2OK)
            {
                label_Net1TCPStatus.Visibility = Visibility.Collapsed;
                label_Net1TotalSend.Visibility = Visibility.Collapsed;
                label_Net1TotalReceive.Visibility = Visibility.Collapsed;
                label_Net1.Visibility = Visibility.Collapsed;
            }
            label_Net1TCPStatus.Content = Net1TCPStatus;
            label_Net1TotalReceive.Content = "总接收:" + Net1TotalReceive;
            label_Net1TotalSend.Content = "总发送:" + Net1TotalSend;
        }
    }
}