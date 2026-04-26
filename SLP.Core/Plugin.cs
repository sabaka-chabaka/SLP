using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;

namespace SLP.Core;

public class Plugin : Plugin<Config>
{
    public override string Name => "SLP.Core";
    public override string Author => "sabaka-chabaka";
    public override string Prefix => "slp";
    public override Version Version => new(1, 0, 0);
    public override PluginPriority Priority => PluginPriority.High;

    private ModuleManager _moduleManager;

    public static Plugin Instance { get; private set; }
    
    public void SetProjects(List<IProject> projects)
    {
        _moduleManager = new ModuleManager(projects);
        _moduleManager.Initialize();
    }

    public override void OnEnabled()
    {
        Instance = this;
        Exiled.Events.Handlers.Server.RestartingRound += OnRestartingRound;
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Instance = null;
        _moduleManager = null;
        Exiled.Events.Handlers.Server.RestartingRound -= OnRestartingRound;
        base.OnDisabled();
    }

    private void OnRestartingRound()
    {
        _moduleManager.ReInit();
    }
}