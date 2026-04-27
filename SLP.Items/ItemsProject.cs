using System.Collections.Generic;
using SLP.Core;

namespace SLP.Items
{
    public class ItemsProject : IProject
    {
        public List<Module> Modules { get; set; }
        public string Name { get; set; } = "Items";
    }
}