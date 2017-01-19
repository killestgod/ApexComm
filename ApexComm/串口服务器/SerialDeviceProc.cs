using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApexComm.串口服务器
{
    public static class CMDCode
    {
        //APEXTECH 超级密码  ascii +40
        public static byte[] SuperPWD = new byte[] { 0x81, 0x90, 0x85, 0x98, 0x94, 0x85, 0x83, 0x88 };

        //CMD-L CMD-H
        //EB 90 EB 90 FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF 80 80 00 00 80 80 03 03
        //EB 90 EB 90 FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF 80 80 00 00 03 03
        public static CMDSR 搜索设备 = new CMDSR(new byte[] { 0x80, 0x80 }, new byte[] { 0xD0, 0xD0 });

        public static CMDSR 获取所有配置 = new CMDSR(new byte[] { 0x5A, 0x5A }, new byte[] { 0xAA, 0xAA });
        public static CMDSR 保存所有配置 = new CMDSR(new byte[] { 0x5B, 0x5B }, new byte[] { 0xAB, 0xAB });
    }

    public class SerialDeviceProc
    {
        /// <summary>
        /// 读取所有配置信息
        /// //  10. 查看所有设置命令报文（下行）
        //      20       5AH     报文命令码
        //      21       5AH
        //      22       0AH     报文正文长度
        //      23       0
        //      24～31   Pass    维护口令
        //      32       FLAG-L  是否从EEPROM中读取; 0xA5A5-是, 其他-直接上送RAM中的设置映射
        //      33       FLAG-H
        /// </summary>
        /// <returns></returns>
        public static byte[] ReadAllCfg(SerialDevice device)
        {
            List<byte> cmd = new List<byte>();
            //APEXTECH
            cmd.AddRange(CMDCode.SuperPWD);//维护密码
                                           //00 00 直接上送RAM中的设置映射
                                           //A5 A5 从EEPROM中读取
            cmd.AddRange(new byte[] { 0x00, 0x00 });
            // cmd.AddRange(new byte[] { 0xa5, 0xa5 });
            return CMDFactory.MakeCMDBytes(CMDCode.获取所有配置.CMD_Send, cmd.ToArray(), CMDFactory.ConvertStrTobytes(device.SN, 16));
        }

        /// <summary>
        /// 修改所有设置
        //      20       5BH    报文命令码
        //      21       5BH
        //      22       ??     报文正文长度, LOW（SetLen + 8）
        //      23       ??     HIGH（SetLen + 8）
        //      24～31   Pass   维护口令
        //      32～     Set    设置内容, 长度为SetLen

        /// </summary>
        /// <param name="cmdcode"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static byte[] SaveAllCfg()
        {
            List<byte> cmd = new List<byte>();
            //APEXTECH
            cmd.AddRange(CMDCode.SuperPWD);//维护密码
            //APEX有效设置标识1: 本部分长度: 2 + 2 = 4 (下一部分偏移: 4)

            return null;
        }
    }
}