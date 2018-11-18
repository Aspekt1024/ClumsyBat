using System;
using System.Collections.Generic;

namespace ClumsyBat.DataManagement
{
    [Serializable]
    public class LevelDataContainer : CloneableContainerBase<LevelDataContainer>
    {
        public const string FILE_NAME = "levelData";

        public List<LevelData> Levels;
    }
}
