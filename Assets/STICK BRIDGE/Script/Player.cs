using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, IListener {
    [Header("Shop item")]       //Use for shop setting
    public bool unlockDefault = false;  //no need gems for unlocking new character
    public int price = 10;  //gems needed to unlock
	[Header("Moving")]
	public float moveSpeed = 3;
    
    [Header("Sound")]
	public AudioClip deadSound;
    public AudioClip soundWalk;
    AudioSource audioWalkScr;

   [ReadOnly] public Vector2 input = Vector2.zero;
    bool isDead = false;

	[HideInInspector] public Animator anim;
    GameObject lastHitGround;

   [ReadOnly] public bool isPlaying;
	public bool isFinish { get; set;}

    bool forceStannding = false;
    Rigidbody2D rig;
    BoxCollider2D boxCollider;
    Bounds bounds;

    void Awake(){
		anim = GetComponent<Animator> ();
        boxCollider = GetComponent<BoxCollider2D>();
    }

	void Start() {
        rig = GetComponent<Rigidbody2D>();

        isPlaying = true;

        audioWalkScr = gameObject.AddComponent<AudioSource>();
        audioWalkScr.loop = true;
        audioWalkScr.clip = soundWalk;
        audioWalkScr.playOnAwake = false;
       
    }

    private void FixedUpdate()
    {
        HandleAnimation();
        rig.velocity = new Vector2(input.x * moveSpeed, rig.velocity.y);

        if (!isPlaying)
            return;

        if (!LevelManager.Instance.isBridgeReachToPlatform)
        {
            if (!isGroudBehide())
            {
                StopMove();
                Fall();
                var allColls = GetComponents<Collider2D>();
                foreach (var col in allColls)
                {
                    col.isTrigger = true;
                }
            }
        }
        else
        {
            if (!isGroudAhead() && rig.velocity.x != 0)
            {
                GameManager.Instance.SpawnNextPlatform();
                StopMove();
            }
        }
    }

    public bool isGroudAhead()
    {
        bounds = boxCollider.bounds;
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(bounds.max.x, bounds.min.y), Vector2.down, 1, GameManager.Instance.groundLayer);
        Debug.DrawRay(new Vector2(bounds.max.x, bounds.min.y), Vector2.down);
        if (hit)
        {
            lastHitGround = hit.collider.gameObject;
            return true;
        }
        else
            return false;
    }

    public bool isGroudBehide()
    {

        bounds = boxCollider.bounds;
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(bounds.min.x, bounds.min.y), Vector2.down, 1, GameManager.Instance.groundLayer);
        Debug.DrawRay(new Vector2(bounds.max.x, bounds.min.y), Vector2.down);
        if (hit)
            lastHitGround = hit.collider.gameObject;

        return hit;
    }

    public void PausePlayer(bool pause)
    {
        StopMove();
        isPlaying = !pause;
    }

    public void Fall()
    {
        LevelManager.Instance.BreakBridge();
        GameManager.Instance.GameOver();
        SoundManager.PlaySfx(deadSound);
        isPlaying = false;
    }

    public void Move()
    {
        input.x = 1;
        audioWalkScr.Play();
    }

	public void StopMove(){
		input = Vector2.zero;
        audioWalkScr.Stop();
    }

	void HandleAnimation(){
		anim.SetFloat ("speed", Mathf.Abs(rig.velocity.x));
    }

	void ResetAnimation(){
		anim.SetFloat ("speed", 0);
		anim.SetBool ("isGrounded", true);
		anim.SetTrigger ("reset");
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Star"))
        {
            collision.GetComponent<Star>().Collect();
        }
    }

    public void IPlay()
    {
        isPlaying = true;
        Move();
    }

    public void ISuccess()
    {

    }

    public void IPause()
    {
        audioWalkScr.volume = 0;
    }

    public void IUnPause()
    {
        audioWalkScr.volume = 1;
    }

    public void IGameOver()
    {
     
    }

    public void IOnRespawn()
    {
        
    }

    public void IOnStopMovingOn()
    {
       
    }

    public void IOnStopMovingOff()
    {
      
    }
}
