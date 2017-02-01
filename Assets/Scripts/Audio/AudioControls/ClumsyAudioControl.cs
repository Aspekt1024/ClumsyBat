using System;

public class ClumsyAudioControl : AudioController
{
    public enum PlayerSounds
    {
        Flap,
        Flap2,
        Collision 
    }

    protected override void SetupAudioDict()
    {
        AddToAudioDict(PlayerSounds.Flap, "ClumsyFlap", 1f);
        AddToAudioDict(PlayerSounds.Collision, "RockCollision", 1f);
    }

    protected override void SetupAudioProperties()
    {
        PathFromAudioFolder = string.Empty;
        IsOnRepeat = false;
    }
}
