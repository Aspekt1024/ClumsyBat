using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : MonoBehaviour {

    public BossCreator BossProps;

    private GameObject _boss;
    private readonly List<BossAbility> _abilities = new List<BossAbility>();

    // TODO set this dynamically
    private enum BossStates
    {
        Disabled,
        Active,
        Dead
    }
    private BossStates _state;

    private void OnEnable()
    {
        BossEvents.OnBossFightStart += BossStart;
        BossEvents.OnWait += WaitSeconds;
    }
    private void OnDisable()
    {
        BossEvents.OnBossFightStart -= BossStart;
        BossEvents.OnWait -= WaitSeconds;
    }
    
    private void Start ()
    {
        Toolbox.Instance.Boss = this;  // TODO move this somewhere else? this is so nodes can access it.
        _state = BossStates.Disabled;
        _boss = Instantiate(BossProps.BossPrefab, transform.position, new Quaternion(), transform);
        BossProps.NodeGameSetup(this, _boss);
	}
	
	private void Update () {
        if (_state == BossStates.Disabled) return;
        
	}

    private void BossStart()
    {
        _state = BossStates.Active;
        BossProps.AwakenBoss();
    }

    public void WaitSeconds(float waitTime, BaseNode caller)
    {
        StartCoroutine(Wait(waitTime, caller));
    }

    private IEnumerator Wait(float waitTime, BaseNode caller)
    {
        float timeWaited = 0f;
        while (timeWaited < waitTime)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                timeWaited += Time.deltaTime;
            }
            yield return null;
        }
        ((WaitNode)caller).WaitComplete();  // TODO could be an event instead...?
    }

    public BossAbility GetAbility<T>() where T : BossAbility
    {
        BossAbility ability = null;
        foreach (var a in _abilities)
        {
            if (a.GetType() is T)
                ability = a;
        }

        if (!ability)
        {
            ability = (BossAbility)_boss.AddComponent(typeof(T));
            _abilities.Add(ability);
        }
        return (T)ability;
    }
}
