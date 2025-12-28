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

namespace HealthBar_HitMark_GoldKingZ;

public class Helper
{
    private const byte HitmarkIconChannel = 10;
    private const byte HitmarkDamageChannel = 11;

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
        float damageOffsetX = offsetX + config.HM_DamageOffsetX;
        float damageOffsetY = offsetY + config.HM_DamageOffsetY;
        var damageColor = ParseColor(config.HM_DamageColor, Color.White);

        try
        {
            api.Native_GameHUD_SetParams(
                player,
                HitmarkIconChannel,
            offsetX,
            offsetY,
            offsetZ,
                bodyColor,
                config.HM_BodyShotFontSize,
                config.HM_FontName,
                0.25f,
                PointWorldTextJustifyHorizontal_t.POINT_WORLD_TEXT_JUSTIFY_HORIZONTAL_CENTER,
                PointWorldTextJustifyVertical_t.POINT_WORLD_TEXT_JUSTIFY_VERTICAL_CENTER,
                PointWorldTextReorientMode_t.POINT_WORLD_TEXT_REORIENT_AROUND_UP
            );

            g_Main.Player_Data[player] = new Globals.PlayerDataClass(
                player,
                headSound ?? string.Empty,
                bodySound ?? string.Empty
            );

            if (config.HM_ShowDamageValue)
            {
                api.Native_GameHUD_SetParams(
                    player,
                    HitmarkDamageChannel,
                    damageOffsetX,
                    damageOffsetY,
                    offsetZ,
                    damageColor,
                    config.HM_DamageFontSize,
                    config.HM_FontName,
                    0.25f,
                    PointWorldTextJustifyHorizontal_t.POINT_WORLD_TEXT_JUSTIFY_HORIZONTAL_CENTER,
                    PointWorldTextJustifyVertical_t.POINT_WORLD_TEXT_JUSTIFY_VERTICAL_CENTER,
                    PointWorldTextReorientMode_t.POINT_WORLD_TEXT_REORIENT_AROUND_UP
                );
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
        
        if(player == null || !player.IsValid || !g_Main.Player_Data.ContainsKey(player) || api == null) return;

        var playerData = g_Main.Player_Data[player];

        string icon = HeadShot ? config.HM_HeadShotIcon : config.HM_BodyShotIcon;
        Color color = HeadShot ? ParseColor(config.HM_HeadShotColor, Color.White) : ParseColor(config.HM_BodyShotColor, Color.White);
        int fontSize = HeadShot ? config.HM_HeadShotFontSize : config.HM_BodyShotFontSize;
        float duration = HeadShot ? config.HM_HeadShotDuration : config.HM_BodyShotDuration;
        float offsetX = config.HM_HudOffsetX;
        float offsetY = config.HM_HudOffsetY;
        float offsetZ = config.HM_HudDistance;
        float damageOffsetX = offsetX + config.HM_DamageOffsetX;
        float damageOffsetY = offsetY + config.HM_DamageOffsetY;
        var damageColor = ParseColor(config.HM_DamageColor, Color.White);
        int displayDamage = Math.Max(0, damage);
        string damageText = $"-{displayDamage}";

        try
        {
            api.Native_GameHUD_UpdateParams(
                player,
                HitmarkIconChannel,
            offsetX,
            offsetY,
            offsetZ,
                color,
                fontSize,
                config.HM_FontName,
                0.25f,
                PointWorldTextJustifyHorizontal_t.POINT_WORLD_TEXT_JUSTIFY_HORIZONTAL_CENTER,
                PointWorldTextJustifyVertical_t.POINT_WORLD_TEXT_JUSTIFY_VERTICAL_CENTER,
                PointWorldTextReorientMode_t.POINT_WORLD_TEXT_REORIENT_AROUND_UP
            );

            api.Native_GameHUD_Show(player, HitmarkIconChannel, icon, duration);

            if (config.HM_ShowDamageValue)
            {
                api.Native_GameHUD_UpdateParams(
                    player,
                    HitmarkDamageChannel,
                    damageOffsetX,
                    damageOffsetY,
                    offsetZ,
                    damageColor,
                    config.HM_DamageFontSize,
                    config.HM_FontName,
                    0.25f,
                    PointWorldTextJustifyHorizontal_t.POINT_WORLD_TEXT_JUSTIFY_HORIZONTAL_CENTER,
                    PointWorldTextJustifyVertical_t.POINT_WORLD_TEXT_JUSTIFY_VERTICAL_CENTER,
                    PointWorldTextReorientMode_t.POINT_WORLD_TEXT_REORIENT_AROUND_UP
                );

                api.Native_GameHUD_Show(player, HitmarkDamageChannel, damageText, config.HM_DamageDuration);
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