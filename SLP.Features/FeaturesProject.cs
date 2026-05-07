using System.Collections.Generic;
using SLP.Core;
using SLP.Features.Decontain;
using SLP.Features.Evacuation;
using SLP.Features.Hints;
using SLP.Features.Names;
using SLP.Features.Respawn;
using SLP.Features.Scp106Cont;

namespace SLP.Features;

public class FeaturesProject : IProject
{
    public List<Module> Modules { get; set; } = [new NamesModule(), new EvacuationModule(), new HintsModule(), new RespawnModule(), new Scp106ContModule(), new DecontainModule()];
    public string Name { get; set; } = "Features";
}