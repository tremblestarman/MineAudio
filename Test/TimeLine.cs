using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using Newtonsoft.Json;

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
        private string _timbre = "";
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
        private string _inheritExpression = null;
        public string InheritExpression { get { return _inheritExpression; } set { _inheritExpression = value; } }
        private int _delay = -1;
        public int ExtraDelay { get { return _delay; } set { _delay = value; } }
        private int _volume = 100;
        public int MandaVolume { get { return _volume; } set { _volume = value; } }
        private int _evolume = -1;
        public int PercVolume { get { return _evolume; } set { _evolume = value; } }
        private bool en = true;
        public bool Enable { get { return en; } set { en = value; } }
        private bool _stopsound = false;
        public bool StopSound { get { return _stopsound; } set { _stopsound = value; } }
        private bool _pitchPlayable = false;
        public bool PitchPlayable { get { return _pitchPlayable; } set { _pitchPlayable = value; } }
        private int _pan = -1;
        public void SetPan(int pan)
        {
            _pan = pan;
        }
        public void Stereo(int facing)
        {
            if (facing == -1) return;
            var fi =
                (facing == 0) ? 0 :
                (facing == 1) ? Math.PI :
                (facing == 2) ? - Math.PI / 2:
                (facing == 3) ? Math.PI / 2 : 0;
            double theta = Math.PI / 2;
            if (_pan != 64) theta = (1 - (double)_pan / 127) * Math.PI;
            _cood[0] = Math.Round(Math.Sin((theta + fi)) * 3, 4);
            _cood[2] = Math.Round(Math.Cos((theta + fi)) * 3, 4);
        }
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
        public void Sound_InheritExpression(string inheritExpressionPitch = null, string track = "", string instrument = "", int index = -1)
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
                                    for (int m = 0; m < tickNode.MidiTracks[t][i].Count; m++) tickNode.MidiTracks[t][i][m].PlaySound.InheritExpression = inheritExpressionPitch;
                                else
                                    tickNode.MidiTracks[t][i][index].PlaySound.InheritExpression = inheritExpressionPitch;
                            }
                        }
                        else if (tickNode.MidiTracks[t].ContainsKey(instrument))
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[t][instrument].Count; m++) tickNode.MidiTracks[t][instrument][m].PlaySound.InheritExpression = inheritExpressionPitch;
                            else
                                tickNode.MidiTracks[t][instrument][index].PlaySound.InheritExpression = inheritExpressionPitch;
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
                                for (int m = 0; m < tickNode.MidiTracks[track][i].Count; m++) tickNode.MidiTracks[track][i][m].PlaySound.InheritExpression = inheritExpressionPitch;
                            else
                                tickNode.MidiTracks[track][i][index].PlaySound.InheritExpression = inheritExpressionPitch;
                        }
                    }
                    else if (tickNode.MidiTracks[track].ContainsKey(instrument))
                    {
                        if (index == -1)
                            for (int m = 0; m < tickNode.MidiTracks[track][instrument].Count; m++) tickNode.MidiTracks[track][instrument][m].PlaySound.InheritExpression = inheritExpressionPitch;
                        else
                            tickNode.MidiTracks[track][instrument][index].PlaySound.InheritExpression = inheritExpressionPitch;
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
        public void Sound_PitchPlayable(bool playable = false, string track = "", string instrument = "", int index = -1)
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
                                    for (int m = 0; m < tickNode.MidiTracks[t][i].Count; m++) tickNode.MidiTracks[t][i][m].PlaySound.PitchPlayable = playable;
                                else
                                    tickNode.MidiTracks[t][i][index].PlaySound.PitchPlayable = playable;
                            }
                        }
                        else if (tickNode.MidiTracks[t].ContainsKey(instrument))
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[t][instrument].Count; m++) tickNode.MidiTracks[t][instrument][m].PlaySound.PitchPlayable = playable;
                            else
                                tickNode.MidiTracks[t][instrument][index].PlaySound.PitchPlayable = playable;
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
                                for (int m = 0; m < tickNode.MidiTracks[track][i].Count; m++) tickNode.MidiTracks[track][i][m].PlaySound.PitchPlayable = playable;
                            else
                                tickNode.MidiTracks[track][i][index].PlaySound.PitchPlayable = playable;
                        }
                    }
                    else if (tickNode.MidiTracks[track].ContainsKey(instrument))
                    {
                        if (index == -1)
                            for (int m = 0; m < tickNode.MidiTracks[track][instrument].Count; m++) tickNode.MidiTracks[track][instrument][m].PlaySound.PitchPlayable = playable;
                        else
                            tickNode.MidiTracks[track][instrument][index].PlaySound.PitchPlayable = playable;
                    }
                }
            }
        }
        /// <summary>
        /// Which Direction the Player is Facing.
        /// X+:0, X-:1, Z+:2, Z-:3
        /// </summary>
        public void Sound_Stereo(int facing, string track = "", string instrument = "", int index = -1)
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
                                    for (int m = 0; m < tickNode.MidiTracks[t][i].Count; m++) tickNode.MidiTracks[t][i][m].PlaySound.Stereo(facing);
                                else
                                    tickNode.MidiTracks[t][i][index].PlaySound.Stereo(facing);
                            }
                        }
                        else if (tickNode.MidiTracks[t].ContainsKey(instrument))
                        {
                            if (index == -1)
                                for (int m = 0; m < tickNode.MidiTracks[t][instrument].Count; m++) tickNode.MidiTracks[t][instrument][m].PlaySound.Stereo(facing);
                            else
                                tickNode.MidiTracks[t][instrument][index].PlaySound.Stereo(facing);
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
                                for (int m = 0; m < tickNode.MidiTracks[track][i].Count; m++) tickNode.MidiTracks[track][i][m].PlaySound.Stereo(facing);
                            else
                                tickNode.MidiTracks[track][i][index].PlaySound.Stereo(facing);
                        }
                    }
                    else if (tickNode.MidiTracks[track].ContainsKey(instrument))
                    {
                        if (index == -1)
                            for (int m = 0; m < tickNode.MidiTracks[track][instrument].Count; m++) tickNode.MidiTracks[track][instrument][m].PlaySound.Stereo(facing);
                        else
                            tickNode.MidiTracks[track][instrument][index].PlaySound.Stereo(facing);
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
            {"TotalTicks", new _Node_INT() { Name = "TotalTicks" } } ,
        };
        public List<TickNode> TickNodes = new List<TickNode>();
        //Elements List & Enable List
        public ObservableCollection<MidiSettingInspector> TrackList = new ObservableCollection<MidiSettingInspector>();
        public ObservableCollection<MidiSettingInspector> InstrumentList = new ObservableCollection<MidiSettingInspector>();
        private bool _tick_out = true;
        public bool OutPutTick { get { return _tick_out; } set { _tick_out = value; } }
        public void AutoFill(AutoFill autofill, string mode)
        {
            var m = autofill.Rule.modes[mode];
            if (m.auto == true)
            {
                foreach (var t in TrackList)
                {
                    foreach (var i in t.Instruments)
                    {
                        _auto_fill(m, i);
                    }
                }
                foreach (var i in InstrumentList)
                {
                    _auto_fill(m, i);
                }
            }
        }
        private void _auto_fill(AutoFillMode mode, MidiSettingInspector i)
        {
            try
            {
                var _m = mode._matches.First(a => a.Value.instrument == i.Name);
                i.PlaysoundSetting.SoundName = _m.Key;
                i.PlaysoundSetting.PercVolume = (_m.Value.volume > 200) ? 200 : (_m.Value.volume < 0) ? 0 : (int)_m.Value.volume;
                i.PlaysoundSetting.InheritExpression = _m.Value.expression;
                i.PlaysoundSetting.PlayTarget = _m.Value.target;
                i.PlaysoundSetting.PlaySource = _m.Value.source;
                i.PlaysoundSetting.StopSound = _m.Value.stopsound.enable;
                i.PlaysoundSetting.ExtraDelay = _m.Value.stopsound.extra_delay;
                i.PlaysoundSetting.ExecuteTarget = _m.Value.excute_target;
                i.PlaysoundSetting.ExecuteCood = new double[] { _m.Value.excute_pos.x, _m.Value.excute_pos.y, _m.Value.excute_pos.z };
            }
            catch { }
        }
        public class MidiSettingInspector : INotifyPropertyChanged
        {
            public Guid Uid;
            private List<Guid> _TracksUid = new List<Guid>();
            public List<Guid> TracksUid { get { return _TracksUid; } set { _TracksUid = value;  } }
            private List<Guid> _InstrumentsUid = new List<Guid>();
            public List<Guid> InstrumentsUid { get { return _InstrumentsUid; } set { _InstrumentsUid = value; } }
            public MidiSettingInspector()
            {
                Tracks = new ObservableCollection<MidiSettingInspector>();
                Instruments = new ObservableCollection<MidiSettingInspector>();
                Uid = Guid.NewGuid();
            }
            public MidiSettingInspector(Guid TracksUid)
            {
                Tracks = new ObservableCollection<MidiSettingInspector>();
                Instruments = new ObservableCollection<MidiSettingInspector>();
                Uid = Guid.NewGuid();
                if (this.TracksUid == null)
                    this.TracksUid = new List<Guid>();
                this.TracksUid.Add(TracksUid);
            }

            bool _isEnable;
            public bool Enable
            {
                get { return _isEnable; }
                set
                {
                    _isEnable = value;
                    RaisePropertyChanged("Enable");
                    if (_isEnable == false)
                        unCheckOthers();
                }
            }
            void unCheckOthers()
            {
                if (_instruments != null)
                    foreach (var node in _instruments)
                        node.Enable = false;
            }

            ObservableCollection<MidiSettingInspector> _instruments = new ObservableCollection<MidiSettingInspector>();
            public ObservableCollection<MidiSettingInspector> Instruments { get { return _instruments; } set { _instruments = value; RaisePropertyChanged("Instruments");} }
            ObservableCollection<MidiSettingInspector> _tracks = new ObservableCollection<MidiSettingInspector>();
            public ObservableCollection<MidiSettingInspector> Tracks { get { return _tracks; } set { _tracks = value; RaisePropertyChanged("Tracks"); } }

            string _name;
            public string Name { get { return _name; } set { _name = value; RaisePropertyChanged("Name"); } }

            public MidiSettingType Type = MidiSettingType.Track;
            public PlaysoundSettingInspector PlaysoundSetting = new PlaysoundSettingInspector();

            private bool _enableScore;
            public bool EnableScore { get { return _enableScore; } set { _enableScore = value; } }
            private bool _enablePlaysound;
            public bool EnablePlaysound { get { return _enablePlaysound; } set { _enablePlaysound = value; PlaysoundSetting.Enable = value; } }

            private bool _deltaTickStart;
            public bool DeltaTickStart { get { return _deltaTickStart; } set { _deltaTickStart = value; } }
            private bool _minecraftTickStart;
            public bool MinecraftTickStart { get { return _minecraftTickStart; } set { _minecraftTickStart = value; } }
            private bool _deltaTickDuration;
            public bool DeltaTickDuration { get { return _deltaTickDuration; } set { _deltaTickDuration = value; } }
            private bool _minecraftTickDuration;
            public bool MinecraftTickDuration { get { return _minecraftTickDuration; } set { _minecraftTickDuration = value; } }
            //Bar-related
            private bool _barIndex;
            public bool BarIndex { get { return _barIndex; } set { _barIndex = value; } }
            private bool _beatDuration;
            public bool BeatDuration { get { return _beatDuration; } set { _beatDuration = value; } }
            //Note-related
            private bool _channel;
            public bool Channel { get { return _channel; } set { _channel = value; } }
            private bool _pitch;
            public bool Pitch { get { return _pitch; } set { _pitch = value; } }
            private bool _velocity;
            public bool Velocity { get { return _velocity; } set { _velocity = value; } }
            public class PlaysoundSettingInspector
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
                private string _inheritExpression = null;
                public string InheritExpression { get { return _inheritExpression; } set { _inheritExpression = value; } }
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
                private bool _pitchPlayable = false;
                public bool PitchPlayable { get { return _pitchPlayable; } set { _pitchPlayable = value; } }
            }

            public void Update()
            {
                if (Type == MidiSettingType.Track)
                {
                    UpdateTrackOnly();
                }
                else
                {
                    foreach (var t in Tracks)
                    {
                        foreach (var i in t.Instruments)
                        {
                            if (i.Name == Name)
                            {
                                i.Enable = Enable;
                                i.EnableScore = EnableScore;
                                i.EnablePlaysound = EnablePlaysound;
                                i.BarIndex = BarIndex;
                                i.BeatDuration = BeatDuration;
                                i.Channel = Channel;
                                i.DeltaTickDuration = DeltaTickDuration;
                                i.DeltaTickStart = DeltaTickStart;
                                i.Velocity = Velocity;
                                i.Pitch = Pitch;
                                i.MinecraftTickDuration = MinecraftTickDuration;
                                i.MinecraftTickStart = MinecraftTickStart;
                                i.PlaysoundSetting.Enable = PlaysoundSetting.Enable;
                                i.PlaysoundSetting.ExecuteCood = PlaysoundSetting.ExecuteCood;
                                i.PlaysoundSetting.ExecuteTarget = PlaysoundSetting.ExecuteTarget;
                                i.PlaysoundSetting.ExtraDelay = PlaysoundSetting.ExtraDelay;
                                i.PlaysoundSetting.MandaVolume = PlaysoundSetting.MandaVolume;
                                i.PlaysoundSetting.InheritExpression = PlaysoundSetting.InheritExpression;
                                i.PlaysoundSetting.PercVolume = PlaysoundSetting.PercVolume;
                                i.PlaysoundSetting.PlaySource = PlaysoundSetting.PlaySource;
                                i.PlaysoundSetting.PlayTarget = PlaysoundSetting.PlayTarget;
                                i.PlaysoundSetting.SoundName = PlaysoundSetting.SoundName;
                                i.PlaysoundSetting.StopSound = PlaysoundSetting.StopSound;
                                i.PlaysoundSetting.PitchPlayable = PlaysoundSetting.PitchPlayable;
                            }
                        }
                    }
                }
            }
            public void UpdateTrackOnly()
            {
                if (Type == MidiSettingType.Track)
                {
                    foreach (var i in Instruments)
                    {
                        i.Enable = Enable;
                        i.EnableScore = EnableScore;
                        i.EnablePlaysound = EnablePlaysound;
                        i.BarIndex = BarIndex;
                        i.BeatDuration = BeatDuration;
                        i.Channel = Channel;
                        i.DeltaTickDuration = DeltaTickDuration;
                        i.DeltaTickStart = DeltaTickStart;
                        i.Velocity = Velocity;
                        i.Pitch = Pitch;
                        i.MinecraftTickDuration = MinecraftTickDuration;
                        i.MinecraftTickStart = MinecraftTickStart;
                        i.PlaysoundSetting.Enable = PlaysoundSetting.Enable;
                        i.PlaysoundSetting.ExecuteCood = PlaysoundSetting.ExecuteCood;
                        i.PlaysoundSetting.ExecuteTarget = PlaysoundSetting.ExecuteTarget;
                        i.PlaysoundSetting.ExtraDelay = PlaysoundSetting.ExtraDelay;
                        i.PlaysoundSetting.MandaVolume = PlaysoundSetting.MandaVolume;
                        i.PlaysoundSetting.InheritExpression = PlaysoundSetting.InheritExpression;
                        i.PlaysoundSetting.PercVolume = PlaysoundSetting.PercVolume;
                        i.PlaysoundSetting.PlaySource = PlaysoundSetting.PlaySource;
                        i.PlaysoundSetting.PlayTarget = PlaysoundSetting.PlayTarget;
                        i.PlaysoundSetting.SoundName = PlaysoundSetting.SoundName;
                        i.PlaysoundSetting.StopSound = PlaysoundSetting.StopSound;
                        i.PlaysoundSetting.PitchPlayable = PlaysoundSetting.PitchPlayable;
                    }
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            void RaisePropertyChanged(string propname)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
            }
        }
        public enum MidiSettingType
        {
            Track,
            Instrument
        }
        /// <summary>
        /// Confirm Changes from TrackList
        /// </summary>
        public void UpdateByTrackList()
        {
            foreach (var t in TrackList)
            {
                foreach (var i in t.Instruments)
                {
                    if (i.Enable)
                    {
                        EnableMidi(i.EnableScore, t.Name, i.Name, -1, "");
                        EnableMidi(i.EnablePlaysound, t.Name, i.Name, -1, "PlaySound");
                        if (i.EnableScore)
                        {
                            EnableMidi(i.BarIndex, t.Name, i.Name, -1, "BarIndex");
                            EnableMidi(i.BeatDuration, t.Name, i.Name, -1, "BeatDuration");
                            EnableMidi(i.Channel, t.Name, i.Name, -1, "Channel");
                            EnableMidi(i.DeltaTickDuration, t.Name, i.Name, -1, "DeltaTickDuration");
                            EnableMidi(i.DeltaTickStart, t.Name, i.Name, -1, "DeltaTickStart");
                            EnableMidi(i.Velocity, t.Name, i.Name, -1, "Velocity");
                            EnableMidi(i.Pitch, t.Name, i.Name, -1, "Pitch");
                            EnableMidi(i.MinecraftTickDuration, t.Name, i.Name, -1, "MinecraftTickDuration");
                            EnableMidi(i.MinecraftTickStart, t.Name, i.Name, -1, "MinecraftTickStart");
                        }
                        if (i.EnablePlaysound || i.PlaysoundSetting.Enable)
                        {
                            Sound_ExecuteCood(i.PlaysoundSetting.ExecuteCood, t.Name, i.Name, -1);
                            Sound_ExecuteTarget(i.PlaysoundSetting.ExecuteTarget, t.Name, i.Name, -1);
                            Sound_ExtraDelay(i.PlaysoundSetting.ExtraDelay, t.Name, i.Name, -1);
                            Sound_MandaVolume(i.PlaysoundSetting.MandaVolume, t.Name, i.Name, -1);
                            Sound_InheritExpression(i.PlaysoundSetting.InheritExpression, t.Name, i.Name, -1);
                            Sound_PercVolume(i.PlaysoundSetting.PercVolume, t.Name, i.Name, -1);
                            Sound_PlaySource(i.PlaysoundSetting.PlaySource, t.Name, i.Name, -1);
                            Sound_PlayTarget(i.PlaysoundSetting.PlayTarget, t.Name, i.Name, -1);
                            Sound_SoundName(i.PlaysoundSetting.SoundName, t.Name, i.Name, -1);
                            Sound_StopSound(i.PlaysoundSetting.StopSound, t.Name, i.Name, -1);
                            Sound_PitchPlayable(i.PlaysoundSetting.PitchPlayable, t.Name, i.Name, -1);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Confirm Changes from TrackList
        /// Also Update TrackList
        /// </summary>
        public void UpdateInstrumentList()
        {
            foreach (var i in InstrumentList)
            {
                if (i.Enable)
                {
                    EnableMidi(i.EnableScore, "", i.Name, -1, "");
                    EnableMidi(i.EnablePlaysound, "", i.Name, -1, "PlaySound");
                    if (i.EnableScore)
                    {
                        EnableMidi(i.BarIndex, "", i.Name, -1, "BarIndex");
                        EnableMidi(i.BeatDuration, "", i.Name, -1, "BeatDuration");
                        EnableMidi(i.Channel, "", i.Name, -1, "Channel");
                        EnableMidi(i.DeltaTickDuration, "", i.Name, -1, "DeltaTickDuration");
                        EnableMidi(i.DeltaTickStart, "", i.Name, -1, "DeltaTickStart");
                        EnableMidi(i.Velocity, "", i.Name, -1, "Velocity");
                        EnableMidi(i.Pitch, "", i.Name, -1, "Pitch");
                        EnableMidi(i.MinecraftTickDuration, "", i.Name, -1, "MinecraftTickDuration");
                        EnableMidi(i.MinecraftTickStart, "", i.Name, -1, "MinecraftTickStart");
                        EnableMidi(i.EnablePlaysound, "", i.Name, -1, "PlaySound");
                    }
                    if (i.EnablePlaysound || i.PlaysoundSetting.Enable)
                    {
                        Sound_ExecuteCood(i.PlaysoundSetting.ExecuteCood, "", i.Name, -1);
                        Sound_ExecuteTarget(i.PlaysoundSetting.ExecuteTarget, "", i.Name, -1);
                        Sound_ExtraDelay(i.PlaysoundSetting.ExtraDelay, "", i.Name, -1);
                        Sound_MandaVolume(i.PlaysoundSetting.MandaVolume, "", i.Name, -1);
                        Sound_InheritExpression(i.PlaysoundSetting.InheritExpression, "", i.Name, -1);
                        Sound_PercVolume(i.PlaysoundSetting.PercVolume, "", i.Name, -1);
                        Sound_PlaySource(i.PlaysoundSetting.PlaySource, "", i.Name, -1);
                        Sound_PlayTarget(i.PlaysoundSetting.PlayTarget, "", i.Name, -1);
                        Sound_SoundName(i.PlaysoundSetting.SoundName, "", i.Name, -1);
                        Sound_StopSound(i.PlaysoundSetting.StopSound, "", i.Name, -1);
                        Sound_PitchPlayable(i.PlaysoundSetting.PitchPlayable, "", i.Name, -1);
                    }
                    foreach (var t in i.Tracks)
                    {
                        t.BarIndex = i.BarIndex;
                        t.BeatDuration = i.BeatDuration;
                        t.Channel = i.Channel;
                        t.DeltaTickDuration = i.DeltaTickDuration;
                        t.DeltaTickStart = i.DeltaTickStart;
                        t.Velocity = i.Velocity;
                        t.Pitch = i.Pitch;
                        t.MinecraftTickDuration = i.MinecraftTickDuration;
                        t.MinecraftTickStart = i.MinecraftTickStart;
                        t.PlaysoundSetting = i.PlaysoundSetting;
                    }
                }
            }
        }
        public WaveSettingInspector LeftWaveSetting = new WaveSettingInspector();
        public WaveSettingInspector RightWaveSetting = new WaveSettingInspector() { Enable = false, Frequency = false, Volume = false };
        public class WaveSettingInspector
        {
            private bool _enable = true;
            public bool Enable { get { return _enable; } set { _enable = value; if (value == false) { Frequency = false; Volume = false; } } }
            private bool _fre = true;
            public bool Frequency { get { return _fre; } set { _fre = value; } }
            private bool _vol = true;
            public bool Volume { get { return _vol; } set { _vol = value; } }
        }
        public void UpdateWave()
        {
            if (LeftWaveSetting.Enable == false)
            {
                EnableWave(LeftWaveSetting.Enable, -1, "Left");
            }
            else
            {
                EnableWave(LeftWaveSetting.Frequency, -1, "Left", "FrequencyPerTick");
                EnableWave(LeftWaveSetting.Volume, -1, "Left", "VolumePerTick");
            }

            if (RightWaveSetting.Enable == false)
            {
                EnableWave(RightWaveSetting.Enable, -1, "Right");
            }
            else
            {
                EnableWave(RightWaveSetting.Frequency, -1, "Right", "FrequencyPerTick");
                EnableWave(RightWaveSetting.Volume, -1, "Right", "VolumePerTick");
            }
        }
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