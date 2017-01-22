using UnityEngine;

public class ParralaxBG : MonoBehaviour {

    private enum DepthIndex
    {
        Front, Mid, Rear
    }

    private float _scrollSpeed;
    public float ZLayer;
    public float TileSizeX = 19.2f;
    public const float FrontBgSpeed = 0.3f;
    public const float MidBgSpeed = 0.1f;
    public const float RearBgSpeed = 0.00f;

    private struct BgImgType
    {
        public SpriteRenderer Rendr;
        public DepthIndex Depth;
    }

    private const int NumFrontTextures = 3;
    private const int NumMidTextures = 1;
    private const int NumRearTextures = 1;

    private readonly BgImgType[] _bgImage = new BgImgType[6];
    private readonly Sprite[] _frontSprites = new Sprite[NumFrontTextures];
    private readonly Sprite[] _midSprites = new Sprite[NumMidTextures];
    private readonly Sprite[] _rearSprites = new Sprite[NumRearTextures];

    private readonly Rigidbody2D[] _frontBgPieces = new Rigidbody2D[2];
    private readonly Rigidbody2D[] _midBgPieces = new Rigidbody2D[2];
    private readonly Rigidbody2D[] _rearBgPieces = new Rigidbody2D[2];

    private void Awake ()
    {
        ZLayer = Toolbox.Instance.ZLayers["Background"];
    }

    private void Start()
    {
        transform.position = new Vector3(0, 0, ZLayer);
        GetBgPieces();
        GetBgSprites();
        LoadStartingSprites();
    }

    private void Update()
    {
        UpdateBgPos(_frontBgPieces, FrontBgSpeed*Time.deltaTime);
        UpdateBgPos(_midBgPieces, MidBgSpeed*Time.deltaTime);
        UpdateBgPos(_rearBgPieces, RearBgSpeed*Time.deltaTime);
    }

    private void UpdateBgPos(Rigidbody2D[] bgList, float bgShift)
    {
        foreach (Rigidbody2D bg in bgList)
        {
            float newXPos = bg.transform.position.x - _scrollSpeed * bgShift;
            if (newXPos <= -TileSizeX)
            {
                bg.transform.position = new Vector3(newXPos + 2 * TileSizeX, 0, bg.transform.position.z);
                SelectNewTexture(bg);
            }
            else
            {
                bg.transform.position = new Vector3(newXPos, 0, bg.transform.position.z);
            }
        }
    }

    void SelectNewTexture(Rigidbody2D bg)
    {
        foreach (BgImgType bgImg in _bgImage)
        {
            if (bgImg.Rendr.name == bg.name)
            {
                switch (bgImg.Depth)
                {
                    case DepthIndex.Front:
                        bgImg.Rendr.sprite = _frontSprites[Random.Range(0, NumFrontTextures)];
                        break;

                    case DepthIndex.Mid:
                        bgImg.Rendr.sprite = _midSprites[Random.Range(0, NumMidTextures)];
                        break;

                    case DepthIndex.Rear:
                        bgImg.Rendr.sprite = _rearSprites[Random.Range(0, NumRearTextures)];
                        break;
                }
            }
        }
    }

    void LoadStartingSprites()
    {
        _bgImage[0].Rendr.sprite = _frontSprites[Random.Range(0, NumFrontTextures)];
        _bgImage[1].Rendr.sprite = _frontSprites[Random.Range(0, NumFrontTextures)];
        _bgImage[2].Rendr.sprite = _midSprites[Random.Range(0, NumMidTextures)];
        _bgImage[3].Rendr.sprite = _midSprites[Random.Range(0, NumMidTextures)];
        _bgImage[4].Rendr.sprite = _rearSprites[Random.Range(0, NumRearTextures)];
        _bgImage[5].Rendr.sprite = _rearSprites[Random.Range(0, NumRearTextures)];
    }

    public void SetVelocity(float speed)
    {
        _scrollSpeed = speed;
    }

    private void GetBgSprites()
    {
        LoadSprites("Front", _frontSprites, NumFrontTextures);
        LoadSprites("Mid", _midSprites, NumMidTextures);
        LoadSprites("Rear", _rearSprites, NumRearTextures);
    }

    private void LoadSprites(string bgDepth, Sprite[] bgSprites, int numSprites)
    {
        for (int index = 0; index < numSprites; index++)
        {
            string spritePath = "Backgrounds\\" + bgDepth + "BG_" + (index + 1).ToString();
            bgSprites[index] = (Sprite)Resources.Load(spritePath, typeof(Sprite));
        }
    }
    
    private void GetBgPieces()
    {
        int frontIndex = 0;
        int midIndex = 0;
        int rearIndex = 0;
        foreach (Transform bgPiece in transform)
        {
            switch (bgPiece.name.Substring(0, 1))
            {
                case "F":
                    _bgImage[frontIndex] = SetBGImg(bgPiece.GetComponent<SpriteRenderer>(), DepthIndex.Front);
                    _frontBgPieces[frontIndex] = bgPiece.GetComponent<Rigidbody2D>();
                    _frontBgPieces[frontIndex].transform.position = new Vector3(TileSizeX * frontIndex, 0, transform.position.z + 0);
                    frontIndex++;
                    break;
                case "M":
                    _bgImage[midIndex + 2] = SetBGImg(bgPiece.GetComponent<SpriteRenderer>(), DepthIndex.Mid);
                    _midBgPieces[midIndex] = bgPiece.GetComponent<Rigidbody2D>();
                    _midBgPieces[midIndex].transform.position = new Vector3(TileSizeX * midIndex, 0, transform.position.z + 1);
                    midIndex++;
                    break;
                case "R":
                    _bgImage[rearIndex + 4] = SetBGImg(bgPiece.GetComponent<SpriteRenderer>(), DepthIndex.Rear);
                    _rearBgPieces[rearIndex] = bgPiece.GetComponent<Rigidbody2D>();
                    _rearBgPieces[rearIndex].transform.position = new Vector3(TileSizeX * rearIndex, 0, transform.position.z + 2);
                    rearIndex++;
                    break;
            }
        }
    }
    
    private BgImgType SetBGImg(SpriteRenderer sRenderer, DepthIndex depth)
    {
        BgImgType bgImg = new BgImgType
        {
            Rendr = sRenderer,
            Depth = depth
        };
        return bgImg;
    }

}
