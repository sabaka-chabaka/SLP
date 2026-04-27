using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using ProjectMER.Events.Arguments;
using ProjectMER.Events.Handlers;
using ProjectMER.Features.Objects;
using SLP.Core;
using UnityEngine;

namespace SLP.Features.Scp106Cont;

public enum RecontainmentState
{
    Idle,
    PersonnelInsideWithD,
    DLeftInside,
    SequenceRunning,
}

public class Scp106ContModule : Module
{
    public override string Name => "SCP106Containment";
    public override Version Version => new(1, 0, 0);

    private readonly string _chamberSchematicName = "106_Chamber";

    private readonly string _containmentSchematicName = "106_Containment";

    private readonly string _buttonSchematicName = "106_Button";

    private readonly float _chamberRadius = 6f;

    private readonly float _checkInterval = 1f;

    private readonly float _sequenceDelay = 2f;

    private readonly float _killDelay = 2f;
    
    private RecontainmentState _state = RecontainmentState.Idle;
    private Player _sacrificedClassD;

    private SchematicObject _chamberSchematic;
    private SchematicObject _containmentSchematic;

    private readonly HashSet<string> _enteredChamber = new();
    private CoroutineHandle _proximityLoop;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
        Exiled.Events.Handlers.Server.RoundEnded   += OnRoundEnded;
        Exiled.Events.Handlers.Player.Died         += OnPlayerDied;

        Schematic.ButtonInteracted  += OnButtonInteracted;
        Schematic.SchematicSpawned  += OnSchematicSpawned;
        Schematic.SchematicDestroyed += OnSchematicDestroyed;

        Log.Info($"[{Name}] Enabled.");
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
        Exiled.Events.Handlers.Server.RoundEnded   -= OnRoundEnded;
        Exiled.Events.Handlers.Player.Died         -= OnPlayerDied;

        Schematic.ButtonInteracted   -= OnButtonInteracted;
        Schematic.SchematicSpawned   -= OnSchematicSpawned;
        Schematic.SchematicDestroyed -= OnSchematicDestroyed;

        Timing.KillCoroutines(_proximityLoop);
        ResetState();
        base.OnDisabled();
    }
    
    private void OnRoundStarted()
    {
        ResetState();
        _proximityLoop = Timing.RunCoroutine(ProximityLoop());
    }

    private void OnRoundEnded(Exiled.Events.EventArgs.Server.RoundEndedEventArgs ev)
    {
        Timing.KillCoroutines(_proximityLoop);
        ResetState();
    }

    private void OnSchematicSpawned(SchematicSpawnedEventArgs ev)
    {
        if (ev.Name == _chamberSchematicName)
        {
            _chamberSchematic = ev.Schematic;
            Log.Info($"[{Name}] Chamber schematic resolved: {ev.Name}");
        }
        else if (ev.Name == _containmentSchematicName)
        {
            _containmentSchematic = ev.Schematic;
            Log.Info($"[{Name}] Containment schematic resolved: {ev.Name}");
        }
    }

    private void OnSchematicDestroyed(SchematicDestroyedEventArgs ev)
    {
        if (ev.Name == _chamberSchematicName)
            _chamberSchematic = null;
        else if (ev.Name == _containmentSchematicName)
            _containmentSchematic = null;
    }
    
    private IEnumerator<float> ProximityLoop()
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(_checkInterval);

            if (!Round.IsStarted || _chamberSchematic == null)
                continue;

            Vector3 chamberPos = _chamberSchematic.Position;

            foreach (Player player in Player.List)
            {
                if (player == null || !player.IsAlive) continue;

                bool inside    = Vector3.Distance(player.Position, chamberPos) <= _chamberRadius;
                bool wasInside = _enteredChamber.Contains(player.UserId);

                if (inside && !wasInside)
                {
                    _enteredChamber.Add(player.UserId);
                    OnPlayerEnterChamber(player);
                }
                else if (!inside && wasInside)
                {
                    _enteredChamber.Remove(player.UserId);
                    OnPlayerLeaveChamber(player);
                }
            }
        }
    }
    
    private void OnPlayerEnterChamber(Player player)
        {
            if (player.Role.Type == RoleTypeId.ClassD)
            {
                player.ShowHint(
                    "<color=#ff8800><b>⚠ ВЫ В КАМЕРЕ SCP-106</b></color>\n" +
                    "<color=#ffffff>Вас оставят здесь для реконтайма.</color>",
                    5f
                );
                return;
            }

            bool dInsideToo = Player.List.Any(p =>
                p.IsAlive &&
                p.Role.Type == RoleTypeId.ClassD &&
                _enteredChamber.Contains(p.UserId)
            );

            if (_state == RecontainmentState.Idle && dInsideToo)
            {
                _state = RecontainmentState.PersonnelInsideWithD;
                Log.Info($"[{Name}] Personnel + D inside. State → PersonnelInsideWithD");
            }

            player.ShowHint(
                "<color=#00aaff><b>📋 ШАГ 1 — РЕКОНТАЙМ SCP-106</b></color>\n" +
                "<color=#ffffff>Вы внутри. <b>Выйдите</b>, оставив класс-D в камере.</color>",
                5f
            );
        }
        
        private void OnPlayerLeaveChamber(Player player)
        {
            if (player.Role.Type == RoleTypeId.ClassD || player.Role.Type == RoleTypeId.Scp106)
                return;

            if (_state != RecontainmentState.PersonnelInsideWithD)
                return;

            var classDs = Player.List.Where(p =>
                p.IsAlive &&
                p.Role.Type == RoleTypeId.ClassD &&
                _enteredChamber.Contains(p.UserId)
            ).ToList();

            bool personnelStillInside = Player.List.Any(p =>
                p.IsAlive &&
                p.Role.Type != RoleTypeId.ClassD &&
                p.Role.Type != RoleTypeId.Scp106 &&
                _enteredChamber.Contains(p.UserId)
            );

            if (classDs.Count == 0 || personnelStillInside)
                return;

            _sacrificedClassD = classDs.First();
            _state = RecontainmentState.DLeftInside;

            Log.Info($"[{Name}] D {_sacrificedClassD.Nickname} left in chamber. State → DLeftInside");

            player.ShowHint(
                "<color=#00aaff><b>📋 ШАГ 2 — РЕКОНТАЙМ SCP-106</b></color>\n" +
                "<color=#ffffff>Класс-D остался в камере.\n<b>Нажмите кнопку</b> на панели управления.</color>",
                8f
            );

            _sacrificedClassD.ShowHint(
                "<color=#ff0000><b>🔒 ВЫ ЗАПЕРТЫ</b></color>\n" +
                "<color=#aaaaaa>Персонал покинул камеру. Ждите...</color>",
                6f
            );
        }
        
        private void OnButtonInteracted(ButtonInteractedEventArgs ev)
        {
            if (ev.Schematic.Name != _buttonSchematicName)
                return;

            Player exiledPlayer = Player.Get(ev.Player.ReferenceHub);
            if (exiledPlayer == null) return;

            if (_state != RecontainmentState.DLeftInside)
            {
                exiledPlayer.ShowHint(
                    "<color=#ff4444><b>❌ РЕКОНТАЙМ НЕВОЗМОЖЕН</b></color>\n" +
                    "<color=#aaaaaa>Сначала заведите класс-D в камеру и выйдите.</color>",
                    5f
                );
                return;
            }

            Log.Info($"[{Name}] Button pressed by {exiledPlayer.Nickname}. Starting sequence.");
            _state = RecontainmentState.SequenceRunning;

            exiledPlayer.ShowHint(
                "<color=#00ff88><b>✅ РЕКОНТАЙМ АКТИВИРОВАН</b></color>\n" +
                "<color=#ffffff>SCP-106 телепортируется в камеру...</color>",
                6f
            );

            Timing.RunCoroutine(RecontainmentSequence());
        }
        
        private IEnumerator<float> RecontainmentSequence()
        {
            Player scp106 = Player.List.FirstOrDefault(p => p.Role.Type == RoleTypeId.Scp106);

            if (scp106 == null)
            {
                Log.Warn($"[{Name}] SCP-106 not found.");
                ResetState();
                yield break;
            }

            yield return Timing.WaitForSeconds(_sequenceDelay);

            if (_containmentSchematic != null)
            {
                scp106.Teleport(_containmentSchematic.Position);
                scp106.ShowHint(
                    "<color=#ff0000><b>⚡ ВЫ ВТЯНУТЫ В КАМЕРУ</b></color>\n" +
                    "<color=#ffffff>Реконтайм активирован...</color>",
                    4f
                );
                Log.Info($"[{Name}] SCP-106 teleported to containment.");
            }
            
            if (_sacrificedClassD != null && _sacrificedClassD.IsAlive) _sacrificedClassD.Teleport(_containmentSchematic.Position);

            yield return Timing.WaitForSeconds(_killDelay);

            if (_sacrificedClassD != null && _sacrificedClassD.IsAlive)
            {
                _sacrificedClassD.ShowHint("<color=#ff0000><b>☠</b></color>", 2f);
                yield return Timing.WaitForSeconds(0.5f);
                _sacrificedClassD.Kill(DamageType.Scp106);
                Log.Info($"[{Name}] Class-D killed.");
            }

            yield return Timing.WaitForSeconds(0.8f);
            
            if (scp106.IsAlive)
            {
                scp106.Kill(DamageType.Unknown);
                Log.Info($"[{Name}] SCP-106 recontained (killed).");
            }

            yield return Timing.WaitForSeconds(0.5f);

            Exiled.API.Features.Cassie.MessageTranslated("scp 1 0 6 has been contaied successfully", "<b><color=red>SCP-106</color> успешно сдержан");

            ResetState();
        }
        
        private void OnPlayerDied(DiedEventArgs ev)
        {
            if (_sacrificedClassD == null || ev.Player != _sacrificedClassD) return;

            Log.Info($"[{Name}] Sacrificed D died externally.");
            _sacrificedClassD = null;

            if (_state == RecontainmentState.DLeftInside)
                ResetState();
        }
        
        private void ResetState()
        {
            _state = RecontainmentState.Idle;
            _sacrificedClassD = null;
            _enteredChamber.Clear();
        }
}