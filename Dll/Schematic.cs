using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using fNbt;

namespace Audio2Minecraft
{
    public class Schematic
    {
        public void ExportSchematic(CommandLine commandLine, ExportSetting SettingParam, string ExportPath = "C:\\MyAudioRiptide.schematic")
        {
            try
            {
                var Schematic = Serialize(commandLine, SettingParam);
                //Export Schematic
                if (SettingParam.Type == ExportSetting.ExportType.Universal)
                    new NbtFile(Schematic).SaveToFile(ExportPath, NbtCompression.None);
                else if (SettingParam.Type == ExportSetting.ExportType.WorldEdit)
                    new NbtFile(Schematic).SaveToFile(ExportPath, NbtCompression.GZip);
            }
            catch {}
        }
        public NbtCompound Serialize(CommandLine commandLine, ExportSetting SettingParam)
        {
            try
            {
                var Schematic = new NbtCompound("Schematic");
                //CommandLine -> BlockInfo
                var blockInfo = CommandLine2SchematicInfo(commandLine, SettingParam);
                //BlockInfo -> Schematic
                Schematic.Add(blockInfo.Height);
                Schematic.Add(blockInfo.Length);
                Schematic.Add(blockInfo.Width);
                Schematic.Add(new NbtList("Entities", NbtTagType.Compound));
                Schematic.Add(blockInfo.Data);
                Schematic.Add(blockInfo.Blocks);
                Schematic.Add(blockInfo.TileEntities);
                if (SettingParam.Type == ExportSetting.ExportType.WorldEdit)
                {
                    var weInfo = BlockInfo2WorldEditBlockInfo(blockInfo, SettingParam.Direction);
                    Schematic.Add(weInfo.Materials);
                    Schematic.Add(weInfo.WEOriginX);
                    Schematic.Add(weInfo.WEOriginY);
                    Schematic.Add(weInfo.WEOriginZ);
                    Schematic.Add(weInfo.WEOffsetX);
                    Schematic.Add(weInfo.WEOffsetY);
                    Schematic.Add(weInfo.WEOffsetZ);
                }
                return Schematic;
            }
            catch
            {
                return null;
            }
        }
        private BlockInfo CommandLine2SchematicInfo(CommandLine commandLine, ExportSetting SettingParam)
        {
            try
            {
                var blockInfo = new BlockInfo();
                var count = commandLine.Keyframe.Count + 4;
                /* Define the region size */
                var n = SettingParam.Width; var l = (count % n == 0) ? count / n : count / n + 1; var h = 0;
                /* Define a block position , data */
                var x = 0; var y = 0; var z = 0; var r = 0;//0: down, 1: up, 2: north, 3: south, 4: west, 5: east,
                #region General & End (impulse)
                var general = new Command();
                var end = new Command();
                var dot = new Command();
                dot.Commands = new List<string>();
                //general
                general.Commands = commandLine.Start;
                commandLine.Keyframe.Insert(0, general);
                commandLine.Keyframe.Insert(1, dot);
                //end
                end.Commands = commandLine.End;
                commandLine.Keyframe.Add(dot);
                commandLine.Keyframe.Add(end);
                #endregion
                #region KeyFrames (line)
                /* Get Y_max */
                for (int i = 0; i < count; i++)
                {
                    if (commandLine.Keyframe[i].Commands.Count > h) h = commandLine.Keyframe[i].Commands.Count;
                }
                /* Create Arrays for block storing */
                var blocks = new byte[0]; var datas = new byte[0];
                if (SettingParam.Direction == 0 || SettingParam.Direction == 1)
                {
                    blockInfo.Height.Value = (short)h; blockInfo.Length.Value = (short)n; blockInfo.Width.Value = (short)l;
                    //blocks = new byte[((h - 1) * l + (n - 1)) * n + (l - 1) + 1]; datas = new byte[((h - 1) * l + (n - 1)) * n + (l - 1) + 1];
                }
                if (SettingParam.Direction == 2 || SettingParam.Direction == 3)
                {
                    blockInfo.Height.Value = (short)h; blockInfo.Length.Value = (short)l; blockInfo.Width.Value = (short)n;
                    //blocks = new byte[((h - 1) * n + (l - 1)) * l + (n - 1) + 1]; datas = new byte[((h - 1) * n + (l - 1)) * l + (n - 1) + 1];
                }
                blocks = new byte[h * n * l]; datas = new byte[h * n * l];
                /* Write in position */
                for (int i = 0; i < count; i++)
                {
                    #region get (X & Z & Rotation) -> x y r
                    //X+
                    if (SettingParam.Direction == 0)
                    {
                        x = i / n;
                        z = i % n;
                        if (x % 2 == 0) { if (z == n - 1) r = 5;/*x++*/ else r = 3;/*z++*/ }
                        else { z = n - z - 1; if (z == 0) r = 5; /*x++*/ else r = 2;/*z--*/ }
                    }
                    //X-
                    if (SettingParam.Direction == 1)
                    {
                        x = i / n;
                        z = i % n;
                        if (x % 2 == 0) { if (z == n - 1) r = 4;/*x--*/ else r = 3;/*z++*/ }
                        else { z = n - z - 1; if (z == 0) r = 4; /*x--*/ else r = 2;/*z--*/ }
                        x = l - x - 1;
                    }
                    //Z+
                    if (SettingParam.Direction == 2)
                    {
                        x = i % n;
                        z = i / n;
                        if (z % 2 == 0) { if (x == n - 1) r = 3;/*z++*/ else r = 5;/*x++*/ }
                        else { x = n - x - 1; if (x == 0) r = 3; /*z++*/ else r = 4;/*x--*/ }
                    }
                    //Z-
                    if (SettingParam.Direction == 3)
                    {
                        x = i % n;
                        z = i / n;
                        if (z % 2 == 0) { if (x == n - 1) r = 2;/*z--*/ else r = 5;/*x++*/ }
                        else { x = n - x - 1; if (x == 0) r = 2; /*z--*/ else r = 4;/*x--*/ }
                        z = l - z - 1;
                    }
                    #endregion
                    var vector2 = new int[] { 0, 0 };
                    #region Rotation
                    switch (r)
                    {
                        case 2: vector2[0] = 0; vector2[1] = -1; break;
                        case 3: vector2[0] = 0; vector2[1] = 1; break;
                        case 4: vector2[0] = -1; vector2[1] = 0; break;
                        case 5: vector2[0] = 1; vector2[1] = 0; break;
                    }
                    #endregion
                    //Options about command
                    if (!SettingParam.AlwaysActive) commandLine.Keyframe[i].Commands.Remove("spawnpoint @p ~ ~ ~");
                    if (!SettingParam.AlwaysLoadEntities) commandLine.Keyframe[i].Commands.Remove("tp @e[tag=Tracks] @p");
                    //WriteIn Commands
                    for (y = 0; y < commandLine.Keyframe[i].Commands.Count; y++)
                    {
                        //X+ X-
                        if (SettingParam.Direction == 0 || SettingParam.Direction == 1)
                        {
                            var index = (y * n + z) * l + x;
                            if (y == 0)
                            {
                                blocks[index] = 137;
                                datas[index] = 1;
                                if (commandLine.Keyframe[i].Commands[0] == "$setblock") blockInfo.TileEntities.Add(AddCommand("setblock ~" + vector2[0].ToString() + " ~-1 ~" + vector2[1].ToString() + " minecraft:redstone_block", x, y, z, false));
                                else blockInfo.TileEntities.Add(AddCommand(commandLine.Keyframe[i].Commands[y], x, y, z, false));
                            }
                            else
                            {
                                blocks[index] = 211;
                                datas[index] = 1;
                                blockInfo.TileEntities.Add(AddCommand(commandLine.Keyframe[i].Commands[y], x, y, z, true));
                            }
                        }
                        //Z+ Z-
                        else if (SettingParam.Direction == 2 || SettingParam.Direction == 3)
                        {
                            var index = (y * l + z) * n + x;
                            if (y == 0 && commandLine.Keyframe[i].Commands[0] == "$setblock")
                            {
                                blocks[index] = 137;
                                datas[index] = 1;
                                if (commandLine.Keyframe[i].Commands[0] == "$setblock") blockInfo.TileEntities.Add(AddCommand("setblock ~" + vector2[0].ToString() + " ~-1 ~" + vector2[1].ToString() + " minecraft:redstone_block", x, y, z, false));
                                else blockInfo.TileEntities.Add(AddCommand(commandLine.Keyframe[i].Commands[y], x, y, z, false));
                            }
                            else
                            {
                                blocks[index] = 211;
                                datas[index] = 1;
                                blockInfo.TileEntities.Add(AddCommand(commandLine.Keyframe[i].Commands[y], x, y, z, true));
                            }
                        }
                    }
                }
                blockInfo.Blocks.Value = blocks;
                blockInfo.Data.Value = datas;
                #endregion
                return blockInfo;
            }
            catch
            {
                return null;
            }
        }
        private NbtCompound AddCommand(string command, int x, int y, int z, bool auto)
        {
            var NodePoint = new NbtCompound();
            NodePoint.Add(new NbtString("id", "minecraft:command_block"));
            NodePoint.Add(new NbtString("Command", command));
            NodePoint.Add(new NbtByte("TrackOutput", 1));
            NodePoint.Add(new NbtString("CustomName", "@"));
            NodePoint.Add(new NbtInt("SuccessCount", 0));
            NodePoint.Add(new NbtByte("auto", auto ? (byte)1 : (byte)0));
            NodePoint.Add(new NbtByte("powered", 0));
            NodePoint.Add(new NbtByte("conditionMet", 0));
            NodePoint.Add(new NbtByte("UpdateLastExecution", 1));
            NodePoint.Add(new NbtInt("x", x));
            NodePoint.Add(new NbtInt("y", y));
            NodePoint.Add(new NbtInt("z", z));
            return NodePoint;
        }
        private WorldEditBlockInfo BlockInfo2WorldEditBlockInfo(BlockInfo blockInfo, int direction)
        {
            var weInfo = new WorldEditBlockInfo();
            weInfo.Blocks = blockInfo.Blocks;
            weInfo.Data = blockInfo.Data;
            weInfo.Height = blockInfo.Height;
            weInfo.Length = blockInfo.Length;
            weInfo.TileEntities = blockInfo.TileEntities;
            weInfo.Width = blockInfo.Height;

            switch (direction)
            {
                case 0:
                    weInfo.WEOffsetZ.Value = 1;
                    break;
                case 1:
                    weInfo.WEOffsetX.Value = weInfo.Length.IntValue - 1;
                    weInfo.WEOffsetZ.Value = weInfo.Width.IntValue - 2;
                    break;
                case 2:
                    weInfo.WEOffsetZ.Value = -1;
                    break;
                case 3:
                    weInfo.WEOffsetX.Value = -(weInfo.Length.IntValue - 2);
                    weInfo.WEOffsetZ.Value = weInfo.Width.IntValue - 1;
                    break;
            } 
            return weInfo;
        }
    }
    public class ExportSetting
    {
        private int _dir = 0;
        /// <summary>
        /// Which Direction the CommandLine Extends.
        /// X+:0, X-:1, Z+:2, Z-:3
        /// </summary>
        public int Direction { get { return _dir; } set { _dir = value; } }//Xf = 0, Xb = 1, Zf = 2, Zb = 3 
        private int _width = 64;
        /// <summary>
        /// the Width of the CommandLine
        /// </summary>
        public int Width { get { return _width; } set { _width = value; } }
        private bool _AlwaysActive = true;
        /// <summary>
        /// Whether the Chunks Always Loaded
        /// </summary>
        public bool AlwaysActive { get { return _AlwaysActive; } set { _AlwaysActive = value; } }
        private bool _AlwaysLoadEntities = true;
        /// <summary>
        /// Whether the Entities Always Loaded
        /// </summary>
        public bool AlwaysLoadEntities { get { return _AlwaysLoadEntities; } set { _AlwaysLoadEntities = value; } }
        private ExportType _exportType = ExportType.Universal;
        public ExportType Type { get { return _exportType; } set { _exportType = value; } }
        public enum ExportType
        {
            Universal,
            WorldEdit
        }
    }

    class BlockInfo
    {
        public NbtByteArray Blocks = new NbtByteArray("Blocks");
        public NbtByteArray Data = new NbtByteArray("Data");
        public NbtList TileEntities = new NbtList("TileEntities");
        public NbtShort Height = new NbtShort("Height", 0);
        public NbtShort Length = new NbtShort("Length", 0);
        public NbtShort Width = new NbtShort("Width", 0);
    }
    class WorldEditBlockInfo : BlockInfo
    {
        public NbtInt WEOriginX = new NbtInt("WEOriginX", 0);
        public NbtInt WEOriginY = new NbtInt("WEOriginY", 0);
        public NbtInt WEOriginZ = new NbtInt("WEOriginZ", 0);
        public NbtInt WEOffsetX = new NbtInt("WEOffsetX", 0);
        public NbtInt WEOffsetY = new NbtInt("WEOffsetY", 0);
        public NbtInt WEOffsetZ = new NbtInt("WEOffsetZ", 0);
        public NbtString Materials = new NbtString("Materials", "Alpha");
    }
}
