using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using System.Diagnostics;

namespace HealthBar_HitMark_GoldKingZ;

public class Globals
{
    public class PlayerDataClass
    {
        public CCSPlayerController Player { get; set; }
        public string Sound_HeadShot { get; set; }       
        public string Sound_BodyShot { get; set; }       
    
        public PlayerDataClass(CCSPlayerController player, string sound_HeadShot, string sound_BodyShot)
        {
            Player = player;
            Sound_HeadShot = sound_HeadShot;
            Sound_BodyShot = sound_BodyShot;
        }
    }
    public Dictionary<CCSPlayerController, PlayerDataClass> Player_Data = new Dictionary<CCSPlayerController, PlayerDataClass>();
}