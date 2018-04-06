using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using System.Windows.Forms;

namespace Audio2MinecraftScore
{
    class AudioStreamWave
    {
        public TimeLine Serialize(string fileName, TimeLine timeLine, int fre_count = 1, int vol_count = 1, int tick_cycle = 1)
        {
            try
            {
                var audioNodesL = new List<AudioNode>();
                var audioNodesR = new List<AudioNode>();
                var reader = new WaveFileReader(fileName);
                reader.Position = 0;
                #region Nodes
                float[] samplesL = new float[reader.Length / reader.BlockAlign];
                float[] samplesR = new float[reader.Length / reader.BlockAlign];
                //store fundamental time parameter
                var Hz = (int)(samplesL.Length / reader.TotalTime.TotalSeconds);
                var FperTick = Hz / 20;
                var SampleCycle = FperTick * tick_cycle;
                var totalTick = samplesL.Length / FperTick;
                //store frequency an volume
                int[] Fre = new int[] {/*l*/0,/*r*/0 };
                float[] Peak = new float[] {/*lmin*/0,/*rmin*/0,/*lmax*/0,/*rmax*/0 };
                /*20 T/S sample*/
                int _temp = -1;
                for (int i = 0; i < samplesL.Length; i++)
                {
                    float[] sample = reader.ReadNextSampleFrame();

                    samplesL[i] = sample[0]; //mono
                    samplesR[i] = sample[1]; //stereo
                    if (i > 0) //write in peak & frequency
                    {
                        //Get Frequency
                        if (samplesL[i] * samplesL[i - 1] < 0) Fre[0]++;
                        if (samplesR[i] * samplesR[i - 1] < 0) Fre[1]++;
                        //Get Volume
                        if (samplesL[i] < Peak[0]) Peak[0] = samplesL[i]; else if (samplesL[i] > Peak[1]) Peak[1] = samplesL[i];
                        if (samplesR[i] < Peak[2]) Peak[2] = samplesR[i]; else if (samplesR[i] > Peak[3]) Peak[3] = samplesR[i];

                        int v = (i - 1) % SampleCycle;
                        int g = (i - 1) % FperTick;
                        int t = (int)(i - 1) / FperTick;
                        if (v == 0) //Creat objects
                        {
                            audioNodesL.Add(new AudioNode()
                            {
                                MinecraftStartTick = t
                            });
                            audioNodesR.Add(new AudioNode()
                            {
                                MinecraftStartTick = t
                            });
                            _temp = t;
                        }
                        else if (g == 0)
                        {
                            audioNodesL.Add(new AudioNode()
                            {
                                MinecraftStartTick = t
                            });
                            audioNodesR.Add(new AudioNode()
                            {
                                MinecraftStartTick = t
                            });
                        }

                        float ft = (float)SampleCycle / fre_count;
                        float vt = (float)SampleCycle / vol_count;
                        if (v % ft < 1) //Write to objects
                        {
                            audioNodesL[_temp].MinecraftFrequencyPerTick.Add(Fre[0] / 2);
                            audioNodesR[_temp].MinecraftFrequencyPerTick.Add(Fre[1] / 2);
                            Fre[0] = 0; Fre[1] = 0;
                        }
                        if (v % vt < 1)
                        {
                            audioNodesL[_temp].MinecraftVolumePerTick.Add((int)((Peak[1] - Peak[0]) * 1000));
                            audioNodesR[_temp].MinecraftVolumePerTick.Add((int)((Peak[3] - Peak[2]) * 1000));
                            Peak[0] = 0; Peak[1] = 0; Peak[2] = 0; Peak[3] = 0;
                        }
                        /*if (v % fre_count == 0) //Write to objects
                        {
                            MessageBox.Show("" + v + fre_count);
                            audioNodesL[t].MinecraftFrequencyPerTick.Add(Fre[0] / 2);
                            audioNodesL[t].MinecraftVolumePerTick.Add((int)((Peak[1] - Peak[0]) * 1000));
                            audioNodesR[t].MinecraftFrequencyPerTick.Add(Fre[1] / 2);
                            audioNodesR[t].MinecraftVolumePerTick.Add((int)((Peak[3] - Peak[2]) * 1000));
                        }
                        if (audioNodesL[t].MinecraftFrequencyPerTick.Count < fre_count || audioNodesR[t].MinecraftFrequencyPerTick.Count < fre_count)
                            for (int j = 1; j <= fre_count; j++)
                            {
                                audioNodesL[t].MinecraftFrequencyPerTick.Add(audioNodesL[t].MinecraftFrequencyPerTick[0]);
                                audioNodesR[t].MinecraftFrequencyPerTick.Add(audioNodesR[t].MinecraftFrequencyPerTick[0]);
                            }
                        if (audioNodesL[t].MinecraftFrequencyPerTick.Count < vol_count || audioNodesR[t].MinecraftFrequencyPerTick.Count < vol_count)
                            for (int j = 1; j <= vol_count; j++)
                            {
                                audioNodesL[t].MinecraftVolumePerTick.Add(audioNodesL[t].MinecraftVolumePerTick[0]);
                                audioNodesR[t].MinecraftVolumePerTick.Add(audioNodesR[t].MinecraftVolumePerTick[0]);
                            }*/
                    }
                }
                /*Dynamicly creat and set lenth for TickNodes*/
                if (timeLine.TickNodes == null) timeLine.TickNodes = new List<TickNode>();
                if (totalTick >= timeLine.TickNodes.Count)
                {
                    var nowCount = timeLine.TickNodes.Count;
                    for (int i = 0; i < totalTick - nowCount + 1; i++) timeLine.TickNodes.Add(new TickNode());
                }
                /*Write MidiNodes into TickNodes.AudioNodes*/
                foreach (AudioNode a in audioNodesL)
                {
                    if (timeLine.TickNodes[a.MinecraftStartTick].AudioNodesLeft == null) timeLine.TickNodes[a.MinecraftStartTick].AudioNodesLeft = new List<AudioNode>();
                    timeLine.TickNodes[a.MinecraftStartTick].AudioNodesLeft.Add(new AudioNode
                    {
                        MinecraftFrequencyPerTick = a.MinecraftFrequencyPerTick,
                        MinecraftVolumePerTick = a.MinecraftVolumePerTick,
                        MinecraftStartTick = a.MinecraftStartTick
                    });
                }
                foreach (AudioNode a in audioNodesR)
                {
                    if (timeLine.TickNodes[a.MinecraftStartTick].AudioNodesRight == null) timeLine.TickNodes[a.MinecraftStartTick].AudioNodesRight = new List<AudioNode>();
                    timeLine.TickNodes[a.MinecraftStartTick].AudioNodesRight.Add(new AudioNode
                    {
                        MinecraftFrequencyPerTick = a.MinecraftFrequencyPerTick,
                        MinecraftVolumePerTick = a.MinecraftVolumePerTick,
                        MinecraftStartTick = a.MinecraftStartTick
                    });
                }
                #endregion
                return timeLine;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
        }
    }

    class AudioStreamOhterAudio
    {
        public TimeLine Serialize(string fileName, TimeLine timeLine)
        {
            var reader = new Mp3FileReader(fileName);


            return timeLine;
        }
    }
}
