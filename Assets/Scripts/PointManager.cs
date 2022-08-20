using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    public static PointManager Instance;
    [SerializeField] private Transform pointPrefab;


    private void Awake() 
    {
        if (Instance != null)
        {
            Debug.LogError("Theres more than one PointManager " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public PointPopup Create(Vector3 _position, int _damage) 
    {
        Transform popupTransform = Instantiate(pointPrefab, _position, Quaternion.identity);

        PointPopup popup = popupTransform.GetComponent<PointPopup>();
        popup.Init(_damage);

        return popup;
    }
}
