using UnityEngine;
using Sirenix.OdinInspector;

public abstract class OrbSO : ScriptableObject
{
    [PreviewField]
    public Sprite sprite;
    public string orbName;
    public AnimatorOverrideController orbAnimation;
    public abstract void Execute(GridPosition gridPosition, OrbSO orbType);
}