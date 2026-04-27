using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;

namespace SLP.Core
{
    public class ModuleManager(List<IProject> projects)
    {
        private readonly List<Module> _modules = [];

        public void Initialize()
        {
            foreach (var project in projects)
            {
                Log.Info($"Loading project {project.Name}");
                foreach (var projectModule in project.Modules)
                {
                    Log.Info($"Loading module '{projectModule.Name}' v{projectModule.Version} from project '{project.Name}'");
                    try
                    {
                        AddModule(projectModule);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                        Timing.CallDelayed(15.0f, () => RestartModule(projectModule));
                    }
                }
            }
        }
        
        private void AddModule(Module module)
        {
            _modules.Add(module);
            EnableModule(module);
        }
        
        private void EnableModule(Module module)
        {
            module.IsEnabled = true;

            module.OnEnabled();
        }

        private void DisableModule(Module module)
        {
            module.OnDisabled();

            module.IsEnabled = false;
        }
        
        private void RestartModule(Module module) { DisableModule(module); EnableModule(module); }
    }
}