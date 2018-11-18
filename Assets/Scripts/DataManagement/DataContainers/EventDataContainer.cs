using System;

namespace ClumsyBat.DataManagement
{
    [Serializable]
    public class EventDataContainer : CloneableContainerBase<EventDataContainer>
    {
        public static string FILE_NAME = "eventData";

        public bool BossLeftRightTapTutorialSeen;
    }
}
