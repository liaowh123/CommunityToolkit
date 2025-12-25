using CommunityToolkit.Mvvm.ComponentModel;
using HslCommunication;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Profinet.Omron;
using HslCommunication.Profinet.Omron.Helper;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace CommunityToolkit示例.DELL
{
    public  class OmromServer 
    {
       
        HslCommunication.Profinet.Omron.OmronFinsNet Plc = new HslCommunication.Profinet.Omron.OmronFinsNet();

        

        private bool ConnectToPlc(string ipAddress, int port)
        {

            Plc.PlcType = OmronPlcType.CSCJ;
            Plc.DA2 = 0;
            Plc.ReceiveUntilEmpty = false;
            Plc.ByteTransform.DataFormat = HslCommunication.Core.DataFormat.CDAB;
            Plc.ByteTransform.IsStringReverseByteWord = true;

            Plc.CommunicationPipe = new HslCommunication.Core.Pipe.PipeTcpNet(ipAddress, port)
            {
                ConnectTimeOut = 5000,    // 连接超时时间，单位毫秒
                ReceiveTimeOut = 10000,    // 接收设备数据反馈的超时时间
            };
            // 连接
            OperateResult connectResult = Plc.ConnectServer();
            if (connectResult.IsSuccess)
            {
                //MessageBox.Show("连接PLC成功!");
                return true;
            }
            else
            {
                //MessageBox.Show("连接PLC失败!");
                return false;
            }
        }

        private bool DisconnectPlc()
        {
            // 停止监控逻辑
            OperateResult connectResult = Plc.ConnectClose();
            if (connectResult.IsSuccess)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private string ReadPLC(string storageAreaAddress, Type type, ushort areaLength)
        {
            if (Plc == null)
            {
                MessageBox.Show("PLC 未连接，请先连接 PLC。");
                return string.Empty;
            }
            string result;
            OperateResult? readResult = null;

            if (type == typeof(short) || type == typeof(Int16))
            {
                var res = Plc.ReadInt16(storageAreaAddress, areaLength);
                readResult = res;
                if (res.IsSuccess)
                {
                    result = string.Join(", ", res.Content); // 修复：使用正确的泛型类型 }  
                    return result;
                }
                else
                {
                    result = string.Empty;
                    return result;
                }
            }
            else if (type == typeof(int) || type == typeof(Int32))
            {
                var res = Plc.ReadInt32(storageAreaAddress, areaLength);
                readResult = res;
                if (res.IsSuccess)
                {
                    result = string.Join(", ", res.Content); // 修复：使用正确的泛型类型 }  
                    return result;
                }
                else
                {
                    result = string.Empty;
                    return result;
                }
            }
            else if (type == typeof(float) || type == typeof(Single))
            {
                var res = Plc.ReadFloat(storageAreaAddress, areaLength);
                readResult = res;
                if (res.IsSuccess)
                {
                    result = string.Join(", ", res.Content); // 修复：使用正确的泛型类型 }  
                    return result;
                }
                else
                {
                    result = string.Empty;
                    return result;
                }
            }
            else if (type == typeof(string))
            {
                var res = Plc.ReadString(storageAreaAddress, areaLength);
                readResult = res;
                if (res.IsSuccess)
                {
                    result = string.Join(", ", res.Content); // 修复：使用正确的泛型类型 }  
                    return result;
                }
                else
                {
                    result = string.Empty;
                    return result;
                }
            }
            else if (readResult != null && !readResult.IsSuccess)
            {
                MessageBox.Show($"读取失败: {readResult.Message}");
                return string.Empty;
            }
            else
            {
                MessageBox.Show("暂不支持的类型读取。");
                return string.Empty;
            }
        }
        private bool WritePLC(string storageAreaAddress, string writeValue, Type type)
        {
            if (Plc == null)
            {
                MessageBox.Show("PLC 未连接，请先连接 PLC。");
                return false;
            }
            if (type == typeof(short) || type == typeof(Int16))
            {
                string[] values = writeValue?.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
                short[] intValues = Array.ConvertAll(values, short.Parse);
                OperateResult writeResult = Plc.Write(storageAreaAddress, intValues);
                if (writeResult.IsSuccess)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (type == typeof(float) || type == typeof(Single))
            {
                string[] values = writeValue?.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
                float[] floatValues = Array.ConvertAll(values, float.Parse);
                OperateResult writeResult = Plc.Write(storageAreaAddress, floatValues);
                if (writeResult.IsSuccess)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                MessageBox.Show("暂不支持的类型写入。");
                return false;
            }
            
        }
    }
}