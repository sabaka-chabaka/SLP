using System;
using SLP.Core;

namespace SLP.Features.Commands;

public class CommandsModule : Module
{
    public override string Name => "Commands";
    public override Version Version => new(1, 0, 0);
}