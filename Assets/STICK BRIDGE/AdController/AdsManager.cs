using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;
    //delegate   ()
    public delegate void RewardedAdResult(bool isSuccess, int rewarded);

    //event  
    public static event RewardedAdResult AdResult;

    public enum AD_NETWORK { Unity, Admob}

    [Header("REWARDED VIDEO AD")]
    public AD_NETWORK rewardedUnit;
    public int rewardedWatchAd = 50;

    //[Header("SHOW AD ON GAME FINISH")]
    //public AD_NETWORK adGameFinishUnit;
    //public int showAdGameFinishCounter = 1;
    //[ReadOnly] int counter_gameFinish = 0;

    [Header("SHOW AD ON PLAYER DIE")]
    public AD_NETWORK adGameOverUnit;
    public int showAdGameOverCounter = 1;
    [ReadOnly] int counter_gameOver = 0;

    private void Awake()
    {
        if (AdsManager.Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ShowAdmobBanner(bool show)
    {
        AdmobController.Instance.ShowBanner(show);
    }

    #region NORMAL AD

    public void ShowNormalAd(GameManager.GameState state)
    {
        Debug.Log("SHOW NORMAL AD " + state);

        if (state == GameManager.GameState.Dead)
            StartCoroutine(ShowNormalAdCo(state, 0.8f));
        else
            StartCoroutine(ShowNormalAdCo(state, 0));
    }

    IEnumerator ShowNormalAdCo(GameManager.GameState state, float delay)
    {
        yield return new WaitForSeconds(delay);

        //if (state == GameManager.GameState.Finish)
        //{
        //    counter_gameFinish++;
        //    if (counter_gameFinish >= showAdGameFinishCounter)
        //    {
        //        if (adGameFinishUnit == AD_NETWORK.Unity)
        //        {
        //            //try show Unity video
        //            if (UnityAds.Instance.ForceShowNormalAd())
        //            {
        //                counter_gameFinish = 0;
        //            }
        //        }
        //        else if (adGameFinishUnit == AD_NETWORK.Admob)
        //        {
        //            if (!AdmobController.Instance.ForceShowInterstitialAd())
        //            {
        //                counter_gameFinish = 0;
        //            }
        //        }
        //    }
        //}
        //else if (state == GameManager.GameState.Dead)
        //{
            counter_gameOver++;
            if (counter_gameOver >= showAdGameOverCounter)
            {
                if (adGameOverUnit == AD_NETWORK.Unity)
                {
                    //try show Unity video
                    if (UnityAds.Instance.ForceShowNormalAd())
                    {
                        counter_gameOver = 0;
                    }
                }
                else if (adGameOverUnit == AD_NETWORK.Admob)
                {
                    if (AdmobController.Instance.ForceShowInterstitialAd())
                    {
                        counter_gameOver = 0;
                    }
                }
            }   
        //}
    }

    public void ResetCounter()
    {
        counter_gameOver = 0;
        //counter_gameFinish = 0;
    }

    #endregion

    #region REWARDED VIDEO AD

    public bool isRewardedAdReady()
    {
        if ((rewardedUnit == AD_NETWORK.Unity) && UnityAds.Instance.isRewardedAdReady())
            return true;

        if ((rewardedUnit == AD_NETWORK.Admob) && AdmobController.Instance.isRewardedVideoAdReady())
            return true;

        return false;

    }

    public void ShowRewardedAds()
    {
        if(rewardedUnit == AD_NETWORK.Unity)
        {
            UnityAds.AdResult += UnityAds_AdResult;
            UnityAds.Instance.ShowRewardVideo();
        }
        else
        {
            AdmobController.AdResult += AdmobController_AdResult;
            AdmobController.Instance.WatchRewardedVideoAd();
        }
    }

    private void AdmobController_AdResult(bool isWatched)
    {
        AdmobController.AdResult -= AdmobController_AdResult;
        AdResult(true, rewardedWatchAd);
    }

    private void UnityAds_AdResult(WatchAdResult result)
    {
        UnityAds.AdResult -= UnityAds_AdResult;
        AdResult(result == WatchAdResult.Finished, rewardedWatchAd);
    }

    #endregion
}
