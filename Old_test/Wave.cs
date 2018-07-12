using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using System.Windows.Forms;

namespace Audio2Minecraft
{
    /// <summary>
    /// 波形序列操作
    /// </summary>
    public class AudioStreamWave
    {
        /// <summary>
        /// 通过波形生成时间序列
        /// </summary>
        /// <param name="fileName">波形文件路径</param>
        /// <param name="timeLine">时间序列</param>
        /// <param name="fre_count">频率采样数</param>
        /// <param name="vol_count">振幅采样数</param>
        /// <param name="tick_cycle">采样周期</param>
        /// <returns>时间序列</returns>
        public TimeLine Serialize(string fileName, TimeLine timeLine, int fre_count = 1, int vol_count = 1, int tick_cycle = 1)
        {
            try
            {
                //Read Waves
                var reader = new WaveFileReader(fileName);
                reader.Position = 0;
                #region WaveFile -> WaveNodes
                //Create WaveNodes
                var waveNodesL = new List<WaveNode>();
                var waveNodesR = new List<WaveNode>();
                float[] samplesL = new float[reader.Length / reader.BlockAlign];
                float[] samplesR = new float[reader.Length / reader.BlockAlign];
                //Get Time-related Param
                var Hz = (int)(samplesL.Length / reader.TotalTime.TotalSeconds);
                var FperTick = Hz / 20;
                var SampleCycle = FperTick * tick_cycle;
                var totalTick = samplesL.Length / FperTick;
                //Frequency & Peak
                int[] Fre = new int[] {/* left */0, /* right */0 };
                float[] Peak = new float[] {/* left_min */0, /* right_min */0, /* left_max */0, /* right_max */0 };
                #region Set Nodes' 
                //Foreach Frequency & Peak in WaveFile
                int _temp = -1;
                for (int i = 0; i < samplesL.Length; i++)
                {
                    //Get Sample
                    float[] sample = reader.ReadNextSampleFrame();
                    samplesL[i] = sample[0]; //Mono - Right
                    samplesR[i] = sample[1]; //Stereo - Left
                    //Get Frequency & Peak
                    if (i > 0) 
                    {
                        //Get Frequency
                        if (samplesL[i] * samplesL[i - 1] < 0) Fre[0]++;
                        if (samplesR[i] * samplesR[i - 1] < 0) Fre[1]++;
                        //Get Peak
                        if (samplesL[i] < Peak[0]) Peak[0] = samplesL[i]; else if (samplesL[i] > Peak[1]) Peak[1] = samplesL[i];
                        if (samplesR[i] < Peak[2]) Peak[2] = samplesR[i]; else if (samplesR[i] > Peak[3]) Peak[3] = samplesR[i];

                        //Creat Objects
                        int v = (i - 1) % SampleCycle;
                        int g = (i - 1) % FperTick;
                        int t = (i - 1) / FperTick;
                        if (v == 0)
                        {
                            waveNodesL.Add(new WaveNode()
                            {
                                TickStart = t,
                                IsLeft = true
                            });
                            waveNodesR.Add(new WaveNode()
                            {
                                TickStart = t,
                                IsLeft = false
                            });
                            _temp = t;
                        }
                        else if (g == 0)
                        {
                            waveNodesL.Add(new WaveNode()
                            {
                                TickStart = t,
                                IsLeft = true
                            });
                            waveNodesR.Add(new WaveNode()
                            {
                                TickStart = t,
                                IsLeft = false
                            });
                        }
                        //Write To Nodes
                        float ft = (float)SampleCycle / fre_count;
                        float vt = (float)SampleCycle / vol_count;
                        if (v % ft < 1)
                        {
                            waveNodesL[_temp].Param["FrequencyPerTick"].Add(new _Node_INT() { Name = "Fre", Value = Fre[0] });
                            waveNodesR[_temp].Param["FrequencyPerTick"].Add(new _Node_INT() { Name = "Fre", Value = Fre[1] });
                            Fre[0] = 0; Fre[1] = 0;
                        }
                        if (v % vt < 1)
                        {
                            waveNodesL[_temp].Param["VolumePerTick"].Add( new _Node_INT() { Name = "Vol", Value = (int)((Peak[1] - Peak[0]) * 1000) });
                            waveNodesR[_temp].Param["VolumePerTick"].Add(new _Node_INT() { Name = "Vol", Value = (int)((Peak[3] - Peak[2]) * 1000) });
                            Peak[0] = 0; Peak[1] = 0; Peak[2] = 0; Peak[3] = 0;
                        }
                    }
                }
                #endregion
                #endregion
                #region WaveNodes -> TickNodes
                //Creat and Set Lenth of TickNodes
                if (timeLine.TickNodes == null) timeLine.TickNodes = new List<TickNode>();
                if (totalTick >= timeLine.TickNodes.Count)
                {
                    var nowCount = timeLine.TickNodes.Count;
                    for (int i = 0; i < totalTick - nowCount + 1; i++) timeLine.TickNodes.Add(new TickNode());
                }
                //WaveNodes -> TickNodes
                foreach (WaveNode waveNodeL in waveNodesL)
                {
                    if (timeLine.TickNodes[waveNodeL.TickStart].WaveNodesLeft == null) timeLine.TickNodes[waveNodeL.TickStart].WaveNodesLeft = new List<WaveNode>();
                    timeLine.TickNodes[waveNodeL.TickStart].WaveNodesLeft.Add(waveNodeL);
                }
                foreach (WaveNode waveNodeR in waveNodesR)
                {
                    if (timeLine.TickNodes[waveNodeR.TickStart].WaveNodesRight == null) timeLine.TickNodes[waveNodeR.TickStart].WaveNodesRight = new List<WaveNode>();
                    timeLine.TickNodes[waveNodeR.TickStart].WaveNodesRight.Add(waveNodeR);
                }
                #endregion
                timeLine.Param["TotalTicks"].Value = timeLine.TickNodes.Count;
                return timeLine;
            }
            catch
            {
                return null;
            }
        }
    }
}
