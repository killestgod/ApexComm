using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;

namespace ApexComm
{
    /// <summary>
    /// 所有设备类型的基类
    /// </summary>
    public class MyDevice : INotifyPropertyChanged
    {
        //本地地址
        public IPEndPoint PC_Endpoint;

        //设备地址
        public IPEndPoint Client_Endpoint;

        private string sN;

        /// <summary>
        /// SN 16
        /// </summary>
        public string SN
        {
            get
            {
                return sN;
            }

            set
            {
                sN = value;
                OnPropertyChanged("SN");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public override string ToString()
        {
            return "SN:" + sN;
        }
    }
}