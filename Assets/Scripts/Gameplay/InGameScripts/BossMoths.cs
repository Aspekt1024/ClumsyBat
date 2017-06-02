using UnityEngine;
using System.Collections;

public class BossMoths : MonoBehaviour
{
    private MothPool _moths;

    private bool _bPaused;
    private const float ProbabilityGold = 1f;//0.35f;
    private const float MothInterval = 5f;
    private const float MothVariance = 3f;
    private float _timeSinceLastMoth = 2f;
    private int _numCrappyMothsSinceTheOneWeWant;
    private bool bEnabled;

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

    private void Start ()
    {
		_moths = new MothPool();
    }

    public void SpawnInMoth(Moth.MothColour color, Vector2 fromLocation, Vector2 toLocation)
    {
        _moths.ActivateMothFromEssence(fromLocation, toLocation, color, despawnTimer: 4.2f);
    }

    private void SpawnMothFromEssence()
    {
        Vector2 spawnLoc = new Vector2(Random.Range(-7f, 6f), Random.Range(-3f, 3f));
        var mothColour = GetRandomMothColour();
        _moths.ActivateMothFromEssence(spawnLoc, spawnLoc, mothColour, despawnTimer: 4.2f);
    }

    private void SpawnMothFromRight()
    {
        var mothColour = GetRandomMothColour();
        _moths.ActivateMothInRange(-2.5f, 3f, mothColour);
    }

    private Moth.MothColour GetRandomMothColour()
    {
        var mothColour = Random.Range(0f, 1f) < ProbabilityGold ? Moth.MothColour.Gold : Moth.MothColour.Green;
        if (mothColour == Moth.MothColour.Green)
        {
            _numCrappyMothsSinceTheOneWeWant++;
        }
        else
        {
            _numCrappyMothsSinceTheOneWeWant = 0;
        }
        if (_numCrappyMothsSinceTheOneWeWant > 4)
        {
            mothColour = Moth.MothColour.Gold;
        }
        return mothColour;
    }

    public MothPool GetMothPool()
    {
        return _moths;
    }

    private void PauseGame()
    {
        _bPaused = true;
        _moths.PauseGame(true);
    }

    private void ResumeGame()
    {
        _bPaused = false;
        _moths.PauseGame(false);
    }

    public void Enable()
    {
        bEnabled = true;
    }

    public void Disable()
    {
        bEnabled = false;
    }
}
