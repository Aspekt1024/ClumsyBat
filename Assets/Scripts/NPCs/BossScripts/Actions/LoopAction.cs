
public class LoopAction : BaseAction {
    
    public override void Activate()
    {
        foreach (var node in bossBehaviour.BossProps.CurrentState.Actions)
        {
            if (node.GetType().Equals(typeof(StartAction)))
                node.Activate();
        }
    }
}
