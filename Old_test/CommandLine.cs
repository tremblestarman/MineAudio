using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audio2MinecraftScore
{
    public class CommandLine
    {
        public List<string> General = new List<string>();
        public List<string> End = new List<string>();
        public List<Command> Keyframe = new List<Command>();
    }
    public class Command
    {
        public List<string> Commands = new List<string>() { "$setblock", "setblock ~ ~-2 ~ minecraft:air", "spawnpoint @p ~ ~ ~", "tp @e[tag=Tracks] @p"};
    }
}
