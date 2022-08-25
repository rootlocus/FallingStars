using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/OrbType")]
public class OrbTypeSO : ScriptableObject
{
    public enum OrbType
    {
        Normal,
        Special,
    }
    public OrbType type;
    public Color color;
    public string orbName;
    public Sprite sprite;
    public Animator orbAnimation;
}