using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : MonoBehaviour {
    
    private GameObject _boss;
    private BossCreator _props;
    private int _health;
    private readonly List<BossAbility> _abilities = new List<BossAbility>();

    // TODO set this dynamically
    private enum BossStates
    {
        Disabled,
        Idle,
        Jumping,
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
        _state = BossStates.Disabled;
        _props = GetComponent<BossProperties>().BossProps;
        _boss = Instantiate(_props.BossPrefab, transform.position, new Quaternion(), transform);
        AddAbilities();
	}
	
	private void Update () {
        //if (_state == BossStates.Disabled) return;    // TODO re-enable once we're done testing
        
        foreach(var ability in _abilities)
        {
            if (ability.GetType().ToString() == "JumpPound" && _state == BossStates.Idle)
            {
                ability.Activate();
                _state = BossStates.Jumping;
            }
        }
	}

    private void BossStart()
    {
        _state = BossStates.Idle;
    }

    private void AddAbilities()
    {
        foreach (UnityEditor.MonoScript ability in _props.AbilitySet)
        {
            _abilities.Add((BossAbility)_boss.AddComponent(ability.GetClass()));
        }
    }
}
