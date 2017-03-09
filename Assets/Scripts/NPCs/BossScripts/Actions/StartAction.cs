
[System.Serializable]
public class StartAction : BaseAction {

    public int TestVariable = 199;

    public override void Activate()
    {
        CallNext();
    }

}
