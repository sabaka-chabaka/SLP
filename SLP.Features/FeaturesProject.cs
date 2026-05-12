using System.Collections.Generic;
using SLP.Core;
using SLP.Features.Hints;

namespace SLP.Features;

public class FeaturesProject : IProject
{
    public List<Module> Modules { get; set; } = [new HintsModule()];
    public string Name { get; set; } = "Features";
}