using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using SLP.Core;
using UnityEngine;

namespace SLP.Features.Pig;

public class PigModule : Module
{
    public static PigModule Instance { get; private set; }
    public override string Name => "Pig";
    public override Version Version => new(1, 0, 0);
    
    public float MaxHp { get; set; } = 200f;
    public float HealPerSecond { get; set; } = 2f;
    public RoleTypeId BaseRole { get; set; } = RoleTypeId.Tutorial;
    public string SpawnMessage { get; set; } = "Вы — <b><color=#e06c00>СВИНОПАС</color></b>!";
    
    public Player CurrentSwinopas { get; set; }

    private EventHandlers _handlers;

    public override void OnEnabled()
    {
        Instance = this;
        _handlers = new EventHandlers();

        Exiled.Events.Handlers.Server.RoundStarted += _handlers.OnRoundStarted;
        Exiled.Events.Handlers.Player.Dying += _handlers.OnDying;

        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Server.RoundStarted -= _handlers.OnRoundStarted;
        Exiled.Events.Handlers.Player.Dying -= _handlers.OnDying;

        _handlers = null;
        Instance = null;

        base.OnDisabled();
    }
}

public class EventHandlers
{
    private CoroutineHandle _healCoroutine;

    public void OnRoundStarted()
    {
        Timing.CallDelayed(2f, SpawnSwinopas);
    }

    private void SpawnSwinopas()
    {
        var players = Player.List
            .Where(p => p.IsAlive && p != Server.Host)
            .ToList();

        if (players.Count == 0)
        {
            Log.Warn("[Swinopas] Нет живых игроков для назначения свинопаса.");
            return;
        }

        var chosen = players[UnityEngine.Random.Range(0, players.Count)];

        SetupSwinopas(chosen);
    }

    private void SetupSwinopas(Player player)
    {
        PigModule.Instance.CurrentSwinopas = player;

        player.Role.Set(PigModule.Instance.BaseRole, SpawnReason.ForceClass, RoleSpawnFlags.All);

        Timing.CallDelayed(0.3f, () =>
        {
            player.MaxHealth = PigModule.Instance.MaxHp;
            player.Health = PigModule.Instance.MaxHp;

            player.ClearInventory();

            player.AddItem(ItemType.ArmorHeavy);
            player.AddItem(ItemType.GunCOM18);
            player.AddItem(ItemType.Painkillers);

            player.AddAmmo(AmmoType.Nato9, 60);

            player.CustomInfo = "Свинопас";

            player.ShowHint(PigModule.Instance.SpawnMessage, 8f);
            player.Broadcast(5, "<b><color=#e06c00>🐷 СВИНОПАС</color> ОБНАРУЖЕН</b>");

            Log.Info($"[Swinopas] Свинопас назначен: {player.Nickname}");

            if (_healCoroutine.IsRunning)
                Timing.KillCoroutines(_healCoroutine);

            _healCoroutine = Timing.RunCoroutine(HealCoroutine(player));
        });
    }

    private IEnumerator<float> HealCoroutine(Player player)
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(1f);

            if (player == null || !player.IsAlive || PigModule.Instance.CurrentSwinopas != player)
                yield break;

            float maxHp = PigModule.Instance.MaxHp;
            if (player.Health < maxHp)
            {
                player.Health = Mathf.Min(player.Health + PigModule.Instance.HealPerSecond, maxHp);
            }
        }
    }

    public void OnDying(DyingEventArgs ev)
    {
        if (ev.Player == PigModule.Instance.CurrentSwinopas)
        {
            Log.Info($"[Swinopas] Свинопас {ev.Player.Nickname} умер.");
            PigModule.Instance.CurrentSwinopas = null;
            ev.Player.CustomInfo = null;

            if (_healCoroutine.IsRunning)
                Timing.KillCoroutines(_healCoroutine);

            Map.Broadcast(5, "<b><color=#e06c00>🐷 СВИНОПАС</color></b> был убит!");
        }
    }
}