using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Audio2Minecraft
{
    /// <summary>
    /// 自动补全
    /// </summary>
    public class AutoFill
    {
        /// <summary>
        /// 自动补全规则
        /// </summary>
        public AutoFillRule Rule;
        /// <summary>
        /// 规则名称
        /// </summary>
        public string RuleName;
        /// <summary>
        /// 规则路径
        /// </summary>
        public string RulePath;
        /// <summary>
        /// 通过配置文件生成自动补全
        /// </summary>
        /// <param name="configPath">配置文件路径</param>
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
    /// <summary>
    /// 自动补全规则
    /// </summary>
    public class AutoFillRule
    {
        /// <summary>
        /// 匹配列表
        /// </summary>
        public Dictionary<string, AutoFillMatch> matches = new Dictionary<string, AutoFillMatch>();
        /// <summary>
        /// 模式列表
        /// </summary>
        public Dictionary<string, AutoFillMode> modes = new Dictionary<string, AutoFillMode>();
    }
    /// <summary>
    /// 自动补全匹配
    /// </summary>
    public class AutoFillMatch
    {
        /// <summary>
        /// 乐器
        /// </summary>
        public string instrument = "";
        /// <summary>
        /// 音色名
        /// </summary>
        public string sound_name = "";
        /// <summary>
        /// 子表达式
        /// </summary>
        public string expression = "";
        /// <summary>
        /// 音量
        /// </summary>
        public double volume = 100;
        /// <summary>
        /// 播放目标
        /// </summary>
        public string target = "@a";
        /// <summary>
        /// 播放源
        /// </summary>
        public string source = "record";
        /// <summary>
        /// stopsound相关设置
        /// </summary>
        public StopSound stopsound = new StopSound();
        public class StopSound
        {
            /// <summary>
            /// 启用stopsound
            /// </summary>
            public bool enable = false;
            /// <summary>
            /// 额外延时
            /// </summary>
            public int extra_delay = -1;
        }
        /// <summary>
        /// 相对玩家
        /// </summary>
        public string excute_target = "@a";
        /// <summary>
        /// 相对坐标
        /// </summary>
        public ExecutePos excute_pos = new ExecutePos();
        public class ExecutePos
        {
            public double x = 0.0;
            public double y = 0.0;
            public double z = 0.0;
        }
        /// <summary>
        /// 音高播放
        /// </summary>
        public bool pitch_playable;
    }
    /// <summary>
    /// 自动补全模式
    /// </summary>
    public class AutoFillMode
    {
        /// <summary>
        /// 模式简介
        /// </summary>
        public string description = "";
        /// <summary>
        /// 自动补全
        /// </summary>
        public bool auto = false;
        /// <summary>
        /// 模式下的匹配
        /// </summary>
        public List<string> matches = new List<string>();
        [JsonIgnore]
        public Dictionary<string, AutoFillMatch> _matches = new Dictionary<string, AutoFillMatch>();
    }
}
