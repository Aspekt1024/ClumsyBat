using ClumsyBat;
using UnityEngine;

public class ParralaxBG : MonoBehaviour {
    
    public BackgroundColour DefaultColour;

    private float prevXPosition;
    private Transform cameraToTrack;

    private enum DepthIndex
    {
        Front, Mid, Rear
    }

    public enum BackgroundColour
    {
        Red, Blue, Green
    }
    
    [HideInInspector] public float ZLayer;
    public const float FrontBgSpeed = 0.7f;
    public const float MidBgSpeed = 0.9f;
    public const float RearBgSpeed = 1f;

    private struct BgImgType
    {
        public SpriteRenderer Rendr;
        public DepthIndex Depth;
    }
    
    private BackgroundColour bgColour;

    private const int NumFrontTextures = 3;
    private const int NumMidTextures = 1;
    private const int NumRearTextures = 1;

    private const float backgroundTileSize = 19.185f;

    private readonly BgImgType[] _bgImage = new BgImgType[6];
    private readonly Sprite[] _frontSprites = new Sprite[NumFrontTextures];
    private readonly Sprite[] _midSprites = new Sprite[NumMidTextures];
    private readonly Sprite[] _rearSprites = new Sprite[NumRearTextures];

    private readonly Rigidbody2D[] _frontBgPieces = new Rigidbody2D[2];
    private readonly Rigidbody2D[] _midBgPieces = new Rigidbody2D[2];
    private readonly Rigidbody2D[] _rearBgPieces = new Rigidbody2D[2];
    
    private void OnDestroy()
    {
        GameStatics.Camera.OnCameraChanged -= CameraChanged;
    }

    private void Start()
    {
        ZLayer = Toolbox.Instance.ZLayers["Background"];
        GameStatics.Camera.OnCameraChanged += CameraChanged;

        cameraToTrack = GameStatics.Camera.CurrentCamera.transform;

        ChooseColourFromLevel(GameStatics.LevelManager.Level);

        transform.position = new Vector3(0, cameraToTrack.position.y, ZLayer);
        GetBgPieces();
        GetBgSprites();
        LoadStartingSprites();
        prevXPosition = cameraToTrack.position.x;
    }

    private void FixedUpdate()
    {
        float dist = cameraToTrack.position.x - prevXPosition;
        prevXPosition = cameraToTrack.position.x;

        UpdateBgPos(_frontBgPieces, FrontBgSpeed * dist);
        UpdateBgPos(_midBgPieces, MidBgSpeed * dist);
        UpdateBgPos(_rearBgPieces, RearBgSpeed * dist);
    }

    private void UpdateBgPos(Rigidbody2D[] bgList, float bgShift)
    {
        foreach (Rigidbody2D bg in bgList)
        {
            Vector3 pos = bg.transform.position;
            pos.y = cameraToTrack.position.y;
            bg.transform.position = pos;

            bg.transform.position += Vector3.right * bgShift;
            if (bg.transform.position.x <= cameraToTrack.position.x - backgroundTileSize * 1.01f)
            {
                bg.transform.position += Vector3.right * 2 * backgroundTileSize;
                SelectNewTexture(bg);
            }
            else if (bg.transform.position.x >= cameraToTrack.position.x + backgroundTileSize * 1.01f)
            {
                bg.transform.position += Vector3.left * 2 * backgroundTileSize;
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
            switch (bgColour)
            {
                case BackgroundColour.Green:
                    spritePath += "_green";
                    break;
                case BackgroundColour.Blue:
                    spritePath += "_blue";
                    break;
            }
            bgSprites[index] = (Sprite)Resources.Load(spritePath, typeof(Sprite));
        }
    }
    
    private void GetBgPieces()
    {
        int frontIndex = 0;
        int midIndex = 0;
        int rearIndex = 0;

        float yPos = 0f;
        foreach (Transform bgPiece in transform)
        {
            switch (bgPiece.name.Substring(0, 1))
            {
                case "F":
                    _bgImage[frontIndex] = SetBGImg(bgPiece.GetComponent<SpriteRenderer>(), DepthIndex.Front);
                    _frontBgPieces[frontIndex] = bgPiece.GetComponent<Rigidbody2D>();
                    _frontBgPieces[frontIndex].transform.position = new Vector3(backgroundTileSize * frontIndex, yPos, transform.position.z + 0);
                    frontIndex++;
                    break;
                case "M":
                    _bgImage[midIndex + 2] = SetBGImg(bgPiece.GetComponent<SpriteRenderer>(), DepthIndex.Mid);
                    _midBgPieces[midIndex] = bgPiece.GetComponent<Rigidbody2D>();
                    _midBgPieces[midIndex].transform.position = new Vector3(backgroundTileSize * midIndex, yPos, transform.position.z + 1);
                    midIndex++;
                    break;
                case "R":
                    _bgImage[rearIndex + 4] = SetBGImg(bgPiece.GetComponent<SpriteRenderer>(), DepthIndex.Rear);
                    _rearBgPieces[rearIndex] = bgPiece.GetComponent<Rigidbody2D>();
                    _rearBgPieces[rearIndex].transform.position = new Vector3(backgroundTileSize * rearIndex, yPos, transform.position.z + 2);
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

    private void ChooseColourFromLevel(LevelProgressionHandler.Levels level)
    {
        if (level <= LevelProgressionHandler.Levels.Boss2)
        {
            bgColour = BackgroundColour.Red;
        }
        else if (level <= LevelProgressionHandler.Levels.Boss4)
        {
            bgColour = BackgroundColour.Blue;
        }
        else
        {
            bgColour = BackgroundColour.Green;
        }
    }

    private void CameraChanged(Camera newCam)
    {
        cameraToTrack = newCam.transform;
        prevXPosition = newCam.transform.position.x;
        transform.position = new Vector3(newCam.transform.position.x, cameraToTrack.position.y, ZLayer);
    }
}
