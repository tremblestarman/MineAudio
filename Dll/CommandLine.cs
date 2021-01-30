﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Audio2Minecraft
{
    /// <summary>
    /// 命令序列
    /// </summary>
    public class CommandLine
    {
        private int totalProgress = 0;
        public int TotalProgress { get { return totalProgress; } }
        private int currentProgress = 0;
        public int CurrentProgress { get { return currentProgress; } }
        /// <summary>
        /// 通过时间序列生成命令序列
        /// </summary>
        /// <param name="timeLine">时间序列</param>
        /// <param name="version">游戏版本</param>
        /// <returns></returns>
        public CommandLine Serialize(TimeLine timeLine, string version = "1.12", ShowProgress showProgress = null)
        {
            try
            {
                var commandLine = new CommandLine();
                //List of Scoreboards
                var scoreboards = new List<string>();
                //List of Entities
                var entities = new Dictionary<string, DescribeEntity>();
                #region Head of TimeLine
                entities.Add("GenParam", new DescribeEntity() { Feature = "GenParam", Count = 1 }); bool hasGenParam = timeLine.OutPutTick || timeLine.OutPutBPM;
                scoreboards = Param2ScoreboardsList(timeLine.Param, scoreboards);
                foreach (string param in timeLine.Param.Keys)
                {
                    if (timeLine.Param[param].Enable == true)
                    {
                        hasGenParam = true;
                        commandLine.Start.Add(setCommand("GenParam", timeLine.Param[param].Name, timeLine.Param[param].Value, version));
                    }
                }
                if (timeLine.OutPutTick)
                    scoreboards.Add("CurrentTick");
                if (timeLine.OutPutBPM)
                    scoreboards.Add("CurrentBPM");
                #endregion
                #region Keyframes
                //Command:
                var cmdExecute = "execute "; var cmdRelative = ""; var isrun = false;
                if (version == "1.13") { cmdExecute = "execute as "; cmdRelative = " at @s positioned"; isrun = true; }
                //Create Keyframes
                for (int c = 0; c < timeLine.TickNodes.Count; c++) commandLine.Keyframe.Add(new Command());
                //Foreach Tick
                for (int i = 0; i < timeLine.TickNodes.Count; i++)
                {
                    var tickNode = timeLine.TickNodes[i];
                    var midiNodes = tickNode.MidiTracks;
                    var waveNodesL = tickNode.WaveNodesLeft;
                    var waveNodesR = tickNode.WaveNodesRight;
                    if (timeLine.OutPutTick)
                    {
                        commandLine.Keyframe[i].Commands.Add(setCommand("GenParam", "CurrentTick", i, version));
                    }
                    #region Midi
                    if (midiNodes.Count > 0)
                    {
                        foreach (string track in midiNodes.Keys)
                        {
                            foreach (string instrument in midiNodes[track].Keys)
                            {
                                var nodes = midiNodes[track][instrument];
                                for (int j = 0; j < nodes.Count; j++)
                                {
                                    var node = nodes[j];
                                    if (node.IsEvent == true)
                                    {
                                        if (timeLine.OutPutBPM)
                                            commandLine.Keyframe[i].Commands.Add(setCommand("GenParam", "CurrentBPM", node.Param["BeatPerMinute"].Value, version));
                                        continue;
                                    }
                                    //Set Midi Tag
                                    var regex = new Regex("[^a-z^A-Z^0-9_]");
                                    var TrackName = regex.Replace(node.TrackName, "_");
                                    var Instrument = regex.Replace(node.Instrument, "_");
                                    var _track = "t_" + TrackName;
                                    var _instrument = "i_" + Instrument;
                                    var feature = ((TrackName.Length > 5) ? TrackName.Substring(0, 5) : TrackName) + "_" + ((Instrument.Length > 5) ? Instrument.Substring(0, 5) : Instrument);
                                    //Add Command
                                    foreach (string k in node.Param.Keys)
                                    {
                                        if (node.Param[k].Enable)
                                        {
                                            //Update ScoreboardsList & EntitiesList
                                            if (entities.Keys.Contains(feature))
                                            {
                                                if (j >= entities[feature].Count)
                                                    entities[feature].Count = j + 1;
                                            }
                                            else
                                            {
                                                entities.Add(feature, new DescribeEntity() { Feature = feature, Instrument = _instrument, Track = _track, Count = 1 });
                                            }
                                            scoreboards = Param2ScoreboardsList(node.Param, scoreboards);
                                            var f = feature + "_" + j;
                                            commandLine.Keyframe[i].Commands.Add(setCommand(f, node.Param[k].Name, node.Param[k].Value, version));
                                        }
                                    }
                                    #region Playsound & Stopsound
                                    if (node.PlaySound.Enable && node.PlaySound.PlaySource != "" && node.PlaySound.PlaySource != null)//Enable Playsound
                                    {
                                        //PlaySound
                                        var playsound = node.PlaySound;
                                        //Set Expression
                                        var subName = InheritExpression.Expression(playsound.InheritExpression, node.Param["Pitch"].Value, node.Param["MinecraftTickDuration"].Value, node.Param["Velocity"].Value, node.Param["BarIndex"].Value, node.Param["BeatDuration"].Value, node.Param["Channel"].Value);
                                        var rxp = (playsound.SoundName != "" && subName != "") ? "." : "";
                                        if (playsound.StopSound)//Enable Stopsound
                                        {
                                            var endtick = node.Param["MinecraftTickStart"].Value + node.Param["MinecraftTickDuration"].Value + node.PlaySound.ExtraDelay;
                                            if (endtick >= timeLine.TickNodes.Count)
                                            {
                                                var nowCount = commandLine.Keyframe.Count;
                                                for (int m = 0; m < endtick - nowCount + 1; m++) commandLine.Keyframe.Add(new Command());
                                            }
                                            var command1 = cmdExecute + playsound.ExecuteTarget + cmdRelative + " ~ ~ ~ " + ((isrun) ? "run " : "") + "stopsound " + playsound.PlayTarget + " " + playsound.PlaySource + " " + playsound.SoundName + rxp + subName;
                                            for (int _t = node.Param["MinecraftTickStart"].Value + 1; _t < endtick; _t++)//Avoid Stopping Ahead
                                            {
                                                if (commandLine.Keyframe[_t].Commands.Contains(command1)) commandLine.Keyframe[_t].Commands.Remove(command1);
                                            }
                                            commandLine.Keyframe[endtick].Commands.Add(command1);
                                        }
                                        //Set Cood
                                        string cood = "~" + ((playsound.ExecuteCood[0] == 0) ? "" : playsound.ExecuteCood[0].ToString()) + " ~" + ((playsound.ExecuteCood[1] == 0) ? "" : playsound.ExecuteCood[1].ToString()) + " ~" + ((playsound.ExecuteCood[2] == 0) ? "" : playsound.ExecuteCood[2].ToString());
                                        //Set Volume
                                        double vp = ((playsound.PercVolume < 0) ? (double)100 : (double)playsound.PercVolume) / 100;
                                        double manda_vol = (playsound.MandaVolume < 0) ? 1 : (double)playsound.MandaVolume / 100;
                                        var volume = (node.Param["Velocity"].Value * manda_vol * vp / 100 > 2) ? 2 : (node.Param["Velocity"].Value * manda_vol * vp / 100 < 0) ? 0 : (double)node.Param["Velocity"].Value * vp * manda_vol / 100;
                                        var command = cmdExecute + playsound.ExecuteTarget + cmdRelative + " ~ ~ ~ " + ((isrun) ? "run " : "") + "playsound " + playsound.SoundName + rxp + subName + " " + playsound.PlaySource + " " + playsound.PlayTarget + " " + cood + " " + volume + ((node.PlaySound.PitchPlayable) ? (" " + setPlaysoundPitch(node.Param["Pitch"].Value)) : "");
                                        commandLine.Keyframe[i].Commands.Add(command);
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                    #endregion
                    #region Wave
                    //Wave Left
                    if (waveNodesL.Count > 0)
                    {
                        for (int j = 0; j < waveNodesL.Count; j++)
                        {
                            var node = waveNodesL[j];
                            //Set Wave Tag
                            var feature = "wave" + j.ToString() + "_l";

                            var param = node.Param;
                            foreach (string k in node.Param.Keys)
                            {
                                for (int n = 0; n < node.Param[k].Count; n++)
                                {
                                    if (node.Param[k][n].Enable)
                                    {
                                        //Update ScoreboardsList & EntitiesList
                                        if (!entities.Keys.Contains(feature))
                                            entities.Add(feature, new DescribeEntity() { Feature = feature, Count = node.Param[k].Count });
                                        if (param[k][n].Enable && !scoreboards.Contains(param[k][n].Name))
                                            scoreboards.Add(param[k][n].Name);
                                        commandLine.Keyframe[i].Commands.Add(setCommand(feature + "_" + n, param[k][n].Name, param[k][n].Value, version));
                                    }
                                }
                            }

                        }
                    }
                    //Wave Right
                    if (waveNodesR.Count > 0)
                    {
                        for (int j = 0; j < waveNodesR.Count; j++)
                        {
                            var node = waveNodesR[j];
                            //Set Wave Tag
                            var feature = "wave" + j.ToString() + "_r";
                            var param = node.Param;
                            foreach (string k in node.Param.Keys)
                            {
                                for (int n = 0; n < node.Param[k].Count; n++)
                                {
                                    if (node.Param[k][n].Enable)
                                    {
                                        //Update ScoreboardsList & EntitiesList
                                        if (!entities.Keys.Contains(feature))
                                            entities.Add(feature, new DescribeEntity() { Feature = feature, Count = node.Param[k].Count });
                                        if (param[k][n].Enable && !scoreboards.Contains(param[k][n].Name))
                                            scoreboards.Add(param[k][n].Name);
                                        commandLine.Keyframe[i].Commands.Add(setCommand(feature + "_" + n, param[k][n].Name, param[k][n].Value, version));
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    /* Update Current Progress */ this.currentProgress++; if (showProgress != null && this.totalProgress > 0) showProgress((double)this.currentProgress / this.totalProgress);
                }
                #endregion
                #region End of Timeline
                foreach (var feature in entities.Keys)
                {
                    var entity = entities[feature];
                    for (int m = 0; m < entity.Count; m++)
                    {
                        if (entity.Feature == "GenParam" && !hasGenParam) continue; // No General Param
                        var tags = (entity.Feature == "GenParam") ? "GenParam" : entity.Feature + "_" + m + "," + entity.Track + "," + entity.Instrument;
                        if (version == "1.12")
                            commandLine.Start.Insert(0, "summon area_effect_cloud ~ ~ ~ {Tags:[" + tags + ",AudioRiptideNode],Duration:" + (commandLine.Keyframe.Count * 10).ToString() + "}");
                        else
                            commandLine.End.Add("scoreboard players reset " + entity.Feature + "_" + m);
                    }
                }
                foreach (var scoreboard in scoreboards)
                {
                    commandLine.Start.Insert(0, "scoreboard objectives add " + scoreboard + " dummy");
                    commandLine.End.Add("scoreboard objectives remove " + scoreboard);
                }
                commandLine.End.Add("kill @e[tag=AudioRiptideNode]");
                #endregion
                return commandLine;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 通过时间序列生成指定音轨/乐器的命令序列
        /// </summary>
        /// <param name="timeLine">时间序列</param>
        /// <param name="trackName">音轨名(默认null)</param>
        /// <param name="instrumentName">乐器名(默认null)</param>
        /// <param name="version">游戏版本</param>
        /// <returns></returns>
        public CommandLine SerializeSpecified(TimeLine timeLine, string trackName = null, string instrumentName = null, bool waveLeft = true, bool waveRight = true, string version = "1.12", ShowProgress showProgress = null)
        {
            try
            {
                var commandLine = new CommandLine();
                //List of Scoreboards
                var scoreboards = new List<string>();
                //List of Entities
                var entities = new Dictionary<string, DescribeEntity>();
                #region Head of TimeLine
                entities.Add("GenParam", new DescribeEntity() { Feature = "GenParam", Count = 1 }); bool hasGenParam = timeLine.OutPutTick || timeLine.OutPutBPM;
                scoreboards = Param2ScoreboardsList(timeLine.Param, scoreboards);
                foreach (string param in timeLine.Param.Keys)
                {
                    if (timeLine.Param[param].Enable == true)
                    {
                        hasGenParam = true;
                        commandLine.Start.Add(setCommand("GenParam", timeLine.Param[param].Name, timeLine.Param[param].Value, version));
                    }
                }
                if (timeLine.OutPutTick)
                    scoreboards.Add("CurrentTick");
                if (timeLine.OutPutBPM)
                    scoreboards.Add("CurrentBPM");
                #endregion
                #region Keyframes
                /* Set Total Progress */ this.totalProgress = timeLine.TickNodes.Count;
                //Command:
                var cmdExecute = "execute "; var cmdRelative = ""; var isrun = false;
                if (version == "1.13") { cmdExecute = "execute as "; cmdRelative = " at @s positioned"; isrun = true; }
                //Create Keyframes
                for (int c = 0; c < timeLine.TickNodes.Count; c++) commandLine.Keyframe.Add(new Command());
                //Foreach Tick
                for (int i = 0; i < timeLine.TickNodes.Count; i++)
                {
                    var tickNode = timeLine.TickNodes[i];
                    var midiNodes = tickNode.MidiTracks;
                    var waveNodesL = tickNode.WaveNodesLeft;
                    var waveNodesR = tickNode.WaveNodesRight;
                    if (timeLine.OutPutTick)
                    {
                        commandLine.Keyframe[i].Commands.Add(setCommand("GenParam", "CurrentTick", i, version));
                    }
                    #region Midi
                    if (midiNodes.Count > 0 && !(trackName == null && instrumentName == null))
                    {
                        foreach (string track in midiNodes.Keys)
                        {
                            if (trackName != track && trackName != null) continue;
                            foreach (string instrument in midiNodes[track].Keys)
                            {
                                if (instrumentName != track && instrumentName != null) continue;
                                var nodes = midiNodes[track][instrument];
                                for (int j = 0; j < nodes.Count; j++)
                                {
                                    var node = nodes[j];
                                    if (node.IsEvent == true)
                                    {
                                        if (timeLine.OutPutBPM)
                                            commandLine.Keyframe[i].Commands.Add(setCommand("GenParam", "CurrentBPM", node.Param["BeatPerMinute"].Value, version));
                                        continue;
                                    }
                                    //Set Midi Tag
                                    var regex = new Regex("[^a-z^A-Z^0-9_]");
                                    var TrackName = regex.Replace(node.TrackName, "_");
                                    var Instrument = regex.Replace(node.Instrument, "_");
                                    var _track = "t_" + TrackName;
                                    var _instrument = "i_" + Instrument;
                                    var feature = ((TrackName.Length > 5) ? TrackName.Substring(0, 5) : TrackName) + "_" + ((Instrument.Length > 5) ? Instrument.Substring(0, 5) : Instrument);
                                    //Add Command
                                    foreach (string k in node.Param.Keys)
                                    {
                                        if (node.Param[k].Enable)
                                        {
                                            //Update ScoreboardsList & EntitiesList
                                            if (entities.Keys.Contains(feature))
                                            {
                                                if (j >= entities[feature].Count)
                                                    entities[feature].Count = j + 1;
                                            }
                                            else
                                            {
                                                entities.Add(feature, new DescribeEntity() { Feature = feature, Instrument = _instrument, Track = _track, Count = 1 });
                                            }
                                            scoreboards = Param2ScoreboardsList(node.Param, scoreboards);
                                            var f = feature + "_" + j;
                                            commandLine.Keyframe[i].Commands.Add(setCommand(f, node.Param[k].Name, node.Param[k].Value, version));
                                        }
                                    }
                                    #region Playsound & Stopsound
                                    if (node.PlaySound.Enable && node.PlaySound.PlaySource != "" && node.PlaySound.PlaySource != null)//Enable Playsound
                                    {
                                        //PlaySound
                                        var playsound = node.PlaySound;
                                        //Set Expression
                                        var subName = InheritExpression.Expression(playsound.InheritExpression, node.Param["Pitch"].Value, node.Param["MinecraftTickDuration"].Value, node.Param["Velocity"].Value, node.Param["BarIndex"].Value, node.Param["BeatDuration"].Value, node.Param["Channel"].Value);
                                        var rxp = (playsound.SoundName != "" && subName != "") ? "." : "";
                                        if (playsound.StopSound)//Enable Stopsound
                                        {
                                            var endtick = node.Param["MinecraftTickStart"].Value + node.Param["MinecraftTickDuration"].Value + node.PlaySound.ExtraDelay;
                                            if (endtick >= timeLine.TickNodes.Count)
                                            {
                                                var nowCount = commandLine.Keyframe.Count;
                                                for (int m = 0; m < endtick - nowCount + 1; m++) commandLine.Keyframe.Add(new Command());
                                            }
                                            var command1 = cmdExecute + playsound.ExecuteTarget + cmdRelative + " ~ ~ ~ " + ((isrun) ? "run " : "") + "stopsound " + playsound.PlayTarget + " " + playsound.PlaySource + " " + playsound.SoundName + rxp + subName;
                                            for (int _t = node.Param["MinecraftTickStart"].Value + 1; _t < endtick; _t++)//Avoid Stopping Ahead
                                            {
                                                if (commandLine.Keyframe[_t].Commands.Contains(command1)) commandLine.Keyframe[_t].Commands.Remove(command1);
                                            }
                                            commandLine.Keyframe[endtick].Commands.Add(command1);
                                        }
                                        //Set Cood
                                        string cood = "~" + ((playsound.ExecuteCood[0] == 0) ? "" : playsound.ExecuteCood[0].ToString()) + " ~" + ((playsound.ExecuteCood[1] == 0) ? "" : playsound.ExecuteCood[1].ToString()) + " ~" + ((playsound.ExecuteCood[2] == 0) ? "" : playsound.ExecuteCood[2].ToString());
                                        //Set Volume
                                        double vp = ((playsound.PercVolume < 0) ? (double)100 : (double)playsound.PercVolume) / 100;
                                        double manda_vol = (playsound.MandaVolume < 0) ? 1 : (double)playsound.MandaVolume / 100;
                                        var volume = (node.Param["Velocity"].Value * manda_vol * vp / 100 > 2) ? 2 : (node.Param["Velocity"].Value * manda_vol * vp / 100 < 0) ? 0 : (double)node.Param["Velocity"].Value * vp * manda_vol / 100;
                                        var command = cmdExecute + playsound.ExecuteTarget + cmdRelative + " ~ ~ ~ " + ((isrun) ? "run " : "") + "playsound " + playsound.SoundName + rxp + subName + " " + playsound.PlaySource + " " + playsound.PlayTarget + " " + cood + " " + volume + ((node.PlaySound.PitchPlayable) ? (" " + setPlaysoundPitch(node.Param["Pitch"].Value)) : "");
                                        commandLine.Keyframe[i].Commands.Add(command);
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                    #endregion
                    #region Wave
                    //Wave Left
                    if (waveNodesL.Count > 0 && waveLeft == true)
                    {
                        for (int j = 0; j < waveNodesL.Count; j++)
                        {
                            var node = waveNodesL[j];
                            //Set Wave Tag
                            var feature = "wave" + j.ToString() + "_l";

                            var param = node.Param;
                            foreach (string k in node.Param.Keys)
                            {
                                for (int n = 0; n < node.Param[k].Count; n++)
                                {
                                    if (node.Param[k][n].Enable)
                                    {
                                        //Update ScoreboardsList & EntitiesList
                                        if (!entities.Keys.Contains(feature))
                                            entities.Add(feature, new DescribeEntity() { Feature = feature, Count = node.Param[k].Count });
                                        if (param[k][n].Enable && !scoreboards.Contains(param[k][n].Name))
                                            scoreboards.Add(param[k][n].Name);
                                        commandLine.Keyframe[i].Commands.Add(setCommand(feature + "_" + n, param[k][n].Name, param[k][n].Value, version));
                                    }
                                }
                            }

                        }
                    }
                    //Wave Right
                    if (waveNodesR.Count > 0 && waveRight == true)
                    {
                        for (int j = 0; j < waveNodesR.Count; j++)
                        {
                            var node = waveNodesR[j];
                            //Set Wave Tag
                            var feature = "wave" + j.ToString() + "_r";
                            var param = node.Param;
                            foreach (string k in node.Param.Keys)
                            {
                                for (int n = 0; n < node.Param[k].Count; n++)
                                {
                                    if (node.Param[k][n].Enable)
                                    {
                                        //Update ScoreboardsList & EntitiesList
                                        if (!entities.Keys.Contains(feature))
                                            entities.Add(feature, new DescribeEntity() { Feature = feature, Count = node.Param[k].Count });
                                        if (param[k][n].Enable && !scoreboards.Contains(param[k][n].Name))
                                            scoreboards.Add(param[k][n].Name);
                                        commandLine.Keyframe[i].Commands.Add(setCommand(feature + "_" + n, param[k][n].Name, param[k][n].Value, version));
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    /* Update Current Progress */ this.currentProgress++; if (showProgress != null && this.totalProgress > 0) showProgress((double)this.currentProgress / this.totalProgress);
                }
                #endregion
                #region End of Timeline
                foreach (var feature in entities.Keys)
                {
                    var entity = entities[feature];
                    for (int m = 0; m < entity.Count; m++)
                    {
                        if (entity.Feature == "GenParam" && !hasGenParam) continue; // No General Param
                        var tags = (entity.Feature == "GenParam") ? "GenParam" : entity.Feature + "_" + m + "," + entity.Track + "," + entity.Instrument;
                        if (version == "1.12")
                            commandLine.Start.Insert(0, "summon area_effect_cloud ~ ~ ~ {Tags:[" + tags + ",AudioRiptideNode],Duration:" + (commandLine.Keyframe.Count * 10).ToString() + "}");
                        else
                            commandLine.End.Add("scoreboard players reset " + entity.Feature + "_" + m);
                    }
                }
                foreach (var scoreboard in scoreboards)
                {
                    commandLine.Start.Insert(0, "scoreboard objectives add " + scoreboard + " dummy");
                    commandLine.End.Add("scoreboard objectives remove " + scoreboard);
                }
                commandLine.End.Add("kill @e[tag=AudioRiptideNode]");
                #endregion
                return commandLine;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 合并两命令序列(起始时间相同)
        /// </summary>
        /// <param name="A">命令序列A</param>
        /// <param name="B">命令序列B</param>
        /// <returns></returns>
        public CommandLine Combine(CommandLine A, CommandLine B)
        {
            var C = new CommandLine();
            var keyi = (A.Keyframe.Count > B.Keyframe.Count) ? A.Keyframe.Count : B.Keyframe.Count;
            //var starti = (A.Start.Count > B.Start.Count) ? A.Start.Count : B.Start.Count;
            //var endi = (A.End.Count > B.End.Count) ? A.End.Count : B.End.Count;
            //Keyframe
            for (int i = 0; i < keyi; i++)
            {
                C.Keyframe.Add(new Command());
                if (A.Keyframe.Count <= i) C.Keyframe[i] = B.Keyframe[i];
                else if (B.Keyframe.Count <= i) C.Keyframe[i] = A.Keyframe[i];
                else
                {
                    C.Keyframe[i].Commands = A.Keyframe[i].Commands.Union(B.Keyframe[i].Commands).ToList<string>();
                }
            }
            //Start
            if (A.Start.Count == 0) C.Start = B.Start;
            else if (B.Start.Count == 0) C.Start = A.Start;
            else
            {
                C.Start = A.Start.Union(B.Start).ToList<string>();
            }
            //End
            if (A.End.Count == 0) C.End = B.End;
            else if (B.End.Count == 0) C.End = A.End;
            else
            {
                C.End = A.End.Union(B.End).ToList<string>();
            }
            return C;
        }
        private List<string> Param2ScoreboardsList(Dictionary<string, _Node_INT> Param, List<string> scoreboards)
        {
            foreach (string k in Param.Keys)
            {
                if (Param[k].Enable && !scoreboards.Contains(Param[k].Name))
                    scoreboards.Add(Param[k].Name);
            }
            return scoreboards;
        }
        private string setPlaysoundPitch(int pitch)
        {
            return Math.Pow(2, (((double)pitch + 18) % 24 - 12) / 12).ToString("0.00000");
        }
        private string setCommand(string target, string score_name, int score, string version = "1.12")
        {
            if (version == "1.13")
                return "scoreboard players set " + target + " " + score_name + " " + score.ToString();
            else
                return "scoreboard players set @e[type=area_effect_cloud,tag=" + target + "] " + score_name + " " + score.ToString();
        }
        /// <summary>
        /// 初始化命令
        /// </summary>
        public List<string> Start = new List<string>();
        /// <summary>
        /// 重置命令
        /// </summary>
        public List<string> End = new List<string>();
        /// <summary>
        /// 关键帧
        /// </summary>
        public List<Command> Keyframe = new List<Command>();
    }
    public class Command
    {
        public List<string> Commands = new List<string>() { "$setblock", "setblock ~ ~-2 ~ minecraft:air", "setworldspawn ~ ~ ~", "tp @e[tag=Tracks] @p" };
    }
    public class DescribeEntity
    {
        private string f = "none";
        public string Feature { get { return f; } set { f = value; } }
        private string t = "t_none";
        public string Track { get { return t; } set { t = value; } }
        private string i = "i_none";
        public string Instrument { get { return i; } set { i = value; } }
        private int c = 0;
        public int Count { get { return c; } set { c = value; } }
    }
}