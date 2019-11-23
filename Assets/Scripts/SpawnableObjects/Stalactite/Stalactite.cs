using UnityEngine;
using System.Collections;

namespace ClumsyBat.Objects
{
    // TODO there are really two different objects described in this and they should be split up.
    // They work fine, but i don't like it. Create a base class
    public class Stalactite : Spawnable
    {
        public bool DropEnabled;
        [Range(2.5f, 7.5f)]
        public float TriggerPosX;

        public SpawnStalAction.StalTypes Type;

        public enum StalStates
        {
            Normal, Falling, Exploding, Forming, Broken
        }
        private StalStates state;

        private Collider2D stalCollider;
        private SpriteRenderer stalRenderer;
        private StalAnimationHandler anim;
        private StalDropComponent dropControl;
        private bool isExploding;

        private const string unbrokenStalPath = "Obstacles/Stalactite/UnbrokenStal";
        private const string brokenStalPath = "Obstacles/Stalactite/BrokenStal";
        private const string unbrokenCrystalPath = "Obstacles/Stalactite/UnbrokenCrystal";
        private const string brokenCrystalPath = "Obstacles/Stalactite/BrokenCrystal";
        private GameObject stalUnbroken;
        private GameObject stalBroken;

        private GameObject stalPrefabUnbroken;
        private GameObject stalPrefabBroken;
        private GameObject crystalPrefabBroken;
        private GameObject crystalPrefabUnbroken;

        private Transform moth;
        private Animator mothAnim;
        private Moth.MothColour color;
        private float greenMothChance;
        private float goldMothChance;
        private float blueMothChance;

        private int poolHandlerIndex;
        private SpawnStalAction.StalSpawnDirection direction;

        private void Awake()
        {
            GetStalComponents();

            stalRenderer.enabled = false;   // used for editor only.
            anim.enabled = true;
        }
        
        private void FixedUpdate()
        {
            if (Type == SpawnStalAction.StalTypes.Crystal)
                moth.Rotate(Vector3.back, 64 * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (state == StalStates.Exploding || state == StalStates.Broken) return;

            if (other.tag == "Boss")
            {
                Break();
            }
            else if (other.tag == "Player" && Type == SpawnStalAction.StalTypes.Crystal)
            {
                Break();
                GameStatics.Objects.ObjectHandler.Moths.CollectMothFromCrystal(moth.transform.position, color);
            }
        }

        private void GetStalComponents()
        {
            crystalPrefabUnbroken = Resources.Load<GameObject>(unbrokenCrystalPath);
            crystalPrefabBroken = Resources.Load<GameObject>(brokenCrystalPath);
            stalPrefabUnbroken = Resources.Load<GameObject>(unbrokenStalPath);
            stalPrefabBroken = Resources.Load<GameObject>(brokenStalPath);

            stalCollider = GetComponent<PolygonCollider2D>();
            stalRenderer = GetComponent<SpriteRenderer>();
            body = GetComponent<Rigidbody2D>();
            dropControl = gameObject.AddComponent<StalDropComponent>();
            anim = gameObject.AddComponent<StalAnimationHandler>();
        }

        private void GetMothComponents()
        {
            foreach (Transform tf in stalUnbroken.transform)
            {
                if (tf.name == "Moth")
                {
                    mothAnim = tf.GetComponent<Animator>();
                    moth = tf;
                    break;
                }
            }
        }

        protected override void Init()
        {
        }

        public void Spawn(StalPool.StalType stalProps, float xOffset = 0)
        {
            gameObject.SetActive(true);
            Type = stalProps.Type;
            isExploding = false;

            if (stalBroken != null) Destroy(stalBroken);
            if (stalUnbroken != null) Destroy(stalUnbroken);

            if (Type == SpawnStalAction.StalTypes.Crystal)
            {
                stalUnbroken = Instantiate(crystalPrefabUnbroken, transform);
                GetMothComponents();
                stalCollider.enabled = false;
            }
            else
            {
                stalUnbroken = Instantiate(stalPrefabUnbroken, transform);
                anim.SetAnimator(stalUnbroken.GetComponent<Animator>());
                anim.NewStalactite();
                dropControl.SetAnim(anim);
                stalCollider.enabled = true;
            }

            stalUnbroken.SetActive(true);
            stalUnbroken.transform.position = transform.position;

            transform.position = stalProps.SpawnTransform.Pos + Vector2.right * xOffset;
            transform.position += Vector3.forward * Toolbox.Instance.ZLayers["Stalactite"];

            transform.localScale = stalProps.SpawnTransform.Scale;
            transform.rotation = stalProps.SpawnTransform.Rotation;
            TriggerPosX = stalProps.TriggerPosX;

            dropControl.NewStalactite();

            DropEnabled = stalProps.DropEnabled;
            greenMothChance = stalProps.GreenMothChance;
            goldMothChance = stalProps.GoldMothChance;
            blueMothChance = stalProps.BlueMothChance;
            poolHandlerIndex = stalProps.PoolHandlerIndex;
            direction = stalProps.Direction;

            if (Type == SpawnStalAction.StalTypes.Crystal)
            {
                ActivateCrystal();
            }
        }

        private void ActivateCrystal()
        {
            color = DetermineMothColor();
            stalUnbroken.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.7f);

            string mothAnimationName = "";
            switch (color)
            {
                case Moth.MothColour.Blue:
                    mothAnimationName = "MothBlueCaptured";
                    break;

                case Moth.MothColour.Gold:
                    mothAnimationName = "MothGoldCaptured";
                    break;

                case Moth.MothColour.Green:
                    mothAnimationName = "MothGreenCaptured";
                    break;
            }

            mothAnim.Play(mothAnimationName, 0, 0f);
        }

        public void DestroyStalactite()
        {
            if (isExploding || state == StalStates.Broken) return;

            isExploding = true;
            stalCollider.enabled = false;
            StartCoroutine(CrumbleAnim());
        }

        private IEnumerator CrumbleAnim()
        {
            anim.Explode();
            stalCollider.enabled = false;
            stalUnbroken.GetComponent<PolygonCollider2D>().enabled = false;
            dropControl.Exploded();

            float timer = 0f;
            float duration = 0.67f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            Deactivate();
        }

        private void Break()
        {
            state = StalStates.Broken;

            if (stalBroken != null)
            {
                Destroy(stalBroken);
            }

            if (Type == SpawnStalAction.StalTypes.Crystal)
            {
                stalBroken = Instantiate(crystalPrefabBroken, transform);
                GameStatics.Audio.Main.PlaySound(MainSounds.BreakCrystal);
            }
            else
            {
                stalBroken = Instantiate(stalPrefabBroken, transform);
                stalBroken.transform.position = transform.position;
            }

            if (stalUnbroken != null) stalUnbroken.SetActive(false);
            if (stalBroken != null) stalBroken.SetActive(true);
            gameObject.GetComponent<Rigidbody2D>().Sleep();
            StartCoroutine(DissolveBrokenStalactite());
        }

        private IEnumerator DissolveBrokenStalactite()
        {
            float timer = 0;
            const float timeBeforeDestroy = 4f;

            while (timer < timeBeforeDestroy)
            {
                if (!Toolbox.Instance.GamePaused)
                    timer += Time.deltaTime;
                yield return null;
            }

            Deactivate();
        }

        public void Crack()
        {
            anim.CrackOnImpact();
            StartCoroutine(Impact());
        }

        private IEnumerator Impact()
        {
            float impactTime = 0;
            const float impactDuration = 0.25f;
            const float impactIntensity = 0.07f;
            const float period = 0.04f;
            bool bForward = true;

            while (impactTime < impactDuration)
            {
                if (!Toolbox.Instance.GamePaused)
                {
                    body.transform.position += new Vector3(bForward ? impactIntensity : -impactIntensity, 0f, 0f);
                    bForward = !bForward;
                    impactTime += period;
                }
                yield return new WaitForSeconds(period);
            }
            if (DropEnabled) { dropControl.Drop(); }
        }

        private Moth.MothColour DetermineMothColor()
        {
            float colorTotals = greenMothChance + goldMothChance + blueMothChance;
            if (colorTotals <= 0) return Moth.MothColour.Green;

            float weightedGreen = greenMothChance / colorTotals;
            float weightedGold = goldMothChance / colorTotals;

            float randomVal = Random.Range(0f, 1f);

            if (randomVal < weightedGreen)
                return Moth.MothColour.Green;
            else if (randomVal < weightedGreen + weightedGold)
                return Moth.MothColour.Gold;
            else
                return Moth.MothColour.Blue;
        }

        public void Drop()
        {
            if (state == StalStates.Broken || state == StalStates.Falling || state == StalStates.Exploding) return;
            dropControl.Drop();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            StalEvents.Destroy(poolHandlerIndex, direction);
            if (stalBroken != null) Destroy(stalBroken);
            if (stalUnbroken != null) Destroy(stalUnbroken);
        }

        public bool IsActive { get { return gameObject.activeSelf; } }
        public bool IsForming { get { return state == StalStates.Forming; } }
        public bool IsFalling { get { return state == StalStates.Falling; } }
        public bool IsBroken { get { return state == StalStates.Broken; } }
        public void SetState(StalStates newState) { state = newState; }
        public SpawnStalAction.StalSpawnDirection SpawnDirection { get { return direction; } }
    }
}