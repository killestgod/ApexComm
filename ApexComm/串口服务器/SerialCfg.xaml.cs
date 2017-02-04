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
    /// SerialCfg.xaml 的交互逻辑
    /// </summary>
    public partial class SerialCfg : UserControl
    {
        private SerialDevice Device;
        private int selectcomNum;

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
                "流方式--定时转发",
                "帧方式--分帧转发"
            };
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
            comboBox_comport.SelectedIndex = 0;
        }

        private void comboBox_comport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectcomNum = comboBox_comport.SelectedIndex;
            if (selectcomNum == -1)
                return;
            int mod = 0;
            if (selectcomNum % 2 == 0)
            {
                mod = 1;
            }
            else
            {
                mod = -1;
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
            //停止位
            int i = selectcomNum / 8;
            int b = selectcomNum % 8;
            byte[] tbytes = CMDFactory.ConvertSwapBytes(Device.Struct_SS.comm_cfg_comStopBit);
            comboBox1_StopBit.SelectedIndex = BytesHelper.get_bit(tbytes[i], b) ? 1 : 0;
            //tcp失败时是否丢弃数据
            tbytes = CMDFactory.ConvertSwapBytes(Device.Struct_SS.tcp_cfg_ldischrwhennotcp);
            checkBox_ldischrwhennotcp.IsChecked = BytesHelper.get_bit(tbytes[i], b) ? true : false;
            //tcp是否断线重连
            comboBox_ReLinkTcpCtrl.SelectedIndex = (int)Device.Struct_SS.tcp_cfg_ReLinkTcpCtrl[selectcomNum + mod];
            //网络超时
            textBox_NetIdleTTL.Text = Device.Struct_SS.tcp_cfg_NetIdleTTL.CloneRange(selectcomNum * 2, 2).bytesToInt(0, 2).ToString();
            //串口超时
            textBox_ComIdleTTL.Text = Device.Struct_SS.tcp_cfg_ComIdleTTL.CloneRange(selectcomNum * 2, 2).bytesToInt(0, 2).ToString();
            //帧 流方式
            tbytes = CMDFactory.ConvertSwapBytes(Device.Struct_SS.net_cfg_IsFrameMode);
            comboBoxIsFrameMode.SelectedIndex = BytesHelper.get_bit(tbytes[i], b) ? 1 : 0;
            //网络延迟
            textBox_Delay.Text = Device.Struct_SS.net_cfg_Delay.CloneRange(selectcomNum * 2, 2).bytesToInt(0, 2).ToString();
            //网口自动连接
            tbytes = CMDFactory.ConvertSwapBytes(Device.Struct_SS.net_cfg_AutoTcpCnntA);
            checkBoxAutoTcpCnntA.IsChecked = BytesHelper.get_bit(tbytes[i], b) ? true : false;
            tbytes = CMDFactory.ConvertSwapBytes(Device.Struct_SS.net_cfg_AutoTcpCnntB);
            checkBoxAutoTcpCnntB.IsChecked = BytesHelper.get_bit(tbytes[i], b) ? true : false;
            //对方IP
            tbytes = Device.Struct_SS.net_cfg_Remoteipa.CloneRange(selectcomNum * 4, 4);

            textBox_netIPA.Text = tbytes.Swap01To10().ToHexString_10(".");
            tbytes = Device.Struct_SS.net_cfg_Remoteipb.CloneRange(selectcomNum * 4, 4);
            textBox_netIPB.Text = tbytes.Swap01To10().ToHexString_10(".");
            //对方端口
            textBox1_netportA.Text = BytesHelper.bytesToInt(Device.Struct_SS.net_cfg_parta, selectcomNum * 2, 2).ToString();
            textBox1_netportB.Text = BytesHelper.bytesToInt(Device.Struct_SS.net_cfg_partb, selectcomNum * 2, 2).ToString();
        }

        private void comboBoxIsFrameMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxIsFrameMode.SelectedIndex == 0)
            {
                label_danwei.Content = "×10ms";//
            }
            else
            {
                label_danwei.Content = "×1字节传输时间";
            }
        }
    }
}