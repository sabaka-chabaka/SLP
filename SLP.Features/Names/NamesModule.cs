using System;
using Exiled.Events.EventArgs.Player;
using SLP.Core;

namespace SLP.Features.Names
{
    public class NamesModule : Module
    {
        public override string Name => "Names";
        public override Version Version => new(1, 0, 0);

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
            base.OnDisabled();
        }

        private void OnChangingRole(ChangingRoleEventArgs e)
        {
            //TODO: implement onchangingrole
        }
    }
}