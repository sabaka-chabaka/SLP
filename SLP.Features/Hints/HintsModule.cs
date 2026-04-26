using System;
using SLP.Core;

namespace SLP.Features.Hints;

public class HintsModule : Module
{
    public override string Name => "Hints";
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