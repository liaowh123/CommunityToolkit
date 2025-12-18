using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.Windows;
using CommunityToolkit示例;
namespace CommunityToolkit示例
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AppSettings Settings { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();           
            Settings = config.GetSection("AppSettings").Get<AppSettings>();
        }
    }

}
