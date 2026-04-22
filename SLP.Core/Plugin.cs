using System;
using Exiled.API.Features;

namespace SLP.Core;

public class Plugin : Plugin<Config>
{
    public override string Name => "SLP.Core";
    public override string Author => "sabaka-chabaka";
    public override string Prefix => "slp";
    public override Version Version => new();
    
    private ModuleManager _moduleManager;

    public override void OnEnabled()
    {
        _moduleManager = new ModuleManager([]);
        _moduleManager.Initialize();
        Exiled.Events.Handlers.Server.RestartingRound += OnRestartingRound;
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        _moduleManager = null;
        base.OnDisabled();
    }

    private void OnRestartingRound()
    {
        _moduleManager.ReInit();
    }
}