using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using UnityEngine.Advertisements;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject Startmenu;
    public GameObject GUI;
    public GameObject Gameover;
    public GameObject GamePause;
    public GameObject Controller;
    public GameObject Loading;
    public GameObject Shop;

    public GameObject rewardedAdsBtn;

    [Space]
    public Text[] coinTxts;
    public Text[] bestTxts;
    public Text[] currentDistanceTxts;
    public Text[] rewardedTxts;

    public Image[] soundImages;
    public Sprite imageOn, imageOff;

    void Awake()
    {
        Instance = this;
        Startmenu.SetActive(false);
        GUI.SetActive(false);
        Gameover.SetActive(false);
        GamePause.SetActive(false);
        Loading.SetActive(false);
        Controller.SetActive(false);
        Shop.SetActive(false);
    }

    // Use this for initialization
    IEnumerator Start()
    {
        InvokeRepeating("UpdateTextCo", 0, 0.1f);

        yield return null;
        if (GlobalValue.isPlayed)
            Play();
        else
        {
            Startmenu.SetActive(true);
            GlobalValue.isPlayed = true;
        }

        TurnSound(false);
    }

    void UpdateTextCo()
    {
        foreach(var txt in coinTxts)
        {
            txt.text = GlobalValue.SavedCoins + "";
        }

        foreach (var txt in bestTxts)
        {
            txt.text = "BEST SCORE: " + GlobalValue.BestScore + "";
        }

        foreach (var txt in currentDistanceTxts)
        {
            txt.text = GameManager.Instance.PlayerScore + "";
        }

        if (AdsManager.Instance)
        {
            foreach (var txt in rewardedTxts)
            {
                txt.text = AdsManager.Instance.rewardedWatchAd + "";
            }
        }

        rewardedAdsBtn.SetActive(AdsManager.Instance && AdsManager.Instance.isRewardedAdReady());
    }

    public void Play()
    {
        Startmenu.SetActive(false);
        GUI.SetActive(true);
        Controller.SetActive(true);
        GameManager.Instance.StartGame();
    }

    public void OpenShop(bool open)
    {
        SoundManager.Click();
        Shop.SetActive(open);
        Gameover.SetActive(!open);
    }

    public void TurnSound(bool turn = true)
    {
        var on = AudioListener.volume == 1;
        if (turn)
            on = !on;

        foreach (var img in soundImages)
        {
            img.sprite = on ? imageOn : imageOff;
        }
        SoundManager.Click();
        AudioListener.volume = on ? 1 : 0;
    }

    [Header("LOADING PROGRESS")]
    public Slider slider;
    public Text progressText;
    IEnumerator LoadAsynchronously(string name)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (slider != null)
                slider.value = progress;
            if (progressText != null)
                progressText.text = (int)progress * 100f + "%";
            yield return null;
        }
    }

    public void TurnController(bool turnOn)
    {
        Controller.SetActive(turnOn);
    }
    public void TurnGUI(bool turnOn)
    {
        GUI.SetActive(turnOn);
    }
    public void RestartGame()
    {
        Time.timeScale = 1;
        SoundManager.PlaySfx(SoundManager.Instance.soundClick);
        //if (!GlobalValue.RemoveAds && DefaultValue.Instance && DefaultValue.Instance.restartLevelAd && Advertisement.IsReady())
        //{
        //    watchVideoType = WatchVideoType.Restart;
        //    var options = new ShowOptions { resultCallback = HandleShowResult };
        //    if (!Advertisement.isShowing)
        //        Advertisement.Show(options);
        //}
        //else
        //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Loading.SetActive(true);
        StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().name));
    }

    public void HomeScene()
    {
        SoundManager.PlaySfx(SoundManager.Instance.soundClick);
        Time.timeScale = 1;
        //SceneManager.LoadSceneAsync("MainMenu");
        Loading.SetActive(true);
        StartCoroutine(LoadAsynchronously("MainMenu"));

    }

    public void OpenStoreLink()
    {
        GameMode.Instance.OpenStoreLink();
    }

    public void GameOver()
    {
        StartCoroutine(GameOverCo(1));
    }

    public void Pause()
    {
        SoundManager.PlaySfx(SoundManager.Instance.soundClick);
        if (Time.timeScale == 0)
        {
            GamePause.SetActive(false);
            GUI.SetActive(true);
            Time.timeScale = 1;
            SoundManager.Instance.PauseMusic(false);
            GameManager.Instance.PauseGame(false);
        }
        else
        {
            GamePause.SetActive(true);
            GUI.SetActive(false);
            Time.timeScale = 0;
            SoundManager.Instance.PauseMusic(true);
            GameManager.Instance.PauseGame(true);
        }
    }

    public enum WatchVideoType { Checkpoint, Restart, Next }
    public WatchVideoType watchVideoType;



    IEnumerator GameOverCo(float time)
    {
        GUI.SetActive(false);
        Controller.SetActive(false);

        yield return new WaitForSeconds(time);

        //show ads
        SoundManager.PlaySfx(SoundManager.Instance.soundGameover, 0.5f);
        Gameover.SetActive(true);
        if (AdsManager.Instance)
            AdsManager.Instance.ShowAdmobBanner(true);
    }

    public void WatchRewardedAds()
    {
        AdsManager.AdResult += AdsManager_AdResult;
        AdsManager.Instance.ShowRewardedAds();
    }

    private void AdsManager_AdResult(bool isSuccess, int rewarded)
    {
        AdsManager.AdResult -= AdsManager_AdResult;
        if (isSuccess)
        {
            SoundManager.PlaySfx(SoundManager.Instance.soundPurchased);
            GlobalValue.SavedCoins += rewarded;
        }
    }

    public void ResetGame()
    {
        PlayerPrefs.DeleteAll();
        RestartGame();
    }
}
