using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointPopup : MonoBehaviour
{
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float animationSpeed = 3f;
    [SerializeField] private float TIME_BEFORE_FADE = 0.5f;
    [SerializeField] private float expandAmountPerFrame = 0.5f;
    [SerializeField] private float shrinkAmountPerFrame = 0.5f;
    [SerializeField] private float initialScaleSize = 0.1f;
    [SerializeField] private TextMeshPro damageText;
    private Color textColor;
    private float currentFadeTimer;


    private void Awake()
    {
        textColor = damageText.color;
    }

    private void Update() 
    {
        currentFadeTimer -= Time.deltaTime;

        if (currentFadeTimer < 0) {
            FadeAndShrink();
            if (textColor.a < 0) Destroy(gameObject);
        } else {
            AnimateGrowAndPop();
        }
    }

    private void AnimateGrowAndPop()
    {
        transform.localScale += Vector3.one * expandAmountPerFrame * animationSpeed * Time.deltaTime;
        transform.position += new Vector3(0f, moveDistance * 2) * Time.deltaTime;
    }

    private void FadeAndShrink()
    {
        textColor.a -= animationSpeed * Time.deltaTime;
        damageText.color = textColor;
        // transform.localScale -= Vector3.one * shrinkAmountPerFrame * animationSpeed * Time.deltaTime;
    }
    
    public void Init(int _damage)
    {
        damageText.SetText(_damage.ToString());
        currentFadeTimer = TIME_BEFORE_FADE;
        transform.localScale = new Vector3(initialScaleSize, initialScaleSize, initialScaleSize);
    }
}
