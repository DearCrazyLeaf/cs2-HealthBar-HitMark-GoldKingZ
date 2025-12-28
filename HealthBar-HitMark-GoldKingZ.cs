using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Attributes;
using HealthBar_HitMark_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;
using CS2_GameHUDAPI;
using CounterStrikeSharp.API.Core.Capabilities;
using System.IO;

namespace HealthBar_HitMark_GoldKingZ;


public class HealthBarHitMarkGoldKingZ : BasePlugin
{
    public override string ModuleName => "Custom HitMarks with Sounds (GameHUD)";
    public override string ModuleVersion => "2.0.0";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
	public static HealthBarHitMarkGoldKingZ Instance { get; set; } = new();
    public Globals g_Main = new();
    
    private static IGameHUDAPI? _gameHudApi;
    private static PluginCapability<IGameHUDAPI> PluginCapability = new("gamehud:api");
    
    public override void Load(bool hotReload)
    {
        Instance = this;
        Configs.Load(ModuleDirectory);
        Configs.Shared.CookiesModule = ModuleDirectory;
        Configs.Shared.StringLocalizer = Localizer;
    
        RegisterEventHandler<EventPlayerHurt>(OnEventPlayerHurt);
        RegisterEventHandler<EventRoundStart>(OnEventRoundStart);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        RegisterListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);


        if(Configs.GetConfigData().HM_MuteDefaultHeadShotBodyShot)
        {
            HookUserMessage(208, um =>
            {
                var soundevent = um.ReadUInt("soundevent_hash");
                uint HeadShotHit_ClientSide = 2831007164;
                uint Player_Got_Damage_ClientSide = 708038349;

                bool HH = g_Main.Player_Data.Any(playerdata => !string.IsNullOrEmpty(playerdata.Value.Sound_HeadShot) && soundevent == HeadShotHit_ClientSide) ? true : false;
                bool BH = g_Main.Player_Data.Any(playerdata => !string.IsNullOrEmpty(playerdata.Value.Sound_BodyShot) && soundevent == Player_Got_Damage_ClientSide) ? true : false;
                if (HH || BH)
                {
                    return HookResult.Stop;
                }
                return HookResult.Continue;

            }, HookMode.Pre);
        }
    }

    public void OnServerPrecacheResources(ResourceManifest manifest)
    {
        try
        {
            string configDir = Path.Combine(ModuleDirectory, "config");
            string filePath = Path.Combine(configDir, "ServerPrecacheResources.txt");

            Helper.CreateResource(filePath);

            foreach (string line in File.ReadAllLines(filePath))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("//"))
                {
                    continue;
                }
                manifest.AddResource(trimmed);
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"[HitMark] Failed to precache resources: {ex.Message}");
        }
    }

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        try
        {
            _gameHudApi = PluginCapability.Get();
            if (_gameHudApi == null)
            {
                Helper.DebugMessage("[HitMark] GameHUD API not found! Plugin will not work correctly.");
            }
            else
            {
                Helper.DebugMessage("[HitMark] GameHUD API loaded successfully!");
            }
        }
        catch (Exception ex)
        {
            _gameHudApi = null;
            Helper.DebugMessage($"[HitMark] Failed to load GameHUD API: {ex.Message}");
        }
    }

    public HookResult OnEventRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        Helper.ClearVariables();

        foreach(var players in Helper.GetPlayersController())
        {
            if(players == null || !players.IsValid)continue;

            Helper.InitializePlayerHUD(players);
        }
        return HookResult.Continue;
    }

    public HookResult OnEventPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        var victim = @event.Userid;
        var dmgHealth = @event.DmgHealth;
        var health = @event.Health;
        var Hitgroup = @event.Hitgroup;

        if (victim == null || !victim.IsValid || victim.PlayerPawn == null || !victim.PlayerPawn.IsValid
        || victim.PlayerPawn.Value == null || !victim.PlayerPawn.Value.IsValid) return HookResult.Continue;

        var attacker = @event.Attacker;
        if (attacker == null || !attacker.IsValid) return HookResult.Continue;

        bool Check_teammates_are_enemies = ConVar.Find("mp_teammates_are_enemies")!.GetPrimitiveValue<bool>() == false && attacker.TeamNum != victim.TeamNum || ConVar.Find("mp_teammates_are_enemies")!.GetPrimitiveValue<bool>() == true;
        if (!Check_teammates_are_enemies) return HookResult.Continue;

        float oldHealth = health + dmgHealth;
        if (oldHealth == health) return HookResult.Continue;
        
        if(Configs.GetConfigData().HM_EnableHitMark)
        {
            if(!Configs.GetConfigData().HM_DisableOnWarmUp || Configs.GetConfigData().HM_DisableOnWarmUp && !Helper.IsWarmup())
            {
                if(Hitgroup == 1)
                {
                    Helper.StartHitMark(attacker, true, dmgHealth);
                }
                else
                {
                    Helper.StartHitMark(attacker, false, dmgHealth);
                }
            }
        }
        
        return HookResult.Continue;
    }

    private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;
        var player = @event.Userid;

        if (player == null || !player.IsValid)return HookResult.Continue;

        if (g_Main.Player_Data.ContainsKey(player))g_Main.Player_Data.Remove(player);

        return HookResult.Continue;
    }

    private void OnMapEnd()
    {
        Helper.ClearVariables();
    }

    public override void Unload(bool hotReload)
    {
        Helper.ClearVariables();
    }

    public static IGameHUDAPI? GetGameHudApi()
    {
        return _gameHudApi;
    }
}