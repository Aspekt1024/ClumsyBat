using UnityEngine;
using System.Collections.Generic;
using System;

public class KingRockbreath : Boss
{
    private enum HealthLevels
    {
        Normal, Injured, Damaged, Critical, Dead
    }
    private HealthLevels healthLevel;
    private Dictionary<HealthLevels, string> spriteDict = new Dictionary<HealthLevels, string>();
    private Animator anim;

    private void Start()
    {
        PopulateSpriteDict();
        anim = GetComponent<Animator>();
        anim.Play(spriteDict[HealthLevels.Normal], 0, 0f);
    }

    private void PopulateSpriteDict()
    {
        spriteDict.Add(HealthLevels.Normal, "RockbreathNormal");
        spriteDict.Add(HealthLevels.Injured, "RockbreathInjured");
        spriteDict.Add(HealthLevels.Damaged, "RockbreathDamaged");
        spriteDict.Add(HealthLevels.Critical, "RockbreathCritical");
        spriteDict.Add(HealthLevels.Dead, "RockbreathDead");
    }

    protected override void HealthUpdate()
    {
        // TODO using hard coding for now, but could update this so that damage levels are set in the editor. Depends what Scott wants to implement graphically
        switch (health)
        {
            case 4:
                anim.Play(spriteDict[HealthLevels.Normal], 0, 0f);
                break;
            case 3:
                anim.Play(spriteDict[HealthLevels.Injured], 0, 0f);
                break;
            case 2:
                anim.Play(spriteDict[HealthLevels.Damaged], 0, 0f);
                break;
            case 1:
                anim.Play(spriteDict[HealthLevels.Critical], 0, 0f);
                break;
            case 0:
                anim.Play(spriteDict[HealthLevels.Dead], 0, 0f);
                break;
        }
    }

}
