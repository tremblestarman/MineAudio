using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
    /// LrcSetting.xaml 的交互逻辑
    /// </summary>
    public partial class LrcSetting : UserControl
    {
        public LrcSetting()
        {
            InitializeComponent();
        }
        private void Elements_Click(object sender, RoutedEventArgs e)
        {
            Done.IsEnabled = true;
        } //选中/取消选中元素
        private void DoneChanges(object sender, RoutedEventArgs e)
        {
            Done.IsEnabled = false;
            MainWindow.LyricMode.Title = 使用Title输出.IsChecked == true;
            MainWindow.LyricMode.SubTitle = 使用SubTitle输出.IsChecked == true;
            MainWindow.LyricMode.ActionBar = 使用ActionBar输出.IsChecked == true;
            MainWindow.LyricMode.Tellraw = 使用Tellraw输出.IsChecked == true;
        } //确认修改

        public void Update()
        {
            var file = new FileInfo(MainWindow.Lrcpath);
            var lrcs = new List<_Lrc>();
            if (file.Extension == ".lrc")
            {
                使用Title输出.IsEnabled = true;
                使用Title输出.IsChecked = false;
                C1.Foreground = new SolidColorBrush(Colors.White);
                使用SubTitle输出.IsEnabled = true;
                使用SubTitle输出.IsChecked = false;
                C2.Foreground = new SolidColorBrush(Colors.White);
                使用ActionBar输出.IsEnabled = true;
                使用ActionBar输出.IsChecked = false;
                C3.Foreground = new SolidColorBrush(Colors.White);
                使用Tellraw输出.IsEnabled = true;
                使用Tellraw输出.IsChecked = false;
                C4.Foreground = new SolidColorBrush(Colors.White);
                var _l = new Lrc().Serialize(file.FullName);
                foreach (var n in _l.Lrcs)
                {
                    lrcs.Add(new _Lrc() { Content = n.Content, Time = n.Start + " t", appearT = n.Start});
                }
            }
            else if (file.Extension == ".amlrc")
            {
                使用Title输出.IsEnabled = false;
                使用Title输出.IsChecked = true;
                C1.Foreground = new SolidColorBrush(Colors.DarkGray);
                使用SubTitle输出.IsEnabled = false;
                使用SubTitle输出.IsChecked = true;
                C2.Foreground = new SolidColorBrush(Colors.DarkGray);
                使用ActionBar输出.IsEnabled = false;
                使用ActionBar输出.IsChecked = false;
                C3.Foreground = new SolidColorBrush(Colors.DarkGray);
                使用Tellraw输出.IsEnabled = false;
                使用Tellraw输出.IsChecked = false;
                C4.Foreground = new SolidColorBrush(Colors.DarkGray);
                var _l = new AMLrc().Serialize(file.FullName);
                foreach (var n in _l.Bars)
                {
                    var g = n.Main.Concat(n.Sub);
                    foreach (var _n in g)
                    {
                        lrcs.Add(new _Lrc() { Content = _n.Content, Time = _n.AppearTime + " t", appearT = _n.AppearTime });
                    }
                }
            }
            var _lrcs = from l in lrcs orderby l.appearT select l;
            歌词列表.ItemsSource = _lrcs;
            使用Title输出.IsChecked = MainWindow.LyricMode.Title;
            使用SubTitle输出.IsChecked = MainWindow.LyricMode.SubTitle;
            使用ActionBar输出.IsChecked = MainWindow.LyricMode.ActionBar;
            使用Tellraw输出.IsChecked = MainWindow.LyricMode.Tellraw;
        }

        private class _Lrc
        {
            public int appearT;
            private string _time;
            public string Time { get { return _time; } set { _time = value; } }
            private string _content;
            public string Content { get { return _content; } set { _content = value; } }
        }

        public void UpdateCheckBox()
        {
            使用Title输出.IsChecked = MainWindow.LyricMode.Title;
            使用SubTitle输出.IsChecked = MainWindow.LyricMode.SubTitle;
            使用ActionBar输出.IsChecked = MainWindow.LyricMode.ActionBar;
            使用Tellraw输出.IsChecked = MainWindow.LyricMode.Tellraw;
        }
    }
}
