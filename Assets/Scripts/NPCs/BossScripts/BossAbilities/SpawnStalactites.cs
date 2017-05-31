using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStalactites : BossAbility {

    private const int NumStals = 20;
    private readonly List<Stalactite> _stals = new List<Stalactite>();
    
    private void Start()
    {
        CreateStals();
    }

    public void Spawn(float spawnPosX, SpawnStalAction.StalSpawnDirection direction)
    {
        StartCoroutine(SpawnStal(spawnPosX, direction));
    }

    public void Drop()
    {
        StartCoroutine(DropStalactites());
    }

    private IEnumerator SpawnStal(float spawnPosX, SpawnStalAction.StalSpawnDirection direction)
    {
        int index = GetUnusedStalIndex();
        ActivateStal(index, spawnPosX);
        Transform stalTf = _stals[index].transform;
        stalTf.localRotation = new Quaternion();
        
        float startY = stalTf.position.y;
        float endY = 5f;
        if (direction == SpawnStalAction.StalSpawnDirection.FromBottom)
        {
            startY = -10f;
            endY = -5f;
            stalTf.Rotate(Vector3.forward, 180f);
        }
        else
        {
            GameObject rubbleEffect = Instantiate(Resources.Load<GameObject>("Obstacles/Stalactite/FormingRockEffect"));
            rubbleEffect.transform.position = new Vector3(stalTf.position.x, endY, stalTf.position.z - 0.1f);
            Destroy(rubbleEffect, 5f);
        }

        const float animDuration = 1.5f;
        float animTimer = 0f;

        _stals[index].SetState(Stalactite.StalStates.Forming);
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
        _stals[index].SetState(Stalactite.StalStates.Normal);
    }

    private IEnumerator DropStalactites()
    {
        foreach(var stal in _stals)
        {
            if (stal.IsActive && !stal.IsForming())
            {
                stal.Drop();
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private void ActivateStal(int index, float spawnPosX)
    {
        const float startY = 10f;
        Spawnable.SpawnType spawnTf = new Spawnable.SpawnType
        {
            Pos = new Vector2(spawnPosX, startY),
            Rotation = new Quaternion(),
            Scale = Vector2.one
        };
        StalPool.StalType stalProps = new StalPool.StalType()
        {
            SpawnTransform = spawnTf,
            DropEnabled = false,
            TriggerPosX = 0
        };
        _stals[index].Activate(stalProps, 0);
    }

    private int GetUnusedStalIndex()
    {
        int i = 0;
        for (i = 0; i < _stals.Count - 1; i++)
        {
            if (!_stals[i].IsActive)
                break;
        }
        return i;
    }


    private void CreateStals()
    {
        Transform stalParent = new GameObject("Stalactites").transform;
        stalParent.position = new Vector3(0f, 0f, Toolbox.Instance.ZLayers["Stalactite"]);
        for (int i = 0; i < NumStals; i++)
        {
            Stalactite newStal = Instantiate(Resources.Load<Stalactite>("Obstacles/Stalactite"), stalParent);
            newStal.transform.position = Toolbox.Instance.HoldingArea;
            newStal.DropEnabled = false;
            _stals.Add(newStal);
        }
    }

    public override void Pause()
    {
        foreach (var stal in _stals)
        {
            stal.PauseGame(true);
        }
    }

    public override void Resume()
    {
        foreach (var stal in _stals)
        {
            stal.PauseGame(false);
        }
    }
}
