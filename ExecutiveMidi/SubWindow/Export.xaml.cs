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
            Textbox_float.Text = MainWindow.Rate.ToString();
            保持区块加载.IsChecked = true;
            保持区块加载.IsChecked = false;
            Info2.Text = MainWindow.preTimeLine.Param["TotalTicks"].Value.ToString() + " ticks";
            var m = MainWindow.preTimeLine.Param["TotalTicks"].Value / 1200;
            var s = MainWindow.preTimeLine.Param["TotalTicks"].Value % 1200 / 20;
            Info1.Text = m.ToString() + " : " + s.ToString();
            Done.IsEnabled = true;
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
                if (old_text_float != t.Text) Ok.IsEnabled = true;
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
                if (old_text != t.Text) Ok.IsEnabled = true;
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

        public bool usingRate = true;
        public TimeLine markLine;
        public List<SubWindow.BeatElement> beatElements = new List<SubWindow.BeatElement>();
        private void OK(object sender, MouseButtonEventArgs e)
        {
            if (MainWindow.Midipath != "")
            {
                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("^[0-9]+\\.$");
                if (reg.IsMatch(Textbox_float.Text)) Textbox_float.Text += "0";
                var m_ = MainWindow.Midipath;
                var g = new SubWindow.Waiting(); g.Owner = this;
                BackgroundWorker waiting = new BackgroundWorker();
                waiting.DoWork += (ee, ea) => { };
                waiting.RunWorkerCompleted += (ee, ea) =>
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try { g.ShowDialog(); } catch { }
                    }));
                };
                waiting.RunWorkerAsync();
                //Work
                if (usingRate)
                {
                    var b = double.Parse(Textbox_float.Text); if (b == 0) { Textbox_float.Text = "1"; b = 1; }
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    worker.DoWork += (ee, ea) =>
                    {
                        markLine = new AudioStreamMidi().SerializeByRate(m_, new TimeLine(), b, MainWindow.SetProgressBar);
                    };
                    worker.RunWorkerCompleted += (ee, ea) =>
                    {
                        g.Close();
                        Info2.Text = markLine.Param["TotalTicks"].Value.ToString() + " ticks";
                        var m = markLine.Param["TotalTicks"].Value / 1200;
                        var s = markLine.Param["TotalTicks"].Value % 1200 / 20;
                        Info1.Text = m.ToString() + " : " + s.ToString();
                        MainWindow.Rate = Double.Parse(Textbox_float.Text);
                        MainWindow.SetProgressBar(0);
                    };
                    worker.RunWorkerAsync();
                }
                else
                {
                    beatElements.Clear();
                    var b = Int32.Parse(Textbox_int.Text); if (b <= 0) { Textbox_int.Text = "1"; b = 1; }
                    //Work
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    worker.DoWork += (ee, ea) =>
                    {
                        MainWindow.SetTotalProgressStage(2);
                        markLine = new AudioStreamMidi().SerializeByBeat(m_, new TimeLine(), b, MainWindow.SetProgressBar);
                        MainWindow.AddProgressStage();
                        int last_bpm = -1;
                        for (int i = 0; i < markLine.TickNodes.Count; i++)
                        {
                            if (markLine.TickNodes[i].BPM >= 0 && markLine.TickNodes[i].BPM != last_bpm) //BPM changed
                            {
                                double tps = (double)markLine.TickNodes[i].BPM * markLine.TicksPerBeat / 60 / b;
                                beatElements.Add(new SubWindow.BeatElement()
                                {
                                    BPM = markLine.TickNodes[i].BPM.ToString(),
                                    TickStart = i,
                                    TPS = tps.ToString("0.0000")
                                });
                                last_bpm = markLine.TickNodes[i].BPM;
                            }
                            MainWindow.SetStagedProgressBar((double)i / markLine.TickNodes.Count);
                        }
                    };
                    worker.RunWorkerCompleted += (ee, ea) =>
                    {
                        g.Close();
                        Info1.Text = (markLine.SynchronousRate * 100).ToString("0.00") + "%";
                        var toolTip = new TextBlock();
                        toolTip.Text = "有 " + (int)((1 - markLine.SynchronousRate) * markLine.Param["TotalTicks"].Value) + " 个音符与原Midi不同步.\n序列总长度为 " + markLine.TickNodes.Count + " .";
                        Info1.ToolTip = toolTip;
                        MainWindow.SynchroTick = Int32.Parse(Textbox_int.Text);
                        MainWindow.ResetProgressStage();
                    };
                    worker.RunWorkerAsync();
                }
                Ok.IsEnabled = false;
            }
        }

        public void Update()
        {
            延伸方向.SelectedIndex = MainWindow.export.Direction;
            序列宽度.Text = MainWindow.export.Width.ToString();
            保持区块加载.IsChecked = MainWindow.export.AlwaysActive;
            if (MainWindow.Midipath != "")
            {
                if (MainWindow.SynchroTick <= 0)
                {
                    SwitchRate();
                    Textbox_float.Text = MainWindow.Rate.ToString();
                }
                else
                {
                    SwitchBeat();
                    Textbox_int.Text = MainWindow.SynchroTick.ToString();
                }
                Ok.IsEnabled = true;
            }
            else
            {
                SwitchRate();
                Textbox_float.Text = "1";
            }
        }
        public void SwitchBeat(int beat = -1)
        {
            Tip1.Text = "重设节奏间隔";
            var tooltip = new TextBlock();
            tooltip.Text = "重设节奏间隔（midt/mct）\n（midt是Midi中的绝对tick，mct是Minecraft中的tick）\n按照节奏而不是绝对时间，生成Midi序列.\n初始值为TPB(tick / beat，即每拍的tick)，不一定准确，请自行修改.";
            Tip1.ToolTip = tooltip;
            Textbox_float.Visibility = Visibility.Hidden;
            Textbox_int.Visibility = Visibility.Visible;
            Tip2.Text = "同步率";
            var _tooltip = new TextBlock();
            _tooltip.Text = "以该刻为间隔 与原Midi同步的音符占比";
            Tip2.ToolTip = _tooltip;
            Info1.Text = "";
            Info1.ToolTip = null;
            BeatList.Visibility = Visibility.Visible;
            usingRate = false;
            Info2.Text = "";
            Info2.Visibility = Visibility.Hidden;
            if (beat > 0) Textbox_int.Text = beat.ToString();
        }
        public void SwitchRate(double rate = -1)
        {
            Tip1.Text = "设播放倍率";
            var tooltip = new TextBlock();
            tooltip.Text = "重设播放倍率 调整Midi的时长";
            Tip1.ToolTip = tooltip;
            Textbox_float.Visibility = Visibility.Visible;
            Textbox_int.Visibility = Visibility.Hidden;
            Tip2.Text = "Midi时长";
            var _tooltip = new TextBlock();
            _tooltip.Text = "在该BPM下 Midi的时长";
            Tip2.ToolTip = _tooltip;
            Info1.Text = "";
            Info1.ToolTip = null;
            BeatList.Visibility = Visibility.Hidden;
            usingRate = true;
            Info2.Text = "";
            Info2.Visibility = Visibility.Visible;
            if (rate > 0) Textbox_float.Text = rate.ToString();
        }
        private void SwitchView(object sender, MouseButtonEventArgs e)
        {
            if (usingRate) SwitchBeat();
            else SwitchRate();
            Ok.IsEnabled = true;
        }
        private void DoneChanges(object sender, MouseButtonEventArgs e)
        {
            MainWindow.export = new Audio2Minecraft.ExportSetting() { AlwaysActive = 保持区块加载.IsChecked == true, AlwaysLoadEntities = false, AutoTeleport = false, Direction = 延伸方向.SelectedIndex, Width = Int32.Parse(序列宽度.Text) };
            MainWindow.preTimeLine = markLine;
            Hide();
        }
        private void ShowBeatList(object sender, MouseButtonEventArgs e)
        {
            var n = new SubWindow.BeatList(beatElements); n.Owner = MainWindow.main;
            Console.WriteLine(beatElements.Count);
            n.ShowDialog();
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!MainWindow.closing)
            {
                Hide();
                e.Cancel = true;
            }
        }
    }
}
