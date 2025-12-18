using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace CommunityToolkit示例.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {
        
        private object? currentView;
        public object? CurrentView
        {
            get => currentView;
            set => SetProperty(ref currentView, value);
        }

        [RelayCommand] //NavButtonCommand
        public void NavButtonCommand(object parameter)
        {
            try
            {
                if (parameter is Type viewType)
                {
                    var view = Activator.CreateInstance(viewType);
                    CurrentView = view;
                }
                else if (parameter is string viewName)
                {
                    var type = Type.GetType($"CommunityToolkit示例.View.{viewName}");
                    if (type != null)
                    {
                        var view = Activator.CreateInstance(type);
                        CurrentView = view;
                    }
                    else
                    {
                        MessageBox.Show($"未找到类型: CommunityToolkit示例.View.{viewName}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Navigation Error: " + ex.Message);
            }
        }

        [RelayCommand]
        private static void Minimize()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        [RelayCommand]
        private static void Maximize()
        {
            var window = Application.Current.MainWindow;
            window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        [RelayCommand]
        private static void CloseButton()
        {
            Application.Current.MainWindow.Close();
        }

        public MainWindowViewModel()
        {
            // 默认视图
            CurrentView = new View.MainPanelView();
        }
    }
}
