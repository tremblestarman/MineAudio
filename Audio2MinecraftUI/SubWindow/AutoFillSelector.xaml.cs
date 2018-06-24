using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Audio2Minecraft;

namespace Audio2MinecraftUI.SubWindow
{
    /// <summary>
    /// AutoFillSelector.xaml 的交互逻辑
    /// </summary>
    public partial class AutoFillSelector : MetroWindow
    {
        public Dictionary<string, AutoFill> AutoFills;
        public AutoFillSelector()
        {
            InitializeComponent();
        }

        private void MetroWindow_Initialized(object sender, EventArgs e)
        {

        }

        private void rule_Selected(object sender, RoutedEventArgs e)
        {
            if (rule.SelectedItem.ToString() == "无") mode.ItemsSource = null;
            else { mode.ItemsSource = (from m in AutoFills[rule.SelectedItem.ToString()].Rule.modes select m.Key); mode.SelectedIndex = 0; }
        }

        private void mode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (rule.SelectedItem.ToString() != "无") discribe.Text = AutoFills[rule.SelectedItem.ToString()].Rule.modes[mode.SelectedItem.ToString()].description;
            else discribe.Text = "无";
        }

        private void Done_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.autoFillRule = rule.SelectedItem.ToString();
            if (rule.SelectedItem.ToString() != "无") { MainWindow.preTimeLine.AutoFill(AutoFills[rule.SelectedItem.ToString()], mode.SelectedItem.ToString()); MainWindow.autoFillMode = mode.SelectedItem.ToString(); }//Initial AutoFill 
            else MainWindow.autoFillMode = "";
        }
    }
}
