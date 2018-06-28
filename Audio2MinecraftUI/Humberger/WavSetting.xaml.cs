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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Audio2Minecraft;

namespace Audio2MinecraftUI.Humberger
{
    /// <summary>
    /// WavSetting.xaml 的交互逻辑
    /// </summary>
    public partial class WavSetting : UserControl
    {
        private List<TextBlock> LFreT;
        private List<TextBlock> LVolT;
        private List<TextBlock> RFreT;
        private List<TextBlock> RVolT;
        public WavSetting()
        {
            InitializeComponent();
        }
        private void UserControl_Initialized(object sender, EventArgs e)
        {
            LFreT = new List<TextBlock>()
            {
                C1,
                C2,
                C3,
            };
            LVolT = new List<TextBlock>()
            {
                C4,
                C5,
                C6,
            };
            RFreT = new List<TextBlock>()
            {
                C7,
                C8,
                C9,
            };
            RVolT = new List<TextBlock>()
            {
                C10,
                C11,
                C12,
            };
            Update();
        }

        public void ItemChanged()
        {
            左声道.IsChecked = MainWindow.preTimeLine.LeftWaveSetting.Enable;
            当刻频率L.IsChecked = MainWindow.preTimeLine.LeftWaveSetting.Frequency;
            当刻振幅L.IsChecked = MainWindow.preTimeLine.LeftWaveSetting.Volume;
            右声道.IsChecked = MainWindow.preTimeLine.RightWaveSetting.Enable;
            当刻频率R.IsChecked = MainWindow.preTimeLine.RightWaveSetting.Frequency;
            当刻振幅R.IsChecked = MainWindow.preTimeLine.RightWaveSetting.Volume;
        }

        private void Elements_Click(object sender, RoutedEventArgs e)
        {
            Done.IsEnabled = true;
            Update();
        }
        private void DoneChanges(object sender, MouseButtonEventArgs e)
        {
            Done.IsEnabled = false;
            var fre_c = Int32.Parse(单刻频率采样数.Text);
            var vol_c = Int32.Parse(单刻振幅采样数.Text);
            var cir = Int32.Parse(采样周期.Text);

            if (!(_COMMIT[0] == cir && _COMMIT[1] == fre_c && _COMMIT[2] == vol_c))
            {
                var path = MainWindow.Wavepath;
                var wav = new AudioStreamWave().Serialize(MainWindow.Wavepath, new TimeLine(), fre_c, vol_c, cir);
                最大频率L.Text = Math.Round(highest(wav, "l", "FrequencyPerTick"), 2).ToString();
                最大振幅L.Text = Math.Round(highest(wav, "l", "VolumePerTick"), 2).ToString();
                最小频率L.Text = Math.Round(lowest(wav, "l", "FrequencyPerTick"), 2).ToString();
                最小振幅L.Text = Math.Round(lowest(wav, "l", "VolumePerTick"), 2).ToString();
                平均频率L.Text = Math.Round(average(wav, "l", "FrequencyPerTick"), 2).ToString();
                平均振幅L.Text = Math.Round(average(wav, "l", "VolumePerTick"), 2).ToString();
                最大频率R.Text = Math.Round(highest(wav, "r", "FrequencyPerTick"), 2).ToString();
                最大振幅R.Text = Math.Round(highest(wav, "r", "VolumePerTick"), 2).ToString();
                最小频率R.Text = Math.Round(lowest(wav, "r", "FrequencyPerTick"), 2).ToString();
                最小振幅R.Text = Math.Round(lowest(wav, "r", "VolumePerTick"), 2).ToString();
                平均频率R.Text = Math.Round(average(wav, "r", "FrequencyPerTick"), 2).ToString();
                平均振幅R.Text = Math.Round(average(wav, "r", "VolumePerTick"), 2).ToString();
            }
            _COMMIT[0] = cir;
            _COMMIT[1] = fre_c;
            _COMMIT[2] = vol_c;
            Update();
            MainWindow.preTimeLine.LeftWaveSetting = new TimeLine.WaveSettingInspector()
            {
                Enable = 左声道.IsChecked == true,
                Frequency = 当刻频率L.IsChecked == true,
                Volume = 当刻振幅L.IsChecked == true,
            };
            MainWindow.preTimeLine.RightWaveSetting = new TimeLine.WaveSettingInspector()
            {
                Enable = 右声道.IsChecked == true,
                Frequency = 当刻频率R.IsChecked == true,
                Volume = 当刻振幅R.IsChecked == true,
            };
        }
        private void Update()
        {
            foreach (var t in LFreT)
            {
                if (左声道.IsChecked == true && 当刻频率L.IsChecked == true)
                    t.Foreground = new SolidColorBrush(Colors.White);
                else
                    t.Foreground = new SolidColorBrush(Colors.DarkGray);
            }
            foreach (var t in LVolT)
            {
                if (左声道.IsChecked == true && 当刻振幅L.IsChecked == true)
                    t.Foreground = new SolidColorBrush(Colors.White);
                else
                    t.Foreground = new SolidColorBrush(Colors.DarkGray);
            }
            foreach (var t in RFreT)
            {
                if (右声道.IsChecked == true && 当刻频率R.IsChecked == true)
                    t.Foreground = new SolidColorBrush(Colors.White);
                else
                    t.Foreground = new SolidColorBrush(Colors.DarkGray);
            }
            foreach (var t in RVolT)
            {
                if (右声道.IsChecked == true && 当刻振幅R.IsChecked == true)
                    t.Foreground = new SolidColorBrush(Colors.White);
                else
                    t.Foreground = new SolidColorBrush(Colors.DarkGray);
            }
            if (左声道.IsChecked == true) 左声道栏.IsEnabled = true;
            else 左声道栏.IsEnabled = false;
            if (当刻频率L.IsChecked == true) 左频率栏.IsEnabled = true;
            else 左频率栏.IsEnabled = false;
            if (当刻振幅L.IsChecked == true) 左振幅栏.IsEnabled = true;
            else 左振幅栏.IsEnabled = false;
            if (右声道.IsChecked == true) 右声道栏.IsEnabled = true;
            else 右声道栏.IsEnabled = false;
            if (当刻频率R.IsChecked == true) 右频率栏.IsEnabled = true;
            else 右频率栏.IsEnabled = false;
            if (当刻振幅R.IsChecked == true) 右振幅栏.IsEnabled = true;
            else 右振幅栏.IsEnabled = false;
        }
        private double highest(TimeLine wav, string lor, string param)
        {
            double s = 0;
            foreach (var v in wav.TickNodes)
            {
                var m = v.WaveNodesLeft;
                if (lor == "r") m = v.WaveNodesRight;
                if (m != null)
                    foreach (var h in m)
                    {
                        if (h.Param[param].Count != 0)
                        {
                            var a = (double)h.Param[param].Sum(t => t.Value) / h.Param[param].Count;
                            if (a > s) s = a;
                        }
                    }
            }
            return s;
        }
        private double lowest(TimeLine wav, string lor, string param)
        {
            double s = -1;
            foreach (var v in wav.TickNodes)
            {
                var m = v.WaveNodesLeft;
                if (lor == "r") m = v.WaveNodesRight;
                if (m != null)
                    foreach (var h in m)
                    {
                        if (h.Param[param].Count != 0)
                        {
                            var a = (double)h.Param[param].Sum(t => t.Value) / h.Param[param].Count;
                            if (a < s || s == -1) s = a;
                        }
                    }
            }
            return s;
        }
        private double average(TimeLine wav, string lor, string param)
        {
            double s = 0;
            int times = 0;
            foreach (var v in wav.TickNodes)
            {
                var m = v.WaveNodesLeft;
                if (lor == "r") m = v.WaveNodesRight;
                if (m != null)
                    foreach (var h in m)
                    {
                        if (h.Param[param].Count != 0)
                        {
                            var a = (double)h.Param[param].Sum(t => t.Value) / h.Param[param].Count;
                            s += a;
                            times++;
                        }
                    }
            }
            if (times != 0) return s / times;
            else return 0;
        }

        string old_text = "";
        int[] _COMMIT = new int[] { -1, -1, -1 };
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
    }
}
