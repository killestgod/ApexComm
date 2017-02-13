using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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
using System.Collections.ObjectModel;
using System.Threading;
using Xceed.Wpf.DataGrid;
using ApexComm.串口服务器;

namespace ApexComm
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// udp服务
        /// </summary>
        private static int UdpPort = 61199;

        /// <summary>
        /// udp服务列表
        /// </summary>
        private List<UdpClient> udpclients = new List<UdpClient>();

        /// <summary>
        /// 设备列表集合
        /// </summary>
        public static MyObservableCollection<MyDevice> DeviceCollection = new MyObservableCollection<MyDevice>();

        public MainWindow()
        {
            InitializeComponent();

            //数据绑定
            Binding binding = new Binding();
            binding.Source = DeviceCollection;
            datagrid.SetBinding(DataGridControl.ItemsSourceProperty, binding);
            DefaultStyle = busywait.Style;
            WindowHelper.ReSetWindowsSize(this);
        }

        /// <summary>
        /// yong
        /// </summary>
        private object lockobj = new object();

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

                //
                IPEndPoint localendpoint = udpClient.Client.LocalEndPoint as IPEndPoint;

                Console.WriteLine("clientendip:" + clientendip + "     localendpoint" + localendpoint);
                if (localendpoint.Address.ToString() == clientendip.Address.ToString())
                {
                    return;
                }
                lock (lockobj)
                {
                    ReceiveMsg(ReceiveByte, localendpoint, clientendip);
                }

                LogMsg.log(clientendip.Address + " :   " + BytesHelper.ToHexString(ReceiveByte));
            }
            catch (ObjectDisposedException ex)
            {
            }
            catch (Exception ex)
            {
                LogMsg.log_error("通讯失败:" + ex.ToString());
            }
        }

        private Style DefaultStyle;

        private void btn_searchall_Click(object sender, RoutedEventArgs e)
        {
            //Style="{DynamicResource mybusy_scan}"

            busywait.Style = FindResource("mybusy_scan") as Style;
            DeviceCollection.Clear();
            busywait.IsBusy = true;
            Task task = new Task(() =>
            {
                for (int i = 0; i < 2; i++)
                {
                    UdpBroadcast(CMDFactory.MakeCMDBytes(ApexComm.串口服务器.CMDCode.搜索设备.CMD_Send, new byte[] { }));
                    Thread.Sleep(1000);
                }
            });

            task.Start();
            task.ContinueWith((t) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    busywait.IsBusy = false;
                    busywait.Style = DefaultStyle;
                    foreach (var udp in udpclients)
                    {
                        try
                        {
                            udp.Close();
                        }
                        catch
                        {
                        }
                    }
                    //udpclients.Clear();
                    LogMsg.log("清空udp列表");
                }));
            });
        }

        private void UdpBroadcast(byte[] cmd)
        {
            if (udpclients.Count == 0)
            {
                foreach (var ip in NetHelper.GetIPv4List())
                {
                    try
                    {
                        LogMsg.log("IP:" + ip.ToString());
                        UdpClient tempudp = new UdpClient(new IPEndPoint(ip, UdpPort));
                        tempudp.Client.ReceiveTimeout = 3000;
                        tempudp.BeginReceive(ReceiveCallback, tempudp);
                        udpclients.Add(tempudp);
                    }
                    catch
                    {
                        // LogMsg.log_error(ip + "    udp初始化:" + ex.ToString());
                    }
                }
            }

            foreach (var udp in udpclients)
            {
                try
                {
                    udp.Send(cmd, cmd.Length, new IPEndPoint(IPAddress.Broadcast, UdpPort));
                }
                catch (Exception ex)
                {
                    LogMsg.log_error("UDP广播出错:" + ex.ToString());
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void datagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine(datagrid.SelectedItem.ToString());

            if (datagrid.SelectedItem != null)
            {
                if (datagrid.SelectedItem is SerialDevice)
                {
                    SerialDevice sd = datagrid.SelectedItem as SerialDevice;
                    Showcfg(sd);
                }
            }
        }

        private void datagrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
            if (datagrid.SelectedItem != null)
            {
                if (datagrid.SelectedItem is SerialDevice)
                {
                    SerialDevice sd = datagrid.SelectedItem as SerialDevice;
                    DeviceCfgControl.Content = null;
                    SerialDeviceControl sdControl = new SerialDeviceControl();
                    DeviceCfgControl.Content = sdControl;
                    sdControl.ShowCfg = Showcfg;
                    sdControl.mainWindow = this;
                    sdControl.SetDevice(sd);
                }
            }
            else
            {
                DeviceCfgControl.Content = null;
            }
        }

        public void Showcfg(SerialDevice sd)
        {
            if (sd.SVer != "Ver 04.2D")
            {
                MessageBox.Show(this, "此设备的程序版本不兼容,只支持 Ver04.2D");
                return;
            }
            this.Hide();
            ApexComm.串口服务器.SerialDeviceWindow sw = new ApexComm.串口服务器.SerialDeviceWindow();
            sw.MyDevice = sd;
            sw.Owner = this;
            sw.ShowDialog();

            this.Show();
            btn_searchall_Click(null, null);
        }

        #region 子控件的udp通讯

        ////

        #endregion 子控件的udp通讯

        private void btn_searcharea_Click(object sender, RoutedEventArgs e)
        {
            // busywait.IsBusy = true;
        }

        private void button_init_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}