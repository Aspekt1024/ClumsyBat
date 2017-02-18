
using UnityEngine;

public class BossHandler : MonoBehaviour {
    // This is a factory class?

    private Boss _boss;
    private readonly Vector3 _bossStartPos = new Vector3(Toolbox.TileSizeX + 5f,0,0);

    public void SpawnLevelBoss(LevelProgressionHandler.Levels level)
    {
        string bossName = string.Empty;
        switch (level)
        {
            case LevelProgressionHandler.Levels.Boss1:
                bossName = "EvilClumsy";
                _boss = Instantiate(Resources.Load<EvilClumsy>("NPCs/Bosses/" + bossName), gameObject.transform);   // TODO make this generic?
                break;
            case LevelProgressionHandler.Levels.Boss2:
                bossName = "KingRockbreath";
                _boss = Instantiate(Resources.Load<KingRockbreath>("NPCs/Bosses/" + bossName), gameObject.transform);
                break;
            default:
                Debug.Log("Unable to load boss for level " + level.ToString());
                return;
        }

        _boss.transform.position = _bossStartPos;
    }
}
