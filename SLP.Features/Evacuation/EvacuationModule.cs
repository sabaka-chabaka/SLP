using System;
using SLP.Core;
using UnityEngine;

namespace SLP.Features.Evacuation;

public class EvacuationModule : Module
{
    public override string Name => "Evacuation";
    public override Version Version => new(1, 0, 0);

    public Vector3 HelipadPosition = new(131f, 295f, -43f);
    public Vector3 CarPosition = new(9f, 291f, -42f);

    public float HelipadRadius = 8f;
    public float CheckInterval = 1f;
    public int EvacuationTime = 30;
    
    
}