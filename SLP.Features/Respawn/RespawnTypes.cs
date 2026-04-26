namespace SLP.Features.Respawn;

public enum WaveType
{
    NineTailedFox,
    HammerDown,
    ChaosInsurgency
}

public record struct Wave(WaveType WaveType, string Announcement, string Subtitles);