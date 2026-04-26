using System.Collections.Generic;
using SLP.Core;
using SLP.Features.Evacuation;
using SLP.Features.Hints;
using SLP.Features.Names;

namespace SLP.Features;

public class FeaturesProject : IProject
{
    public List<Module> Modules { get; set; } = [new NamesModule(), new EvacuationModule(), new HintsModule()];
    public string Name { get; set; } = "Features";
}