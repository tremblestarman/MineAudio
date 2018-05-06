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
            InheritExpression.SetCompareLists(System.Windows.Forms.Application.StartupPath + "\\test");
            var a = new TimeLine().Serialize(MidiPath.Text, null, 72);
            //所有音轨的发生发声延长3tick
            a.Sound_ExtraDelay(3);
            a.Sound_StopSound(false);   //禁用/stopsound
            a.EnableWave(false);    //禁用Wave
            a.Sound_InheritExpression("%p");
            //设定音轨及其音色
            for (var i = 1; i <= 8; i++) { a.Sound_SoundName("1", i.ToString()); a.Sound_StopSound(true, i.ToString()); } //钢琴
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
            new Schematic().ExportSchematic(b, new ExportSetting() { AlwaysActive = true, AlwaysLoadEntities = false, Direction = 0, Width = 5 }, "E:\\timet.schematic");
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
    }
}
