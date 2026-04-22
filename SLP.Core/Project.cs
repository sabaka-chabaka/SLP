using System.Collections.Generic;

namespace SLP.Core;

public interface IProject
{
    public List<Module> Modules { get; set; }
    public string Name { get; set; }
}