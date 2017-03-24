using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothEditorHandler : BaseObjectHandler {

    public MothEditorHandler(LevelEditorObjectHandler objHandler) : base(objHandler)
    {
        parentObj = GetParentTransform("Moths");
        zLayer = LevelEditorConstants.MothZ;
    }

    protected override void Update()
    {
        AlignMoths();
    }

    private void AlignMoths()
    {
        foreach (Transform moth in parentObj)
        {
            Moth mothScript = moth.GetComponent<Moth>();
            foreach (Transform mothTf in moth.transform)
            {
                if (mothTf.name != "MothTrigger") continue;
                if (mothTf.position != moth.transform.position)
                {
                    mothTf.position = moth.transform.position;
                }
            }

            SpriteRenderer mothRenderer = moth.GetComponentInChildren<SpriteRenderer>();
            switch (mothScript.Colour)
            {
                case Moth.MothColour.Blue:
                    mothRenderer.color = new Color(0f, 0f, 1f);
                    break;
                case Moth.MothColour.Green:
                    mothRenderer.color = new Color(0f, 1f, 0f);
                    break;
                case Moth.MothColour.Gold:
                    mothRenderer.color = new Color(1f, 1f, 0f);
                    break;
            }

        }
    }
}
