using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NAudio.Midi;

namespace Audio2Minecraft
{
    /// <summary>
    /// Midi序列操作
    /// </summary>
    public class AudioStreamMidi
    {
        /// <summary>
        /// 通过Midi生成时间序列
        /// </summary>
        /// <param name="fileName">Midi文件路径</param>
        /// <param name="timeLine">时间序列</param>
        /// <param name="rate">播放速率(New BPM = BPM * rate)</param>
        /// <returns>时间序列</returns>
        public TimeLine SerializeByRate(string fileName, TimeLine timeLine, double rate = -1, ShowProgress showProgress = null)
        {
            return Serialize(fileName, timeLine, rate, -1, showProgress);
        }
        /// <summary>
        /// 通过Midi生成时间序列
        /// </summary>
        /// <param name="fileName">Midi文件路径</param>
        /// <param name="timeLine">时间序列</param>
        /// <param name="synchroTick">节奏间隔(MidiTick)</param>
        /// <returns>时间序列</returns>
        public TimeLine SerializeByBeat(string fileName, TimeLine timeLine, int synchroTick = -1, ShowProgress showProgress = null)
        {
            return Serialize(fileName, timeLine, -1, synchroTick, showProgress);
        }
        /// <summary>
        /// 通过Midi生成时间序列
        /// </summary>
        /// <param name="fileName">Midi文件路径</param>
        /// <param name="timeLine">时间序列</param>
        /// <param name="rate">播放速率(New BPM = BPM * rate)</param>
        /// <param name="synchroTick">节奏间隔(MidiTick),设置此项后将忽略播放速率</param>
        /// <returns>时间序列</returns>
        public TimeLine Serialize(string fileName, TimeLine timeLine, double rate = -1, int synchroTick = -1, ShowProgress showProgress = null)
        {
            try
            {
                if (timeLine == null) timeLine = new TimeLine();
                var midiFile = new MidiFile(fileName, false);
                #region HeadParam
                timeLine.Param["MidiFileFormat"].Value = midiFile.FileFormat;
                timeLine.Param["MidiTracksCount"].Value = midiFile.Tracks;
                timeLine.Param["MidiDeltaTicksPerQuarterNote"].Value = midiFile.DeltaTicksPerQuarterNote;
                #endregion
                #region Nodes
                //Public Event
                var timeSignature = midiFile.Events[0].OfType<TimeSignatureEvent>().FirstOrDefault();
                double bpm = 0;
                timeLine.BeatsPerBar = timeSignature == null ? 4 : timeSignature.Numerator;
                timeLine.TicksPerBar = timeSignature == null ? midiFile.DeltaTicksPerQuarterNote * 4 : (timeSignature.Numerator * midiFile.DeltaTicksPerQuarterNote * 4) / (1 << timeSignature.Denominator);
                timeLine.TicksPerBeat = timeLine.TicksPerBar / timeLine.BeatsPerBar;
                #region MidiFile -> MidiNodes(Unordered)
                List<MidiNode> MidiNodes = new List<MidiNode>();
                //Foreach Events in MidiFile
                for (int i = 0; i < midiFile.Tracks; i++)
                {
                    //Track Events
                    var track = "";
                    var instrument = "";
                    var vol = -1;
                    var pan = -1;
                    foreach (MidiEvent midiEvent in midiFile.Events[i])
                    {
                        //Event BPM
                        if (new Regex("(?<=SetTempo )\\d+(?=bpm \\(\\d+\\))").Match(midiEvent.ToString()).Success)
                        {
                            bpm = Int32.Parse(new Regex("(?<=SetTempo )\\d+(?=bpm \\(\\d+\\))").Match(midiEvent.ToString()).Value);
                            if (rate > 0) bpm *= rate;
                            MidiNodes.Add(new MidiNode() { IsEvent = true, Param = new Dictionary<string, _Node_INT>() { { "BeatPerMinute", new _Node_INT() { Value = (int)bpm } }, { "DeltaTickStart", new _Node_INT() { Value = midiEvent.DeltaTime } }, { "MinecraftTickStart", new _Node_INT() { Value = 0 } } } });
                        }
                        //Event Track Name
                        if (new Regex("(?<=SequenceTrackName ).+(?=$)").Match(midiEvent.ToString()).Success) track = new Regex("(?<=SequenceTrackName ).+(?=$)").Match(midiEvent.ToString()).Value;
                        //Event Instrument Name
                        if (new Regex("(?<=PatchChange Ch: \\d+ ).+(?=$)").Match(midiEvent.ToString()).Success) instrument = new Regex("(?<=PatchChange Ch: \\d+ ).+(?=$)").Match(midiEvent.ToString()).Value;
                        //Event Track Volume
                        if (new Regex("(?<=MainVolume Value )\\d*(?=$)").Match(midiEvent.ToString()).Success) Int32.TryParse(new Regex("(?<=MainVolume Value )\\d*(?=$)").Match(midiEvent.ToString()).Value, out vol);
                        //Event Track Pan
                        if (new Regex("(?<=Pan Value )\\d*(?=$)").Match(midiEvent.ToString()).Success) Int32.TryParse(new Regex("(?<=Pan Value )\\d*(?=$)").Match(midiEvent.ToString()).Value, out pan);
                        if (!MidiEvent.IsNoteOff(midiEvent))
                        {
                            //Get Param
                            var MBT = GetMBT(midiEvent.AbsoluteTime, midiFile.DeltaTicksPerQuarterNote, timeSignature);
                            var EventAnalysis = AnalysisEvent(midiEvent, instrument);
                            //Write into MidiNodes
                            if (EventAnalysis != null)
                            {
                                var MidiNode = new MidiNode();
                                #region Param
                                //Time-related
                                MidiNode.Param["DeltaTickStart"].Value = (int)EventAnalysis.StartTick;
                                //MidiNode Starts Needs more Calculation
                                MidiNode.Param["DeltaTickDuration"].Value = (int)EventAnalysis.Length;
                                MidiNode.Param["MinecraftTickDuration"].Value = (int)MinecraftTickDuration(EventAnalysis.Length, midiFile.DeltaTicksPerQuarterNote, timeSignature, (int)bpm);
                                MidiNode.Param["BeatPerMinute"].Value = (int)bpm;
                                //Bar-related
                                MidiNode.Param["BarIndex"].Value = (int)MBT[0];
                                MidiNode.Param["BeatDuration"].Value = (int)MBT[1];
                                //Note-related
                                MidiNode.Param["Channel"].Value = (int)EventAnalysis.Channel;
                                MidiNode.Param["Pitch"].Value = (int)EventAnalysis.Pitch;
                                MidiNode.Param["Velocity"].Value = (int)EventAnalysis.Velocity;
                                MidiNode.Param["Panning"].Value = pan;
                                //Track-related
                                MidiNode.Instrument = EventAnalysis.Instrument;
                                MidiNode.TrackName = track;
                                //PlaySound-related
                                MidiNode.PlaySound = new PlaySoundInfo();
                                MidiNode.PlaySound.MandaVolume = (vol < 0) ? 100 : vol;
                                MidiNode.PlaySound.SetPan(pan);
                                //Generate Track & Instrument List
                                var currentTrack = timeLine.TrackList.AsEnumerable().FirstOrDefault(t => t.Name == track);
                                if (currentTrack == null) { currentTrack = new TimeLine.MidiSettingInspector { Name = track, Type = TimeLine.MidiSettingType.Track, Enable = true }; timeLine.TrackList.Add(currentTrack); } //Add new Track
                                var currentInstrument = timeLine.InstrumentList.AsEnumerable().FirstOrDefault(ins => ins.Name == EventAnalysis.Instrument);
                                if (currentInstrument == null) { currentInstrument = new TimeLine.MidiSettingInspector { Name = EventAnalysis.Instrument, Type = TimeLine.MidiSettingType.Instrument, Enable = true }; timeLine.InstrumentList.Add(currentInstrument); } //Add new Instrument
                                if (!currentTrack.Instruments.Any(ins => ins.Name == EventAnalysis.Instrument))
                                {
                                    currentTrack.Instruments.Add(currentInstrument);
                                    currentTrack.InstrumentsUid.Add(currentInstrument.Uid);
                                }//Line Track
                                if (!currentInstrument.Tracks.Any(t => t.Name == track))
                                {
                                    currentInstrument.Tracks.Add(currentTrack);
                                    currentInstrument.TracksUid.Add(currentTrack.Uid);
                                }
                                #endregion
                                MidiNodes.Add(MidiNode);
                            }
                        }
                    }
                }
                #endregion
                #region MidiNodes in Order
                /* Set Total Progress */ timeLine.totalProgress = MidiNodes.Count * 2;
                bpm = 0;
                long bpm_key_t = 0; //When BPM Changes
                long bpm_key_mt = 0;
                MidiNodes = (from n in MidiNodes
                             orderby n.Param["DeltaTickStart"].Value
                             select n).ToList(); //Make Nodes in Order
                int synchroCount = 0;
                for (int i = 0; i < MidiNodes.Count; i++) //Calculate Tick Start
                {
                    var n = MidiNodes[i];
                    if (n.IsEvent)
                    {
                        if (bpm != 0)
                            bpm_key_mt = MinecraftTickStart(n.Param["DeltaTickStart"].Value, midiFile.DeltaTicksPerQuarterNote, timeSignature, bpm, bpm_key_t, bpm_key_mt);
                        bpm = n.Param["BeatPerMinute"].Value;
                        bpm_key_t = n.Param["DeltaTickStart"].Value;
                    }
                    else n.Param["BeatPerMinute"].Value = (int)bpm;
                    if (synchroTick <= 0) n.Param["MinecraftTickStart"].Value = (int)MinecraftTickStart(n.Param["DeltaTickStart"].Value, midiFile.DeltaTicksPerQuarterNote, timeSignature, bpm, bpm_key_t, bpm_key_mt);
                    else
                    { //Using synchroTick
                        n.Param["MinecraftTickStart"].Value = n.Param["DeltaTickStart"].Value / synchroTick;
                        if (n.Param["DeltaTickStart"].Value % synchroTick == 0) synchroCount++;
                    }
                    /* Update Current Progress */ timeLine.currentProgress++; if (showProgress != null && timeLine.totalProgress > 0) showProgress((double)timeLine.currentProgress / timeLine.totalProgress);
                }
                timeLine.SynchronousRate = (double)synchroCount / MidiNodes.Count;
                #endregion
                #region MidiNodes -> TickNodes
                //Creat and Set Lenth of TickNodes
                if (timeLine.TickNodes == null) timeLine.TickNodes = new List<TickNode>();
                var maxTick = MidiNodes.Max(n => n.Param["MinecraftTickStart"].Value);
                if (maxTick >= timeLine.TickNodes.Count)
                {
                    var nowCount = timeLine.TickNodes.Count;
                    for (int i = 0; i < maxTick - nowCount + 1; i++) timeLine.TickNodes.Add(new TickNode());
                }
                //MidiNodes -> TickNodes
                foreach (MidiNode node in MidiNodes)
                {
                    var index = node.Param["MinecraftTickStart"].Value;
                    var track = node.TrackName;
                    var instrument = node.Instrument;
                    if (timeLine.TickNodes[index].MidiTracks.ContainsKey(track) == false) timeLine.TickNodes[index].MidiTracks.Add(track, new Dictionary<string, List<MidiNode>>());
                    if (timeLine.TickNodes[index].MidiTracks[track].ContainsKey(instrument) == false) timeLine.TickNodes[index].MidiTracks[track].Add(instrument, new List<MidiNode>());
                    timeLine.TickNodes[index].MidiTracks[track][instrument].Add(node);
                    timeLine.TickNodes[index].BPM = node.Param["BeatPerMinute"].Value;
                    timeLine.TickNodes[index].CurrentTick = index;
                    /* Update Current Progress */ timeLine.currentProgress++; if (showProgress != null && timeLine.totalProgress > 0) showProgress((double)timeLine.currentProgress / timeLine.totalProgress);
                }
                #endregion
                timeLine.Param["TotalTicks"].Value = timeLine.TickNodes.Count;
                return timeLine;
                //minecraft tick  = AbsoluteTime / (bpm * ticksPerBeat) * 1200
                #endregion
            }
            catch
            {
                return null;
            }
        }
        #region Param
        private long[] GetMBT(long eventTime, int ticksPerQuarterNote, TimeSignatureEvent timeSignature)
        {
            int beatsPerBar = timeSignature == null ? 4 : timeSignature.Numerator;
            int ticksPerBar = timeSignature == null ? ticksPerQuarterNote * 4 : (timeSignature.Numerator * ticksPerQuarterNote * 4) / (1 << timeSignature.Denominator);
            int ticksPerBeat = ticksPerBar / beatsPerBar;
            long bar = 1 + (eventTime / ticksPerBar);
            long beat = 1 + ((eventTime % ticksPerBar) / ticksPerBeat);
            long tick = eventTime % ticksPerBeat;
            //bar, beat, tick
            long[] mbt = new[] { bar, beat, tick };
            return mbt;
        }
        private MidiEventParameter AnalysisEvent(MidiEvent midiEvent, string instrument)
        {
            MidiEventParameter para = new MidiEventParameter();
            if (instrument == "") instrument = "Default";
            para.Instrument = instrument; //default instrument
            if (new Regex("(?<=^\\d+ )NoteOn(?= Ch:)").Match(midiEvent.ToString()).Success)
            {
                //Get StartTick
                if (new Regex("^\\d+(?= NoteOn )").Match(midiEvent.ToString()).Success)
                {
                    para.StartTick = long.Parse(new Regex("^\\d+(?= NoteOn )").Match(midiEvent.ToString()).Value);
                }
                //Get Channel
                if (new Regex("(?<=Ch: )\\d+(?=\\s|$)").Match(midiEvent.ToString()).Success)
                {
                    para.Channel = long.Parse(new Regex("(?<=Ch: )\\d+(?=\\s|$)").Match(midiEvent.ToString()).Value);
                }
                //Get Velocity
                if (new Regex("(?<=Vel:)\\d+(?=\\s|$)").Match(midiEvent.ToString()).Success)
                {
                    para.Velocity = long.Parse(new Regex("(?<=Vel:)\\d+(?=\\s|$)").Match(midiEvent.ToString()).Value);
                }
                //Get Length
                if (new Regex("(?<=Len: )\\d+(?=\\s|$)").Match(midiEvent.ToString()).Success)
                {
                    para.Length = long.Parse(new Regex("(?<=Len: )\\d+(?=\\s|$)").Match(midiEvent.ToString()).Value);
                }
                //Get Pitch & Change Instrument
                if (new Regex("(?<=Ch: \\d+ ).*(?= Vel:\\d+)").Match(midiEvent.ToString()).Success)
                {
                    var Change = new Regex("(?<=Ch: \\d+ ).*(?= Vel:\\d+)").Match(midiEvent.ToString()).Value;
                    var pitch = returnPitch(Change);
                    if (pitch == -1) para.Instrument = Change;
                    else para.Pitch = pitch;
                }
                return para;
            }
            else
                return null;
        }
        private int GetBeatsPerMeasure(IEnumerable<MidiEvent> midiEvents)
        {
            int beatsPerMeasure = 4;
            foreach (MidiEvent midiEvent in midiEvents)
            {
                TimeSignatureEvent tse = midiEvent as TimeSignatureEvent;
                if (tse != null)
                {
                    beatsPerMeasure = tse.Numerator;
                }
            }
            return beatsPerMeasure;
        }
        #endregion
        /// <summary>
        /// Get Minecraft Tick When a Midi Event Starts
        /// </summary>
        /// <param name="eventTime">Current Tick</param>
        /// <param name="ticksPerQuarterNote">TP QuarterNote</param>
        /// <param name="timeSignature">timeSignate</param>
        /// <param name="bpm">Current BPM</param>
        /// <param name="bpm_key_t">Current Tick of BPM Event</param>
        /// <param name="bpm_key_mt">Current Minecraft Tick of BPM Event</param>
        /// <returns>Tick Start</returns>
        #region Functional
        private long MinecraftTickStart(long eventTime, int ticksPerQuarterNote, TimeSignatureEvent timeSignature, double bpm, long bpm_key_t, long bpm_key_mt)
        {
            int beatsPerBar = timeSignature == null ? 4 : timeSignature.Numerator;
            int ticksPerBar = timeSignature == null ? ticksPerQuarterNote * 4 : (timeSignature.Numerator * ticksPerQuarterNote * 4) / (1 << timeSignature.Denominator);
            int ticksPerBeat = ticksPerBar / beatsPerBar;
            return (long)(((double)(eventTime - bpm_key_t)) * 1200 / ((double)(ticksPerBeat * bpm)) + bpm_key_mt); //Tick per Minute
        }
        /// <summary>
        /// Get Duration of a Midi Event as Minecraft Tick
        /// </summary>
        /// <param name="eventTime">Current Tick</param>
        /// <param name="ticksPerQuarterNote">TP QuarterNote</param>
        /// <param name="timeSignature">timeSignate</param>
        /// <param name="bpm">Current BPM</param>
        /// <returns>Tick Duration</returns>
        private long MinecraftTickDuration(long eventTime, int ticksPerQuarterNote, TimeSignatureEvent timeSignature, int bpm)
        {
            int beatsPerBar = timeSignature == null ? 4 : timeSignature.Numerator;
            int ticksPerBar = timeSignature == null ? ticksPerQuarterNote * 4 : (timeSignature.Numerator * ticksPerQuarterNote * 4) / (1 << timeSignature.Denominator);
            int ticksPerBeat = ticksPerBar / beatsPerBar;
            return (long)(((double)eventTime) * 1200 / ((double)(ticksPerBeat * bpm)));
        }
        private long returnPitch(string NoteName)
        {
            try
            {
                string regex = Regex.Match(NoteName, @"\d+$").Value;
                long i = 0;
                if (regex != "")
                {
                    i = long.Parse(regex);
                    string name = NoteName.Replace(regex, "");
                    Dictionary<string, long> name_i = new Dictionary<string, long>();
                    name_i.Add("C", 0); name_i.Add("C#", 1); name_i.Add("D", 2); name_i.Add("D#", 3); name_i.Add("E", 4); name_i.Add("F", 5); name_i.Add("F#", 6); name_i.Add("G", 7); name_i.Add("G#", 8); name_i.Add("A", 9); name_i.Add("A#", 10); name_i.Add("B", 11);
                    return i * 12 + name_i[name];
                }
            }
            catch
            {
                return -1;
            }
            return -1;
        }
        #endregion
    }

    class MidiEventParameter
    {
        public long StartTick { get; set; }
        public long Channel { get; set; }
        public long Velocity { get; set; }
        public long Length { get; set; }
        public long Pitch { get; set; }
        public string Instrument { get; set; }
    }
}