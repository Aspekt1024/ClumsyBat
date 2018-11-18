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

        public void SetPaused(bool pauseGame)
        {
            Shrooms.PauseGame(pauseGame);
            Stals.PauseGame(pauseGame);
            Moths.PauseGame(pauseGame);
            Spiders.PauseGame(pauseGame);
            Webs.PauseGame(pauseGame);
            Triggers.PauseGame(pauseGame);
            Npcs.PauseGame(pauseGame);
        }
    }
}
