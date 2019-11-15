using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossAbility : MonoBehaviour {
    
    public virtual void Pause() { }
    public virtual void Resume() { }
    public virtual void Clear() { }
    
    public void Interrupt()
    {
        StopAllCoroutines();
    }
}
