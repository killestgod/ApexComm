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

        public SerialDeviceWindow SDWindow;
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

            texbox_port.MaxLength = 5;
            texbox_port.SetInputOnlyNumber();

            for (int i = 0; i < 6; i++)
            {
                TextBox tb = FindName("textBoxipFilter1_" + i) as TextBox;
                tb.LostFocus += Tb_LostFocus;
                tb = FindName("textBoxmask" + i) as TextBox;
                tb.LostFocus += Tb_LostFocus;
                tb = FindName("textBox_UDPIP" + i) as TextBox;
                tb.LostFocus += Tb_LostFocus;
                tb = FindName("textBox_UDPPort" + i) as TextBox;
                tb.MaxLength = 5;
                tb.SetInputOnlyNumber();
                tb.LostFocus += Tb_LostFocus2;
            }
        }

        private void SetLimitIpMode(int num)
        {
            switch (num)
            {
                case 0:
                    comboBoxLimitIPMode.SelectedIndex = 0;
                    break;

                case 0xA5A5:
                    comboBoxLimitIPMode.SelectedIndex = 1;
                    break;

                case 0x5A5A:
                    comboBoxLimitIPMode.SelectedIndex = 2;
                    break;
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
                int num = Device.Struct_SS.Net0_cfg_LimitIPMode.bytesToInt(0, 2);
                SetLimitIpMode(num);
                //comboBoxLimitIPMode.SelectedIndex = Device.Struct_SS.Net0_cfg_LimitIPMode.bytesToInt(0, 2);
                for (int i = 0; i < 6; i++)
                {
                    tbytes = Device.Struct_SS.Net0_cfg_utLimitIP.CloneRange(i * 4, 4);
                    TextBox tt = this.FindName("textBoxipFilter1_" + i) as TextBox;
                    tt.Text = tbytes.Swap01To10().ToHexString_10(".");

                    tbytes = Device.Struct_SS.Net0_cfg_utLimitMask.CloneRange(i * 4, 4);
                    TextBox tt2 = this.FindName("textBoxmask" + i) as TextBox;
                    tt2.Text = tbytes.Swap01To10().ToHexString_10(".");

                    tbytes = Device.Struct_SS.Net0_udp.CloneRange(i * 4, 4);
                    TextBox tt3 = this.FindName("textBox_UDPIP" + i) as TextBox;
                    tt3.Text = tbytes.Swap01To10().ToHexString_10(".");
                    tbytes = Device.Struct_SS.Net0_udp_port.CloneRange(i * 2, 2);
                    TextBox tt4 = this.FindName("textBox_UDPPort" + i) as TextBox;
                    tt4.Text = tbytes.bytesToInt(0, 2).ToString();
                }
                //ip地址
                texbox_ip.Text = Device.Struct_SS.Net0_cfg_netip.Swap01To10().ToHexString_10(".");
                texbox_mask.Text = Device.Struct_SS.Net0_cfg_netmask.Swap01To10().ToHexString_10(".");
                texbox_gateway.Text = Device.Struct_SS.Net0_cfg_netGateway.Swap01To10().ToHexString_10(".");
                texbox_port.Text = Device.Struct_SS.Net0BasePort.bytesToInt(0, 2).ToString();
                checkBox_mac.IsChecked = Device.Struct_SS.Net0IsMacAsigned.bytesToInt(0, 2) == 0 ? false : true;
                textBox_mac.Text = Device.Struct_SS.Net0MacAsigned.Swap01To10().ToHexString(":");

                comboBox_IsUseUDP.SelectedIndex = Device.Struct_SS.Net0IsUdpProtocol.bytesToInt(0, 2) == 0 ? 0 : 1;
                comboBox_mode.SelectedIndex = Device.Struct_SS.Net0TcpMode.bytesToInt(0, 2);
                //udp 所有都发送到指定端口 特殊为真FFFF
                checkBox_portAll.IsChecked = Device.Struct_SS.Net0UdpComPortAllSame.bytesToInt(0, 2) == 0 ? false : true;
            }
            else
            {
                int num = Device.Struct_SS.Net1_cfg_LimitIPMode.bytesToInt(0, 2);
                SetLimitIpMode(num);
                //comboBoxLimitIPMode.SelectedIndex = Device.Struct_SS.Net1_cfg_LimitIPMode.bytesToInt(0, 2);
                for (int i = 0; i < 6; i++)
                {
                    tbytes = Device.Struct_SS.Net1_cfg_utLimitIP.CloneRange(i * 4, 4);
                    TextBox tt = this.FindName("textBoxipFilter1_" + i) as TextBox;
                    tt.Text = tbytes.Swap01To10().ToHexString_10(".");
                    tbytes = Device.Struct_SS.Net1_cfg_utLimitMask.CloneRange(i * 4, 4);
                    TextBox tt2 = this.FindName("textBoxmask" + i) as TextBox;
                    tt2.Text = tbytes.Swap01To10().ToHexString_10(".");

                    tbytes = Device.Struct_SS.Net1_udp.CloneRange(i * 4, 4);
                    TextBox tt3 = this.FindName("textBox_UDPIP" + i) as TextBox;
                    tt3.Text = tbytes.Swap01To10().ToHexString_10(".");
                    tbytes = Device.Struct_SS.Net1_udp_port.CloneRange(i * 2, 2);
                    TextBox tt4 = this.FindName("textBox_UDPPort" + i) as TextBox;
                    tt4.Text = tbytes.bytesToInt(0, 2).ToString();
                }
                texbox_ip.Text = Device.Struct_SS.Net1_cfg_netip.Swap01To10().ToHexString_10(".");
                texbox_mask.Text = Device.Struct_SS.Net1_cfg_netmask.Swap01To10().ToHexString_10(".");
                texbox_gateway.Text = Device.Struct_SS.Net1_cfg_netGateway.Swap01To10().ToHexString_10(".");
                texbox_port.Text = Device.Struct_SS.Net1BasePort.bytesToInt(0, 2).ToString();
                checkBox_mac.IsChecked = Device.Struct_SS.Net1IsMacAsigned.bytesToInt(0, 2) == 0 ? false : true;
                textBox_mac.Text = Device.Struct_SS.Net1MacAsigned.Swap01To10().ToHexString(":");
                comboBox_IsUseUDP.SelectedIndex = Device.Struct_SS.Net1IsUdpProtocol.bytesToInt(0, 2) == 0 ? 0 : 1;
                comboBox_mode.SelectedIndex = Device.Struct_SS.Net1TcpMode.bytesToInt(0, 2);
                //udp 所有都发送到指定端口
                checkBox_portAll.IsChecked = Device.Struct_SS.Net1UdpComPortAllSame.bytesToInt(0, 2) == 0 ? false : true;
            }
            //sd.Struct_SS.net_cfg_LimitIPMode.
        }

        private void comboBox_IsUseUDP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isudp = comboBox_IsUseUDP.SelectedIndex == 1 ? true : false;

            groupBox_UDP.Visibility = isudp ? Visibility.Visible : Visibility.Collapsed;
            label_mode.Visibility = isudp ? Visibility.Hidden : Visibility.Visible;
            comboBox_mode.Visibility = isudp ? Visibility.Hidden : Visibility.Visible;
            //协议
            SSSetValue($"Net{NetNum}IsUdpProtocol", BytesHelper.intToBytes(isudp ? 1 : 0, 2));
            SDWindow.UI_tcpudp();
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

        /// <summary>
        /// 结构体修改属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="msg"></param>
        private void SSSetValue(string name, object msg)
        {
            //ip地址
            Object xx = Device.Struct_SS;
            ReflectionHelper.SetFieldValue(xx, name, msg);
            Device.Struct_SS = (SerialDevice_Struct)xx;
        }

        private void texbox_ip_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text.IsIP())
            {
                //ip地址
                SSSetValue($"Net{NetNum}_cfg_netip", CMDCode.StringIpToBytes(texbox_ip.Text));
            }
            else
            {
                MessageBox.Show("输入的IP地址不正确");
                SetDevice(Device);
            }
        }

        private void texbox_gateway_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text.IsIP())
            {
                //ip地址
                SSSetValue($"Net{NetNum}_cfg_netGateway", CMDCode.StringIpToBytes(texbox_gateway.Text));
            }
            else
            {
                MessageBox.Show("输入的网关地址不正确");
                SetDevice(Device);
            }
        }

        private void texbox_mask_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text.IsIP())
            {
                //ip地址
                SSSetValue($"Net{NetNum}_cfg_netmask", CMDCode.StringIpToBytes(texbox_mask.Text));
            }
            else
            {
                MessageBox.Show("输入的子网掩码不正确");
                SetDevice(Device);
            }
        }

        private void texbox_port_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            int num;
            if (tb.Text.IsNetPort(out num))
            {
                //ip地址
                SSSetValue($"Net{NetNum}BasePort", BytesHelper.intToBytes(num, 2));
            }
            else
            {
                MessageBox.Show("输入的基础端口号错误");
                SetDevice(Device);
            }
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
            UInt16 num = 0;
            switch (selectindex)
            {
                case 0:
                    num = 0;
                    break;

                case 1:
                    num = 0xA5A5;
                    break;

                case 2:
                    num = 0x5A5A;
                    break;
            }
            //限制模式
            SSSetValue($"Net{NetNum}_cfg_LimitIPMode", BytesHelper.intToBytes(num, 2));
        }

        private void Tb_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text.IsIP())
            {
                if (NetNum == 0)
                {
                    int index = int.Parse(tb.Name.Substring(tb.Name.Length - 1));
                    if (tb.Name.StartsWith("textBoxipFilter1_"))//限制允许地址
                    {
                        Device.Struct_SS.Net0_cfg_utLimitIP.Replace(CMDCode.StringIpToBytes(tb.Text.Trim()), index * 4);
                    }
                    else if (tb.Name.StartsWith("textBoxmask"))
                    {
                        Device.Struct_SS.Net0_cfg_utLimitMask.Replace(CMDCode.StringIpToBytes(tb.Text.Trim()), index * 4);
                    }
                    else if (tb.Name.StartsWith("textBox_UDPIP"))
                    {
                        Device.Struct_SS.Net0_udp.Replace(CMDCode.StringIpToBytes(tb.Text.Trim()), index * 4);
                    }
                }
                else
                {
                    int index = int.Parse(tb.Name.Substring(tb.Name.Length - 2));
                    if (tb.Name.StartsWith("textBoxipFilter1_"))//限制允许地址
                    {
                        Device.Struct_SS.Net1_cfg_utLimitIP.Replace(CMDCode.StringIpToBytes(tb.Text.Trim()), index * 4);
                    }
                    else if (tb.Name.StartsWith("textBoxmask"))
                    {
                        Device.Struct_SS.Net1_cfg_utLimitMask.Replace(CMDCode.StringIpToBytes(tb.Text.Trim()), index * 4);
                    }
                    else if (tb.Name.StartsWith("textBox_UDPIP"))
                    {
                        Device.Struct_SS.Net1_udp.Replace(CMDCode.StringIpToBytes(tb.Text.Trim()), index * 4);
                    }
                }
            }
            else
            {
                MessageBox.Show("输入的格式错误");
                SetDevice(Device);
            }
        }

        private void Tb_LostFocus2(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            int num;
            if (tb.Text.IsNetPort(out num))
            {
                if (NetNum == 0)
                {
                    int index = int.Parse(tb.Name.Substring(tb.Name.Length - 1));

                    if (tb.Name.StartsWith("textBox_UDPPort"))
                    {
                        Device.Struct_SS.Net0_udp_port.Replace(BytesHelper.intToBytes(num, 2), index * 2);
                    }
                }
                else
                {
                    int index = int.Parse(tb.Name.Substring(tb.Name.Length - 2));
                    if (tb.Name.StartsWith("textBox_UDPPort"))
                    {
                        Device.Struct_SS.Net1_udp_port.Replace(BytesHelper.intToBytes(num, 2), index * 2);
                    }
                }
            }
            else
            {
                MessageBox.Show("输入的格式错误");
                SetDevice(Device);
            }
        }

        private void checkBox_mac_Click(object sender, RoutedEventArgs e)
        {
            SSSetValue($"Net{NetNum}IsMacAsigned", BytesHelper.intToBytes(checkBox_mac.IsChecked.Value ? 1 : 0, 2));
        }

        private void textBox_mac_LostFocus(object sender, RoutedEventArgs e)
        {
            //修改mac
            TextBox tb = sender as TextBox;

            if (tb.Text.IsMac())
            {
                //ip地址
                SSSetValue($"Net{NetNum}MacAsigned", CMDCode.StringMACToBytes(tb.Text));
            }
            else
            {
                MessageBox.Show("输入的MAC错误");
                SetDevice(Device);
            }
        }

        private void checkBox_portAll_Click(object sender, RoutedEventArgs e)
        {
            //udp 所有都发送到指定端口
            SSSetValue($"Net{NetNum}UdpComPortAllSame", BytesHelper.intToBytes(checkBox_portAll.IsChecked.Value ? 65535 : 0, 2));
        }

        private void comboBox_mode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbbox = sender as ComboBox;
            if (cbbox.SelectedIndex > -1)
            {
                //模式
                SSSetValue($"Net{NetNum}TcpMode", BytesHelper.intToBytes(cbbox.SelectedIndex, 2));
                SDWindow.UI_tcpudp();
            }
        }

        // void SSbytesReplace(byte[] bytes,byte[] rebytes,int beginpos)
        //{
        //    bytes.Replace()
        //}
    }
}