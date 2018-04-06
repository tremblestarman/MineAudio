using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Midi;

namespace Audio2MinecraftScore
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void MIDI_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Midi|*.mid";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == DialogResult.OK) { MIDI.Text = fileDialog.FileName; }
        }
        private void WAV_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Wav|*.wav";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == DialogResult.OK) { WAV.Text = fileDialog.FileName; }
        }
        private void GO_Click(object sender, EventArgs e)
        {
            var b = new Audio2ScoreOperation();
            var timeline = b.SerializeTimeLine(MIDI.Text, null, 128);
            timeline.timeLineInfo.AllMidibarIndex = false;
            timeline.timeLineInfo.AllMidibeatDuration = false;
            timeline.timeLineInfo.AllMidiMinecraftDurationTick = false;
            timeline.timeLineInfo.AllMidistartTick = false;
            timeline.timeLineInfo.AllMiditickDuration = false;
            timeline.timeLineInfo.AllMidiMinecraftStartTick = false;
            timeline.timeLineInfo.AllMidiTimbreEnable = true;
            foreach (MidiTracksInfo a in timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo.Values)
            {
                a.pitch = false;
                a.pitch_end = false;
                a.velocity = false;
                a.timbreEnable = true;
                a.timbreStopEnable = false;
            }
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["9"].timbreInfo = new TimbreInfo() { playTimbre = "9" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["33"].timbreInfo = new TimbreInfo() { playTimbre = "33" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["12"].timbreInfo = new TimbreInfo() { playTimbre = "12" };
            //timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["51"].timbreInfo = new TimbreInfo() { playTimbre = "51" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["1"].timbreInfo = new TimbreInfo() { playTimbre = "1" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["5"].timbreInfo = new TimbreInfo() { playTimbre = "5" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["7"].timbreInfo = new TimbreInfo() { playTimbre = "7" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["14"].timbreInfo = new TimbreInfo() { playTimbre = "14" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["26"].timbreInfo = new TimbreInfo() { playTimbre = "26" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["Pedal Hi-Hat"].timbreInfo = new TimbreInfo() { playTimbre = "0", overlapPitch = "44" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["Bass Drum1"].timbreInfo = new TimbreInfo() { playTimbre = "0", overlapPitch = "36" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["Low-Mid Tom"].timbreInfo = new TimbreInfo() { playTimbre = "0", overlapPitch = "47" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["Ride Cymbal2"].timbreInfo = new TimbreInfo() { playTimbre = "0", overlapPitch = "59" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["Vibra-slap"].timbreInfo = new TimbreInfo() { playTimbre = "0", overlapPitch = "58" };

            b.SerializeSchematic(timeline, new V2() { Width = 64, Direction = 0, AlwaysActive = true, AlwaysLoadEntities = false }, "E:\\time.schematic");
            b = null;
            ////////////////////////
            //大力调试stop sound //
            //////////////////////
            #region name of sickness is love
            /*var b = new GeneralOperation();
            var timeline = b.SerializeTimeLine(MIDI.Text, WAV.Text, 87, 5, 5, 4);
            timeline.timeLineInfo.AllMidibarIndex = false;
            timeline.timeLineInfo.AllMidibeatDuration = false;
            timeline.timeLineInfo.AllMidiMinecraftDurationTick = false;
            timeline.timeLineInfo.AllMidistartTick = false;
            timeline.timeLineInfo.AllMiditickDuration = false;
            timeline.timeLineInfo.AllMidiMinecraftStartTick = false;
            timeline.timeLineInfo.AllMidiTimbreEnable = true;
            foreach (MidiTracksInfo a in timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo.Values)
            {
                a.pitch = false;
                a.pitch_end = false;
                a.velocity = false;
                a.timbreEnable = true;
                a.timbreStopEnable = false;
            }
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["1c"].pitch = true;
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["1c"].velocity = true;
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["1c"].timbreInfo = new TimbreInfo() { playTimbre = "1c" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["1c"].timbreInfo.executeCood = new double[] { 0.8, 0, 0 };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["3c"].timbreInfo = new TimbreInfo() { playTimbre = "3c" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["3c"].timbreInfo.executeCood = new double[] { -0.8, 0, 0 };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["5c"].timbreInfo = new TimbreInfo() { playTimbre = "5c" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["5c"].timbreInfo.executeCood = new double[] { 0, 0, -0.6 };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["44"].timbreInfo = new TimbreInfo() { playTimbre = "0", overlapPitch = "44" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["minecraft:block.note.basedrum"].timbreInfo = new TimbreInfo() { playTimbre = "minecraft:block.note", overlapPitch = "basedrum" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["minecraft:block.note.basedrum"].volume = 1.25f;
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["33"].timbreInfo = new TimbreInfo() { playTimbre = "0", overlapPitch = "33" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["33"].volume = 1.25f;
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["33"].velocity = true;
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["0.42"].timbreInfo = new TimbreInfo() { playTimbre = "0", overlapPitch = "42" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["0.42"].volume = 1.25f;
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["35"].timbreInfo = new TimbreInfo() { playTimbre = "0", overlapPitch = "35" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["35"].velocity = true;
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["0.70"].timbreInfo = new TimbreInfo() { playTimbre = "0", overlapPitch = "70"};
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["0.70"].volume = 0.75f;
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["0.42"].volume = 1.75f;
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["12"].timbreInfo = new TimbreInfo() { playTimbre = "12" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["12"].timbreInfo.executeCood = new double[] { 0, 0, -0.6 };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["34"].timbreInfo = new TimbreInfo() { playTimbre = "27" };
            timeline.timeLineInfo.TickNodesInfo.MidiTracksInfo["34"].timbreInfo.executeCood = new double[] { 0, 0, 0.5 };

            b.SerializeSchematic(timeline, new V2() { Width = 64, Direction = 0, AlwaysActive = true, AlwaysLoadEntities = false }, "E:\\time.schematic");
            b = null;*/
            #endregion

            //输出nbt
            //var nbt = new NbtFile();
            //nbt.LoadFromFile(MIDI.Text);
            //nbt.LoadFromFile("E:\\time.schematic");
            //var root = nbt.RootTag;
            //textBox1.Text = root.ToString("    ");

            //输出Midi的bpm
            /*var f = new MidiFile(MIDI.Text);
            var a = f.Events[0];
            MidiEvent b = a[0];
            textBox1.Text = Int32.Parse(f.Events[0][0].ToString().Split(' ')[2].Replace("bpm", "")).ToString();*/

            //输出wave
            //var a = new AudioStreamWave().Serialize(MIDI.Text, new TimeLine()).TickNodes;
            //textBox1.Text = a.Count.ToString();

            //输出Midi音轨
            /*
            var a = new TimeLine();
            a = new AudioStreamMidi().Serialize(MIDI.Text, a);
            string g = "";
            foreach(TickNode t in a.TickNodes)
            {
                if (t.MidiNodes == null)
                    g = g + Environment.NewLine;
                if (t.MidiNodes != null)
                    foreach(MidiNode n in t.MidiNodes)
                        g = g + n.MinecraftStartTick.ToString() + " " + n.pitch.ToString() + "|";
            }
            textBox1.Text = g;*/

            //输出sch
            //Export.ExportSchematic("‪C:\\Users\\Administrator\\Desktop");
        }
    }
}
