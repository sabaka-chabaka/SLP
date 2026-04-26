using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Utilities;
using MEC;
using PlayerRoles;
using SLP.Core;
using HSMHint = HintServiceMeow.Core.Models.Hints.Hint;

namespace SLP.Features.Hints;

public class HintsModule : Module
{
    public override string Name => "Hints";
    public override Version Version => new(1, 0, 0);
    
    public readonly Dictionary<Player, CoroutineHandle> ActiveHints = new();
    
    private Dictionary<RoleTypeId, string> RoleTranslations { get; set; } = new()
    {
        {
            RoleTypeId.ClassD,
            "Персонал класса D"
        },
        {
            RoleTypeId.Scientist,
            "Научный сотрудник"
        },
        {
            RoleTypeId.FacilityGuard,
            "Офицер безопасности"
        },
        {
            RoleTypeId.NtfCaptain,
            "Капитан МОГ"
        },
        {
            RoleTypeId.NtfPrivate,
            "Рядовой МОГ"
        },
        {
            RoleTypeId.NtfSergeant,
            "Сержант МОГ"
        },
        {
            RoleTypeId.NtfSpecialist,
            "Специалист МОГ"
        },
        {
            RoleTypeId.ChaosConscript,
            "Новичок ПХ"
        },
        {
            RoleTypeId.ChaosMarauder,
            "Марадёр ПХ"
        },
        {
            RoleTypeId.ChaosRepressor,
            "Репрессор ПХ"
        },
        {
            RoleTypeId.ChaosRifleman,
            "Стрелок ПХ"
        },
        {
            RoleTypeId.Tutorial,
            "Туториал"
        },
        {
            RoleTypeId.Spectator,
            "Наблюдатель"
        },
        {
            RoleTypeId.Overwatch,
            "Надзиратель"
        }
    };
    
    public void OnSpawned(SpawnedEventArgs ev)
    {
        var player = ev.Player;

        var handle = Timing.RunCoroutine(DisplayHintLoop(player));
        ActiveHints[player] = handle;
    }
    
    public void OnChangingRole(ChangingRoleEventArgs ev)
    {
        var player = ev.Player;

        if (ActiveHints.TryGetValue(player, out var handle))
        {
            Timing.KillCoroutines(handle);
            ActiveHints.Remove(player);
        }

        PlayerDisplay.Get(player).ClearHint();
    }

    private IEnumerator<float> DisplayHintLoop(Player player)
    {
        var color = "#996633";
        var display = PlayerDisplay.Get(player);

        while (true)
        {
            display.ClearHint();

            var hintName = new HSMHint
            {
                Text = $"<b><size=26><color={color}>{player.DisplayNickname}</color></size></b>",
                FontSize = 25,
                YCoordinate = 1000,
                YCoordinateAlign = HintVerticalAlign.Bottom,
                Alignment = HintAlignment.Center
            };

            var hintDesc = new HSMHint
            {
                Text =
                    "<size=25><b>[<color=white>R</color><color=blue>U</color><color=red>S</color>] <color=#45FE07>С</color><color=#50EE07>а</color><color=#5BDE07>б</color><color=#66CE07>а</color><color=#71BE07>к</color><color=#7CAE07>а</color><color=#879E07>Р</color><color=#928E07>П</color> <color=#A86E07>|</color> <color=#BE4E07>M</color><color=#C93E07>e</color><color=#D42E07>d</color><color=#DF1E07>i</color><color=#EA0E07>u</color><color=#F50007>m</color></b></size>",
                FontSize = 23,
                YCoordinate = 1060,
                YCoordinateAlign = HintVerticalAlign.Bottom,
                Alignment = HintAlignment.Center
            };

            var hintAge = new HSMHint
            {
                Text = player.CustomInfo,
                FontSize = 23,
                YCoordinate = 1030,
                YCoordinateAlign = HintVerticalAlign.Bottom,
                Alignment = HintAlignment.Center
            };

            var hintRole = new HSMHint
            {
                Text = RoleTranslations[player.Role.Type],
                FontSize = 23,
                YCoordinate = 970,
                YCoordinateAlign = HintVerticalAlign.Bottom,
                Alignment = HintAlignment.Center
            };

            display.AddHint(hintDesc);
            display.AddHint(hintName);
            display.AddHint(hintAge);
            display.AddHint(hintRole);

            var index = player.DisplayNickname.IndexOf('c');
            if (index != -1 && player.DisplayNickname.IndexOf('o') == index + 1 &&
                player.DisplayNickname.IndexOf('l') == index + 2)
            {
                player.DisplayNickname = player.DisplayNickname.Remove(index);
            }

            yield return Timing.WaitForSeconds(1f);
        }
    }

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
        Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
            
        foreach (var v in ActiveHints.Values)
            Timing.KillCoroutines(v);
        
        base.OnDisabled();
    }
}