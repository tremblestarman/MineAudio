using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Audio2Minecraft
{
    public class Lrc
    {
        public List<LrcNode> Lrcs = new List<LrcNode>();
        public Lrc Serialize(string filePath = @"c:\lyrics.lrc")
        {
            var lr = new List<string>(System.IO.File.ReadAllLines(filePath));
            lr.RemoveAll(l => l == null || l == Environment.NewLine || l == "");
            for (int i = 0; i < lr.Count(); i++)
            {
                var _l = new LrcNode();
                var time = Regex.Match(lr[i], @"(?<=\[).*(?=\])").Value;
                _l.Content = Regex.Match(lr[i], @"(?<=\]).*(?=$)").Value; //Get Content
                var minute = Regex.Match(time, @"(?<=^).*(?=:)").Value.TrimStart('0'); if (minute == "") minute = "0";
                var second = Regex.Match(time, @"(?<=:).*(?=$)").Value.TrimStart('0'); if (second == "") second = "0";
                if (!Regex.Match(minute, @"^\d+$").Success || !Regex.Match(second, @"^(\d+(.\d*)?)$").Success) continue;
                
                try //Set Start Tick
                {
                    var m = double.Parse(minute);
                    var s = double.Parse(second);
                    _l.Start = (int)(m * 1200 + s * 20);
                }
                catch (Exception e) { MessageBox.Show(e.ToString()); }

                if (Lrcs.Count > 0) Lrcs[Lrcs.Count - 1].Duration = _l.Start - Lrcs[Lrcs.Count - 1].Start; //Get Duration Ticks

                Lrcs.Add(_l);
            }
            return this;
        }
    }
    public class LrcNode
    {
        public string Content { get; set; }
        public int Start { get; set; }

        public int Duration = -1;
    }


    public class AMLrc
    {
        public List<AMLrcContent> Bars = new List<AMLrcContent>();
        public CommandLine AMLrcLine = new CommandLine();
        public AMLrc Serialize(string filePath = @"c:\lyrics.txt")
        {
            #region Bars
            var bar = new List<AMLrcContent>();
            string lr = System.IO.File.ReadAllText(filePath);
            var pr = new Regex("(?<=^|\r\n\r\n)((?!\r\n\r\n)[\\s\\S])*(?=$|\r\n\r\n)");
            var ps = pr.Matches(lr);
            var maxtime = -1;
            //String -> Bars
            for (int t = 0; t < ps.Count; t++)
            {
                bar.Add(new AMLrcContent());
                //Get Bar
                var tr = new Regex("(?<=^|\r\n).*(?=$|\n)");
                var title = ""; var subtitle = "";
                if (tr.Matches(ps[t].Value).Count>0)
                    title = tr.Matches(ps[t].Value)[0].Value;
                if (tr.Matches(ps[t].Value).Count > 1)
                    subtitle = tr.Matches(ps[t].Value)[1].Value;
                //Get Content & ApperTime
                var regex_param = new Regex("(?<=^|;)((?!;{1}).)*-\\d+(?=$|;)");
                var title_l = regex_param.Matches(title);
                var subtitle_l = regex_param.Matches(subtitle);
                for (int g = 0; g < title_l.Count; g++)
                {
                    if (bar[t].Main.Count <= g)
                        bar[t].Main.Add(new AMLrcNode());
                    if (new Regex("(?<=^).*(?=-\\d+)").Matches(title_l[g].Value).Count > 0)
                        bar[t].Main[g].Content = new Regex("(?<=^).*(?=-\\d+)").Matches(title_l[g].Value)[0].Value;
                    if (new Regex("(?<=-)\\d+(?=$)").Matches(title_l[g].Value).Count > 0)
                        bar[t].Main[g].AppearTime = Int32.Parse(new Regex("(?<=-)\\d+(?=$)").Matches(title_l[g].Value)[0].Value);
                    if (bar[t].Main[g].AppearTime > maxtime) maxtime = bar[t].Main[g].AppearTime;
                }
                for (int g = 0; g < subtitle_l.Count; g++)
                {
                    if (bar[t].Sub.Count <= g)
                        bar[t].Sub.Add(new AMLrcNode());
                    if (new Regex("(?<=^).*(?=-\\d+)").Matches(subtitle_l[g].Value).Count > 0)
                        bar[t].Sub[g].Content = new Regex("(?<=^).*(?=-\\d+)").Matches(subtitle_l[g].Value)[0].Value;
                    if (new Regex("(?<=-)\\d+(?=$)").Matches(subtitle_l[g].Value).Count > 0)
                        bar[t].Sub[g].AppearTime = Int32.Parse(new Regex("(?<=-)\\d+(?=$)").Matches(subtitle_l[g].Value)[0].Value);
                    if (bar[t].Sub[g].AppearTime > maxtime) maxtime = bar[t].Sub[g].AppearTime;
                }
            }
            #endregion
            #region AMLrcLine
            var lyricsLine = new CommandLine();
            for (int i = 0; i < maxtime + 1; i++)
            {
                lyricsLine.Keyframe.Add(new Command() { Commands = new List<string>()});
            }
            for(int j = 0; j < bar.Count; j++)
            {
                var b = bar[j];
                b.Main.OrderBy(i => i.AppearTime);
                b.Sub.OrderBy(i => i.AppearTime);
                var main = "title @a title [";
                var sub = "title @a subtitle [";
                for (int i = 0; i < b.Main.Count; i++)
                {
                    main = main + "\"" + b.Main[i].Content + "\"" + ",";
                    lyricsLine.Keyframe[b.Main[i].AppearTime] = new Command() { Commands = new List<string>() { main.TrimEnd(',') + "]" } };
                }
                for (int i = 0; i < b.Sub.Count; i++)
                {
                    sub = sub + "\"" + b.Sub[i].Content + "\"" + ",";
                    lyricsLine.Keyframe[b.Sub[i].AppearTime] = new Command() { Commands = new List<string>() { sub.TrimEnd(',') + "]" } };
                }
                if (b.Main.Count > 0)
                    lyricsLine.Keyframe[b.Main[0].AppearTime].Commands.Insert(0, "title @a title [\"\"]");
                if (b.Sub.Count > 0)
                    lyricsLine.Keyframe[b.Sub[0].AppearTime].Commands.Insert(0, "title @a subtitle [\"\"]");
                bar[j] = b;
            }
            this.Bars = bar;
            this.AMLrcLine = lyricsLine;
            #endregion
            return this;
        }
    }
    public class AMLrcContent
    {
        public List<AMLrcNode> Main = new List<AMLrcNode>();
        public List<AMLrcNode> Sub = new List<AMLrcNode>();
    }
    public class AMLrcNode
    {
        public string Content { get; set; }
        public int AppearTime { get; set; }
    }
}