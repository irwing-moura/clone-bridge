using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public AudioClip collectStarSound;
    public GameObject effect;

    public void Collect()
    {
        SoundManager.PlaySfx(collectStarSound);
        Instantiate(effect, transform.position, Quaternion.identity);
        GlobalValue.SavedCoins++;
        Destroy(gameObject);
    }
}
