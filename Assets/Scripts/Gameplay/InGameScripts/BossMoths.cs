using UnityEngine;
using System.Collections;

public class BossMoths : MonoBehaviour
{
    private MothPool _moths;

    private bool _bPaused;
    private const float ProbabilityGold = 0.35f;
    private const float MothInterval = 5f;
    private const float MothVariance = 3f;
    private float _timeSinceLastMoth = 2f;
    private int _numCrappyMothsSinceTheOneWeWant;

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
	
	private void Update ()
    {
		if (_bPaused) { return; }
        _timeSinceLastMoth += Time.deltaTime;
        if (_timeSinceLastMoth > MothInterval)
        {
            _timeSinceLastMoth = 0f + Random.Range(-MothVariance, MothVariance);
            SpawnMothFromEssence();
        }
    }

    private void SpawnMothFromEssence()
    {
        Vector2 spawnLoc = new Vector2(Random.Range(-7f, 6f), Random.Range(-3f, 3f));
        var mothColour = GetRandomMothColour();
        _moths.ActivateMothFromEssence(spawnLoc, mothColour, despawnTimer: 4.2f);
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
}
