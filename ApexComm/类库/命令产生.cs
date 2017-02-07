using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyHelper;

namespace ApexComm
{
    #region 命令结构体

    public class CMDSR
    {
        /// <summary>
        /// 发送命令
        /// </summary>
        public byte[] CMD_Send;

        /// <summary>
        /// 回复命令
        /// </summary>
        public byte[] CMD_Receive;

        public CMDSR(byte[] send, byte[] receive)
        {
            CMD_Send = send;
            CMD_Receive = receive;
        }
    }

    #endregion 命令结构体

    /// <summary>
    /// 用于快速生成命令字节数组
    /// </summary>
    public static class CMDFactory
    {
        #region 功能函数

        /// <summary>
        ///
        /// 空包包长28
        /// 0-23 固定格式
        /// 正文
        /// 2 校验
        /// 2 结束
        /// </summary>
        /// <param name="cmdcode"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static byte[] MakeCMDBytes(byte[] cmdcode, byte[] body, byte[] sn = null)
        {
            List<byte> cmd = new List<byte>();
            //同步字符
            cmd.AddRange(new byte[] { 0xEB, 0x90, 0xEB, 0x90 });
            if (sn == null)
            {
                //id 标识
                for (int i = 0; i < 16; i++)
                {
                    cmd.Add(0xFF);
                }
            }
            else
            {
                cmd.AddRange(sn);
            }

            //报文命令码
            cmd.AddRange(cmdcode);
            //报文正文长度  LEN-L LEN-H
            cmd.AddRange(BytesHelper.intToBytes(body.Length, 2));
            //正文
            cmd.AddRange(body);
            //从 报文命令码  开始的包括所有正文内容的字方式XOR和   XOR L XOR H
            cmd.AddRange(Xor(cmd, 4, cmd.Count - 4));
            //结束标志
            cmd.AddRange(new byte[] { 0x03, 0x03 });
            return cmd.ToArray();
        }

        /// <summary>
        /// 用于数据包校验的异或操作
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static byte[] Xor(IEnumerable<byte> cmd, int offset, int length)
        {
            byte xorl = 0;
            byte xorh = 0;
            for (int i = offset; i < offset + length; i++)
            {
                if (i % 2 == 0)
                {
                    xorl = (byte)(xorl ^ cmd.ElementAt(i));
                }
                else
                {
                    xorh = (byte)(xorh ^ cmd.ElementAt(i));
                }
            }
            return new byte[] { xorl, xorh };
        }

        #region 字符串处理

        /// <summary>
        /// 字符串处理  交换高位和地位
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public static string ConvertbyteToStr(this byte[] bytes, int offset = 0, int length = -1)
        {
            if (length == -1)
                length = bytes.Length;
            byte[] str = new byte[length];
            for (int i = 0; i < length; i = i + 2)
            {
                str[i + 1] = bytes[i + offset];
                str[i] = bytes[i + 1 + offset];
            }

            return Encoding.GetEncoding("GBK").GetString(str).Trim('\0');
        }

        /// <summary>
        /// 字符串处理  交换高位和地位
        /// 只支持偶数个
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public static byte[] ConvertStrTobytes(this string str, int maxlength = -1)
        {
            byte[] tempbytes = Encoding.GetEncoding("GBK").GetBytes(str);
            if (maxlength == -1)
            {
                maxlength = tempbytes.Length;
            }
            byte[] bytes = new byte[maxlength];
            for (int i = 0; i < tempbytes.Length; i++)
            {
                if (i % 2 == 0)
                {
                    bytes[i + 1] = tempbytes[i];
                }
                else
                {
                    bytes[i - 1] = tempbytes[i];
                    if (i >= maxlength)
                    {
                        break;
                    }
                }
            }

            return bytes;
        }

        public static int ConvertbyteToInt(this byte[] bytes, int offset = 0)
        {
            byte[] temp = bytes.CloneRange(0, 4);
            List<byte> ll = new List<byte>();
            // 6 7 4 5
            ll.Add(temp[2]);
            ll.Add(temp[3]);
            ll.Add(temp[0]);
            ll.Add(temp[1]);
            return BytesHelper.bytesToInt(ll.ToArray(), 0);
        }

        public static byte[] ConvertIntToByte(this int num)
        {
            byte[] temp = BytesHelper.intToBytes(num);
            List<byte> ll = new List<byte>();
            // 6 7 4 5
            ll.Add(temp[2]);
            ll.Add(temp[3]);
            ll.Add(temp[0]);
            ll.Add(temp[1]);
            return ll.ToArray();
        }

        /// <summary>
        /// 4字节交换操作
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static byte[] ConvertSwapBytes(byte[] bytes, int offset = 0)
        {
            byte[] temp = bytes.CloneRange(0, 4);
            List<byte> ll = new List<byte>();
            // 6 7 4 5
            ll.Add(temp[2]);
            ll.Add(temp[3]);
            ll.Add(temp[0]);
            ll.Add(temp[1]);
            return ll.ToArray();
        }

        #endregion 字符串处理

        #endregion 功能函数

        /// <summary>
        /// 检查数据包是否完整
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool CheckReceiveMsg(byte[] msg)
        {
            if (msg.Length < 28)
            {
                LogMsg.log_error("收到的包长度过小");
                return false;
            }
            else if (msg[0] != 0xeb || msg[1] != 0x90 || msg[2] != 0xeb || msg[3] != 0x90)
            {
                LogMsg.log_error("收到的包头部标识错误");
                return false;
            }
            else if (msg[msg.Length - 2] != 0x03 || (msg[msg.Length - 1] != 0x03 && msg[msg.Length - 1] != 0x00))
            {
                LogMsg.log_error("收到的包尾部标识错误");
                return false;
            }
            else if (BytesHelper.bytesToInt(msg, 22, 2) != msg.Length - 28)
            {
                LogMsg.log_error("收到的包正文长度不足");
                return false;
            }
            else if (!BytesHelper.Equalbybyte(CMDFactory.Xor(msg, 4, msg.Length - 8), msg.CloneRange(msg.Length - 4, 2)))
            {
                LogMsg.log_error("收到的包XOR校验错误");
                return false;
            }
            else
            {
                // bodyLength = BytesHelper.bytesToInt(msg, 22, 2);//正文长度
                return true;
            }
        }

        /// <summary>
        /// 获取正文内容
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] GetBody(byte[] msg)
        {
            return msg.CloneRange(24, BytesHelper.bytesToInt(msg, 22, 2));
        }

        public static string ByteToDateStr(this byte[] bytes, int offset = 0)
        {
            if (bytes.Length - offset != 6)
                return "";
            else
            {
                for (int i = 0; i < 6; i++)
                {
                }
            }
            return "";
        }

        public static byte[] Swap01To10(this byte[] bytes)
        {
            byte[] tempbyte = new byte[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                int mod = i % 2 == 0 ? 1 : -1;
                tempbyte[i + mod] = bytes[i];
            }
            return tempbyte;
        }
    }
}