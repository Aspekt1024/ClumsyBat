namespace ClumsyBat.Objects
{/// <summary>
 /// Handles the generation, positioning and movement of all cave pieces
 /// </summary>
    public class LevelObjectHandler
    {
        public readonly ShroomPool Shrooms = new ShroomPool();
        public readonly MothPool Moths = new MothPool();
        public readonly StalPool Stals = new StalPool();
        public readonly SpiderPool Spiders = new SpiderPool();
        public readonly WebPool Webs = new WebPool();
        public readonly TriggerHandler Triggers = new TriggerHandler();
        public readonly NpcPool Npcs = new NpcPool();

        private ISpawnPool[] spawnPools = new ISpawnPool[7];

        public LevelObjectHandler()
        {
            spawnPools[0] = Shrooms;
            spawnPools[1] = Moths;
            spawnPools[2] = Stals;
            spawnPools[3] = Spiders;
            spawnPools[4] = Webs;
            spawnPools[5] = Triggers;
            spawnPools[6] = Npcs;
        }
        
        public void SetCaveObstacles(LevelContainer.CaveType cave, int index)
        {
            var xOffset = index * Toolbox.TileSizeX;
            
            Shrooms.SetupMushroomsInList(cave.Shrooms, xOffset);
            Stals.SetupStalactitesInList(cave.Stals, xOffset);
            Moths.SetupMothsInList(cave.Moths, xOffset);
            Spiders.SetupSpidersInList(cave.Spiders, xOffset);
            Webs.SetupWebsInList(cave.Webs, xOffset);
            Triggers.SetupTriggersInList(cave.Triggers, xOffset);
            Npcs.SetupObjectsInList(cave.Npcs, xOffset);
        }

        public void DisableAllObjects()
        {
            foreach (var pool in spawnPools)
            {
                pool.DisableObjects();
            }
        }
    }
}
