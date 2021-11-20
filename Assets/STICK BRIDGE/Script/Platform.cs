using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    //public float minWidth = 0.5f;
    //public float maxWidth = 3;

    SpriteRenderer spriteRen;

    [Header("Show up effect")]
    public float moveFromBottom = 5;
    public float moveSpeed = 5;

    private void Awake()
    {
        spriteRen = GetComponent<SpriteRenderer>();
    }

    //called by Levelmanager
    public void Init(float minSize, float maxSize)
    {
        spriteRen.size = new Vector2(Random.Range(minSize, maxSize), spriteRen.size.y);
        StartCoroutine(MovingUpEffectCo());
    }

    IEnumerator MovingUpEffectCo()
    {
        float percent = 0;
        Vector3 originalPos = transform.position;
        var fromPos = transform.position - Vector3.up * moveFromBottom;
        while(percent<1)
        {
            percent += moveSpeed * Time.deltaTime;
            percent = Mathf.Clamp01(percent);
            transform.position = Vector3.Lerp(fromPos, originalPos, percent);
            yield return null;
        }
    }

    public Vector2 GetTopRightPosition()
    {
        return GetComponent<BoxCollider2D>().bounds.max;
    }

    public float GetMaxXPosition()
    {
        return GetComponent<BoxCollider2D>().bounds.max.x;
    }

    public Vector2 GetCenterTopPosition()
    {
        var bounds = GetComponent<BoxCollider2D>().bounds;
        return new Vector2(bounds.center.x, bounds.max.y);
    }
}
