using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : MonoBehaviour {

    public BossCreator BossProps;

    private GameObject _boss;
    private readonly List<BossAbility> _abilities = new List<BossAbility>();
    
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
    }
    private void OnDisable()
    {
        BossEvents.OnBossFightStart -= BossStart;
    }
    
    private void Start ()
    {
        Toolbox.Instance.Boss = this;  // TODO move this somewhere else? this is so nodes can access it.
        _state = BossStates.Disabled;
        _boss = Instantiate(BossProps.BossPrefab, transform.position, new Quaternion(), transform);
        BossProps.NodeGameSetup(this, _boss);
	}
	
	private void Update () {
        if (_state == BossStates.Disabled || Toolbox.Instance.GamePaused) return;
        
        foreach(var action in BossProps.Actions)
        {
            action.Tick(Time.deltaTime);
        }
	}

    private void BossStart()
    {
        _state = BossStates.Active;
        BossProps.AwakenBoss();
    }

    public BossAbility GetAbility<T>() where T : BossAbility
    {
        BossAbility ability = null;
        foreach (var a in _abilities)
        {
            if (a.GetType() == typeof(T))
            {
                ability = a;
                break;
            }
        }

        if (ability == null)
        {
            ability = (BossAbility)_boss.AddComponent(typeof(T));
            _abilities.Add(ability);
        }
        return (T)ability;
    }
}
