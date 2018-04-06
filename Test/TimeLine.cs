using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;

namespace Audio2Minecraft
{
    public class _Node_INT
    {
        private string name = "none";
        public string Name { get { return name; } set { name = value; } }
        private int val = -1;
        public int Value { get { return val; } set { val = value; } }
        private bool en = true;
        public bool Enable { get { return en; } set { en = value; } }
    }
    public class PlaySoundInfo
    {
        private string _timbre = "1";
        public string SoundName { get { return _timbre; } set { _timbre = value; } }
        private double[] _cood = new double[] { 0, 0, 0 };
        public double[] ExecuteCood { get { return _cood; } set { _cood = value; } }

        private string _target_0 = "@a";
        public string ExecuteTarget { get { return _target_0; } set { _target_0 = value; } }
        private string _target = "@a";
        public string PlayTarget { get { return _target; } set { _target = value; } }
        private string _source = "record";
        public string PlaySource { get { return _source; } set { _source = value; } }
        //Extra Definations
        private string _overlap = null;
        public string OverlapPitch { get { return _overlap; } set { _overlap = value; } }
        private int _delay = -1;
        public int ExtraDelay { get { return _delay; } set { _delay = value; } }
        private int _volume = -1;
        public int MandaVolume { get { return _volume; } set { _volume = value; } }
        private int _evolume = -1;
        public int PercVolume { get { return _evolume; } set { _evolume = value; } }
        private bool en = true;
        public bool Enable { get { return en; } set { en = value; } }
        private bool _stopsound = false;
        public bool StopSound { get { return _stopsound; } set { _stopsound = value; } }
    }
    public class TimeLine
    {
        #region Enable
        public void Enable(bool enable = true, string param = "")
        {
            if (param == "")
            {
                foreach (string p in this.Param.Keys)
                {
                    this.Param[p].Enable = enable;
                }
            }
            else if (this.Param.ContainsKey(param))
            {
                this.Param[param].Enable = enable;
            }
        }
        public void EnableMidi(bool enable = true, string track = "", string instrument = "", int index = -1, string param = "")
        {
            for (int i = 0; i < this.TickNodes.Count; i++)
            {
                this.TickNodes[i].EnableMidi(enable, track, instrument, index, param);
            }
        }
        public void EnableWave(bool enable = true, int index = -1, string channel = "", string param = "")
        {
            for (int i = 0; i < this.TickNodes.Count; i++)
            {
                this.TickNodes[i].EnableWave(enable, index, channel, param);
            }
        }
        #endregion
        #region Playsound
        public void Sound_SoundName(string sound = "1", string track = "", string instrument = "", int index = -1)
        {
            foreach (TickNode tickNode in TickNodes)
            {
                if (track == "")
                {
                    foreach (string t in tickNode.MidiTracks.Keys)
                    {
                        if (instrument == "")
                        {
                            foreach (string i in tickNode.MidiTracks[t].Keys)
                            {
                                if (index == -1)
                                    for (int m = 0; m < tickNode.MidiTracks[t][i].Count; m++) tickNode.MidiTracks[t][i][m].PlaySound.SoundName = sound;
                                else
                                    tickNode.MidiTracks[t][i][index].PlaySound.SoundName = sound;
                            }
                        }
                        else if (tickNode.MidiTracks[t].ContainsKey(instrument))
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[t][instrument].Count; m++) tickNode.MidiTracks[t][instrument][m].PlaySound.SoundName = sound;
                            else
                                tickNode.MidiTracks[t][instrument][index].PlaySound.SoundName = sound;
                        }
                    }
                }
                else if (tickNode.MidiTracks.ContainsKey(track))
                {
                    if (instrument == "")
                    {
                        foreach (string i in tickNode.MidiTracks[track].Keys)
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[track][i].Count; m++) tickNode.MidiTracks[track][i][m].PlaySound.SoundName = sound;
                            else
                                tickNode.MidiTracks[track][i][index].PlaySound.SoundName = sound;
                        }
                    }
                    else if (tickNode.MidiTracks[track].ContainsKey(instrument))
                    {
                        if (index == -1)
                            for (int m = 0; m < tickNode.MidiTracks[track][instrument].Count; m++) tickNode.MidiTracks[track][instrument][m].PlaySound.SoundName = sound;
                        else
                            tickNode.MidiTracks[track][instrument][index].PlaySound.SoundName = sound;
                    }
                }
            }
        }
        public void Sound_ExecuteCood(double[] cood, string track = "", string instrument = "", int index = -1)
        {
            foreach (TickNode tickNode in TickNodes)
            {
                if (track == "")
                {
                    foreach (string t in tickNode.MidiTracks.Keys)
                    {
                        if (instrument == "")
                        {
                            foreach (string i in tickNode.MidiTracks[t].Keys)
                            {
                                if (index == -1)
                                    for (int m = 0; m < tickNode.MidiTracks[t][i].Count; m++) tickNode.MidiTracks[t][i][m].PlaySound.ExecuteCood = cood;
                                else
                                    tickNode.MidiTracks[t][i][index].PlaySound.ExecuteCood = cood;
                            }
                        }
                        else if (tickNode.MidiTracks[t].ContainsKey(instrument))
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[t][instrument].Count; m++) tickNode.MidiTracks[t][instrument][m].PlaySound.ExecuteCood = cood;
                            else
                                tickNode.MidiTracks[t][instrument][index].PlaySound.ExecuteCood = cood;
                        }
                    }
                }
                else if (tickNode.MidiTracks.ContainsKey(track))
                {
                    if (instrument == "")
                    {
                        foreach (string i in tickNode.MidiTracks[track].Keys)
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[track][i].Count; m++) tickNode.MidiTracks[track][i][m].PlaySound.ExecuteCood = cood;
                            else
                                tickNode.MidiTracks[track][i][index].PlaySound.ExecuteCood = cood;
                        }
                    }
                    else if (tickNode.MidiTracks[track].ContainsKey(instrument))
                    {
                        if (index == -1)
                            for (int m = 0; m < tickNode.MidiTracks[track][instrument].Count; m++) tickNode.MidiTracks[track][instrument][m].PlaySound.ExecuteCood = cood;
                        else
                            tickNode.MidiTracks[track][instrument][index].PlaySound.ExecuteCood = cood;
                    }
                }
            }
        }
        public void Sound_ExecuteTarget(string executeTarget = "@a", string track = "", string instrument = "", int index = -1)
        {
            foreach (TickNode tickNode in TickNodes)
            {
                if (track == "")
                {
                    foreach (string t in tickNode.MidiTracks.Keys)
                    {
                        if (instrument == "")
                        {
                            foreach (string i in tickNode.MidiTracks[t].Keys)
                            {
                                if (index == -1)
                                    for (int m = 0; m < tickNode.MidiTracks[t][i].Count; m++) tickNode.MidiTracks[t][i][m].PlaySound.ExecuteTarget = executeTarget;
                                else
                                    tickNode.MidiTracks[t][i][index].PlaySound.ExecuteTarget = executeTarget;
                            }
                        }
                        else if (tickNode.MidiTracks[t].ContainsKey(instrument))
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[t][instrument].Count; m++) tickNode.MidiTracks[t][instrument][m].PlaySound.ExecuteTarget = executeTarget;
                            else
                                tickNode.MidiTracks[t][instrument][index].PlaySound.ExecuteTarget = executeTarget;
                        }
                    }
                }
                else if (tickNode.MidiTracks.ContainsKey(track))
                {
                    if (instrument == "")
                    {
                        foreach (string i in tickNode.MidiTracks[track].Keys)
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[track][i].Count; m++) tickNode.MidiTracks[track][i][m].PlaySound.ExecuteTarget = executeTarget;
                            else
                                tickNode.MidiTracks[track][i][index].PlaySound.ExecuteTarget = executeTarget;
                        }
                    }
                    else if (tickNode.MidiTracks[track].ContainsKey(instrument))
                    {
                        if (index == -1)
                            for (int m = 0; m < tickNode.MidiTracks[track][instrument].Count; m++) tickNode.MidiTracks[track][instrument][m].PlaySound.ExecuteTarget = executeTarget;
                        else
                            tickNode.MidiTracks[track][instrument][index].PlaySound.ExecuteTarget = executeTarget;
                    }
                }
            }
        }
        public void Sound_PlayTarget(string playTarget = "@a", string track = "", string instrument = "", int index = -1)
        {
            foreach (TickNode tickNode in TickNodes)
            {
                if (track == "")
                {
                    foreach (string t in tickNode.MidiTracks.Keys)
                    {
                        if (instrument == "")
                        {
                            foreach (string i in tickNode.MidiTracks[t].Keys)
                            {
                                if (index == -1)
                                    for (int m = 0; m < tickNode.MidiTracks[t][i].Count; m++) tickNode.MidiTracks[t][i][m].PlaySound.PlayTarget = playTarget;
                                else
                                    tickNode.MidiTracks[t][i][index].PlaySound.PlayTarget = playTarget;
                            }
                        }
                        else if (tickNode.MidiTracks[t].ContainsKey(instrument))
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[t][instrument].Count; m++) tickNode.MidiTracks[t][instrument][m].PlaySound.PlayTarget = playTarget;
                            else
                                tickNode.MidiTracks[t][instrument][index].PlaySound.PlayTarget = playTarget;
                        }
                    }
                }
                else if (tickNode.MidiTracks.ContainsKey(track))
                {
                    if (instrument == "")
                    {
                        foreach (string i in tickNode.MidiTracks[track].Keys)
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[track][i].Count; m++) tickNode.MidiTracks[track][i][m].PlaySound.PlayTarget = playTarget;
                            else
                                tickNode.MidiTracks[track][i][index].PlaySound.PlayTarget = playTarget;
                        }
                    }
                    else if (tickNode.MidiTracks[track].ContainsKey(instrument))
                    {
                        if (index == -1)
                            for (int m = 0; m < tickNode.MidiTracks[track][instrument].Count; m++) tickNode.MidiTracks[track][instrument][m].PlaySound.PlayTarget = playTarget;
                        else
                            tickNode.MidiTracks[track][instrument][index].PlaySound.PlayTarget = playTarget;
                    }
                }
            }
        }
        public void Sound_PlaySource(string playSource = "record", string track = "", string instrument = "", int index = -1)
        {
            foreach (TickNode tickNode in TickNodes)
            {
                if (track == "")
                {
                    foreach (string t in tickNode.MidiTracks.Keys)
                    {
                        if (instrument == "")
                        {
                            foreach (string i in tickNode.MidiTracks[t].Keys)
                            {
                                if (index == -1)
                                    for (int m = 0; m < tickNode.MidiTracks[t][i].Count; m++) tickNode.MidiTracks[t][i][m].PlaySound.PlaySource = playSource;
                                else
                                    tickNode.MidiTracks[t][i][index].PlaySound.PlaySource = playSource;
                            }
                        }
                        else if (tickNode.MidiTracks[t].ContainsKey(instrument))
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[t][instrument].Count; m++) tickNode.MidiTracks[t][instrument][m].PlaySound.PlaySource = playSource;
                            else
                                tickNode.MidiTracks[t][instrument][index].PlaySound.PlaySource = playSource;
                        }
                    }
                }
                else if (tickNode.MidiTracks.ContainsKey(track))
                {
                    if (instrument == "")
                    {
                        foreach (string i in tickNode.MidiTracks[track].Keys)
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[track][i].Count; m++) tickNode.MidiTracks[track][i][m].PlaySound.PlaySource = playSource;
                            else
                                tickNode.MidiTracks[track][i][index].PlaySound.PlaySource = playSource;
                        }
                    }
                    else if (tickNode.MidiTracks[track].ContainsKey(instrument))
                    {
                        if (index == -1)
                            for (int m = 0; m < tickNode.MidiTracks[track][instrument].Count; m++) tickNode.MidiTracks[track][instrument][m].PlaySound.PlaySource = playSource;
                        else
                            tickNode.MidiTracks[track][instrument][index].PlaySound.PlaySource = playSource;
                    }
                }
            }
        }
        public void Sound_OverlapPitch(string overlapPitch = null, string track = "", string instrument = "", int index = -1)
        {
            foreach (TickNode tickNode in TickNodes)
            {
                if (track == "")
                {
                    foreach (string t in tickNode.MidiTracks.Keys)
                    {
                        if (instrument == "")
                        {
                            foreach (string i in tickNode.MidiTracks[t].Keys)
                            {
                                if (index == -1)
                                    for (int m = 0; m < tickNode.MidiTracks[t][i].Count; m++) tickNode.MidiTracks[t][i][m].PlaySound.OverlapPitch = overlapPitch;
                                else
                                    tickNode.MidiTracks[t][i][index].PlaySound.OverlapPitch = overlapPitch;
                            }
                        }
                        else if (tickNode.MidiTracks[t].ContainsKey(instrument))
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[t][instrument].Count; m++) tickNode.MidiTracks[t][instrument][m].PlaySound.OverlapPitch = overlapPitch;
                            else
                                tickNode.MidiTracks[t][instrument][index].PlaySound.OverlapPitch = overlapPitch;
                        }
                    }
                }
                else if (tickNode.MidiTracks.ContainsKey(track))
                {
                    if (instrument == "")
                    {
                        foreach (string i in tickNode.MidiTracks[track].Keys)
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[track][i].Count; m++) tickNode.MidiTracks[track][i][m].PlaySound.OverlapPitch = overlapPitch;
                            else
                                tickNode.MidiTracks[track][i][index].PlaySound.OverlapPitch = overlapPitch;
                        }
                    }
                    else if (tickNode.MidiTracks[track].ContainsKey(instrument))
                    {
                        if (index == -1)
                            for (int m = 0; m < tickNode.MidiTracks[track][instrument].Count; m++) tickNode.MidiTracks[track][instrument][m].PlaySound.OverlapPitch = overlapPitch;
                        else
                            tickNode.MidiTracks[track][instrument][index].PlaySound.OverlapPitch = overlapPitch;
                    }
                }
            }
        }
        public void Sound_ExtraDelay(int delay = -1, string track = "", string instrument = "", int index = -1)
        {
            foreach (TickNode tickNode in TickNodes)
            {
                if (track == "")
                {
                    foreach (string t in tickNode.MidiTracks.Keys)
                    {
                        if (instrument == "")
                        {
                            foreach (string i in tickNode.MidiTracks[t].Keys)
                            {
                                if (index == -1)
                                    for (int m = 0; m < tickNode.MidiTracks[t][i].Count; m++) tickNode.MidiTracks[t][i][m].PlaySound.ExtraDelay = delay;
                                else
                                    tickNode.MidiTracks[t][i][index].PlaySound.ExtraDelay = delay;
                            }
                        }
                        else if (tickNode.MidiTracks[t].ContainsKey(instrument))
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[t][instrument].Count; m++) tickNode.MidiTracks[t][instrument][m].PlaySound.ExtraDelay = delay;
                            else
                                tickNode.MidiTracks[t][instrument][index].PlaySound.ExtraDelay = delay;
                        }
                    }
                }
                else if (tickNode.MidiTracks.ContainsKey(track))
                {
                    if (instrument == "")
                    {
                        foreach (string i in tickNode.MidiTracks[track].Keys)
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[track][i].Count; m++) tickNode.MidiTracks[track][i][m].PlaySound.ExtraDelay = delay;
                            else
                                tickNode.MidiTracks[track][i][index].PlaySound.ExtraDelay = delay;
                        }
                    }
                    else if (tickNode.MidiTracks[track].ContainsKey(instrument))
                    {
                        if (index == -1)
                            for (int m = 0; m < tickNode.MidiTracks[track][instrument].Count; m++) tickNode.MidiTracks[track][instrument][m].PlaySound.ExtraDelay = delay;
                        else
                            tickNode.MidiTracks[track][instrument][index].PlaySound.ExtraDelay = delay;
                    }
                }
            }
        }
        public void Sound_MandaVolume(int volume = -1, string track = "", string instrument = "", int index = -1)
        {
            foreach (TickNode tickNode in TickNodes)
            {
                if (track == "")
                {
                    foreach (string t in tickNode.MidiTracks.Keys)
                    {
                        if (instrument == "")
                        {
                            foreach (string i in tickNode.MidiTracks[t].Keys)
                            {
                                if (index == -1)
                                    for (int m = 0; m < tickNode.MidiTracks[t][i].Count; m++) tickNode.MidiTracks[t][i][m].PlaySound.MandaVolume = volume;
                                else
                                    tickNode.MidiTracks[t][i][index].PlaySound.MandaVolume = volume;
                            }
                        }
                        else if (tickNode.MidiTracks[t].ContainsKey(instrument))
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[t][instrument].Count; m++) tickNode.MidiTracks[t][instrument][m].PlaySound.MandaVolume = volume;
                            else
                                tickNode.MidiTracks[t][instrument][index].PlaySound.MandaVolume = volume;
                        }
                    }
                }
                else if (tickNode.MidiTracks.ContainsKey(track))
                {
                    if (instrument == "")
                    {
                        foreach (string i in tickNode.MidiTracks[track].Keys)
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[track][i].Count; m++) tickNode.MidiTracks[track][i][m].PlaySound.MandaVolume = volume;
                            else
                                tickNode.MidiTracks[track][i][index].PlaySound.MandaVolume = volume;
                        }
                    }
                    else if (tickNode.MidiTracks[track].ContainsKey(instrument))
                    {
                        if (index == -1)
                            for (int m = 0; m < tickNode.MidiTracks[track][instrument].Count; m++) tickNode.MidiTracks[track][instrument][m].PlaySound.MandaVolume = volume;
                        else
                            tickNode.MidiTracks[track][instrument][index].PlaySound.MandaVolume = volume;
                    }
                }
            }
        }
        public void Sound_PercVolume(int percent = -1, string track = "", string instrument = "", int index = -1)
        {
            foreach (TickNode tickNode in TickNodes)
            {
                if (track == "")
                {
                    foreach (string t in tickNode.MidiTracks.Keys)
                    {
                        if (instrument == "")
                        {
                            foreach (string i in tickNode.MidiTracks[t].Keys)
                            {
                                if (index == -1)
                                    for (int m = 0; m < tickNode.MidiTracks[t][i].Count; m++) tickNode.MidiTracks[t][i][m].PlaySound.PercVolume = percent;
                                else
                                    tickNode.MidiTracks[t][i][index].PlaySound.PercVolume = percent;
                            }
                        }
                        else if (tickNode.MidiTracks[t].ContainsKey(instrument))
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[t][instrument].Count; m++) tickNode.MidiTracks[t][instrument][m].PlaySound.PercVolume = percent;
                            else
                                tickNode.MidiTracks[t][instrument][index].PlaySound.PercVolume = percent;
                        }
                    }
                }
                else if (tickNode.MidiTracks.ContainsKey(track))
                {
                    if (instrument == "")
                    {
                        foreach (string i in tickNode.MidiTracks[track].Keys)
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[track][i].Count; m++) tickNode.MidiTracks[track][i][m].PlaySound.PercVolume = percent;
                            else
                                tickNode.MidiTracks[track][i][index].PlaySound.PercVolume = percent;
                        }
                    }
                    else if (tickNode.MidiTracks[track].ContainsKey(instrument))
                    {
                        if (index == -1)
                            for (int m = 0; m < tickNode.MidiTracks[track][instrument].Count; m++) tickNode.MidiTracks[track][instrument][m].PlaySound.PercVolume = percent;
                        else
                            tickNode.MidiTracks[track][instrument][index].PlaySound.PercVolume = percent;
                    }
                }
            }
        }
        public void Sound_StopSound(bool stop = false, string track = "", string instrument = "", int index = -1)
        {
            foreach (TickNode tickNode in TickNodes)
            {
                if (track == "")
                {
                    foreach (string t in tickNode.MidiTracks.Keys)
                    {
                        if (instrument == "")
                        {
                            foreach (string i in tickNode.MidiTracks[t].Keys)
                            {
                                if (index == -1)
                                    for (int m = 0; m < tickNode.MidiTracks[t][i].Count; m++) tickNode.MidiTracks[t][i][m].PlaySound.StopSound = stop;
                                else
                                    tickNode.MidiTracks[t][i][index].PlaySound.StopSound = stop;
                            }
                        }
                        else if (tickNode.MidiTracks[t].ContainsKey(instrument))
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[t][instrument].Count; m++) tickNode.MidiTracks[t][instrument][m].PlaySound.StopSound = stop;
                            else
                                tickNode.MidiTracks[t][instrument][index].PlaySound.StopSound = stop;
                        }
                    }
                }
                else if (tickNode.MidiTracks.ContainsKey(track))
                {
                    if (instrument == "")
                    {
                        foreach (string i in tickNode.MidiTracks[track].Keys)
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[track][i].Count; m++) tickNode.MidiTracks[track][i][m].PlaySound.StopSound = stop;
                            else
                                tickNode.MidiTracks[track][i][index].PlaySound.StopSound = stop;
                        }
                    }
                    else if (tickNode.MidiTracks[track].ContainsKey(instrument))
                    {
                        if (index == -1)
                            for (int m = 0; m < tickNode.MidiTracks[track][instrument].Count; m++) tickNode.MidiTracks[track][instrument][m].PlaySound.StopSound = stop;
                        else
                            tickNode.MidiTracks[track][instrument][index].PlaySound.StopSound = stop;
                    }
                }
            }
        }
        #endregion
        //Identity
        public TimeLine Serialize(string MidiFilePath = null, string WaveFilePath = null, int tBpm = 160, int fre_count = 1, int vol_count = 1, int tick_cycle = 1)
        {
            var timeLine = new TimeLine();
            if (MidiFilePath != null && MidiFilePath != "") timeLine = new AudioStreamMidi().Serialize(MidiFilePath, timeLine, tBpm);
            if (WaveFilePath != null && WaveFilePath != "") timeLine = new AudioStreamWave().Serialize(WaveFilePath, timeLine, fre_count, vol_count, tick_cycle);
            return timeLine;
        }
        public void Export(ExportSetting SettingParam, string ExportPath = "C:\\MyAudioRiptide.schematic", string MidiFilePath = null, string WaveFilePath = null, int tBpm = 160, int fre_count = 1, int vol_count = 1, int tick_cycle = 1)
        {
            new Schematic().ExportSchematic(new CommandLine().Serialize(Serialize(MidiFilePath, WaveFilePath, tBpm, fre_count, vol_count, tick_cycle)), SettingParam, ExportPath);
        }
        public Dictionary<string, _Node_INT> Param = new Dictionary<string, _Node_INT>()
        {
            {"MidiFileFormat", new _Node_INT() { Name = "MidiFileFormat" } } ,
            {"MidiTracksCount", new _Node_INT() { Name = "MidiTracksCount" } } ,
            {"MidiDeltaTicksPerQuarterNote", new _Node_INT() { Name = "MidiDeltaTdiv4" } } ,
            {"MidiBeatPerMinute", new _Node_INT() { Name = "MidiBPM" } } ,
            {"AudioFileFormat", new _Node_INT() { Name = "AudioFileFormat" } } ,
        };
        public List<TickNode> TickNodes = new List<TickNode>();
        private bool _tick_out = true;
        public bool OutPutTick { get { return _tick_out; } set { _tick_out = value; } }
    }

    public class TickNode
    {
        public void EnableMidi(bool enable = true, string track = "", string instrument = "", int index = -1, string param = "")
        {
            if (track == "")
            {
                foreach(string t in this.MidiTracks.Keys)
                {
                    if (instrument == "")
                    {
                        foreach(string i in this.MidiTracks[t].Keys)
                        {
                            if (index == -1)
                                for(int m = 0; m < this.MidiTracks[t][i].Count; m++) this.MidiTracks[t][i][m].Enable(enable, param);
                            else
                                this.MidiTracks[t][i][index].Enable(enable, param);
                        }
                    }
                    else if (this.MidiTracks[t].ContainsKey(instrument))
                    {
                        if (index == -1)
                            for (int m = 0; m < this.MidiTracks[t][instrument].Count; m++) this.MidiTracks[t][instrument][m].Enable(enable, param);
                        else
                            this.MidiTracks[t][instrument][index].Enable(enable, param);
                    }
                }
            }
            else if (this.MidiTracks.ContainsKey(track))
            {
                if (instrument == "")
                {
                    foreach (string i in this.MidiTracks[track].Keys)
                    {
                        if (index == -1)
                            for (int m = 0; m < this.MidiTracks[track][i].Count; m++) this.MidiTracks[track][i][m].Enable(enable, param);
                        else
                            this.MidiTracks[track][i][index].Enable(enable, param);
                    }
                }
                else if (this.MidiTracks[track].ContainsKey(instrument))
                {
                    if (index == -1)
                        for (int m = 0; m < this.MidiTracks[track][instrument].Count; m++) this.MidiTracks[track][instrument][m].Enable(enable, param);
                    else
                        this.MidiTracks[track][instrument][index].Enable(enable, param);
                }
            }
        }
        public Dictionary<string, Dictionary<string, List<MidiNode>>> MidiTracks = new Dictionary<string, Dictionary<string, List<MidiNode>>>();
        public void EnableWave(bool enable = true, int index = -1, string channel = "", string param = "")
        {
            if (index < 0)
            {
                if (channel == "")
                {
                    for(int i = 0; i < this.WaveNodesLeft.Count; i++)
                    {
                        this.WaveNodesLeft[i].Enable(enable, param);
                    }
                    for (int i = 0; i < this.WaveNodesRight.Count; i++)
                    {
                        this.WaveNodesRight[i].Enable(enable, param);
                    }
                }
                else if (channel == "Left")
                {
                    for (int i = 0; i < this.WaveNodesLeft.Count; i++)
                    {
                        this.WaveNodesLeft[i].Enable(enable, param);
                    }
                }
                else if (channel == "Right")
                {
                    for (int i = 0; i < this.WaveNodesRight.Count; i++)
                    {
                        this.WaveNodesRight[i].Enable(enable, param);
                    }
                }
            }
            else
            {
                if (channel == "")
                {
                    this.WaveNodesLeft[index].Enable(enable, param);
                    this.WaveNodesRight[index].Enable(enable, param);
                }
                else if (channel == "Left")
                {
                    this.WaveNodesLeft[index].Enable(enable, param);
                }
                else if (channel == "Right")
                {
                    this.WaveNodesRight[index].Enable(enable, param);
                }
            }
        }
        public List<WaveNode> WaveNodesLeft = new List<WaveNode>();
        public List<WaveNode> WaveNodesRight = new List<WaveNode>();
        public int CurrentTick = -1;
    }

    public class MidiNode
    {
        public void Enable(bool enable = true, string param = "")
        {
            if (param == "")
            {
                foreach(string p in this.Param.Keys)
                {
                    this.Param[p].Enable = enable;
                }
                this.PlaySound.Enable = enable;
            }
            else if (param == "PlaySound")
            {
                this.PlaySound.Enable = enable;
            }
            else if (this.Param.ContainsKey(param))
            {
                this.Param[param].Enable = enable;
            }
        }
        public Dictionary<string, _Node_INT> Param = new Dictionary<string, _Node_INT>()
        {
            //Time-related
            {"DeltaTickStart", new _Node_INT() { Name = "DelSta" } } ,
            {"MinecraftTickStart", new _Node_INT() { Name = "TickSta" } } ,
            {"DeltaTickDuration", new _Node_INT() { Name = "DelDur" } } ,
            {"MinecraftTickDuration", new _Node_INT() { Name = "TickDur" } } ,
            //Bar-related
            {"BarIndex", new _Node_INT() { Name = "BarInd" } } ,
            {"BeatDuration", new _Node_INT() { Name = "BeatDur" } } ,
            //Note-related
            {"Channel", new _Node_INT() { Name = "Channel" } } ,
            {"Pitch", new _Node_INT() { Name = "Pitch" } } ,
            {"Velocity", new _Node_INT() { Name = "Velocity" } } ,
        };
        //Track-related
        private string _trackname = "none";
        public string TrackName { get { return _trackname; } set { _trackname = value; } }
        private string _instrumentname = "none";
        public string Instrument { get { return _instrumentname; } set { _instrumentname = value; } }
        //Timbre-related
        public PlaySoundInfo PlaySound = new PlaySoundInfo();
    }

    public class WaveNode
    {
        public void Enable(bool enable = true, string param = "")
        {
            if (param == "")
            {
                foreach (string p in this.Param.Keys)
                {
                    var k = this.Param[p];
                    for(int i = 0; i < k.Count; i++)
                        this.Param[p][i].Enable = enable;
                }
            }
            else if (this.Param.ContainsKey(param))
            {
                var k = this.Param[param];
                for (int i = 0; i < k.Count; i++)
                    this.Param[param][i].Enable = enable;
            }
        }
        public Dictionary<string, List<_Node_INT>> Param = new Dictionary<string, List<_Node_INT>>()
        {
            {"FrequencyPerTick", new List<_Node_INT>() } ,
            {"VolumePerTick", new List<_Node_INT>() },
        };
        private int _tick = -1;
        public int TickStart { get { return _tick; } set { _tick = value; } }
        private bool _isleft = true;
        public bool IsLeft { get { return _isleft; } set { _isleft = value; } }
    }

}
