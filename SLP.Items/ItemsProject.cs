using System.Collections.Generic;
using SLP.Core;
using SLP.Items.Sniper;

namespace SLP.Items
{
    public class ItemsProject : IProject
    {
        public List<Module> Modules { get; set; } = [new SniperModule()];
        public string Name { get; set; } = "Items";
    }
}