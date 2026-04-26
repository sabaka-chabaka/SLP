using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using MEC;
using PlayerRoles;
using SLP.Core;

namespace SLP.Features.Respawn;

public class RespawnModule : Module
{
    public override string Name => "Respawn";
    public override Version Version => new(1, 0, 0);
    
    private Spawner _spawner;
    private CoroutineHandle _coroutine;
    
    public override void OnEnabled()
    {
        _spawner = new Spawner();
        _coroutine = Timing.RunCoroutine(CheckWave(Player.List.ToList()));
        Exiled.Events.Handlers.Server.RespawningTeam += OnRespawningTeam;
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_coroutine);
        _spawner = null;
        Exiled.Events.Handlers.Server.RespawningTeam -= OnRespawningTeam;
        base.OnDisabled();
    }

    private void OnRespawningTeam(RespawningTeamEventArgs ev)
    {
        ev.IsAllowed = false;
    }

    private IEnumerator<float> CheckWave(List<Player> players)
    {
        var mtf = players.Where(x => x.Role.Team == Team.FoundationForces);

        var toSpawn = mtf.Count() < 2 ? players.Any(x => x.Role.Team == Team.SCPs) ? WaveType.NineTailedFox : WaveType.HammerDown : WaveType.ChaosInsurgency;

        _spawner.Spawn(GenerateWave(toSpawn));
        
        yield return Timing.WaitForSeconds(300.0f);
    }

    private static Wave GenerateWave(WaveType waveType)
    {
        switch (waveType)
        {
            case WaveType.ChaosInsurgency:
            {
                return new Wave(waveType, "", "");
            }

            case WaveType.HammerDown:
            {
                return new Wave(waveType, "", "");
            }

            case WaveType.NineTailedFox:
            {
                return new Wave(waveType, "", "");
            }
            
            default: return GenerateWave(WaveType.NineTailedFox);
        }
    }
}