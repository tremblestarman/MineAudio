using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
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
using System.Globalization;
using MahApps.Metro.Controls;
using Audio2Minecraft;
using Newtonsoft.Json;

namespace Audio2MinecraftUI.Humberger
{
    /// <summary>
    /// MidiSetting.xaml 的交互逻辑
    /// </summary>
    public partial class MidiSetting : UserControl
    {
        public MidiViewType ViewType = MidiViewType.Track;
        private List<TextBlock> TextBlockElements;
        private List<CheckBox> CheckElements;
        private List<TextBox> TextBoxElements;
        TimeLine.MidiSettingInspector SelectedItem;
        TimeLine.MidiSettingInspector LastSavedItem;
        Dictionary<string, AutoFillMatch> _matches;

        public MidiSetting()
        {
            InitializeComponent();
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            DataObject.AddPastingHandler(播放相对坐标X, new DataObjectPastingEventHandler(OnPaste));
            DataObject.AddPastingHandler(播放相对坐标Y, new DataObjectPastingEventHandler(OnPaste));
            DataObject.AddPastingHandler(播放相对坐标Z, new DataObjectPastingEventHandler(OnPaste));
            DataObject.AddPastingHandler(额外延时, new DataObjectPastingEventHandler(OnPaste));
            Plat.IsEnabled = false;
            KeyScore.IsEnabled = false;
            PlaySound.IsEnabled = false;
            Done.IsEnabled = false;

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
                Stopsound
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
        }
        public void UpdateAutoFill(AutoFill autofill, string rule)
        {
            if (autofill == null) _matches = null;
            else _matches = autofill.Rule.matches;
            ItemChanged();
        }

        private void SwitchViewType(object sender, MouseButtonEventArgs e)
        {
            Plat.IsEnabled = false;
            KeyScore.IsEnabled = false;
            PlaySound.IsEnabled = false;
            Done.IsEnabled = false;
            PlaySoundEnableUpdate();
            if (ViewType == MidiViewType.Track)
            {
                ViewType = MidiViewType.Instrument;
                TracksView.ItemsSource = MainWindow.preTimeLine.InstrumentList; //Instrument
                SwitcherViewType.Source = new BitmapImage(new Uri(@"\img\instrument_view.png", UriKind.Relative));
            }
            else
            {
                ViewType = MidiViewType.Track;
                TracksView.ItemsSource = MainWindow.preTimeLine.TrackList; //Track
                SwitcherViewType.Source = new BitmapImage(new Uri(@"\img\track_view.png", UriKind.Relative));
            }
            //if (SelectedItem != null) ItemChanged();
        } //切换乐器/音轨视图
        public enum MidiViewType
        {
            Instrument,
            Track
        }

        public void ItemChanged()
        {
            SelectedItem = TracksView.SelectedItem as TimeLine.MidiSettingInspector;
            Done.IsEnabled = false;

            if (SelectedItem == null) return;
            Plat.IsEnabled = SelectedItem.Enable;
            KeyScore.IsEnabled = SelectedItem.EnableScore && SelectedItem.Enable;
            PlaySound.IsEnabled = SelectedItem.EnablePlaysound && SelectedItem.Enable;
            PlaySoundEnableUpdate();

            if (SelectedItem.Type == TimeLine.MidiSettingType.Track) SelectedName.Text = "已选中音轨：" + SelectedItem.Name;
            if (SelectedItem.Type == TimeLine.MidiSettingType.Instrument) SelectedName.Text = "已选中乐器：" + SelectedItem.Name;

            foreach (var E in CheckElements)
            {
                var Checked = CheckOrNot(SelectedItem, E.Uid);
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
            }
            TextSet(SelectedItem);
            ComboSet(SelectedItem);
            SlideSet(SelectedItem);
        } //选项更新
        private void TracksView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ItemChanged();
        } //选中项目切换
        private bool? CheckOrNot(TimeLine.MidiSettingInspector i, string element)
        {
            bool? BaseResult = false;
            if (element == "EnableScore") BaseResult = i.EnableScore;
            if (element == "EnablePlaysound") BaseResult = i.EnablePlaysound;
            if (element == "BarIndex") BaseResult = i.BarIndex;
            if (element == "BeatDuration") BaseResult = i.BeatDuration;
            if (element == "Channel") BaseResult = i.Channel;
            if (element == "DeltaTickDuration") BaseResult = i.DeltaTickDuration;
            if (element == "DeltaTickStart") BaseResult = i.DeltaTickStart;
            if (element == "Velocity") BaseResult = i.Velocity;
            if (element == "Pitch") BaseResult = i.Pitch;
            if (element == "MinecraftTickDuration") BaseResult = i.MinecraftTickDuration;
            if (element == "MinecraftTickStart") BaseResult = i.MinecraftTickStart;
            if (element == "StopSound") BaseResult = i.PlaysoundSetting.StopSound;
            if (i.Type == TimeLine.MidiSettingType.Instrument && ViewType == MidiViewType.Track) return BaseResult;
            bool? ParentResult = false;
            if (i.Type == TimeLine.MidiSettingType.Track && ViewType == MidiViewType.Track)
            {
                if (element == "EnableScore") if (i.Instruments.All(_i => _i.EnableScore == true)) ParentResult = true; else if (i.Instruments.All(_i => _i.EnableScore == false)) ParentResult = false; else ParentResult = null;
                if (element == "EnablePlaysound") if (i.Instruments.All(_i => _i.EnablePlaysound == true)) ParentResult = true; else if (i.Instruments.All(_i => _i.EnablePlaysound == false)) ParentResult = false; else ParentResult = null;
                if (element == "BarIndex") if (i.Instruments.All(_i => _i.BarIndex == true)) ParentResult = true; else if (i.Instruments.All(_i => _i.BarIndex == false)) ParentResult = false; else ParentResult = null;
                if (element == "BeatDuration") if (i.Instruments.All(_i => _i.BeatDuration == true)) ParentResult = true; else if (i.Instruments.All(_i => _i.BeatDuration == false)) ParentResult = false; else ParentResult = null;
                if (element == "Channel") if (i.Instruments.All(_i => _i.Channel == true)) ParentResult = true; else if (i.Instruments.All(_i => _i.Channel == false)) ParentResult = false; else ParentResult = null;
                if (element == "DeltaTickDuration") if (i.Instruments.All(_i => _i.DeltaTickDuration == true)) ParentResult = true; else if (i.Instruments.All(_i => _i.DeltaTickDuration == false)) ParentResult = false; else ParentResult = null;
                if (element == "DeltaTickStart") if (i.Instruments.All(_i => _i.DeltaTickStart == true)) ParentResult = true; else if (i.Instruments.All(_i => _i.DeltaTickStart == false)) ParentResult = false; else ParentResult = null;
                if (element == "Velocity") if (i.Instruments.All(_i => _i.Velocity == true)) ParentResult = true; else if (i.Instruments.All(_i => _i.Velocity == false)) ParentResult = false; else ParentResult = null;
                if (element == "Pitch") if (i.Instruments.All(_i => _i.Pitch == true)) ParentResult = true; else if (i.Instruments.All(_i => _i.Pitch == false)) ParentResult = false; else ParentResult = null;
                if (element == "MinecraftTickDuration") if (i.Instruments.All(_i => _i.MinecraftTickDuration == true)) ParentResult = true; else if (i.Instruments.All(_i => _i.MinecraftTickDuration == false)) ParentResult = false; else ParentResult = null;
                if (element == "MinecraftTickStart") if (i.Instruments.All(_i => _i.MinecraftTickStart == true)) ParentResult = true; else if (i.Instruments.All(_i => _i.MinecraftTickStart == false)) ParentResult = false; else ParentResult = null;
                if (element == "StopSound") if (i.Instruments.All(_i => _i.PlaysoundSetting.StopSound == true)) ParentResult = true; else if (i.Instruments.All(_i => _i.PlaysoundSetting.StopSound == false)) ParentResult = false; else ParentResult = null;
            }
            else if (i.Type == TimeLine.MidiSettingType.Instrument && ViewType == MidiViewType.Instrument)
            {
                var instrument = i.Name;
                if (element == "EnableScore")
                {
                    if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.EnableScore == true))) ParentResult = true;
                    else if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.EnableScore == false))) ParentResult = false;
                    else ParentResult = null;
                }
                if (element == "EnablePlaysound")
                {
                    if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.EnablePlaysound == true))) ParentResult = true;
                    else if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.EnablePlaysound == false))) ParentResult = false;
                    else ParentResult = null;
                }
                if (element == "BarIndex")
                {
                    if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.BarIndex == true))) ParentResult = true;
                    else if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.BarIndex == false))) ParentResult = false;
                    else ParentResult = null;
                }
                if (element == "BeatDuration")
                {
                    if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.BeatDuration == true))) ParentResult = true;
                    else if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.BeatDuration == false))) ParentResult = false;
                    else ParentResult = null;
                }
                if (element == "Channel")
                {
                    if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.Channel == true))) ParentResult = true;
                    else if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.Channel == false))) ParentResult = false;
                    else ParentResult = null;
                }
                if (element == "DeltaTickDuration")
                {
                    if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.DeltaTickDuration == true))) ParentResult = true;
                    else if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.DeltaTickDuration == false))) ParentResult = false;
                    else ParentResult = null;
                }
                if (element == "DeltaTickStart")
                {
                    if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.DeltaTickStart == true))) ParentResult = true;
                    else if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.DeltaTickStart == false))) ParentResult = false;
                    else ParentResult = null;
                }
                if (element == "Pitch")
                {
                    if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.Pitch == true))) ParentResult = true;
                    else if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.Pitch == false))) ParentResult = false;
                    else ParentResult = null;
                }
                if (element == "Velocity")
                {
                    if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.Velocity == true))) ParentResult = true;
                    else if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.Velocity == false))) ParentResult = false;
                    else ParentResult = null;
                }
                if (element == "MinecraftTickDuration")
                {
                    if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.MinecraftTickDuration == true))) ParentResult = true;
                    else if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.MinecraftTickDuration == false))) ParentResult = false;
                    else ParentResult = null;
                }
                if (element == "MinecraftTickStart")
                {
                    if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.MinecraftTickStart == true))) ParentResult = true;
                    else if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.MinecraftTickStart == false))) ParentResult = false;
                    else ParentResult = null;
                }
                if (element == "StopSound")
                {
                    if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.PlaysoundSetting.StopSound == true))) ParentResult = true;
                    else if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.PlaysoundSetting.StopSound == false))) ParentResult = false;
                    else ParentResult = null;
                }
            }
            return ParentResult;
        } //返回选项栏是否一致
        private void TextSet(TimeLine.MidiSettingInspector i) //返回文本信息是否一致
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
                if (i.Type == TimeLine.MidiSettingType.Instrument && ViewType == MidiViewType.Track)
                {
                    if (E.Uid == "ExecuteCood1") { 播放相对坐标X.Text = i.PlaysoundSetting.ExecuteCood[0].ToString(); 播放相对坐标X.SetValue(TextBoxHelper.WatermarkProperty, ""); }
                    if (E.Uid == "ExecuteCood2") { 播放相对坐标Y.Text = i.PlaysoundSetting.ExecuteCood[1].ToString(); 播放相对坐标Y.SetValue(TextBoxHelper.WatermarkProperty, ""); }
                    if (E.Uid == "ExecuteCood2") { 播放相对坐标Z.Text = i.PlaysoundSetting.ExecuteCood[2].ToString(); 播放相对坐标Z.SetValue(TextBoxHelper.WatermarkProperty, ""); }
                    if (E.Uid == "ExecuteTarget") { 相对玩家.Text = i.PlaysoundSetting.ExecuteTarget; 相对玩家.SetValue(TextBoxHelper.WatermarkProperty, ""); }
                    if (E.Uid == "PlayTarget") { 播放对象.Text = i.PlaysoundSetting.PlayTarget; 播放对象.SetValue(TextBoxHelper.WatermarkProperty, ""); }
                    if (E.Uid == "PlaySource") { 源.Text = i.PlaysoundSetting.PlaySource; 源.SetValue(TextBoxHelper.WatermarkProperty, ""); }
                    if (E.Uid == "InheritExpression") { 子表达式.Text = (i.PlaysoundSetting.InheritExpression != null) ? i.PlaysoundSetting.InheritExpression : ""; 子表达式.SetValue(TextBoxHelper.WatermarkProperty, ""); }
                    if (E.Uid == "ExtraDelay") { 额外延时.Text = (i.PlaysoundSetting.ExtraDelay > 0) ? i.PlaysoundSetting.ExtraDelay.ToString() : ""; 额外延时.SetValue(TextBoxHelper.WatermarkProperty, ""); }
                }
                else if (i.Type == TimeLine.MidiSettingType.Track && ViewType == MidiViewType.Track)
                {
                    if (E.Uid == "ExecuteCood1")
                    {
                        if (i.Instruments.All(_i => _i.PlaysoundSetting.ExecuteCood[0] == i.Instruments[0].PlaysoundSetting.ExecuteCood[0])) 播放相对坐标X.Text = i.Instruments[0].PlaysoundSetting.ExecuteCood[0].ToString();
                        else { 播放相对坐标X.Text = ""; 播放相对坐标X.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                    }
                    if (E.Uid == "ExecuteCood2")
                    {
                        if (i.Instruments.All(_i => _i.PlaysoundSetting.ExecuteCood[1] == i.Instruments[0].PlaysoundSetting.ExecuteCood[1])) 播放相对坐标Y.Text = i.Instruments[0].PlaysoundSetting.ExecuteCood[1].ToString();
                        else { 播放相对坐标Y.Text = ""; 播放相对坐标Y.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                    }
                    if (E.Uid == "ExecuteCood2")
                    {
                        if (i.Instruments.All(_i => _i.PlaysoundSetting.ExecuteCood[2] == i.Instruments[0].PlaysoundSetting.ExecuteCood[2])) 播放相对坐标Z.Text = i.Instruments[0].PlaysoundSetting.ExecuteCood[2].ToString();
                        else { 播放相对坐标Z.Text = ""; 播放相对坐标Z.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                    }
                    if (E.Uid == "ExecuteTarget")
                    {
                        if (i.Instruments.All(_i => _i.PlaysoundSetting.ExecuteTarget == i.Instruments[0].PlaysoundSetting.ExecuteTarget)) 相对玩家.Text = i.Instruments[0].PlaysoundSetting.ExecuteTarget;
                        else { 相对玩家.Text = ""; 相对玩家.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                    }
                    if (E.Uid == "PlayTarget")
                    {
                        if (i.Instruments.All(_i => _i.PlaysoundSetting.PlayTarget == i.Instruments[0].PlaysoundSetting.PlayTarget)) 播放对象.Text = i.Instruments[0].PlaysoundSetting.PlayTarget;
                        else { 播放对象.Text = ""; 播放对象.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                    }
                    if (E.Uid == "PlaySource")
                    {
                        if (i.Instruments.All(_i => _i.PlaysoundSetting.PlaySource == i.Instruments[0].PlaysoundSetting.PlaySource)) 源.Text = i.Instruments[0].PlaysoundSetting.PlaySource;
                        else { 源.Text = ""; 源.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                    }
                    if (E.Uid == "InheritExpression")
                    {
                        if (i.Instruments.All(_i => _i.PlaysoundSetting.InheritExpression == i.Instruments[0].PlaysoundSetting.InheritExpression)) 子表达式.Text = (i.Instruments[0].PlaysoundSetting.InheritExpression != null) ? i.Instruments[0].PlaysoundSetting.InheritExpression : "";
                        else { 子表达式.Text = ""; 子表达式.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                    }
                    if (E.Uid == "ExtraDelay")
                    {
                        if (i.Instruments.All(_i => _i.PlaysoundSetting.ExtraDelay == i.Instruments[0].PlaysoundSetting.ExtraDelay)) 额外延时.Text = (i.Instruments[0].PlaysoundSetting.ExtraDelay > 0) ? i.Instruments[0].PlaysoundSetting.ExtraDelay.ToString().ToString() : "";
                        else { 额外延时.Text = ""; 额外延时.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                    }
                }
                else if (i.Type == TimeLine.MidiSettingType.Instrument && ViewType == MidiViewType.Instrument)
                {
                    var instrument = i.Name;
                    if (E.Uid == "ExecuteCood1")
                    {
                        if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.PlaysoundSetting.ExecuteCood[0] == i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.ExecuteCood[0]))) 播放相对坐标X.Text = i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.ExecuteCood[0].ToString();
                        else { 播放相对坐标X.Text = ""; 播放相对坐标X.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                    }
                    if (E.Uid == "ExecuteCood2")
                    {
                        if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.PlaysoundSetting.ExecuteCood[1] == i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.ExecuteCood[1]))) 播放相对坐标Y.Text = i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.ExecuteCood[1].ToString();
                        else { 播放相对坐标Y.Text = ""; 播放相对坐标Y.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                    }
                    if (E.Uid == "ExecuteCood2")
                    {
                        if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.PlaysoundSetting.ExecuteCood[2] == i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.ExecuteCood[2]))) 播放相对坐标Z.Text = i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.ExecuteCood[2].ToString();
                        else { 播放相对坐标Z.Text = ""; 播放相对坐标Z.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                    }
                    if (E.Uid == "ExecuteTarget")
                    {
                        if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.PlaysoundSetting.ExecuteTarget == i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.ExecuteTarget))) 相对玩家.Text = i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.ExecuteTarget;
                        else { 相对玩家.Text = ""; 相对玩家.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                    }
                    if (E.Uid == "PlayTarget")
                    {
                        if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.PlaysoundSetting.PlayTarget == i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.PlayTarget))) 播放对象.Text = i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.PlayTarget;
                        else { 播放对象.Text = ""; 播放对象.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                    }
                    if (E.Uid == "PlaySource")
                    {
                        if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.PlaysoundSetting.PlaySource == i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.PlaySource))) 源.Text = i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.PlaySource;
                        else { 源.Text = ""; 源.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                    }
                    if (E.Uid == "InheritExpression")
                    {
                        if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.PlaysoundSetting.InheritExpression == i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.InheritExpression))) 子表达式.Text = (i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.InheritExpression != null) ? i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.InheritExpression : "";
                        else { 子表达式.Text = ""; 子表达式.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                    }
                    if (E.Uid == "ExtraDelay")
                    {
                        if (i.Tracks.All(t => (from v in t.Instruments where v.Name == instrument select v).All(_i => _i.PlaysoundSetting.ExtraDelay == i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.ExtraDelay))) 额外延时.Text = (i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.ExtraDelay > 0) ? i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.ExtraDelay.ToString() : "";
                        else { 额外延时.Text = ""; 额外延时.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                    }
                }
            }
        }
        private void ComboSet(TimeLine.MidiSettingInspector i) //返回选项框是否一致
        {
            if (SelectedItem.Type == TimeLine.MidiSettingType.Instrument && ViewType == MidiViewType.Track)
            {
                音色名称.Text = i.PlaysoundSetting.SoundName.ToString();
                音色名称.SetValue(TextBoxHelper.WatermarkProperty, "");
                ComboDefaults(i.Name);
            }
            else if (SelectedItem.Type == TimeLine.MidiSettingType.Track && ViewType == MidiViewType.Track)
            {
                if (i.Instruments.All(_i => _i.PlaysoundSetting.SoundName == i.Instruments[0].PlaysoundSetting.SoundName)) 音色名称.Text = i.Instruments[0].PlaysoundSetting.SoundName;
                else { 音色名称.Text = ""; 音色名称.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                ComboDefaults(i);
            }
            else if (i.Type == TimeLine.MidiSettingType.Instrument && ViewType == MidiViewType.Instrument)
            {
                if (i.Tracks.All(t => (from v in t.Instruments where v.Name == i.Name select v).All(_i => _i.PlaysoundSetting.SoundName == i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.SoundName))) 音色名称.Text = (i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.SoundName != null) ? i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.SoundName : "";
                else { 音色名称.Text = ""; 音色名称.SetValue(TextBoxHelper.WatermarkProperty, "――"); }
                ComboDefaults(i.Name);
            }
        }
        private void SlideSet(TimeLine.MidiSettingInspector i) //返回滑动栏是否一致
        {
            if (SelectedItem.Type == TimeLine.MidiSettingType.Instrument && ViewType == MidiViewType.Track)
            {
                音量增益.Value = (i.PlaysoundSetting.PercVolume == -1) ? 50 : (i.PlaysoundSetting.PercVolume > 200) ? 100 : (i.PlaysoundSetting.PercVolume < 0) ? 0 : (double)i.PlaysoundSetting.PercVolume / 2;
                音量大小.Visibility = Visibility.Visible;
            }
            else if (SelectedItem.Type == TimeLine.MidiSettingType.Track && ViewType == MidiViewType.Track)
            {
                if (i.Instruments.All(_i => _i.PlaysoundSetting.PercVolume == i.Instruments[0].PlaysoundSetting.PercVolume)) 音量增益.Value = (i.Instruments[0].PlaysoundSetting.PercVolume == -1) ? 50 : (i.Instruments[0].PlaysoundSetting.PercVolume > 200) ? 100 : (i.Instruments[0].PlaysoundSetting.PercVolume < 0) ? 0 : (double)i.Instruments[0].PlaysoundSetting.PercVolume / 2;
                else { 音量增益.Value = 50; 音量大小.Visibility = Visibility.Hidden; }
            }
            else if (i.Type == TimeLine.MidiSettingType.Instrument && ViewType == MidiViewType.Instrument)
            {
                if (i.Tracks.All(t => (from v in t.Instruments where v.Name == i.Name select v).All(_i => _i.PlaysoundSetting.PercVolume == i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.PercVolume))) 音量增益.Value = (i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.PercVolume == -1) ? 50 : (i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.PercVolume > 200) ? 100 : (i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.PercVolume < 0) ? 0 : (double)i.Tracks.FirstOrDefault(_t => _t.Instruments.Any(__i => __i.Name == i.Name)).Instruments.FirstOrDefault(__i => __i.Name == i.Name).PlaysoundSetting.PercVolume / 2;
                else { 音量增益.Value = 50; 音量大小.Visibility = Visibility.Hidden; }
            }
        }

        private void ComboDefaults(string instrument)
       {
            音色名称.Items.Clear();
            if (_matches != null)
            {
                var c = (from m in _matches where m.Value.instrument == instrument select m);
                foreach (var i in c) 音色名称.Items.Add(i.Key);
            }
        }
        private void ComboDefaults(TimeLine.MidiSettingInspector track)
        {
            音色名称.Items.Clear();
            foreach (var instrument in track.Instruments)
            {
                ComboDefaults(instrument.Name);
            }
        }

        private void Elements_Click(object sender, RoutedEventArgs e)
        {
            var E = e.OriginalSource as CheckBox;
            if (E.Uid == "EnableScore") KeyScore.IsEnabled = (E.IsChecked == true);
            if (E.Uid == "EnablePlaysound") { PlaySound.IsEnabled = (E.IsChecked == true); PlaySoundEnableUpdate(); }

            Done.IsEnabled = true;
        } //选中/取消选中元素
        private void DoneChanges(object sender, RoutedEventArgs e)
        {
            LastSavedItem = SelectedItem;
            if (SelectedItem.Type == TimeLine.MidiSettingType.Instrument && ViewType == MidiViewType.Track)
            {
                Update(SelectedItem);
                SelectedItem.UpdateTrackOnly(); //向上更新音轨列表
            }
            else if (SelectedItem.Type == TimeLine.MidiSettingType.Track && ViewType == MidiViewType.Track)
            {
                UpdateTracks(); //向下更新音轨列表
            }
            else if (ViewType == MidiViewType.Instrument)
            {
                UpdateInstruments(); //更新乐器列表
            }
            Done.IsEnabled = false;
        } //确认修改
        private void Enable_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                Plat.IsEnabled = SelectedItem.Enable;
                KeyScore.IsEnabled = 计分板输出.IsChecked != false;
                PlaySound.IsEnabled = Playsound输出.IsChecked != false;
                PlaySoundEnableUpdate();
            }
        } //选中/取消选中项目
        private void UpdateInstruments()
        {
            foreach (var t in SelectedItem.Tracks)
            {
                foreach (var i in t.Instruments)
                {
                    if (i.Name == SelectedItem.Name)
                    {
                        Update(i);
                    }
                }
            }
        }
        private void UpdateTracks()
        {
            foreach (var i in SelectedItem.Instruments)
            {
                Update(i);
            }
        }
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
        private void 音色名称_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Done.IsEnabled = true;
            if (音色名称.GetValue(TextBoxHelper.WatermarkProperty).ToString() != "")
            {
                音色名称.SetValue(TextBoxHelper.WatermarkProperty, "");
            }

            if (_matches != null && 音色名称.SelectedItem != null)
            {
                if (SelectedItem.Type == TimeLine.MidiSettingType.Instrument)
                {
                    子表达式.Text = (!_matches.ContainsKey(音色名称.SelectedItem.ToString())) ? "" : _matches[音色名称.SelectedItem.ToString()].expression;
                    播放对象.Text = (!_matches.ContainsKey(音色名称.SelectedItem.ToString())) ? "@a" : _matches[音色名称.SelectedItem.ToString()].target;
                    源.Text = (!_matches.ContainsKey(音色名称.SelectedItem.ToString())) ? "@a" : _matches[音色名称.SelectedItem.ToString()].source;
                    Stopsound.IsChecked = (!_matches.ContainsKey(音色名称.SelectedItem.ToString())) ? false : _matches[音色名称.SelectedItem.ToString()].stopsound.enable;
                    额外延时.Text = (!_matches.ContainsKey(音色名称.SelectedItem.ToString())) ? "" : (Stopsound.IsChecked == false) ? "" : _matches[音色名称.SelectedItem.ToString()].stopsound.extra_delay.ToString();
                    相对玩家.Text = (!_matches.ContainsKey(音色名称.SelectedItem.ToString())) ? "@a" : _matches[音色名称.SelectedItem.ToString()].excute_target;
                    播放相对坐标X.Text = (!_matches.ContainsKey(音色名称.SelectedItem.ToString())) ? "0" : _matches[音色名称.SelectedItem.ToString()].excute_pos.x.ToString();
                    播放相对坐标Y.Text = (!_matches.ContainsKey(音色名称.SelectedItem.ToString())) ? "0" : _matches[音色名称.SelectedItem.ToString()].excute_pos.y.ToString();
                    播放相对坐标Z.Text = (!_matches.ContainsKey(音色名称.SelectedItem.ToString())) ? "0" : _matches[音色名称.SelectedItem.ToString()].excute_pos.z.ToString();
                    if (!_matches.ContainsKey(音色名称.SelectedItem.ToString()))
                        音量增益.Value = 50;
                    else { var v = _matches[音色名称.SelectedItem.ToString()].volume / 2; 音量增益.Value = (v > 100) ? 100 : (v < 0) ? 0 : v; }
                }
                else if (SelectedItem.Type == TimeLine.MidiSettingType.Track)
                {
                    子表达式.Text = (!_matches.ContainsKey(音色名称.SelectedItem.ToString())) ? "" : _matches[音色名称.SelectedItem.ToString()].expression;
                    播放对象.Text = (!_matches.ContainsKey(音色名称.SelectedItem.ToString())) ? "@a" : _matches[音色名称.SelectedItem.ToString()].target;
                    源.Text = (!_matches.ContainsKey(音色名称.SelectedItem.ToString())) ? "@a" : _matches[音色名称.SelectedItem.ToString()].source;
                    Stopsound.IsChecked = (!_matches.ContainsKey(音色名称.SelectedItem.ToString())) ? false : _matches[音色名称.SelectedItem.ToString()].stopsound.enable;
                    额外延时.Text = (!_matches.ContainsKey(音色名称.SelectedItem.ToString())) ? "" : (Stopsound.IsChecked == false) ? "" : _matches[音色名称.SelectedItem.ToString()].stopsound.extra_delay.ToString();
                    相对玩家.Text = (!_matches.ContainsKey(音色名称.SelectedItem.ToString())) ? "@a" : _matches[音色名称.SelectedItem.ToString()].excute_target;
                    播放相对坐标X.Text = (!_matches.ContainsKey(音色名称.SelectedItem.ToString())) ? "0" : _matches[音色名称.SelectedItem.ToString()].excute_pos.x.ToString();
                    播放相对坐标Y.Text = (!_matches.ContainsKey(音色名称.SelectedItem.ToString())) ? "0" : _matches[音色名称.SelectedItem.ToString()].excute_pos.y.ToString();
                    播放相对坐标Z.Text = (!_matches.ContainsKey(音色名称.SelectedItem.ToString())) ? "0" : _matches[音色名称.SelectedItem.ToString()].excute_pos.z.ToString();
                    if (!_matches.ContainsKey(音色名称.SelectedItem.ToString()))
                        音量增益.Value = 50;
                    else { var v = _matches[音色名称.SelectedItem.ToString()].volume / 2; 音量增益.Value = (v > 100) ? 100 : (v < 0) ? 0 : v; }
                }
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
        private void TextInpute(object sender, TextCompositionEventArgs e)
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
        private void 播放相对坐标X_TextInput(object sender, TextCompositionEventArgs e)
        {
            double val;
            if (!Double.TryParse(播放相对坐标X.Text, out val) && 播放相对坐标X.Text != "")
            {
                播放相对坐标X.TextInput -= 播放相对坐标X_TextInput;
                播放相对坐标X.Text = oldText;
                播放相对坐标X.CaretIndex = oldIndex;
                播放相对坐标X.TextInput += 播放相对坐标X_TextInput;
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
        private void 播放相对坐标Y_TextInput(object sender, TextCompositionEventArgs e)
        {
            double val;
            if (!Double.TryParse(播放相对坐标Y.Text, out val) && 播放相对坐标Y.Text != "")
            {
                播放相对坐标Y.TextInput -= 播放相对坐标Y_TextInput;
                播放相对坐标Y.Text = oldText;
                播放相对坐标Y.CaretIndex = oldIndex;
                播放相对坐标Y.TextInput += 播放相对坐标Y_TextInput;
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
        private void 播放相对坐标Z_TextInput(object sender, TextCompositionEventArgs e)
        {
            double val;
            if (!Double.TryParse(播放相对坐标Z.Text, out val) && 播放相对坐标Z.Text != "")
            {
                播放相对坐标Z.TextInput -= 播放相对坐标Z_TextInput;
                播放相对坐标Z.Text = oldText;
                播放相对坐标Z.CaretIndex = oldIndex;
                播放相对坐标Z.TextInput += 播放相对坐标Z_TextInput;
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
        private void 额外延时_TextInput(object sender, TextCompositionEventArgs e)
        {
            int val;
            if (!Int32.TryParse(额外延时.Text, out val) && 额外延时.Text != "" || 额外延时.Text == "-")
            {
                额外延时.TextInput -= 额外延时_TextInput;
                额外延时.Text = oldText;
                额外延时.CaretIndex = oldIndex;
                额外延时.TextInput += 额外延时_TextInput;
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
    }

    public class SlidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "";
            var v = (double)value * 2;
            result = ((int)v).ToString() + "%";
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}