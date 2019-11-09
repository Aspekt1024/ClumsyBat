using System.Collections.Generic;
using UnityEngine;

namespace ClumsyBat.Players
{
    public class ClumsyAnimator
    {
        private Player player;

        private const float defaultRotation = -13.7f;
        private const float defaultScale = 0.5680108f;
        private const float defaultColliderRadius = 0.54f;

        public enum ClumsyAnimations
        {
            Flap, FlapBlink, Hover, Die, Rush, RushContinuous, WingClose, Land,
            FlapSlower, Perch, Unperch,
        }
        private ClumsyAnimations currentAnimType;

        private struct AnimationData
        {
            public string name;
            public float scale;
            public float rotation;
        }

        private readonly Dictionary<ClumsyAnimations, AnimationData> animDict = new Dictionary<ClumsyAnimations, AnimationData>()
        {
            {ClumsyAnimations.Die, new AnimationData() {name = "Die", scale = 1f, rotation = 0f}},
            {ClumsyAnimations.Flap, new AnimationData() {name = "Flap", scale = 1f, rotation = 0f}},
            {ClumsyAnimations.FlapBlink, new AnimationData() {name = "FlapBlink", scale = 1f, rotation = 0f}},
            {ClumsyAnimations.FlapSlower, new AnimationData() {name = "FlapSlower", scale = 1f, rotation = 0f}},
            {ClumsyAnimations.Perch, new AnimationData() {name = "Perch", scale = 1.28f, rotation = 13.7f}},
            {ClumsyAnimations.Unperch, new AnimationData() {name = "Unperch", scale = 1.1f, rotation = 0f}},
            {ClumsyAnimations.Land, new AnimationData() {name = "Land", scale = 1f, rotation = 0f}},
            {ClumsyAnimations.Hover, new AnimationData() {name = "Hover", scale = 1f, rotation = 0f}},
            {ClumsyAnimations.Rush, new AnimationData() {name = "Rush", scale = .25f, rotation = 0f}},
            {ClumsyAnimations.RushContinuous, new AnimationData() {name = "RushContinuous", scale = .25f, rotation = 0f}},
            {ClumsyAnimations.WingClose, new AnimationData() {name = "WingClose", scale = 1f, rotation = 0f}},
        };

        private AnimationData currentAnimation;
        private float animTimer;

        private Animator anim;
        private Rigidbody2D playerBody;
        private Transform clumsy;

        public ClumsyAnimator(Player player)
        {
            this.player = player;
            playerBody = player.GetComponent<Rigidbody2D>();
            anim = player.GetComponentInChildren<Animator>();
            clumsy = anim.transform;

            anim.enabled = true;
            currentAnimation = animDict[ClumsyAnimations.Flap];
        }

        private void Update()
        {
            if (currentAnimType == ClumsyAnimations.Flap || currentAnimType == ClumsyAnimations.FlapBlink || currentAnimType == ClumsyAnimations.FlapSlower)
            {
                animTimer += Time.deltaTime;
                if (animTimer > 0.7f && Mathf.Abs(playerBody.velocity.y) < 1)
                {
                    PlayAnimation(ClumsyAnimations.WingClose);
                }
            }
        }

        public void PlayAnimation(ClumsyAnimations animId)
        {
            animTimer = 0f;
            if (animId == ClumsyAnimations.Flap)
            {
                if (Random.Range(0f, 1f) > 0.73f)
                    animId = ClumsyAnimations.FlapBlink;
            }
            if (animId == ClumsyAnimations.Die)
            {
                clumsy.localRotation = Quaternion.identity;
            }

            currentAnimType = animId;
            currentAnimation = animDict[animId];
            ChangeScale(currentAnimation.scale);
            ChangeRotation(currentAnimation.rotation);
            anim.Play(currentAnimation.name, 0, 0f);
        }

        private void ChangeScale(float newScale)
        {
            if (player.IsFacingRight)
            {
                clumsy.localScale = new Vector3(defaultScale * newScale, defaultScale * newScale, 1);
            }
            else
            {
                clumsy.localScale = new Vector3(-defaultScale * newScale, defaultScale * newScale, 1);
            }
            clumsy.GetComponent<CircleCollider2D>().radius = defaultColliderRadius / newScale;
        }

        private void ChangeRotation(float newRotation)
        {
            if (player.IsFacingRight)
            {
                clumsy.localRotation = Quaternion.Euler(0, 0, defaultRotation + currentAnimation.rotation);
            }
            else
            {
                clumsy.localRotation = Quaternion.Euler(0, 0, -(defaultRotation + currentAnimation.rotation));
            }
        }
    }
}
