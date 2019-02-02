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

namespace ExecutiveMidi.SubWindow
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
            Max.Text = MainWindow.DataPackMax.ToString();
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

            scoreboard = Guid.NewGuid().ToString("N").Substring(0, 8);
            DataPackWork();
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
                    w.ShowDialog();
                }));
            };
            waiting.RunWorkerAsync();
            //Work
            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (o, ea) =>
            {
                var space = new DataPackSpace();
                space.mcfunctions.Add(getPackName(), modifyCommands(MainWindow.cmdLine));
                DatapackSpaces.Add(space);
                cutByMaximum(commandMax);
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                GenerateDataPack();
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
            }
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
            File.WriteAllText(path + "\\data\\" + name + "\\functions\\start.mcfunction", startfunction);
            //Reset
            var resetfunction = "tag @s remove " +scoreboard + Environment.NewLine + "scoreboard players reset @s " +scoreboard;
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
        public static int commandMax = 65536;
        static string scoreboard = "tick";

        #region DataPack
        /// <summary>
        /// DataPack命名空间
        /// </summary>
        public static List<DataPackSpace> DatapackSpaces = new List<DataPackSpace>();
        public static List<string> TrackEnabled = new List<string>();
        public static List<string> InstrumentEnabled = new List<string>();
        public static int StreamLength = -1;

        private List<string> modifyCommands(CommandLine commandLine)
        {
            var commands = new List<string>();
            for (var t = 0; t < commandLine.Keyframe.Count; t++)
            {
                var c = commandLine.Keyframe[t];
                for (var i = 4; i < c.Commands.Count; i++)
                {
                    var _ = c.Commands[i];
                    if (_ != null && _ != "")
                        commands.Add(setTick(_, t));
                }
            }
            return commands;
        }//Cut MCFunction
        private static void cutByMaximum(int commandMax) //Cut MCFunction
        {
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
            }
        }
        private static string setTick(string command, int tick)
        {
            return "execute as @a[scores={" + scoreboard + "=" + tick + "}] run " + command;
        }
        private static int getTick(string command)
        {
            var regex = "(?<=(execute as @[aesp]\\[scores=\\{" + scoreboard + "=))\\d+(?=\\})";
            var tick = -1;
            Int32.TryParse(new Regex(regex).Match(command).Value, out tick);
            return tick;
        }
        #endregion
        #region Schedule
        public static bool Scheduled = false;//ongoing
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