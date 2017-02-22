using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "Custom/New Boss", order = 1)]
public class BossCreator : ScriptableObject
{
    public string BossName;
    public UnityEditor.MonoScript BossObject;
    public UnityEditor.MonoScript[] AbilitySet;
}
