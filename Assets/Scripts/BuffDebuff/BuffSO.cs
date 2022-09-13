using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class BuffSO : ScriptableObject
{
    [PreviewField]
    public Sprite icon;
    public string buffName;
    public float duration;
    public abstract void Execute();
}
