using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Audio2Minecraft;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using MahApps.Metro.Controls;
using Newtonsoft.Json;

namespace Audio2MinecraftUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static _Version currentV = new _Version();
        public static TimeLine preTimeLine = new TimeLine(); //预览时间序列
        public static string projName = ""; //工程名
        private static List<UserControl> Controls; //所有的子用户控件
        public static string Midipath = "", oldMidi = ""; //Midi路径
        public static string Wavepath = ""; //波形路径
        public static string Lrcpath = ""; //歌词路径
        public static double Rate = 1; //播放速率
        public static int preTick = 0; //重设BPM后的预览序列的播放时长
        public static ExportSetting ExportSetting = new ExportSetting() //导出设置
        {
            Direction = 0, //序列方向
            Width = 16, //序列宽度
            AlwaysActive = true, //保持命令加载
            AlwaysLoadEntities = false, //保持实体加载
            AutoTeleport = false //自动Tp
        };
        public static class PublicSet //通用设置
        {
            static public bool OBPM = false; //BPM计分板输出
            static public bool Q = false; //¼音符占刻输出
            static public bool TC = false; //总刻数输出
            static public int ST = 0; //双声道方向
        }
        public static class LyricMode //歌词设置
        {
            static public bool Title = false;
            static public bool SubTitle = false;
            static public bool ActionBar = false;
            static public bool Tellraw = false;
            public static class LyricOutSet //歌词显示设置
            {
                static public bool repeat = true;
                static public string color1 = "white";
                static public string color2 = "white";
            }
        }
        public static string autoFillRule = "无", autoFillMode = ""; //自动补全规则 & 模式
        public static List<ExtensionFile> ExtensionFiles = new List<ExtensionFile>();
        public static string datapackName = "NewMusic"; //数据包名称

        public MainWindow()
        {
            InitializeComponent();
        }
        private void MetroWindow_Initialized(object sender, EventArgs e)
        {
            load.IsEnabled = false;
            A2MSave.IsEnabled = false;
            cancel0.Visibility = Visibility.Hidden;
            cancel1.Visibility = Visibility.Hidden;
            cancel2.Visibility = Visibility.Hidden;

            Controls = new List<UserControl>()
            {
                MidiSetting,
                WavSetting,
                PublicSetting,
                LrcSetting,
                Export
            };
            DisableAllControls();
            HideAllControls();
            MidiSetting.Visibility = Visibility.Visible;
        }
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _left = this.Left + 170; _top = this.Top + 140;
            var n = new Thread(g => alarmVersion()) { IsBackground = true };
            n.SetApartmentState(ApartmentState.STA);
            n.Start();
        }
        double _top = 0, _left = 0;
        private void alarmVersion()//查看版本更新
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; //建立安全通道
            //查看版本更新
            if (IsConnectInternet())
            {
                try
                {
                    _Version _v = new _Version();
                    Encoding encoding = Encoding.UTF8;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://tremblestarman.github.io/Audio2Minecraft/resource/version.json");
                    request.Method = "GET";
                    request.Accept = "text/html, application/xhtml+xml, */*";
                    request.ContentType = "application/json";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        _v = JsonConvert.DeserializeObject<_Version>(reader.ReadToEnd());
                    }
                    if (_v.version != currentV.version) //版本不同
                    {
                        var alarm = new SubWindow.VersionAlarm();
                        alarm.Top = _top; alarm.Left = _left;
                        alarm.new_version.Text = "检测到最新版本: " + _v.version;
                        alarm.log.Text = _v.log.Replace("\n", Environment.NewLine);
                        alarm.download_url = _v.download;
                        alarm.Topmost = true;
                        alarm.Show();
                    }
                }
                catch { }
            }
            System.Windows.Threading.Dispatcher.Run();
        }
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(int Description, int ReservedValue);
        public static bool IsConnectInternet() //检测联网
        {
            int Description = 0;
            return InternetGetConnectedState(Description, 0);
        }

        //文件路径确认
        private void MidiSelect(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Midi|*.mid";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == true && fileDialog.FileName != null && fileDialog.FileName != "")
            {
                MidiPath.Text = fileDialog.FileName;
                load.IsEnabled = true;
                SetFileShow();
                cancel0.Visibility = Visibility.Visible;
            }
        }
        private void MidiCancel(object sender, MouseButtonEventArgs e)
        {
            MidiPath.Text = "";
            SetFileShow();
            if (MidiPath.Text == "" && WavePath.Text == "" && LrcPath.Text == "")
            {
                load.IsEnabled = false;
                A2MSave.IsEnabled = false;
            }
            cancel0.Visibility = Visibility.Hidden;
            MidiSetting.TracksView.ItemsSource = null;
            MidiSetting.IsEnabled = false;
        }

        private void WaveSelect(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Wave|*.wav";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == true && fileDialog.FileName != null && fileDialog.FileName != "")
            {
                WavePath.Text = fileDialog.FileName;
                load.IsEnabled = true;
                SetFileShow();
                cancel1.Visibility = Visibility.Visible;
            }
        }
        private void WaveCancel(object sender, MouseButtonEventArgs e)
        {
            WavePath.Text = "";
            SetFileShow();
            if (MidiPath.Text == "" && WavePath.Text == "" && LrcPath.Text == "")
            {
                load.IsEnabled = false;
                A2MSave.IsEnabled = false;
            }
            cancel1.Visibility = Visibility.Hidden;
            WavSetting.IsEnabled = false;
        }

        private void LrcSelect(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Lrc|*.lrc;*.amlrc;*.txt";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == true && fileDialog.FileName != null && fileDialog.FileName != "")
            {
                LrcPath.Text = fileDialog.FileName;
                load.IsEnabled = true;
                SetFileShow();
                cancel2.Visibility = Visibility.Visible;
            }
        }
        private void LrcCancel(object sender, MouseButtonEventArgs e)
        {
            LrcPath.Text = "";
            SetFileShow();
            if (MidiPath.Text == "" && WavePath.Text == "" && LrcPath.Text == "")
            {
                load.IsEnabled = false;
                A2MSave.IsEnabled = false;
            }
            cancel2.Visibility = Visibility.Hidden;
        }

        //确认导入
        private void SetFileShow()
        {
            var midiName = (File.Exists(MidiPath.Text)) ? " Midi: \"" + new FileInfo(MidiPath.Text).Name + "\"" : "";
            var waveName = (File.Exists(WavePath.Text)) ? " Wave: \"" + new FileInfo(WavePath.Text).Name + "\"" : "";
            var lrcName = (File.Exists(LrcPath.Text)) ? " Lrc: \"" + new FileInfo(LrcPath.Text).Name + "\"" : "";
            if (midiName == "" && waveName == "" && lrcName == "") projName = "";
            FileShow.Content = (projName != "") ? "A2mproj: \"" + projName + "\"" : (midiName == "" && waveName == "" && lrcName == "") ? "" : midiName + waveName + lrcName;
        }
        private void Load(object sender, MouseButtonEventArgs e)
        {
            PublicSetting.IsEnabled = true;
            Export.IsEnabled = true;
            if (MidiPath.Text != "" && new FileInfo(MidiPath.Text).Exists)
            {
                var m = MidiPath.Text;
                //Waiting...
                var w = new SubWindow.Waiting(); w.Owner = this;
                BackgroundWorker waiting = new BackgroundWorker();
                waiting.DoWork += (ee, ea) => { };
                waiting.RunWorkerCompleted += (ee, ea) =>
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        w.ShowDialog();
                    }));
                };
                waiting.RunWorkerAsync();
                //Work
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += (o, ea) =>
                {
                    if (oldMidi == m) preTimeLine = UpdateMidiInspector(new AudioStreamMidi().Serialize(m, new TimeLine()), preTimeLine);
                    else preTimeLine = new AudioStreamMidi().Serialize(m, new TimeLine());
                };
                worker.RunWorkerCompleted += (o, ea) =>
                {
                    w.Close();
                    Midipath = MidiPath.Text;
                    MidiSetting.IsEnabled = true;
                    MidiSetting.TracksView.ItemsSource = preTimeLine.TrackList; //Track
                    MidiSetting.ItemChanged();
                    PublicSetting.MidiPlat.IsEnabled = true;

                    PublicSetting.OBPM.IsChecked = preTimeLine.OutPutBPM;
                    PublicSetting.TTC.Text = preTimeLine.Param["MidiTracksCount"].Value.ToString();
                    PublicSetting.TQ.Text = preTimeLine.Param["MidiDeltaTicksPerQuarterNote"].Value.ToString();

                    oldMidi = MidiPath.Text;

                    preTick = preTimeLine.Param["TotalTicks"].Value;
                };
                worker.RunWorkerAsync();
            }
            if (WavePath.Text != "" && new FileInfo(WavePath.Text).Exists)
            {
                Wavepath = WavePath.Text;
                WavSetting.IsEnabled = true;
                WavSetting.最大频率L.Text = "";
                WavSetting.最大振幅L.Text = "";
                WavSetting.最小频率L.Text = "";
                WavSetting.最小振幅L.Text = "";
                WavSetting.平均频率L.Text = "";
                WavSetting.平均振幅L.Text = "";
                WavSetting.最大频率R.Text = "";
                WavSetting.最大振幅R.Text = "";
                WavSetting.最小频率R.Text = "";
                WavSetting.最小振幅R.Text = "";
                WavSetting.平均频率R.Text = "";
                WavSetting.平均振幅R.Text = "";
                WavSetting.左声道.IsChecked = true;
                WavSetting.当刻频率L.IsChecked = true;
                WavSetting.当刻振幅L.IsChecked = true;
                WavSetting.右声道.IsChecked = false;
                WavSetting.当刻频率R.IsChecked = false;
                WavSetting.当刻振幅R.IsChecked = false;
                WavSetting.ItemChanged();
            }
            if (LrcPath.Text != "" && new FileInfo(LrcPath.Text).Exists)
            {
                Lrcpath = LrcPath.Text;
                LrcSetting.IsEnabled = true;
                LrcSetting.Update();
            }
            PublicSetting.ItemChanged();
            Export.Update();
            A2MSave.IsEnabled = true;
        }

        public static Dictionary<string, AutoFill> AutoFills = new Dictionary<string, AutoFill>();
        //自动补全设置
        private void OpenSetting(object sender, MouseButtonEventArgs e)
        {
            //Get AutoFills
            GetAutoFills(AppDomain.CurrentDomain.BaseDirectory + "config\\autofill");
            SubWindow.AutoFillSelector n = new SubWindow.AutoFillSelector(); n.Owner = this;
            //Initialize
            var rules = (from r in AutoFills select r.Value.RuleName).OrderBy(r => r).ToList();
            rules.Add("无");
            n.rule.ItemsSource = rules;
            n.rule.SelectedItem = "无";
            n.mode.ItemsSource = null;
            n.AutoFills = AutoFills;
            //Show
            if (autoFillRule != "") n.rule.SelectedItem = autoFillRule;
            if (autoFillMode != "") n.mode.SelectedItem = autoFillMode;
            n.ShowDialog();
            //Set AutoFill
            if (autoFillRule == "无")
                MidiSetting.UpdateAutoFill(null, autoFillRule);
            else
            {
                MidiSetting.UpdateAutoFill(AutoFills[autoFillRule], autoFillRule);
                if (MidiSetting.Visibility == Visibility.Visible) Midi设置显示(null, null);
                if (WavSetting.Visibility == Visibility.Visible) Wav设置显示(null, null);
                if (PublicSetting.Visibility == Visibility.Visible) 全局设置显示(null, null);
                if (LrcSetting.Visibility == Visibility.Visible) 歌词设置显示(null, null);
                if (Export.Visibility == Visibility.Visible) 导出设置显示(null, null);//refresh
            }
        }
        private void ExportText(object sender, MouseButtonEventArgs e)
        {
            if (MidiPath.Text == "")
            {
                MessageBox.Show("你还没有选中任何Midi文件", "导出Midi信息");
                return;
            }
            if (!new FileInfo(MidiPath.Text).Exists)
            {
                MessageBox.Show("Midi文件不存在", "导出Midi信息");
                return;
            }
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "MidiInfo in Excel(*.xlsx)|*.xlsx|MidiInfo in Txt(*.txt)|*.txt";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == true && fileDialog.FileName != null && fileDialog.FileName != "")
            {
                if (fileDialog.FilterIndex == 1) //Excel
                {
                    //Waiting
                    var w = new SubWindow.Waiting(); w.Owner = this;
                    BackgroundWorker waiting = new BackgroundWorker();
                    waiting.DoWork += (ee, ea) => { };
                    waiting.RunWorkerCompleted += (ee, ea) =>
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            w.ShowDialog();
                        }));
                    };
                    waiting.RunWorkerAsync();
                    //Work
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    var midi = MidiPath.Text;
                    worker.DoWork += (o, ea) =>
                    {
                        new MidiInfoExcel(midi, fileDialog.FileName, Rate);
                    };
                    worker.RunWorkerCompleted += (o, ea) =>
                    {
                        w.Close();
                    };
                    worker.RunWorkerAsync();
                }
                else if (fileDialog.FilterIndex == 2) //TXT
                {
                    //Waiting
                    var w = new SubWindow.Waiting(); w.Owner = this;
                    BackgroundWorker waiting = new BackgroundWorker();
                    waiting.DoWork += (ee, ea) => { };
                    waiting.RunWorkerCompleted += (ee, ea) =>
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            w.ShowDialog();
                        }));
                    };
                    waiting.RunWorkerAsync();
                    //Work
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    var midi = MidiPath.Text;
                    worker.DoWork += (o, ea) =>
                    {
                        new MidiInfoTxt(midi, fileDialog.FileName, Rate);
                    };
                    worker.RunWorkerCompleted += (o, ea) =>
                    {
                        w.Close();
                    };
                    worker.RunWorkerAsync();
                }
            }
        }
        private void OpenExtension(object sender, MouseButtonEventArgs e)
        {
            var n = new SubWindow.Extension(); n.Owner = this;
            n.ShowDialog();
            if (ExtensionFiles.Count > 0 && MidiPath.Text == "" && WavePath.Text == "" && LrcPath.Text == "") A2MSave.IsEnabled = true;
            if (ExtensionFiles.Count == 0 && MidiPath.Text == "" && WavePath.Text == "" && LrcPath.Text == "") A2MSave.IsEnabled = false;
        }
        private void GetAutoFills(string directoryPath, string upper = "")
        {
            FileInfo[] files = new DirectoryInfo(directoryPath).GetFiles();
            DirectoryInfo[] directories = new DirectoryInfo(directoryPath).GetDirectories();
            foreach (var file in files)
            {
                if (file.Extension == ".json")
                {
                    try { AutoFills.Add(upper + file.Name, new AutoFill(file.FullName)); } catch { }
                }
            }
            foreach (var directory in directories)
            {
                GetAutoFills(directory.FullName, upper + directory.Name + "\\");
            }
        }

        //子选项
        private void Midi设置显示(object sender, MouseButtonEventArgs e)
        {
            if (MidiSetting.IsEnabled == true)
            {
                MidiSetting.ItemChanged();
            }
            HideAllControls();
            MidiSetting.Visibility = Visibility.Visible;
        }
        private void Wav设置显示(object sender, MouseButtonEventArgs e)
        {
            if (WavSetting.左.IsEnabled == true || WavSetting.左.IsEnabled == true)
            {
                WavSetting.ItemChanged();
            }
            HideAllControls();
            WavSetting.Visibility = Visibility.Visible;
        }
        private void 全局设置显示(object sender, MouseButtonEventArgs e)
        {
            if (PublicSetting.IsEnabled == true)
            {
                PublicSetting.ItemChanged();
            }
            HideAllControls();
            PublicSetting.Visibility = Visibility.Visible;
        }
        private void 歌词设置显示(object sender, MouseButtonEventArgs e)
        {
            HideAllControls();
            LrcSetting.Visibility = Visibility.Visible;
        }
        private void 导出设置显示(object sender, MouseButtonEventArgs e)
        {
            HideAllControls();
            Export.Visibility = Visibility.Visible;
            Export.Update();
        }
        private void DisableAllControls()
        {
            foreach (var c in Controls)
            {
                c.IsEnabled = false;
            }
        }
        private void HideAllControls()
        {
            foreach (var c in Controls)
            {
                c.Visibility = Visibility.Hidden;
            }
        }
        //更新Midi预览
        private TimeLine UpdateMidiInspector(TimeLine newTimeline, TimeLine baseTimeline)
        {
            Task task = Task.Run(() =>
            {
                foreach (var i in newTimeline.InstrumentList)
                {
                    foreach (var _i in baseTimeline.InstrumentList)
                    {
                        if (i.Name == _i.Name)
                        {
                            i.EnableScore = _i.EnableScore;
                            i.EnablePlaysound = _i.EnablePlaysound;
                            i.BarIndex = _i.BarIndex;
                            i.BeatDuration = _i.BeatDuration;
                            i.Channel = _i.Channel;
                            i.DeltaTickDuration = _i.DeltaTickDuration;
                            i.DeltaTickStart = _i.DeltaTickStart;
                            i.Velocity = _i.Velocity;
                            i.Pitch = _i.Pitch;
                            i.MinecraftTickDuration = _i.MinecraftTickDuration;
                            i.MinecraftTickStart = _i.MinecraftTickStart;
                            i.PlaysoundSetting.StopSound = _i.PlaysoundSetting.StopSound;
                            i.PlaysoundSetting.ExecuteCood[0] = _i.PlaysoundSetting.ExecuteCood[0];
                            i.PlaysoundSetting.ExecuteCood[1] = _i.PlaysoundSetting.ExecuteCood[1];
                            i.PlaysoundSetting.ExecuteCood[2] = _i.PlaysoundSetting.ExecuteCood[2];
                            i.PlaysoundSetting.ExecuteTarget = _i.PlaysoundSetting.ExecuteTarget;
                            i.PlaysoundSetting.PlayTarget = _i.PlaysoundSetting.PlayTarget;
                            i.PlaysoundSetting.PlaySource = _i.PlaysoundSetting.PlaySource;
                            i.PlaysoundSetting.InheritExpression = _i.PlaysoundSetting.InheritExpression;
                            i.PlaysoundSetting.ExtraDelay = _i.PlaysoundSetting.ExtraDelay;
                            i.PlaysoundSetting.SoundName = _i.PlaysoundSetting.SoundName;
                            i.PlaysoundSetting.PercVolume = _i.PlaysoundSetting.PercVolume;
                        }
                    }
                }
                foreach (var t in newTimeline.TrackList)
                {
                    foreach (var i in t.Instruments)
                    {
                        foreach (var _t in baseTimeline.TrackList)
                        {
                            if (t.Name == _t.Name)
                            {
                                foreach (var _i in _t.Instruments)
                                {
                                    if (i.Name == _i.Name)
                                    {
                                        i.EnableScore = _i.EnableScore;
                                        i.EnablePlaysound = _i.EnablePlaysound;
                                        i.BarIndex = _i.BarIndex;
                                        i.BeatDuration = _i.BeatDuration;
                                        i.Channel = _i.Channel;
                                        i.DeltaTickDuration = _i.DeltaTickDuration;
                                        i.DeltaTickStart = _i.DeltaTickStart;
                                        i.Velocity = _i.Velocity;
                                        i.Pitch = _i.Pitch;
                                        i.MinecraftTickDuration = _i.MinecraftTickDuration;
                                        i.MinecraftTickStart = _i.MinecraftTickStart;
                                        i.PlaysoundSetting.StopSound = _i.PlaysoundSetting.StopSound;
                                        i.PlaysoundSetting.ExecuteCood[0] = _i.PlaysoundSetting.ExecuteCood[0];
                                        i.PlaysoundSetting.ExecuteCood[1] = _i.PlaysoundSetting.ExecuteCood[1];
                                        i.PlaysoundSetting.ExecuteCood[2] = _i.PlaysoundSetting.ExecuteCood[2];
                                        i.PlaysoundSetting.ExecuteTarget = _i.PlaysoundSetting.ExecuteTarget;
                                        i.PlaysoundSetting.PlayTarget = _i.PlaysoundSetting.PlayTarget;
                                        i.PlaysoundSetting.PlaySource = _i.PlaysoundSetting.PlaySource;
                                        i.PlaysoundSetting.InheritExpression = _i.PlaysoundSetting.InheritExpression;
                                        i.PlaysoundSetting.ExtraDelay = _i.PlaysoundSetting.ExtraDelay;
                                        i.PlaysoundSetting.SoundName = _i.PlaysoundSetting.SoundName;
                                        i.PlaysoundSetting.PercVolume = _i.PlaysoundSetting.PercVolume;
                                    }
                                }
                            }
                        }
                    }
                }
                baseTimeline = newTimeline;
                baseTimeline.TickNodes = new List<TickNode>(); //Delect TickNodes
            });
            task.Wait();
            return baseTimeline;
        }

        //歌词获取
        public CommandLine GetLyrics()
        {
            var lrc = new FileInfo(Lrcpath);
            if (lrc.Extension == ".amlrc")
            {
                return new AMLrc().Serialize(lrc.FullName).AMLrcLine;
            }
            else if (lrc.Extension == ".lrc" || lrc.Extension == ".txt")
            {
                var _lrc = new Lrc().Serialize(lrc.FullName);
                return textLrc(_lrc, LyricMode.LyricOutSet.repeat);
            }
            else return null;
        }
        private CommandLine textLrc(Lrc lrc, bool repeat = true) //.lrc -> Tellraw
        {
            var textLrc = new CommandLine();
            foreach (var l in lrc.Lrcs)
            {
                var start = l.Start;
                var duration = l.Duration;
                var length = l.Content.Length; if (length == 0) continue;
                var pTick = (double)duration / length;
                for (int i = 0; i < duration; i++)
                {
                    if ((!repeat && (i + 1) % pTick < 1) || repeat)
                    {
                        var index = start + i;
                        var playedChar = (int)((i + 1) / pTick) + 1;

                        if (playedChar >= length) playedChar = length;
                        var hltext = l.Content.Substring(0, playedChar);
                        var ntext = l.Content.Substring(playedChar);

                        var tlw = new Json();
                        tlw.texts = new List<Json.Text>()
                        {
                            new Json.Text() { text = hltext.Replace("\u3000",@"  "), color = LyricMode.LyricOutSet.color1},
                            new Json.Text() { text = ntext.Replace("\u3000",@"  "), color = LyricMode.LyricOutSet.color2},
                        };
                        if (index >= textLrc.Keyframe.Count) for (int j = textLrc.Keyframe.Count; j <= index; j++) { textLrc.Keyframe.Add(new Command()); }
                        if (LyricMode.Title) { textLrc.Keyframe[index].Commands.Add("title @a title " + JsonConvert.SerializeObject(tlw.texts)); }
                        if (LyricMode.SubTitle) { textLrc.Keyframe[index].Commands.Add("title @a title [\"\"]"); textLrc.Keyframe[index].Commands.Add("title @a subtitle " + JsonConvert.SerializeObject(tlw.texts)); }
                        if (LyricMode.ActionBar) { textLrc.Keyframe[index].Commands.Add("title @a actionbar " + JsonConvert.SerializeObject(tlw.texts)); }
                        if (LyricMode.Tellraw) { tlw.texts.Insert(0, new Json.Text() { text = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" }); textLrc.Keyframe[index].Commands.Add("tellraw @a " + JsonConvert.SerializeObject(tlw.texts)); }
                    }
                }
            }
            if (LyricMode.Title || LyricMode.SubTitle || LyricMode.ActionBar) { textLrc.Keyframe[0].Commands.Insert(2, "title @a times 0 1000 0"); textLrc.Keyframe[textLrc.Keyframe.Count - 1].Commands.Add("title @a times 10 70 20"); }
            return textLrc;
        }

        //文件操作
        public void Save(object sender, MouseButtonEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "A2M Project(*.amproj)|*.amproj|Universal Schematic(*.schematic)|*.schematic|WorldEdit Schematic(*.schematic)|*.schematic|WorldEdit 1.13 Schematic(*.schem)|*.schem";
            fileDialog.FilterIndex = 1;
            foreach (var t in preTimeLine.TrackList) //New Tracks List
            {
                foreach (var i in t.Instruments)
                {
                    i.Tracks = new ObservableCollection<TimeLine.MidiSettingInspector>();
                }
            }
            if (fileDialog.ShowDialog() == true && fileDialog.FileName != null && fileDialog.FileName != "")
            {
                if (fileDialog.FilterIndex == 1)
                {
                    //Waiting
                    var w = new SubWindow.Waiting(); w.Owner = this;
                    BackgroundWorker waiting = new BackgroundWorker();
                    waiting.DoWork += (ee, ea) => { };
                    waiting.RunWorkerCompleted += (ee, ea) =>
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            w.ShowDialog();
                        }));
                    };
                    waiting.RunWorkerAsync();
                    int m_1 = Int32.Parse(WavSetting.单刻频率采样数.Text), m_2 = Int32.Parse(WavSetting.单刻振幅采样数.Text), m_3 = Int32.Parse(WavSetting.采样周期.Text);
                    //Work
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    worker.DoWork += (o, ea) =>
                    {
                        foreach (var ex in ExtensionFiles) //Update ExtensionFiles
                        {
                            ex.AbsToR(fileDialog.FileName);
                        }
                        var f = new FileOutPut() //写入文件结构
                        {
                            MidiTracks = preTimeLine.TrackList,
                            MidiInstruments = preTimeLine.InstrumentList,
                            LeftWaveSetting = preTimeLine.LeftWaveSetting,
                            RightWaveSetting = preTimeLine.RightWaveSetting,
                            Midipath = Midipath,
                            rMidipath = (Midipath != "") ? new Uri(fileDialog.FileName.Replace(" ", "*20")).MakeRelativeUri(new Uri(Midipath.Replace(" ", "*20"))) : null,
                            Wavepath = Wavepath,
                            rWavepath = (Wavepath != "") ? new Uri(fileDialog.FileName.Replace(" ", "*20")).MakeRelativeUri(new Uri(Wavepath.Replace(" ", "*20"))) : null,
                            Lrcpath = Lrcpath,
                            rLrcpath = (Lrcpath != "") ? new Uri(fileDialog.FileName.Replace(" ", "*20")).MakeRelativeUri(new Uri(Lrcpath.Replace(" ", "*20"))) : null,
                            ExportSetting = ExportSetting,
                            Rate = Rate,
                            preTick = preTick,
                            PublicSetting = new FileOutPut._PublicSetting()
                            {
                                OBPM = preTimeLine.OutPutBPM,
                                Q = PublicSet.Q,
                                TC = PublicSet.TC,
                                ST = PublicSet.ST,
                                TTC = preTimeLine.Param["MidiTracksCount"].Value,
                                TQ = preTimeLine.Param["MidiDeltaTicksPerQuarterNote"].Value
                            },
                            LyricMode = new FileOutPut._LyricMode()
                            {
                                Tellraw = LyricMode.Tellraw,
                                SubTitle = LyricMode.SubTitle,
                                ActionBar = LyricMode.ActionBar,
                                Title = LyricMode.Title,
                                LyricOutSetting = new FileOutPut._LyricMode.LyricOutSet()
                                {
                                    color1 = LyricMode.LyricOutSet.color1,
                                    color2 = LyricMode.LyricOutSet.color2,
                                    repeat = LyricMode.LyricOutSet.repeat,
                                }
                            },
                            ExtensionFiles = ExtensionFiles,
                            wav_COMMIT = new int[] { m_1, m_3, m_3 }
                        };
                        File.WriteAllText(fileDialog.FileName, _coding.Compress(JsonConvert.SerializeObject(f))); //加密压缩并输出
                        projName = new FileInfo(fileDialog.FileName).Name; SetFileShow(); //更新文件显示
                    };
                    worker.RunWorkerCompleted += (o, ea) =>
                    {
                        w.Close();
                    };
                    worker.RunWorkerAsync();
                }
                else
                {
                    //Waiting
                    var w = new SubWindow.Waiting(); w.Owner = this;
                    BackgroundWorker waiting = new BackgroundWorker();
                    waiting.DoWork += (ee, ea) => { };
                    waiting.RunWorkerCompleted += (ee, ea) =>
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            w.ShowDialog();
                        }));
                    };
                    waiting.RunWorkerAsync();
                    int m_1 = Int32.Parse(WavSetting.单刻频率采样数.Text), m_2 = Int32.Parse(WavSetting.单刻振幅采样数.Text), m_3 = Int32.Parse(WavSetting.采样周期.Text);
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    //Work
                    worker.DoWork += (o, ea) =>
                    {
                        InheritExpression.SetCompareLists(AppDomain.CurrentDomain.BaseDirectory + "config\\compare"); //设置匹配列表 *

                        //命令序列实例化
                        var commandLine = new CommandLine().Serialize(ConfirmTimeLine(m_1, m_2, m_3));
                        foreach (var ex in ExtensionFiles)
                        {
                            commandLine = commandLine.Combine(commandLine, ex.ToCommandLine());
                        }
                        if (Lrcpath != "") //添加歌词
                        {
                            var lrcLine = GetLyrics();
                            commandLine = commandLine.Combine(commandLine, lrcLine);
                        }
                        //导出
                        var exportSetting = new ExportSetting() { AlwaysActive = ExportSetting.AlwaysActive, AlwaysLoadEntities = ExportSetting.AlwaysLoadEntities, Direction = ExportSetting.Direction, Width = ExportSetting.Width, AutoTeleport = ExportSetting.AutoTeleport };
                        if (fileDialog.FilterIndex == 3) exportSetting.Type = ExportSetting.ExportType.WorldEdit; //For WorldEdit
                        else if (fileDialog.FilterIndex == 4) exportSetting.Type = ExportSetting.ExportType.WorldEdit_113; //For WorldEdit 1.13
                        new Schematic().ExportSchematic(commandLine, exportSetting, fileDialog.FileName);
                    };
                    worker.RunWorkerCompleted += (o, ea) =>
                    {
                        w.Close();
                    };
                    worker.RunWorkerAsync();
                }
            }
        }
        public static TimeLine ConfirmTimeLine(int frec, int volc, int circle)
        {
            TimeLine exportLine = new TimeLine().Serialize(Midipath, Wavepath, Rate, frec, volc, circle); //时间序列                                                                                                                                                                                    //时间序列写入 & 更新
            exportLine.InstrumentList = preTimeLine.InstrumentList;
            exportLine.TrackList = preTimeLine.TrackList;
            exportLine.LeftWaveSetting = preTimeLine.LeftWaveSetting;
            exportLine.RightWaveSetting = preTimeLine.RightWaveSetting;
            exportLine.UpdateByTrackList();
            exportLine.UpdateWave();
            exportLine.Param["MidiFileFormat"].Enable = false;
            exportLine.Param["AudioFileFormat"].Enable = false;
            exportLine.Param["TotalTicks"].Enable = false;
            exportLine.OutPutBPM = PublicSet.OBPM;
            exportLine.Param["MidiTracksCount"].Enable = PublicSet.TC;
            exportLine.Param["MidiDeltaTicksPerQuarterNote"].Enable = PublicSet.Q;
            exportLine.Sound_Stereo(PublicSet.ST - 1);
            return exportLine;
        }
        public void LoadFile(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "A2M Project|*.amproj";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == true && fileDialog.FileName != null && fileDialog.FileName != "")
            {
                var o = JsonConvert.DeserializeObject<FileOutPut>(_coding.Decompress(File.ReadAllText(fileDialog.FileName)));
                //relative or absolute or cancel
                var msg = MessageBox.Show("是否使用相对路径导入？", "使用相对路径导入", MessageBoxButton.YesNoCancel); if (msg == MessageBoxResult.Cancel) return;
                if (msg == MessageBoxResult.Yes)
                {
                    o.Midipath = (o.rMidipath != null) ? new Uri(new Uri(fileDialog.FileName.Replace(" ", "*20")), o.rMidipath).LocalPath.Replace("*20", " ") : "";
                    o.Wavepath = (o.rWavepath != null) ? new Uri(new Uri(fileDialog.FileName.Replace(" ", "*20")), o.rWavepath).LocalPath.Replace("*20", " ") : "";
                    o.Lrcpath = (o.rLrcpath != null) ? new Uri(new Uri(fileDialog.FileName.Replace(" ", "*20")), o.rLrcpath).LocalPath.Replace("*20", " ") : "";
                }
                var tracks = o.MidiTracks;
                var instruments = o.MidiInstruments;
                foreach (var T in tracks)
                {
                    foreach (var i in T.Instruments)
                    {
                        foreach (var _t in (from t in tracks where i.TracksUid.Contains(t.Uid) select t))
                        {
                            i.Tracks.Add(_t);
                        }
                    }
                } //Link
                foreach (var i in instruments)
                {
                    foreach (var _t in (from t in tracks where i.TracksUid.Contains(t.Uid) select t))
                    {
                        i.Tracks.Add(_t);
                    }
                } //Link
                if (Midipath == o.Midipath && preTimeLine.TrackList.Count != 0 && preTimeLine.InstrumentList.Count != 0) //Update preTimeLine if the same Midi
                    preTimeLine = UpdateMidiInspector(preTimeLine, new TimeLine() { InstrumentList = instruments, TrackList = tracks });
                else
                    preTimeLine = new TimeLine() { InstrumentList = instruments, TrackList = tracks };
                MidiSetting.TracksView.ItemsSource = preTimeLine.TrackList;
                MidiPath.Text = o.Midipath;
                Midipath = o.Midipath; oldMidi = o.Midipath;
                preTimeLine.OutPutBPM = o.PublicSetting.OBPM;
                preTimeLine.Param["MidiTracksCount"].Value = o.PublicSetting.TTC;
                preTimeLine.Param["MidiDeltaTicksPerQuarterNote"].Value = o.PublicSetting.TQ; //Midi
                WavSetting.采样周期.Text = (o.wav_COMMIT[0] < 1) ? "1" : o.wav_COMMIT[0].ToString();
                WavSetting.单刻频率采样数.Text = (o.wav_COMMIT[1] < 1) ? "1" : o.wav_COMMIT[1].ToString();
                WavSetting.单刻振幅采样数.Text = (o.wav_COMMIT[2] < 1) ? "1" : o.wav_COMMIT[2].ToString();
                preTimeLine.LeftWaveSetting = o.LeftWaveSetting;
                preTimeLine.RightWaveSetting = o.RightWaveSetting;
                WavSetting.ItemChanged();
                WavePath.Text = o.Wavepath;
                Wavepath = o.Wavepath; //Wav
                LyricMode.Tellraw = o.LyricMode.Tellraw;
                LyricMode.SubTitle = o.LyricMode.SubTitle;
                LyricMode.ActionBar = o.LyricMode.ActionBar;
                LyricMode.Title = o.LyricMode.Title;
                LrcPath.Text = o.Lrcpath;
                Lrcpath = o.Lrcpath; //Lrc
                LyricMode.LyricOutSet.color1 = o.LyricMode.LyricOutSetting.color1;
                LyricMode.LyricOutSet.color2 = o.LyricMode.LyricOutSetting.color2;
                LyricMode.LyricOutSet.repeat = o.LyricMode.LyricOutSetting.repeat;
                ExportSetting = o.ExportSetting; //Export
                PublicSetting.ItemChanged();
                PublicSetting.OBPM.IsChecked = o.PublicSetting.OBPM;
                PublicSetting.音符占刻.IsChecked = o.PublicSetting.Q;
                PublicSetting.音轨数.IsChecked = o.PublicSetting.TC;
                PublicSet.OBPM = o.PublicSetting.OBPM;
                PublicSet.Q = o.PublicSetting.Q;
                PublicSet.TC = o.PublicSetting.TC;
                PublicSet.ST = o.PublicSetting.ST; //Public
                Rate = o.Rate;
                preTick = o.preTick;
                Export.重设播放倍率.Text = Rate.ToString();
                Export.Update(); //Export
                if (Midipath != "" && new FileInfo(Midipath).Exists) //MidiPath
                {
                    MidiSetting.IsEnabled = true;
                    MidiSetting.ItemChanged();
                    cancel0.Visibility = Visibility.Visible;
                    PublicSetting.IsEnabled = true;
                    PublicSetting.MidiPlat.IsEnabled = true;
                    Export.IsEnabled = true;
                    //Export
                    var a = new TimeLine();
                    var w = new SubWindow.Waiting(); w.Owner = this;
                    BackgroundWorker waiting = new BackgroundWorker();
                    waiting.DoWork += (ee, ea) => { };
                    waiting.RunWorkerCompleted += (ee, ea) =>
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            w.ShowDialog();
                        }));
                    };
                    waiting.RunWorkerAsync();
                    //Work
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    worker.DoWork += (ee, ea) =>
                    {
                        a = new AudioStreamMidi().Serialize(MainWindow.Midipath, new TimeLine(), Rate);
                    };
                    worker.RunWorkerCompleted += (ee, ea) =>
                    {
                        w.Close();
                        //Caculate
                        Export.Midi刻长.Text = a.Param["TotalTicks"].Value.ToString() + " ticks";
                        var m = a.Param["TotalTicks"].Value / 1200;
                        var s = a.Param["TotalTicks"].Value % 1200 / 20;
                        Export.Midi时长.Text = m.ToString() + " : " + s.ToString();
                        preTimeLine.Param["TotalTicks"].Value = a.Param["TotalTicks"].Value;
                        //PublicSetting
                        PublicSetting.TTC.Text = preTimeLine.Param["MidiTracksCount"].Value.ToString();
                        PublicSetting.TQ.Text = preTimeLine.Param["MidiDeltaTicksPerQuarterNote"].Value.ToString();
                    };
                    worker.RunWorkerAsync();
                }
                else
                {
                    MidiSetting.IsEnabled = false;
                    cancel0.Visibility = Visibility.Hidden;
                }
                if (Wavepath != "" && new FileInfo(Wavepath).Exists) //WavePath
                {
                    WavSetting.IsEnabled = true;
                    WavSetting.ItemChanged();
                    cancel1.Visibility = Visibility.Visible;
                    PublicSetting.IsEnabled = true;
                    PublicSetting.WavePlat.IsEnabled = true;
                    Export.IsEnabled = true;
                }
                else
                {
                    WavSetting.IsEnabled = false;
                    cancel1.Visibility = Visibility.Hidden;
                    WavSetting.最大频率L.Text = "";
                    WavSetting.最大振幅L.Text = "";
                    WavSetting.最小频率L.Text = "";
                    WavSetting.最小振幅L.Text = "";
                    WavSetting.平均频率L.Text = "";
                    WavSetting.平均振幅L.Text = "";
                    WavSetting.最大频率R.Text = "";
                    WavSetting.最大振幅R.Text = "";
                    WavSetting.最小频率R.Text = "";
                    WavSetting.最小振幅R.Text = "";
                    WavSetting.平均频率R.Text = "";
                    WavSetting.平均振幅R.Text = "";
                    WavSetting.左声道.IsChecked = true;
                    WavSetting.当刻频率L.IsChecked = true;
                    WavSetting.当刻振幅L.IsChecked = true;
                    WavSetting.右声道.IsChecked = false;
                    WavSetting.当刻频率R.IsChecked = false;
                    WavSetting.当刻振幅R.IsChecked = false;
                }
                if (Lrcpath != "" && new FileInfo(Lrcpath).Exists) //LrcPath
                {
                    LrcSetting.IsEnabled = true;
                    LrcSetting.Update();
                    cancel2.Visibility = Visibility.Visible;
                    PublicSetting.IsEnabled = true;
                    Export.IsEnabled = true;
                }
                else
                {
                    LrcSetting.IsEnabled = false;
                    LrcSetting.使用Tellraw输出.IsChecked = false;
                    LrcSetting.使用Title输出.IsChecked = false;
                    cancel2.Visibility = Visibility.Hidden;
                }
                LrcSetting.UpdateCheckBox();
                ExtensionFiles = o.ExtensionFiles; //ExtensionFiles
                foreach (var ex in ExtensionFiles)
                {
                    ex.AbsToR(fileDialog.FileName);
                }

                load.IsEnabled = true;
                A2MSave.IsEnabled = true;
                projName = new FileInfo(fileDialog.FileName).Name;
                SetFileShow();  //更新文件显示
            }
        }
        //DataPack操作
        public static string DataPackPath = "";
        public static int DataPackMax = 65536;
        public static bool DataPackOrderByInstruments = false;
        public void SaveAsDatapack(object sender, MouseButtonEventArgs e)
        {
            var n = new SubWindow.DataPackOutPut(); n.Owner = this; var c = GetLyrics();
            n.frec = Int32.Parse(WavSetting.单刻频率采样数.Text); n.volc = Int32.Parse(WavSetting.单刻振幅采样数.Text); n.cycle = Int32.Parse(WavSetting.采样周期.Text); if (c != null && c.Keyframe.Count > 0) n.lrcs = c;
            n.Show();
        }
    }
    /// <summary>
    /// .amproj文件结构
    /// </summary>
    public class FileOutPut
    {
        public ObservableCollection<TimeLine.MidiSettingInspector> MidiTracks;
        public ObservableCollection<TimeLine.MidiSettingInspector> MidiInstruments;
        public TimeLine.WaveSettingInspector LeftWaveSetting;
        public TimeLine.WaveSettingInspector RightWaveSetting;
        public string Midipath = "";
        public Uri rMidipath;
        public string Wavepath = "";
        public Uri rWavepath;
        public string Lrcpath = "";
        public Uri rLrcpath;
        public double Rate = -1;
        public int preTick = 0;
        public ExportSetting ExportSetting;
        public _PublicSetting PublicSetting;
        public _LyricMode LyricMode;
        public class _PublicSetting
        {
            public bool Q = false;
            public bool TC = false;
            public bool OBPM = false;
            public int TQ = 0;
            public int TTC = 0;
            public int ST = 0;
        }
        public class _LyricMode
        {
            public bool Title = false;
            public bool SubTitle = false;
            public bool ActionBar = false;
            public bool Tellraw = false;
            public LyricOutSet LyricOutSetting = new LyricOutSet();
            public class LyricOutSet
            {
                public bool repeat;
                public string color1;
                public string color2;
            }
        }
        public List<ExtensionFile> ExtensionFiles = new List<ExtensionFile>(); //Extension
        public int[] wav_COMMIT = new int[] { 5, 5, 5 };
    }

    public class Json //Json for Tellraw or so
    {
        public List<Text> texts = new List<Text>();
        public class Text
        {
            public string text = "";
            public string color = "white";
        }
    }

    public class ExtensionFile
    {
        bool missing = false;
        public string IsMissing {
            get
            {
                if (Path != "" && new FileInfo(Path).Exists) missing = false;
                else missing = true;
                return missing == true ? "缺失" : "正常";
            }
        }
        public string Name
        {
            get
            {
                return new FileInfo(Path).Name;
            }
        }
        public string Path = "";
        public Uri rPath;
        public CommandLine ToCommandLine()
        {
            return JsonConvert.DeserializeObject<CommandLine>(_coding.Decompress(File.ReadAllText(Path)));
        }
        public void RToAbs(string ProjFileName) //When Reading
        {
            Path = (rPath != null) ? new Uri(new Uri(ProjFileName.Replace(" ", "*20")), rPath).LocalPath.Replace("*20", " ") : "";
        }
        public void AbsToR(string ProjFileName) //When Writing
        {
            rPath = (Path != "") ? new Uri(ProjFileName.Replace(" ", "*20")).MakeRelativeUri(new Uri(Path.Replace(" ", "*20"))) : null;
        }
    }

    public class MidiInfoTxt
    {
        public MidiInfoTxt(string fileName, string fileOut, double rate = -1)
        {
            var midi = new AudioStreamMidi().Serialize(fileName, new TimeLine(), rate);
            foreach (var n in midi.TickNodes)
            {
                if (n.CurrentTick > -1)
                    Lines.AppendLine("==== Tick : " + n.CurrentTick + " ====");
                foreach (var t in n.MidiTracks.Keys)
                {
                    var m_v = 0;
                    if (n.MidiTracks[t].Count > 0 && n.MidiTracks[t].First().Value.Count > 0) m_v = n.MidiTracks[t].First().Value[0].PlaySound.MandaVolume;
                    Lines.AppendLine("\t=-- Track : \"" + t + "\", Volume : " + m_v.ToString() + "%");
                    foreach (var i in n.MidiTracks[t].Keys)
                    {
                        Lines.AppendLine("\t\t--- Instrument : \"" + i + "\"");
                        foreach (var g in n.MidiTracks[t][i])
                        {
                            if (g.IsEvent == true) continue;
                            Lines.AppendLine("\t\t\t- P : " + g.Param["Pitch"].Value + "(" + returnNote(g.Param["Pitch"].Value) + ")" + "\t- V : " + g.Param["Velocity"].Value + "  \t- L : " + g.Param["MinecraftTickDuration"].Value + " ticks");
                        }
                    }
                }
            }
            File.WriteAllText(fileOut, Lines.ToString());
        }
        public StringBuilder Lines = new StringBuilder();

        private string returnNote(int pitch)
        {
            if (pitch > 0 && pitch < 128)
            {
                List<string> name_i = new List<string>();
                name_i.Add("C"); name_i.Add("C#"); name_i.Add("D"); name_i.Add("D#"); name_i.Add("E"); name_i.Add("F"); name_i.Add("F#"); name_i.Add("G"); name_i.Add("G#"); name_i.Add("A"); name_i.Add("A#"); name_i.Add("B");
                return name_i[pitch % 12] + (pitch/12).ToString();
            }
            return "NaN";
        }
    }
    public class MidiInfoExcel
    {
        public MidiInfoExcel(string fileName, string fileOut, double rate = -1)
        {
            var timeLine = new TimeLine();
            var midi = new AudioStreamMidi().Serialize(fileName, timeLine, rate); //MidiFile
            var midi_index = new Dictionary<string, int>();
            var excel = new ExcelPackage();
            //Midi
            for (int i = 1; i <= midi.TrackList.Count; i++)
            {
                midi_index.Add(midi.TrackList[i - 1].Name == "" ? "default" : midi.TrackList[i - 1].Name, 1/* Index */);
                var wks = excel.Workbook.Worksheets.Add(midi.TrackList[i-1].Name == "" ? "default" : midi.TrackList[i - 1].Name);
                wks.Cells[1, 1].Value = "起始时间";
                wks.Cells[1, 2].Value = "乐器";
                wks.Cells[1, 3].Value = "MIDI音高";
                wks.Cells[1, 4].Value = "音高";
                wks.Cells[1, 5].Value = "力度";
                wks.Cells[1, 6].Value = "持续时间";
                wks.Cells[1, 8].Value = "控制器音量";
                wks.Cells[1, 9].Value = "控制器相位";
                wks.Cells[1, 10].Value = "控制器BPM";
                wks.Cells[1, 12].Value = "小节";
                wks.Cells[1, 13].Value = "拍";
                wks.Cells[1, 14].Value = "频道";
                wks.Row(1).Height = 20;
                wks.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wks.Row(1).Style.Font.Bold = true;
            }

            foreach (var n in midi.TickNodes)
            {
                if (n.CurrentTick > -1)
                {
                    foreach (var t in n.MidiTracks.Keys)
                    {
                        foreach (var i in n.MidiTracks[t].Keys)
                        {
                            foreach (var g in n.MidiTracks[t][i])
                            {
                                if (g.IsEvent == true) //BPM Change
                                {
                                    foreach (var t1 in n.MidiTracks.Keys)
                                    {
                                        if (t1 == "none") continue; //BPM Event
                                        midi_index[t1 == "" ? "default" : t1]++;
                                        var wks1 = excel.Workbook.Worksheets[t1 == "" ? "default" : t1];
                                        wks1.Cells[midi_index[t1 == "" ? "default" : t1], 1].Value = n.CurrentTick;
                                        wks1.Cells[midi_index[t1 == "" ? "default" : t1], 10].Value = g.Param["BeatPerMinute"].Value.ToString();
                                    }
                                    continue;
                                }
                                midi_index[t == "" ? "default" : t]++;

                                var wks = excel.Workbook.Worksheets[t == "" ? "default" : t];
                                wks.Cells[midi_index[t == "" ? "default" : t], 1].Value = n.CurrentTick;
                                wks.Cells[midi_index[t == "" ? "default" : t], 2].Value = i;
                                wks.Cells[midi_index[t == "" ? "default" : t], 3].Value = g.Param["Pitch"].Value;
                                wks.Cells[midi_index[t == "" ? "default" : t], 4].Value = returnNote(g.Param["Pitch"].Value);
                                wks.Cells[midi_index[t == "" ? "default" : t], 5].Value = g.Param["Velocity"].Value;
                                wks.Cells[midi_index[t == "" ? "default" : t], 6].Value = g.Param["MinecraftTickDuration"].Value;
                                wks.Cells[midi_index[t == "" ? "default" : t], 8].Value = g.PlaySound.MandaVolume == -1 ? 100 : g.PlaySound.MandaVolume;
                                wks.Cells[midi_index[t == "" ? "default" : t], 9].Value = g.PlaySound.GetPan() == -1 ? 64 : g.PlaySound.GetPan();
                                wks.Cells[midi_index[t == "" ? "default" : t], 12].Value = g.Param["BarIndex"].Value;
                                wks.Cells[midi_index[t == "" ? "default" : t], 13].Value = g.Param["BeatDuration"].Value;
                                wks.Cells[midi_index[t == "" ? "default" : t], 14].Value = g.Param["Channel"].Value;
                            }
                        }
                    }
                }
            }
            //Export
            foreach (var t in midi.TrackList)
            {
                var k = t.Name;
                for (var m = 1; m <= 14; m++)
                    excel.Workbook.Worksheets[k == "" ? "default" : k].Column(m).AutoFit();
            }
            excel.SaveAs(new FileInfo(fileOut));
        }

        private string returnNote(int pitch)
        {
            if (pitch > 0 && pitch < 128)
            {
                List<string> name_i = new List<string>();
                name_i.Add("C"); name_i.Add("C#"); name_i.Add("D"); name_i.Add("D#"); name_i.Add("E"); name_i.Add("F"); name_i.Add("F#"); name_i.Add("G"); name_i.Add("G#"); name_i.Add("A"); name_i.Add("A#"); name_i.Add("B");
                return name_i[pitch % 12] + (pitch / 12).ToString();
            }
            return "NaN";
        }
    }

    public static class _coding
    {
        #region Compress
        public static string Compress(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            MemoryStream ms = new MemoryStream();
            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }

            ms.Position = 0;
            MemoryStream outStream = new MemoryStream();

            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            byte[] gzBuffer = new byte[compressed.Length + 4];
            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(gzBuffer);
        }
        public static string Decompress(string compressedText)
        {
            byte[] gzBuffer = Convert.FromBase64String(compressedText);
            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                byte[] buffer = new byte[msgLength];

                ms.Position = 0;
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
        #endregion
    }

    public class _Version
    {
        public string version = "A-1.5-2";
        public string download;
        public string log;
    }
}