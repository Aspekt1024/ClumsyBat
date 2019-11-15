using ClumsyBat;
using ClumsyBat.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossData : MonoBehaviour {

    public StateMachine BossStateMachine;

    private GameObject bossObject;
    private Boss bossScripts;
    private List<BossAbility> _abilities = new List<BossAbility>();

    private Player player;
    
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
        BossEvents.OnBossDeath += OnBossDeath;
    }
    private void OnDisable()
    {
        BossEvents.OnBossFightStart -= BossStart;
        BossEvents.OnBossDeath -= OnBossDeath;
    }
    
    public void LoadBoss(StateMachine bossStateMachine)
    {
        BossStateMachine = bossStateMachine;
        _state = BossStates.Disabled;
        player = GameStatics.Player.Clumsy;
        bossObject = Instantiate(BossStateMachine.BossPrefab, transform.position, new Quaternion(), transform);
        bossScripts = bossObject.GetComponent<Boss>();
        if (bossScripts == null)
            Debug.LogError("Error: no boss script found on boss object");

        bossScripts.SetBaseProperties(BossStateMachine);
        BossStateMachine.StateMachineSetup(this, bossObject);
        Toolbox.Instance.GamePaused = false;    // TODO shouldnt be done here...
    }

    public void ClearBoss()
    {
        BossStateMachine.Stop();
        BossStateMachine = null;
        
        foreach (var ability in _abilities)
        {
            ability.Clear();
        }
        
        _abilities = new List<BossAbility>();
        
        if (bossObject != null)
        {
            Destroy(bossObject);
        }

    }
	
	private void Update () {
        if (_state == BossStates.Disabled || Toolbox.Instance.GamePaused) return;

        foreach(var action in BossStateMachine.Actions)
        {
            action.Tick(Time.deltaTime);
        }
	}

    private void BossStart()
    {
        _state = BossStates.Active;
        BossStateMachine.AwakenBoss();
    }
    
    public T GetAbility<T>() where T : BossAbility
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
            ability = (BossAbility)bossScripts.Body.gameObject.AddComponent(typeof(T));
            _abilities.Add(ability);
        }
        return (T)ability;
    }

    private void OnBossDeath()
    {
        if (_state == BossStates.Dead) return;
        
        _state = BossStates.Dead;
        StartCoroutine(BossDeathRoutine());
    }

    private IEnumerator BossDeathRoutine()
    {
        BossStateMachine.Stop();
        if (!player.State.IsAlive) yield break;
        
        yield return new WaitForSeconds(2.5f);
        
        GameStatics.LevelManager.GameHandler.LevelComplete();
    }
}
