using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossData : MonoBehaviour {

    public StateMachine BossStateMachine;

    private GameObject bossObject;
    private Boss bossScripts;
    private readonly List<BossAbility> _abilities = new List<BossAbility>();

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
        EventListener.OnPauseGame += PauseGame;
        EventListener.OnResumeGame += ResumeGame;
        BossEvents.OnBossFightStart += BossStart;
        BossEvents.OnBossDeath += Die;
    }
    private void OnDisable()
    {
        EventListener.OnPauseGame -= PauseGame;
        EventListener.OnResumeGame -= ResumeGame;
        BossEvents.OnBossFightStart -= BossStart;
        BossEvents.OnBossDeath -= Die;
    }
    
    private void Start ()
    {
        _state = BossStates.Disabled;
        player = Toolbox.Player;
        bossObject = Instantiate(BossStateMachine.BossPrefab, transform.position, new Quaternion(), transform);
        bossScripts = bossObject.GetComponent<Boss>();
        if (bossScripts == null)
            Debug.LogError("Error: no boss script found on boss object");

        bossScripts.SetBaseProperties(BossStateMachine);
        SetupAbilities();
        BossStateMachine.StateMachineSetup(this, bossObject);
        Toolbox.Instance.GamePaused = false;    // TODO shouldnt be done here...
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

    private void SetupAbilities()
    {
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

    private void Die()
    {
        _state = BossStates.Dead;

        BossStateMachine.Stop();
        if (!player.IsAlive()) return;
        player.GetGameHandler().LevelComplete();
    }

    private void PauseGame()
    {
        foreach(var ability in _abilities)
        {
            ability.Pause();
        }
    }

    private void ResumeGame()
    {
        foreach (var ability in _abilities)
        {
            ability.Resume();
        }
    }
}
