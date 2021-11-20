using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public float rotateSpeed = 6;
    public float stretchSpeed = 10;
    public LayerMask platformLayer;
    [Header("Sound")]
    public AudioClip soundStretching;
    public AudioClip soundBridgeHit;
    AudioSource audioScr;
   [ReadOnly] public  bool isStretching = false;
    bool isFinishStretching = false;

    private void Start()
    {
        GetComponent<Collider2D>().enabled = false;

        audioScr = gameObject.AddComponent<AudioSource>();
        audioScr.loop = true;
        audioScr.clip = soundStretching;
        audioScr.playOnAwake = false;
    }

    public void Stretch(bool begin)
    {
        if (isFinishStretching)
            return;

        if (begin)
        {
            StartCoroutine(StretchCo());
            audioScr.Play();
        }
        else
        {
            isFinishStretching = true;
            StopAllCoroutines();
            StartCoroutine(RotateCo());
            audioScr.Stop();
        }
    }

    IEnumerator StretchCo()
    {
        var spriteRen = GetComponent<SpriteRenderer>();
        isStretching = true;
        while (true)
        {
            spriteRen.size = new Vector2(spriteRen.size.x, spriteRen.size.y + stretchSpeed * Time.deltaTime);
            //transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y + stretchSpeed * Time.deltaTime);
            yield return null;

            if (spriteRen.size.y > 20)
                Stretch(false);
        }
    }

    IEnumerator RotateCo()
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += rotateSpeed * Time.deltaTime;
            percent = Mathf.Clamp01(percent);
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -90, percent));
            yield return null;
        }
        GetComponent<Collider2D>().enabled = true;
        GameManager.Instance.AllowPlayerMove();

        var bounds = GetComponent<BoxCollider2D>().bounds;
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(bounds.max.x, bounds.min.y), Vector2.down, 1, platformLayer);
        Debug.DrawRay(new Vector2(bounds.max.x, bounds.min.y), Vector2.down);
        if (!hit)
        {
            LevelManager.Instance.PrepareToBreak();
            FloatingTextManager.Instance.ShowText("MISS!", Vector2.up * 3, Color.red, new Vector2(bounds.max.x, bounds.min.y));
        }
        else
        {
            GameManager.Instance.PlayerScore++;
            FloatingTextManager.Instance.ShowText("GOOD!", Vector2.up * 3, Color.yellow, new Vector2(bounds.max.x, bounds.min.y));
            
        }

        SoundManager.PlaySfx(soundBridgeHit);
    }

    IEnumerator RotateFallingCo()
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += rotateSpeed * Time.deltaTime;
            percent = Mathf.Clamp01(percent);
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(-90, -180, percent));
            yield return null;
        }
    }

    public void BreakBridge()
    {
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(RotateFallingCo());
    }
}
