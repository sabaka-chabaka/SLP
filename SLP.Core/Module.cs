using System;
using Exiled.API.Features;

namespace SLP.Core
{
    public class Module
    {
        public virtual string Name { get; set; }
        public virtual Version Version { get; set; }
        public bool IsEnabled { get; set; }

        public virtual void OnEnabled()
        {
            Log.Info($"Module '{Name}' v{Version} is enabled");
        }

        public virtual void OnDisabled()
        {
            Log.Info($"Module '{Name}' v{Version} is disabled");
        }
    }
}