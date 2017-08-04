using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelEditorInputHandler
{
    private LevelEditor editor;
    private LevelEditorActions actions;
    private LevelEditorContextMenu menu;

    private enum InputBindings
    {
        SpawnObjectMenu,
        SpawnMoth, SpawnStal, SpawnSpider, SpawnNpc, SpawnShroom, SpawnTrigger, SpawnWeb,
        DestroyObject,
        RotateLeft, RotateRight, RandomiseRotation, Flip,
        RandomiseScale, ScaleUp, ScaleDown,
        ResetRotationAndScale,
        LevelEditorInspector,
        AddCavePiece, RemoveCavePiece
    }

    static Dictionary<InputBindings, KeyCode> bindingDict = new Dictionary<InputBindings, KeyCode>()
    {
        { InputBindings.SpawnObjectMenu, KeyCode.A },

        { InputBindings.SpawnMoth, KeyCode.Alpha1 },
        { InputBindings.SpawnStal, KeyCode.Alpha2 },
        { InputBindings.SpawnShroom, KeyCode.Alpha3 },
        { InputBindings.SpawnSpider, KeyCode.Alpha4 },
        { InputBindings.SpawnWeb, KeyCode.Alpha5 },
        { InputBindings.SpawnTrigger, KeyCode.Alpha6 },
        { InputBindings.SpawnNpc, KeyCode.Alpha7 },

        { InputBindings.DestroyObject, KeyCode.Escape },

        { InputBindings.RotateLeft, KeyCode.A },
        { InputBindings.RotateRight, KeyCode.D },
        { InputBindings.RandomiseRotation, KeyCode.G },
        { InputBindings.Flip, KeyCode.S },

        { InputBindings.ScaleUp, KeyCode.KeypadPlus },
        { InputBindings.ScaleDown, KeyCode.KeypadMinus },
        { InputBindings.RandomiseScale, KeyCode.C },

        { InputBindings.ResetRotationAndScale, KeyCode.Space },
        { InputBindings.LevelEditorInspector, KeyCode.Space },

        { InputBindings.AddCavePiece, KeyCode.KeypadPlus },
        { InputBindings.RemoveCavePiece, KeyCode.KeypadMinus }
    };

    public void ProcessInput(LevelEditor editorRef, LevelEditorActions actionsRef)
    {
        editor = editorRef;
        actions = actionsRef;
        if (menu == null)
            menu = new LevelEditorContextMenu(editor, actions);
        
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
        else if (BindingPressed(InputBindings.DestroyObject))
            actions.DestroyHeldObject();
        else
            bUnused = true;

        if (!bUnused)
            Event.current.Use();
    }

    private void ProcessFreeKeyUp()
    {
        bool bUnused = false;

        if (BindingPressed(InputBindings.LevelEditorInspector))
            actions.LevelEditorInspector();
        else if (BindingPressed(InputBindings.SpawnObjectMenu))
            menu.ShowMenu();
        else if (BindingPressed(InputBindings.AddCavePiece))
            actions.AddCavePiece();
        else if (BindingPressed(InputBindings.RemoveCavePiece))
            actions.RemoveCavePiece();
        else if (Event.current.shift)
        {
            if (BindingPressed(InputBindings.SpawnMoth))
                editor.HeldObject = actions.ObjectHandler.SpawnObject<MothEditorHandler>();
            else if (BindingPressed(InputBindings.SpawnStal))
                editor.HeldObject = actions.ObjectHandler.SpawnObject<StalEditorHandler>();
            else if (BindingPressed(InputBindings.SpawnShroom))
                editor.HeldObject = actions.ObjectHandler.SpawnObject<MushroomEditorHandler>();
            else if (BindingPressed(InputBindings.SpawnSpider))
                editor.HeldObject = actions.ObjectHandler.SpawnObject<SpiderEditorHandler>();
            else if (BindingPressed(InputBindings.SpawnWeb))
                editor.HeldObject = actions.ObjectHandler.SpawnObject<WebEditorHandler>();
            else if (BindingPressed(InputBindings.SpawnTrigger))
                editor.HeldObject = actions.ObjectHandler.SpawnObject<TriggerEditorHandler>();
            else if (BindingPressed(InputBindings.SpawnNpc))
                editor.HeldObject = actions.ObjectHandler.SpawnObject<NpcEditorHandler>();
            else
                bUnused = true;
        }
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
            if (t.TotalSeconds - editor.timeClicked < 0.2f || Event.current.control || Event.current.shift)
            {
                actions.PickupObject(Event.current.mousePosition);
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
