using Exiled.API.Interfaces;

namespace SLP.Core;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; } = false;
}
