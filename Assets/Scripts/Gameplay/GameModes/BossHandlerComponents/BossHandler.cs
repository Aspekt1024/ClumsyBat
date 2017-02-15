
using UnityEngine;

public class BossHandler : MonoBehaviour {
    // This is a factory class?

    private Boss _boss;
    private readonly Vector3 _bossStartPos = new Vector3(5,0,0);

    public void SpawnLevelBoss(LevelProgressionHandler.Levels level)
    {
        switch (level)
        {
            case LevelProgressionHandler.Levels.Boss1:
                //_boss = gameObject.AddComponent<EvilClumsy>();
                break;
            case LevelProgressionHandler.Levels.Boss2:
                _boss = Instantiate(Resources.Load<KingRockbreath>("NPCs/Bosses/KingRockbreath"), gameObject.transform);
                _boss.transform.position = _bossStartPos;
                break;
        }
    }
}
