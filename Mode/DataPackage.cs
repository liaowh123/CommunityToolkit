using HslCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityToolkit示例.Mode
{
    public struct DataPackage
    {
        /// <summary>
        /// 需要接收数据的长度
        /// </summary>
        public int NeedLength { get; set; }

        /// <summary>
        /// 发送字节数组
        /// </summary>
        public OperateResult[] SendBuffer { get;set; }
        /// <summary>
        /// 接收字节数组
        /// </summary>
        public OperateResult[] ReceiveBuffer { get;set; }
        /// <summary>
        /// 指令编号
        /// </summary>
        public long CommandNumber { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime { get; set; }  
        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime ReceiveTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime FinishTime { get; set; }
        /// <summary>
        /// 发送次数
        /// </summary>
        public int SendTimes { get;set; }
        
      
       
    }
}
