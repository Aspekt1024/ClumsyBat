using UnityEngine;
using System.IO;
using System.Xml.Serialization;

[XmlRoot("LevelCollection")]
public class LevelContainer {

    public struct CaveType
    {
        public int TopIndex;
        public int BottomIndex;
        public bool bTopSecretPath;
        public bool bBottomSecretPath;
        public ShroomPool.ShroomType[] Shrooms;
        public StalPool.StalType[] Stals;
        public MothPool.MothType[] Moths;
        public SpiderPool.SpiderType[] Spiders;
        public WebPool.WebType[] Webs;
        public TriggerHandler.TriggerType[] Triggers;
        public NPCPool.NpcType[] Npcs;
    }
    
    public struct ClumsyType
    {
        public Vector2 Pos;
        public Vector2 Scale;
        public Quaternion Rotation;
    }

    [XmlArray("CaveList"), XmlArrayItem("Cave")]
    public CaveType[] Caves;

    public ClumsyType Clumsy;

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(LevelContainer));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    public static LevelContainer Load(string path)
    {
        var serializer = new XmlSerializer(typeof(LevelContainer));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as LevelContainer;
        }
    }

    public static LevelContainer LoadFromText(string text)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(LevelContainer));
        LevelContainer lc = serializer.Deserialize(new StringReader(text)) as LevelContainer;
        return lc;
    }
}
