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
            listView.SetBinding(ListView.ItemsSourceProperty, binding);
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

                //
                IPEndPoint localendpoint = udpClient.Client.LocalEndPoint as IPEndPoint;

                Console.WriteLine("clientendip:" + clientendip + "     localendpoint" + localendpoint);
                if (localendpoint.Address.ToString() == clientendip.Address.ToString())
                {
                    return;
                }

                ReceiveMsg(ReceiveByte, localendpoint, clientendip);
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

        private void btn_searchall_Click(object sender, RoutedEventArgs e)
        {
            DeviceCollection.Clear();
            busywait.IsBusy = true;
            Task task = new Task(() =>
            {
                Console.WriteLine(NetHelper.GetIPv4List().Count);
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
                    catch (Exception ex)
                    {
                        LogMsg.log_error(ip + "    udp初始化:" + ex.ToString());
                    }
                }

                for (int i = 0; i < 1; i++)
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
                    foreach (var udp in udpclients)
                    {
                        udp.Close();
                    }
                    udpclients.Clear();
                    LogMsg.log("清空udp列表");
                }));
            });
        }

        private void UdpBroadcast(byte[] cmd)
        {
            foreach (var udp in udpclients)
            {
                try
                {
                    udp.Send(cmd, cmd.Length, new IPEndPoint(IPAddress.Broadcast, UdpPort));
                }
                catch (Exception ex)
                {
                    LogMsg.log_error("UDP广播:" + ex.ToString());
                }
            }
        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine(listView.SelectedItems.ToString());
            if (listView.SelectedValue != null)
            {
                if (listView.SelectedValue is SerialDevice)
                {
                    SerialDevice sd = listView.SelectedValue as SerialDevice;
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
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
    }
}