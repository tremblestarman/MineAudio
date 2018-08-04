using System;
using System.Collections.Generic;
using Microsoft.Win32;
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
    /// Extension.xaml 的交互逻辑
    /// </summary>
    public partial class Extension : MetroWindow
    {
        List<ExtensionFile> preExtensions = new List<ExtensionFile>();
        public Extension()
        {
            InitializeComponent();
        }
        private void MetroWindow_Initialized(object sender, EventArgs e)
        {
            preExtensions = MainWindow.ExtensionFiles;
            ListView.ItemsSource = GetDescribe(MainWindow.ExtensionFiles);
        }

        private void Done_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.ExtensionFiles = preExtensions;
            this.Close();
        }
        private void Add_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "A2M Extended Content|*.amextension";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == true && fileDialog.FileName != null && fileDialog.FileName != "")
            {
                preExtensions.Add(new ExtensionFile() { Path = fileDialog.FileName });
                ListView.ItemsSource = GetDescribe(preExtensions);
            }
        }

        private List<ExetensionDescription> GetDescribe(List<ExtensionFile> ExtensionFiles)
        {
            var o = new List<ExetensionDescription>();
            foreach (var e in ExtensionFiles) o.Add(new ExetensionDescription() { Name = e.Name, IsMissing = e.IsMissing });
            return o;
        }

        public string filename = null;
        private void ListView_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (ListView.SelectedIndex != -1)
            {
                var n = new ExtensionOption(); n.Owner = this;
                n.prePath = preExtensions[ListView.SelectedIndex].Path;
                n.setPath();
                n.ShowDialog();
                if (filename == "") preExtensions.RemoveAt(ListView.SelectedIndex);
                else if (filename != null) preExtensions[ListView.SelectedIndex].Path = filename;
            }
            ListView.ItemsSource = GetDescribe(preExtensions);
            filename = null;
            ListView.SelectedIndex = -1;
        }
    }

    public class ExetensionDescription
    {
        private string _name;
        public string Name { get { return _name; } set { _name = value; } }
        private string _isMissing;
        public string IsMissing { get { return _isMissing; } set { _isMissing = value; } }
    }
}
