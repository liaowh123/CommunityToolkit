using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HslCommunication;
using HslCommunication.Core;
using HslCommunication.Profinet.Omron;

namespace CommunityToolkit示例.PLCBase
{
    /// <summary>
    /// 欧姆龙PLC通信服务类
    /// </summary>
    public class OmronFinsClient : IDisposable
    {
        private readonly OmronFinsNet _plcClient;
        private bool _isConnected = false;

        // 事件声明
        public event EventHandler<string> ConnectionStatusChanged;
        public event EventHandler<string> OperationLog;

        /// <summary>
        /// PLC连接状态
        /// </summary>
        public bool IsConnected => _isConnected;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ipAddress">PLC IP地址</param>
        /// <param name="port">端口号（默认9600）</param>
        /// <param name="dataFormat">字节顺序（默认CDAB）</param>
        public OmronFinsClient(string ipAddress, int port = 9600,
                               DataFormat dataFormat = DataFormat.CDAB)
        {
            _plcClient = new OmronFinsNet(ipAddress, port);
            _plcClient.ByteTransform.DataFormat = dataFormat;
            _plcClient.ConnectTimeOut = 5000; // 5秒超时
        }

        /// <summary>
        /// 连接PLC
        /// </summary>
        /// <returns>是否成功</returns>
        public async Task<bool> ConnectAsync()
        {
            try
            {
                OperateResult result = await Task.Run(() => _plcClient.ConnectServer());
                _isConnected = result.IsSuccess;

                OnConnectionStatusChanged(_isConnected ? "已连接" : $"连接失败: {result.Message}");
                LogOperation($"PLC连接: {(_isConnected ? "成功" : "失败")}");

                return _isConnected;
            }
            catch (Exception ex)
            {
                _isConnected = false;
                OnConnectionStatusChanged($"连接异常: {ex.Message}");
                LogOperation($"连接异常: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            try
            {
                _plcClient.ConnectClose();
                _isConnected = false;
                OnConnectionStatusChanged("已断开");
                LogOperation("PLC连接已断开");
            }
            catch (Exception ex)
            {
                LogOperation($"断开连接异常: {ex.Message}");
            }
        }

        #region 批量读取方法

        /// <summary>
        /// 读取连续的16位整数数组
        /// </summary>
        /// <param name="startAddress">起始地址（如 "D100"）</param>
        /// <param name="length">读取数量</param>
        /// <returns>整数数组</returns>
        public async Task<short[]> ReadInt16ArrayAsync(string startAddress, ushort length)
        {
            if (!_isConnected) throw new InvalidOperationException("PLC未连接");

            try
            {
                OperateResult<short[]> result = await Task.Run(() =>
                    _plcClient.ReadInt16(startAddress, length));

                if (result.IsSuccess)
                {
                    LogOperation($"读取成功: {startAddress}[{length}]");
                    return result.Content;
                }
                else
                {
                    LogOperation($"读取失败: {result.Message}");
                    throw new Exception($"读取失败: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                LogOperation($"读取异常: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 读取连续的32位浮点数数组
        /// </summary>
        /// <param name="startAddress">起始地址</param>
        /// <param name="length">读取数量</param>
        /// <returns>浮点数数组</returns>
        public async Task<float[]> ReadFloatArrayAsync(string startAddress, ushort length)
        {
            if (!_isConnected) throw new InvalidOperationException("PLC未连接");

            try
            {
                OperateResult<float[]> result = await Task.Run(() =>
                    _plcClient.ReadFloat(startAddress, length));

                if (result.IsSuccess)
                {
                    LogOperation($"读取成功: {startAddress}[{length}]");
                    return result.Content;
                }
                else
                {
                    LogOperation($"读取失败: {result.Message}");
                    throw new Exception($"读取失败: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                LogOperation($"读取异常: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 批量读取不同地址的数据
        /// </summary>
        /// <param name="addressMap">地址-类型映射字典</param>
        /// <returns>包含读取结果的字典</returns>
        public async Task<Dictionary<string, object>> BatchReadAsync(
            Dictionary<string, Type> addressMap)
        {
            var results = new Dictionary<string, object>();

            foreach (var kvp in addressMap)
            {
                try
                {
                    if (kvp.Value == typeof(short) || kvp.Value == typeof(short[]))
                    {
                        // 简化：这里假设都是读单个值，实际可根据需要扩展
                        var value = await ReadInt16ArrayAsync(kvp.Key, 1);
                        results[kvp.Key] = value[0];
                    }
                    else if (kvp.Value == typeof(float) || kvp.Value == typeof(float[]))
                    {
                        var value = await ReadFloatArrayAsync(kvp.Key, 1);
                        results[kvp.Key] = value[0];
                    }
                }
                catch (Exception ex)
                {
                    LogOperation($"批量读取失败 [{kvp.Key}]: {ex.Message}");
                    results[kvp.Key] = null;
                }
            }

            return results;
        }

        #endregion

        #region 批量写入方法

        /// <summary>
        /// 写入16位整数数组
        /// </summary>
        /// <param name="startAddress">起始地址</param>
        /// <param name="values">要写入的值</param>
        public async Task WriteInt16ArrayAsync(string startAddress, short[] values)
        {
            if (!_isConnected) throw new InvalidOperationException("PLC未连接");

            try
            {
                OperateResult result = await Task.Run(() =>
                    _plcClient.Write(startAddress, values));

                if (result.IsSuccess)
                {
                    LogOperation($"写入成功: {startAddress}[{values.Length}]");
                }
                else
                {
                    LogOperation($"写入失败: {result.Message}");
                    throw new Exception($"写入失败: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                LogOperation($"写入异常: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 写入浮点数数组
        /// </summary>
        /// <param name="startAddress">起始地址</param>
        /// <param name="values">要写入的值</param>
        public async Task WriteFloatArrayAsync(string startAddress, float[] values)
        {
            if (!_isConnected) throw new InvalidOperationException("PLC未连接");

            try
            {
                OperateResult result = await Task.Run(() =>
                    _plcClient.Write(startAddress, values));

                if (result.IsSuccess)
                {
                    LogOperation($"写入成功: {startAddress}[{values.Length}]");
                }
                else
                {
                    LogOperation($"写入失败: {result.Message}");
                    throw new Exception($"写入失败: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                LogOperation($"写入异常: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 批量写入数据
        /// </summary>
        /// <param name="writeOperations">写入操作列表（地址-值对）</param>
        public async Task BatchWriteAsync(Dictionary<string, object> writeOperations)
        {
            foreach (var kvp in writeOperations)
            {
                try
                {
                    if (kvp.Value is short[] shortArray)
                    {
                        await WriteInt16ArrayAsync(kvp.Key, shortArray);
                    }
                    else if (kvp.Value is float[] floatArray)
                    {
                        await WriteFloatArrayAsync(kvp.Key, floatArray);
                    }
                    else if (kvp.Value is short shortValue)
                    {
                        await WriteInt16ArrayAsync(kvp.Key, new[] { shortValue });
                    }
                    else if (kvp.Value is float floatValue)
                    {
                        await WriteFloatArrayAsync(kvp.Key, new[] { floatValue });
                    }
                }
                catch (Exception ex)
                {
                    LogOperation($"批量写入失败 [{kvp.Key}]: {ex.Message}");
                    // 可根据需要决定是否继续执行
                }
            }
        }

        #endregion

        #region 事件触发方法

        protected virtual void OnConnectionStatusChanged(string status)
        {
            ConnectionStatusChanged?.Invoke(this, status);
        }

        protected virtual void LogOperation(string message)
        {
            string logMessage = $"[{DateTime.Now:HH:mm:ss}] {message}";
            OperationLog?.Invoke(this, logMessage);
        }

        #endregion

        #region IDisposable实现

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Disconnect();
                    _plcClient?.Dispose();
                }
                _disposed = true;
            }
        }

        ~OmronFinsClient()
        {
            Dispose(false);
        }

        #endregion
    }
}
