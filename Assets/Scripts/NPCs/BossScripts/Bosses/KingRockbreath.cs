using UnityEngine;
using System.Collections.Generic;
using System;

public class KingRockbreath : Boss
{
    private enum Animations
    {
        Idle, Walk, Jump
    }
    private Animations currentAnim;
    private Dictionary<Animations, string> spriteDict = new Dictionary<Animations, string>();
    private Animator anim;

    private enum RockbreathSprites
    {
        Tooth1 = 0, Tooth2, Tooth3, Antlers
    }
    private SpriteRenderer[] sprites;

    private void Start()
    {
        PopulateSpriteDict();
        GetSprites();
        anim = GetComponentInChildren<Animator>();
        anim.Play(spriteDict[Animations.Idle], 0, 0f);
    }

    private void PopulateSpriteDict()
    {
        spriteDict.Add(Animations.Idle, "RockbreathIdle");
        spriteDict.Add(Animations.Walk, "RockbreathWalk");
        spriteDict.Add(Animations.Jump, "RockbreathJump");
    }

    protected override void GetBossComponents()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void GetSprites()
    {
        SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>();
        sprites = new SpriteRenderer[4];
        foreach(SpriteRenderer sprite in allSprites)
        {
            if (sprite.name == "Tooth1")
                sprites[(int)RockbreathSprites.Tooth1] = sprite;
            if (sprite.name == "Tooth2")
                sprites[(int)RockbreathSprites.Tooth2] = sprite;
            if (sprite.name == "Tooth3")
                sprites[(int)RockbreathSprites.Tooth3] = sprite;
            if (sprite.name == "Antlers")
            {
                sprites[(int)RockbreathSprites.Antlers] = sprite;
                bossCollider = sprite.GetComponent<Collider2D>();
            }
            if (sprite.name == "Body")
                bossRenderer = sprite;
        }
    }

    protected override void HealthUpdate()
    {
        // TODO using hard coding for now, but could update this so that damage levels are set in the editor. Depends what Scott wants to implement graphically
        switch (health)
        {
            case 5:
                foreach (SpriteRenderer sprite in sprites)
                    sprite.enabled = true;
                break;
            case 4:
                sprites[(int)RockbreathSprites.Tooth1].enabled = false;
                break;
            case 3:
                sprites[(int)RockbreathSprites.Tooth2].enabled = false;
                break;
            case 2:
                sprites[(int)RockbreathSprites.Tooth3].enabled = false;
                break;
            case 1:
                sprites[(int)RockbreathSprites.Antlers].enabled = false;
                break;
        }
    }

    public override void Walk()
    {
        anim.Play(spriteDict[Animations.Walk], 0, 0f);
    }
    public override void EndWalk()
    {
        anim.Play(spriteDict[Animations.Idle], 0, 0f);
    }

    public override void Jump()
    {
        anim.Play(spriteDict[Animations.Jump], 0, 0f);
    }
    public override void EndJump()
    {
        anim.Play(spriteDict[Animations.Idle], 0, 0f);
    }
}
