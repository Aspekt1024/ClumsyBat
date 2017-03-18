using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClumsyAnimator : MonoBehaviour {

    public enum ClumsyAnimations
    {
        Flap, FlapBlink, Perch, Hover, Die, Rush, WingClose
    }
    private ClumsyAnimations currentAnimType;

    private struct AnimationData
    {
        public string name;
        public float scale;
        public float rotation;
    }
    private AnimationData death = new AnimationData() { name = "Die", scale = 1f, rotation = 0f };
    private AnimationData flap = new AnimationData() { name = "Flap", scale = 1f, rotation = 0f };
    private AnimationData flapBlink = new AnimationData() { name = "FlapBlink", scale = 1f, rotation = 0f };
    private AnimationData perch = new AnimationData() { name = "Perch", scale = 1f, rotation = 0f };
    private AnimationData hover = new AnimationData() { name = "Hover", scale = 1f, rotation = 0f };
    private AnimationData rush = new AnimationData() { name = "Rush", scale = 1f, rotation = 0f };
    private AnimationData wingClose = new AnimationData() { name = "WingClose", scale = 1f, rotation = 0f };

    private Dictionary<ClumsyAnimations, AnimationData> animDict = new Dictionary<ClumsyAnimations, AnimationData>();

    private AnimationData currentAnimation;
    private float currentScaleModifier;
    //private Quaternion initialRotation; // TODO remove if no rotation modifier is required for clumsy's animations
    private float animTimer;

    private Animator anim;
    private Rigidbody2D playerBody;

    private void OnEnable()
    {
        EventListener.OnPauseGame += PauseGame;
        EventListener.OnResumeGame += ResumeGame;
    }
    private void OnDisable()
    {
        EventListener.OnPauseGame -= PauseGame;
        EventListener.OnResumeGame -= ResumeGame;
    }

    private void Start()
    {
        playerBody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        anim.enabled = true;
        currentAnimation = flap;
        currentScaleModifier = 1f;
        PopulateAnimDict();
    }

    private void Update()
    {
        if (currentAnimType == ClumsyAnimations.Flap || currentAnimType == ClumsyAnimations.FlapBlink)
        {
            animTimer += Time.deltaTime;
            if (animTimer > 0.7f && Mathf.Abs(playerBody.velocity.y) < 1)
            {
                PlayAnimation(ClumsyAnimations.WingClose);
            }
        }
    }

    private void PopulateAnimDict()
    {
        animDict.Add(ClumsyAnimations.Die, death);
        animDict.Add(ClumsyAnimations.Flap, flap);
        animDict.Add(ClumsyAnimations.FlapBlink, flapBlink);
        animDict.Add(ClumsyAnimations.Perch, perch);
        animDict.Add(ClumsyAnimations.Hover, hover);
        animDict.Add(ClumsyAnimations.Rush, rush);
        animDict.Add(ClumsyAnimations.WingClose, wingClose);
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
            transform.localRotation = Quaternion.identity;
        }

        currentAnimType = animId;
        currentAnimation = animDict[animId];
        ChangeScale(currentAnimation.scale);
        anim.Play(currentAnimation.name, 0, 0f);
    }

    private void ChangeScale(float newScale)
    {
        transform.localScale *= newScale / currentScaleModifier;
        currentScaleModifier = newScale;
    }

    private void PauseGame()
    {
        anim.enabled = false;
    }

    private void ResumeGame()
    {
        anim.enabled = true;
    }

    // TODO rotation;
    
}
