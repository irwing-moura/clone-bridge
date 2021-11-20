using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public LevelParemeter[] levelParameter;
   [ReadOnly] public  int currentLevel = 0;

    public Bridge bridge;
    public Platform currentPlatform;
    public Platform originalPlatform;
    public float platformMinSize = 0.8f;
    public float platformMaxSize = 2;
    public float minDistance = 1;
    public float maxDistance = 3;

    [Header("Spawn Star")]
    public GameObject star;
    [Range(0,1)]
    public float starShowChance = 0.2f;

    [Space]

   
    Bridge currentBridge;

  [ReadOnly]  public bool isBridgeReachToPlatform = true; /* { get; set; }*/

    bool isWaiting = false;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InvokeRepeating("CheckingNextLevel", 0, 0.1f);
    }

    void CheckingNextLevel()
    {
        if (GameManager.Instance.PlayerScore >= levelParameter[currentLevel].activeAtScore)
        {
            //set parameters
            platformMinSize = levelParameter[currentLevel].platformMinSize;
            platformMaxSize = levelParameter[currentLevel].platformMaxSize;

            currentLevel++;
            if (currentLevel >= levelParameter.Length)
            {
                currentLevel--;
                CancelInvoke("CheckingNextLevel");
            }
        }
    }

    public void SpawnNextPlatform()
    {
        isWaiting = false;
        currentBridge = Instantiate(bridge, currentPlatform.GetTopRightPosition(), Quaternion.identity);
        currentPlatform = Instantiate(originalPlatform, new Vector2(currentPlatform.GetMaxXPosition() + Random.Range(minDistance, maxDistance), currentPlatform.transform.position.y), Quaternion.identity);
        currentPlatform.Init(platformMinSize, platformMaxSize);
        if (Random.Range(0f,1f) < starShowChance)
        {
            Instantiate(star, currentPlatform.GetCenterTopPosition() + Vector2.up, Quaternion.identity);
        }
    }

    public void BeginStretchBridge()
    {
        if (isWaiting || currentBridge == null)
            return;

        if (!currentBridge.isStretching)
            currentBridge.Stretch(true);
    }

    public void StopStretchBridge()
    {
        if (isWaiting || currentBridge == null)
            return;

        if (currentBridge.isStretching)
        {
            isWaiting = true;
            currentBridge.Stretch(false);
        }
    }

    public void PrepareToBreak()
    {
        isBridgeReachToPlatform = false;
        currentPlatform.GetComponent<Collider2D>().enabled = false;
    }

    public void BreakBridge()
    {
        currentBridge.BreakBridge();
    }
}

[System.Serializable]
public class LevelParemeter
{
    [Tooltip("active the parameter if player's score reach this")]
    public int activeAtScore = 10;
    public float platformMinSize = 1;
    public float platformMaxSize = 3;
}
