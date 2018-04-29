using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Audio2Minecraft;

namespace Audio2MinecraftUI.Humberger
{
    /// <summary>
    /// Export.xaml 的交互逻辑
    /// </summary>
    public partial class Export : UserControl
    {
        public Export()
        {
            InitializeComponent();
        }
        private void UserControl_Initialized(object sender, EventArgs e)
        {
            延伸方向.Items.Add("东（x+）");
            延伸方向.Items.Add("西（x-）");
            延伸方向.Items.Add("南（z+）");
            延伸方向.Items.Add("北（z-）");
            延伸方向.SelectedIndex = 0;
            序列宽度.Text = "16";
            保持区块加载.IsChecked = true;
            保持区块加载.IsChecked = false;
            播放模式.Items.Add("同步更新");
            播放模式.Items.Add("单次输出");
            播放模式.SelectedIndex = 0;
            var colors = new List<string>() { "black", "dark_blue", "dark_green", "dark_aqua", "dark_red", "dark_purple", "gold", "gray", "dark_gray", "blue", "green", "aqua", "red", "light_purple", "yellow", "white" };
            已播放颜色.ItemsSource = colors;
            已播放颜色.SelectedIndex = 15;
            未播放颜色.ItemsSource = colors;
            未播放颜色.SelectedIndex = 15;
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
            else old_text = t.Text;
        }
        private void _KeyDown(object sender, KeyEventArgs e)
        {
            Done.IsEnabled = true;
        }
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
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

        private void DoneChanges(object sender, MouseButtonEventArgs e)
        {
            MainWindow.ExportSetting.Direction = 延伸方向.SelectedIndex;
            MainWindow.ExportSetting.Width = Int32.Parse(序列宽度.Text);
            MainWindow.ExportSetting.AlwaysActive = 保持区块加载.IsChecked == true;
            MainWindow.ExportSetting.AlwaysLoadEntities = 保持实体加载.IsChecked == true;
            MainWindow.preTimeLine.Param["MidiBeatPerMinute"].Value = Int32.Parse(重设BPM.Text);
            MainWindow.LyricMode.LyricOutSet.repeat = 播放模式.SelectedIndex == 0;
            MainWindow.LyricMode.LyricOutSet.color1 = 已播放颜色.Text;
            MainWindow.LyricMode.LyricOutSet.color2 = 未播放颜色.Text;
            if (MainWindow.Midipath != "" && MainWindow.BPM.ToString() != 重设BPM.Text)
            {
                var a = new AudioStreamMidi().Serialize(MainWindow.Midipath, new TimeLine(), Int32.Parse(重设BPM.Text));
                Midi刻长.Text = a.Param["TotalTicks"].Value.ToString() + " ticks";
                var m = a.Param["TotalTicks"].Value / 1200;
                var s = a.Param["TotalTicks"].Value % 1200 / 20;
                Midi时长.Text = m.ToString() + " : " + s.ToString();
                MainWindow.BPM = Int32.Parse(重设BPM.Text);
            }
            Done.IsEnabled = false;
        }

        public void Update()
        {
            延伸方向.SelectedIndex = MainWindow.ExportSetting.Direction;
            序列宽度.Text = MainWindow.ExportSetting.Width.ToString();
            保持区块加载.IsChecked = MainWindow.ExportSetting.AlwaysActive;
            保持实体加载.IsChecked = MainWindow.ExportSetting.AlwaysLoadEntities;
            if (MainWindow.Midipath != "")
            {
                Midi.IsEnabled = true;
                重设BPM.Text = MainWindow.preTimeLine.Param["MidiBeatPerMinute"].Value.ToString();
                MainWindow.BPM = MainWindow.preTimeLine.Param["MidiBeatPerMinute"].Value;
                Midi刻长.Text = MainWindow.preTimeLine.Param["TotalTicks"].Value.ToString() + " ticks";
                var m = MainWindow.preTimeLine.Param["TotalTicks"].Value / 1200;
                var s = MainWindow.preTimeLine.Param["TotalTicks"].Value % 1200 / 20;
                Midi时长.Text = m.ToString() + " : " + s.ToString();
                MainWindow.BPM = MainWindow.preTimeLine.Param["MidiBeatPerMinute"].Value;
            }
            else
            {
                Midi.IsEnabled = false;
                重设BPM.Text = "1";
                Midi时长.Text = "";
                Midi刻长.Text = "";
            }

            if (MainWindow.Lrcpath != "")
            {
                Lrc.IsEnabled = true;
                播放模式.SelectedIndex = (MainWindow.LyricMode.LyricOutSet.repeat) ? 0 : 1;
                已播放颜色.Text = MainWindow.LyricMode.LyricOutSet.color1;
                未播放颜色.Text = MainWindow.LyricMode.LyricOutSet.color2;
            }
            else
            {
                Lrc.IsEnabled = false;
            }
            Done.IsEnabled = false;
        }
    }
}
