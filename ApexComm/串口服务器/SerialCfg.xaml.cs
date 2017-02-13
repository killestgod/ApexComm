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
using ApexComm;

namespace ApexComm.串口服务器
{
    /// <summary>
    /// SerialCfg.xaml 的交互逻辑
    /// </summary>
    public partial class SerialCfg : UserControl
    {
        private SerialDevice Device;

        /// <summary>
        /// 当前选中项
        /// </summary>
        private int selectcomNum = 0;

        private int LastSelectIndex = 0;
        private int mod = 0;

        /// <summary>
        /// 位运算索引
        /// </summary>
        private int wi, wb;

        public SerialCfg()
        {
            InitializeComponent();

            comboBox_baudrate.ItemsSource = new int[] {300,600,1200,2400,4800,9600,19200,
                38400,57600,115200,230400,345600,
                460800,691200};
            comboBox1_databit.ItemsSource = new int[] {
                5,6,7,8
            };
            comboBox1_Parity.ItemsSource = new string[] {
                "None(无)","Odd(奇)","Even(偶)","Space","Mark"
            };
            comboBox1_StopBit.ItemsSource = new int[] { 1, 2 };
            comboBox1_driverMode.ItemsSource = new string[] {
                "全双工RS232(用于近距离点对点通信)",
                "全双工RS422(用于远距离点对点通信)",
                "半双工RS485(用于远距离的主从和问答式多机总线通信)"
            };
            comboBox_ReLinkTcpCtrl.ItemsSource = new string[]
            {
                "禁止",
                "同一IP可以(条件:网络接收空闲超时)",
                "同一IP可以(无延时)",
                "任意IP可以(条件:网络接收空闲超时)",
                "任意IP均可(无延时)"
            };
            comboBoxIsFrameMode.ItemsSource = new string[]
            {
                "流方式 --定时转发",
                "帧方式 --分帧转发"
            };
            textBox_NetIdleTTL.SetInputOnlyNumber();
            textBox_NetIdleTTL.MaxLength = 3;
            textBox_ComIdleTTL.SetInputOnlyNumber();
            textBox_ComIdleTTL.MaxLength = 3;
            textBox_Delay.SetInputOnlyNumber();
            textBox_Delay.MaxLength = 3;
        }

        public void SetDevice(SerialDevice sd)
        {
            comboBox_comport.SelectedIndex = -1;
            Device = sd;
            SerialDevice_Struct ss = sd.Struct_SS;
            //获取串口数量
            int comnum = int.Parse(sd.TypeName.Substring(3, 2));

            string[] coms = new string[comnum];
            for (int i = 0; i < coms.Length; i++)
            {
                coms[i] = string.Format("Port{0:D2}", i + 1);
            }
            comboBox_comport.ItemsSource = coms;
            comboBox_comport.SelectedIndex = LastSelectIndex;
            //网口数量
            int netnum = int.Parse(Device.TypeName.Substring(1, 1));

            net1cfg.Visibility = netnum == 1 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void comboBox_comport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectcomNum = comboBox_comport.SelectedIndex;
            if (selectcomNum == -1)
                return;
            LastSelectIndex = selectcomNum;
            mod = 0;
            if (selectcomNum % 2 == 0)
            {
                mod = 1;
            }
            else
            {
                mod = -1;
            }
            //停止位
            wi = selectcomNum / 8;
            wb = selectcomNum % 8;

            if (wi >= 2)
            {
                wi = wi - 2;
            }
            else
            {
                wi = wi + 2;
            }
            //波特率
            comboBox_baudrate.SelectedValue = Device.Struct_SS.comm_cfg_baudrate.CloneRange(selectcomNum * 4, 4).ConvertbyteToInt();

            //数据位
            comboBox1_databit.SelectedValue = (int)Device.Struct_SS.comm_cfg_databit[selectcomNum + mod];
            //校验
            comboBox1_Parity.SelectedIndex = Device.Struct_SS.comm_cfg_comParity[selectcomNum + mod];
            //tcp是否断线重连

            //外部驱动接口类型
            comboBox1_driverMode.SelectedIndex = Device.Struct_SS.comm_cfg_driverMode.CloneRange(2 * selectcomNum, 2).bytesToInt(0, 2);

            // byte[] tbytes = CMDFactory.ConvertSwapBytes(Device.Struct_SS.comm_cfg_comStopBit);
            comboBox1_StopBit.SelectedIndex = BytesHelper.get_bit(Device.Struct_SS.comm_cfg_comStopBit[wi], wb) ? 1 : 0;
            //tcp失败时是否丢弃数据
            checkBox_ldischrwhennotcp.IsChecked = BytesHelper.get_bit(Device.Struct_SS.tcp_cfg_ldischrwhennotcp[wi], wb) ? true : false;
            //tcp是否断线重连
            comboBox_ReLinkTcpCtrl.SelectedIndex = (int)Device.Struct_SS.tcp_cfg_ReLinkTcpCtrl[selectcomNum + mod];
            //网络超时
            textBox_NetIdleTTL.Text = Device.Struct_SS.tcp_cfg_NetIdleTTL.CloneRange(selectcomNum * 2, 2).bytesToInt(0, 2).ToString();
            //串口超时
            textBox_ComIdleTTL.Text = Device.Struct_SS.tcp_cfg_ComIdleTTL.CloneRange(selectcomNum * 2, 2).bytesToInt(0, 2).ToString();
            //帧 流方式
            comboBoxIsFrameMode.SelectedIndex = BytesHelper.get_bit(Device.Struct_SS.net_cfg_IsFrameMode[wi], wb) ? 1 : 0;
            //网络延迟
            textBox_Delay.Text = Device.Struct_SS.net_cfg_Delay.CloneRange(selectcomNum * 2, 2).bytesToInt(0, 2).ToString();
            //网口自动连接

            checkBoxAutoTcpCnntA.IsChecked = BytesHelper.get_bit(Device.Struct_SS.net_cfg_AutoTcpCnntA[wi], wb) ? true : false;

            checkBoxAutoTcpCnntB.IsChecked = BytesHelper.get_bit(Device.Struct_SS.net_cfg_AutoTcpCnntB[wi], wb) ? true : false;
            //对方IP
            byte[] tbytes = Device.Struct_SS.net_cfg_Remoteipa.CloneRange(selectcomNum * 4, 4);

            textBox_netIPA.Text = tbytes.Swap01To10().ToHexString_10(".");
            tbytes = Device.Struct_SS.net_cfg_Remoteipb.CloneRange(selectcomNum * 4, 4);
            textBox_netIPB.Text = tbytes.Swap01To10().ToHexString_10(".");
            //对方端口
            textBox1_netportA.Text = BytesHelper.bytesToInt(Device.Struct_SS.net_cfg_parta, selectcomNum * 2, 2).ToString();
            textBox1_netportB.Text = BytesHelper.bytesToInt(Device.Struct_SS.net_cfg_partb, selectcomNum * 2, 2).ToString();
        }

        private void comboBoxIsFrameMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isframe = false;
            if (comboBoxIsFrameMode.SelectedIndex == 0)
            {
                isframe = false;
                label_danwei.Content = "×10ms";//
            }
            else
            {
                isframe = true;
                label_danwei.Content = "×1字节传输时间";
            }

            //帧 流方式

            // comboBoxIsFrameMode.SelectedIndex = BytesHelper.get_bit(Device.Struct_SS.net_cfg_IsFrameMode[wi], wb) ? 1 : 0;

            Device.Struct_SS.net_cfg_IsFrameMode[wi] = BytesHelper.set_bit(Device.Struct_SS.net_cfg_IsFrameMode[wi], wb, isframe);
        }

        private void comboBox_baudrate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbbox = sender as ComboBox;
            if (cbbox.SelectedIndex > -1)
            {
                //波特率
                // comboBox_baudrate.SelectedValue = Device.Struct_SS.comm_cfg_baudrate.CloneRange(selectcomNum * 4, 4).ConvertbyteToInt();
                byte[] temp = ((int)(cbbox.Items.GetItemAt(cbbox.SelectedIndex))).ConvertIntToByte();
                Device.Struct_SS.comm_cfg_baudrate.Replace(temp, selectcomNum * 4);
            }
        }

        private void comboBox1_databit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbbox = sender as ComboBox;
            if (cbbox.SelectedIndex > -1)
            {
                //数据位
                //comboBox1_databit.SelectedValue = (int)Device.Struct_SS.comm_cfg_databit[selectcomNum + mod];
                Device.Struct_SS.comm_cfg_databit[selectcomNum + mod] = (byte)((int)cbbox.Items.GetItemAt(cbbox.SelectedIndex));
            }
        }

        private void comboBox1_Parity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbbox = sender as ComboBox;
            if (cbbox.SelectedIndex > -1)
            {
                //校验
                //comboBox1_Parity.SelectedIndex = Device.Struct_SS.comm_cfg_comParity[selectcomNum + mod];
                Device.Struct_SS.comm_cfg_comParity[selectcomNum + mod] = (byte)cbbox.SelectedIndex;
            }
        }

        private void comboBox1_driverMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbbox = sender as ComboBox;
            if (cbbox.SelectedIndex > -1)
            { //外部驱动接口类型
                byte[] temp = BytesHelper.intToBytes(cbbox.SelectedIndex, 2);

                Device.Struct_SS.comm_cfg_driverMode.Replace(temp, 2 * selectcomNum);
            }
        }

        private void checkBox_ldischrwhennotcp_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            //tcp失败时是否丢弃数据
            Device.Struct_SS.tcp_cfg_ldischrwhennotcp[wi] = BytesHelper.set_bit(Device.Struct_SS.tcp_cfg_ldischrwhennotcp[wi], wb, cb.IsChecked.Value);
        }

        private void comboBox_ReLinkTcpCtrl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbbox = sender as ComboBox;
            if (cbbox.SelectedIndex > -1)
            {
                //tcp是否断线重连
                Device.Struct_SS.tcp_cfg_ReLinkTcpCtrl[selectcomNum + mod] = (byte)cbbox.SelectedIndex;
            }
        }

        private void textBox_NetIdleTTL_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tbx = sender as TextBox;
            int num;
            Device.IsCheck = tbx.Text.Is0_999(out num);
            if (Device.IsCheck)
            {
                //网络超时
                Device.Struct_SS.tcp_cfg_NetIdleTTL.Replace(BytesHelper.intToBytes(num, 2), selectcomNum * 2);
            }
            else
            {
                MessageBox.Show("输入有误:\r\n  链接状态下网络接收空闲限时（双网口模式下UDP允许切换超时）, 0-4: 链接不关闭, 5-999: 超时关闭链接（默认: 0）");
                SetDevice(Device);
            }
        }

        private void textBox_ComIdleTTL_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tbx = sender as TextBox;
            int num;
            Device.IsCheck = tbx.Text.Is0_999(out num);
            if (Device.IsCheck)
            {
                //网络超时
                Device.Struct_SS.tcp_cfg_ComIdleTTL.Replace(BytesHelper.intToBytes(num, 2), selectcomNum * 2);
            }
            else
            {
                MessageBox.Show("输入有误:\r\n  链接状态下串口接收空闲限时, 0-4: 链接不关闭, 5-999: 超时关闭链接（默认: 0）");
                SetDevice(Device);
            }
        }

        private void textBox_Delay_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tbx = sender as TextBox;
            int num;
            Device.IsCheck = tbx.Text.Is0_999(out num);
            if (Device.IsCheck)
            {
                //超时个数
                Device.Struct_SS.net_cfg_Delay.Replace(BytesHelper.intToBytes(num, 2), selectcomNum * 2);
            }
            else
            {
                MessageBox.Show("输入有误:\r\n 串口信息转发延时: 流方式－单位: 10ms; 帧方式－单位: 1字节传输时间, 与波特率有关; 使用时自动加1个单位; （默认: 10）; 0-999）");
                SetDevice(Device);
            }
        }

        private void checkBoxAutoTcpCnntA_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            //tcp失败时是否丢弃数据
            Device.Struct_SS.net_cfg_AutoTcpCnntA[wi] = BytesHelper.set_bit(Device.Struct_SS.net_cfg_AutoTcpCnntA[wi], wb, cb.IsChecked.Value);
        }

        private void checkBoxAutoTcpCnntB_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            //tcp失败时是否丢弃数据
            Device.Struct_SS.net_cfg_AutoTcpCnntB[wi] = BytesHelper.set_bit(Device.Struct_SS.net_cfg_AutoTcpCnntB[wi], wb, cb.IsChecked.Value);
        }

        private void textBox_netIPA_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tbx = sender as TextBox;
            if (tbx.Text.IsIP())
            {
                byte[] ip = CMDCode.StringIpToBytes(textBox_netIPA.Text);
                Device.Struct_SS.net_cfg_Remoteipa.Replace(ip, selectcomNum * 4);
            }
            else
            {
                MessageBox.Show("输入的IP地址不正确");
                SetDevice(Device);
            }
        }

        private void textBox_netIPB_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tbx = sender as TextBox;
            if (tbx.Text.IsIP())
            {
                byte[] ip = CMDCode.StringIpToBytes(textBox_netIPB.Text);
                Device.Struct_SS.net_cfg_Remoteipb.Replace(ip, selectcomNum * 4);
            }
            else
            {
                MessageBox.Show("输入的IP地址不正确");
                SetDevice(Device);
            }
        }

        private void textBox1_netportA_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tbx = sender as TextBox;
            int num;
            Device.IsCheck = tbx.Text.IsNetPort(out num);
            if (Device.IsCheck)
            {
                //端口
                Device.Struct_SS.net_cfg_parta.Replace(BytesHelper.intToBytes(num, 2), selectcomNum * 2);
            }
            else
            {
                MessageBox.Show("输入有误:\r\n  对方PORT（仅用于Client方式的主动链接）（默认: 5101）");
                SetDevice(Device);
            }
        }

        private void textBox1_netportB_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tbx = sender as TextBox;
            int num;
            Device.IsCheck = tbx.Text.IsNetPort(out num);
            if (Device.IsCheck)
            {
                //端口
                Device.Struct_SS.net_cfg_partb.Replace(BytesHelper.intToBytes(num, 2), selectcomNum * 2);
            }
            else
            {
                MessageBox.Show("输入有误:\r\n  对方PORT（仅用于Client方式的主动链接）（默认: 5101）");
                SetDevice(Device);
            }
        }

        private void comboBox1_StopBit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbbox = sender as ComboBox;
            if (cbbox.SelectedIndex > -1)
            {
                bool t = cbbox.SelectedIndex == 1 ? true : false;
                //停止位
                Device.Struct_SS.comm_cfg_comStopBit[wi] = BytesHelper.set_bit(Device.Struct_SS.comm_cfg_comStopBit[wi], wb, t);
            }
        }

        private void button_setAll_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("您确认要使其它所有串口使用和当前显示串口相同的设置吗", "警告", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                for (int i = 0; i < 32; i++)
                {
                    int selectcomNum = i;
                    int mod = 0;

                    if (selectcomNum % 2 == 0)
                    {
                        mod = 1;
                    }
                    else
                    {
                        mod = -1;
                    }
                    //停止位
                    int wi = selectcomNum / 8;
                    int wb = selectcomNum % 8;

                    if (wi >= 2)
                    {
                        wi = wi - 2;
                    }
                    else
                    {
                        wi = wi + 2;
                    }
                    //波特率
                    byte[] temp = ((int)(comboBox_baudrate.Items.GetItemAt(comboBox_baudrate.SelectedIndex))).ConvertIntToByte();
                    Device.Struct_SS.comm_cfg_baudrate.Replace(temp, selectcomNum * 4);
                    //数据位
                    Device.Struct_SS.comm_cfg_databit[selectcomNum + mod] = (byte)((int)comboBox1_databit.Items.GetItemAt(comboBox1_databit.SelectedIndex));
                    //校验
                    Device.Struct_SS.comm_cfg_comParity[selectcomNum + mod] = (byte)comboBox1_Parity.SelectedIndex;
                    //停止位
                    bool t = comboBox1_StopBit.SelectedIndex == 1 ? true : false;
                    Device.Struct_SS.comm_cfg_comStopBit[wi] = BytesHelper.set_bit(Device.Struct_SS.comm_cfg_comStopBit[wi], wb, t);
                    //外部驱动接口类型
                    temp = BytesHelper.intToBytes(comboBox1_driverMode.SelectedIndex, 2);

                    Device.Struct_SS.comm_cfg_driverMode.Replace(temp, 2 * selectcomNum);
                    //帧 流方式
                    bool isframe = comboBoxIsFrameMode.SelectedIndex == 0 ? false : true;
                    Device.Struct_SS.net_cfg_IsFrameMode[wi] = BytesHelper.set_bit(Device.Struct_SS.net_cfg_IsFrameMode[wi], wb, isframe);
                    //tcp失败时是否丢弃数据
                    Device.Struct_SS.tcp_cfg_ldischrwhennotcp[wi] = BytesHelper.set_bit(Device.Struct_SS.tcp_cfg_ldischrwhennotcp[wi], wb, checkBox_ldischrwhennotcp.IsChecked.Value);
                    //tcp是否断线重连
                    Device.Struct_SS.tcp_cfg_ReLinkTcpCtrl[selectcomNum + mod] = (byte)comboBox_ReLinkTcpCtrl.SelectedIndex;
                    //网络超时
                    Device.Struct_SS.tcp_cfg_NetIdleTTL.Replace(BytesHelper.intToBytes(int.Parse(textBox_NetIdleTTL.Text), 2), selectcomNum * 2);
                    //网络超时
                    Device.Struct_SS.tcp_cfg_ComIdleTTL.Replace(BytesHelper.intToBytes(int.Parse(textBox_ComIdleTTL.Text), 2), selectcomNum * 2);
                    //超时个数
                    Device.Struct_SS.net_cfg_Delay.Replace(BytesHelper.intToBytes(int.Parse(textBox_Delay.Text), 2), selectcomNum * 2);
                    // tcp失败时是否丢弃数据
                    Device.Struct_SS.net_cfg_AutoTcpCnntA[wi] = BytesHelper.set_bit(Device.Struct_SS.net_cfg_AutoTcpCnntA[wi], wb, checkBoxAutoTcpCnntA.IsChecked.Value);
                    Device.Struct_SS.net_cfg_AutoTcpCnntB[wi] = BytesHelper.set_bit(Device.Struct_SS.net_cfg_AutoTcpCnntB[wi], wb, checkBoxAutoTcpCnntB.IsChecked.Value);
                    //ip
                    byte[] ip = CMDCode.StringIpToBytes(textBox_netIPA.Text);
                    Device.Struct_SS.net_cfg_Remoteipa.Replace(ip, selectcomNum * 4);
                    ip = CMDCode.StringIpToBytes(textBox_netIPB.Text);
                    Device.Struct_SS.net_cfg_Remoteipb.Replace(ip, selectcomNum * 4);
                    //端口
                    Device.Struct_SS.net_cfg_parta.Replace(BytesHelper.intToBytes(int.Parse(textBox1_netportA.Text), 2), selectcomNum * 2);
                    Device.Struct_SS.net_cfg_partb.Replace(BytesHelper.intToBytes(int.Parse(textBox1_netportB.Text), 2), selectcomNum * 2);
                }
                SetDevice(Device);
            }
        }

        private void UI_tcpudp(bool Isshow)
        {
        }
    }
}