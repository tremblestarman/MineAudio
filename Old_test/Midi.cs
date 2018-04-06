using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NAudio.Midi;
using System.Windows.Forms;

namespace Audio2MinecraftScore
{
    public class AudioStreamMidi
    {
        public TimeLine Serialize(string fileName, TimeLine timeLine, int tBpm = 160)
        {
            var midiFile = new MidiFile(fileName, false);
            var maxTick = 0;
            #region Head
            /*Midi Head*/
            timeLine.MidiFileFormat = midiFile.FileFormat;
            timeLine.MidiTracksCount = midiFile.Tracks;
            timeLine.MidiDeltaTicksPerQuarterNote = midiFile.DeltaTicksPerQuarterNote;
            #endregion
            #region Nodes
            /* Wirte in Midi Nodes */
            var timeSignature = midiFile.Events[0].OfType<TimeSignatureEvent>().FirstOrDefault();
            /*When SetTempo*/
            List<MidiNode> MidiNodes = new List<MidiNode>();
            for (int i = 0; i < midiFile.Tracks; i++)
            {
                var bpm = returnBpm(midiFile.Events, tBpm);
                var name = "";
                var instrument = "";
                foreach (MidiEvent midiEvent in midiFile.Events[i])
                {
                    //Get Track Name
                    if (new Regex("(?<=SequenceTrackName ).+(?=$)").Match(midiEvent.ToString()).Success) name = new Regex("(?<=SequenceTrackName ).+(?=$)").Match(midiEvent.ToString()).Value;
                    //Get Instrument Name
                    if (new Regex("(?<=PatchChange Ch: \\d+ ).+(?=$)").Match(midiEvent.ToString()).Success) instrument = new Regex("(?<=PatchChange Ch: \\d+ ).+(?=$)").Match(midiEvent.ToString()).Value;
                    /*When not End*/
                    if (!MidiEvent.IsNoteOff(midiEvent))
                    {
                        var nowTick = 0;
                        /*  
                         *  long barIndex
                         *  long beatDuration
                         *  long tickDuration
                        */
                        var MBT = GetMBT(midiEvent.AbsoluteTime, midiFile.DeltaTicksPerQuarterNote, timeSignature);

                        /*  
                         *  int startTick
                         *  int MinecraftStartTick
                         *  int channel
                         *  int pitch
                         *  int velocity
                         *  int MinecraftDurationTick
                         *  string instrument
                         *  string track_name
                        */
                        var EventAnalysis = AnalysisEvent(midiEvent, instrument);
                        if (EventAnalysis != null)
                        {
                            MidiNodes.Add(new MidiNode
                            {
                                barIndex = MBT[0],
                                beatDuration = MBT[1],
                                tickDuration = MBT[2],
                                startTick = EventAnalysis.StartTick,
                                MinecraftStartTick = (int)toMinecraftTick(EventAnalysis.StartTick, midiFile.DeltaTicksPerQuarterNote, timeSignature, bpm),
                                channel = (int)EventAnalysis.Channel,
                                pitch = (int)EventAnalysis.Pitch,
                                velocity = (int)EventAnalysis.Velocity,
                                MinecraftDurationTick = (int)toMinecraftTick(EventAnalysis.Length, midiFile.DeltaTicksPerQuarterNote, timeSignature, bpm),
                                instrument = EventAnalysis.Instrument,
                                track_name = name
                            });
                            nowTick = (int)toMinecraftTick(EventAnalysis.StartTick, midiFile.DeltaTicksPerQuarterNote, timeSignature, bpm);
                        }
                        else
                            continue;
                        if (nowTick > maxTick) maxTick = nowTick;
                    }
                }
            }
            /* Dynamicly creat and set lenth for TickNodes */
            if (timeLine.TickNodes == null) timeLine.TickNodes = new List<TickNode>();
            if (maxTick >= timeLine.TickNodes.Count)
            {
                var nowCount = timeLine.TickNodes.Count;
                for (int i = 0; i < maxTick - nowCount + 1; i++) timeLine.TickNodes.Add(new TickNode());
            }
            /* Write MidiNodes into TickNodes.MidiNodes */
            foreach(MidiNode n in MidiNodes)
            {
                if (timeLine.TickNodes[n.MinecraftStartTick].MidiNodes == null) timeLine.TickNodes[n.MinecraftStartTick].MidiNodes = new List<MidiNode>();
                timeLine.TickNodes[n.MinecraftStartTick].MidiNodes.Add(new MidiNode
                {
                    barIndex = n.barIndex,
                    beatDuration = n.beatDuration,
                    tickDuration = n.tickDuration,
                    startTick = n.startTick,
                    MinecraftStartTick = n.MinecraftStartTick,
                    channel = n.channel,
                    pitch = n.pitch,
                    velocity = n.velocity,
                    MinecraftDurationTick = n.MinecraftDurationTick,
                    track_name = n.track_name,
                });
            }
            /* WriteIn pitch_end */
            for (int i = 0; i < timeLine.TickNodes.Count; i++)
            {
                if (timeLine.TickNodes[i].MidiNodes != null)
                {
                    for (int j = 0; j < timeLine.TickNodes[i].MidiNodes.Count; j++)
                    {
                        if (timeLine.TickNodes[i].MidiNodes[j].MinecraftDurationTick != 0)
                        {
                            //get end_index and create new tick 
                            int endDuration = (timeLine.TickNodes[i].MidiNodes[j].MinecraftDurationTick == 0) ? 1 : timeLine.TickNodes[i].MidiNodes[j].MinecraftDurationTick;
                            int end_index =  + timeLine.TickNodes[i].MidiNodes[j].MinecraftStartTick;
                            if (end_index >= timeLine.TickNodes.Count)
                            {
                                var nowTick = timeLine.TickNodes.Count;
                                for (int t = 0; t < end_index - nowTick + 1; t++) timeLine.TickNodes.Add(new TickNode());
                            }
                            //find the target and writeIn pitch_end
                            if (timeLine.TickNodes[end_index].MidiNodes == null)
                            {
                                timeLine.TickNodes[end_index].MidiNodes = new List<MidiNode>();

                            }
                            timeLine.TickNodes[end_index].MidiNodes.Add(new MidiNode() { pitch_end = timeLine.TickNodes[i].MidiNodes[j].pitch });
                        }
                    }
                }
            }
            #endregion
            return timeLine;
            //minecraft tick  = AbsoluteTime / (bpm * ticksPerBeat) * 1200
        }

        /// <summary>
        /// Get the number of bar, beat, tick
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Analysis an Event
        /// </summary>
        /// <param name="midiEvent">An MidiEvent</param>
        /// <returns></returns>
        private MidiEventParameter AnalysisEvent(MidiEvent midiEvent, string instrument)
        {
            MidiEventParameter para = new MidiEventParameter();
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
        /// <summary>
        /// Get the number of beats per measure
        /// </summary>
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

        #region functional
        /// <summary>
        /// Simply convert absolute tick to Minecraft Tick
        /// </summary>
        /// <param name="eventTime"></param>
        /// <param name="ticksPerQuarterNote"></param>
        /// <param name="timeSignature"></param>
        /// <param name="bpm"></param>
        /// <returns></returns>
        private long toMinecraftTick(long eventTime, int ticksPerQuarterNote, TimeSignatureEvent timeSignature, int bpm)
        {
            int beatsPerBar = timeSignature == null ? 4 : timeSignature.Numerator;
            int ticksPerBar = timeSignature == null ? ticksPerQuarterNote * 4 : (timeSignature.Numerator * ticksPerQuarterNote * 4) / (1 << timeSignature.Denominator);
            int ticksPerBeat = ticksPerBar / beatsPerBar;
            return (long)(((double)eventTime) * 1200 / ((double)(ticksPerBeat * bpm)));
        }
        /// <summary>
        /// return BPM of the EventsCollection
        /// </summary>
        /// <param name="midiEvents">the String Contains Bmp</param>
        /// <returns></returns>
        private int returnBpm(MidiEventCollection Events, int tBpm)
        {
            if (tBpm != 160)
                return tBpm;
            else
            {
                foreach (MidiEvent avent in Events)
                {
                    var EventString = avent.ToString();
                    if (EventString.Contains("SetTempo"))
                    {
                        if (EventString.Split(' ')[2].Replace("bpm", "") != null)
                            return Int32.Parse(EventString.Split(' ')[2].Replace("bpm", ""));
                    }
                    else
                        return 160;
                }
                return 160;
            }
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
        public long StartTick { get; set;}
        public long Channel { get; set; }
        public long Velocity { get; set; }
        public long Length { get; set; }
        public long Pitch { get; set; }
        public string Instrument { get; set; }
    }
        
}