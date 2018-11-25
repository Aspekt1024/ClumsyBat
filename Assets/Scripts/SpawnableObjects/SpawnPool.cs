using UnityEngine;
using System.Collections.Generic;

namespace ClumsyBat.Objects
{
    public interface ISpawnPool
    {
        void DisableObjects();
    }

    public abstract class SpawnPool<T> : ISpawnPool where T : Spawnable
    {
        public string ObjTag;

        protected Transform ParentObject;
        protected string ParentName;
        protected float ParentZ;
        protected string ResourcePath;
        protected int numObjectsInPool;

        protected readonly List<T> ObjPool = new List<T>();
        private int index;
        private int numObjects;

        public void DisableObjects()
        {
            foreach (var obj in ObjPool)
            {
                obj.gameObject.SetActive(false);
            }
            index = 0;
        }

        protected T GetObjectFromPool()
        {
            if (ParentObject == null)
            {
                CreateParent();
            }

            for (int i = index; i < numObjects; i++)
            {
                if (ObjPool[i].isActiveAndEnabled) continue;
                return ObjPool[i];
            }
            return CreateObject(numObjects);
        }

        protected void CreateParent()
        {
            ParentObject = new GameObject(ParentName).transform;
            ParentObject.position = new Vector3(0f, 0f, ParentZ);
        }

        protected T CreateObject(int objNum)
        {
            var newObj = (GameObject)Object.Instantiate(Resources.Load(ResourcePath), ParentObject);
            newObj.name = ObjTag + objNum;
            newObj.transform.position = Toolbox.Instance.HoldingArea;
            var objScript = newObj.GetComponent<T>();
            ObjPool.Add(objScript);
            numObjects++;
            return objScript;
        }
    }
}