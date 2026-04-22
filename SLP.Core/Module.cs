using System;

namespace SLP.Core
{
    public class Module
    {
        public virtual string Name { get; set; }
        public virtual Version Version { get; set; }
        public bool IsEnabled { get; set; }
        
        public virtual void OnEnabled() {}
        public virtual void OnDisabled() {}
    }
}