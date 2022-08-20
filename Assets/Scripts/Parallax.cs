using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    
    public float animationSpeed = 1f;
    [SerializeField] private float xDirection = 0f;
    [SerializeField] private float yDirection = 0f;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update() 
    {
        meshRenderer.material.mainTextureOffset += new Vector2(xDirection * animationSpeed * Time.deltaTime, yDirection * animationSpeed * Time.deltaTime);
    }


}
