using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
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
            //生成TimeLine(时间序列)
            var a = new TimeLine().Serialize(MidiPath.Text, WavePath.Text, 72);

            //所有音轨的发生发声延长3tick
            a.Sound_ExtraDelay(3);
            a.Sound_StopSound(false);   //禁用/stopsound
            a.EnableWave(false);    //禁用Wave
            //设定音轨及其音色
            for (var i = 1; i <= 8; i++) { a.Sound_SoundName("1", i.ToString());} //钢琴
            a.Sound_SoundName("74c", "74"); a.Sound_StopSound(true, "74"); a.Sound_PercVolume(90, "45");
            a.Sound_SoundName("45c", "45"); a.Sound_StopSound(true, "45"); a.Sound_PercVolume(175, "45"); a.Sound_ExtraDelay(4);
            a.Sound_SoundName("96c", "96"); a.Sound_StopSound(true, "96"); a.Sound_PercVolume(160, "45"); a.Sound_ExtraDelay(4);
            a.Sound_SoundName("52c", "52"); a.Sound_StopSound(true, "52"); a.Sound_PercVolume(160, "45"); a.Sound_ExtraDelay(4); //弦乐
            a.Sound_SoundName("0.86", "beat", "Drum 86");
            a.Sound_SoundName("0.40", "beat", "Electric Snare");
            a.Sound_SoundName("0.43", "beat", "High Floor Tom");
            a.Sound_SoundName("0.41", "beat", "Low Floor Tom");
            a.Sound_SoundName("0.69", "beat", "Cabasa");
            a.Sound_SoundName("0.44", "beat", "Pedal Hi-Hat");
            a.Sound_SoundName("0.67", "beat", "High Agogo");
            a.Sound_SoundName("0.45", "beat", "Low Tom");
            a.Sound_SoundName("0.57", "beat", "Crash Cymbal 2"); //设定音色

            a.EnableMidi(false); //禁用Midi
            a.EnableMidi(true, "", "", -1, "PlaySound"); //只启用Midi的/playsound

            //生成CommandLine(命令序列)
            var b = new CommandLine().Serialize(a);
            b.Start.Clear(); b.End.Clear();

            //生成聊天栏文本(命令序列)
            var text = textShow(a);
            for (int i = 0; i < 51; i++) b.Keyframe.Insert(0, new Command()); //插入18个tick用于'琴键'的下落
            b = b.Combine(b, text); //读取Text嵌入命令序列

            //每一刻都增加"tp @p ~0.2 ~ ~"的命令
            for (int i = 0; i < b.Keyframe.Count; i ++) b.Keyframe[i].Commands.Add("tp @p ~0.2 ~ ~");
            //生成schematic文件
            new Schematic().ExportSchematic(b, new ExportSetting() { AlwaysActive = true, AlwaysLoadEntities = false, Direction = 0, Width = 5 }, "E:\\time.schematic");
        }

        private void MidiSelect(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Midi|*.mid";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) { MidiPath.Text = fileDialog.FileName; }
        }

        private void WaveSelect(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Wave|*.wav";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) { WavePath.Text = fileDialog.FileName; }
        }
        private void LrcSelect(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Lrc|*.lrc;*.amlrc;*.txt";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) { LrcPath.Text = fileDialog.FileName; }
        }


        private CommandLine textShow(TimeLine a) //根据时间序列生成聊天栏文本
        {
            var text = new CommandLine(); //实例化聊天栏文本(命令序列)
            var wav = new AudioStreamWave().Serialize(WavePath.Text, new TimeLine(), 60, 60); //实例化Wav(时间序列)

            var timeD = new List<Pixel[,]>(); //实例化文本的二维平面

            //获取频率&音色的特征信息（平均值&最大值）
            /*var highest_fre = wav.TickNodes.Max(v => v.WaveNodesLeft.Max(h => h.Param["FrequencyPerTick"].Max(t => t.Value)));
            var highest_vol = wav.TickNodes.Max(v => v.WaveNodesLeft.Max(h => h.Param["VolumePerTick"].Max(t => t.Value)));
            var av_highest_fre = wav.TickNodes.Average(v => v.WaveNodesLeft.Max(h => h.Param["FrequencyPerTick"].Max(t => t.Value)));
            var av_highest_vol = wav.TickNodes.Average(v => v.WaveNodesLeft.Max(h => h.Param["VolumePerTick"].Max(t => t.Value)));
            var av_fre = (double)wav.TickNodes.Sum(v => v.WaveNodesLeft.Sum(h => h.Param["FrequencyPerTick"].Sum(t => t.Value))) / wav.TickNodes.Count / 160;
            var av_vol = (double)wav.TickNodes.Sum(v => v.WaveNodesLeft.Sum(h => h.Param["VolumePerTick"].Sum(t => t.Value))) / wav.TickNodes.Count / 160;
             => average: 77 - levle: 6*/

            for (int i = 0; i < wav.TickNodes.Count; i++) //创建文本平面的像素点(64*18)
            {
                timeD.Add(new Pixel[64, 18]);
                for (int x = 0; x < 63; x++)
                    for (int y = 0; y < 18; y++)
                    {
                        timeD[i][x, y] = new Pixel();
                    }
            }
            for (int i = 0; i < wav.TickNodes.Count; i++) //节奏可视化
            {
                if (a.TickNodes[i].MidiTracks.Count > 0)
                {
                    if (a.TickNodes[i].MidiTracks.ContainsKey("beat")) //遍历beat音轨下的乐器
                    {
                        if (a.TickNodes[i].MidiTracks["beat"].ContainsKey("Crash Cymbal 2")) //单面钹
                        {
                            for (int j = 0; j < 64; j++)
                            {
                                var delT = Math.Sqrt(64 * 64 - (j + 0.5 - 32) * (j + 0.5 - 32)); //延时
                                timeD = crash(timeD, i + (int)delT - 64, j, "dark_gray"); //生成动画
                            }
                        }
                        if (a.TickNodes[i].MidiTracks["beat"].ContainsKey("Low Tom") || a.TickNodes[i].MidiTracks["beat"].ContainsKey("Low Floor Tom") || a.TickNodes[i].MidiTracks["beat"].ContainsKey("High Floor Tom")) //低音鼓&低音落地鼓&高音落地鼓
                        {
                            timeD = beat(timeD, i, 0, "dark_aqua", "aqua"); //在x=0和x=63生成动画
                            timeD = beat(timeD, i, 63, "dark_aqua", "aqua");
                        }
                        if (a.TickNodes[i].MidiTracks["beat"].ContainsKey("Electric Snare")) //电子鼓
                        {
                            timeD = beat(timeD, i, 1, "dark_green", "green"); //在x=1和x=62生成动画
                            timeD = beat(timeD, i, 62, "dark_green", "green");
                        }
                    }
                }
            }

            for (int i = 0; i < wav.TickNodes.Count; i++) //波形可视化
            {
                var display = timeD[i];
                foreach (var node in wav.TickNodes[i].WaveNodesLeft)
                {
                    for (int j = 0; j < 64; j++)
                    {
                        if (node.Param["VolumePerTick"].Count == 60 && j > 1 && j < 62) //遍历x=2到x=61
                        {
                            var h = node.Param["VolumePerTick"][j - 2].Value / 18; //获取音量
                            if (h > 18) h = 18;

                            if (i == 0) h = h / 2;
                            else
                            {
                                var mh = 0;
                                for (int d = 0; d < 18; d++) if (timeD[i - 1][j, d].Color != "black") mh++;
                                h = (h + mh) / 2;
                            }  //获取上一刻该处的波形高度

                            var p = (double)(60 - Math.Abs(j - 30)) / 60 * 100;
                            h = (int)(h * p / 100); //按照x坐标得到该处的波形高度(中间高两边低)

                            for (int k = 0; k < h; k++)
                            {
                                var color = "dark_purple";
                                if (k == h - 1)
                                    color = "blue"; //最上层为蓝色
                                display[j, k] = new Pixel() { Color = color }; //设置像素
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < 54; i++) timeD.Insert(0, new Pixel[64, 18]); //插入18个tick用于'琴键'的下落
            //最高音: 97 , 最低音: 25
            for (int i = 0; i < a.TickNodes.Count; i++) //琴键可视化
            {
                if (a.TickNodes[i].MidiTracks.Count > 0)
                {
                    foreach (var track in a.TickNodes[i].MidiTracks.Keys)
                    {
                        var color = "white";
                        if (track == "2") color = "yellow";
                        if (track == "3") color = "gray";
                        if (track == "4") color = "yellow";
                        if (track == "5") color = "yellow";
                        if (track == "6") color = "red";
                        if (track == "7") color = "red";
                        if (track == "8") color = "red"; //设置各音轨的琴键颜色

                        foreach (string instrument in a.TickNodes[i].MidiTracks[track].Keys)
                        {
                            if (instrument == "Acoustic Grand") //遍历所有Acoustic Grand
                            {
                                var nodes = a.TickNodes[i].MidiTracks[track][instrument];
                                foreach (var node in nodes)
                                {
                                    var pitch = node.Param["Pitch"].Value; //获取音高
                                    var x = pitch - 31; if (x > 63 || x < 0) continue; //删除出现在平面外的琴键
                                    for (int s = 0; s < 3; s++) //琴键高度=3
                                    {
                                        for (int y = 0; y < 60; y++) //遍历x
                                        {
                                            var h = y / 3 + s - 3;
                                            if (h == 0 && s > 0)
                                                timeD[i + 60 - y][x, h] = new Pixel() { Char = "◙", Color = color }; //表示正在消失的琴键样式
                                            else if (h == 0 && s == 0)
                                                timeD[i + 60 - y][x, h] = new Pixel() { Char = "◘", Color = color }; //表示刚刚敲击的琴键样式
                                            else if (h < 18 && h >= 0)
                                                timeD[i + 60 - y][x, h] = new Pixel() { Color = color };
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var lrc = new Lrc().Serialize(LrcPath.Text); //获取.lrc的内容
            var Lrcs = textLrc(lrc, timeD.Count, 54); //.lrc -> Tellraw

            for (int i = 0; i < timeD.Count; i++) //像素点 -> Tellraw
            {
                var display = timeD[i];
                text.Keyframe.Add(new Command());
                var tlw = new Tellraw();
                for (int y = 17; y > -1; y--)
                {
                    var sumT = new Tellraw.Text();
                    var lastT = new Tellraw.Text();
                    for (int x = 0; x < 64; x++)
                    {
                        var t = new Tellraw.Text();
                        if (display[x, y] == null) t = new Tellraw.Text() { text = "▌", color = "black" };
                        else t = new Tellraw.Text() { text = display[x, y].Char, color = display[x, y].Color };

                        if (x == 0) lastT = t;

                        if (lastT.text == t.text && lastT.color == t.color)
                        {
                            sumT.text += t.text;
                            sumT.color = t.color;
                            if (x == 63) tlw.texts.Add(sumT);
                        }
                        else
                        {
                            tlw.texts.Add(sumT);
                            lastT = t;
                            sumT = t;
                            if (x == 63) tlw.texts.Add(t);
                        }
                    }
                    tlw.texts[tlw.texts.Count - 1].text += "\n";
                }
                if (Lrcs[i] != null) tlw.texts.AddRange(Lrcs[i].texts);
                else tlw.texts.Add(new Tellraw.Text() { text = "\n" });

                //播放进度
                var proBar = new Tellraw.Text[3];
                proBar[0] = new Tellraw.Text() { color = "dark_purple" };
                proBar[1] = new Tellraw.Text() { text = "▪", color = "gold" };
                proBar[2] = new Tellraw.Text() { color = "white" };
                var did = (int)((double)i / timeD.Count * 94) + 1;
                for (int t = 0; t < did - 1; t++) proBar[0].text += "▪";
                for (int t = did; t < 94; t++) proBar[2].text += "▫";
                tlw.texts.Add(new Tellraw.Text() { text = " ▎▎ ", color = "white" });
                tlw.texts.AddRange(proBar);
                var m = (i / 1200 > 9) ? (i / 1200).ToString() : "0" + (i / 1200).ToString();
                var s = (i % 1200 / 20 > 9) ? (i % 1200 / 20).ToString() : "0" + (i % 1200 / 20).ToString();
                tlw.texts.Add(new Tellraw.Text() { text = " " + m + ":" + s, color = "white" });

                text.Keyframe[i].Commands.Add("tellraw @p " + JsonConvert.SerializeObject(tlw.texts)); //生成tellraw命令
            }
            return text;
        }

        private List<Pixel[,]> beat(List<Pixel[,]> timeD, int i, int j, string c1, string c2)
        {
            for (int m = 0; m < 1; m++) timeD[i - 3][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 6; m++) timeD[i - 2][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 11; m++) timeD[i - 1][j, m] = new Pixel() { Color = c2 };
            for (int m = 0; m < 16; m++) timeD[i][j, m] = new Pixel() { Color = c2 };
            for (int m = 0; m < 13; m++) timeD[i + 1][j, m] = new Pixel() { Color = c2 };
            for (int m = 0; m < 11; m++) timeD[i + 2][j, m] = new Pixel() { Color = c2 };
            for (int m = 0; m < 10; m++) timeD[i + 3][j, m] = new Pixel() { Color = c2 };
            for (int m = 0; m < 9; m++) timeD[i + 4][j, m] = new Pixel() { Color = c2 };
            for (int m = 0; m < 8; m++) timeD[i + 5][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 7; m++) timeD[i + 6][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 6; m++) timeD[i + 7][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 5; m++) timeD[i + 8][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 4; m++) timeD[i + 9][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 3; m++) timeD[i + 10][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 2; m++) timeD[i + 11][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 1; m++) timeD[i + 12][j, m] = new Pixel() { Color = c1 };
            return timeD;
        } //打击乐动画
        private List<Pixel[,]> crash(List<Pixel[,]> timeD, int i, int j, string c1)
        {
            for (int m = 0; m < 5; m++) timeD[i - 3][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 10; m++) timeD[i - 2][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 18; m++) timeD[i - 1][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 18; m++) timeD[i][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 18; m++) timeD[i + 1][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 16; m++) timeD[i + 2][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 14; m++) timeD[i + 3][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 13; m++) timeD[i + 4][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 12; m++) timeD[i + 5][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 11; m++) timeD[i + 6][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 10; m++) timeD[i + 7][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 9; m++) timeD[i + 8][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 8; m++) timeD[i + 9][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 7; m++) timeD[i + 10][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 6; m++) timeD[i + 11][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 5; m++) timeD[i + 12][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 4; m++) timeD[i + 13][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 3; m++) timeD[i + 14][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 2; m++) timeD[i + 15][j, m] = new Pixel() { Color = c1 };
            for (int m = 0; m < 1; m++) timeD[i + 16][j, m] = new Pixel() { Color = c1 };
            return timeD;
        } //单面钹动画

        private Tellraw[] textLrc(Lrc lrc, int sum = 0, int delay = 0) //.lrc -> Tellraw
        {
            var textLrc = new Tellraw[sum];
            foreach(var l in lrc.Lrcs)
            {
                var start = l.Start;
                var duration = l.Duration;
                var length = l.Content.Length; if (length == 0) continue;
                var pTick = (double)duration / length;
                for (int i = 0; i < duration; i++)
                {
                    var index = start + i + delay;
                    var playedChar = (int)((i + 1) / pTick) + 1;

                    if (playedChar >= length) playedChar = length;
                    var hltext = l.Content.Substring(0, playedChar);
                    var ntext = l.Content.Substring(playedChar);

                    var tlw = new Tellraw();
                    tlw.texts = new List<Tellraw.Text>()
                    {
                        new Tellraw.Text() { text = " --  「", color = "dark_gray"},
                        new Tellraw.Text() { text = hltext.Replace("\u3000",@"  "), color = "yellow"},
                        new Tellraw.Text() { text = ntext.Replace("\u3000",@"  "), color = "gray"},
                        new Tellraw.Text() { text = "」\n", color = "dark_gray"}
                    };

                    textLrc[index] = tlw;
                }
            }
            return textLrc;
        }
    }

    public class Pixel //像素点
    {
        public string Char = "▌";
        public string Color = "black";
    }

    public class Tellraw //Tellraw
    {
        public List<Text> texts = new List<Text>();
        public class Text
        {
            public string text = "";
            public string color = "white";
        }
    }
}
