using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class EntranceAnimation : MonoBehaviour
{
    [SerializeField] private Vector2 fromPosition;
    [SerializeField] private Ease easeType;
    [SerializeField] private float duration;
    // [SerializeField] private float timeToActivate;

    private Vector3 defaultTransform;
    private float currentTime;
    // private bool isActive;


    private void Start() 
    {
        defaultTransform = transform.position;
        transform.position = fromPosition;
        // isActive = true;
    }

    // private void FixedUpdate()
    // {
    //     currentTime += Time.fixedDeltaTime;

    //     if (!isActive) return;
    //     if (currentTime < timeToActivate) return;

    //     MoveTransform();
    //     isActive = false;
    // }

    public void MoveTransform()
    {
        transform.DOMove(defaultTransform, duration).SetEase(easeType);
    }

    [Button("Test Animation")]
    private void TestAnimation()
    {
        defaultTransform = transform.position;
        transform.position = fromPosition;  

        transform.DOMove(defaultTransform, duration).SetEase(easeType);
    }

    private void OnDrawGizmos() 
    {
        if (Application.isPlaying) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(fromPosition, 2f);    
    }
}
