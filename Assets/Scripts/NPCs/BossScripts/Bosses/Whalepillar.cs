using UnityEngine;
using System.Collections.Generic;
using System;

public class Whalepillar : Boss
{
    private enum Animations
    {
        Idle, Walk, Jump
    }
    private Animations currentAnim;
    private Dictionary<Animations, string> spriteDict = new Dictionary<Animations, string>();
    private Animator anim;

    private SpriteRenderer bodySprite;
    private HeadPiece head;

    private enum RockbreathSprites
    {
        Tooth1 = 0, Tooth2, Tooth3, Antlers
    }
    private SpriteRenderer[] sprites;

    public override void FaceDirection(Direction dir)
    {
        if (dir == Direction.Left && bodySprite.flipX)
        {
            bodySprite.flipX = false;
        }
        else if (dir == Direction.Right && !bodySprite.flipX)
        {
            bodySprite.flipX = true;
        }
    }

    private void Start()
    {
        PopulateSpriteDict();
        anim = GetComponentInChildren<Animator>();
        //anim.Play(spriteDict[Animations.Idle], 0, 0f);
    }

    private void PopulateSpriteDict()
    {
        //spriteDict.Add(Animations.Idle, "RockbreathIdle");
        //spriteDict.Add(Animations.Walk, "RockbreathWalk");
        //spriteDict.Add(Animations.Jump, "RockbreathJump");
    }

    protected override void GetBossComponents()
    {
        foreach (Transform tf in transform)
        {
            if (tf.name == "Body")
            {
                foreach (Transform t in tf)
                {
                    if (t.name == "Head")
                    {
                        head = t.GetComponent<HeadPiece>();
                        Body = t.GetComponent<Rigidbody2D>();
                        bodySprite = t.GetComponent<SpriteRenderer>();
                        head.SetBossScript(this);
                    }
                }
            }
        }
    }

    protected override void HealthUpdate()
    {
        // TODO using hard coding for now, but could update this so that damage levels are set in the editor
        switch (health)
        {
            case 5:
                break;
        }
    }
    
    public override void Walk()
    {
        //anim.Play(spriteDict[Animations.Walk], 0, 0f);
    }
    public override void EndWalk()
    {
        //anim.Play(spriteDict[Animations.Idle], 0, 0f);
    }

    public override void Jump()
    {
        //anim.Play(spriteDict[Animations.Jump], 0, 0f);
    }
    public override void EndJump()
    {
        //anim.Play(spriteDict[Animations.Idle], 0, 0f);
    }

    protected override void PauseGame()
    {
        base.PauseGame();
        //anim.enabled = false;
    }

    protected override void ResumeGame()
    {
        base.ResumeGame();
        //anim.enabled = true;
    }
}
