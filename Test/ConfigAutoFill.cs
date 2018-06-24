using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Audio2Minecraft
{
    public class AutoFill
    {
        public AutoFillRule Rule;
        public string RuleName;
        public string RulePath;
        public AutoFill(string configPath)
        {
            try { Rule = JsonConvert.DeserializeObject<AutoFillRule>(File.ReadAllText(configPath)); }
            catch (Exception e) { throw e; }
            RuleName = new FileInfo(configPath).Name;
            RulePath = configPath;
            foreach (var m in Rule.modes.Keys)
            {
                if (Rule.modes[m].matches == null)
                {
                    Rule.modes[m]._matches = Rule.matches;
                }
                else
                {
                    foreach (var _m in Rule.modes[m].matches)
                    {
                        Rule.modes[m]._matches.Add(_m, Rule.matches[_m]);
                    }
                }
            }
        }
    }

    public class AutoFillRule
    {
        public Dictionary<string, AutoFillMatch> matches = new Dictionary<string, AutoFillMatch>();
        public Dictionary<string, AutoFillMode> modes = new Dictionary<string, AutoFillMode>();
    }

    public class AutoFillMatch
    {
        public string instrument = "";
        public string expression = "";
        public double volume = 100;
        public string target = "@a";
        public string source = "record";
        public StopSound stopsound = new StopSound();
        public class StopSound
        {
            public bool enable = false;
            public int extra_delay = 1;
        }
        public string excute_target = "@a";
        public ExecutePos excute_pos = new ExecutePos();
        public class ExecutePos
        {
            public double x = 0.0;
            public double y = 0.0;
            public double z = 0.0;
        }
    }
    public class AutoFillMode
    {
        public string description = "";
        public bool auto = false;
        public List<string> matches = new List<string>();
        [JsonIgnore]
        public Dictionary<string, AutoFillMatch> _matches = new Dictionary<string, AutoFillMatch>();
    }
}
