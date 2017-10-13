using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStalactites : BossAbility {

    private const int NumStals = 20;
    private readonly List<Stalactite> _stals = new List<Stalactite>();
    private GameObject rubblePrefab;
    
    private void Start()
    {
        CreateStals();
        rubblePrefab = Resources.Load<GameObject>("Obstacles/Stalactite/FormingRockEffect");
    }

    public int Spawn(float spawnPosX, SpawnStalAction.StalSpawnDirection direction, SpawnStalAction.StalTypes type, float greenChance = 1, float goldChance = 0, float blueChance = 0) // TODO wow parameters. fix it.
    {
        int index = GetUnusedStalIndex();
        StartCoroutine(SpawnStal(index, spawnPosX, direction, type, greenChance, goldChance, blueChance));
        return index;
    }

    public void DropAllStalactites()
    {
        StartCoroutine(DropStalactites());
    }

    public void Drop(int[] dropOrder = null)
    {
        if (dropOrder == null) return;

        StartCoroutine(DropStalactites(dropOrder));
    }

    private IEnumerator SpawnStal(int index, float spawnPosX, SpawnStalAction.StalSpawnDirection direction, SpawnStalAction.StalTypes type, float greenChance, float goldChance, float blueChance)
    {
        ActivateStal(index, spawnPosX, type, greenChance, goldChance, blueChance);
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
            GameObject rubbleEffect = Instantiate(rubblePrefab);
            rubbleEffect.transform.position = new Vector3(stalTf.position.x, endY, stalTf.position.z - 0.1f);
            StartCoroutine(CheckStalExists(index, rubbleEffect));
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

    private IEnumerator CheckStalExists(int index, GameObject rubbleEffect)
    {
        while (_stals[index].IsActive)
        {
            yield return null;
        }
        Destroy(rubbleEffect);
    }

    private IEnumerator DropStalactites()
    {
        foreach(var stal in _stals)
        {
            if (stal.IsActive && !stal.IsForming)
            {
                stal.Drop();
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private IEnumerator DropStalactites(int[] dropOrder)
    {
        for (int i = 0; i < dropOrder.Length; i++)
        {
            int index = dropOrder[i];
            if (_stals[index].IsActive && !_stals[index].IsForming)
            {
                _stals[index].Drop();
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private void ActivateStal(int index, float spawnPosX, SpawnStalAction.StalTypes type, float greenChance, float goldChance, float blueChance)
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
            TriggerPosX = 0,
            Type = type,
            GreenMothChance = greenChance,
            GoldMothChance = goldChance,
            BlueMothChance = blueChance
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
