using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using SLP.Core;
using UnityEngine;

namespace SLP.Features.Evacuation;

public class EvacuationModule : Module
{
    public override string Name => "Evacuation";
    public override Version Version => new(1, 0, 0);

    private readonly Vector3 _helipadPosition = new(131f, 295f, -43f);
    private readonly Vector3 _carPosition = new(9f, 291f, -42f);

    private readonly float _helipadRadius = 8f;
    private readonly float _checkInterval = 1f;
    private readonly int _evacuationTime = 30;

    private readonly HashSet<Player> _playersOnHelipad = new();
    private readonly HashSet<Player> _playersOnCar = new();
    private CoroutineHandle _checkCoroutine;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Died += OnPlayerDied;
        Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
        Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

        _checkCoroutine = Timing.RunCoroutine(HelipadCheckLoop());
        Log.Info($"[{Name}] Plugin enabled! Monitoring Gate B helipad for evacuation.");
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
        Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
        Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

        Timing.KillCoroutines(_checkCoroutine);
        _playersOnHelipad.Clear();
        base.OnDisabled();
    }

    private void OnRoundStarted()
    {
        _playersOnHelipad.Clear();
        Log.Info($"[{Name}] Round started. Helipad tracking reset.");
    }

    private void OnRoundEnded(Exiled.Events.EventArgs.Server.RoundEndedEventArgs ev)
    {
        _playersOnHelipad.Clear();
        Timing.KillCoroutines(_checkCoroutine);
    }

    private void OnPlayerDied(DiedEventArgs ev)
    {
        if (_playersOnHelipad.Contains(ev.Player))
        {
            _playersOnHelipad.Remove(ev.Player);
            Log.Info($"[{Name}] {ev.Player.Nickname} died on helipad. Removed from tracking.");
        }
    }

    private IEnumerator<float> HelipadCheckLoop()
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(_checkInterval);

            if (!Round.IsStarted)
                continue;

            Vector3 helipadCenter = _helipadPosition;
            Vector3 carPosition = _carPosition;
            float radius = _helipadRadius;

            foreach (Player player in Player.List)
            {
                if (player == null || !player.IsAlive)
                    continue;

                bool isOnHelipad = Vector3.Distance(player.Position, helipadCenter) <= radius;
                bool isOnCar = Vector3.Distance(player.Position, carPosition) <= radius;

                if (isOnHelipad && _playersOnHelipad.Add(player))
                {
                    OnPlayerEnterHelipad(player);
                }
                else if (!isOnHelipad && _playersOnHelipad.Contains(player))
                {
                    _playersOnHelipad.Remove(player);
                    OnPlayerLeaveHelipad(player);
                }

                if (isOnCar && _playersOnCar.Contains(player))
                {
                    _playersOnCar.Add(player);
                    OnPlayerEnterHelipad(player);
                }
                else if (!isOnCar && _playersOnCar.Contains(player))
                {
                    _playersOnCar.Remove(player);
                    OnPlayerLeaveHelipad(player);
                }
            }
        }
        // ReSharper disable once IteratorNeverReturns
    }

    private void OnPlayerEnterHelipad(Player player)
    {
        Log.Info($"[{Name}] {player.Nickname} entered Gate B helipad.");

        player.ShowHint(
            $"<color=#00ff88><b>🚁 ПЛОЩАДКА</b></color>\n" +
            $"<color=#ffffff>Оставайтесь на площадке <b>{_evacuationTime}</b> сек. для эвакуации.</color>",
            5f
        );

        Timing.RunCoroutine(EvacuationCountdown(player));
    }

    private void OnPlayerLeaveHelipad(Player player)
    {
        Log.Info($"[{Name}] {player.Nickname} left Gate B helipad. Evacuation cancelled.");
        player.ShowHint(
            "<color=#ff4444><b>❌ Эвакуация отменена!</b></color>\n" +
            "<color=#ffffff>Вы покинули вертолётную площадку.</color>",
            4f
        );
    }

    private IEnumerator<float> EvacuationCountdown(Player player)
    {
        int timeLeft = _evacuationTime;
        while (timeLeft > 0)
        {
            yield return Timing.WaitForSeconds(1f);

            if (player == null || !player.IsAlive || !_playersOnHelipad.Contains(player))
                yield break;

            timeLeft--;

            string color = timeLeft <= 5 ? "#ff4444" : "#00ff88";
            player.ShowHint(
                $"<color=#00ff88><b>🚁 ЭВАКУАЦИЯ</b></color>\n" +
                $"<color={color}>Осталось: <b>{timeLeft}</b> сек.</color>\n" +
                $"<color=#aaaaaa>Не покидайте площадку!</color>",
                1.5f
            );
        }

        if (player == null || !player.IsAlive || !_playersOnHelipad.Contains(player))
            yield break;

        EvacuatePlayer(player);
    }

    private void EvacuatePlayer(Player player)
    {
        Log.Info($"[{Name}] Evacuating {player.Nickname} from Gate B helipad!");

        _playersOnHelipad.Remove(player);
        _playersOnCar.Remove(player);

        player.ShowHint(
            "<color=#00ff88><b>✅ ВЫ ЭВАКУИРОВАНЫ</b></color>\n" +
            "<color=#ffffff>Успешно забрали вас с площадки.</color>",
            8f
        );

        player.Role.Set(RoleTypeId.Spectator);

        Log.Info($"[{Name}] Player {player.Nickname} evacuation complete.");
    }
}