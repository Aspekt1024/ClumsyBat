using UnityEngine;

public class BossMoths : MonoBehaviour
{
    private MothPool _moths;

    private bool _bPaused;
    private const float ProbabilityGold = 0.35f;
    private const float MothInterval = 10f;
    private const float MothVariance = 3f;
    private float _timeSinceLastMoth = 2f;
    private int _numCrappyMothsSinceTheOneWeWant;

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
            SpawnMoth();
        }
    }

    private void SpawnMoth()
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
        _moths.ActivateMothInRange(-2.5f, 3f, mothColour);
    }

    public void PauseGame(bool paused)
    {
        _bPaused = paused;
    }
}
