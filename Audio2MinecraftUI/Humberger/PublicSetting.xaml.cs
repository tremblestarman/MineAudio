using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MahApps.Metro.Controls;
using Audio2Minecraft;
using System.Globalization;

namespace Audio2MinecraftUI.Humberger
{
    /// <summary>
    /// PublicSetting.xaml 的交互逻辑
    /// </summary>
    public partial class PublicSetting : UserControl
    {
        #region Midi
        private List<TextBlock> TextBlockElements;
        private List<CheckBox> CheckElements;
        private List<TextBox> TextBoxElements;
        public PublicSetting()
        {
            InitializeComponent();
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            DataObject.AddPastingHandler(播放相对坐标X, new DataObjectPastingEventHandler(OnPaste));
            DataObject.AddPastingHandler(播放相对坐标Y, new DataObjectPastingEventHandler(OnPaste));
            DataObject.AddPastingHandler(播放相对坐标Z, new DataObjectPastingEventHandler(OnPaste));
            DataObject.AddPastingHandler(额外延时, new DataObjectPastingEventHandler(OnPaste));
            MidiPlat.IsEnabled = false;
            KeyScore.IsEnabled = false;
            PlaySound.IsEnabled = false;
            //Done.IsEnabled = false;

            TextBlockElements = new List<TextBlock>()
            {
                C1,
                C2,
                C3,
                C4,
                C5,
                C6,
                C7,
                C8,
                C9,
                音量大小
            };
            CheckElements = new List<CheckBox>()
            {
                计分板输出,
                键起始时间,
                键持续时间,
                小节索引,
                小节长度,
                键起始刻数,
                键持续刻数,
                频道,
                音高,
                力度,
                Playsound输出,
                Stopsound,
                音高播放
            };
            TextBoxElements = new List<TextBox>()
            {
                播放相对坐标X,
                播放相对坐标Y,
                播放相对坐标Z,
                相对玩家,
                播放对象,
                源,
                子表达式,
                额外延时
            };
            PlaySoundEnableUpdate();
            Done.IsEnabled = false;
        }
        public void MidiItemChanged()
        {
            Done.IsEnabled = false;

            KeyScore.IsEnabled = 计分板输出.IsChecked == true;
            PlaySound.IsEnabled = Playsound输出.IsChecked == true;
            PlaySoundEnableUpdate();

            var T = MainWindow.preTimeLine;
                foreach (var E in CheckElements)
                {

                    var Checked = CheckOrNot(T.TrackList, E.Uid);
                    if (E.Uid == "EnableScore") { 计分板输出.IsChecked = Checked; KeyScore.IsEnabled = Checked != false; }
                    if (E.Uid == "EnablePlaysound") { Playsound输出.IsChecked = Checked; PlaySound.IsEnabled = Checked != false; PlaySoundEnableUpdate(); }
                    if (E.Uid == "BarIndex") 小节索引.IsChecked = Checked;
                    if (E.Uid == "BeatDuration") 小节长度.IsChecked = Checked;
                    if (E.Uid == "Channel") 频道.IsChecked = Checked;
                    if (E.Uid == "DeltaTickDuration") 键持续时间.IsChecked = Checked;
                    if (E.Uid == "DeltaTickStart") 键起始时间.IsChecked = Checked;
                    if (E.Uid == "Velocity") 力度.IsChecked = Checked;
                    if (E.Uid == "Pitch") 音高.IsChecked = Checked;
                    if (E.Uid == "MinecraftTickDuration") 键持续刻数.IsChecked = Checked;
                    if (E.Uid == "MinecraftTickStart") 键起始刻数.IsChecked = Checked;
                    if (E.Uid == "StopSound") Stopsound.IsChecked = Checked;
                    if (E.Uid == "PitchPlayble") 音高播放.IsChecked = Checked;
                }
                TextSet(T.TrackList);
                ComboSet(T.TrackList);
                SlideSet(T.TrackList);
            } //Midi项目更新
        private bool? CheckOrNot(ObservableCollection<TimeLine.MidiSettingInspector> i, string element)
        {
            bool? ParentResult = false;
            if (element == "EnableScore")
            {
                if (i.All(t => t.Instruments.All(_i => _i.EnableScore == true))) ParentResult = true;
                else if (i.All(t => t.Instruments.All(_i => _i.EnableScore == false))) ParentResult = false;
                else ParentResult = null;
            }
            if (element == "EnablePlaysound")
            {
                if (i.All(t => t.Instruments.All(_i => _i.EnablePlaysound == true))) ParentResult = true;
                else if (i.All(t => t.Instruments.All(_i => _i.EnablePlaysound == false))) ParentResult = false;
                else ParentResult = null;
            }
            if (element == "BarIndex")
            {
                if (i.All(t => t.Instruments.All(_i => _i.BarIndex == true))) ParentResult = true;
                else if (i.All(t => t.Instruments.All(_i => _i.BarIndex == false))) ParentResult = false;
                else ParentResult = null;
            }
            if (element == "BeatDuration")
            {
                if (i.All(t => t.Instruments.All(_i => _i.BeatDuration == true))) ParentResult = true;
                else if (i.All(t => t.Instruments.All(_i => _i.BeatDuration == false))) ParentResult = false;
                else ParentResult = null;
            }
            if (element == "Channel")
            {
                if (i.All(t => t.Instruments.All(_i => _i.Channel == true))) ParentResult = true;
                else if (i.All(t => t.Instruments.All(_i => _i.Channel == false))) ParentResult = false;
                else ParentResult = null;
            }
            if (element == "DeltaTickDuration")
            {
                if (i.All(t => t.Instruments.All(_i => _i.DeltaTickDuration == true))) ParentResult = true;
                else if (i.All(t => t.Instruments.All(_i => _i.DeltaTickDuration == false))) ParentResult = false;
                else ParentResult = null;
            }
            if (element == "DeltaTickStart")
            {
                if (i.All(t => t.Instruments.All(_i => _i.DeltaTickStart == true))) ParentResult = true;
                else if (i.All(t => t.Instruments.All(_i => _i.DeltaTickStart == false))) ParentResult = false;
                else ParentResult = null;
            }
            if (element == "Pitch")
            {
                if (i.All(t => t.Instruments.All(_i => _i.Pitch == true))) ParentResult = true;
                else if (i.All(t => t.Instruments.All(_i => _i.Pitch == false))) ParentResult = false;
                else ParentResult = null;
            }
            if (element == "Velocity")
            {
                if (i.All(t => t.Instruments.All(_i => _i.Velocity == true))) ParentResult = true;
                else if (i.All(t => t.Instruments.All(_i => _i.Velocity == false))) ParentResult = false;
                else ParentResult = null;
            }
            if (element == "MinecraftTickDuration")
            {
                if (i.All(t => t.Instruments.All(_i => _i.MinecraftTickDuration == true))) ParentResult = true;
                else if (i.All(t => t.Instruments.All(_i => _i.MinecraftTickDuration == false))) ParentResult = false;
                else ParentResult = null;
            }
            if (element == "MinecraftTickStart")
            {
                if (i.All(t => t.Instruments.All(_i => _i.MinecraftTickStart == true))) ParentResult = true;
                else if (i.All(t => t.Instruments.All(_i => _i.MinecraftTickStart == false))) ParentResult = false;
                else ParentResult = null;
            }
            if (element == "StopSound")
            {
                if (i.All(t => t.Instruments.All(_i => _i.PlaysoundSetting.StopSound == true))) ParentResult = true;
                else if (i.All(t => t.Instruments.All(_i => _i.PlaysoundSetting.StopSound == false))) ParentResult = false;
                else ParentResult = null;
            }
            if (element == "PitchPlayable")
            {
                if (i.All(t => t.Instruments.All(_i => _i.PlaysoundSetting.PitchPlayable == true))) ParentResult = true;
                else if (i.All(t => t.Instruments.All(_i => _i.PlaysoundSetting.PitchPlayable == false))) ParentResult = false;
                else ParentResult = null;
            }
            return ParentResult;
        } //返回选项栏是否一致
        private void TextSet(ObservableCollection<TimeLine.MidiSettingInspector> i) //返回文本信息是否一致
        {
            播放相对坐标X.SetValue(TextBoxHelper.WatermarkProperty, "");
            播放相对坐标Y.SetValue(TextBoxHelper.WatermarkProperty, "");
            播放相对坐标Z.SetValue(TextBoxHelper.WatermarkProperty, "");
            相对玩家.SetValue(TextBoxHelper.WatermarkProperty, "");
            源.SetValue(TextBoxHelper.WatermarkProperty, "");
            子表达式.SetValue(TextBoxHelper.WatermarkProperty, "");
            额外延时.SetValue(TextBoxHelper.WatermarkProperty, "");
            foreach (var E in TextBoxElements)
            {
                if (E.Uid == "ExecuteCood1")
                {
                    if (i.All(t => t.Instruments.All(_i => _i.PlaysoundSetting.ExecuteCood[0] == i[0].Instruments[0].PlaysoundSetting.ExecuteCood[0]))) 播放相对坐标X.Text = i[0].Instruments[0].PlaysoundSetting.ExecuteCood[0].ToString();
                    else { 播放相对坐标X.Text = ""; 播放相对坐标X.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                }
                if (E.Uid == "ExecuteCood2")
                {
                    if (i.All(t => t.Instruments.All(_i => _i.PlaysoundSetting.ExecuteCood[1] == i[0].Instruments[0].PlaysoundSetting.ExecuteCood[1]))) 播放相对坐标Y.Text = i[0].Instruments[0].PlaysoundSetting.ExecuteCood[1].ToString();
                    else { 播放相对坐标Y.Text = ""; 播放相对坐标Y.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                }
                if (E.Uid == "ExecuteCood2")
                {
                    if (i.All(t => t.Instruments.All(_i => _i.PlaysoundSetting.ExecuteCood[2] == i[0].Instruments[0].PlaysoundSetting.ExecuteCood[2]))) 播放相对坐标Z.Text = i[0].Instruments[0].PlaysoundSetting.ExecuteCood[2].ToString();
                    else { 播放相对坐标Z.Text = ""; 播放相对坐标Z.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                }
                if (E.Uid == "ExecuteTarget")
                {
                    if (i.All(t => t.Instruments.All(_i => _i.PlaysoundSetting.ExecuteTarget == i[0].Instruments[0].PlaysoundSetting.ExecuteTarget))) 相对玩家.Text = i[0].Instruments[0].PlaysoundSetting.ExecuteTarget;
                    else { 相对玩家.Text = ""; 相对玩家.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                }
                if (E.Uid == "PlayTarget")
                {
                    if (i.All(t => t.Instruments.All(_i => _i.PlaysoundSetting.PlayTarget == i[0].Instruments[0].PlaysoundSetting.PlayTarget))) 播放对象.Text = i[0].Instruments[0].PlaysoundSetting.PlayTarget;
                    else { 播放对象.Text = ""; 播放对象.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                }
                if (E.Uid == "PlaySource")
                {
                    if (i.All(t => t.Instruments.All(_i => _i.PlaysoundSetting.PlaySource == i[0].Instruments[0].PlaysoundSetting.PlaySource))) 源.Text = i[0].Instruments[0].PlaysoundSetting.PlaySource;
                    else { 源.Text = ""; 源.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                }
                if (E.Uid == "InheritExpression")
                {
                    if (i.All(t => t.Instruments.All(_i => _i.PlaysoundSetting.InheritExpression == i[0].Instruments[0].PlaysoundSetting.InheritExpression))) 子表达式.Text = (i[0].Instruments[0].PlaysoundSetting.InheritExpression != null) ? i[0].Instruments[0].PlaysoundSetting.InheritExpression : "";
                    else { 子表达式.Text = ""; 子表达式.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                }
                if (E.Uid == "ExtraDelay")
                {
                    if (i.All(t => t.Instruments.All(_i => _i.PlaysoundSetting.ExtraDelay == i[0].Instruments[0].PlaysoundSetting.ExtraDelay))) 额外延时.Text = (i[0].Instruments[0].PlaysoundSetting.ExtraDelay > 0) ? i[0].Instruments[0].PlaysoundSetting.ExtraDelay.ToString() : "";
                    else { 额外延时.Text = ""; 额外延时.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                }
            }
        }
        private void ComboSet(ObservableCollection<TimeLine.MidiSettingInspector> i) //返回选项框是否一致
        {
            if (i.All(t => t.Instruments.All(_i => _i.PlaysoundSetting.SoundName == i[0].Instruments[0].PlaysoundSetting.SoundName))) 音色名称.Text = i[0].Instruments[0].PlaysoundSetting.SoundName;
            else { 音色名称.Text = ""; 音色名称.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
        }
        private void SlideSet(ObservableCollection<TimeLine.MidiSettingInspector> i) //返回滑动栏是否一致
        {
            if (i.All(t => t.Instruments.All(_i => _i.PlaysoundSetting.PercVolume == i[0].Instruments[0].PlaysoundSetting.PercVolume))) 音量增益.Value = (i[0].Instruments[0].PlaysoundSetting.PercVolume == -1) ? 50 : (i[0].Instruments[0].PlaysoundSetting.PercVolume > 200) ? 100 : (i[0].Instruments[0].PlaysoundSetting.PercVolume < 0) ? 0 : (double)i[0].Instruments[0].PlaysoundSetting.PercVolume / 2;
            else { 音量增益.Value = 50; 音量大小.Visibility = Visibility.Hidden; }
        }


        private void Elements_Click(object sender, RoutedEventArgs e)
        {
            var E = e.OriginalSource as CheckBox;
            if (E.Uid == "EnableScore") KeyScore.IsEnabled = (E.IsChecked == true);
            if (E.Uid == "EnablePlaysound") { PlaySound.IsEnabled = (E.IsChecked == true); PlaySoundEnableUpdate(); }
            Done.IsEnabled = true;
        } //选中/取消选中元素
        private void Update(TimeLine.MidiSettingInspector i)
        {
            if (计分板输出.IsChecked != null) i.EnableScore = 计分板输出.IsChecked == true;
            if (Playsound输出.IsChecked != null) i.EnablePlaysound = Playsound输出.IsChecked == true;
            if (小节索引.IsChecked != null) i.BarIndex = 小节索引.IsChecked == true;
            if (小节长度.IsChecked != null) i.BeatDuration = 小节长度.IsChecked == true;
            if (频道.IsChecked != null) i.Channel = 频道.IsChecked == true;
            if (键持续时间.IsChecked != null) i.DeltaTickDuration = 键持续时间.IsChecked == true;
            if (键起始时间.IsChecked != null) i.DeltaTickStart = 键起始时间.IsChecked == true;
            if (力度.IsChecked != null) i.Velocity = 力度.IsChecked == true;
            if (音高.IsChecked != null) i.Pitch = 音高.IsChecked == true;
            if (键持续刻数.IsChecked != null) i.MinecraftTickDuration = 键持续刻数.IsChecked == true;
            if (键起始刻数.IsChecked != null) i.MinecraftTickStart = 键起始刻数.IsChecked == true;
            if (Stopsound.IsChecked != null) i.PlaysoundSetting.StopSound = Stopsound.IsChecked == true;
            if (音高播放.IsChecked != null) i.PlaysoundSetting.PitchPlayable = 音高播放.IsChecked == true;
            if (播放相对坐标X.GetValue(TextBoxHelper.WatermarkProperty).ToString() == "") i.PlaysoundSetting.ExecuteCood[0] = (播放相对坐标X.Text != "") ? Double.Parse(播放相对坐标X.Text) : 0;
            if (播放相对坐标Y.GetValue(TextBoxHelper.WatermarkProperty).ToString() == "") i.PlaysoundSetting.ExecuteCood[1] = (播放相对坐标X.Text != "") ? Double.Parse(播放相对坐标Y.Text) : 0;
            if (播放相对坐标Z.GetValue(TextBoxHelper.WatermarkProperty).ToString() == "") i.PlaysoundSetting.ExecuteCood[2] = (播放相对坐标X.Text != "") ? Double.Parse(播放相对坐标Z.Text) : 0;
            if (相对玩家.GetValue(TextBoxHelper.WatermarkProperty).ToString() == "") i.PlaysoundSetting.ExecuteTarget = 相对玩家.Text;
            if (播放对象.GetValue(TextBoxHelper.WatermarkProperty).ToString() == "") i.PlaysoundSetting.PlayTarget = 播放对象.Text;
            if (源.GetValue(TextBoxHelper.WatermarkProperty).ToString() == "") i.PlaysoundSetting.PlaySource = 源.Text;
            if (子表达式.GetValue(TextBoxHelper.WatermarkProperty).ToString() == "") i.PlaysoundSetting.InheritExpression = (子表达式.Text != "") ? 子表达式.Text : null;
            if (额外延时.GetValue(TextBoxHelper.WatermarkProperty).ToString() == "") i.PlaysoundSetting.ExtraDelay = (额外延时.Text != "") ? Int32.Parse(额外延时.Text) : -1;
            if (音色名称.GetValue(TextBoxHelper.WatermarkProperty).ToString() == "") i.PlaysoundSetting.SoundName = 音色名称.Text;
            if (音量大小.Visibility == Visibility.Visible) i.PlaysoundSetting.PercVolume = (音量增益.Value != -1) ? (int)(音量增益.Value * 2) : -1;
        }

        private int oldIndex = 0;
        private string oldText = String.Empty;
        private void 音色名称_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Done.IsEnabled = true;
            if (音色名称.GetValue(TextBoxHelper.WatermarkProperty).ToString() != "")
            {
                音色名称.SetValue(TextBoxHelper.WatermarkProperty, "");
            }
        }
        private void 音色名称_TextInput(object sender, TextCompositionEventArgs e)
        {
            Done.IsEnabled = true;
            if (音色名称.GetValue(TextBoxHelper.WatermarkProperty).ToString() != "")
            {
                音色名称.SetValue(TextBoxHelper.WatermarkProperty, "");
            }
        }
        private void 音色名称_KeyBackDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back && 音色名称.Text.Length == 0)
            {
                音色名称.SetValue(TextBoxHelper.WatermarkProperty, "");
            }
        }
        private void TextChanging(object sender, TextChangedEventArgs e)
        {
            var T = e.OriginalSource as TextBox;
            if (T.GetValue(TextBoxHelper.WatermarkProperty).ToString() != "")
            {
                T.SetValue(TextBoxHelper.WatermarkProperty, "");
            }
            Done.IsEnabled = true;
        }
        private void KeyBackDown(object sender, KeyEventArgs e)
        {
            var T = e.OriginalSource as TextBox;
            if (e.Key == Key.Back && T.Text.Length == 0)
            {
                T.SetValue(TextBoxHelper.WatermarkProperty, "");
            }
        }
        private void 播放相对坐标X_TextChanged(object sender, TextChangedEventArgs e)
        {
            double val;
            if (!Double.TryParse(播放相对坐标X.Text, out val) && 播放相对坐标X.Text != "")
            {
                播放相对坐标X.TextChanged -= 播放相对坐标X_TextChanged;
                播放相对坐标X.Text = oldText;
                播放相对坐标X.CaretIndex = oldIndex;
                播放相对坐标X.TextChanged += 播放相对坐标X_TextChanged;
            }
            else Done.IsEnabled = true;
        }
        private void 播放相对坐标X_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            if (e.Key == Key.Back && 播放相对坐标X.Text.Length == 1)
            {
                播放相对坐标X.Text = "";
            }
            if (e.Key == Key.Back && 播放相对坐标X.Text.Length == 0)
            {
                播放相对坐标X.SetValue(TextBoxHelper.WatermarkProperty, "");
            }
            oldIndex = 播放相对坐标X.CaretIndex;
            oldText = 播放相对坐标X.Text;
        }
        private void 播放相对坐标Y_TextChanged(object sender, TextChangedEventArgs e)
        {
            double val;
            if (!Double.TryParse(播放相对坐标Y.Text, out val) && 播放相对坐标Y.Text != "")
            {
                播放相对坐标Y.TextChanged -= 播放相对坐标Y_TextChanged;
                播放相对坐标Y.Text = oldText;
                播放相对坐标Y.CaretIndex = oldIndex;
                播放相对坐标Y.TextChanged += 播放相对坐标Y_TextChanged;
            }
            else Done.IsEnabled = true;
        }
        private void 播放相对坐标Y_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            if (e.Key == Key.Back && 播放相对坐标Y.Text.Length == 1)
            {
                播放相对坐标Y.Text = "";
            }
            if (e.Key == Key.Back && 播放相对坐标Y.Text.Length == 0)
            {
                播放相对坐标Y.SetValue(TextBoxHelper.WatermarkProperty, "");
            }
            oldIndex = 播放相对坐标Y.CaretIndex;
            oldText = 播放相对坐标Y.Text;
        }
        private void 播放相对坐标Z_TextChanged(object sender, TextChangedEventArgs e)
        {
            double val;
            if (!Double.TryParse(播放相对坐标Z.Text, out val) && 播放相对坐标Z.Text != "")
            {
                播放相对坐标Z.TextChanged -= 播放相对坐标Z_TextChanged;
                播放相对坐标Z.Text = oldText;
                播放相对坐标Z.CaretIndex = oldIndex;
                播放相对坐标Z.TextChanged += 播放相对坐标Z_TextChanged;
            }
            else Done.IsEnabled = true;
        }
        private void 播放相对坐标Z_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            if (e.Key == Key.Back && 播放相对坐标Z.Text.Length == 1)
            {
                播放相对坐标Z.Text = "";
            }
            if (e.Key == Key.Back && 播放相对坐标Z.Text.Length == 0)
            {
                播放相对坐标Z.SetValue(TextBoxHelper.WatermarkProperty, "");
            }
            oldIndex = 播放相对坐标Z.CaretIndex;
            oldText = 播放相对坐标Z.Text;
        }
        private void 额外延时_TextChanged(object sender, TextChangedEventArgs e)
        {
            int val;
            if (!Int32.TryParse(额外延时.Text, out val) && 额外延时.Text != "" || 额外延时.Text == "-")
            {
                额外延时.TextChanged -= 额外延时_TextChanged;
                额外延时.Text = oldText;
                额外延时.CaretIndex = oldIndex;
                额外延时.TextChanged += 额外延时_TextChanged;
            }
            else Done.IsEnabled = true;
        }
        private void 额外延时_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.OemMinus)
            {
                e.Handled = true;
            }
            if (e.Key == Key.Back && 额外延时.Text.Length == 1)
            {
                额外延时.Text = "";
            }
            if (e.Key == Key.Back && 额外延时.Text.Length == 0)
            {
                额外延时.SetValue(TextBoxHelper.WatermarkProperty, "");
            }
            oldIndex = 额外延时.CaretIndex;
            oldText = 额外延时.Text;
        }
        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            double testVal;
            bool ok = false;

            var isText = e.SourceDataObject.GetDataPresent(System.Windows.DataFormats.Text, true);
            if (isText)
            {
                var text = e.SourceDataObject.GetData(DataFormats.Text) as string;
                if (Double.TryParse(text, out testVal))
                {
                    ok = true;
                }
            }

            if (!ok)
            {
                e.CancelCommand();
            }
        }

        public void PlaySoundEnableUpdate()
        {
            foreach (var e in TextBlockElements)
            {
                if (PlaySound.IsEnabled == true)
                    e.Foreground = new SolidColorBrush(Colors.White);
                else
                    e.Foreground = new SolidColorBrush(Colors.DarkGray);
            }
        } //更新字体颜色
        private void 音量增益_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Done.IsEnabled = true;
        }
        private void 音量增益_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (音量大小.Visibility == Visibility.Hidden)
                音量大小.Visibility = Visibility.Visible;
        }
        #endregion
        #region Wave
        public void WaveItemChanged()
        {
            var L = MainWindow.preTimeLine.LeftWaveSetting.Enable && MainWindow.preTimeLine.LeftWaveSetting.Frequency && MainWindow.preTimeLine.LeftWaveSetting.Volume;
            var LM = MainWindow.preTimeLine.LeftWaveSetting.Enable || MainWindow.preTimeLine.LeftWaveSetting.Frequency || MainWindow.preTimeLine.LeftWaveSetting.Volume;
            var R = MainWindow.preTimeLine.RightWaveSetting.Enable && MainWindow.preTimeLine.RightWaveSetting.Frequency && MainWindow.preTimeLine.RightWaveSetting.Volume;
            var RM = MainWindow.preTimeLine.RightWaveSetting.Enable || MainWindow.preTimeLine.RightWaveSetting.Frequency || MainWindow.preTimeLine.RightWaveSetting.Volume;
            if (L) 左声道.IsChecked = true; else if (LM && MainWindow.preTimeLine.LeftWaveSetting.Enable) 左声道.IsChecked = null; else 左声道.IsChecked = false;
            if (R) 右声道.IsChecked = true; else if (RM && MainWindow.preTimeLine.RightWaveSetting.Enable) 右声道.IsChecked = null; else 右声道.IsChecked = false;
        }
        #endregion

        private void DoneChanges(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("这将覆盖所有项目，你确定吗？", "确认全局设置", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                foreach (var t in MainWindow.preTimeLine.TrackList)
                {
                    foreach (var i in t.Instruments)
                    {
                        Update(i);
                    }
                }
                MainWindow.preTimeLine.LeftWaveSetting.Enable = 左声道.IsChecked == true;
                MainWindow.preTimeLine.LeftWaveSetting.Frequency = 左声道.IsChecked == true;
                MainWindow.preTimeLine.LeftWaveSetting.Volume = 左声道.IsChecked == true;
                MainWindow.preTimeLine.RightWaveSetting.Enable = 右声道.IsChecked == true;
                MainWindow.preTimeLine.RightWaveSetting.Frequency = 右声道.IsChecked == true;
                MainWindow.preTimeLine.RightWaveSetting.Volume = 右声道.IsChecked == true;
            }
            Done.IsEnabled = false;
        } //确认修改
        public void ItemChanged()
        {
            if (MidiPlat.IsEnabled == true)
                MidiItemChanged();
            if (WavePlat.IsEnabled == true)
                WaveItemChanged();
            BPM.IsChecked = MainWindow.PublicSet.BPM;
            音符占刻.IsChecked = MainWindow.PublicSet.Q;
            音轨数.IsChecked = MainWindow.PublicSet.TC;
            TBPM.Text = MainWindow.preTimeLine.Param["MidiBeatPerMinute"].Value.ToString();
            TTC.Text = MainWindow.preTimeLine.Param["MidiTracksCount"].Value.ToString();
            TQ.Text = MainWindow.preTimeLine.Param["MidiDeltaTicksPerQuarterNote"].Value.ToString();
        }

        private void PublicElements_Click(object sender, RoutedEventArgs e)
        {
            DonePublic.IsEnabled = true;
        } //选中/取消选中元素
        private void DonePublic_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.PublicSet.BPM = BPM.IsChecked == true;
            MainWindow.PublicSet.Q = 音符占刻.IsChecked == true;
            MainWindow.PublicSet.TC = 音轨数.IsChecked == true;
            DonePublic.IsEnabled = false;
        } //确认修改
    }
}