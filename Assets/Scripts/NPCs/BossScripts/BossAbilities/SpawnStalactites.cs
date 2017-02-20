using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStalactites : MonoBehaviour {

    private const int NumStals = 3;
    private readonly List<Stalactite> _stals = new List<Stalactite>();

    private Transform _playerTf;

    private void Start()
    {
        CreateStals();
        _playerTf = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Spawn()
    {
        StartCoroutine("SpawnStals");
    }

    public void Drop()
    {
        StartCoroutine("DropStalactites");
    }

    private IEnumerator SpawnStals()
    {
        ActivateStal(1);
        Transform stalTf = _stals[1].transform;

        float startY = stalTf.position.y;
        const float endY = 5f;
        const float animDuration = 1.2f;
        float animTimer = 0f;

        while (animTimer < animDuration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                animTimer += Time.deltaTime;
                float yPos = startY - (startY - endY) * (animTimer / animDuration);
                stalTf.position = new Vector3(stalTf.position.x, yPos, stalTf.position.z);
            }
            yield return null;
        }
    }

    private IEnumerator DropStalactites()
    {
        _stals[1].Drop();
        yield return null;
    }

    private void ActivateStal(int index)
    {
        const float startY = 10f;
        Spawnable.SpawnType spawnTf = new Spawnable.SpawnType
        {
            Pos = new Vector2(_playerTf.position.x, startY),
            Rotation = new Quaternion(),
            Scale = Vector2.one
        };
        _stals[index].Activate(spawnTf, false, Vector2.zero);
    }

    private void CreateStals()
    {
        Transform stalParent = new GameObject("Stalactites").transform;
        stalParent.position = new Vector3(0f, 0f, Toolbox.Instance.ZLayers["Stalactite"]);
        for (int i = 0; i < NumStals; i++)
        {
            Stalactite newStal = Instantiate(Resources.Load<Stalactite>("Obstacles/Stalactite"), stalParent);
            newStal.transform.position = Toolbox.Instance.HoldingArea;
            newStal.UnstableStalactite = false;
            _stals.Add(newStal);
        }
    }
}
