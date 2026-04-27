using System;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using InventorySystem.Items.Firearms.Attachments;
using SLP.Core;

namespace SLP.Items.Sniper;

public class SniperModule : Module
{
    public override string Name => "SniperRifle";
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
    
[CustomItem(ItemType.GunE11SR)]
public class SniperRifleItem : CustomWeapon
{
    public override ItemType Type { get; set; } = ItemType.GunE11SR;
    public override uint Id { get; set; } = 228;

    public override string Name { get; set; } = "Снайперская винтовка";

    public override string Description { get; set; } = "Ультра имба";

    public override float Weight { get; set; } = 1f;

    public override float Damage { get; set; } = 5000f;

    public override byte ClipSize { get; set; } = 5;

    public override SpawnProperties SpawnProperties { get; set; } = new()
    {
        DynamicSpawnPoints =
        [
            new()
            {
                Location = Exiled.API.Enums.SpawnLocationType.InsideHczArmory,
                Chance = 10
            }
        ]
    };

    public override AttachmentName[] Attachments { get; set; } =
        [AttachmentName.ScopeSight];
}