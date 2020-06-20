using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;
using Audio2Minecraft;
using ExecutiveMidi.SubWindow;

namespace ExecutiveMidi
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static TimeLine preTimeLine = new TimeLine(); //预览时间序列
        public static string Midipath = "", oldMidi = ""; //Midi路径
        class Emidiproj //.EmidiProj
        {
            public string Midipath = "";
            public Uri rMidipath;
            public double Rate = 1;
            public int SynchroTick = -1;
            public ObservableCollection<Humberger.MidiMarker> TrackMarkerList;
            public ObservableCollection<Humberger.MidiMarker> InstrumentMarkerList;
        }
        public static double Rate = 1;
        public static int SynchroTick = -1;
        public static ExportSetting export = new ExportSetting() { Width = 16 };
        public static string datapackName = "NewMusic"; //数据包名称
        public static MainWindow main;
        public static Export exportSetting;

        public MainWindow()
        {
            InitializeComponent();
            main = this;
            exportSetting = new SubWindow.Export();
        }
        private void MetroWindow_Initialized(object sender, EventArgs e)
        {
            save.IsEnabled = false;
            load.IsEnabled = false;
            export_setting.IsEnabled = false;
        }
        public static void SetProgressBar(double progress)
        {
            main.Dispatcher.Invoke(() =>
            {
                main.TaskbarItemInfo.ProgressValue = progress;
            });
        }
        private static double TotalProgressStage = 1;
        private static double CurrentProgressStage = 0;
        public static void SetTotalProgressStage(double totalStage)
        {
            TotalProgressStage = totalStage;
        }
        public static void ResetProgressStage()
        {
            TotalProgressStage = 1;
            CurrentProgressStage = 0;
            SetProgressBar(0);
        }
        public static void AddProgressStage()
        {
            CurrentProgressStage++;
        }
        public static void SetStagedProgressBar(double progress)
        {
            main.Dispatcher.Invoke(() =>
            {
                main.TaskbarItemInfo.ProgressValue = (CurrentProgressStage + progress) / TotalProgressStage;
            });

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
            if (MidiPath.Text == "")
            {
                load.IsEnabled = false;
                save.IsEnabled = false;
                export_setting.IsEnabled = false;
            }
            cancel0.Visibility = Visibility.Hidden;
            MidiSetting.TracksView.ItemsSource = null;
            MidiSetting.IsEnabled = false;
        }

        private void Load(object sender, MouseButtonEventArgs e)
        {
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
                        try { w.ShowDialog(); } catch { }
                    }));
                };
                waiting.RunWorkerAsync();
                //Work
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += (o, ea) =>
                {
                    if (oldMidi == m) preTimeLine = UpdateMidiInspector(new AudioStreamMidi().SerializeByRate(m, new TimeLine(), 1, SetProgressBar), preTimeLine);
                    else preTimeLine = new AudioStreamMidi().SerializeByRate(m, new TimeLine(), 1, SetProgressBar);
                };
                worker.RunWorkerCompleted += (o, ea) =>
                {
                    w.Close();
                    Midipath = MidiPath.Text;
                    MidiSetting.IsEnabled = true;
                    MidiSetting.ReadListView(preTimeLine);
                    MidiSetting.TracksView.ItemsSource = MidiSetting.TrackMarkerList; //Track
                    MidiSetting.Plat.IsEnabled = true;
                    MidiSetting.ItemChanged();

                    save.IsEnabled = true;
                    export_setting.IsEnabled = true;
                    oldMidi = MidiPath.Text;
                    SetProgressBar(0);

                    exportSetting.SwitchBeat((int)preTimeLine.TicksPerBeat);
                    exportSetting.SwitchRate(1);
                    exportSetting.Ok.IsEnabled = false;
                    exportSetting.Info2.Text = preTimeLine.Param["TotalTicks"].Value.ToString() + " ticks";
                    var _m = preTimeLine.Param["TotalTicks"].Value / 1200;
                    var s = preTimeLine.Param["TotalTicks"].Value % 1200 / 20;
                    exportSetting.Info1.Text = _m.ToString() + " : " + s.ToString();
                };
                worker.RunWorkerAsync();
            }
        }
        private void Save(object sender, MouseButtonEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "A2M Extended Content(*.amextension)|*.amextension|ExecutiveMidi Project(*.emidiproj)|*.emidiproj|Universal Schematic(*.schematic)|*.schematic|WorldEdit Schematic(*.schematic)|*.schematic|WorldEdit 1.13 Schematic(*.schem)|*.schem";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == true && fileDialog.FileName != null && fileDialog.FileName != "")
            {
                if (fileDialog.FilterIndex == 2) //Emidiproj
                {
                    var f = new Emidiproj()
                    {
                        Midipath = Midipath,
                        rMidipath = (Midipath != "") ? new Uri(fileDialog.FileName.Replace(" ", "*20")).MakeRelativeUri(new Uri(Midipath.Replace(" ", "*20"))) : null,
                        InstrumentMarkerList = MidiSetting.InstrumentMarkerList,
                        Rate = Rate,
                        SynchroTick = SynchroTick,
                        TrackMarkerList = MidiSetting.TrackMarkerList
                    };
                    File.WriteAllText(fileDialog.FileName, _coding.Compress(JsonConvert.SerializeObject(f))); //加密压缩并输出
                }
                else
                {
                    var commandLine = new CommandLine();
                    if (fileDialog.FilterIndex == 1) //A2Mextension
                    {
                        //Waiting...
                        var w = new SubWindow.Waiting(); w.Owner = this;
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
                        //Work
                        BackgroundWorker worker = new BackgroundWorker();
                        worker.WorkerReportsProgress = true;
                        worker.DoWork += (o, ea) =>
                        {
                            commandLine = getCommandLine();
                        };
                        worker.RunWorkerCompleted += (o, ea) =>
                        {
                            File.WriteAllText(fileDialog.FileName, _coding.Compress(JsonConvert.SerializeObject(commandLine))); //加密压缩并输出
                            w.Close();
                        };
                        worker.RunWorkerAsync();
                    }
                    else if (fileDialog.FilterIndex == 3 || fileDialog.FilterIndex == 4 || fileDialog.FilterIndex == 5) //Schematic
                    {
                        //Waiting...
                        var w = new SubWindow.Waiting(); w.Owner = this;
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
                        //Work
                        BackgroundWorker worker = new BackgroundWorker();
                        worker.WorkerReportsProgress = true;
                        worker.DoWork += (o, ea) =>
                        {
                            commandLine = getCommandLine();
                        };
                        worker.RunWorkerCompleted += (o, ea) =>
                        {
                            if (fileDialog.FilterIndex == 3) export.Type = ExportSetting.ExportType.Universal;
                            if (fileDialog.FilterIndex == 4) export.Type = ExportSetting.ExportType.WorldEdit;
                            if (fileDialog.FilterIndex == 5) export.Type = ExportSetting.ExportType.WorldEdit_113;
                            new Schematic().ExportSchematic(commandLine, export, fileDialog.FileName, SetProgressBar);
                            w.Close();
                        };
                        worker.RunWorkerAsync();
                    }
                }
            }
        }
        public CommandLine getCommandLine()
        {
            var commandLine = new CommandLine();
            for (var i = 0; i < preTimeLine.TickNodes.Count; i++)
            {
                commandLine.Keyframe.Add(new Command());
            }
            for (var i = 0; i < preTimeLine.TickNodes.Count; i++)
            {
                foreach (var t in preTimeLine.TickNodes[i].MidiTracks.Keys)
                {
                    var track = preTimeLine.TickNodes[i].MidiTracks[t];
                    foreach (var _i in track.Keys)
                    {
                        if (track[_i].First() != null && track[_i].First().IsEvent) continue; //When Found BPM
                        var instrument = track[_i];
                        var cmd = "";
                        var start = true;
                        var track_cmd = MidiSetting.TrackMarkerList.First(k => k.Name == t);
                        var instr_cmd = MidiSetting.InstrumentMarkerList.First(k => k.Name == _i);
                        if (track_cmd != null && track_cmd.Command != "")
                        {
                            cmd = track_cmd.Command;
                            start = track_cmd.Location == Humberger.MidiMarker.ExecuteLocation.Start;
                        }
                        if (instr_cmd != null && instr_cmd.Command != "")
                        {
                            cmd = instr_cmd.Command;
                            start = instr_cmd.Location == Humberger.MidiMarker.ExecuteLocation.Start;
                        }
                        if (cmd != "")
                        {
                            foreach (var node in instrument)
                            {
                                var cmds = cmd.Split(Environment.NewLine.ToCharArray());
                                var k = start ? i : i + node.Param["MinecraftTickDuration"].Value;
                                foreach (var c in cmds)
                                {
                                    commandLine.Keyframe[k].Commands.Add(
                                        MathCmd(
                                            InheritExpression.Expression(
                                                cmd,
                                                node.Param["Pitch"].Value,
                                                node.Param["MinecraftTickDuration"].Value,
                                                node.Param["Velocity"].Value,
                                                node.Param["BarIndex"].Value,
                                                node.Param["BeatDuration"].Value,
                                                node.Param["Channel"].Value
                                                )
                                        )
                                    );
                                }
                            }
                        }
                    }
                }
                SetProgressBar((double)(i + 1) / preTimeLine.TickNodes.Count);
            }
            SetProgressBar(0);
            return commandLine;
        }
        private void LoadFile(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "ExecutiveMidi Project(*.emidiproj)|*.emidiproj";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == true && fileDialog.FileName != null && fileDialog.FileName != "")
            {
                var o = JsonConvert.DeserializeObject<Emidiproj>(_coding.Decompress(File.ReadAllText(fileDialog.FileName)));
                //relative or absolute or cancel
                var msg = MessageBox.Show("是否使用相对路径导入？", "使用相对路径导入", MessageBoxButton.YesNoCancel); if (msg == MessageBoxResult.Cancel) return;
                if (msg == MessageBoxResult.Yes)
                {
                    o.Midipath = (o.rMidipath != null) ? new Uri(new Uri(fileDialog.FileName.Replace(" ", "*20")), o.rMidipath).LocalPath.Replace("*20", " ") : "";
                }
                if (o.Midipath != "" && new FileInfo(o.Midipath).Exists) //MidiPath
                {
                    Midipath = o.Midipath;
                    MidiPath.Text = o.Midipath;
                    MidiSetting.IsEnabled = true;
                    MidiSetting.ItemChanged();
                    cancel0.Visibility = Visibility.Visible;
                    Rate = o.Rate;
                    SynchroTick = o.SynchroTick;
                    var m = o.Midipath;
                    //Waiting...
                    var w = new SubWindow.Waiting(); w.Owner = this;
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
                    //Work
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    worker.DoWork += (ou, ea) =>
                    {
                        if (o.SynchroTick <= 0)
                        {
                            if (oldMidi == m) preTimeLine = UpdateMidiInspector(new AudioStreamMidi().SerializeByRate(m, new TimeLine(), o.Rate, SetProgressBar), preTimeLine);
                            else preTimeLine = new AudioStreamMidi().SerializeByRate(m, new TimeLine(), o.Rate, SetProgressBar);
                            SetProgressBar(0);
                        }
                        else
                        {
                            SetTotalProgressStage(2);
                            if (oldMidi == m) preTimeLine = UpdateMidiInspector(new AudioStreamMidi().SerializeByBeat(m, new TimeLine(), o.SynchroTick, SetProgressBar), preTimeLine);
                            else preTimeLine = new AudioStreamMidi().SerializeByBeat(m, new TimeLine(), o.SynchroTick, SetProgressBar);
                            AddProgressStage();
                            int last_bpm = -1;
                            for (int i = 0; i < preTimeLine.TickNodes.Count; i++)
                            {
                                if (preTimeLine.TickNodes[i].BPM >= 0 && preTimeLine.TickNodes[i].BPM != last_bpm) //BPM changed
                                {
                                    double tps = (double)preTimeLine.TickNodes[i].BPM * preTimeLine.TicksPerBeat / 60 / SynchroTick;
                                    exportSetting.beatElements.Add(new SubWindow.BeatElement()
                                    {
                                        BPM = preTimeLine.TickNodes[i].BPM.ToString(),
                                        TickStart = i,
                                        TPS = tps.ToString("0.0000")
                                    });
                                    last_bpm = preTimeLine.TickNodes[i].BPM;
                                }
                                SetStagedProgressBar((double)i / preTimeLine.TickNodes.Count);
                            }
                            ResetProgressStage();
                        }
                    };
                    worker.RunWorkerCompleted += (ou, ea) =>
                    {
                        MidiSetting.ReadListView(preTimeLine);
                        MidiSetting.UpdateListView(o.TrackMarkerList, o.InstrumentMarkerList);
                        w.Close();
                        MidiSetting.IsEnabled = true;
                        MidiSetting.Plat.IsEnabled = true;
                        MidiSetting.TracksView.ItemsSource = MidiSetting.TrackMarkerList;
                        MidiSetting.ItemChanged();
                        save.IsEnabled = true;
                        export_setting.IsEnabled = true;
                        if (SynchroTick > 0)
                        {
                            exportSetting.SwitchBeat(SynchroTick);
                            exportSetting.Info1.Text = (preTimeLine.SynchronousRate * 100).ToString("0.00") + "%";
                            var toolTip = new TextBlock();
                            toolTip.Text = "有 " + (int)((1 - preTimeLine.SynchronousRate) * preTimeLine.Param["TotalTicks"].Value) + " 个音符与原Midi不同步.\n序列总长度为 " + preTimeLine.TickNodes.Count + " .";
                            exportSetting.Info1.ToolTip = toolTip;
                        }
                        else
                        {
                            exportSetting.SwitchRate(Rate);
                            exportSetting.Info2.Text = preTimeLine.Param["TotalTicks"].Value.ToString() + " ticks";
                            var _m = preTimeLine.Param["TotalTicks"].Value / 1200;
                            var s = preTimeLine.Param["TotalTicks"].Value % 1200 / 20;
                            exportSetting.Info1.Text = _m.ToString() + " : " + s.ToString();
                            SynchroTick = -1;
                        }
                        exportSetting.Ok.IsEnabled = false;
                        oldMidi = MidiPath.Text;
                    };
                    worker.RunWorkerAsync();
                }
                else
                {
                    MidiSetting.IsEnabled = false;
                    cancel0.Visibility = Visibility.Hidden;
                }
            }
        }
        private void SetFileShow()
        {
            var midiName = (File.Exists(MidiPath.Text)) ? " Midi: \"" + new FileInfo(MidiPath.Text).Name + "\"" : "";
        }
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
        private void ShowExportSetting(object sender, MouseButtonEventArgs e)
        {
            exportSetting.Owner = this;
            exportSetting.markLine = preTimeLine;
            exportSetting.ShowDialog();
        }

        //DataPack操作
        public static string DataPackPath = "";
        public static int DataPackMax = 65536;
        public static bool DataPackOrderByInstruments = false;
        public static CommandLine cmdLine = new CommandLine();
        public void SaveAsDatapack(object sender, MouseButtonEventArgs e)
        {
            if (preTimeLine == null || preTimeLine.TickNodes.Count == 0) { MessageBox.Show("你还没有导入任何项目", "提示"); return; }
            #region TimeLineGenerate
            //Waiting
            var w = new SubWindow.Waiting(); w.Owner = this;
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
            //Work
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (o, ea) =>
            {
                cmdLine = getCommandLine();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                w.Close();
                var n = new SubWindow.DataPackOutPut(); n.Owner = this;
                n.ShowDialog();
            };
            worker.RunWorkerAsync();
            #endregion
        }

        public static bool closing = false;
        private void MetroWindow_Closing(object sender, CancelEventArgs e) => closing = true;
        #region Math
        private string mCos(string cmd)
        {
            //cos
            var cosL = new Regex(@"(?i)(?<=%cos\()((?<Open>\()|(?<-Open>\))|[^()]+)*(?(Open)(?!))(?=\))").Matches(cmd);
            if (cosL.Count == 0) return cmd;
            foreach (var m in cosL)
            {
                var _c = m as Match;
                double result = 0;
                var s = TryMathExpression(MathCmd(_c.Value), out result);
                result = Math.Cos(result);
                if (s)
                    cmd = cmd.Replace("%cos(" + _c.Value + ")", result.ToString("0.0000"));
            }
            return cmd;
        }
        private string mSin(string cmd)
        {
            //sin
            var sinL = new Regex(@"(?i)(?<=%sin\()((?<Open>\()|(?<-Open>\))|[^()]+)*(?(Open)(?!))(?=\))").Matches(cmd);
            if (sinL.Count == 0) return cmd;
            foreach (var m in sinL)
            {
                var _c = m as Match;
                double result = 0;
                var s = TryMathExpression(MathCmd(_c.Value), out result);
                result = Math.Sin(result);
                if (s)
                    cmd = cmd.Replace("%sin(" + _c.Value + ")", result.ToString("0.0000"));
            }
            return cmd;
        }
        private string mTan(string cmd)
        {
            //tan
            var tanL = new Regex(@"(?i)(?<=%tan\()((?<Open>\()|(?<-Open>\))|[^()]+)*(?(Open)(?!))(?=\))").Matches(cmd);
            if (tanL.Count == 0) return cmd;
            foreach (var m in tanL)
            {
                var _c = m as Match;
                double result = 0;
                var s = TryMathExpression(MathCmd(_c.Value), out result);
                result = Math.Tan(result);
                if (s)
                    cmd = cmd.Replace("%tan(" + _c.Value + ")", result.ToString("0.0000"));
            }
            return cmd;
        }
        private string mCosa(string cmd)
        {
            //cos
            var cosaL = new Regex(@"(?i)(?<=%cos\()((?<Open>\()|(?<-Open>\))|[^()]+)*(?(Open)(?!))(?=\))").Matches(cmd);
            if (cosaL.Count == 0) return cmd;
            foreach (var m in cosaL)
            {
                var _c = m as Match;
                double result = 0;
                var s = TryMathExpression(MathCmd(_c.Value), out result);
                result = Math.Cos(result * Math.PI / 180);
                if (s)
                    cmd = cmd.Replace("%cos(" + _c.Value + ")", result.ToString("0.0000"));
                else
                    cmd = MathCmd(cmd);
            }
            return cmd;
        }
        private string mSina(string cmd)
        {
            //sina
            var sinaL = new Regex(@"(?i)(?<=%sina\()((?<Open>\()|(?<-Open>\))|[^()]+)*(?(Open)(?!))(?=\))").Matches(cmd);
            if (sinaL.Count == 0) return cmd;
            foreach (var m in sinaL)
            {
                var _c = m as Match;
                double result = 0;
                var s = TryMathExpression(MathCmd(_c.Value), out result);
                result = Math.Sin(result * Math.PI / 180);
                if (s)
                    cmd = cmd.Replace("%sina(" + _c.Value + ")", result.ToString("0.0000"));
            }
            return cmd;
        }
        private string mTana(string cmd)
        {
            //tana
            var tanaL = new Regex(@"(?i)(?<=%tana\()((?<Open>\()|(?<-Open>\))|[^()]+)*(?(Open)(?!))(?=\))").Matches(cmd);
            if (tanaL.Count == 0) return cmd;
            foreach (var m in tanaL)
            {
                var _c = m as Match;
                double result = 0;
                var s = TryMathExpression(MathCmd(_c.Value), out result);
                result = Math.Tan(result * Math.PI / 180);
                if (s)
                    cmd = cmd.Replace("%tana(" + _c.Value + ")", result.ToString("0.0000"));
            }
            return cmd;
        }
        private string mCosh(string cmd)
        {
            //cosh
            var coshL = new Regex(@"(?i)(?<=%cosh\()((?<Open>\()|(?<-Open>\))|[^()]+)*(?(Open)(?!))(?=\))").Matches(cmd);
            if (coshL.Count == 0) return cmd;
            foreach (var m in coshL)
            {
                var _c = m as Match;
                double result = 0;
                var s = TryMathExpression(MathCmd(_c.Value), out result);
                result = Math.Cosh(result);
                if (s)
                    cmd = cmd.Replace("%cosh(" + _c.Value + ")", result.ToString("0.0000"));
            }
            return cmd;
        }
        private string mSinh(string cmd)
        {
            //sinh
            var sinhL = new Regex(@"(?i)(?<=%sinh\()((?<Open>\()|(?<-Open>\))|[^()]+)*(?(Open)(?!))(?=\))").Matches(cmd);
            if (sinhL.Count == 0) return cmd;
            foreach (var m in sinhL)
            {
                var _c = m as Match;
                double result = 0;
                var s = TryMathExpression(MathCmd(_c.Value), out result);
                result = Math.Sinh(result);
                if (s)
                    cmd = cmd.Replace("%sinh(" + _c.Value + ")", result.ToString("0.0000"));
            }
            return cmd;
        }
        private string mTanh(string cmd)
        {
            //tanh
            var tanhL = new Regex(@"(?i)(?<=%tanh\()((?<Open>\()|(?<-Open>\))|[^()]+)*(?(Open)(?!))(?=\))").Matches(cmd);
            if (tanhL.Count == 0) return cmd;
            foreach (var m in tanhL)
            {
                var _c = m as Match;
                double result = 0;
                var s = TryMathExpression(MathCmd(_c.Value), out result);
                result = Math.Tanh(result);
                if (s)
                    cmd = cmd.Replace("%tanh(" + _c.Value + ")", result.ToString("0.0000"));
            }
            return cmd;
        }
        private string mAbs(string cmd)
        {
            //abs
            var absL = new Regex(@"(?i)(?<=%abs\()((?<Open>\()|(?<-Open>\))|[^()]+)*(?(Open)(?!))(?=\))").Matches(cmd);
            if (absL.Count == 0) return cmd;
            foreach (var m in absL)
            {
                var _c = m as Match;
                double result = 0;
                var s = TryMathExpression(MathCmd(_c.Value), out result);
                result = Math.Abs(result);
                if (s)
                    cmd = cmd.Replace("%abs(" + _c.Value + ")", result.ToString("0.0000"));
            }
            return cmd;
        }
        private string mRound(string cmd)
        {
            //round
            var roundL = new Regex(@"(?i)(?<=%round\()((?<Open>\()|(?<-Open>\))|[^()]+)*(?(Open)(?!))(?=\))").Matches(cmd);
            if (roundL.Count == 0) return cmd;
            foreach (var m in roundL)
            {
                var _c = m as Match;
                double result = 0;
                var s = TryMathExpression(MathCmd(_c.Value), out result);
                result = Math.Round(result);
                if (s)
                    cmd = cmd.Replace("%round(" + _c.Value + ")", result.ToString("0.0000"));
            }
            return cmd;
        }
        private string mSqrt(string cmd)
        {
            //sqrt
            var sqrtL = new Regex(@"(?i)(?<=%sqrt\()((?<Open>\()|(?<-Open>\))|[^()]+)*(?(Open)(?!))(?=\))").Matches(cmd);
            if (sqrtL.Count == 0) return cmd;
            foreach (var m in sqrtL)
            {
                var _c = m as Match;
                double result = 0;
                var s = TryMathExpression(MathCmd(_c.Value), out result);
                result = Math.Sqrt(result);
                if (s)
                    cmd = cmd.Replace("%sqrt(" + _c.Value + ")", result.ToString("0.0000"));
            }
            return cmd;
        }
        private string mFloor(string cmd)
        {
            //sqrt
            var floorL = new Regex(@"(?i)(?<=%floor\()((?<Open>\()|(?<-Open>\))|[^()]+)*(?(Open)(?!))(?=\))").Matches(cmd);
            if (floorL.Count == 0) return cmd;
            foreach (var m in floorL)
            {
                var _c = m as Match;
                double result = 0;
                var s = TryMathExpression(MathCmd(_c.Value), out result);
                result = Math.Floor(result);
                if (s)
                    cmd = cmd.Replace("%floor(" + _c.Value + ")", result.ToString("0.0000"));
            }
            return cmd;
        }
        private string mCeil(string cmd)
        {
            //sqrt
            var ceilL = new Regex(@"(?i)(?<=%ceil\()((?<Open>\()|(?<-Open>\))|[^()]+)*(?(Open)(?!))(?=\))").Matches(cmd);
            if (ceilL.Count == 0) return cmd;
            foreach (var m in ceilL)
            {
                var _c = m as Match;
                double result = 0;
                var s = TryMathExpression(MathCmd(_c.Value), out result);
                result = Math.Ceiling(result);
                if (s)
                    cmd = cmd.Replace("%ceil(" + _c.Value + ")", result.ToString("0.0000"));
            }
            return cmd;
        }
        private string mLog(string cmd)
        {
            //sqrt
            var logL = new Regex(@"(?i)(?<=%log\()((?<Open>\()|(?<-Open>\))|[^()]+)*(?(Open)(?!))(?=\))").Matches(cmd);
            if (logL.Count == 0) return cmd;
            foreach (var m in logL)
            {
                var _c = m as Match;
                double result = 0;
                var s = TryMathExpression(MathCmd(_c.Value), out result);
                result = Math.Log(result);
                if (s)
                    cmd = cmd.Replace("%log(" + _c.Value + ")", result.ToString("0.0000"));
            }
            return cmd;
        }
        private string mLog10(string cmd)
        {
            //sqrt
            var log10L = new Regex(@"(?i)(?<=%log10\()((?<Open>\()|(?<-Open>\))|[^()]+)*(?(Open)(?!))(?=\))").Matches(cmd);
            if (log10L.Count == 0) return cmd;
            foreach (var m in log10L)
            {
                var _c = m as Match;
                double result = 0;
                var s = TryMathExpression(MathCmd(_c.Value), out result);
                result = Math.Log10(result);
                if (s)
                    cmd = cmd.Replace("%log10(" + _c.Value + ")", result.ToString("0.0000"));
            }
            return cmd;
        }
        private string mExp(string cmd)
        {
            //sqrt
            var expL = new Regex(@"(?i)(?<=%exp\()((?<Open>\()|(?<-Open>\))|[^()]+)*(?(Open)(?!))(?=\))").Matches(cmd);
            if (expL.Count == 0) return cmd;
            foreach (var m in expL)
            {
                var _c = m as Match;
                double result = 0;
                var s = TryMathExpression(MathCmd(_c.Value), out result);
                result = Math.Exp(result);
                if (s)
                    cmd = cmd.Replace("%exp(" + _c.Value + ")", result.ToString("0.0000"));
            }
            return cmd;
        }
        private string mN(string cmd)
        {
            //normal
            var nL = new Regex(@"(?i)(?<=%\()((?<Open>\()|(?<-Open>\))|[^()]+)*(?(Open)(?!))(?=\))").Matches(cmd);
            if (nL.Count == 0) return cmd;
            foreach (var m in nL)
            {
                var _c = m as Match;
                double result = 0;
                var s = TryMathExpression(MathCmd(_c.Value), out result);
                if (s)
                    cmd = cmd.Replace("%(" + _c.Value + ")", result.ToString("0.0000"));
            }
            return cmd;
        }

        private string MathCmd(string cmd)
        {
            //%pi
            cmd = cmd.Replace("%Pi", Math.PI.ToString());
            //other independent expression
            cmd = mCos(cmd);
            cmd = mSin(cmd);
            cmd = mTan(cmd);
            cmd = mCosa(cmd);
            cmd = mSina(cmd);
            cmd = mTana(cmd);
            cmd = mCosh(cmd);
            cmd = mSinh(cmd);
            cmd = mTanh(cmd);
            cmd = mAbs(cmd);
            cmd = mRound(cmd);
            cmd = mSqrt(cmd);
            cmd = mFloor(cmd);
            cmd = mCeil(cmd);
            cmd = mLog(cmd);
            cmd = mLog10(cmd);
            cmd = mExp(cmd);
            cmd = mN(cmd);

            return cmd;
        }
        private bool TryMathExpression(string expression, out double result)
        {
            try
            {
                object r = new DataTable().Compute(expression, "");
                return Double.TryParse(r.ToString(), out result);
            }
            catch { Double.TryParse("", out result); return false; }
        }
        #endregion

        #region Compress
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
        #endregion
    }
}