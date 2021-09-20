using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Counter")]
public class Counter : ScriptableObject
{
    [SerializeField]
    private int _count;
    public int count { get { return _count; } set { _count = value; } }
}
