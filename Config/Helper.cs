using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using System;
using System.Text.RegularExpressions;
using System.Text.Json;
using HealthBar_HitMark_GoldKingZ.Config;
using System.Text.Encodings.Web;
using CounterStrikeSharp.API.Core.Translations;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;
using CS2_GameHUDAPI;

namespace HealthBar_HitMark_GoldKingZ;

public class Helper
{
    private const byte HitmarkIconChannel = 10;
    private const byte HitmarkDamageChannel = 11;
    private static readonly Random DamageRandom = new();

    private sealed record DamageLayer(byte Channel, Color Color, float RelativeOffsetX, float RelativeOffsetY);

    public static void AdvancedPlayerPrintToChat(CCSPlayerController player, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    player.PrintToChat(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            player.PrintToChat(message);
        }
    }
    public static void AdvancedPlayerPrintToConsole(CCSPlayerController player, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    player.PrintToConsole(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            player.PrintToConsole(message);
        }
    }
    public static void AdvancedServerPrintToChatAll(string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    Server.PrintToChatAll(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            Server.PrintToChatAll(message);
        }
    }
    
    public static bool IsPlayerInGroupPermission(CCSPlayerController player, string groups)
    {
        if (string.IsNullOrEmpty(groups))
        {
            return false;
        }
        var Groups = groups.Split(',');
        foreach (var group in Groups)
        {
            if (string.IsNullOrEmpty(group))
            {
                continue;
            }
            string groupId = group[0] == '!' ? group.Substring(1) : group;
            if (group[0] == '#' && AdminManager.PlayerInGroup(player, group))
            {
                return true;
            }
            else if (group[0] == '@' && AdminManager.PlayerHasPermissions(player, group))
            {
                return true;
            }
            else if (group[0] == '!' && player.AuthorizedSteamID != null && (groupId == player.AuthorizedSteamID.SteamId2.ToString() || groupId == player.AuthorizedSteamID.SteamId3.ToString().Trim('[', ']') ||
            groupId == player.AuthorizedSteamID.SteamId32.ToString() || groupId == player.AuthorizedSteamID.SteamId64.ToString()))
            {
                return true;
            }
            else if (AdminManager.PlayerInGroup(player, group))
            {
                return true;
            }
        }
        return false;
    }
    public static List<CCSPlayerController> GetPlayersController(bool IncludeBots = false, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true) 
    {
        var playerList = Utilities
            .FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller")
            .Where(p => p != null && p.IsValid && 
                        (IncludeBots || (!p.IsBot && !p.IsHLTV)) && 
                        p.Connected == PlayerConnectedState.PlayerConnected && 
                        ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) || 
                        (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) || 
                        (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator)))
            .ToList();

        return playerList;
    }
    public static int GetPlayersCount(bool IncludeBots = false, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true)
    {
        return Utilities.GetPlayers().Count(p => 
            p != null && 
            p.IsValid && 
            p.Connected == PlayerConnectedState.PlayerConnected && 
            (IncludeBots || (!p.IsBot && !p.IsHLTV)) && 
            ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) || 
            (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) || 
            (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator))
        );
    }
    
    public static void ClearVariables()
    {
        var g_Main = HealthBarHitMarkGoldKingZ.Instance.g_Main;

        g_Main.Player_Data.Clear();
    }
    
    public static void CreateResource(string jsonFilePath)
    {
        string headerLine = "////// vvvvvv Add Paths For Precache Resources Down vvvvvvvvvv //////";
            string[] defaultLines = new[]
            {
                "sounds/goldkingz/hitmark/headshot.vsnd",
                "sounds/goldkingz/hitmark/bodyhit.vsnd",
                "sounds/goldkingz/hitmark/headshot_2.vsnd",
                "sounds/goldkingz/hitmark/bodyhit_2.vsnd"
            };

        if (!File.Exists(jsonFilePath))
        {
            using StreamWriter sw = File.CreateText(jsonFilePath);
            sw.WriteLine(headerLine);
            foreach (var line in defaultLines)
            {
                sw.WriteLine(line);
            }
            return;
        }

        string[] existingLines = File.ReadAllLines(jsonFilePath);
        if (existingLines.Length == 0 || existingLines[0] != headerLine)
        {
            using StreamWriter sw = new(jsonFilePath);
            sw.WriteLine(headerLine);
            foreach (string line in existingLines)
            {
                sw.WriteLine(line);
            }
        }
    }
    public static void DebugMessage(string message, bool prefix = true)
    {
        if (!Configs.GetConfigData().EnableDebug) return;

        Console.ForegroundColor = ConsoleColor.Magenta;
        string Prefix = $"[HitMark]: ";
        Console.WriteLine(prefix?Prefix:"" + message);
        
        Console.ResetColor();
    }
    public static CCSGameRules? GetGameRules()
    {
        try
        {
            var gameRulesEntities = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules");
            return gameRulesEntities.First().GameRules;
        }
        catch
        {
            return null;
        }
    }
    public static bool IsWarmup()
    {
        return GetGameRules()?.WarmupPeriod ?? false;
    }

    public static void InitializePlayerHUD(CCSPlayerController player)
    {
        var g_Main = HealthBarHitMarkGoldKingZ.Instance.g_Main;
        var api = HealthBarHitMarkGoldKingZ.GetGameHudApi();
        var config = Configs.GetConfigData();
        
        if (player == null || !player.IsValid || api == null) return;

        string? headSound = ResolveSoundForPlayer(config.HM_HeadShotSounds, player);
        string? bodySound = ResolveSoundForPlayer(config.HM_BodyShotSounds, player);
        var bodyColor = ParseColor(config.HM_BodyShotColor, Color.White);
        float offsetX = config.HM_HudOffsetX;
        float offsetY = config.HM_HudOffsetY;
        float offsetZ = config.HM_HudDistance;
        float damageBaseX = offsetX + config.HM_DamageOffsetX;
        float damageBaseY = offsetY + config.HM_DamageOffsetY;
        var damageColor = ParseColor(config.HM_DamageColor, Color.White);

        bool shouldInitHitIcon = config.HM_EnableHitMark && !string.IsNullOrWhiteSpace(config.HM_HitChar);
        bool shouldInitDamageHud = config.HM_ShowDamageValue && config.HM_DamageFontSize > 0;

        try
        {
            g_Main.Player_Data[player] = new Globals.PlayerDataClass(
                player,
                headSound ?? string.Empty,
                bodySound ?? string.Empty
            );

            if (shouldInitHitIcon)
            {
                float baseScale = MathF.Max(1f, config.HM_HitScaleStart);
                SetHitIconHud(api, player, offsetX, offsetY, offsetZ, bodyColor, baseScale, config);
            }

            if (shouldInitDamageHud)
            {
                var damageLayers = BuildDamageLayers(config, damageColor);
                ApplyDamageLayers(api, player, damageLayers, damageBaseX, damageBaseY, offsetZ, config, initialize:true);
            }
        }
        catch (Exception ex)
        {
            DebugMessage($"Failed to initialize HUD for player {player.PlayerName}: {ex.Message}");
        }
    }
    
    public static void StartHitMark(CCSPlayerController player, bool HeadShot, int damage)
    {
        var g_Main = HealthBarHitMarkGoldKingZ.Instance.g_Main;
        var api = HealthBarHitMarkGoldKingZ.GetGameHudApi();
        var config = Configs.GetConfigData();
        
        if(player == null || !player.IsValid || api == null) return;

        if (!g_Main.Player_Data.ContainsKey(player))
        {
            InitializePlayerHUD(player);
        }

        if (!g_Main.Player_Data.ContainsKey(player))
        {
            return;
        }

        var playerData = g_Main.Player_Data[player];

        Color color = HeadShot ? ParseColor(config.HM_HeadShotColor, Color.White) : ParseColor(config.HM_BodyShotColor, Color.White);
        float duration = HeadShot ? config.HM_HeadShotDuration : config.HM_BodyShotDuration;
        float offsetX = config.HM_HudOffsetX;
        float offsetY = config.HM_HudOffsetY;
        float offsetZ = config.HM_HudDistance;
        float damageBaseX = offsetX + config.HM_DamageOffsetX;
        float damageBaseY = offsetY + config.HM_DamageOffsetY;
        var (damageStartX, damageStartY) = GetMotionStartPosition(damageBaseX, damageBaseY, config);
        var defaultDamageColor = ParseColor(config.HM_DamageColor, Color.White);
        var headshotDamageColor = ParseColor(config.HM_DamageHeadShotColor, defaultDamageColor);
        var activeDamageColor = HeadShot ? headshotDamageColor : defaultDamageColor;
        int displayDamage = Math.Max(0, damage);
        string damageText = displayDamage.ToString(CultureInfo.InvariantCulture);
        bool shouldShowDamageValue = config.HM_ShowDamageValue && config.HM_DamageFontSize > 0;
        bool shouldShowHitIcon = config.HM_EnableHitMark && !string.IsNullOrWhiteSpace(config.HM_HitChar);
        string glyph = shouldShowHitIcon ? config.HM_HitChar.Trim() : string.Empty;
        float startScale = MathF.Max(1f, config.HM_HitScaleStart);
        float baseEndScale = MathF.Max(startScale, config.HM_HitScaleEnd);
        float targetScale = baseEndScale;

        try
        {
            if (shouldShowHitIcon)
            {
                ShowHitIcon(api, player, offsetX, offsetY, offsetZ, color, glyph, duration, startScale, targetScale, config);
            }

            if (shouldShowDamageValue)
            {
                var damageLayers = BuildDamageLayers(config, activeDamageColor);
                ApplyDamageLayers(api, player, damageLayers, damageStartX, damageStartY, offsetZ, config, initialize:false);
                ShowDamageLayers(api, player, damageLayers, damageText, config.HM_DamageDuration);

                if (config.HM_DamageMotionMode == 1)
                {
                    StartDamageBounceAnimation(api, player, damageLayers, damageStartX, damageStartY, offsetZ, config);
                }
            }

            if(HeadShot && !string.IsNullOrEmpty(playerData.Sound_HeadShot))
            {
                player.ExecuteClientCommand("play " + playerData.Sound_HeadShot);
            }
            else if(!HeadShot && !string.IsNullOrEmpty(playerData.Sound_BodyShot))
            {
                player.ExecuteClientCommand("play " + playerData.Sound_BodyShot);
            }
        }
        catch (Exception ex)
        {
            DebugMessage($"Failed to show hitmark: {ex.Message}");
        }
    }

    private static void SetDamageHud(IGameHUDAPI api, CCSPlayerController player, byte channel, float posX, float posY, float posZ, Color damageColor, Configs.ConfigData config)
    {
        api.Native_GameHUD_SetParams(
            player,
            channel,
            posX,
            posY,
            posZ,
            damageColor,
            config.HM_DamageFontSize,
            config.HM_FontName,
            GetFontUnits(config),
            PointWorldTextJustifyHorizontal_t.POINT_WORLD_TEXT_JUSTIFY_HORIZONTAL_CENTER,
            PointWorldTextJustifyVertical_t.POINT_WORLD_TEXT_JUSTIFY_VERTICAL_CENTER,
            PointWorldTextReorientMode_t.POINT_WORLD_TEXT_REORIENT_AROUND_UP
        );
    }

    private static void UpdateDamageHud(IGameHUDAPI api, CCSPlayerController player, byte channel, float posX, float posY, float posZ, Color damageColor, Configs.ConfigData config)
    {
        api.Native_GameHUD_UpdateParams(
            player,
            channel,
            posX,
            posY,
            posZ,
            damageColor,
            config.HM_DamageFontSize,
            config.HM_FontName,
            GetFontUnits(config),
            PointWorldTextJustifyHorizontal_t.POINT_WORLD_TEXT_JUSTIFY_HORIZONTAL_CENTER,
            PointWorldTextJustifyVertical_t.POINT_WORLD_TEXT_JUSTIFY_VERTICAL_CENTER,
            PointWorldTextReorientMode_t.POINT_WORLD_TEXT_REORIENT_AROUND_UP
        );
    }

    private static List<DamageLayer> BuildDamageLayers(Configs.ConfigData config, Color baseColor)
    {
        return new List<DamageLayer>
        {
            new DamageLayer(HitmarkDamageChannel, baseColor, 0f, 0f)
        };
    }

    private static void SetHitIconHud(IGameHUDAPI api, CCSPlayerController player, float posX, float posY, float posZ, Color color, float fontSize, Configs.ConfigData config)
    {
        api.Native_GameHUD_SetParams(
            player,
            HitmarkIconChannel,
            posX,
            posY,
            posZ,
            color,
            NormalizeFontSize(fontSize),
            config.HM_FontName,
            GetFontUnits(config),
            PointWorldTextJustifyHorizontal_t.POINT_WORLD_TEXT_JUSTIFY_HORIZONTAL_CENTER,
            PointWorldTextJustifyVertical_t.POINT_WORLD_TEXT_JUSTIFY_VERTICAL_CENTER,
            PointWorldTextReorientMode_t.POINT_WORLD_TEXT_REORIENT_AROUND_UP
        );
    }

    private static void UpdateHitIconHud(IGameHUDAPI api, CCSPlayerController player, float posX, float posY, float posZ, Color color, float fontSize, Configs.ConfigData config)
    {
        api.Native_GameHUD_UpdateParams(
            player,
            HitmarkIconChannel,
            posX,
            posY,
            posZ,
            color,
            NormalizeFontSize(fontSize),
            config.HM_FontName,
            GetFontUnits(config),
            PointWorldTextJustifyHorizontal_t.POINT_WORLD_TEXT_JUSTIFY_HORIZONTAL_CENTER,
            PointWorldTextJustifyVertical_t.POINT_WORLD_TEXT_JUSTIFY_VERTICAL_CENTER,
            PointWorldTextReorientMode_t.POINT_WORLD_TEXT_REORIENT_AROUND_UP
        );
    }

    private static void ShowHitIcon(IGameHUDAPI api, CCSPlayerController player, float posX, float posY, float posZ, Color color, string glyph, float duration, float startScale, float targetScale, Configs.ConfigData config)
    {
        float sanitizedStart = ClampScale(startScale);
        float sanitizedEnd = ClampScale(targetScale);

        SetHitIconHud(api, player, posX, posY, posZ, color, sanitizedStart, config);
        api.Native_GameHUD_Show(player, HitmarkIconChannel, glyph, duration);
        AnimateHitIconScale(api, player, posX, posY, posZ, color, sanitizedStart, sanitizedEnd, config);
    }

    private static void AnimateHitIconScale(IGameHUDAPI api, CCSPlayerController player, float posX, float posY, float posZ, Color color, float startScale, float targetScale, Configs.ConfigData config)
    {
        var plugin = HealthBarHitMarkGoldKingZ.Instance;
        if (plugin == null)
        {
            return;
        }

        float duration = MathF.Max(0.01f, config.HM_HitScaleDuration);
        int steps = Math.Clamp(config.HM_HitScaleSteps, 1, 30);
        float interval = duration / steps;

        for (int step = 1; step <= steps; step++)
        {
            float progress = (float)step / steps;
            float eased = EaseInOut(progress);
            float nextScale = startScale + ((targetScale - startScale) * eased);
            float delay = interval * step;

            plugin.AddTimer(delay, () =>
            {
                if (api == null || player == null || !player.IsValid)
                {
                    return;
                }

                UpdateHitIconHud(api, player, posX, posY, posZ, color, nextScale, config);
            });
        }
    }

    private static int NormalizeFontSize(float fontSize)
    {
        return (int)Math.Clamp(MathF.Round(fontSize), 1f, 96f);
    }

    private static float ClampScale(float scale)
    {
        if (scale < 1f)
        {
            return 1f;
        }

        if (scale > 96f)
        {
            return 96f;
        }

        return scale;
    }

    private static float GetFontUnits(Configs.ConfigData config)
    {
        float units = config.HM_FontUnits;
        if (units < 0.05f)
        {
            return 0.05f;
        }

        if (units > 1f)
        {
            return 1f;
        }

        return units;
    }

    private static float EaseInOut(float t)
    {
        return t < 0.5f
            ? 4f * t * t * t
            : 1f - MathF.Pow(-2f * t + 2f, 3f) / 2f;
    }

    private static void ApplyDamageLayers(IGameHUDAPI api, CCSPlayerController player, IReadOnlyList<DamageLayer> layers, float centerX, float centerY, float posZ, Configs.ConfigData config, bool initialize)
    {
        foreach (var layer in layers)
        {
            float targetX = centerX + layer.RelativeOffsetX;
            float targetY = centerY + layer.RelativeOffsetY;

            if (initialize)
            {
                SetDamageHud(api, player, layer.Channel, targetX, targetY, posZ, layer.Color, config);
            }
            else
            {
                UpdateDamageHud(api, player, layer.Channel, targetX, targetY, posZ, layer.Color, config);
            }
        }
    }

    private static void ShowDamageLayers(IGameHUDAPI api, CCSPlayerController player, IReadOnlyList<DamageLayer> layers, string damageText, float duration)
    {
        foreach (var layer in layers)
        {
            api.Native_GameHUD_Show(player, layer.Channel, damageText, duration);
        }
    }

    private static (float X, float Y) GetMotionStartPosition(float baseX, float baseY, Configs.ConfigData config)
    {
        if (config.HM_DamageMotionMode != 1)
        {
            return (baseX, baseY);
        }

        float jitterX = SampleJitter(config.HM_DamageJitterRangeX);
        float jitterY = SampleJitter(config.HM_DamageJitterRangeY);
        return (baseX + jitterX, baseY + jitterY);
    }

    private static float SampleJitter(float maxRange)
    {
        if (maxRange <= 0f)
        {
            return 0f;
        }

        double sample = DamageRandom.NextDouble() * 2d - 1d;
        return (float)(sample * maxRange);
    }

    private static void StartDamageBounceAnimation(IGameHUDAPI api, CCSPlayerController player, IReadOnlyList<DamageLayer> layers, float centerX, float centerY, float offsetZ, Configs.ConfigData config)
    {
        float bounceHeight = MathF.Max(0f, config.HM_DamageBounceHeight);
        float bounceDuration = MathF.Max(0.05f, config.HM_DamageBounceDuration);
        if (bounceHeight <= 0f)
        {
            return;
        }

        var pluginInstance = HealthBarHitMarkGoldKingZ.Instance;
        if (pluginInstance == null)
        {
            return;
        }

        int steps = Math.Clamp(config.HM_HitScaleSteps * 2, 6, 40);
        float interval = bounceDuration / steps;

        for (int step = 0; step <= steps; step++)
        {
            float progress = (float)step / steps;
            float arc = 1f - MathF.Pow(2f * progress - 1f, 2f); // parabola peak at middle
            float targetY = centerY + (arc * bounceHeight);
            float delay = interval * step;

            pluginInstance.AddTimer(delay, () =>
            {
                if (api == null || player == null || !player.IsValid)
                {
                    return;
                }

                ApplyDamageLayers(api, player, layers, centerX, targetY, offsetZ, config, initialize: false);
            });
        }
    }

    private static string? ResolveSoundForPlayer(List<string> entries, CCSPlayerController player)
    {
        string? defaultSound = null;
        foreach (var entry in entries)
        {
            if (string.IsNullOrWhiteSpace(entry)) continue;

            var parts = entry.Split('|', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();
            if (parts.Length == 0) continue;

            var flag = parts[0];
            var soundPath = parts.Length >= 2 ? parts[1] : null;

            if (flag.Equals("ANY", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(soundPath) && defaultSound == null)
                {
                    defaultSound = soundPath;
                }
                continue;
            }

            if (IsPlayerInGroupPermission(player, flag))
            {
                return soundPath ?? defaultSound;
            }
        }

        return defaultSound;
    }

    private static Color ParseColor(string colorValue, Color fallback)
    {
        if (string.IsNullOrWhiteSpace(colorValue)) return fallback;

        try
        {
            if (colorValue.StartsWith("#"))
            {
                return ColorTranslator.FromHtml(colorValue);
            }

            var color = Color.FromName(colorValue);
            return color.A == 0 && !string.Equals(colorValue, "Transparent", StringComparison.OrdinalIgnoreCase)
                ? fallback
                : color;
        }
        catch
        {
            return fallback;
        }
    }
}