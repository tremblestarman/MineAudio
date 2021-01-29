using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.ComponentModel;
using MahApps.Metro.Controls;
using Audio2Minecraft;

namespace Audio2MinecraftUI.SubWindow
{
    /// <summary>
    /// DataPackOutPut.xaml 的交互逻辑
    /// </summary>
    public partial class DataPackOutPut : MetroWindow
    {
        public DataPackOutPut()
        {
            InitializeComponent();
        }
        private void MetroWindow_Initialized(object sender, EventArgs e)
        {
            OK.IsEnabled = false;
            if (MainWindow.DataPackPath != "") Path.Text = MainWindow.DataPackPath;
            if (MainWindow.DataPackOrderByInstruments) Switcher.Source = new BitmapImage(new Uri(@"\img\instrument_view.png", UriKind.Relative)); //If Selected Instrument
        }

        private void Select(object sender, MouseButtonEventArgs e)
        {
            var fb = new FolderBrowserDialog();
            fb.Description = "请选择datapacks文件夹";
            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(fb.SelectedPath) || new DirectoryInfo(fb.SelectedPath).Name != "datapacks")
                {
                    System.Windows.MessageBox.Show("所选文件夹不正确, 必须为datapacks文件夹", "提示");
                }
                else
                {
                    Path.Text = fb.SelectedPath;
                    MainWindow.DataPackPath = fb.SelectedPath;
                    var regex = new Regex(@"^[A-Za-z0-9_]+$");
                    if (regex.Match(Name1.Text).Success) OK.IsEnabled = true;
                }
            }
        }
        private int oldIndex = 0;
        private string oldText = String.Empty;
        private void Name_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            if (e.Key == Key.Back && Name1.Text.Length == 1)
            {
                Name1.Text = "";
            }
            if (e.Key == Key.Back && Name1.Text.Length == 0)
            {
                Name1.SetValue(TextBoxHelper.WatermarkProperty, "请输入数据包名称");
                OK.IsEnabled = false;
            }
            oldIndex = Name1.CaretIndex;
            oldText = Name1.Text;
        }
        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            var regex = new Regex(@"^[A-Za-z0-9_]+$");
            if (!regex.Match(Name1.Text).Success && Name1.Text != "")
            {
                Name1.TextChanged -= Name_TextChanged;
                Name1.Text = oldText;
                Name1.CaretIndex = oldIndex;
                Name1.TextChanged += Name_TextChanged;
                OK.IsEnabled = false;
            }
            else if (Path.Text != "") OK.IsEnabled = true;
        }
        private void Number_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            if (e.Key == Key.Back && Max.Text.Length == 1)
            {
                Max.Text = "";
            }
            if (e.Key == Key.Back && Max.Text.Length == 0)
            {
                Max.SetValue(TextBoxHelper.WatermarkProperty, "");
            }
            oldIndex = Max.CaretIndex;
            oldText = Max.Text;
            MainWindow.DataPackMax = Int32.Parse(Max.Text);
        }
        private void Number_TextChanged(object sender, TextChangedEventArgs e)
        {
            int val;
            if (!Int32.TryParse(Max.Text, out val) && Max.Text != "" || Max.Text == "-")
            {
                Max.TextChanged -= Number_TextChanged;
                Max.Text = oldText;
                Max.CaretIndex = oldIndex;
                Max.TextChanged += Number_TextChanged;
            }
        }

        private void Switch(object sender, MouseButtonEventArgs e)
        {
            MainWindow.DataPackOrderByInstruments = !MainWindow.DataPackOrderByInstruments;
            if (MainWindow.DataPackOrderByInstruments)
                Switcher.Source = new BitmapImage(new Uri(@"\img\instrument_view.png", UriKind.Relative));
            else
                Switcher.Source = new BitmapImage(new Uri(@"\img\track_view.png", UriKind.Relative));
        }
        private void Done(object sender, MouseButtonEventArgs e)
        {
            //Detect Directory
            var path = Path.Text + "\\" + Name1.Text;
            if (Directory.Exists(path))
            {
                var msg = System.Windows.MessageBox.Show("是否替换同名数据包", "替换数据包", MessageBoxButton.YesNoCancel); if (msg == MessageBoxResult.Cancel) return;
                if (msg == MessageBoxResult.Yes)
                    try { new DirectoryInfo(path).Delete(true); } catch { System.Windows.MessageBox.Show("文件夹被占用" + Environment.NewLine + "请从资源管理器中关闭此文件夹", "错误"); return; }
            }
            //Confirm TimeLine
            MainWindow.datapackName = Name1.Text;
            commandMax = Int32.Parse(Max.Text);

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
                int stage = 3;
                if (MainWindow.Midipath != "") stage++;
                if (MainWindow.Wavepath != "") stage++;
                if (lrcs != null) stage++;
                MainWindow.SetTotalProgressStage(3);
                exportLine = MainWindow.ConfirmTimeLine(frec, volc, cycle);
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                w.Close();
                if ((exportLine == null || exportLine.TickNodes.Count == 0) && (lrcs == null || lrcs.Keyframe.Count == 0))
                {
                    System.Windows.MessageBox.Show("输出序列为空", "提示"); return;
                }
                scoreboard = Guid.NewGuid().ToString("N").Substring(0, 8);
                DataPackWork();
            };
            worker.RunWorkerAsync();
            #endregion
        }
        #region DataPackGenerate
        private void DataPackWork()
        {
            //Waiting
            var w = new SubWindow.Waiting(); w.Owner = this;
            var waiting = new BackgroundWorker();
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
            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            bool error = false;
            worker.DoWork += (o, ea) =>
            {
                StartCommands.Clear(); ResetCommands.Clear();
                if (MainWindow.Midipath != "") sliceMidi(exportLine, !MainWindow.DataPackOrderByInstruments);
                if (MainWindow.Wavepath != "") sliceWav(exportLine);
                if (lrcs != null) sliceLrc();
                try { cutByMaximum(commandMax); } catch { Dispatcher.Invoke(() => { w.Close(); MainWindow.ResetProgressStage(); }); System.Windows.MessageBox.Show("问题可能由以下原因造成：\n1.导出序列不包含任何命令\n2.命令溢出，数量大于2,147,483,647", "分类错误"); error = true; }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                if (!error) try { GenerateDataPack(); } catch { Dispatcher.Invoke(() => { w.Close(); MainWindow.ResetProgressStage(); }); System.Windows.MessageBox.Show("问题可能由以下原因造成：\n1.目标文件被占用\n2.目标文件夹被占用", "导出错误"); return; }
                w.Close();
                this.Close();
                this.Owner.Focus();
            };
            worker.RunWorkerAsync();
        }
        private void GenerateDataPack()
        {
            var name = getPackName();
            var description = "{" + Environment.NewLine +
                              "    \"pack\":{" + Environment.NewLine +
                              "        \"pack_format\":0," + Environment.NewLine +
                              "        \"description\":\"Made by A2M.\"" + Environment.NewLine +
                              "    }" + Environment.NewLine +
                              "}";
            var path = Path.Text + "\\" + getPackName();
            path = setDirectoryCopy(path);
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(path + "\\data");
            File.WriteAllText(path + "\\pack.mcmeta", description); //Datapack initialization
            var runfunction = "";

            int currentProgress = 0;
            foreach (var space in DatapackSpaces)
            {
                var spaceName = Guid.NewGuid().ToString("N").Substring(0, 8);
                var spacePath = setDirectoryCopy(path + "\\data\\" + spaceName);
                Directory.CreateDirectory(spacePath);
                spacePath += "\\functions";
                Directory.CreateDirectory(spacePath);
                
                foreach (var function in space.mcfunctions)
                {
                    var functionName = Guid.NewGuid().ToString("N").Substring(0, 8);
                    File.WriteAllText(spacePath + "\\" + functionName + ".mcfunction", String.Join(Environment.NewLine, function.Value));
                    runfunction += "execute as @a[tag=" +scoreboard + ",scores={" +scoreboard + "=" + space.timeinfos[function.Key].Start + ".." + space.timeinfos[function.Key].End + "}] run function " + spaceName + ":" + functionName + Environment.NewLine; //Run Functions
                }
                MainWindow.SetStagedProgressBar(((double)currentProgress++) / DatapackSpaces.Count);
            }
            MainWindow.SetStagedProgressBar(1);
            ///Runtime :
            //Run
            runfunction += "scoreboard players add @a[tag=" +scoreboard + ",scores={" +scoreboard + "=.." + StreamLength + "}] " +scoreboard + " 1"; //Time Add
            Directory.CreateDirectory(path + "\\data\\" + name + "");
            Directory.CreateDirectory(path + "\\data\\" + name + "\\functions");
            File.WriteAllText(path + "\\data\\" + name + "\\functions\\run.mcfunction", runfunction);
            //Init
            var init = "scoreboard objectives add " +scoreboard + " dummy";
            File.WriteAllText(path + "\\data\\" + name + "\\functions\\init.mcfunction", init);

            ///Operations :
            //Start
            var startfunction = "tag @s add " +scoreboard + Environment.NewLine + "scoreboard players set @s " +scoreboard + " 0";
            foreach (var c in StartCommands) startfunction += Environment.NewLine + c;
            File.WriteAllText(path + "\\data\\" + name + "\\functions\\start.mcfunction", startfunction);
            //Reset
            var resetfunction = "tag @s remove " +scoreboard + Environment.NewLine + "scoreboard players reset @s " +scoreboard;
            foreach (var c in ResetCommands) resetfunction += Environment.NewLine + c;
            File.WriteAllText(path + "\\data\\" + name + "\\functions\\reset.mcfunction", resetfunction);
            //Pause
            var pausefunction = "tag @s remove " +scoreboard;
            File.WriteAllText(path + "\\data\\" + name + "\\functions\\pause.mcfunction", pausefunction);
            //Continue
            var continuefunction = "tag @s add " +scoreboard;
            File.WriteAllText(path + "\\data\\" + name + "\\functions\\continue.mcfunction", continuefunction);

            ///Tags :
            //tick & tick
            Directory.CreateDirectory(path + "\\data\\minecraft");
            Directory.CreateDirectory(path + "\\data\\minecraft\\tags");
            Directory.CreateDirectory(path + "\\data\\minecraft\\tags\\functions");
            var load =  "{" + Environment.NewLine +
                        "    \"values\": [" + Environment.NewLine +
                        "        \"" + name + ":init\"" + Environment.NewLine +
                        "    ]" + Environment.NewLine +
                        "}";
            var tick =  "{" + Environment.NewLine +
                        "    \"values\": [" + Environment.NewLine +
                        "        \"" + name + ":run\"" + Environment.NewLine +
                        "    ]" + Environment.NewLine +
                        "}";
            File.WriteAllText(path + "\\data\\minecraft\\tags\\functions\\load.json", load);
            File.WriteAllText(path + "\\data\\minecraft\\tags\\functions\\tick.json", tick);
            MainWindow.ResetProgressStage();
        }
        private string getPackName()
        {
            return MainWindow.datapackName;
        }
        private string setFileCopy(string path)
        {
            if (File.Exists(path))
                path = setFileCopy(path + "_");
            return path;
        }
        private string setDirectoryCopy(string path)
        {
            if (Directory.Exists(path))
                path = setDirectoryCopy(path + "_");
            return path;
        }
        #endregion
        public int frec, volc, cycle = 0;
        private TimeLine exportLine = new TimeLine(); public CommandLine lrcs = new CommandLine();
        public int commandMax = 65536;
        string scoreboard = "tick";

        #region DataPack
        /// <summary>
        /// DataPack命名空间
        /// </summary>
        public List<DataPackSpace> DatapackSpaces = new List<DataPackSpace>();
        public List<string> TrackEnabled = new List<string>();
        public List<string> InstrumentEnabled = new List<string>();
        public List<string> StartCommands = new List<string>();
        public List<string> ResetCommands = new List<string>();
        public int StreamLength = -1;
        private void sliceMidi(TimeLine timeLine, bool isTrack = true)
        {
            TrackEnabled = new List<string>(); InstrumentEnabled = new List<string>();
            CommandLine commandLine;
            if (isTrack) //Track
            {
                foreach (var track in timeLine.TrackList)
                {
                    commandLine = new CommandLine().SerializeSpecified(timeLine, track.Name, null, false, false, "1.13");
                    StartCommands = StartCommands.Union(commandLine.Start).ToList<string>(); ResetCommands = ResetCommands.Union(commandLine.End).ToList<string>();
                    DatapackSpaces.Add(new DataPackSpace() { Name = track.Name, mcfunctions = new Dictionary<string, List<string>>() { { track.Name, simplifyCommands(commandLine) } } }); //New Space for Track
                }
            }
            else //Instrument
            {
                foreach (var instrument in timeLine.InstrumentList) {
                    commandLine = new CommandLine().SerializeSpecified(timeLine, null, instrument.Name, false, false, "1.13");
                    StartCommands = StartCommands.Union(commandLine.Start).ToList<string>(); ResetCommands = ResetCommands.Union(commandLine.End).ToList<string>();
                    DatapackSpaces.Add(new DataPackSpace() { Name = instrument.Name, mcfunctions = new Dictionary<string, List<string>>() { { instrument.Name, simplifyCommands(commandLine) } } }); //New Space for Instrument
                }
            }
            if (exportLine.TickNodes.Count > StreamLength) StreamLength = exportLine.TickNodes.Count; //GetLength
            MainWindow.AddProgressStage();
            MainWindow.SetStagedProgressBar(0);
        }
        private void sliceWav(TimeLine timeLine)
        {
            CommandLine commandLine;
            if (timeLine.LeftWaveSetting.Enable == true)
            {
                commandLine = new CommandLine().SerializeSpecified(timeLine, null, null, true, false, "1.13");
                StartCommands = StartCommands.Union(commandLine.Start).ToList<string>(); ResetCommands = ResetCommands.Union(commandLine.End).ToList<string>();
                DatapackSpaces.Add(new DataPackSpace() { Name = "WaveLeftChannel", mcfunctions = new Dictionary<string, List<string>>() { { "left", simplifyCommands(commandLine) } } }); //New Space for WaveLeft
            }
            if (timeLine.RightWaveSetting.Enable == true)
            {
                commandLine = new CommandLine().SerializeSpecified(timeLine, null, null, false, true, "1.13");
                StartCommands = StartCommands.Union(commandLine.Start).ToList<string>(); ResetCommands = ResetCommands.Union(commandLine.End).ToList<string>();
                DatapackSpaces.Add(new DataPackSpace() { Name = "WaveRightChannel", mcfunctions = new Dictionary<string, List<string>>() { { "right", simplifyCommands(commandLine) } } }); //New Space for WaveRight
            }
            MainWindow.AddProgressStage();
            MainWindow.SetStagedProgressBar(0);
        }
        private void sliceLrc()
        {
            if (lrcs == null || lrcs.Keyframe.Count == 0) return;
            StartCommands = StartCommands.Union(lrcs.Start).ToList<string>(); ResetCommands = ResetCommands.Union(lrcs.End).ToList<string>();
            DatapackSpaces.Add(new DataPackSpace() { Name = "Lyrics", mcfunctions = new Dictionary<string, List<string>>() { { "lrc", simplifyCommands(lrcs) } } }); //New Space for Lrc
            if (lrcs.Keyframe.Count > StreamLength) StreamLength = lrcs.Keyframe.Count;
            MainWindow.AddProgressStage();
            MainWindow.SetStagedProgressBar(0);
        }

        private List<string> simplifyCommands(CommandLine commandLine)
        {
            var commands = new List<string>();
            for (var t = 0; t < commandLine.Keyframe.Count; t++)
            {
                var c = commandLine.Keyframe[t];
                for (var i = 0; i < c.Commands.Count; i++)
                {
                    var _ = c.Commands[i];
                    if (_.StartsWith("execute"))
                    {
                        var regex = "^execute as @[aesp]\\[";
                        var r = new Regex(regex).Match(_).Value;
                        if (r == "")
                        {
                            regex = "^execute as @[aesp]"; r = new Regex(regex).Match(_).Value;
                            commands.Add(new Regex(regex).Replace(_, "execute as @s[scores={" + scoreboard + "=" + t + "}]"));
                        } //without []
                        else commands.Add(new Regex(regex).Replace(_, "execute as @s[scores={" + scoreboard + "=" + t + "},"));
                    }
                    else if ((_.StartsWith("scoreboard") && !_.StartsWith("scoreboard players set @e[type=area_effect_cloud,tag=GenParam] CurrentTick")))
                        commands.Add("execute as @a[scores={" + scoreboard + "=" + t + "}] run " + _);
                    else if (_.StartsWith("title") || _.StartsWith("tellraw"))
                        commands.Add("execute as @a[scores={" + scoreboard + "=" + t + "}] run " + _);
                }
            }
            return commands;
        }//Cut MCFunction
        private void cutByMaximum(int commandMax) //Cut MCFunction
        {
            int currentProgress = 0;
            foreach (var c in DatapackSpaces)
            {
                var _adding = new Dictionary<string, List<string>>();
                var _c = c.mcfunctions.First();
                var commands = _c.Value;
                if (commands.Count < commandMax)
                {
                    c.timeinfos.Add(_c.Key, new DataPackSpace.TimeInfo() { Start = getTick(commands.First()), End = getTick(commands.Last()) });
                    continue;
                }

                var temp = 0;
                var newcmd = new List<string>();
                for (int i = 0; i < commands.Count; i++)
                {
                    newcmd.Add(commands[i]);
                    temp++;
                    if (temp == commandMax)
                    {
                        temp = 0;
                        _adding.Add(_c.Key + "_" + (i / commandMax), newcmd);
                        c.timeinfos.Add(_c.Key + "_" + (i / commandMax), new DataPackSpace.TimeInfo() { Start = getTick(newcmd.First()), End = getTick(newcmd.Last()) });
                        newcmd = new List<string>(); //Reset
                    }
                }
                c.mcfunctions = new Dictionary<string, List<string>>(); //Reset mcfunction
                foreach (var _cmdl in _adding) { c.mcfunctions.Add(_cmdl.Key, _cmdl.Value); } //Add ne
                MainWindow.SetStagedProgressBar(((double)currentProgress++) / DatapackSpaces.Count);
            }
            MainWindow.AddProgressStage();
        }
        private int getTick(string command)
        {
            var regex = "(?<=(execute as @[aesp]\\[scores=\\{" + scoreboard + "=))\\d+(?=\\})";
            var tick = -1;
            Int32.TryParse(new Regex(regex).Match(command).Value, out tick);
            return tick;
        }
        #endregion
        #region Schedule
        public bool Scheduled = false;//ongoing
        #endregion
    }

    public class DataPackSpace
    {
        public string Name = "none";
        public Dictionary<string, List<string>> mcfunctions = new Dictionary<string, List<string>>();
        public Dictionary<string, TimeInfo> timeinfos = new Dictionary<string, TimeInfo>();
        public class TimeInfo
        {
            public int Start = -1;
            public int End = -1;
        }
    }
}