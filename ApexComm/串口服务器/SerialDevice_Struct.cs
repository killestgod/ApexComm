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

        //接收正文
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] cmdtype;//请求方式 eprom 还是映射

        //发送时正文开始位置 26
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

        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public UInt16 backmode_q;

        //串口设置 32*4 波特率
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] comm_cfg_baudrate;

        //串口设置 32*1 串口数据位数
        //按字节给出(COM1在[0]的高字节)串口数据位数: 串口数据位数: 5、6、7或8（默认）
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] comm_cfg_databit;

        //串口设置 32*1 串口校验方式
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] comm_cfg_comParity;

        //串口停止位数 按位
        //按位给出串口停止位数: 0-1位（默认）, 1-2位
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
        public byte[] Net0_cfg_LimitIPMode;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Net1_cfg_LimitIPMode;

        // 访问IP限制功能模式 6*4
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] Net0_cfg_utLimitIP;

        // 访问IP限制功能模式
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] Net1_cfg_utLimitIP;

        // 访问IP限制功能模式
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] Net0_cfg_utLimitMask;

        // 访问IP限制功能模式
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] Net1_cfg_utLimitMask;

        //netipa
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Net0_cfg_netip;

        //netipb
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Net1_cfg_netip;

        //netmaska
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Net0_cfg_netmask;

        //netmaskb
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Net1_cfg_netmask;

        //netGatewaya
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Net0_cfg_netGateway;

        //netGatewayb
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Net1_cfg_netGateway;

        //对应的通信端口号
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Net0BasePort;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Net1BasePort;

        //是否指定MAC
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Net0IsMacAsigned;

        //是否指定MAC
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Net1IsMacAsigned;

        //指定MAC
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] Net0MacAsigned;

        //指定MAC
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] Net1MacAsigned;

        //协议
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Net0IsUdpProtocol;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Net1IsUdpProtocol;

        //TCP协议下的工作模式
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Net0TcpMode;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Net1TcpMode;

        //udp组
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] Net0_udp;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] Net1_udp;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] Net0_udp_port;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] Net1_udp_port;

        //对于同一组接收主机IP, 装置的所有串口的信息发送到同一个（即基础）端口号（TRUE-是, FALSE-否（默认）
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Net0UdpComPortAllSame;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Net1UdpComPortAllSame;

        //装置名称: 同时作为装置名称和DNS用户访问名称（默认: APEXTECH）
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] DeviceName;

        // 维护口令: 字符串, 为0结束（默认: 12345678）
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] DevicePass;

        // APEX有效设置标识2
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] ApexSetFlag;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        //xor对齐
        public byte[] xor_cfg_q;

        //结构的xor
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] xor_cfg;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] xor;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] end;
    }
}