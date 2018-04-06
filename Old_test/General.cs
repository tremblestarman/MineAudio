using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fNbt;
using System.Windows.Forms;

namespace Audio2MinecraftScore
{
    public class Audio2ScoreOperation
    {
        /// <summary>
        /// to serialize a timeline
        /// </summary>
        /// <param name="midiPath">path of midi file</param>
        /// <param name="wavePath">path of wave file</param>
        /// <param name="tBpm">bpm of midi (default is 160)</param>
        /// <param name="fre_count">count of frequency samples (default is 1)</param>
        /// <param name="vol_count">count of volume samples (default is 1)</param>
        /// <param name="tick_cycle">cycle of wave samples (default is 1)</param>
        /// <returns></returns>
        public TimeLine SerializeTimeLine(string midiPath = null, string wavePath = null, int tBpm = 160, int fre_count = 1, int vol_count = 1, int tick_cycle = 1)
        {
            var timeLine = new TimeLine();
            if (midiPath != null && midiPath != "") timeLine = new AudioStreamMidi().Serialize(midiPath, timeLine, tBpm);
            if (wavePath != null && wavePath != "") timeLine = new AudioStreamWave().Serialize(wavePath, timeLine, fre_count, vol_count, tick_cycle);

            //generate TimelineInfo
            var timeLineInfo = new TimeLineInfo();
            for (int i = 0; i < timeLine.TickNodes.Count; i++)
            {
                timeLine.TickNodes[i].currentTick = i;
                if (timeLine.TickNodes[i].MidiNodes != null)
                    for (int j = 0; j < timeLine.TickNodes[i].MidiNodes.Count; j++)
                    {
                        if (!timeLineInfo.TickNodesInfo.MidiTracksInfo.ContainsKey(timeLine.TickNodes[i].MidiNodes[j].track_name)) timeLineInfo.TickNodesInfo.MidiTracksInfo.Add(timeLine.TickNodes[i].MidiNodes[j].track_name, new MidiTracksInfo());
                    }
                if (timeLine.TickNodes[i].AudioNodesLeft != null)
                    for (int j = 0; j < timeLine.TickNodes[i].AudioNodesLeft.Count; j++)
                    {
                        if (timeLineInfo.TickNodesInfo.AudioChannelsLeftInfo.Count <= j) timeLineInfo.TickNodesInfo.AudioChannelsLeftInfo.Add(new AudioChannelsInfo());
                    }
                if (timeLine.TickNodes[i].AudioNodesRight != null)
                    for (int j = 0; j < timeLine.TickNodes[i].AudioNodesRight.Count; j++)
                    {
                        if (timeLineInfo.TickNodesInfo.AudioChannelsRightInfo.Count <= j) timeLineInfo.TickNodesInfo.AudioChannelsRightInfo.Add(new AudioChannelsInfo());
                    }
            }
            timeLine.timeLineInfo = timeLineInfo;
            return timeLine;
        }
        public void SerializeSchematic(TimeLine timeLine, V2 SizeParam, string exportPath = "C:\\time.schematic")
        {
            var commmandLine = filterToCommands(timeLine);
            MessageBox.Show("1");
            ExportSchematic(exportPath, commmandLine, SizeParam);
            MessageBox.Show("2");
        }
        private CommandLine filterToCommands(TimeLine timeLine)
        {
            var timeLineInfo = timeLine.timeLineInfo;
            var commandLine = new CommandLine();
            //name of scoreboards
            var scoreboards = new List<string>();
            //name of track, batch count of the track
            var tracks = new Dictionary<string, int>();
            #region Initialize command for Public properties
            /* public
             *  - add track
             *  - add scoreboard
             */
            tracks.Add("property", 1);
            if (timeLineInfo.MidiFileFormat)
            { scoreboards.Add("MidiFileFormat"); commandLine.General.Add(setCommand("property", "MidiFileFormat", timeLine.MidiFileFormat)); }
            if (timeLineInfo.MidiTracksCount)
            { scoreboards.Add("MidiTracksCount"); commandLine.General.Add(setCommand("property", "MidiTracksCount", timeLine.MidiTracksCount)); }
            if (timeLineInfo.MidiDeltaTicksPerQuarterNote)
            { scoreboards.Add("MidiDeltaTicksPerQuarterNote"); commandLine.General.Add(setCommand("property", "MidiDeltaTicksPerQuarterNote", timeLine.MidiDeltaTicksPerQuarterNote)); }
            if (timeLineInfo.AudioFileFormat)
            { scoreboards.Add("AudioFileFormat"); commandLine.General.Add(setCommand("property", "AudioFileFormat", timeLine.AudioFileFormat)); }
            if (timeLineInfo.AudioBit)
            { scoreboards.Add("AudioBit"); commandLine.General.Add(setCommand("property", "AudioBit", timeLine.AudioBit)); }
            if (timeLineInfo.currentTick)
            { scoreboards.Add("currentTick"); }
            #endregion
            #region Initialize command for TimeLine properties
            /* timeline
             *  - midi
             *  - audio wave left
             *  - audio wave right
             */
            for (int i = 0; i < timeLine.TickNodes.Count; i++)
            {
                if (commandLine.Keyframe.Count <= i) commandLine.Keyframe.Add(new Command());
                if (timeLine.timeLineInfo.currentTick)
                {
                        commandLine.Keyframe[i].Commands.Add(setCommand("property", "currentTick", i));
                }
                #region Midi
                    /* midi
                     *  - midi track
                     *    - midi node
                     *      - add track
                     *      - add scoreboard
                     */
                if (timeLine.TickNodes[i].MidiNodes != null)
                {
                    //foreach nodes
                    for (int j = 0; j < timeLine.TickNodes[i].MidiNodes.Count; j++)
                    {
                        var c = timeLine.TickNodes[i].MidiNodes[j].track_name;
                        if (timeLineInfo.AllMidiEnable && timeLineInfo.TickNodesInfo.MidiTracksInfo[c].Enable)
                        {
                            /* target */
                            int pre = -1;
                            string keyTrack = "midi_" + timeLine.TickNodes[i].MidiNodes[j].track_name;
                            if (!tracks.ContainsKey(keyTrack)) tracks.Add(keyTrack, 1); //create new track
                            else if (tracks[keyTrack] < j) { pre = tracks[keyTrack]; tracks[keyTrack] = j; } //refresh track batch count
                            var Output = false;
                            /* scoreboard & commands */
                            if ((timeLineInfo.AllMidibarIndex && timeLineInfo.TickNodesInfo.MidiTracksInfo[c].barIndex) && !(timeLineInfo.IgnoreZero && timeLine.TickNodes[i].MidiNodes[j].barIndex == 0))
                            {
                                if (!scoreboards.Contains("midi_barIndex")) scoreboards.Add("midi_barIndex");
                                commandLine.Keyframe[i].Commands.Add(setCommand(keyTrack + "_" + j.ToString(), "midi_barIndex", (int)timeLine.TickNodes[i].MidiNodes[j].barIndex));
                                Output = true;
                            }
                            if ((timeLineInfo.AllMidibeatDuration && timeLineInfo.TickNodesInfo.MidiTracksInfo[c].beatDuration) && !(timeLineInfo.IgnoreZero && timeLine.TickNodes[i].MidiNodes[j].beatDuration == 0))
                            {
                                if (!scoreboards.Contains("midi_beatDur")) scoreboards.Add("midi_beatDur");
                                commandLine.Keyframe[i].Commands.Add(setCommand(keyTrack + "_" + j.ToString(), "midi_beatDur", (int)timeLine.TickNodes[i].MidiNodes[j].beatDuration));
                                Output = true;
                            }
                            if ((timeLineInfo.AllMiditickDuration && timeLineInfo.TickNodesInfo.MidiTracksInfo[c].tickDuration) && !(timeLineInfo.IgnoreZero && timeLine.TickNodes[i].MidiNodes[j].tickDuration == 0))
                            {
                                if (!scoreboards.Contains("midi_tickDur")) scoreboards.Add("midi_tickDur");
                                commandLine.Keyframe[i].Commands.Add(setCommand(keyTrack + "_" + j.ToString(), "midi_tickDur", (int)timeLine.TickNodes[i].MidiNodes[j].tickDuration));
                                Output = true;
                            }
                            if ((timeLineInfo.AllMidistartTick && timeLineInfo.TickNodesInfo.MidiTracksInfo[c].startTick) && !(timeLineInfo.IgnoreZero && timeLine.TickNodes[i].MidiNodes[j].startTick == 0))
                            {
                                if (!scoreboards.Contains("midi_startTick")) scoreboards.Add("midi_startTick");
                                commandLine.Keyframe[i].Commands.Add(setCommand(keyTrack + "_" + j.ToString(), "midi_startTick", (int)timeLine.TickNodes[i].MidiNodes[j].startTick));
                                Output = true;
                            }
                            if ((timeLineInfo.AllMidiMinecraftStartTick && timeLineInfo.TickNodesInfo.MidiTracksInfo[c].MinecraftStartTick) && !(timeLineInfo.IgnoreZero && timeLine.TickNodes[i].MidiNodes[j].MinecraftStartTick == 0))
                            {
                                if (!scoreboards.Contains("midi_StartTick")) scoreboards.Add("midi_StartTick");
                                commandLine.Keyframe[i].Commands.Add(setCommand(keyTrack + "_" + j.ToString(), "midi_StartTick", timeLine.TickNodes[i].MidiNodes[j].MinecraftStartTick));
                                Output = true;
                            }
                            if ((timeLineInfo.AllMidichannel && timeLineInfo.TickNodesInfo.MidiTracksInfo[c].channel) && !(timeLineInfo.IgnoreZero && timeLine.TickNodes[i].MidiNodes[j].channel == 0))
                            {
                                if (!scoreboards.Contains("midi_channel")) scoreboards.Add("midi_channel");
                                commandLine.Keyframe[i].Commands.Add(setCommand(keyTrack + "_" + j.ToString(), "midi_channel", timeLine.TickNodes[i].MidiNodes[j].channel));
                                Output = true;
                            }
                            if ((timeLineInfo.AllMidipitch && timeLineInfo.TickNodesInfo.MidiTracksInfo[c].pitch) && !(timeLineInfo.IgnoreZero && timeLine.TickNodes[i].MidiNodes[j].pitch == 0))
                            {
                                if (!scoreboards.Contains("midi_pitch")) scoreboards.Add("midi_pitch");
                                commandLine.Keyframe[i].Commands.Add(setCommand(keyTrack + "_" + j.ToString(), "midi_pitch", timeLine.TickNodes[i].MidiNodes[j].pitch));
                                Output = true;
                            }
                            if ((timeLineInfo.AllMidivelocity && timeLineInfo.TickNodesInfo.MidiTracksInfo[c].velocity) && !(timeLineInfo.IgnoreZero && timeLine.TickNodes[i].MidiNodes[j].velocity == 0))
                            {
                                if (!scoreboards.Contains("midi_velocity")) scoreboards.Add("midi_velocity");
                                commandLine.Keyframe[i].Commands.Add(setCommand(keyTrack + "_" + j.ToString(), "midi_velocity", timeLine.TickNodes[i].MidiNodes[j].velocity));
                                Output = true;
                            }
                            if ((timeLineInfo.AllMidiMinecraftDurationTick && timeLineInfo.TickNodesInfo.MidiTracksInfo[c].MinecraftDurationTick) && !(timeLineInfo.IgnoreZero && timeLine.TickNodes[i].MidiNodes[j].MinecraftDurationTick == 0))
                            {
                                if (!scoreboards.Contains("midi_DurTick")) scoreboards.Add("midi_DurTick");
                                commandLine.Keyframe[i].Commands.Add(setCommand(keyTrack + "_" + j.ToString(), "midi_DurTick", timeLine.TickNodes[i].MidiNodes[j].MinecraftDurationTick));
                                Output = true;
                            }
                            if ((timeLineInfo.AllMidipitch_end && timeLineInfo.AllMidipitch_end && timeLineInfo.TickNodesInfo.MidiTracksInfo[c].pitch_end) && !(timeLineInfo.IgnoreZero && timeLine.TickNodes[i].MidiNodes[j].pitch_end == 0))
                            {
                                if (!scoreboards.Contains("midi_pitch_end")) scoreboards.Add("midi_pitch_end");
                                commandLine.Keyframe[i].Commands.Add(setCommand(keyTrack + "_" + j.ToString(), "midi_pitch_end", timeLine.TickNodes[i].MidiNodes[j].pitch_end));
                                Output = true;
                            }
                            #region playsound
                            //output sound
                            var pitch = timeLine.TickNodes[i].MidiNodes[j].pitch.ToString();
                            if (timeLineInfo.TickNodesInfo.MidiTracksInfo[c].timbreEnable && !(timeLineInfo.IgnoreZero && timeLine.TickNodes[i].MidiNodes[j].pitch == 0))
                            {
                                if (timeLineInfo.TickNodesInfo.MidiTracksInfo[c].timbreInfo.overlapPitch != null) pitch = timeLineInfo.TickNodesInfo.MidiTracksInfo[c].timbreInfo.overlapPitch;
                                string cood = "~" + timeLineInfo.TickNodesInfo.MidiTracksInfo[c].timbreInfo.executeCood[0] + " ~" + timeLineInfo.TickNodesInfo.MidiTracksInfo[c].timbreInfo.executeCood[1] + " ~" + timeLineInfo.TickNodesInfo.MidiTracksInfo[c].timbreInfo.executeCood[2];
                                commandLine.Keyframe[i].Commands.Add("execute " + timeLineInfo.TickNodesInfo.MidiTracksInfo[c].timbreInfo.executeTarget + " ~ ~ ~ playsound " + timeLineInfo.TickNodesInfo.MidiTracksInfo[c].timbreInfo.playTimbre + "." + pitch + " " + timeLineInfo.AllMidiTimbreInfo.playSource + " " + timeLineInfo.TickNodesInfo.MidiTracksInfo[c].timbreInfo.playTarget + " " + cood + " " + (((float)timeLine.TickNodes[i].MidiNodes[j].velocity * timeLineInfo.TickNodesInfo.MidiTracksInfo[c].volume / 100) > 2 ? 2 : (float)timeLine.TickNodes[i].MidiNodes[j].velocity * timeLineInfo.TickNodesInfo.MidiTracksInfo[c].volume / 100).ToString());
                            }
                            else if (timeLineInfo.AllMidiTimbreEnable && !(timeLineInfo.IgnoreZero && timeLine.TickNodes[i].MidiNodes[j].pitch == 0))
                            {
                                if (timeLineInfo.AllMidiTimbreInfo.overlapPitch != null) pitch = timeLineInfo.AllMidiTimbreInfo.overlapPitch;
                                string cood = "~" + timeLineInfo.AllMidiTimbreInfo.executeCood[0] + " ~" + timeLineInfo.AllMidiTimbreInfo.executeCood[1] + " ~" + timeLineInfo.AllMidiTimbreInfo.executeCood[2];
                                commandLine.Keyframe[i].Commands.Add("execute " + timeLineInfo.AllMidiTimbreInfo.executeTarget + " ~ ~ ~ playsound " + timeLineInfo.AllMidiTimbreInfo.playTimbre + "." + pitch + " " + timeLineInfo.AllMidiTimbreInfo.playSource + " " + timeLineInfo.AllMidiTimbreInfo.playTarget + " " + cood + " " + (((float)timeLine.TickNodes[i].MidiNodes[j].velocity * timeLineInfo.AllMidivolume / 100) > 2 ? 2 : (float)timeLine.TickNodes[i].MidiNodes[j].velocity * timeLineInfo.AllMidivolume / 100).ToString());
                            }
                            //stop sound
                            var pitch_end = timeLine.TickNodes[i].MidiNodes[j].pitch_end.ToString();
                            if (timeLineInfo.TickNodesInfo.MidiTracksInfo[c].timbreStopEnable && timeLineInfo.TickNodesInfo.MidiTracksInfo[c].timbreEnable && !(timeLineInfo.IgnoreZero && timeLine.TickNodes[i].MidiNodes[j].pitch_end == 0))
                            {
                                if (timeLineInfo.TickNodesInfo.MidiTracksInfo[c].timbreInfo.overlapPitch != null) pitch_end = timeLineInfo.TickNodesInfo.MidiTracksInfo[c].timbreInfo.overlapPitch;
                                commandLine.Keyframe[i].Commands.Add("execute " + timeLineInfo.TickNodesInfo.MidiTracksInfo[c].timbreInfo.executeTarget + " ~ ~ ~ stopsound " + timeLineInfo.TickNodesInfo.MidiTracksInfo[c].timbreInfo.playTarget + " " + timeLineInfo.TickNodesInfo.MidiTracksInfo[c].timbreInfo.playSource + " " + timeLineInfo.TickNodesInfo.MidiTracksInfo[c].timbreInfo.playTimbre + "." + pitch_end);
                            }
                            else if (timeLineInfo.timbreStopEnable && timeLineInfo.AllMidiTimbreEnable && !(timeLineInfo.IgnoreZero && timeLine.TickNodes[i].MidiNodes[j].pitch_end == 0))
                            {
                                if (timeLineInfo.AllMidiTimbreInfo.overlapPitch != null) pitch_end = timeLineInfo.AllMidiTimbreInfo.overlapPitch;
                                commandLine.Keyframe[i].Commands.Add("execute " + timeLineInfo.AllMidiTimbreInfo.executeTarget + " ~ ~ ~ stopsound " + timeLineInfo.AllMidiTimbreInfo.playTarget + " " + timeLineInfo.AllMidiTimbreInfo.playSource + " " + timeLineInfo.AllMidiTimbreInfo.playTimbre + "." + pitch_end);
                            }
                            #endregion
                            if (!Output) // empty nodes
                            {
                                if (pre == -1) tracks.Remove(keyTrack); //delect the new track
                                else tracks[keyTrack] = pre; //undo the value
                            }
                        }
                    }
                }
                #endregion
                #region WaveLeft
                /* wave_left
                 *  - wave track
                 *    - wave node
                 *      - add track
                 *      - add scoreboard
                 */
                if (timeLine.TickNodes[i].AudioNodesLeft != null)
                {
                    //foreach channel
                    for (int j = 0; j < timeLine.TickNodes[i].AudioNodesLeft.Count; j++)
                    {
                        if (timeLineInfo.AllAudioLeftEnable && timeLineInfo.TickNodesInfo.AudioChannelsLeftInfo[j].Enable)
                        {
                            /* target */
                            int pre = -1;
                            string keyTrack = "wavel";
                            if (!tracks.ContainsKey(keyTrack)) tracks.Add(keyTrack, 1); //create new track
                            else if (tracks[keyTrack] < j) { pre = tracks[keyTrack]; tracks[keyTrack] = j; } //refresh track batch count                                                                    
                            var Output = false;
                            /* scoreboard & commands */
                            if ((timeLineInfo.AllAudioMinecraftStartTickL && timeLineInfo.TickNodesInfo.AudioChannelsLeftInfo[j].MinecraftStartTick) && !(timeLineInfo.IgnoreZero && timeLine.TickNodes[i].AudioNodesLeft[j].MinecraftStartTick == 0))
                            {
                                if (!scoreboards.Contains("wave_StartTick")) scoreboards.Add("wave_StartTick");
                                commandLine.Keyframe[i].Commands.Add(setCommand(keyTrack, "wave_StartTick", (int)timeLine.TickNodes[i].AudioNodesLeft[j].MinecraftStartTick));
                                Output = true;
                            }
                            if ((timeLineInfo.AllAudioMinecraftFrequencyPerTickL && timeLineInfo.TickNodesInfo.AudioChannelsLeftInfo[j].MinecraftFrequencyPerTick) && !(timeLine.TickNodes[i].AudioNodesLeft[j].MinecraftFrequencyPerTick.Count == 0))
                            {
                                for (int t = 0; t < timeLine.TickNodes[i].AudioNodesLeft[j].MinecraftFrequencyPerTick.Count; t++)
                                {
                                    if (!scoreboards.Contains("wave_Fre_" + t.ToString())) scoreboards.Add("wave_Fre_" + t.ToString());
                                    commandLine.Keyframe[i].Commands.Add(setCommand(keyTrack, "wave_Fre_" + t.ToString(), (int)timeLine.TickNodes[i].AudioNodesLeft[j].MinecraftFrequencyPerTick[t]));
                                }
                                Output = true;
                            }
                            if ((timeLineInfo.AllAudioMinecraftVolumePerTickL && timeLineInfo.TickNodesInfo.AudioChannelsLeftInfo[j].MinecraftVolumePerTick) && !(timeLine.TickNodes[i].AudioNodesLeft[j].MinecraftVolumePerTick.Count == 0))
                            {
                                if (!scoreboards.Contains("wave_Volume")) scoreboards.Add("wave_Volume");
                                for (int t = 0; t < timeLine.TickNodes[i].AudioNodesLeft[j].MinecraftVolumePerTick.Count; t++)
                                {
                                    if (!scoreboards.Contains("wave_Vol_" + t.ToString())) scoreboards.Add("wave_Vol_" + t.ToString());
                                    commandLine.Keyframe[i].Commands.Add(setCommand(keyTrack, "wave_Vol_" + t.ToString(), (int)timeLine.TickNodes[i].AudioNodesLeft[j].MinecraftVolumePerTick[t]));
                                }
                                Output = true;
                            }
                            if (!Output) // empty nodes
                            {
                                if (pre == -1) tracks.Remove(keyTrack); //delect the new track
                                else tracks[keyTrack] = pre; //undo the value
                            }
                        }
                    }
                }
                #endregion
                #region WaveRight
                /* wave_left
                 *  - wave track
                 *    - wave node
                 *      - add track
                 *      - add scoreboard
                 */
                if (timeLine.TickNodes[i].AudioNodesRight != null)
                {
                    //foreach chaneel
                    for (int j = 0; j < timeLine.TickNodes[i].AudioNodesRight.Count; j++)
                    {
                        if (timeLineInfo.AllAudioRightEnable && timeLineInfo.TickNodesInfo.AudioChannelsRightInfo[j].Enable)
                        {
                            /* target */
                            int pre = -1;
                            string keyTrack = "wavel";
                            if (!tracks.ContainsKey(keyTrack)) tracks.Add(keyTrack, 1); //create new track
                            else if (tracks[keyTrack] < j) { pre = tracks[keyTrack]; tracks[keyTrack] = j; } //refresh track batch count
                            var Output = false;
                            /* scoreboard & commands */
                            if ((timeLineInfo.AllAudioMinecraftStartTickR && timeLineInfo.TickNodesInfo.AudioChannelsRightInfo[j].MinecraftStartTick) && !(timeLineInfo.IgnoreZero && timeLine.TickNodes[i].AudioNodesRight[j].MinecraftStartTick == 0))
                            {
                                if (!scoreboards.Contains("wave_StartTick")) scoreboards.Add("wave_StartTick");
                                commandLine.Keyframe[i].Commands.Add(setCommand(keyTrack, "wave_StartTick", (int)timeLine.TickNodes[i].AudioNodesRight[j].MinecraftStartTick));
                                Output = true;
                            }
                            if ((timeLineInfo.AllAudioMinecraftFrequencyPerTickL && timeLineInfo.TickNodesInfo.AudioChannelsRightInfo[j].MinecraftFrequencyPerTick) && !(timeLine.TickNodes[i].AudioNodesRight[j].MinecraftFrequencyPerTick.Count == 0))
                            {
                                for (int t = 0; t < timeLine.TickNodes[i].AudioNodesRight[j].MinecraftFrequencyPerTick.Count; t++)
                                {
                                    if (!scoreboards.Contains("wave_Fre_" + t.ToString())) scoreboards.Add("wave_Fre_" + t.ToString());
                                    commandLine.Keyframe[i].Commands.Add(setCommand(keyTrack, "wave_Fre_" + t.ToString(), (int)timeLine.TickNodes[i].AudioNodesRight[j].MinecraftFrequencyPerTick[t]));
                                }
                                Output = true;
                            }
                            if ((timeLineInfo.AllAudioMinecraftVolumePerTickL && timeLineInfo.TickNodesInfo.AudioChannelsRightInfo[j].MinecraftVolumePerTick) && !(timeLine.TickNodes[i].AudioNodesRight[j].MinecraftVolumePerTick.Count == 0))
                            {
                                for (int t = 0; t < timeLine.TickNodes[i].AudioNodesRight[j].MinecraftVolumePerTick.Count; t++)
                                {
                                    if (!scoreboards.Contains("wave_Vol_" + t.ToString())) scoreboards.Add("wave_Vol_" + t.ToString());
                                    commandLine.Keyframe[i].Commands.Add(setCommand(keyTrack, "wave_Vol_" + t.ToString(), (int)timeLine.TickNodes[i].AudioNodesRight[j].MinecraftVolumePerTick[t]));
                                }
                                Output = true;
                            }
                            if (!Output) // empty nodes
                            {
                                if (pre == -1) tracks.Remove(keyTrack); //delect the new track
                                else tracks[keyTrack] = pre; //undo the value
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion
            //generate scoreboard & target
            var Git = new List<string>();
            foreach (var scoreboard in scoreboards)
            {
                Git.Add("scoreboard objectives add " + scoreboard + " dummy");
                commandLine.End.Add("scoreboard objectives remove " + scoreboard);
            }
            foreach (var track in tracks)
            {
                for (int i = 0; i < track.Value; i++)
                {
                    Git.Add("summon area_effect_cloud ~ ~ ~ {Tags:[" + track.Key + "_" + i.ToString() + "," + track.Key + ",Tracks],Duration:" + commandLine.Keyframe.Count + 1 + "}");
                }
            }
            commandLine.End.Add("kill @e[tag=Tracks]");
            commandLine.General = Git.Union(commandLine.General).ToList<string>();
            return commandLine;
        }
        private string setCommand(string target, string score_name, int score)
        {
            return "scoreboard players set @e[type=area_effect_cloud,tag=" + target + "] " + score_name + " " + score.ToString();
        }
        private BlockInfo CommandLine2SchematicInfo(CommandLine commandLine, V2 SizeParam)
        {
            var blockInfo = new BlockInfo();
            /* Define the region size */
            var n = SizeParam.Width; var l = (commandLine.Keyframe.Count % n == 0) ? commandLine.Keyframe.Count / n : commandLine.Keyframe.Count / n + 1; var h = 0;
            /* Define a block position , data */
            var x = 0; var y = 0; var z = 0; var r = 0;//0: down, 1: up, 2: north, 3: south, 4: west, 5: east,
            #region General & End (impulse)
            var general = new Command();
            var end = new Command();
            var dot = new Command();
            dot.Commands = new List<string>();
            //general
            general.Commands = commandLine.General;
            commandLine.Keyframe.Insert(0, general);
            commandLine.Keyframe.Insert(1, dot);
            //end
            end.Commands = commandLine.End;
            commandLine.Keyframe.Add(dot);
            commandLine.Keyframe.Add(end);
            #endregion
            #region KeyFrames (line)
            /* Get Y_max */
            for (int i = 0; i < commandLine.Keyframe.Count; i++)
            {
                if (commandLine.Keyframe[i].Commands.Count > h) h = commandLine.Keyframe[i].Commands.Count;
            }
            MessageBox.Show(h.ToString());
            /* Create Arrays for block storing */
            var blocks = new byte[0]; var datas = new byte[0];
            if (SizeParam.Direction == 0 || SizeParam.Direction == 1)
            {
                blockInfo.Height.Value = (short)h; blockInfo.Length.Value = (short)n; blockInfo.Width.Value = (short)l;
                //blocks = new byte[((h - 1) * l + (n - 1)) * n + (l - 1) + 1]; datas = new byte[((h - 1) * l + (n - 1)) * n + (l - 1) + 1];
            }
            if (SizeParam.Direction == 2 || SizeParam.Direction == 3)
            {
                blockInfo.Height.Value = (short)h; blockInfo.Length.Value = (short)l; blockInfo.Width.Value = (short)n;
                //blocks = new byte[((h - 1) * n + (l - 1)) * l + (n - 1) + 1]; datas = new byte[((h - 1) * n + (l - 1)) * l + (n - 1) + 1];
            }
            blocks = new byte[h * n * l]; datas = new byte[h * n * l];
            /* Write in position */
            for (int i = 0; i < commandLine.Keyframe.Count; i++)
            {
                #region get (X & Z & Rotation) -> x y r
                //X+
                if (SizeParam.Direction == 0)
                {
                    x = i / n;
                    z = i % n;
                    if (x % 2 == 0) { if (z == n - 1) r = 5;/*x++*/ else r = 3;/*z++*/ }
                    else { z = n - z - 1; if (z == 0) r = 5; /*x++*/ else r = 2;/*z--*/ }
                }
                //X-
                if (SizeParam.Direction == 1)
                {
                    x = i / n;
                    z = i % n;
                    if (x % 2 == 0) { if (z == n - 1) r = 4;/*x--*/ else r = 3;/*z++*/ }
                    else { z = n - z - 1; if (z == 0) r = 4; /*x--*/ else r = 2;/*z--*/ }
                    x = l - x - 1;
                }
                //Z+
                if (SizeParam.Direction == 2)
                {
                    x = i % n;
                    z = i / n;
                    if (z % 2 == 0) { if (x == n - 1) r = 3;/*z++*/ else r = 5;/*x++*/ }
                    else { x = n - x - 1; if (x == 0) r = 3; /*z++*/ else r = 4;/*x--*/ }
                }
                //Z-
                if (SizeParam.Direction == 3)
                {
                    x = i % n;
                    z = i / n;
                    if (z % 2 == 0) { if (x == n - 1) r = 2;/*z--*/ else r = 5;/*x++*/ }
                    else { x = n - x - 1; if (x == 0) r = 2; /*z--*/ else r = 4;/*x--*/ }
                    z = l - z - 1;
                }
                #endregion
                var vector2 = new int[] { 0, 0 };
                #region Rotation
                switch (r)
                {
                    case 2: vector2[0] = 0; vector2[1] = -1; break;
                    case 3: vector2[0] = 0; vector2[1] = 1; break;
                    case 4: vector2[0] = -1; vector2[1] = 0; break;
                    case 5: vector2[0] = 1; vector2[1] = 0; break;
                }
                #endregion
                //Options about command
                if (!SizeParam.AlwaysActive) commandLine.Keyframe[i].Commands.Remove("spawnpoint @p ~ ~ ~");
                if (!SizeParam.AlwaysLoadEntities) commandLine.Keyframe[i].Commands.Remove("tp @e[tag=Tracks] @p");
                //WriteIn Commands
                for (y = 0; y < commandLine.Keyframe[i].Commands.Count; y++)
                {
                    //X+ X-
                    if (SizeParam.Direction == 0 || SizeParam.Direction == 1)
                    {
                        var index = (y * n + z) * l + x;
                        if (y == 0)
                        {
                            blocks[index] = 137;
                            datas[index] = 1;
                            if (commandLine.Keyframe[i].Commands[0] == "$setblock") blockInfo.TileEntities.Add(AddCommand("setblock ~" + vector2[0].ToString() + " ~-1 ~" + vector2[1].ToString() + " minecraft:redstone_block", x, y, z, false));
                            else blockInfo.TileEntities.Add(AddCommand(commandLine.Keyframe[i].Commands[y], x, y, z, false));
                        }
                        else
                        {
                            blocks[index] = 211;
                            datas[index] = 1;
                            blockInfo.TileEntities.Add(AddCommand(commandLine.Keyframe[i].Commands[y], x, y, z, true));
                        }
                    }
                    //Z+ Z-
                    else if (SizeParam.Direction == 2 || SizeParam.Direction == 3)
                    {
                        var index = (y * l + z) * n + x;
                        if (y == 0 && commandLine.Keyframe[i].Commands[0] == "$setblock")
                        {
                            blocks[index] = 137;
                            datas[index] = 1;
                            if (commandLine.Keyframe[i].Commands[0] == "$setblock") blockInfo.TileEntities.Add(AddCommand("setblock ~" + vector2[0].ToString() + " ~-1 ~" + vector2[1].ToString() + " minecraft:redstone_block", x, y, z, false));
                            else blockInfo.TileEntities.Add(AddCommand(commandLine.Keyframe[i].Commands[y], x, y, z, false));
                        }
                        else
                        {
                            blocks[index] = 211;
                            datas[index] = 1;
                            blockInfo.TileEntities.Add(AddCommand(commandLine.Keyframe[i].Commands[y], x, y, z, true));
                        }
                    }
                }
            }
            blockInfo.Blocks.Value = blocks;
            blockInfo.Data.Value = datas;
            #endregion
            return blockInfo;
        }
        public void ExportSchematic(string Path, CommandLine commandLine, V2 SizeParam)
        {
            var Schematic = new NbtCompound("Schematic");
            //size
            var blockInfo = CommandLine2SchematicInfo(commandLine, SizeParam);
            Schematic.Add(blockInfo.Height);
            Schematic.Add(blockInfo.Length);
            Schematic.Add(blockInfo.Width);
            Schematic.Add(new NbtList("Entities", NbtTagType.Compound));
            Schematic.Add(blockInfo.Data);
            Schematic.Add(blockInfo.Blocks);
            Schematic.Add(blockInfo.TileEntities);

            new NbtFile(Schematic).SaveToFile(Path, NbtCompression.None);
        }
        private NbtCompound AddCommand(string command, int x, int y, int z, bool auto)
        {
            var NodePoint = new NbtCompound();
            NodePoint.Add(new NbtString("id", "minecraft:command_block"));
            NodePoint.Add(new NbtString("Command", command));
            NodePoint.Add(new NbtByte("TrackOutput", 1));
            NodePoint.Add(new NbtString("CustomName", "@"));
            NodePoint.Add(new NbtInt("SuccessCount", 0));
            NodePoint.Add(new NbtByte("auto", auto ? (byte)1 : (byte)0));
            NodePoint.Add(new NbtByte("powered", 0));
            NodePoint.Add(new NbtByte("conditionMet", 0));
            NodePoint.Add(new NbtByte("UpdateLastExecution", 1));
            NodePoint.Add(new NbtInt("x", x));
            NodePoint.Add(new NbtInt("y", y));
            NodePoint.Add(new NbtInt("z", z));
            return NodePoint;
        }
    }

    public class V2
    {
        private int _dir = 0;
        /// <summary>
        /// Which Direction the CommandLine Extends.
        /// X+:0, X-:1, Z+:2, Z-:3
        /// </summary>
        public int Direction { get { return _dir; } set { _dir = value; } }//Xf = 0, Xb = 1, Zf = 2, Zb = 3 
        private int _width = 64;
        /// <summary>
        /// the Width of the CommandLine
        /// </summary>
        public int Width { get { return _width; } set { _width = value; } }
        private bool _AlwaysActive = true;
        /// <summary>
        /// Whether the Chunks Always Loaded
        /// </summary>
        public bool AlwaysActive { get { return _AlwaysActive; } set { _AlwaysActive = value; } }
        private bool _AlwaysLoadEntities = true;
        /// <summary>
        /// Whether the Entities Always Loaded
        /// </summary>
        public bool AlwaysLoadEntities { get { return _AlwaysLoadEntities; } set { _AlwaysLoadEntities = value; } }
    }

    class BlockInfo
    {
        public NbtByteArray Blocks = new NbtByteArray("Blocks");
        public NbtByteArray Data = new NbtByteArray("Data");
        public NbtList TileEntities = new NbtList("TileEntities");
        public NbtShort Height = new NbtShort("Height", 0);
        public NbtShort Length = new NbtShort("Length", 0);
        public NbtShort Width = new NbtShort("Width", 0);
    }
}