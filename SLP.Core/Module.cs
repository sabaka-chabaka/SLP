using System;
using System.Collections.Generic;

namespace SLP.Core
{
    public class Module
    {
        public string Name { get; set; }
        public Version Version { get; set; }
        public bool IsEnabled { get; set; }
        
        public List<Action> OnEnable { get; set; } = [];
        public List<Action> OnDisable { get; set; } = [];
    }
}