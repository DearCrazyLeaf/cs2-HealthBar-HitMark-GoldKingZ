using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Text;
using System.Collections.Generic;

namespace HealthBar_HitMark_GoldKingZ.Config
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RangeAttribute : Attribute
    {
        public int Min { get; }
        public int Max { get; }
        public int Default { get; }
        public string Message { get; }

        public RangeAttribute(int min, int max, int defaultValue, string message)
        {
            Min = min;
            Max = max;
            Default = defaultValue;
            Message = message;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CommentAttribute : Attribute
    {
        public string Comment { get; }

        public CommentAttribute(string comment)
        {
            Comment = comment;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class BreakLineAttribute : Attribute
    {
        public string BreakLine { get; }

        public BreakLineAttribute(string breakLine)
        {
            BreakLine = breakLine;
        }
    }
    public static class Configs
    {
        public static class Shared {
            public static string? CookiesModule { get; set; }
            public static IStringLocalizer? StringLocalizer { get; set; }
        }
        
        private static readonly string ConfigDirectoryName = "config";
        private static readonly string ConfigFileName = "config.json";
        private static readonly string jsonFilePath2 = "ServerPrecacheResources.txt";
        private static string? _jsonFilePath2;
        private static string? _configFilePath;
        private static ConfigData? _configData;

        private static readonly JsonSerializerOptions SerializationOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter()
            },
            WriteIndented = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        public static bool IsLoaded()
        {
            return _configData is not null;
        }

        public static ConfigData GetConfigData()
        {
            if (_configData is null)
            {
                throw new Exception("Config not yet loaded.");
            }
            
            return _configData;
        }

        public static ConfigData Load(string modulePath)
        {
            var configFileDirectory = Path.Combine(modulePath, ConfigDirectoryName);
            if(!Directory.Exists(configFileDirectory))
            {
                Directory.CreateDirectory(configFileDirectory);
            }
            _jsonFilePath2 = Path.Combine(configFileDirectory, jsonFilePath2);
            Helper.CreateResource(_jsonFilePath2);

            _configFilePath = Path.Combine(configFileDirectory, ConfigFileName);
            if (File.Exists(_configFilePath))
            {
                _configData = JsonSerializer.Deserialize<ConfigData>(File.ReadAllText(_configFilePath), SerializationOptions);
                _configData!.Validate();
            }
            else
            {
                _configData = new ConfigData();
                _configData.Validate();
            }

            if (_configData is null)
            {
                throw new Exception("Failed to load configs.");
            }

            SaveConfigData(_configData);
            
            return _configData;
        }

        private static void SaveConfigData(ConfigData configData)
{
    if (_configFilePath is null)
        throw new Exception("Config not yet loaded.");

    string json = JsonSerializer.Serialize(configData, SerializationOptions);
    json = Regex.Unescape(json);

    var lines = json.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
    var newLines = new List<string>();

    foreach (var line in lines)
    {
        var match = Regex.Match(line, @"^\s*""(\w+)""\s*:.*");
        bool isPropertyLine = false;
        PropertyInfo? propInfo = null;

        if (match.Success)
        {
            string propName = match.Groups[1].Value;
            propInfo = typeof(ConfigData).GetProperty(propName);

            var breakLineAttr = propInfo?.GetCustomAttribute<BreakLineAttribute>();
            if (breakLineAttr != null)
            {
                string breakLine = breakLineAttr.BreakLine;

                if (breakLine.Contains("{space}"))
                {
                    breakLine = breakLine.Replace("{space}", "").Trim();

                    if (breakLineAttr.BreakLine.StartsWith("{space}"))
                    {
                        newLines.Add("");
                    }

                    newLines.Add("// " + breakLine);
                    newLines.Add("");
                }
                else
                {
                    newLines.Add("// " + breakLine);
                }
            }

            var commentAttr = propInfo?.GetCustomAttribute<CommentAttribute>();
            if (commentAttr != null)
            {
                var commentLines = commentAttr.Comment.Split('\n');
                foreach (var commentLine in commentLines)
                {
                    newLines.Add("// " + commentLine.Trim());
                }
            }

            isPropertyLine = true;
        }

        newLines.Add(line);

        // Add empty line after the property line
        if (isPropertyLine && propInfo?.GetCustomAttribute<CommentAttribute>() != null)
        {
            newLines.Add("");
        }
    }

    // Second pass: Add empty line after closing ] of arrays
    var adjustedLines = new List<string>();
    foreach (var line in newLines)
    {
        adjustedLines.Add(line);
        if (Regex.IsMatch(line, @"^\s*\],?\s*$"))
        {
            adjustedLines.Add("");
        }
    }

    File.WriteAllText(_configFilePath, string.Join(Environment.NewLine, adjustedLines), Encoding.UTF8);
}

        public class ConfigData
        {
            private string? _Version;
            [BreakLine("----------------------------[ ↓ Plugin Info ↓ ]----------------------------{space}")]
            public string Version
            {
                get => _Version!;
                set
                {
                    _Version = value;
                    if (_Version != HealthBarHitMarkGoldKingZ.Instance.ModuleVersion)
                    {
                        Version = HealthBarHitMarkGoldKingZ.Instance.ModuleVersion;
                    }
                }
            }

            [BreakLine("{space}----------------------------[ ↓ HitMark Config ↓ ]----------------------------{space}")]
            [Comment("Enable HitMark?\ntrue = Yes\nfalse = No")]
            public bool HM_EnableHitMark { get; set; }

            [Comment("Disable HitMark On WarmUp?\ntrue = Yes\nfalse = No")]
            public bool HM_DisableOnWarmUp { get; set; }

            [Comment("Mute Default HeadShot And BodyShot Only On If There Is Custom Sounds?\ntrue = Yes\nfalse = No")]
            public bool HM_MuteDefaultHeadShotBodyShot { get; set; }

            [Comment("HeadShot icon color (#RRGGBB or color name)")]
            public string HM_HeadShotColor { get; set; }

            [Comment("HeadShot icon display duration (seconds)")]
            public float HM_HeadShotDuration { get; set; }

            [Comment("BodyShot icon color (#RRGGBB or color name)")]
            public string HM_BodyShotColor { get; set; }

            [Comment("BodyShot icon display duration (seconds)")]
            public float HM_BodyShotDuration { get; set; }

            [Comment("Font name used for HitMark HUD text")]
            public string HM_FontName { get; set; }

            [Comment("World units per pixel for GameHUD text (smaller = sharper)")]
            public float HM_FontUnits { get; set; }

            [Comment("Hit character that appears on damage (e.g. ✚)")]
            public string HM_HitChar { get; set; }

            [Comment("Hit character starting font size")]
            public float HM_HitScaleStart { get; set; }

            [Comment("Hit character target font size")]
            public float HM_HitScaleEnd { get; set; }

            [Comment("Hit character scale animation duration (seconds)")]
            public float HM_HitScaleDuration { get; set; }

            [Comment("Hit character scale animation steps (1-30)")]
            [Range(1, 30, 5, "[HitMark] HM_HitScaleSteps invalid. Using 5 (1-30)")]
            public int HM_HitScaleSteps { get; set; }

            [Comment("Show damage value next to icon? true = Yes / false = No")]
            public bool HM_ShowDamageValue { get; set; }

            [Comment("Damage number color (#RRGGBB or color name)")]
            public string HM_DamageColor { get; set; }

            [Comment("Damage number color on headshot (#RRGGBB or color name)")]
            public string HM_DamageHeadShotColor { get; set; }

            [Comment("Damage number font size (0 hides the value)")]
            [Range(0, 72, 20, "[HitMark] HM_DamageFontSize invalid. Using 20 (0-72)")]
            public int HM_DamageFontSize { get; set; }

            [Comment("Damage number display duration (seconds)")]
            public float HM_DamageDuration { get; set; }

            [Comment("Damage number horizontal offset relative to icon (-30 to 30 recommended)")]
            public float HM_DamageOffsetX { get; set; }

            [Comment("Damage number vertical offset relative to icon (-15 to 15 recommended)")]
            public float HM_DamageOffsetY { get; set; }

            [Comment("Damage motion mode: 0 = Static, 1 = Random bounce")]
            public int HM_DamageMotionMode { get; set; }

            [Comment("Random horizontal jitter range when motion enabled")]
            public float HM_DamageJitterRangeX { get; set; }

            [Comment("Random vertical jitter range when motion enabled")]
            public float HM_DamageJitterRangeY { get; set; }

            [Comment("Peak height added during bounce (motion mode 1)")]
            public float HM_DamageBounceHeight { get; set; }

            [Comment("Bounce duration in seconds (motion mode 1)")]
            public float HM_DamageBounceDuration { get; set; }

            [Comment("Horizontal HUD offset relative to view origin (-50 to 50 recommended)")]
            public float HM_HudOffsetX { get; set; }

            [Comment("Vertical HUD offset relative to view origin (-10 to 10 recommended)")]
            public float HM_HudOffsetY { get; set; }

            [Comment("Distance from player view to HUD text (40 to 70 recommended)")]
            public float HM_HudDistance { get; set; }

            [Comment("Set up HeadShot sounds per player/group. Format: <PlayerFlag or Group or SteamID or ANY>|<SoundPath>")]
            public List<string> HM_HeadShotSounds { get; set; }

            [Comment("Set up BodyShot sounds per player/group. Format: <PlayerFlag or Group or SteamID or ANY>|<SoundPath>")]
            public List<string> HM_BodyShotSounds { get; set; }

            [Comment("Enable Debug Plugin In Server Console (Helps You To Debug Issues You Facing)?\ntrue = Yes\nfalse = No")]
            [BreakLine("{space}----------------------------[ ↓ Utilities  ↓ ]----------------------------{space}")]
            public bool EnableDebug { get; set; }
            
            public ConfigData()
            {
                Version = HealthBarHitMarkGoldKingZ.Instance.ModuleVersion;
                HM_EnableHitMark = true;
                HM_DisableOnWarmUp = false;
                HM_MuteDefaultHeadShotBodyShot = false;
                HM_HeadShotColor = "#FFFFFF";
                HM_HeadShotDuration = 0.5f;
                HM_BodyShotColor = "#FFFFFF";
                HM_BodyShotDuration = 0.45f;
                HM_FontName = "Bahnschrift";
                HM_FontUnits = 0.18f;
                HM_HitChar = "✚";
                HM_HitScaleStart = 10f;
                HM_HitScaleEnd = 18f;
                HM_HitScaleDuration = 0.25f;
                HM_HitScaleSteps = 5;
                HM_ShowDamageValue = true;
                HM_DamageColor = "#FFD966";
                HM_DamageHeadShotColor = "#FF4D4D";
                HM_DamageFontSize = 16;
                HM_DamageDuration = 0.4f;
                HM_DamageOffsetX = 3.5f;
                HM_DamageOffsetY = 1.5f;
                HM_DamageMotionMode = 1;
                HM_DamageJitterRangeX = 0.9f;
                HM_DamageJitterRangeY = 0.6f;
                HM_DamageBounceHeight = 1.8f;
                HM_DamageBounceDuration = 0.35f;
                HM_HudOffsetX = 0f;
                HM_HudOffsetY = 0f;
                HM_HudDistance = 42f;
                HM_HeadShotSounds = new List<string>
                {
                    "ANY | sounds/goldkingz/hitmark/headshot.vsnd",
                    "@css/vips,#css/vips | sounds/goldkingz/hitmark/headshot_2.vsnd"
                };
                HM_BodyShotSounds = new List<string>
                {
                    "ANY | sounds/goldkingz/hitmark/bodyhit.vsnd",
                    "@css/vips,#css/vips | sounds/goldkingz/hitmark/bodyhit_2.vsnd"
                };
                EnableDebug = false;
            }

            public void Validate()
            {
                foreach (var prop in GetType().GetProperties())
                {
                    var rangeAttr = prop.GetCustomAttribute<RangeAttribute>();
                    if (rangeAttr != null && prop.PropertyType == typeof(int))
                    {
                        int value = (int)prop.GetValue(this)!;
                        if (value < rangeAttr.Min || value > rangeAttr.Max)
                        {
                            prop.SetValue(this, rangeAttr.Default);
                            Helper.DebugMessage(rangeAttr.Message,false);
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(HM_HeadShotColor)) HM_HeadShotColor = "#FFFFFF";
                if (string.IsNullOrWhiteSpace(HM_BodyShotColor)) HM_BodyShotColor = "#FFFFFF";
                if (string.IsNullOrWhiteSpace(HM_FontName)) HM_FontName = "Bahnschrift";
                if (HM_FontUnits < 0.05f || HM_FontUnits > 1f) HM_FontUnits = 0.18f;
                if (string.IsNullOrWhiteSpace(HM_HitChar)) HM_HitChar = "✚";
                if (HM_HitScaleStart < 1f) HM_HitScaleStart = 1f;
                if (HM_HitScaleStart > 96f) HM_HitScaleStart = 96f;
                if (HM_HitScaleEnd < 1f) HM_HitScaleEnd = 1f;
                if (HM_HitScaleEnd > 96f) HM_HitScaleEnd = 96f;
                if (HM_HitScaleDuration <= 0f || HM_HitScaleDuration > 1.0f) HM_HitScaleDuration = 0.25f;
                if (string.IsNullOrWhiteSpace(HM_DamageColor)) HM_DamageColor = "#FFFFFF";
                if (string.IsNullOrWhiteSpace(HM_DamageHeadShotColor)) HM_DamageHeadShotColor = "#FF4D4D";
                if (HM_HeadShotDuration <= 0) HM_HeadShotDuration = 0.5f;
                if (HM_BodyShotDuration <= 0) HM_BodyShotDuration = 0.45f;
                if (HM_DamageDuration <= 0) HM_DamageDuration = 0.4f;
                if (HM_DamageMotionMode < 0 || HM_DamageMotionMode > 1) HM_DamageMotionMode = 1;
                if (HM_HudOffsetX < -50f || HM_HudOffsetX > 50f) HM_HudOffsetX = 0f;
                if (HM_HudOffsetY < -10f || HM_HudOffsetY > 10f) HM_HudOffsetY = 0f;
                if (HM_HudDistance < 35f || HM_HudDistance > 80f) HM_HudDistance = 42f;
                if (HM_DamageOffsetX < -30f || HM_DamageOffsetX > 30f) HM_DamageOffsetX = 3.5f;
                if (HM_DamageOffsetY < -15f || HM_DamageOffsetY > 15f) HM_DamageOffsetY = 1.5f;
                if (HM_DamageJitterRangeX < 0f || HM_DamageJitterRangeX > 5f) HM_DamageJitterRangeX = 0.9f;
                if (HM_DamageJitterRangeY < 0f || HM_DamageJitterRangeY > 5f) HM_DamageJitterRangeY = 0.6f;
                if (HM_DamageBounceHeight < 0f || HM_DamageBounceHeight > 10f) HM_DamageBounceHeight = 1.8f;
                if (HM_DamageBounceDuration <= 0f || HM_DamageBounceDuration > 1.0f) HM_DamageBounceDuration = 0.35f;
            }
        }
    }
}