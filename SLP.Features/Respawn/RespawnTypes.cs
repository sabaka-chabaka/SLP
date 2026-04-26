namespace SLP.Features.Respawn;

enum WaveType
{
    NineTailedFox,
    HammerDown,
    ChaosInsurgency
}

record struct Wave(WaveType WaveType, string Announcement, string Subtitles);