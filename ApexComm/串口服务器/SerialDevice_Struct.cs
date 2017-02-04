using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace ApexComm
{
    public struct SerialDevice_Struct
    {
        //EB 90 EB 90
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] head;

        // 16 文本
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] sn;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] cmdcode;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] len;

        //正文
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] cmdtype;//请求方式 eprom 还是映射

        //标识 文本 APEX
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] setFlagA_B;

        //程序版本
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] pVer;

        //编译版本日期
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] pDate;

        //设置版本
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] sVer;

        //设置版本日期
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] sDate;

        //装置类型
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] typename;

        //装置SN
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] sn2;

        //网络备用模式
        // [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public UInt16 backmode;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] backmode_q;

        //串口设置 32*4 波特率
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] comm_cfg_baudrate;

        //串口设置 32*4 串口数据位数
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] comm_cfg_databit;

        //串口设置 32*4 串口校验方式
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] comm_cfg_comParity;

        //串口停止位数 按位
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] comm_cfg_comStopBit;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        //串口外部驱动接口类型 32*2
        public byte[] comm_cfg_driverMode;

        //tcp失败时是否丢弃数据
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] tcp_cfg_ldischrwhennotcp;

        //tcp是否断线重连
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] tcp_cfg_ReLinkTcpCtrl;

        //tcp 网络空闲断开 32*2
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] tcp_cfg_NetIdleTTL;

        //tcp 串口空闲断开 32*2
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] tcp_cfg_ComIdleTTL;

        //网络数据模式
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] net_cfg_IsFrameMode;

        //网络延迟
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] net_cfg_Delay;

        //tcp 按位给出本串口对应的socket是否启用主动申请TCP链接功能
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] net_cfg_AutoTcpCnntA;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] net_cfg_AutoTcpCnntB;

        //网口a对应的32路ip
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] net_cfg_Remoteipa;

        //网口b对应的32路ip
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] net_cfg_Remoteipb;

        //网口a对应的32路端口
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] net_cfg_parta;

        //网口b对应的32路端口
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] net_cfg_partb;

        // 访问IP限制功能模式
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] net_cfg_LimitIPModeA;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] net_cfg_LimitIPModeB;

        // 访问IP限制功能模式
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] net_cfg_utLimitIPa;

        // 访问IP限制功能模式
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] net_cfg_utLimitIPb;

        // 访问IP限制功能模式
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] net_cfg_utLimitMaska;

        // 访问IP限制功能模式
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] net_cfg_utLimitMaskb;

        //netipa
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] net_cfg_netipa;

        //netipb
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] net_cfg_netipb;

        //netmaska
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] net_cfg_netmaska;

        //netmaskb
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] net_cfg_netmaskb;

        //netGatewaya
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] net_cfg_netGatewaya;

        //netGatewayb
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] net_cfg_netGatewayb;

        //对应的通信端口号
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] net1BasePort;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] net2BasePort;

        //是否指定MAC
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] IsMacAsignedA;

        //是否指定MAC
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] IsMacAsignedB;

        //指定MAC
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] MacAsignedA;

        //指定MAC
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] MacAsignedB;

        //协议
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] IsUdpProtocolA;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] IsUdpProtocolB;

        //TCP协议下的工作模式
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] TcpModeA;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] TcpModeB;

        //udp组
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] netA_udp;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] netB_udp;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] netA_udp_port;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] netB_udp_port;

        //对于同一组接收主机IP, 装置的所有串口的信息发送到同一个（即基础）端口号（TRUE-是, FALSE-否（默认）
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] UdpComPortAllSameA;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] UdpComPortAllSameB;

        //装置名称: 同时作为装置名称和DNS用户访问名称（默认: APEXTECH）
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] DeviceName;

        // 维护口令: 字符串, 为0结束（默认: 12345678）
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] DevicePass;

        // APEX有效设置标识2
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] ApexSetFlag;

        //????
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] WWWW;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] xor;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] end;
    }
}