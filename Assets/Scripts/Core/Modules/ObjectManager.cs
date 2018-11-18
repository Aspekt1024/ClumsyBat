using ClumsyBat.Objects;
using UnityEngine;

namespace ClumsyBat
{
    public class ObjectManager
    {
        private CaveHandler caveHandler;
        public LevelObjectHandler ObjectHandler;


        public ObjectManager()
        {
            GameObject scriptsObject = GameObject.Find("LevelScripts");
            
            var caveObject = new GameObject("Caves");
            caveHandler = caveObject.AddComponent<CaveHandler>();

            ObjectHandler = new LevelObjectHandler();
        }

        public void SetupLevel(LevelContainer levelContainer)
        {
            caveHandler.SetupCave(levelContainer.Caves);

            for (int i = 0; i < levelContainer.Caves.Length; i++)
            {
                ObjectHandler.SetCaveObstacles(levelContainer.Caves[i], i);
            }
        }
        
    }
}
