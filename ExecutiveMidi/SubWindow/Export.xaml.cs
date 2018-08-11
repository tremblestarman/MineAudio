using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Audio2Minecraft;

namespace ExecutiveMidi.SubWindow
{
    /// <summary>
    /// Export.xaml 的交互逻辑
    /// </summary>
    public partial class Export : MetroWindow 
    {
        static bool ButtonClosed = true;
        public Export()
        {
            InitializeComponent();
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            延伸方向.Items.Add("东（x+）");
            延伸方向.Items.Add("西（x-）");
            延伸方向.Items.Add("南（z+）");
            延伸方向.Items.Add("北（z-）");
            延伸方向.SelectedIndex = 0;
            序列宽度.Text = "16";
            重设BPM.Text = MainWindow.BPM.ToString();
            保持区块加载.IsChecked = true;
            保持区块加载.IsChecked = false;
            Midi刻长.Text = MainWindow.preTimeLine.Param["TotalTicks"].Value.ToString() + " ticks";
            var m = MainWindow.preTimeLine.Param["TotalTicks"].Value / 1200;
            var s = MainWindow.preTimeLine.Param["TotalTicks"].Value % 1200 / 20;
            Midi时长.Text = m.ToString() + " : " + s.ToString();
            Done.IsEnabled = true;
        }

        string old_text = "";
        private void NumericOnly(object sender, TextChangedEventArgs e)
        {
            var t = e.OriginalSource as TextBox;
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("\\D");
            if (reg.IsMatch(t.Text))
                t.Text = old_text;
            else if (t.Text == "" || Int32.Parse(t.Text) < 1)
                t.Text = "1";
            else old_text = t.Text;
        }
        private void _KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D0 || e.Key == Key.D1 || e.Key == Key.D2 || e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5 || e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9 || e.Key == Key.NumPad0 || e.Key == Key.NumPad1 || e.Key == Key.NumPad2 || e.Key == Key.NumPad3 || e.Key == Key.NumPad4 || e.Key == Key.NumPad5 || e.Key == Key.NumPad6 || e.Key == Key.NumPad7 || e.Key == Key.NumPad8 || e.Key == Key.NumPad9)
                Done.IsEnabled = true;
        }
        private void _Click(object sender, RoutedEventArgs e)
        {
            Done.IsEnabled = true;
        }
        private void _SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Done.IsEnabled = true;
        }

        private void OK(object sender, MouseButtonEventArgs e)
        {
            if (MainWindow.Midipath != "" && MainWindow.BPM.ToString() != 重设BPM.Text)
            {
                var m_ = MainWindow.Midipath; var b = Int32.Parse(重设BPM.Text);
                var a = new TimeLine();
                var g = new SubWindow.Waiting(); g.Owner = this;
                BackgroundWorker waiting = new BackgroundWorker();
                waiting.DoWork += (ee, ea) => { };
                waiting.RunWorkerCompleted += (ee, ea) =>
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        g.ShowDialog();
                    }));
                };
                waiting.RunWorkerAsync();
                //Work
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += (ee, ea) =>
                {
                    a = new AudioStreamMidi().Serialize(m_, new TimeLine(), b);
                };
                worker.RunWorkerCompleted += (ee, ea) =>
                {
                    g.Close();

                    Midi刻长.Text = a.Param["TotalTicks"].Value.ToString() + " ticks";
                    var m = a.Param["TotalTicks"].Value / 1200;
                    var s = a.Param["TotalTicks"].Value % 1200 / 20;
                    Midi时长.Text = m.ToString() + " : " + s.ToString();
                    MainWindow.BPM = Int32.Parse(重设BPM.Text);
                };
                worker.RunWorkerAsync();
            }
        }
        private void DoneChanges(object sender, MouseButtonEventArgs e)
        {
            MainWindow.export = new Audio2Minecraft.ExportSetting() { AlwaysActive = 保持区块加载.IsChecked == true, AlwaysLoadEntities = false, AutoTeleport = false, Direction = 延伸方向.SelectedIndex, Width = Int32.Parse(序列宽度.Text) };
            ButtonClosed = false;
            this.Close();
        }
        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            if (ButtonClosed == true)
                MainWindow.export_cancel = true;
        }
    }
}
