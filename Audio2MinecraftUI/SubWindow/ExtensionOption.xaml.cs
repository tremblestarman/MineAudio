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
using Microsoft.Win32;
using MahApps.Metro.Controls;

namespace Audio2MinecraftUI.SubWindow
{
    /// <summary>
    /// ExtensionOption.xaml 的交互逻辑
    /// </summary>
    public partial class ExtensionOption : MetroWindow
    {
        public string prePath = "";
        public void setPath() { Path.Text = prePath; }
        public ExtensionOption()
        {
            InitializeComponent();
        }

        private void Select(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "A2M Extended Content|*.amextension";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == true && fileDialog.FileName != null && fileDialog.FileName != "")
            {
                prePath = fileDialog.FileName;
                setPath();
            }
        }

        private void Cancel(object sender, MouseButtonEventArgs e)
        {
            prePath = "";
            setPath();
        }

        private void Done(object sender, MouseButtonEventArgs e)
        {
            var o = this.Owner as Extension;
            o.filename = prePath;
            this.Close();
        }
    }
}
