using UnityEngine;

[CreateAssetMenu(fileName = "ObjSet", menuName = "Scriptable Objects/ObjSet")]
public class ObjSet : ScriptableObject
{
    public string objCode;
    public APoolingObject poolingObject;
}
