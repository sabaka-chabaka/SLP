#nullable enable
using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using SLP.Core;

namespace SLP.Items.EnergyDrink;

public class EnergyDrinkModule : Module
{
    public override string Name => "EnergyDrink";
    public override Version Version => new(1, 0, 0);

    public override void OnEnabled()
    {
        CustomItem.RegisterItems();
        base.OnEnabled();
    }
    
    public override void OnDisabled()
    {
        CustomItem.UnregisterItems();
        base.OnDisabled();
    }
}

[CustomItem(ItemType.SCP207)]
public class EnergyDrinkItem : CustomItem
{
    public override ItemType Type { get; set; } = ItemType.SCP207;
    public override uint Id { get; set; } = 229;

    public override string Name { get; set; } = "Энергетик";

    public override string Description { get; set; } = "Ультра имба";

    public override float Weight { get; set; } = 1f;

    public override SpawnProperties SpawnProperties { get; set; } = new()
    {
        DynamicSpawnPoints =
        [
            new DynamicSpawnPoint
            {
                Location = Exiled.API.Enums.SpawnLocationType.InsideIntercom,
                Chance = 100
            }
        ]
    };

    protected override void OnAcquired(Player player, Item item, bool displayMessage)
    {
        if (TryGet(item, out CustomItem? itema) && itema is not EnergyDrinkItem) return;
        
        Timing.CallDelayed(1.0f, () =>
        {
            player.Heal(100);
            player.DisableAllEffects();
            player.EnableEffect(EffectType.Invigorated, 10, 10);
            player.EnableEffect(EffectType.MovementBoost, 60, 10);
            player.EnableEffect(EffectType.Burned, 10, 10);
            Timing.CallDelayed(10.0f, () => player.EnableEffect(EffectType.CardiacArrest, 1, 5));
        });
        
        base.OnAcquired(player, item, displayMessage);
    }
}