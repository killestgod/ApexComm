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
    /// NetCfg.xaml 的交互逻辑
    /// </summary>
    public partial class NetCfg : UserControl
    {
        public SerialDevice_Struct ss;

        /// <summary>
        /// 使用的是网口几
        /// </summary>
        public int NetPort = 1;

        public NetCfg()
        {
            InitializeComponent();
            this.Height = 270;
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
    }
}