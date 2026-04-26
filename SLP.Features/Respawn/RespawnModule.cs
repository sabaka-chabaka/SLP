using System;
using SLP.Core;

namespace SLP.Features.Respawn;

public class RespawnModule : Module
{
    public override string Name => "Respawn";
    public override Version Version => new(1, 0, 0);

    public override void OnEnabled()
    {
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
    }
}