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
        PathFromAudioFolder = string.Empty;
        AddToAudioDict(PlayerSounds.Flap, "ClumsyFlap", 1f);
        AddToAudioDict(PlayerSounds.Collision, "RockCollision", 1f);
    }
}
