using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Audio2Minecraft;

namespace Audio2MinecraftUI.Humberger
{
    /// <summary>
    /// Export.xaml 的交互逻辑
    /// </summary>
    public partial class Export : UserControl
    {
        public Export()
        {
            InitializeComponent();
        }
        private void UserControl_Initialized(object sender, EventArgs e)
        {
            延伸方向.Items.Add("东（x+）");
            延伸方向.Items.Add("西（x-）");
            延伸方向.Items.Add("南（z+）");
            延伸方向.Items.Add("北（z-）");
            延伸方向.SelectedIndex = 0;
            序列宽度.Text = "16";
            保持区块加载.IsChecked = true;
            保持区块加载.IsChecked = false;
            播放模式.Items.Add("同步更新");
            播放模式.Items.Add("单次输出");
            播放模式.SelectedIndex = 0;
            var colors = new List<string>() { "black", "dark_blue", "dark_green", "dark_aqua", "dark_red", "dark_purple", "gold", "gray", "dark_gray", "blue", "green", "aqua", "red", "light_purple", "yellow", "white" };
            已播放颜色.ItemsSource = colors;
            已播放颜色.SelectedIndex = 15;
            未播放颜色.ItemsSource = colors;
            未播放颜色.SelectedIndex = 15;
            双声道.Items.Add("无双声道");
            双声道.Items.Add("面向X+");
            双声道.Items.Add("面向X-");
            双声道.Items.Add("面向Z+");
            双声道.Items.Add("面向Z-");
        }
        string old_text_float = "";
        private void FloatOnly(object sender, TextChangedEventArgs e)
        {
            var t = e.OriginalSource as TextBox;
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("^[0-9]+([.][0-9]*)?$");
            if (!reg.IsMatch(t.Text))
                t.Text = old_text_float;
            else if (t.Text == "")
                t.Text = "1";
            else
            {
                if (old_text_float != t.Text) Done.IsEnabled = true;
                old_text_float = t.Text;
            }
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
            else
            {
                if (old_text != t.Text) Done.IsEnabled = true;
                old_text = t.Text;
            }
        }
        private void _KeyDownFloat(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D0 || e.Key == Key.D1 || e.Key == Key.D2 || e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5 || e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9 || e.Key == Key.NumPad0 || e.Key == Key.NumPad1 || e.Key == Key.NumPad2 || e.Key == Key.NumPad3 || e.Key == Key.NumPad4 || e.Key == Key.NumPad5 || e.Key == Key.NumPad6 || e.Key == Key.NumPad7 || e.Key == Key.NumPad8 || e.Key == Key.NumPad9 || e.Key == Key.Decimal || e.Key == Key.OemPeriod)
                Done.IsEnabled = true;
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
        private void 双声道_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Done.IsEnabled = true;
        }

        public List<SubWindow.BeatElement> beatElements = new List<SubWindow.BeatElement>();
        private void DoneChanges(object sender, MouseButtonEventArgs e)
        {
            MainWindow.ExportSetting.Direction = 延伸方向.SelectedIndex;
            MainWindow.ExportSetting.Width = Int32.Parse(序列宽度.Text);
            MainWindow.ExportSetting.AlwaysActive = 保持区块加载.IsChecked == true;
            MainWindow.ExportSetting.AlwaysLoadEntities = 保持实体加载.IsChecked == true;
            MainWindow.ExportSetting.AutoTeleport = 自动传送.IsChecked == true;
            MainWindow.LyricMode.LyricOutSet.repeat = 播放模式.SelectedIndex == 0;
            MainWindow.LyricMode.LyricOutSet.color1 = 已播放颜色.Text;
            MainWindow.LyricMode.LyricOutSet.color2 = 未播放颜色.Text;
            MainWindow.PublicSet.ST = 双声道.SelectedIndex;
            if (MainWindow.Midipath != "" && (MainWindow.Rate.ToString() != 重设播放倍率.Text || MainWindow.SynchroTick.ToString() != 重设节奏间隔.Text))
            {
                var m_ = MainWindow.Midipath;
                var a = new TimeLine();
                var w = new SubWindow.Waiting(); w.Owner = Application.Current.MainWindow;
                BackgroundWorker waiting = new BackgroundWorker();
                waiting.DoWork += (ee, ea) => { };
                waiting.RunWorkerCompleted += (ee, ea) =>
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try { w.ShowDialog(); } catch { }
                    }));
                };
                waiting.RunWorkerAsync();
                if (Midi.IsVisible) //Using Rate
                {
                    System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("^[0-9]+\\.$");
                    if (reg.IsMatch(重设播放倍率.Text)) 重设播放倍率.Text += "0";
                    var b = double.Parse(重设播放倍率.Text); if (b == 0) { 重设播放倍率.Text = "1"; b = 1; }
                    //Work
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    worker.DoWork += (ee, ea) =>
                    {
                        a = new AudioStreamMidi().SerializeByRate(m_, new TimeLine(), b, MainWindow.SetProgressBar);
                    };
                    worker.RunWorkerCompleted += (ee, ea) =>
                    {
                        w.Close();
                        Midi刻长.Text = a.Param["TotalTicks"].Value.ToString() + " ticks";
                        var m = a.Param["TotalTicks"].Value / 1200;
                        var s = a.Param["TotalTicks"].Value % 1200 / 20;
                        Midi时长.Text = m.ToString() + " : " + s.ToString();
                        MainWindow.Rate = Double.Parse(重设播放倍率.Text);
                        MainWindow.preTick = a.Param["TotalTicks"].Value;
                        MainWindow.SetProgressBar(0);
                    };
                    worker.RunWorkerAsync();
                }
                else //Using Beat
                {
                    beatElements.Clear();
                    var b = Int32.Parse(重设节奏间隔.Text); if (b <= 0) { 重设节奏间隔.Text = "1"; b = 1; }
                    //Work
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    worker.DoWork += (ee, ea) =>
                    {
                        MainWindow.SetTotalProgressStage(2);
                        a = new AudioStreamMidi().SerializeByBeat(m_, new TimeLine(), b, MainWindow.SetProgressBar);
                        MainWindow.AddProgressStage();
                        int last_bpm = -1;
                        for (int i = 0; i < a.TickNodes.Count; i++)
                        {
                            if (a.TickNodes[i].BPM >= 0 && a.TickNodes[i].BPM != last_bpm) //BPM changed
                            {
                                double tps = (double)a.TickNodes[i].BPM * a.TicksPerBeat / 60 / b;
                                beatElements.Add(new SubWindow.BeatElement()
                                {
                                    BPM = a.TickNodes[i].BPM.ToString(),
                                    TickStart = i,
                                    TPS = tps.ToString("0.0000")
                                });
                                last_bpm = a.TickNodes[i].BPM;
                            }
                            MainWindow.SetStagedProgressBar((double)i / a.TickNodes.Count);
                        }
                    };
                    worker.RunWorkerCompleted += (ee, ea) =>
                    {
                        w.Close();
                        Midi同步率.Text = (a.SynchronousRate * 100).ToString("0.00") + "%";
                        var toolTip = new TextBlock();
                        toolTip.Text = "有 " + (int)((1 - a.SynchronousRate) * a.Param["TotalTicks"].Value) + " 个音符与原Midi不同步.\n序列总长度为 " + a.TickNodes.Count + " .";
                        Midi同步率.ToolTip = toolTip;
                        MainWindow.preTick = a.Param["TotalTicks"].Value;
                        MainWindow.SynchroTick = Int32.Parse(重设节奏间隔.Text);
                        MainWindow.ResetProgressStage();
                    };
                    worker.RunWorkerAsync();   
                }
            }
            Done.IsEnabled = false;
        }

        public void SwitchToRate()
        {
            Midi.Visibility = Visibility.Visible;
            Midi_Beat.Visibility = Visibility.Hidden;
            Done.IsEnabled = true;
            SwitchImage.Source = new BitmapImage(new Uri(@"\img\rate_view.png", UriKind.Relative));
        }
        public void SwitchToBeat()
        {
            Midi.Visibility = Visibility.Hidden;
            Midi_Beat.Visibility = Visibility.Visible;
            Done.IsEnabled = true;
            SwitchImage.Source = new BitmapImage(new Uri(@"\img\beat_view.png", UriKind.Relative));
        }
        private void SwitchView(object sender, MouseButtonEventArgs e)
        {
            if (Midi.IsVisible) SwitchToBeat();
            else SwitchToRate();
        }
        public void setBeat(int synchroTick = -1)
        {
            if (synchroTick > 0) 重设节奏间隔.Text = synchroTick.ToString();
            else 重设节奏间隔.Text = "1";
        }
        public void Update()
        {
            延伸方向.SelectedIndex = MainWindow.ExportSetting.Direction;
            序列宽度.Text = MainWindow.ExportSetting.Width.ToString();
            保持区块加载.IsChecked = MainWindow.ExportSetting.AlwaysActive;
            保持实体加载.IsChecked = MainWindow.ExportSetting.AlwaysLoadEntities;
            自动传送.IsChecked = MainWindow.ExportSetting.AutoTeleport;
            双声道.SelectedIndex = MainWindow.PublicSet.ST;
            if (MainWindow.Midipath != "")
            {
                Midi.IsEnabled = true;
                重设播放倍率.Text = MainWindow.Rate.ToString();
                Midi刻长.Text = MainWindow.preTick.ToString() + " ticks";
                var m = MainWindow.preTick / 1200;
                var s = MainWindow.preTick % 1200 / 20;
                Midi时长.Text = m.ToString() + " : " + s.ToString();
            }
            else
            {
                Midi.IsEnabled = false;
                重设播放倍率.Text = "1";
                Midi时长.Text = "";
                Midi刻长.Text = "";
                重设节奏间隔.Text = "1";
                Midi同步率.ToolTip = "";
                Midi同步率.Text = "";
            }

            if (MainWindow.Lrcpath != "")
            {
                Lrc.IsEnabled = true;
                播放模式.SelectedIndex = (MainWindow.LyricMode.LyricOutSet.repeat) ? 0 : 1;
                已播放颜色.Text = MainWindow.LyricMode.LyricOutSet.color1;
                未播放颜色.Text = MainWindow.LyricMode.LyricOutSet.color2;
            }
            else
            {
                Lrc.IsEnabled = false;
            }
            Done.IsEnabled = false;
        }
        private void ShowBeatList(object sender, MouseButtonEventArgs e)
        {
            var n = new SubWindow.BeatList(beatElements); n.Owner = MainWindow.main;
            Console.WriteLine(beatElements.Count);
            n.ShowDialog();
        }
    }
}
