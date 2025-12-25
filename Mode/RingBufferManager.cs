using HslCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityToolkit示例.Mode
{
    /// <summary>
    /// 环形缓冲区管理器
    /// </summary>
    public class RingBufferManager
    {
        /// <summary>
        /// 存放指令数组
        /// </summary>
        public OperateResult[] Buffer { get; set; }

        /// <summary>
        /// 已写入数据的总长度
        /// </summary>
        public int DataCount { get; set; } = 0;

        /// <summary>
        /// 数据起始索引
        /// </summary>
        public int DataStart { get; set; } = 0;

        /// <summary>
        /// 数据结束索引
        /// </summary>
        public int DataEnd { get; set; } = 0;

        public RingBufferManager(int size)
        {
            Buffer = new OperateResult[size];
        }
        public void WriteBuffer(OperateResult[] buffer)
        {
            WriteBuffer(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 写入数据到缓冲区
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void WriteBuffer(OperateResult<short[]>[] buffer, int offset, int count)
        {
            int reserveCount = Buffer.Length - DataCount;//计算可用空间
            if (reserveCount >= count)
            {
                if (DataEnd + count < Buffer.Length)
                {
                    //数据没到结尾
                    Array.Copy(buffer, offset, Buffer, DataEnd, count);
                    DataEnd += count;
                    DataCount += count;
                }
                else
                {
                    //数据超过结尾
                    int overflowIndexLength = (DataEnd + count) - Buffer.Length;//计算超过索引的长度
                    int endPushIndexLength = count - overflowIndexLength;//填充在末尾的数据长度
                    Array.Copy(buffer, offset, Buffer, DataEnd, endPushIndexLength);
                    DataEnd = 0;
                    offset += endPushIndexLength;
                    DataCount += endPushIndexLength;
                    if (overflowIndexLength != 0)
                    {
                        Array.Copy(buffer, offset, Buffer, DataEnd, overflowIndexLength);
                    }
                    DataEnd += overflowIndexLength;
                    DataCount += overflowIndexLength;
                }
            }
            else
            {
                //缓存溢出
            }
        }

        public void WriteBuffer(OperateResult[] buffer, int offset, int count)
        {
            int reserveCount = Buffer.Length - DataCount;//计算可用空间
            if (reserveCount >= count)
            {
                if (DataEnd + count < Buffer.Length)
                {
                    //数据没到结尾
                    Array.Copy(buffer, offset, Buffer, DataEnd, count);
                    DataEnd += count;
                    DataCount += count;
                }
                else
                {
                    //数据超过结尾
                    int overflowIndexLength = (DataEnd + count) - Buffer.Length;//计算超过索引的长度
                    int endPushIndexLength = count - overflowIndexLength;//填充在末尾的数据长度
                    Array.Copy(buffer, offset, Buffer, DataEnd, endPushIndexLength);
                    DataEnd = 0;
                    offset += endPushIndexLength;
                    DataCount += endPushIndexLength;
                    if (overflowIndexLength != 0)
                    {
                        Array.Copy(buffer, offset, Buffer, DataEnd, overflowIndexLength);
                    }
                    DataEnd += overflowIndexLength;
                    DataCount += overflowIndexLength;
                }
            }
            else
            {
                //缓存溢出
            }
        }

        /// <summary>
        /// 从缓冲区读出数据
        /// </summary>
        /// <param name="targetBytes"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <exception cref="Exception"></exception>
        public void ReadBuffer(OperateResult[] targetBytes, int offset, int count)
        {
            if (count > DataCount)
            {
                throw new Exception("读取长度大于缓冲区数据长度");
            }

            if (DataStart + count < Buffer.Length)
            {
                Array.Copy(Buffer, DataStart, targetBytes, offset, count);
            }
            else
            {
                int overflowIndexLength = (DataStart + count) - Buffer.Length;//超出索引长度
                int endPushIndexLength = count - overflowIndexLength;
                Array.Copy(Buffer, DataStart, targetBytes, offset, endPushIndexLength);
                offset += endPushIndexLength;
                if (overflowIndexLength != 0)
                {
                    Array.Copy(Buffer, 0, targetBytes, offset, overflowIndexLength);
                }
            }
        }
        //实例化时使用范例
        //// 假设 manager 是您的 RingBufferManager 实例
        //OperateResult[] readData = new OperateResult[10];
        //manager.ReadBuffer(readData, 0, 10);

        //foreach (var result in readData)
        //{
        //    if (result == null) continue;

        //    // 检查 result 是否为 OperateResult<short[]> 类型
        //    if (result is OperateResult<short[]> shortArrayResult)
        //    {
        //        // 类型转换成功，可以安全地访问 Content 属性
        //        short[] content = shortArrayResult.Content;
        //        Console.WriteLine($"读取到 short[] 数据，长度为: {content.Length}");
        //    }
        //    else
        //    {
        //        // 这是一个普通的 OperateResult 对象
        //        Console.WriteLine($"读取到 OperateResult，成功状态: {result.IsSuccess}");
        //    }
        //}

        /// <summary>
        /// 清缓冲区数据并移动起始索引
        /// </summary>
        /// <param name="count"></param>
        public void Clear(int count)
        {
            if (count >= DataCount)
            {
                DataCount = 0;
                DataStart = 0;
                DataEnd = 0;
            }
            else
            {
                if (DataStart + count >= Buffer.Length)
                {
                    DataStart = (DataStart + count) - Buffer.Length;
                }
                else
                {
                    DataStart += count;
                }

                DataCount -= count;
            }
        }

        public void Clear()
        {
            DataCount = 0;
        }

        /// <summary>
        /// 获取当前缓冲区数据长度
        /// </summary>
        /// <returns></returns>
        public int GetDataCount() => DataCount;

        /// <summary>
        /// 获取当前缓冲区剩余空间
        /// </summary>
        /// <returns></returns>
        public int GetReserverCount() => Buffer.Length - DataCount;

        /// <summary>
        /// 获取指定位置的字节
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public OperateResult this[int index]
        {
            get
            {
                if (index >= DataCount)
                {
                    throw new Exception("读取元素的下标超过数据索引");
                }

                if (DataStart + index < Buffer.Length)
                {
                    return Buffer[DataStart + index];
                }
                else
                {
                    return Buffer[DataStart + index - Buffer.Length];
                }
            }
        }
    }
}
