using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Audio2Minecraft
{
    public class CommandLine
    {
        public CommandLine Serialize(TimeLine timeLine, string version = "1.12")
        {
            try
            {
                var commandLine = new CommandLine();
                //List of Scoreboards
                var scoreboards = new List<string>();
                //List of Entities
                var entities = new Dictionary<string, DescribeEntity>();
                #region Head of TimeLine
                entities.Add("GenParam", new DescribeEntity() { Feature = "GenParam", Count = 1 });
                scoreboards = Param2ScoreboardsList(timeLine.Param, scoreboards);
                foreach (string param in timeLine.Param.Keys)
                {
                    if (timeLine.Param[param].Enable == true)
                        commandLine.Start.Add(setCommand("GenParam", timeLine.Param[param].Name, timeLine.Param[param].Value, version));
                }
                if (timeLine.OutPutTick)
                    scoreboards.Add("CurrentTick");
                #endregion
                #region Keyframes
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
                                        var subName = InheritExpression.Expression(playsound.InheritExpression, node.Param["Pitch"].Value, node.Param["MinecraftTickDuration"].Value);
                                        var rxp = (playsound.SoundName != "" && subName != "") ? "." : "";
                                        if (playsound.StopSound)//Enable Stopsound
                                        {
                                            var endtick = node.Param["MinecraftTickStart"].Value + node.Param["MinecraftTickDuration"].Value + node.PlaySound.ExtraDelay;
                                            if (endtick >= timeLine.TickNodes.Count)
                                            {
                                                var nowCount = commandLine.Keyframe.Count;
                                                for (int m = 0; m < endtick - nowCount + 1; m++) commandLine.Keyframe.Add(new Command());
                                            }
                                            var command1 = "execute " + playsound.ExecuteTarget + " ~ ~ ~ stopsound " + playsound.PlayTarget + " " + playsound.PlaySource + " " + playsound.SoundName + rxp + subName;
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
                                        var command = "execute " + playsound.ExecuteTarget + " ~ ~ ~ playsound " + playsound.SoundName + rxp + subName + " " + playsound.PlaySource + " " + playsound.PlayTarget + " " + cood + " " + volume;
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
                }
                #endregion
                #region End of Timeline
                foreach (var feature in entities.Keys)
                {
                    var entity = entities[feature];
                    for (int m = 0; m < entity.Count; m++)
                    {
                        var tags = entity.Feature + "_" + m + "," + entity.Track + "," + entity.Instrument;
                        commandLine.Start.Insert(0, "summon area_effect_cloud ~ ~ ~ {Tags:[" + tags + ",AudioRiptideNode],Duration:" + (commandLine.Keyframe.Count * 10).ToString() + "}");
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
        private string setCommand(string target, string score_name, int score, string version = "1.12")
        {
            if (version == "1.13")
                return "";
            else
                return "scoreboard players set @e[type=area_effect_cloud,tag=" + target + "] " + score_name + " " + score.ToString();
        }
        public List<string> Start = new List<string>();
        public List<string> End = new List<string>();
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