using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace Audio2MinecraftUI.SubWindow
{
    /// <summary>
    /// VersionAlarm.xaml 的交互逻辑
    /// </summary>
    public partial class VersionAlarm : MetroWindow
    {
        public string download_url = "";
        public VersionAlarm()
        {
            InitializeComponent();
        }

        private void download_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(download_url);
        }
    }
}