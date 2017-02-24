using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class BaseInputNode : BaseNodeOld {

	public virtual string GetResult()
    {
        return "None";
    }

    public override void DrawCurves()
    {
    }
}
