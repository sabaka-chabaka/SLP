using System.Collections.Generic;
using SLP.Core;
using SLP.Features.Hints;
using SLP.Features.Pig;

namespace SLP.Features;

public class FeaturesProject : IProject
{
    public List<Module> Modules { get; set; } = [new HintsModule(), new PigModule()];
    public string Name { get; set; } = "Features";
}