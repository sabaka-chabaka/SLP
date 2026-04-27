using System;
using SLP.Core;

namespace SLP.Features.Scp106Cont;

public class Scp106ContModule : Module
{
    public override string Name => "SCP106Containment";
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