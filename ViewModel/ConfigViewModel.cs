using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace CommunityToolkit示例.ViewModel
{
    public partial class ConfigViewModel :ObservableObject
    {
        public ObservableCollection<string> AvailableSerialPorts { get; } = new ObservableCollection<string>();

        [ObservableProperty]
        private string? _selectedSerialPort;
        //public string? SelectedSerialPort
        //{
        //    get => _selectedSerialPort;
        //    set => SetProperty(ref _selectedSerialPort, value);
        //}

        //public ICommand RefreshSerialPortsCommand { get; }

        public ConfigViewModel()
        {
            //RefreshSerialPortsCommand = new RelayCommand(RefreshSerialPorts);
            RefreshSerialPorts();
            
        }


        [RelayCommand]
        private void RefreshSerialPorts()
        {
            try
            {
                var names = SerialPort.GetPortNames();
                AvailableSerialPorts.Clear();
                foreach (var n in names)
                {
                    AvailableSerialPorts.Add(n);
                }

                // 如果当前选择的端口不在列表中，则清空或设置为第一个
                if (!string.IsNullOrEmpty(SelectedSerialPort) && !AvailableSerialPorts.Contains(SelectedSerialPort))
                {
                    SelectedSerialPort = AvailableSerialPorts.Count > 0 ? AvailableSerialPorts[0] : null;
                }
                else if (string.IsNullOrEmpty(SelectedSerialPort) && AvailableSerialPorts.Count > 0)
                {
                    SelectedSerialPort = AvailableSerialPorts[0];
                }
            }
            catch (Exception)
            {
                // 忽略或记录异常（在真实项目可注入日志）
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
        #endregion
    }

     


}
