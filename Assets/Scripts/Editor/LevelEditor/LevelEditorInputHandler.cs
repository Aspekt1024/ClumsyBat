using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelEditorInputHandler
{
    private LevelEditor editor;
    private LevelEditorActions actions;

    private enum InputBindings
    {
        SpawnMoth, SpawnStal, SpawnSpider, SpawnNpc, SpawnShroom, SpawnTrigger, SpawnWeb,
        RotateLeft, RotateRight, RandomiseRotation, Flip,
        RandomiseScale, ScaleUp, ScaleDown,
        ResetRotationAndScale,
        LevelEditorInspector
    }

    static Dictionary<InputBindings, KeyCode> bindingDict = new Dictionary<InputBindings, KeyCode>()
    {
        { InputBindings.SpawnMoth, KeyCode.Keypad1 },
        { InputBindings.SpawnStal, KeyCode.Keypad2 },
        { InputBindings.SpawnShroom, KeyCode.Keypad3 },
        { InputBindings.SpawnSpider, KeyCode.Keypad4 },
        { InputBindings.SpawnNpc, KeyCode.Keypad5 },
        { InputBindings.SpawnTrigger, KeyCode.Keypad6 },
        { InputBindings.SpawnWeb, KeyCode.Keypad7 },

        { InputBindings.RotateLeft, KeyCode.A },
        { InputBindings.RotateRight, KeyCode.D },
        { InputBindings.RandomiseRotation, KeyCode.G },
        { InputBindings.Flip, KeyCode.S },

        { InputBindings.ScaleUp, KeyCode.KeypadPlus },
        { InputBindings.ScaleDown, KeyCode.KeypadMinus },
        { InputBindings.RandomiseScale, KeyCode.C },

        { InputBindings.ResetRotationAndScale, KeyCode.Space },
        { InputBindings.LevelEditorInspector, KeyCode.Keypad0 }
    };

    public void ProcessInput(LevelEditor editorRef, LevelEditorActions actionsRef)
    {
        editor = editorRef;
        actions = actionsRef;

        if (Event.current.type == EventType.keyUp)
        {
            if (editor.HeldObject != null)
                ProcessHeldKeyUp();
            else
                ProcessFreeKeyUp();
        }
        else if (Event.current.type == EventType.MouseUp)
        {
            if (editor.HeldObject != null)
                ProcessHeldMouseUp();
            else
                ProcessFreeMouseUp();
        }
    }
    private void ProcessHeldKeyUp()
    {
        bool bUnused = false;

        if (BindingPressed(InputBindings.RotateLeft))
            actions.RotateLeft();
        else if (BindingPressed(InputBindings.RotateRight))
            actions.RotateRight();
        else if (BindingPressed(InputBindings.Flip))
            actions.Flip();
        else if (BindingPressed(InputBindings.RandomiseRotation))
            actions.RandomRotation();
        else if (BindingPressed(InputBindings.ScaleUp))
            actions.ScaleUp();
        else if (BindingPressed(InputBindings.ScaleDown))
            actions.ScaleDown();
        else if (BindingPressed(InputBindings.RandomiseScale))
            actions.RandomScale();
        else if (BindingPressed(InputBindings.ResetRotationAndScale))
            actions.ResetRotationAndScale();
        else
            bUnused = true;

        if (!bUnused)
            Event.current.Use();
    }

    private void ProcessFreeKeyUp()
    {
        bool bUnused = false;

        if (BindingPressed(InputBindings.SpawnMoth))
            editor.HeldObject = actions.ObjectHandler.SpawnObject<MothEditorHandler>();
        else if (BindingPressed(InputBindings.SpawnStal))
            editor.HeldObject = actions.ObjectHandler.SpawnObject<StalEditorHandler>();
        else if (BindingPressed(InputBindings.SpawnShroom))
            editor.HeldObject = actions.ObjectHandler.SpawnObject<MushroomEditorHandler>();
        else if (BindingPressed(InputBindings.LevelEditorInspector))
            actions.LevelEditorInspector();
        else
        {
            bUnused = true;
        }
        if (!bUnused)
            Event.current.Use();
    }

    private void ProcessHeldMouseUp()
    {
        switch (Event.current.button)
        {
            case 1:
                actions.DropHeldObject();
                break;
        }
    }

    private void ProcessFreeMouseUp()
    {
        if (Event.current.button == 1)
        {
            TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            if (t.TotalSeconds - editor.timeClicked < 0.2f)
            {
                actions.PickupObject();
            }
            else
            {
                editor.timeClicked = t.TotalSeconds;
            }
        }
    }

    private bool BindingPressed(InputBindings binding)
    {
        return bindingDict[binding] == Event.current.keyCode;
    }
}
