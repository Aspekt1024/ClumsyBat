public class ClumsyAudioControl : AudioController
{
    public enum PlayerSounds
    {
        Flap,
        Flap2,
        Collision 
    }

    protected override void SetupAudioProperties()
    {
        PathFromAudioFolder = string.Empty;
        AudioType = AudioTypes.SoundFx;
        IsOnRepeat = false;
    }

    protected override void SetupAudioDict()
    {
        AddToAudioDict(PlayerSounds.Flap, "ClumsyFlap", 1f);
        AddToAudioDict(PlayerSounds.Collision, "RockCollision", 1f);
    }

}
