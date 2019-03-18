using UnityEngine;

namespace ClumsyBat.Objects
{
    public sealed class MothPool : SpawnPool<Moth>
    {
        public MothPool()
        {
            ParentName = "Moths";
            ParentZ = Toolbox.Instance.ZLayers["Moth"];
            ResourcePath = "Collectibles/Moth";
            ObjTag = "Moth";
        }

        public struct MothType
        {
            public Spawnable.SpawnType SpawnTransform;
            public Moth.MothColour Colour;
            public MothPathHandler.MothPathTypes PathType;
        }

        public void SetupMothsInList(MothType[] mothList, float xOffset)
        {
            foreach (MothType moth in mothList)
            {
                Moth newMoth = GetObjectFromPool();
                Spawnable.SpawnType spawnTf = moth.SpawnTransform;
                spawnTf.Pos += new Vector2(xOffset, 0f);
                newMoth.Activate(spawnTf, moth.Colour, moth.PathType);
            }
        }

        /// <summary>
        /// Moth activation used in stationary levels e.g. Boss fights
        /// </summary>
        public void ActivateMothInRange(float minY, float maxY, Moth.MothColour colour)
        {
            Debug.Log("Activating moth in range");
            Moth newMoth = GetObjectFromPool();
            float xPos = 10f + GameStatics.Camera.CurrentCamera.transform.position.x;
            var spawnTf = new Spawnable.SpawnType
            {
                Pos = new Vector2(xPos, Random.Range(minY, maxY)),
                Rotation = new Quaternion(),
                Scale = Vector3.one
            };
            newMoth.Activate(spawnTf, colour, MothPathHandler.MothPathTypes.Sine);
        }

        public void ActivateMothFromEssence(Vector2 spawnLoc, Vector2 appearanceLoc, Moth.MothColour colour, float despawnTimer)
        {
            Moth newMoth = GetObjectFromPool();
            var spawnTf = new Spawnable.SpawnType
            {
                Pos = spawnLoc,
                Rotation = new Quaternion(),
                Scale = Vector3.one
            };
            newMoth.Activate(spawnTf, colour, MothPathHandler.MothPathTypes.Clover);
            newMoth.StartCoroutine(newMoth.SpawnFromEssence(appearanceLoc, despawnTimer));
        }

        public void CollectMothFromCrystal(Vector2 spawnLoc, Moth.MothColour colour)
        {
            Moth newMoth = GetObjectFromPool();
            var spawnTf = new Spawnable.SpawnType
            {
                Pos = spawnLoc,
                Rotation = new Quaternion(),
                Scale = Vector3.one
            };
            newMoth.Activate(spawnTf, colour);
            newMoth.StartCoroutine(newMoth.CollectFromCrystal());
        }
    }
}
