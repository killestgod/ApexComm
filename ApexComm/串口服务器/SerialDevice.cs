using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApexComm
{
    public class SerialDevice : MyDevice
    {
        /// <summary>
        /// 数据结构体对象
        /// </summary>
        public SerialDevice_Struct Struct_SS;

        /// <summary>
        /// 有效设置标识
        /// </summary>
        public string ApexSetFlag;

        private string name;

        //装置类型
        private string typeName;

        /// <summary>
        /// 程序版本号
        /// </summary>
        private string pVer;

        /// <summary>
        /// 设置版本号
        /// </summary>
        private string sVer;

        private string net1_IP;
        private string net1_Mask;
        private string net2_IP;
        private string net2_Mask;

        #region INotifyPropertyChanged 接口实现

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public string TypeName
        {
            get
            {
                return typeName;
            }

            set
            {
                typeName = value;
                OnPropertyChanged("TypeName");
            }
        }

        public string PVer
        {
            get
            {
                return pVer;
            }

            set
            {
                pVer = value;
                OnPropertyChanged("PVer");
            }
        }

        public string SVer
        {
            get
            {
                return sVer;
            }

            set
            {
                sVer = value;
                OnPropertyChanged("SVer");
            }
        }

        public string Net1_IP
        {
            get
            {
                return net1_IP;
            }

            set
            {
                net1_IP = value;
                OnPropertyChanged("Net1_IP");
            }
        }

        public string Net1_Mask
        {
            get
            {
                return net1_Mask;
            }

            set
            {
                net1_Mask = value;
                OnPropertyChanged("Net1_Mask");
            }
        }

        public string Net2_IP
        {
            get
            {
                return net2_IP;
            }

            set
            {
                net2_IP = value;
                OnPropertyChanged("Net2_IP");
            }
        }

        public string Net2_Mask
        {
            get
            {
                return net2_Mask;
            }

            set
            {
                net2_Mask = value;
                OnPropertyChanged("Net2_Mask");
            }
        }

        #endregion INotifyPropertyChanged 接口实现
    }
}