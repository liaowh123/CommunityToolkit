using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HslCommunication;
using HslCommunication.Core;
using HslCommunication.Profinet.Omron;
using HslCommunication.Profinet.Omron.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommunityToolkit示例.ViewModel
{
    public partial class TestViewModel : ObservableObject
    {
        #region //声明变量属性
        [ObservableProperty]
        private string? _plcStatus = "连接失败";

        private OmronFinsNet? plc; // 声明为字段

        [ObservableProperty]
        private bool _isIntType = true;// 是否为整型

        [ObservableProperty]
        private bool _isFloatType = false; // 是否为浮点型

        [ObservableProperty]
        private string _storageArea = "D"; // 存储区

        [ObservableProperty]
        private string _startAddress = "100"; // 起始地址

        public string StorageAreaAddress => $"{StorageArea}{StartAddress}";

        [ObservableProperty]
        private string _areaLength = "1"; // 读取长度
        

        [ObservableProperty]
        private string? readValue;

        [ObservableProperty]
        private string writeValue ="";
        #endregion  

        [RelayCommand]
        private void StartMonitor()
        {
            plc = new OmronFinsNet("192.168.250.1", 9600);
            plc.ByteTransform.DataFormat = HslCommunication.Core.DataFormat.CDAB; // 确保DataFormat枚举中实际存在CDAB值
            // 连接
            OperateResult connectResult = plc.ConnectServer();
            if (connectResult.IsSuccess)
            {
                //MessageBox.Show("连接PLC成功!");
                PlcStatus = "连接成功";
            }
            else
            {
                MessageBox.Show("连接PLC失败!");
                PlcStatus = "连接失败";
            }

        }

        [RelayCommand]
        private void StopMonitor()
        {
            //停止监控逻辑
            OperateResult connectResult = plc.ConnectClose();
            if (connectResult.IsSuccess)
            {
                MessageBox.Show("已断开连接!");
                PlcStatus = "连接失败";
            }
        }

        [RelayCommand]
        private void ReadPLC()
        {
            if (plc == null)
            {
                MessageBox.Show("PLC 未连接，请先连接 PLC。");
                return;
            }

            if (IsIntType)
            {
                ushort areaLength = ushort.Parse(AreaLength); // Use the generated property instead of the backing field
                OperateResult<short[]> readResult = plc.ReadInt16(StorageAreaAddress, areaLength);
                if (readResult.IsSuccess)
                {
                    ReadValue = string.Join(", ", readResult.Content);
                }
                else
                {
                    MessageBox.Show($"读取失败：{readResult.Message}");
                }
            }
            else if (IsFloatType)
            {
                ushort areaLength = ushort.Parse(AreaLength); // Use the generated property instead of the backing field
                OperateResult<float[]> readResult = plc.ReadFloat(StorageAreaAddress, areaLength);
                if (readResult.IsSuccess)
                {
                    ReadValue = string.Join(", ", readResult.Content);
                }
                else
                {
                    MessageBox.Show($"读取失败：{readResult.Message}");
                }
            }
        }

        [RelayCommand]
        private void WritePLC()
        {
            if (plc == null)
            {
                MessageBox.Show("PLC 未连接，请先连接 PLC。");
                return;
            }
                if (IsIntType)
                {
                    string[] values = WriteValue?.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
                    short[] intValues = Array.ConvertAll(values, short.Parse);
                    OperateResult writeResult = plc.Write(StorageAreaAddress, intValues);
                    if (writeResult.IsSuccess)
                    {
                        MessageBox.Show("写入成功！");
                    }
                    else
                    {
                        MessageBox.Show($"写入失败：{writeResult.Message}");
                    }
                }
                else if (IsFloatType)
                {
                    string[] values = WriteValue?.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
                    float[] floatValues = Array.ConvertAll(values, float.Parse);
                    OperateResult writeResult = plc.Write(StorageAreaAddress, floatValues);
                    if (writeResult.IsSuccess)
                    {
                        MessageBox.Show("写入成功！");
                    }
                    else
                    {
                        MessageBox.Show($"写入失败：{writeResult.Message}");
                    }
            }
        }
    }
}
