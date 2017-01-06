using UnityEngine;
using System.Collections.Generic;

public class Spawnable : MonoBehaviour {

    public string Path;
    public int NumObjects;
    public int Index;

    public List<Rigidbody2D> ObjectPool = new List<Rigidbody2D>();

    public Spawnable()
    {
        Path = string.Empty;
        NumObjects = 0;
        Index = 0;
    }

    void Awake()
    {
    }

    public void SetObjectName(string name)
    {
        Path = name;
    }

    public void IncrementIndex()
    {
        Index++;
        if (Index == NumObjects)
        {
            Index = 0;
        }
    }

    public void CreateObjectPool()
    {
        if (Path == string.Empty)
        {
            Debug.LogError("Object type not set!");
            return;
        }

        for (int i = 0; i < NumObjects; i++)
        {
            GameObject NewObject = (GameObject) Instantiate(Resources.Load(Path));
            NewObject.name = Path + "_" + i;
            ObjectPool.Add(NewObject.GetComponent<Rigidbody2D>());
            SendToInactivePool(i);
        }
    }

    public virtual void SetVelocity(float Speed)
    {
        foreach (Rigidbody2D ObjectBody in ObjectPool)
        {
            ObjectBody.velocity = new Vector2(-Speed, 0);
        }
    }

    public void SendToInactivePool(int Index)
    {
        ObjectPool[Index].velocity = Vector2.zero;
        ObjectPool[Index].transform.position = Toolbox.Instance.HoldingArea;
    }
}
