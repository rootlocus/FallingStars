using UnityEngine;
using Sirenix.OdinInspector;

//Deprecated
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
    [PreviewField]
    public Sprite sprite;
    public AnimatorOverrideController orbAnimation;
}