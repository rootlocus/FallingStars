using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    private static MouseWorld Instance;
    [SerializeField] private LayerMask mousePlaneLayerMask;

    private void Awake() {
        Instance = this;    
    }

    public static Vector2 GetPosition()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        return (Vector2) worldPosition;
    }
}
