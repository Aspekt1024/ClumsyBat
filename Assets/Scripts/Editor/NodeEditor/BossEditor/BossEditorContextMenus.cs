using UnityEngine;
using UnityEditor;

public class BossEditorContextMenus : BaseContextMenus {
    
    public BossEditorContextMenus(BaseEditor editorInstance) : base(editorInstance)
    {
        editor = editorInstance;
    }

    public override void ShowMenu()
    {
        if (editor.BehaviourSet.IsType<StateMachine>())
            ShowStateMachineMenu();
        else if (editor.BehaviourSet.IsType<State>())
            ShowStateMenu();
    }

    private void ShowStateMachineMenu()
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Events/Collision"), false, ContextCallback, typeof(CollisionNode));
        menu.AddItem(new GUIContent("Actions/Damage"), false, ContextCallback, typeof(DamageNode));
        menu.AddItem(new GUIContent("Actions/Set Health"), false, ContextCallback, typeof(HealthNode));
        menu.AddItem(new GUIContent("Actions/Wait"), false, ContextCallback, typeof(WaitNode));
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("New State"), false, ContextCallback, typeof(StateNode));
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Add Loop"), false, ContextCallback, typeof(LoopNode));
        menu.AddItem(new GUIContent("Operator/Multiple Out"), false, ContextCallback, typeof(MultipleOutNode));
        menu.AddItem(new GUIContent("Operator/Multiple In"), false, ContextCallback, typeof(MultipleInNode));
        menu.AddItem(new GUIContent("Operator/Alternating Output"), false, ContextCallback, typeof(AlternatingNode));

        menu.AddSeparator("");
        if (editor.StartExists())
            menu.AddItem(new GUIContent("Find Start"), false, ContextCallback, NodeMenuSelections.FindStart);
        else
            menu.AddItem(new GUIContent("Add Start"), false, ContextCallback, typeof(StartNode));

        menu.ShowAsContext();
    }

    private void ShowStateMenu()
    {
        GenericMenu menu = new GenericMenu();
        // TODO get all ability nodes and generate them dynamically
        // Use the window title of each node
        menu.AddItem(new GUIContent("Abilities/Jump"), false, ContextCallback, typeof(JumpNode));
        menu.AddItem(new GUIContent("Abilities/Projectile"), false, ContextCallback, typeof(ProjectileNode));
        menu.AddItem(new GUIContent("Abilities/Stalactite"), false, ContextCallback, typeof(SpawnStalNode));
        menu.AddItem(new GUIContent("Abilities/Movement"), false, ContextCallback, typeof(WalkNode));
        menu.AddItem(new GUIContent("Abilities/Charge"), false, ContextCallback, typeof(ChargeNode));
        menu.AddItem(new GUIContent("Flying Abilities/Air Charge"), false, ContextCallback, typeof(AirChargeNode));

        menu.AddItem(new GUIContent("Events/New"), false, ContextCallback, typeof(StateEventNode));
        menu.AddItem(new GUIContent("Events/Player"), false, ContextCallback, typeof(PlayerEventNode));
        menu.AddItem(new GUIContent("Events/Moth"), false, ContextCallback, typeof(SpawnMothNode));

        menu.AddItem(new GUIContent("Operator/Multiple Out"), false, ContextCallback, typeof(MultipleOutNode));
        menu.AddItem(new GUIContent("Operator/Multiple In"), false, ContextCallback, typeof(MultipleInNode));
        menu.AddItem(new GUIContent("Operator/Comparison"), false, ContextCallback, typeof(CompareNode));
        menu.AddItem(new GUIContent("Operator/Position"), false, ContextCallback, typeof(PositionNode));
        menu.AddItem(new GUIContent("Operator/Random Float"), false, ContextCallback, typeof(RandomNode));

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("Player Reference"), false, ContextCallback, typeof(PlayerNode));
        menu.AddItem(new GUIContent("Boss Reference"), false, ContextCallback, typeof(BossNode));

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("Add Wait"), false, ContextCallback, typeof(WaitNode));

        menu.AddSeparator("");

        if (editor.StartExists())
            menu.AddItem(new GUIContent("Find Start"), false, ContextCallback, NodeMenuSelections.FindStart);
        else
            menu.AddItem(new GUIContent("Add Start"), false, ContextCallback, typeof(StartNode));
        menu.AddItem(new GUIContent("Add Loop"), false, ContextCallback, typeof(LoopNode));
        menu.ShowAsContext();
    }
    
    public override void ShowNodeMenu(BaseNode mouseDownNode)
    {
        selectedNode = mouseDownNode;

        GenericMenu menu = new GenericMenu();
        if (mouseDownNode.IsType<StateNode>())
        {
            menu.AddItem(new GUIContent("Edit State"), false, ContextCallback, NodeMenuSelections.EditState);
            menu.AddSeparator("");
        }
        menu.AddItem(new GUIContent("Find Start"), false, ContextCallback, NodeMenuSelections.FindStart);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, NodeMenuSelections.DeleteNode);
        menu.ShowAsContext();
    }
}
