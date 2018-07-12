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
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            else {
                var source = from m in AutoFills[rule.SelectedItem.ToString()].Rule.modes select m.Key;
                if (source != null && source.Count() > 0)
                {
                    mode.ItemsSource = source;
                    mode.SelectedIndex = 0;
                }
                else { MessageBox.Show(rule.SelectedItem.ToString() + "导入失败..." + Environment.NewLine + "请检查该配置文件内容是否正确无误。", "配置文件导入失败"); }
            }
        }

        private void mode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mode.SelectedItem != null && rule.SelectedItem.ToString() != "无") discribe.Text = AutoFills[rule.SelectedItem.ToString()].Rule.modes[mode.SelectedItem.ToString()].description;
            else discribe.Text = "无";
        }

        private void Done_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.autoFillRule = rule.SelectedItem.ToString();
            if (rule.SelectedItem != null && rule.SelectedItem.ToString() != "无") { MainWindow.preTimeLine.AutoFill(AutoFills[rule.SelectedItem.ToString()], mode.SelectedItem.ToString()); MainWindow.autoFillMode = mode.SelectedItem.ToString(); }//Initial AutoFill 
            else MainWindow.autoFillMode = "";
        }
    }
}
