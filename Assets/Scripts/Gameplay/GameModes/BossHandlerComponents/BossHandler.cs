
using UnityEngine;

public class BossHandler : MonoBehaviour {
    // This is a factory class?

    private Boss _boss;
    private readonly Vector3 _bossStartPos = new Vector3(5,0,0);

    public void SpawnLevelBoss(LevelProgressionHandler.Levels level)
    {
        string bossName = string.Empty;
        switch (level)
        {
            case LevelProgressionHandler.Levels.Boss1:
                bossName = "EvilClumsy";
                break;
            case LevelProgressionHandler.Levels.Boss2:
                bossName = "KingRockbreath";
                break;
            default:
                Debug.Log("Unable to load boss for level " + level.ToString());
                return;
        }

        _boss = Instantiate(Resources.Load<EvilClumsy>("NPCs/Bosses/" + bossName), gameObject.transform);
        _boss.transform.position = _bossStartPos;
    }
}
