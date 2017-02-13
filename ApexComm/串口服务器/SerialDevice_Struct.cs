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

    /// <summary>
    /// 共932 ==22+ 904(正文)+6
    /// </summary>
    public struct SerialDevice_Monitor_Struct
    {
        //EB 90 EB 90
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] head;

        // 16 文本
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] sn;

        //'   7. 上送装置工作状态（上行）
        //'      20   A6H       报文命令码
        //'      21   A6H
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] cmdcode;

        //'      22   88H       报文正文长度: 8 + 64 * 2 + 128 * 2 + 128 * 4 = 904 = 0x388
        //'      23   03H
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] len;

        //'      24   FLAG1-L   异常状态（参考: 装置异常工作状态定义）
        //'      25   FLAG1-H   异常状态（参考: 装置异常工作状态定义）
        //'      26   FLAG2-L   异常状态（参考: 装置异常工作状态定义）, 备用
        //'      27   FLAG2-H   异常状态（参考: 装置异常工作状态定义）, 备用
        //stPowerPortInforInLib(iCurrCmdPowerPortIndex).bStatusFlag(i) = bRecvFrameBuffer(24 + i) ' 装置异常标志信息
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] StatusFlag;

        //'      28   ComCount  串口数n
        //'      29   0
        public UInt16 ComCount;

        //'      30   NetCount  网口数
        //'      31   0
        public UInt16 NetCount;

        //'
        //'      基本字节偏移: 16, 变量: COM * 2, 字节数共: 64
        //'      ========================================
        //'      32   SckA-01L  COM1的TCP链接状态L（A网口）  该部分共: ComCount×2字节
        //'      33   SckA-01H  COM1的TCP链接状态H（A网口）
        //'      34   ...
        //'      35   ...
        //'
        //'      基本字节偏移: 16 + 32*2 = 80, 变量: COM * 2, 字节数共: 64
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] Net0TCPStatus;

        //'      ====================================================
        //'      96   SckB-01L  COM1的TCP链接状态L（B网口）  若为双网口, 该部分共: ComCount * 2字节; 单网口则不存在
        //'      97   SckB-01H  COM1的TCP链接状态H（B网口）
        //'      98   ...
        //'      99   ...
        //'
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] Net1TCPStatus;

        //'      基本字节偏移: 80 + 32*2 = 144, 变量: COM * 4, 字节数共: 128
        //'      ======================================================
        //'      160   Com-01LL  COM1实际接收累计字节数（4字节）  该部分共: ComCount×4字节
        //'      161   Com-01L
        //'      162   Com-01H
        //'      163   Com-01HH
        //'      164   ...
        //'      165   ...
        //'      166   ...
        //'      167   ...
        //'
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] COMTotalReceive;

        //'      基本字节偏移: 144 + 32*4 = 272, 变量: COM * 4, 字节数共: 128
        //'      =======================================================
        //'      288   Com-01LL  COM1实际发送累计字节数（4字节）  该部分共: ComCount×4字节
        //'      289   Com-01L
        //'      290   Com-01H
        //'      291   Com-01HH
        //'      292   ...
        //'      293   ...
        //'      294   ...
        //'      295   ...
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] COMTotalSend;

        //'      基本字节偏移: 272 + 32*4 = 400, 变量: COM * 4, 字节数共: 128
        //'      ========================================================
        //'      416   SckA-01LL 网口A实际接收的累计字节数（4字节）  该部分共: ComCount * 4字节
        //'      417   SckA-01L
        //'      418   SckA-01H
        //'      419   SckA-01HH
        //'      420   ...
        //'      421   ...
        //'      422   ...
        //'      423   ...
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] Net0TotalReceive;

        //'      基本字节偏移: 400 + 32*4 = 528, 变量: COM * 4, 字节数共: 128
        //'      ========================================================
        //'      544   SckB-01LL 网口B实际接收的累计字节数（4字节）  该部分共: ComCount * 4字节
        //'      545   SckB-01L
        //'      546   SckB-01H
        //'      547   SckB-01HH
        //'      548   ...
        //'      549   ...
        //'      550   ...
        //'      551   ...
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] Net1TotalReceive;

        //'      基本字节偏移: 528 + 32*4 = 656, 变量: COM * 4, 字节数共: 128
        //'      ========================================================
        //'      672   SckA-01LL 网口A实际发送的累计字节数（4字节）  该部分共: ComCount * 4字节
        //'      673   SckA-01L
        //'      674   SckA-01H
        //'      675   SckA-01HH
        //'      676   ...
        //'      677   ...
        //'      678   ...
        //'      679   ...
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] Net0TotalSend;

        //'      基本字节偏移: 656 + 32*4 = 784, 变量: COM * 4, 字节数共: 128
        //'      ========================================================
        //'      800   SckB-01LL 网口B实际发送的累计字节数（4字节）  该部分共: ComCount * 4字节
        //'      801   SckB-01L
        //'      802   SckB-01H
        //'      803   SckB-01HH
        //'      804   ...
        //'      805   ...
        //'      806   ...
        //'      807   ...
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] Net1TotalSend;

        //'      ------------------------------
        //'      累计字节偏移: 784 + 32 * 4 = 912
        //'      ------------------------------

        //'      22   88H       报文正文长度: 8 + 64 * 2 + 128 * 2 + 128 * 4 = 904 = 0x388
        //'      23   03H

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] xor;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] end;
    }
}