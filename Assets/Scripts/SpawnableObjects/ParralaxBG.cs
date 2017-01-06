using UnityEngine;

enum DepthIndex
{
    Front,
    Mid,
    Rear
}

public class ParralaxBG : MonoBehaviour {

    private float ScrollSpeed = 0;
    public float ZLayer;
    public float tileSizeX = 19.2f;
    public const float FrontBGSpeed = 0.3f;
    public const float MidBGSpeed = 0.1f;
    public const float RearBGSpeed = 0.00f;

    private struct BGImgType
    {
        public SpriteRenderer rendr;
        public DepthIndex depth;
        public int index;
    }

    private const int NumFrontTextures = 3;
    private const int NumMidTextures = 1;
    private const int NumRearTextures = 1;

    private BGImgType[] BGImage = new BGImgType[6];
    private Sprite[] FrontSprites = new Sprite[NumFrontTextures];
    private Sprite[] MidSprites = new Sprite[NumMidTextures];
    private Sprite[] RearSprites = new Sprite[NumRearTextures];

    private Rigidbody2D[] FrontBGPieces = new Rigidbody2D[2];
    private Rigidbody2D[] MidBGPieces = new Rigidbody2D[2];
    private Rigidbody2D[] RearBGPieces = new Rigidbody2D[2];

    void Awake ()
    {
        ZLayer = Toolbox.Instance.ZLayers["Background"];
    }

    void Start()
    {
        transform.position = new Vector3(0, 0, ZLayer);
        GetBGPieces();
        GetBGSprites();
        LoadStartingSprites();
    }

    void Update()
    {
        UpdateBGPos(FrontBGPieces, FrontBGSpeed*Time.deltaTime);
        UpdateBGPos(MidBGPieces, MidBGSpeed*Time.deltaTime);
        UpdateBGPos(RearBGPieces, RearBGSpeed*Time.deltaTime);
    }

    private void UpdateBGPos(Rigidbody2D[] BGList, float BGShift)
    {
        foreach (Rigidbody2D BG in BGList)
        {
            float NewXPos = BG.transform.position.x - ScrollSpeed * BGShift;
            if (NewXPos <= -tileSizeX)
            {
                BG.transform.position = new Vector3(NewXPos + 2 * tileSizeX, 0, BG.transform.position.z);
                SelectNewTexture(BG);
            }
            else
            {
                BG.transform.position = new Vector3(NewXPos, 0, BG.transform.position.z);
            }
        }
    }

    void SelectNewTexture(Rigidbody2D BG)
    {
        foreach (BGImgType BGImg in BGImage)
        {
            if (BGImg.rendr.name == BG.name)
            {
                switch (BGImg.depth)
                {
                    case DepthIndex.Front:
                        BGImg.rendr.sprite = FrontSprites[Random.Range(0, NumFrontTextures)];
                        break;

                    case DepthIndex.Mid:
                        BGImg.rendr.sprite = MidSprites[Random.Range(0, NumMidTextures)];
                        break;

                    case DepthIndex.Rear:
                        BGImg.rendr.sprite = RearSprites[Random.Range(0, NumRearTextures)];
                        break;
                }
            }
        }
    }

    void LoadStartingSprites()
    {
        BGImage[0].rendr.sprite = FrontSprites[Random.Range(0, NumFrontTextures)];
        BGImage[1].rendr.sprite = FrontSprites[Random.Range(0, NumFrontTextures)];
        BGImage[2].rendr.sprite = MidSprites[Random.Range(0, NumMidTextures)];
        BGImage[3].rendr.sprite = MidSprites[Random.Range(0, NumMidTextures)];
        BGImage[4].rendr.sprite = RearSprites[Random.Range(0, NumRearTextures)];
        BGImage[5].rendr.sprite = RearSprites[Random.Range(0, NumRearTextures)];
    }

    public void SetVelocity(float Speed)
    {
        ScrollSpeed = Speed;
    }

    private void GetBGSprites()
    {
        LoadSprites("Front", FrontSprites, NumFrontTextures);
        LoadSprites("Mid", MidSprites, NumMidTextures);
        LoadSprites("Rear", RearSprites, NumRearTextures);
    }

    private void LoadSprites(string BGDepth, Sprite[] BGSprites, int NumSprites)
    {
        for (int index = 0; index < NumSprites; index++)
        {
            string SpritePath = "Backgrounds\\" + BGDepth + "BG_" + (index + 1).ToString();
            BGSprites[index] = (Sprite)Resources.Load(SpritePath, typeof(Sprite));
        }
    }
    
    private void GetBGPieces()
    {
        int FrontIndex = 0;
        int MidIndex = 0;
        int RearIndex = 0;
        foreach (Transform BGPiece in transform)
        {
            switch (BGPiece.name.Substring(0, 1))
            {
                case "F":
                    BGImage[FrontIndex] = SetBGImg(BGPiece.GetComponent<SpriteRenderer>(), DepthIndex.Front);
                    FrontBGPieces[FrontIndex] = BGPiece.GetComponent<Rigidbody2D>();
                    FrontBGPieces[FrontIndex].transform.position = new Vector3(tileSizeX * FrontIndex, 0, transform.position.z + 0);
                    FrontIndex++;
                    break;
                case "M":
                    BGImage[MidIndex + 2] = SetBGImg(BGPiece.GetComponent<SpriteRenderer>(), DepthIndex.Mid);
                    MidBGPieces[MidIndex] = BGPiece.GetComponent<Rigidbody2D>();
                    MidBGPieces[MidIndex].transform.position = new Vector3(tileSizeX * MidIndex, 0, transform.position.z + 1);
                    MidIndex++;
                    break;
                case "R":
                    BGImage[RearIndex + 4] = SetBGImg(BGPiece.GetComponent<SpriteRenderer>(), DepthIndex.Rear);
                    RearBGPieces[RearIndex] = BGPiece.GetComponent<Rigidbody2D>();
                    RearBGPieces[RearIndex].transform.position = new Vector3(tileSizeX * RearIndex, 0, transform.position.z + 2);
                    RearIndex++;
                    break;
            }
        }
    }
    
    private BGImgType SetBGImg(SpriteRenderer SRenderer, DepthIndex depth)
    {
        BGImgType BGImg = new BGImgType();
        BGImg.rendr = SRenderer;
        BGImg.depth = depth;
        BGImg.index = 0;
        return BGImg;
    }

}
