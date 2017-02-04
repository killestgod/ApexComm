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
using MyHelper;

namespace ApexComm.串口服务器
{
    /// <summary>
    /// NetCfg.xaml 的交互逻辑
    /// </summary>
    public partial class NetCfg : UserControl
    {
        /// <summary>
        /// 网口几 0--A 1--B
        /// </summary>
        public int NetNum = 0;

        private SerialDevice Device;

        /// <summary>
        /// 使用的是网口几
        /// </summary>
        public int NetPort = 1;

        public NetCfg()
        {
            InitializeComponent();
            this.Height = 270;

            comboBoxLimitIPMode.ItemsSource = new string[] {
                "不启用","允许以下ip访问","禁止以下ip访问"
            };
            comboBox_IsUseUDP.ItemsSource = new string[] {
                "TCP","UDP"
            };
            comboBox_mode.ItemsSource = new string[] {
                "server","client","Client（串口接收触发链接）","Server/Client",
                "Server/Client（串口接收触发链接）"
            };
        }

        private bool xxx = false;

        private void button_Click(object sender, RoutedEventArgs e)
        {
            xxx = !xxx;
            if (xxx)
            {
                this.Height = 133;
            }
            else
            {
                this.Height = 270;
            }
        }

        /// <summary>
        /// 显示刷新
        /// </summary>
        /// <param name="sd"></param>
        public void SetDevice(SerialDevice sd)
        {
            Device = sd;
            byte[] tbytes;
            if (NetNum == 0)
            {
                comboBoxLimitIPMode.SelectedIndex = Device.Struct_SS.net_cfg_LimitIPModeA.bytesToInt(0, 2);
                for (int i = 0; i < 6; i++)
                {
                    tbytes = Device.Struct_SS.net_cfg_utLimitIPa.CloneRange(i * 4, 4);
                    TextBox tt = this.FindName("textBoxipFilter1_" + i) as TextBox;
                    tt.Text = tbytes.Swap01To10().ToHexString_10(".");

                    tbytes = Device.Struct_SS.net_cfg_utLimitMaska.CloneRange(i * 4, 4);
                    TextBox tt2 = this.FindName("textBoxmask" + i) as TextBox;
                    tt2.Text = tbytes.Swap01To10().ToHexString_10(".");

                    tbytes = Device.Struct_SS.netA_udp.CloneRange(i * 4, 4);
                    TextBox tt3 = this.FindName("textBox_UDPIP" + i) as TextBox;
                    tt3.Text = tbytes.Swap01To10().ToHexString_10(".");
                    tbytes = Device.Struct_SS.netA_udp_port.CloneRange(i * 2, 2);
                    TextBox tt4 = this.FindName("textBox_UDPPort" + i) as TextBox;
                    tt4.Text = tbytes.bytesToInt(0, 2).ToString();
                }

                texbox_ip.Text = Device.Struct_SS.net_cfg_netipa.Swap01To10().ToHexString_10(".");
                texbox_mask.Text = Device.Struct_SS.net_cfg_netmaska.Swap01To10().ToHexString_10(".");
                texbox_gateway.Text = Device.Struct_SS.net_cfg_netGatewaya.Swap01To10().ToHexString_10(".");
                texbox_port.Text = Device.Struct_SS.net_cfg_parta.bytesToInt(0, 2).ToString();
                textBox_mac.IsEnabled = Device.Struct_SS.IsMacAsignedA.bytesToInt(0, 2) == 0 ? false : true;
                textBox_mac.Text = Device.Struct_SS.MacAsignedA.Swap01To10().ToHexString(":");

                comboBox_IsUseUDP.SelectedIndex = Device.Struct_SS.IsUdpProtocolA.bytesToInt(0, 2) == 0 ? 0 : 1;
                comboBox_mode.SelectedIndex = Device.Struct_SS.TcpModeA.bytesToInt(0, 2);
                //udp 所有都发送到指定端口
                checkBox_portAll.IsChecked = Device.Struct_SS.UdpComPortAllSameA.bytesToInt(0, 2) == 1 ? true : false;
            }
            else
            {
                comboBoxLimitIPMode.SelectedIndex = Device.Struct_SS.net_cfg_LimitIPModeB.bytesToInt(0, 2);
                for (int i = 0; i < 6; i++)
                {
                    tbytes = Device.Struct_SS.net_cfg_utLimitIPb.CloneRange(i * 4, 4);
                    TextBox tt = this.FindName("textBoxipFilter1_" + i) as TextBox;
                    tt.Text = tbytes.Swap01To10().ToHexString_10(".");
                    tbytes = Device.Struct_SS.net_cfg_utLimitMaskb.CloneRange(i * 4, 4);
                    TextBox tt2 = this.FindName("textBoxmask" + i) as TextBox;
                    tt2.Text = tbytes.Swap01To10().ToHexString_10(".");

                    tbytes = Device.Struct_SS.netB_udp.CloneRange(i * 4, 4);
                    TextBox tt3 = this.FindName("textBox_UDPIP" + i) as TextBox;
                    tt3.Text = tbytes.Swap01To10().ToHexString_10(".");
                    tbytes = Device.Struct_SS.netB_udp_port.CloneRange(i * 2, 2);
                    TextBox tt4 = this.FindName("textBox_UDPPort" + i) as TextBox;
                    tt4.Text = tbytes.bytesToInt(0, 2).ToString();
                }
                texbox_ip.Text = Device.Struct_SS.net_cfg_netipb.Swap01To10().ToHexString_10(".");
                texbox_mask.Text = Device.Struct_SS.net_cfg_netmaskb.Swap01To10().ToHexString_10(".");
                texbox_gateway.Text = Device.Struct_SS.net_cfg_netGatewayb.Swap01To10().ToHexString_10(".");
                texbox_port.Text = Device.Struct_SS.net_cfg_partb.bytesToInt(0, 2).ToString();
                textBox_mac.IsEnabled = Device.Struct_SS.IsMacAsignedB.bytesToInt(0, 2) == 0 ? false : true;
                textBox_mac.Text = Device.Struct_SS.MacAsignedB.Swap01To10().ToHexString(":");
                comboBox_IsUseUDP.SelectedIndex = Device.Struct_SS.IsUdpProtocolB.bytesToInt(0, 2) == 0 ? 0 : 1;
                comboBox_mode.SelectedIndex = Device.Struct_SS.TcpModeB.bytesToInt(0, 2);
                //udp 所有都发送到指定端口
                checkBox_portAll.IsChecked = Device.Struct_SS.UdpComPortAllSameB.bytesToInt(0, 2) == 1 ? true : false;
            }
            //sd.Struct_SS.net_cfg_LimitIPMode.
        }

        private void comboBoxLimitIPMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectindex = comboBoxLimitIPMode.SelectedIndex;
            if (selectindex == 0)
            {
                groupBox_LimitIPMode.Visibility = Visibility.Collapsed;
            }
            else
            {
                groupBox_LimitIPMode.Visibility = Visibility.Visible;
            }
        }

        private void comboBox_IsUseUDP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isudp = comboBox_IsUseUDP.SelectedIndex == 1 ? true : false;

            groupBox_UDP.Visibility = isudp ? Visibility.Visible : Visibility.Collapsed;
            label_mode.Visibility = isudp ? Visibility.Hidden : Visibility.Visible;
            comboBox_mode.Visibility = isudp ? Visibility.Hidden : Visibility.Visible;
        }

        private void checkBox_portAll_Checked(object sender, RoutedEventArgs e)
        {
            UdpUI();
        }

        private void checkBox_portAll_Unchecked(object sender, RoutedEventArgs e)
        {
            UdpUI();
        }

        private void UdpUI()
        {
            bool isallport = checkBox_portAll.IsChecked.Value;
            for (int i = 0; i < 6; i++)
            {
                TextBox tt4 = this.FindName("textBox_UDPPort" + i) as TextBox;
                tt4.IsEnabled = !isallport;
            }
        }
    }
}