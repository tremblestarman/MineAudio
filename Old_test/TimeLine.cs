using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;

namespace Audio2MinecraftScore
{
    public class TimeLine
    {
        #region MIDI
        public int MidiFileFormat { get; set; }
        /// <summary>
        /// the Numbet of Tracks in Midi
        /// </summary>
        public int MidiTracksCount { get; set; }
        /// <summary>
        /// Delta Ticks Per Quarter Note
        /// </summary>
        public int MidiDeltaTicksPerQuarterNote { get; set; }
        #endregion
        #region Wave
        /// <summary>
        /// Waveformat
        /// </summary>
        public int AudioFileFormat { get; set; }
        /// <summary>
        /// 8 or 16 or 32
        /// </summary>
        public int AudioBit { get; set; }

        #endregion
        #region TickNodes
        /// <summary>
        /// the TickNodes for minecraft ingame output
        /// in Time Array, per 1 Minecraft Tick (0.05s)
        /// </summary>
        public List<TickNode> TickNodes { get; set; }
        #endregion
        public TimeLineInfo timeLineInfo = new TimeLineInfo();
    }

    public class TickNode
    {
        /// <summary>
        /// Nodes storing MidiEvents
        /// </summary>
        public List<MidiNode> MidiNodes { get; set; }
        /// <summary>
        /// Nodes storing AudioInfo in Left Channel
        /// </summary>
        public List<AudioNode> AudioNodesLeft { get; set; }
        /// <summary>
        /// Nodes storing AudioInfo in Right Channel
        /// </summary>
        public List<AudioNode> AudioNodesRight { get; set; }
        public int currentTick { get; set; }
    }

    public class MidiNode
    {
        public long barIndex { get; set; }
        public long beatDuration { get; set; }
        public long tickDuration { get; set; }
        public long startTick { get; set; }
        public int MinecraftStartTick { get; set; }
        public int channel { get; set; }
        public int pitch { get; set; }
        public int velocity { get; set; }
        public int MinecraftDurationTick { get; set; }
        public int pitch_end { get; set; }
        public string instrument { get; set; }
        private string _track_name = "none";
        public string track_name { get { return _track_name; } set { _track_name = value; } }
    }

    public class AudioNode
    {
        public List<int> MinecraftFrequencyPerTick = new List<int>();
        public List<int> MinecraftVolumePerTick = new List<int>();
        public int MinecraftStartTick { get; set; }
    }

    public class TimeLineInfo
    {
        private bool _MidiFileFormat = false;
        public bool MidiFileFormat { get { return _MidiFileFormat; } set { _MidiFileFormat = value; } }
        private bool _MidiTracksCount = true;
        public bool MidiTracksCount { get { return _MidiTracksCount; } set { _MidiTracksCount = value; } }
        private bool _MidiDeltaTicksPerQuarterNote = true;
        public bool MidiDeltaTicksPerQuarterNote { get { return _MidiDeltaTicksPerQuarterNote; } set { _MidiDeltaTicksPerQuarterNote = value; } }
        private bool _AudioFileFormat = false;
        public bool AudioFileFormat { get { return _AudioFileFormat; } set { _AudioFileFormat = value; } }
        private bool _AudioBit = false;
        public bool AudioBit { get { return _AudioBit; } set { _AudioBit = value; } }
        public TickNodesInfo TickNodesInfo = new TickNodesInfo();
        private bool _IgnoreZero = true;
        public bool IgnoreZero { get { return _IgnoreZero; } set { _IgnoreZero = value; } }
        private bool _currentTick = true;
        public bool currentTick { get { return _currentTick; } set { _currentTick = value; } }
        #region public midi
        private bool _MidiNodes = true;
        public bool AllMidiEnable { get { return _MidiNodes; } set { _MidiNodes = value; } }
        private bool _barIndex = true;
        public bool AllMidibarIndex { get { return _barIndex; } set { _barIndex = value; } }
        private bool _beatDuration = false;
        public bool AllMidibeatDuration { get { return _beatDuration; } set { _beatDuration = value; } }
        private bool _tickDuration = false;
        public bool AllMiditickDuration { get { return _tickDuration; } set { _tickDuration = value; } }
        private bool _startTick = false;
        public bool AllMidistartTick { get { return _startTick; } set { _startTick = value; } }
        private bool m_MinecraftStartTick = true;
        public bool AllMidiMinecraftStartTick { get { return m_MinecraftStartTick; } set { m_MinecraftStartTick = value; } }
        private bool _channel = false;
        public bool AllMidichannel { get { return _channel; } set { _channel = value; } }
        private bool _pitch = true;
        public bool AllMidipitch { get { return _pitch; } set { _pitch = value; } }
        private bool _velocity = true;
        public bool AllMidivelocity { get { return _velocity; } set { _velocity = value; } }
        private bool _MinecraftDurationTick = true;
        public bool AllMidiMinecraftDurationTick { get { return _MinecraftDurationTick; } set { _MinecraftDurationTick = value; } }
        private bool _pitch_end = true;
        public bool AllMidipitch_end { get { return _pitch_end; } set { _pitch_end = value; } }
        private float _volume = 1;
        public float AllMidivolume { get { return _volume; } set { _volume = value; } }
        private TimbreInfo _timbre = new TimbreInfo();
        public TimbreInfo AllMidiTimbreInfo { get { return _timbre; } set { _timbre = value; } }
        private bool _timbre_t = false;
        public bool AllMidiTimbreEnable { get { return _timbre_t; } set { _timbre_t = value; } }
        private bool _timbre_st = false;
        public bool timbreStopEnable { get { return _timbre_st; } set { _timbre_st = value; } }
        #endregion
        #region public audio
        private bool _AudioLeftNodes = true;
        public bool AllAudioLeftEnable { get { return _AudioLeftNodes; } set { _AudioLeftNodes = value; } }
        private bool _AudioRightNodes = true;
        public bool AllAudioRightEnable { get { return _AudioRightNodes; } set { _AudioRightNodes = value; } }
        private bool _MinecraftFrequencyPerTickL = true;
        public bool AllAudioMinecraftFrequencyPerTickL { get { return _MinecraftFrequencyPerTickL; } set { _MinecraftFrequencyPerTickL = value; } }
        private bool _MinecraftVolumePerTickL = true;
        public bool AllAudioMinecraftVolumePerTickL { get { return _MinecraftVolumePerTickL; } set { _MinecraftVolumePerTickL = value; } }
        private bool a_MinecraftStartTickL = false;
        public bool AllAudioMinecraftStartTickL { get { return a_MinecraftStartTickL; } set { a_MinecraftStartTickL = value; } }
        private bool _MinecraftFrequencyPerTickR = true;
        public bool AllAudioMinecraftFrequencyPerTickR { get { return _MinecraftFrequencyPerTickR; } set { _MinecraftFrequencyPerTickR = value; } }
        private bool _MinecraftVolumePerTickR = true;
        public bool AllAudioMinecraftVolumePerTickR { get { return _MinecraftVolumePerTickR; } set { _MinecraftVolumePerTickR = value; } }
        private bool a_MinecraftStartTickR = false;
        public bool AllAudioMinecraftStartTickR { get { return a_MinecraftStartTickR; } set { a_MinecraftStartTickR = value; } }

        #endregion
    }

    public class TickNodesInfo
    {
        private bool _TickNodes = true;
        public bool Enable { get { return _TickNodes; } set { _TickNodes = value; } }
        public Dictionary<string,MidiTracksInfo> MidiTracksInfo = new Dictionary<string,MidiTracksInfo>();
        public List<AudioChannelsInfo> AudioChannelsLeftInfo = new List<AudioChannelsInfo>();
        public List<AudioChannelsInfo> AudioChannelsRightInfo = new List<AudioChannelsInfo>(); 
    }

    public class MidiTracksInfo
    {
        private List<string> _name = new List<string> { };
        public List<string> trackName { get { return _name; } set { _name = value; } }
        private bool _MidiNodes = true;
        public bool Enable { get { return _MidiNodes; } set { _MidiNodes = value; } }
        private bool _barIndex = true;
        public bool barIndex { get { return _barIndex; } set { _barIndex = value; } }
        private bool _beatDuration = false;
        public bool beatDuration { get { return _beatDuration; } set { _beatDuration = value; } }
        private bool _tickDuration = false;
        public bool tickDuration { get { return _tickDuration; } set { _tickDuration = value; } }
        private bool _startTick = false;
        public bool startTick { get { return _startTick; } set { _startTick = value; } }
        private bool _MinecraftStartTick = true;
        public bool MinecraftStartTick { get { return _MinecraftStartTick; } set { _MinecraftStartTick = value; } }
        private bool _channel = false;
        public bool channel { get { return _channel; } set { _channel = value; } }
        private bool _pitch = true;
        public bool pitch { get { return _pitch; } set { _pitch = value; } }
        private bool _velocity = true;
        public bool velocity { get { return _velocity; } set { _velocity = value; } }
        private float _volume = 1;
        public float volume { get { return _volume; } set { _volume = value; } }
        private bool _MinecraftDurationTick = true;
        public bool MinecraftDurationTick { get { return _MinecraftDurationTick; } set { _MinecraftDurationTick = value; } }
        private bool _pitch_end = true;
        public bool pitch_end { get { return _pitch_end; } set { _pitch_end = value; } }
        private TimbreInfo _timbre = new TimbreInfo();
        public TimbreInfo timbreInfo { get { return _timbre; } set { _timbre = value; } }
        private bool _timbre_t = false;
        public bool timbreEnable { get { return _timbre_t; } set { _timbre_t = value; } }
        private bool _timbre_st = false;
        public bool timbreStopEnable { get { return _timbre_st; } set { _timbre_st = value; } }
    }

    public class AudioChannelsInfo
    {
        private bool _AudioNodes = true;
        public bool Enable { get { return _AudioNodes; } set { _AudioNodes = value; } }
        private bool _MinecraftFrequencyPerTick = true;
        public bool MinecraftFrequencyPerTick { get { return _MinecraftFrequencyPerTick; } set { _MinecraftFrequencyPerTick = value; } }
        private bool _MinecraftVolumePerTick = true;
        public bool MinecraftVolumePerTick { get { return _MinecraftVolumePerTick; } set { _MinecraftVolumePerTick = value; } }
        private bool _MinecraftStartTick = false;
        public bool MinecraftStartTick { get { return _MinecraftStartTick; } set { _MinecraftStartTick = value; } }
    }

    public class TimbreInfo
    {
        private string _timbre = "1";
        public string playTimbre { get { return _timbre; } set { _timbre = value; } }
        private double[] _cood = new double[] { 0, 0, 0 };
        public double[] executeCood { get { return _cood; } set { _cood = value; } }

        private string _target_0 = "@a";
        public string executeTarget { get { return _target_0; } set { _target_0 = value; } }
        private string _target = "@a";
        public string playTarget { get { return _target; } set { _target = value; } }
        private string _source = "record";
        public string playSource { get { return _source; } set { _source = value; } }
        private string _overlap = null;
        public string overlapPitch { get { return _overlap; } set { _overlap = value; } }
    }
}
