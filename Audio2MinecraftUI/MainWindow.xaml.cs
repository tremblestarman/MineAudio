using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
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
using NAudio.Midi;
using MahApps.Metro.Controls;
using Newtonsoft.Json;

namespace Audio2MinecraftUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static TimeLine preTimeLine = new TimeLine();
        private static List<UserControl> Controls;
        public static string Midipath = "";
        public static string Wavepath = "";
        public static string Lrcpath = "";
        public static int BPM = -1;
        public static ExportSetting ExportSetting = new ExportSetting()
        {
            Direction = 0,
            Width = 16,
            AlwaysActive = true,
            AlwaysLoadEntities = false,
            AutoTeleport = false
        };
        public static class PublicSet
        {
            static public bool BPM = false;
            static public bool Q = false;
            static public bool TC = false;
            static public int ST = 0;
        }
        public static class LyricMode
        {
            static public bool Title = false;
            static public bool SubTitle = false;
            static public bool ActionBar = false;
            static public bool Tellraw = false;
            public static class LyricOutSet
            {
                static public bool repeat = true;
                static public string color1 = "white";
                static public string color2 = "white";
            }
        }
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

        private void SetFileShow()
        {
            var midiName = (File.Exists(MidiPath.Text)) ? " Midi: \"" + new FileInfo(MidiPath.Text).Name + "\"" : "";
            var waveName = (File.Exists(WavePath.Text)) ? " Wave: \"" + new FileInfo(WavePath.Text).Name + "\"" : "";
            var lrcName = (File.Exists(LrcPath.Text)) ? " Lrc: \"" + new FileInfo(LrcPath.Text).Name + "\"" : "";
            FileShow.Content = (midiName == "" && waveName == "" && lrcName == "") ? "" : midiName + waveName + lrcName;
        }
        private void Load(object sender, MouseButtonEventArgs e)
        {
            PublicSetting.IsEnabled = true;
            Export.IsEnabled = true;
            if (MidiPath.Text != "" && new FileInfo(MidiPath.Text).Exists)
            {
                preTimeLine = UpdateMidiInspector(new AudioStreamMidi().Serialize(MidiPath.Text, new TimeLine()), preTimeLine);
                Midipath = MidiPath.Text;
                MidiSetting.IsEnabled = true;
                MidiSetting.TracksView.ItemsSource = preTimeLine.TrackList; //Track
                MidiSetting.ItemChanged();
                PublicSetting.MidiPlat.IsEnabled = true;

                PublicSetting.TBPM.Text = preTimeLine.Param["MidiBeatPerMinute"].Value.ToString();
                PublicSetting.TTC.Text = preTimeLine.Param["MidiTracksCount"].Value.ToString();
                PublicSetting.TQ.Text = preTimeLine.Param["MidiDeltaTicksPerQuarterNote"].Value.ToString();
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

        private TimeLine UpdateMidiInspector(TimeLine newTimeline, TimeLine baseTimeline)
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
            return baseTimeline;
        }

        private CommandLine GetLyrcics()
        {
            var lrc = new FileInfo(Lrcpath);
            if (lrc.Extension == ".amlrc")
            {
                return new AMLrc().Serialize(lrc.FullName).AMLrcLine;
            }
            else if (lrc.Extension == ".lrc")
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
                        if (LyricMode.Tellraw) { tlw.texts.Insert(0, new Json.Text() { text = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" }); textLrc.Keyframe[index].Commands.Add("tellraw @a " + JsonConvert.SerializeObject(tlw.texts)); }
                        if (LyricMode.Title) textLrc.Keyframe[index].Commands.Add("title @a title" + JsonConvert.SerializeObject(tlw.texts));
                        if (LyricMode.SubTitle) textLrc.Keyframe[index].Commands.Add("title @a subtitle" + JsonConvert.SerializeObject(tlw.texts));
                        if (LyricMode.ActionBar) textLrc.Keyframe[index].Commands.Add("title @a actionbar" + JsonConvert.SerializeObject(tlw.texts));
                    }
                }
            }
            return textLrc;
        }

        public void Save(object sender, MouseButtonEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "A2M Project(*.amproj)|*.amproj|Universal Schematic(*.schematic)|*.schematic|WorldEdit Schematic(*.schematic)|*.schematic";
            fileDialog.FilterIndex = 1;
            foreach (var t in preTimeLine.TrackList)
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
                    var f = new FileOutPut()
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
                        PublicSetting = new FileOutPut._PublicSetting()
                        {
                            BPM = PublicSet.BPM,
                            Q = PublicSet.Q,
                            TC = PublicSet.TC,
                            ST = PublicSet.ST,
                            TBPM = preTimeLine.Param["MidiBeatPerMinute"].Value,
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
                        wav_COMMIT = new int[] { Int32.Parse(WavSetting.采样周期.Text), Int32.Parse(WavSetting.单刻频率采样数.Text), Int32.Parse(WavSetting.单刻振幅采样数.Text) }
                    };
                    File.WriteAllText(fileDialog.FileName, Compress(JsonConvert.SerializeObject(f)));
                }
                else
                {
                    InheritExpression.SetCompareLists(AppDomain.CurrentDomain.BaseDirectory + "CompareList");
                    var exportLine = new TimeLine().Serialize(Midipath, Wavepath, BPM, Int32.Parse(WavSetting.单刻频率采样数.Text), Int32.Parse(WavSetting.单刻振幅采样数.Text), Int32.Parse(WavSetting.采样周期.Text));
                    exportLine.InstrumentList = preTimeLine.InstrumentList;
                    exportLine.TrackList = preTimeLine.TrackList;
                    exportLine.LeftWaveSetting = preTimeLine.LeftWaveSetting;
                    exportLine.RightWaveSetting = preTimeLine.RightWaveSetting;
                    exportLine.UpdateByTrackList();
                    exportLine.UpdateWave();
                    exportLine.Param["MidiFileFormat"].Enable = false;
                    exportLine.Param["AudioFileFormat"].Enable = false;
                    exportLine.Param["TotalTicks"].Enable = false;
                    exportLine.Param["MidiBeatPerMinute"].Enable = PublicSet.BPM;
                    exportLine.Param["MidiTracksCount"].Enable = PublicSet.TC;
                    exportLine.Param["MidiDeltaTicksPerQuarterNote"].Enable = PublicSet.Q;
                    exportLine.Sound_Stereo(PublicSet.ST - 1);
                    var commandLine = new CommandLine().Serialize(exportLine);
                    if (Lrcpath != "")
                    {
                        var lrcLine = GetLyrcics();
                        commandLine = commandLine.Combine(commandLine, lrcLine);
                    }
                    var exportSetting = new ExportSetting() { AlwaysActive = ExportSetting.AlwaysActive, AlwaysLoadEntities = ExportSetting.AlwaysLoadEntities, Direction = ExportSetting.Direction, Width = ExportSetting.Width, AutoTeleport = ExportSetting.AutoTeleport };
                    if (fileDialog.FilterIndex == 3) exportSetting.Type = ExportSetting.ExportType.WorldEdit; //For WE
                    new Schematic().ExportSchematic(commandLine, exportSetting, fileDialog.FileName);
                }
            }
        }
        public void LoadFile(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "A2M Project|*.amproj";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == true && fileDialog.FileName != null && fileDialog.FileName != "")
            {
                var o = JsonConvert.DeserializeObject<FileOutPut>(Decompress(File.ReadAllText(fileDialog.FileName)));
                //relative or absolute
                if (MessageBox.Show("是否使用相对路径导入？", "使用相对路径导入", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
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
                }
                foreach (var i in instruments)
                {
                    foreach (var _t in (from t in tracks where i.TracksUid.Contains(t.Uid) select t))
                    {
                        i.Tracks.Add(_t);
                    }
                }
                if (preTimeLine.TrackList.Count != 0 && preTimeLine.InstrumentList.Count != 0) //Update preTimeLine
                    preTimeLine = UpdateMidiInspector(preTimeLine, new TimeLine() { InstrumentList = instruments, TrackList = tracks });
                else
                    preTimeLine = UpdateMidiInspector(new TimeLine() { InstrumentList = instruments, TrackList = tracks }, preTimeLine);
                MidiSetting.TracksView.ItemsSource = preTimeLine.TrackList;
                MidiPath.Text = o.Midipath;
                Midipath = o.Midipath;
                preTimeLine.Param["MidiBeatPerMinute"].Value = o.PublicSetting.TBPM;
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
                PublicSetting.BPM.IsChecked = o.PublicSetting.BPM;
                PublicSetting.音符占刻.IsChecked = o.PublicSetting.Q;
                PublicSetting.音轨数.IsChecked = o.PublicSetting.TC;
                PublicSet.BPM = o.PublicSetting.BPM;
                PublicSet.Q = o.PublicSetting.Q;
                PublicSet.TC = o.PublicSetting.TC;
                PublicSet.ST = o.PublicSetting.ST; //Public
                BPM = o.BPM;
                Export.重设BPM.Text = BPM.ToString();
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
                    var a = new AudioStreamMidi().Serialize(MainWindow.Midipath, new TimeLine(), BPM);
                    Export.Midi刻长.Text = a.Param["TotalTicks"].Value.ToString() + " ticks";
                    var m = a.Param["TotalTicks"].Value / 1200;
                    var s = a.Param["TotalTicks"].Value % 1200 / 20;
                    Export.Midi时长.Text = m.ToString() + " : " + s.ToString();
                    preTimeLine.Param["MidiBeatPerMinute"].Value = a.Param["MidiBeatPerMinute"].Value;
                    preTimeLine.Param["TotalTicks"].Value = a.Param["TotalTicks"].Value;
                    //PublicSetting
                    PublicSetting.TBPM.Text = preTimeLine.Param["MidiBeatPerMinute"].Value.ToString();
                    PublicSetting.TTC.Text = preTimeLine.Param["MidiTracksCount"].Value.ToString();
                    PublicSetting.TQ.Text = preTimeLine.Param["MidiDeltaTicksPerQuarterNote"].Value.ToString();
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

                load.IsEnabled = true;
                A2MSave.IsEnabled = true;
                SetFileShow();
            }
        }
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
        public int BPM = -1;
        public ExportSetting ExportSetting;
        public _PublicSetting PublicSetting;
        public _LyricMode LyricMode;
        public class _PublicSetting
        {
            public bool BPM = false;
            public bool Q = false;
            public bool TC = false;
            public int TBPM = 0;
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
}