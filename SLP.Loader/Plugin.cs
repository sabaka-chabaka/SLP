using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using SLP.Core;
using SLP.Features;
using SLP.Items;

namespace SLP.Loader
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "SLP.Loader";
        public override string Author => "sabaka-chabaka";
        public override string Prefix => "slp_loader";
        public override Version Version => new();
        public override PluginPriority Priority => PluginPriority.Default;

        private readonly List<IProject> _projects = [new FeaturesProject(), new ItemsProject()];

        public override void OnEnabled()
        {
            SLP.Core.Plugin.Instance.SetProjects(_projects);
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
        }
    }
}