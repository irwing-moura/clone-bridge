using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Menu, Playing, Dead, Finish, Waiting };
    public GameState State { get; set; }

    public LayerMask groundLayer;

    LevelManager levelManager;

    public bool isWatchingAd { get; set; }
    
    private MenuManager menuManager;

    [ReadOnly] public Vector2 currentCheckpoint = Vector2.zero;

    public bool isSpecialBullet { get; set; }

    public Player Player { get; private set; }
    SoundManager soundManager;

    [HideInInspector]
    public bool isNoLives = false;

    public int MissionStarCollected { get; set; }

    public CharacterContainer characterList { get; set; }

    int playerBeginPos;
    public int PlayerScore
    {
        get;set;
    }

    void Awake()
    {
        isSpecialBullet = false;
        Instance = this;
        State = GameState.Menu;
        Player = FindObjectOfType<Player>();
        levelManager = FindObjectOfType<LevelManager>();
        characterList = GetComponent<CharacterContainer>();
        playerBeginPos = (int) Player.transform.position.x;
        //MissionStarCollected = 0;
    }

    public void SpawnPlayer()
    {
        if (characterList)
        {
            GameObject _playerSpawn = Instantiate(characterList.GetPlayer(), Player.transform.position, Player.transform.rotation) as GameObject;
            Destroy(Player.gameObject);
            Player = _playerSpawn.GetComponent<Player>();
        }
    }

    void Start()
    {
        menuManager = FindObjectOfType<MenuManager>();
        currentCheckpoint = Player.transform.position;

        soundManager = FindObjectOfType<SoundManager>();

        SoundManager.PlaySfx(SoundManager.Instance.beginSoundInMainMenu);

        SpawnPlayer();
    }

    public void AllowPlayerMove()
    {
        Player.Move();
    }

    public void StartGame()
    {
        State = GameState.Playing;

        var listener_ = FindObjectsOfType<MonoBehaviour>().OfType<IListener>();
        foreach (var _listener in listener_)
        {
            _listener.IPlay();
        }

        if (SoundManager.Instance)
            SoundManager.PlayMusic(SoundManager.Instance.musicsGame);
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            var listener_ = FindObjectsOfType<MonoBehaviour>().OfType<IListener>();
            foreach (var _listener in listener_)
            {
                _listener.IPause();
            }
        }
        else
        {
            var listener_ = FindObjectsOfType<MonoBehaviour>().OfType<IListener>();
            foreach (var _listener in listener_)
            {
                _listener.IUnPause();
            }
        }
    }

    public void SpawnNextPlatform()
    {
        levelManager.SpawnNextPlatform();
    }

    public void GameOver(bool forceGameOver = false)
    {
        StartCoroutine(GameOverCo(forceGameOver));
    }

    public IEnumerator GameOverCo(bool forceGameOver = false)
    {
        if (State == GameState.Dead)
            yield break;

        if (State != GameState.Dead && State != GameState.Waiting && AdsManager.Instance)
        {
            AdsManager.Instance.ShowNormalAd(GameManager.GameState.Dead);
        }

        var listener_ = FindObjectsOfType<MonoBehaviour>().OfType<IListener>();
        foreach (var _listener in listener_)
        {
            _listener.IGameOver();
        }

        //check high score
        if (PlayerScore > GlobalValue.BestScore)
            GlobalValue.BestScore = PlayerScore;

        //ControllerInput.Instance.StopMove();
        State = GameState.Dead;

        MenuManager.Instance.GameOver();
        
        SoundManager.Instance.PauseMusic(true);

    }

    public void ResetLevel()
    {
        MenuManager.Instance.RestartGame();
    }
}
