using ClumsyBat.Objects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClumsyBat;
using UnityEngine;

public class SpawnStalactites : BossAbility {

    private const int InitialStalCount = 5;
    private readonly List<Stalactite> stals = new List<Stalactite>();
    private GameObject rubblePrefab;

    private Transform stalParent;
    
    private void Start()
    {
        CreateStals();
        rubblePrefab = Resources.Load<GameObject>("Obstacles/Stalactite/FormingRockEffect");
    }

    public int Spawn(float spawnPosX, SpawnStalAction.StalSpawnDirection direction, SpawnStalAction.StalTypes type, float greenChance = 1, float goldChance = 0, float blueChance = 0, int poolIndex = -1) // TODO wow parameters. fix it.
    {
        int index = GetUnusedStalIndex();
        StartCoroutine(SpawnStal(index, spawnPosX, direction, type, greenChance, goldChance, blueChance, poolIndex));
        return index;
    }

    public override void Clear()
    {
        StopAllCoroutines();
        stals.Clear();
        Destroy(stalParent.gameObject);
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

    private IEnumerator SpawnStal(int index, float spawnPosX, SpawnStalAction.StalSpawnDirection direction, SpawnStalAction.StalTypes type, float greenChance, float goldChance, float blueChance, int poolIndex)
    {
        GameStatics.Audio.Enemy.PlaySound(EnemySounds.StalactiteForm);
        ActivateStal(index, spawnPosX, type, greenChance, goldChance, blueChance, poolIndex, direction);
        var stalTf = stals[index].transform;
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
            Destroy(rubbleEffect, 5f);
        }

        const float animDuration = 1.5f;
        float animTimer = 0f;

        stals[index].SetState(Stalactite.StalStates.Forming);
        while (animTimer < animDuration)
        {
            if (stals[index].IsBroken) yield break;

            if (!Toolbox.Instance.GamePaused)
            {
                animTimer += Time.deltaTime;
                float yPos = startY - (startY - endY) * (animTimer / animDuration);
                stalTf.position = new Vector3(stalTf.position.x, yPos, stalTf.position.z);
            }
            yield return null;
        }

        if (!stals[index].IsBroken)
        {
            stals[index].SetState(Stalactite.StalStates.Normal);
        }
    }

    private IEnumerator DropStalactites()
    {
        var activeStals = stals.Where(s => s.IsActive && !s.IsForming && !s.IsBroken).ToArray();
        foreach(var stal in activeStals)
        {
            stal.Drop();
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator DropStalactites(int[] dropOrder)
    {
        foreach (var index in dropOrder)
        {
            if (!stals[index].IsActive || stals[index].IsForming) continue;
            
            stals[index].Drop();
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void ActivateStal(int index, float spawnPosX, SpawnStalAction.StalTypes type, float greenChance, float goldChance, float blueChance, int poolHandlerIndex, SpawnStalAction.StalSpawnDirection direction)
    {
        stals[index].gameObject.SetActive(true);
        const float startY = 10f;
        var spawnTf = new Spawnable.SpawnType
        {
            Pos = new Vector2(spawnPosX, startY),
            Rotation = new Quaternion(),
            Scale = Vector2.one
        };
        var stalProps = new StalPool.StalType()
        {
            SpawnTransform = spawnTf,
            DropEnabled = false,
            TriggerPosX = 0,
            Type = type,
            GreenMothChance = greenChance,
            GoldMothChance = goldChance,
            BlueMothChance = blueChance,
            PoolHandlerIndex = poolHandlerIndex,
            Direction = direction
        };
        stals[index].Spawn(stalProps, 0);
    }

    private int GetUnusedStalIndex()
    {
        int i = 0;
        for (i = 0; i < stals.Count - 1; i++)
        {
            if (!stals[i].IsActive) return i;
        }
        AddNewStal();
        return stals.Count - 1;
    }


    private void CreateStals()
    {
        stalParent = new GameObject("Stalactites").transform;
        stalParent.position = new Vector3(0f, 0f, Toolbox.Instance.ZLayers["Stalactite"]);
        for (int i = 0; i < InitialStalCount; i++)
        {
            AddNewStal();
        }
    }

    private void AddNewStal()
    {
        var newStal = Instantiate(Resources.Load<Stalactite>("Obstacles/Stalactite"), stalParent);
        newStal.transform.position = Toolbox.Instance.HoldingArea;
        newStal.DropEnabled = false;
        newStal.gameObject.SetActive(false);
        stals.Add(newStal);
    }
}
