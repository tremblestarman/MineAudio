using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace ExecutiveMidi.Humberger
{
    /// <summary>
    /// MidiSetting.xaml 的交互逻辑
    /// </summary>
    public partial class MidiSetting : UserControl
    {
        public ObservableCollection<MidiMarker> TrackMarkerList = new ObservableCollection<MidiMarker>();
        public ObservableCollection<MidiMarker> InstrumentMarkerList = new ObservableCollection<MidiMarker>();
        public void ReadListView(TimeLine timeline)
        {
            TrackMarkerList = new ObservableCollection<MidiMarker>();
            InstrumentMarkerList = new ObservableCollection<MidiMarker>();
            var _tl = timeline.TrackList;
            var _il = timeline.InstrumentList;
            foreach (var i in _tl)
            {
                TrackMarkerList.Add(new MidiMarker() { Name = i.Name });
            }
            foreach (var i in _il)
            {
                InstrumentMarkerList.Add(new MidiMarker() { Name = i.Name });
            }
        }
        public void UpdateListView(ObservableCollection<MidiMarker> NewTrackMarkerList, ObservableCollection<MidiMarker> NewInstrumentMarkerList)
        {
            foreach (var i in TrackMarkerList)
            {
                var t = NewTrackMarkerList.First(k => k.Name == i.Name);
                if (t != null)
                {
                    i.Command = t.Command;
                    i.Location = t.Location;
                }
            }
            foreach (var i in InstrumentMarkerList)
            {
                var p = NewInstrumentMarkerList.First(k => k.Name == i.Name);
                if (p != null)
                {
                    i.Command = p.Command;
                    i.Location = p.Location;
                }
            }
        }
        public MidiViewType ViewType = MidiViewType.Track;
        MidiMarker SelectedItem;

        public MidiSetting()
        {
            InitializeComponent();
        }
        private void UserControl_Initialized(object sender, EventArgs e)
        {
            Done.IsEnabled = false;
            Plat.IsEnabled = false;
            ExecuteLocation.IsChecked = true;
        }

        private void TracksView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ItemChanged();
        } //选中项目切换
        private void SwitchViewType(object sender, MouseButtonEventArgs e)
        {
            SelectedName.Text = "";
            Done.IsEnabled = false;
            if (ViewType == MidiViewType.Track)
            {
                ViewType = MidiViewType.Instrument;
                TracksView.ItemsSource = InstrumentMarkerList; //Instrument
                SwitcherViewType.Source = new BitmapImage(new Uri(@"\img\instrument_view.png", UriKind.Relative));
            }
            else
            {
                ViewType = MidiViewType.Track;
                TracksView.ItemsSource = TrackMarkerList; //Track
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
            SelectedItem = TracksView.SelectedItem as MidiMarker;
            Setting.IsEnabled = true;
            if (SelectedItem == null) { Setting.IsEnabled = false; return; }
            SelectedName.Text = (ViewType == MidiViewType.Track) ? "音轨：" + SelectedItem.Name : "乐器：" + SelectedItem.Name;
            Command.Text = SelectedItem.Command;
            ExecuteLocation.IsChecked = SelectedItem.Location == MidiMarker.ExecuteLocation.Start;
            Done.IsEnabled = false;
        } //选项更新
        private void DoneChanges(object sender, MouseButtonEventArgs e)
        {
            SelectedItem.Location = ExecuteLocation.IsChecked == true ? MidiMarker.ExecuteLocation.Start : MidiMarker.ExecuteLocation.End ;
            SelectedItem.Command = Command.Text;
            Done.IsEnabled = false;
        }

        private void TextChanging(object sender, TextChangedEventArgs e)
        {
            Done.IsEnabled = true;
        }
        private void Checked(object sender, RoutedEventArgs e)
        {
            Done.IsEnabled = true;
        }
    }

    public class MidiMarker : INotifyPropertyChanged
    {
        string _name;
        public string Name { get { return _name; } set { _name = value; RaisePropertyChanged("Name"); } }
        string _command = "";
        public string Command { get { return _command; } set { _command = value; RaisePropertyChanged("Command"); } }
        ExecuteLocation _loc = ExecuteLocation.Start;
        public ExecuteLocation Location { get { return _loc; } set { _loc = value; RaisePropertyChanged("Location"); } }
        public event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged(string propname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
        }
        public enum ExecuteLocation
        {
            Start,
            End
        }
    }
}
