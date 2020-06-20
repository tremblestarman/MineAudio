using System;
using System.Collections.Generic;
using Microsoft.Win32;
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
using System.ComponentModel;
using Audio2Minecraft;
using Newtonsoft.Json;
using System.IO;

namespace Audio2MinecraftUI.SubWindow
{
    /// <summary>
    /// Extension.xaml 的交互逻辑
    /// </summary>
    public partial class BeatList : MetroWindow
    {
        public List<SubWindow.BeatElement> beatElements = new List<SubWindow.BeatElement>();
        public BeatList(List<SubWindow.BeatElement> beatElements)
        {
            this.beatElements = beatElements;
            InitializeComponent();
        }
        private void MetroWindow_Initialized(object sender, EventArgs e)
        {
            ListView.ItemsSource = beatElements;
        }
        private void Export(object sender, MouseButtonEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "A2M Extended Content(*.amextension)|*.amextension";
            fileDialog.FilterIndex = 1;
            var commandLine = new CommandLine();
            if (fileDialog.ShowDialog() == true && fileDialog.FileName != null && fileDialog.FileName != "")
            {
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
            }
        }
        public CommandLine getCommandLine()
        {
            var commandLine = new CommandLine();
            var beatElement = beatElements.Last();
            if (beatElement == null) return commandLine;
            for (var i = 0; i <= beatElement.TickStart; i++)
            {
                commandLine.Keyframe.Add(new Command());
            }//Add Commands
            foreach(var b in beatElements)
            {
                commandLine.Keyframe[b.TickStart].Commands.Add("tickrate " + b.TPS);
            }
            return commandLine;
        }
    }

    public class BeatElement
    {
        private string _bpm;
        public string BPM { get { return _bpm; } set { _bpm = value; } }
        private int _tick_start;
        public int TickStart { get { return _tick_start; } set { _tick_start = value; } }
        private string _tps;
        public string TPS { get { return _tps; } set { _tps = value; } }
    }
}
