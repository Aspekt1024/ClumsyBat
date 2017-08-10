using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPath : MonoBehaviour {

    private LevelButton[] buttons;

    private List<Path> paths = new List<Path>();

    public void CreateLevelPaths()
    {
        buttons = transform.parent.GetComponentInChildren<LevelButtonHandler>().LevelButtons();
        CreatePaths();
    }

    private void CreatePaths()
    {
        foreach(LevelButton button in buttons)
        {
            if (!button) continue;
            Path newPath = new Path(button, transform);
            paths.Add(newPath);
        }
    }
}

public class Path
{
    public Path(LevelButton level, Transform parent)
    {
        lvlBtn = level;
        pathPointRef = Resources.Load<GameObject>("UIElements/PathPoint");

        if (level.PreviousLevel != null)
        {
            CreatePath(parent);
        }
    }

    public GameObject[] LevelPath;

    private LevelButton lvlBtn;
    private GameObject pathPointRef;

    private void CreatePath(Transform parent)
    {
        Vector2 prevPos = lvlBtn.PreviousLevel.position;
        Vector2 thisPos = lvlBtn.transform.position;

        float dist = Vector2.Distance(prevPos, thisPos);
        int numPoints = Mathf.RoundToInt(dist / 0.28f) - 2;
        LevelPath = new GameObject[numPoints];

        for (int i = 0; i < numPoints; i++)
        {
            LevelPath[i] = Object.Instantiate(pathPointRef, parent);
            LevelPath[i].transform.position = Vector2.Lerp(thisPos, prevPos, ((float)(i + 1) / (numPoints + 1)));
            LevelPath[i].transform.localScale = Vector3.one * 8f;
        }
    }

}
