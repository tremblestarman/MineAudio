using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Audio2Minecraft;
using Newtonsoft.Json;
using System.IO;

namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            InheritExpression.SetCompareLists(System.Windows.Forms.Application.StartupPath + "\\test");
            var a = new TimeLine().Serialize(MidiPath.Text, null, 180);
            a.Sound_ExtraDelay(1); //所有音轨的发生发声延长1tick
            a.Sound_StopSound(false);   //禁用/stopsound
            a.EnableWave(false);    //禁用Wave
            a.Sound_InheritExpression("%p");
            a.Sound_InheritExpression("", "MIDI Out #3"); //设定子表达式 ep: 45 -> 45c.75, MIDI Out #3 -> 0.37
            a.Sound_PercVolume(100);
            //设定音轨及其音色
            a.Sound_SoundName("45c", "45"); a.Sound_StopSound(true, "45"); a.Sound_PercVolume(170, "45"); a.Sound_ExtraDelay(3, "45");
            a.Sound_SoundName("25c", "25r"); a.Sound_StopSound(true, "25r"); a.Sound_PercVolume(175, "25r"); a.Sound_ExtraDelay(2, "25r");
            a.Sound_SoundName("52c", "52l"); a.Sound_StopSound(true, "52l"); a.Sound_PercVolume(165, "52l"); a.Sound_ExtraDelay(2, "25r");
            a.Sound_SoundName("33c", "33"); a.Sound_StopSound(true, "33"); a.Sound_PercVolume(125, "33");
            a.Sound_SoundName("11", "11");
            a.Sound_SoundName("12", "12");
            a.Sound_SoundName("56", "56"); a.Sound_PercVolume(150, "56");
            a.Sound_SoundName("3", "3"); a.Sound_PercVolume(120, "3");
            a.Sound_SoundName("50c", "50"); a.Sound_StopSound(true, "50"); a.Sound_PercVolume(160, "63");
            a.Sound_SoundName("63c", "63"); a.Sound_StopSound(true, "63"); a.Sound_PercVolume(60, "63"); //旋律
            a.Sound_SoundName("0.77", "MIDI Out #3", "Low Wood Block");
            a.Sound_SoundName("0.67", "MIDI Out #3", "High Agogo");
            a.Sound_SoundName("0.45", "MIDI Out #3", "Low Tom");
            a.Sound_SoundName("0.37", "MIDI Out #3", "Side Stick");
            a.Sound_SoundName("0.68", "MIDI Out #3", "Low Agogo");
            a.Sound_SoundName("0.31", "MIDI Out #3", "Drum 31");
            a.Sound_SoundName("0.49", "MIDI Out #3", "Crash Cymbal 1");
            a.Sound_SoundName("0.48", "MIDI Out #3", "Hi-Mid Tom");
            a.Sound_SoundName("0.47", "MIDI Out #3", "Low-Mid Tom");
            a.Sound_SoundName("0.36", "MIDI Out #3", "Bass Drum 1");
            a.Sound_SoundName("0.38", "MIDI Out #3", "Acoustic Snare"); a.Sound_PercVolume(75, "MIDI Out #3", "Acoustic Snare");
            a.Sound_SoundName("0.40", "MIDI Out #3", "Electric Snare"); a.Sound_PercVolume(75, "MIDI Out #3", "Electric Snare");
            a.Sound_SoundName("0.43", "MIDI Out #3", "High Floor Tom");
            a.Sound_SoundName("0.46", "MIDI Out #3", "Open Hi-Hat"); //节奏
            a.Sound_SoundName("$p", "p"); //关键帧标记

            a.EnableMidi(false); //禁用Midi
            a.EnableMidi(true, "", "", -1, "PlaySound"); //只启用Midi的/playsound

            a.Sound_Stereo(2); //双声道

            //生成CommandLine(命令序列)
            var b = new CommandLine().Serialize(a);
            b.Start.Clear(); b.End.Clear();
            //new Schematic().ExportSchematic(b, new ExportSetting() { AlwaysActive = true, AlwaysLoadEntities = false, Direction = 2, Width = 16, AutoTeleport = true }, @"C:\Users\Administrator\Desktop\1.schematic");
            var keyPoint = new List<int>();
            for(int i = 0; i < b.Keyframe.Count; i++)
            {
                var k = b.Keyframe[i];
                for (int g = 0; g < k.Commands.Count; g++)
                {
                    if (k.Commands[g].Contains("$p"))
                    {
                        var p = Regex.Match(k.Commands[g], @"(?<=\s\$p.)\d+(?=\s)").Value;
                        k.Commands[g] = "setblock ~ 64 ~ minecraft:wool " + p; //用方块显示关键帧
                        keyPoint.Add(i);
                    }
                }
            }
            for (int p = 0; p < keyPoint.Count; p++)
            {
                var gap = 0;
                if (p < keyPoint.Count - 1) gap = keyPoint[p + 1] - keyPoint[p];
                else gap = b.Keyframe.Count - keyPoint[p];

                b.Keyframe[keyPoint[p]].Commands.Add("tp @p ~ ~ " + (keyPoint[p] + 1).ToString()); //防止卡墙

                var v = vrate(gap - 1); //获取单刻积分(单刻速率)

                for (int l = 1; l < gap; l++) //写入积分
                {
                    b.Keyframe[keyPoint[p] + l].Commands.Add("tp @p ~ ~ ~" + v[l - 1]);
                }
            }
            b = Theme1(b);
            new Schematic().ExportSchematic(b, new ExportSetting() { AlwaysActive = false, AlwaysLoadEntities = false, Direction = 2, Width = 1, AutoTeleport = false }, @"C:\Users\Administrator\Desktop\steinsgate0.schematic");
        }

        private void MidiSelect(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Midi|*.mid";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == true) { MidiPath.Text = fileDialog.FileName; }
        }

        private void WaveSelect(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Wave|*.wav";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == true) { WavePath.Text = fileDialog.FileName; }
        }
        private void LrcSelect(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Lrc|*.lrc;*.amlrc;*.txt";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == true) { LrcPath.Text = fileDialog.FileName; }
        }

        private CommandLine Theme1(CommandLine cmdl)
        {
            for (int i = 0; i <= 105; i++)
            {
                cmdl.Keyframe[i].Commands.RemoveAt(cmdl.Keyframe[i].Commands.Count - 1);
            }
            cmdl.Keyframe[0].Commands.Add("tp @p 80.0 65 62");
            cmdl.Keyframe[6].Commands.Add("tp @p 80.0 65 63");
            cmdl.Keyframe[13].Commands.Add("tp @p 80.0 65 64");
            cmdl.Keyframe[26].Commands.Add("tp @p 80.0 65 65");
            cmdl.Keyframe[33].Commands.Add("tp @p 80.0 65 66");
            cmdl.Keyframe[40].Commands.Add("tp @p 80.0 65 67");
            cmdl.Keyframe[46].Commands.Add("tp @p 80.0 65 68");
            cmdl.Keyframe[53].Commands.Add("tp @p 80.0 65 69");
            cmdl.Keyframe[60].Commands.Add("tp @p 80.0 65 70");
            cmdl.Keyframe[66].Commands.Add("tp @p 80.0 65 71");
            cmdl.Keyframe[73].Commands.Add("tp @p 80.0 65 72");
            cmdl.Keyframe[80].Commands.Add("tp @p 80.0 65 73");
            cmdl.Keyframe[86].Commands.Add("tp @p 80.0 65 74");
            cmdl.Keyframe[88].Commands.Add("tp @p 80.0 65 86");
            for (int i = 90; i <= 100; i++)
            {
                cmdl.Keyframe[i].Commands.Add("tp @p ~ ~ ~-7");
            }
            for (int i = 101; i <= 104; i++)
            {
                cmdl.Keyframe[i].Commands.Add("tp @p ~ ~ ~-2");
            }
            cmdl.Keyframe[105].Commands.Add("tp @p 80.0 65 105");
            return cmdl;
        }

        private List<double> vrate(int l)
        {
            var v = new List<double>();
            //S(λl^2) = l => (λ/3)l^3 = l
            //a = 3λ
            var a = 1 / (double)(l * l);
            for (int i = 1; i <= l; i++)
            {
                v.Add(a * i * i * i - a * (i - 1) * (i - 1) * (i - 1));
            }
            return v;
        }
    }
}
