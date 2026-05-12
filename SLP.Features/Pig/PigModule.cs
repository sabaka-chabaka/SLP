using System;
using SLP.Core;

namespace SLP.Features.Pig;

public class PigModule : Module
{
    public override string Name => "Pig";
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