using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProperties : MonoBehaviour {

    public BossCreator BossProps;
    
    private void Awake()
    {
        Boss theBoss = Instantiate(Resources.Load<Boss>("NPCs/Bosses/" + BossProps.BossObject.name));
    }

    private void Update()
    {

    }
}
