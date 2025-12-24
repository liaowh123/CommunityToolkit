using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommunityToolkit示例.ViewModel
{
    internal partial class ConfigViewModel :ObservableObject
    {
        public ConfigViewModel()
        {
            // 访问配置
            var communicationProtocol = App.Settings.CommunicationProtocol;
            var iPAddress = App.Settings.IPAddress;           
            var portNumber = App.Settings.PortNumber;
        }

        [RelayCommand]
        private void SaveConfig()
        {
            int a= 1;//待定
            int b= 2;
            int c = 3;
        }
    }

  
}
