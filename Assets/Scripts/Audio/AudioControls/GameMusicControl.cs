public class GameMusicControl : AudioController
{
    public enum GameTrack
    {
        Boss,
        Cave,
        Village,
        Mysterious
    }

    protected override void SetupAudioDict()
    {
        PathFromAudioFolder = string.Empty;
        AddToAudioDict(GameTrack.Boss, "doom", 0.5f);
        AddToAudioDict(GameTrack.Cave, "fields", 0.3f);
        AddToAudioDict(GameTrack.Mysterious, "mysterious scene", 0.5f);
        AddToAudioDict(GameTrack.Village, "village theme", 0.5f);
    }
}
