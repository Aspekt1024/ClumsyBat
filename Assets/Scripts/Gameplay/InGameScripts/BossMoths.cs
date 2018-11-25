using ClumsyBat.Objects;
using UnityEngine;

public class BossMoths : MonoBehaviour
{
    private MothPool _moths;
    
    private void Start ()
    {
		_moths = new MothPool();
    }
}
