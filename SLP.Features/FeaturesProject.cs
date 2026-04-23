using System.Collections.Generic;
using SLP.Core;
using SLP.Features.Names;

namespace SLP.Features;

public class FeaturesProject : IProject
{
    public List<Module> Modules { get; set; } = [new NamesModule()];
    public string Name { get; set; } = "Features";
}