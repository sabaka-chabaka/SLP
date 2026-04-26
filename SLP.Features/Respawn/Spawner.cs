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
        
        SpawnCaptain(players.First(x => !x.IsAlive), wave);
        foreach (var player in players.Where(x => !x.IsAlive))
        {
            if (_random.Next(1, 2) == 1)
            {
                SpawnJust(player, wave);
            }
            else
            {
                SpawnSpecialist(player, wave);
            }
        }
    }
    
    private void SpawnCaptain(Player player, Wave wave)
    {
        player.Role.Set(wave.WaveType == WaveType.ChaosInsurgency ? RoleTypeId.ChaosRepressor : RoleTypeId.NtfCaptain);
        player.ClearInventory();
        player.MaxHealth = 175;

        foreach (var item in new[] { ItemType.None })
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

        foreach (var item in new[] { ItemType.None })
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

        foreach (var item in new[] { ItemType.None })
        {
            player.AddItem(item);
        }
        
        player.AddAmmo(AmmoType.Nato556, 200);
    }
}