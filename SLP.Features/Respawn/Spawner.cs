using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;

namespace SLP.Features.Respawn;

public class Spawner
{
    private readonly Random _random = new();
    
    public void Spawn(Wave wave)
    {
        var players = Player.List;

        var deadPlayers = players.Where(x => !x.IsAlive).ToList();

        if (deadPlayers.Count == 0)
            return;
        
        SpawnJust(deadPlayers.First(), wave);
        foreach (var player in deadPlayers.Skip(1))
        {
            if (_random.Next(0, 2) == 0)
            {
                SpawnJust(player, wave);
            }
            else
            {
                SpawnSpecialist(player, wave);
            }
        }
        
        Exiled.API.Features.Cassie.MessageTranslated(wave.Announcement, wave.Subtitles, false, true, true);
    }
    
    private void SpawnCaptain(Player player, Wave wave)
    {
        player.Role.Set(wave.WaveType == WaveType.ChaosInsurgency ? RoleTypeId.ChaosRepressor : RoleTypeId.NtfCaptain);
        player.ClearInventory();
        player.MaxHealth = 175;

        foreach (var item in new[]
                 { ItemType.ArmorHeavy, ItemType.GunLogicer, ItemType.Adrenaline, ItemType.Medkit, wave.WaveType == WaveType.ChaosInsurgency ? ItemType.KeycardChaosInsurgency : ItemType.KeycardMTFCaptain, ItemType.GrenadeHE, ItemType.GrenadeFlash })
        {
            player.AddItem(item);
        }
        
        player.AddAmmo(AmmoType.Nato762, 200);
    }

    private void SpawnSpecialist(Player player, Wave wave)
    {
        player.Role.Set(wave.WaveType == WaveType.ChaosInsurgency ? RoleTypeId.ChaosMarauder : RoleTypeId.NtfSpecialist);
        player.ClearInventory();
        player.MaxHealth = 125;

        foreach (var item in new[] { ItemType.ArmorCombat, ItemType.GunE11SR, ItemType.Medkit, wave.WaveType == WaveType.ChaosInsurgency ? ItemType.KeycardChaosInsurgency : ItemType.KeycardMTFOperative, ItemType.GrenadeHE })
        {
            player.AddItem(item);
        }
        
        player.AddAmmo(AmmoType.Nato556, 200);
    }

    private void SpawnJust(Player player, Wave wave)
    {
        player.Role.Set(wave.WaveType == WaveType.ChaosInsurgency ? RoleTypeId.ChaosRifleman : RoleTypeId.NtfSergeant);
        player.ClearInventory();
        player.MaxHealth = 100;

        foreach (var item in new[] { ItemType.ArmorCombat, ItemType.GunE11SR, ItemType.Medkit, wave.WaveType == WaveType.ChaosInsurgency ? ItemType.KeycardChaosInsurgency : ItemType.KeycardMTFOperative, ItemType.GrenadeFlash })
        {
            player.AddItem(item);
        }
        
        player.AddAmmo(AmmoType.Nato556, 200);
    }
}