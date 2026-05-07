using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using LightContainmentZoneDecontamination;
using MEC;
using SLP.Core;

namespace SLP.Features.Decontain;

public class DecontainModule : Module
{
    public override string Name => "Decontainment";
    public override Version Version => new(1, 0, 0);
    private CoroutineHandle _coroutineLcz;
    
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Server.RoundStarted += RoundStarted;
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Server.RoundStarted -= RoundStarted;
        base.OnDisabled();
    }
    
    private void RoundStarted()
    {
        Timing.KillCoroutines(_coroutineLcz);
        Server.RunCommand("decont disable");
    
        _coroutineLcz = Timing.RunCoroutine(CoroutineLcz());
    }

    private IEnumerator<float> CoroutineLcz()
    {
        yield return Timing.WaitForSeconds(20*60);
        Server.RunCommand("decont enable");
        DecontaminationController.Singleton.DecontaminationOverride = DecontaminationController.DecontaminationStatus.Forced;
        Exiled.API.Features.Cassie.MessageTranslated(
            message: "decontamination sequence in Light Containment Zone has been manually activated",
            translation: "<size=25><b>Последовательность обеззараживания в Зоне Лёгкого Содержания была принудительно запущена"
        );
    }
}