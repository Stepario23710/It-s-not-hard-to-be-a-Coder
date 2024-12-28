using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float speedDownValue;
    [SerializeField] private float jumpForceDownValue;
    [SerializeField] private float jumpStartTime;
    [SerializeField] private float cantBeAttackedTime;
    [SerializeField] private float isGroundCheckRadius;
    [SerializeField] private float dubinkaSmashRadius;
    [SerializeField] private float whenDubinkaSmashDone;
    [SerializeField] private Vector2 squatColOffset, squatColSize;
    [SerializeField] private GameObject feet;
    [SerializeField] private GameObject dubinkaSmashArea;
    [SerializeField] private LayerMask groundLayer;
    private float moveVector;
    private bool isGrounded, isSquating, speedDown, jumpForceDown, jumpMade, canBeAttacked, isSmashing;
    private Vector2 baseColSize, baseColOffset;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private BoxCollider2D col;
    void Awake()
    {
        if (!GlobalVaribles.isBaseConfigDone){
            SetBaseConfig();
        }
        canBeAttacked = false;
    }
    void Start()
    {
        if(GlobalVaribles.isPlayerWin){
            GlobalVaribles.isPlayerWin = false;
            transform.position = GlobalVaribles.lastPosBeforeBattle;
            Invoke(nameof(NowCanBeAttacked), cantBeAttackedTime);
        } else{
            transform.position = GlobalVaribles.lastPos;
            NowCanBeAttacked();
        }
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        isGrounded = true;
        isSquating = false;
        speedDown = false;
        jumpMade = false;
        isSmashing = false;
        baseColSize = col.size;
        baseColOffset = col.offset;
    }
    void Update()
    {
        if (isGrounded){
            anim.SetBool("IsInAir", false); 
            if (isSquating){
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
        }
        if (!isGrounded){
            anim.SetBool("IsInAir", true);
        }
        if (Input.GetKeyDown(KeyCode.F) && !isSmashing && isGrounded){
            DubinkaSmash();
        }
        if (Input.GetKeyDown(KeyCode.S)){
            Squat(true);
        } else if (Input.GetKeyUp(KeyCode.S)){
            Squat(false);
        }
        moveVector = Input.GetAxisRaw("Horizontal");
        Walk(moveVector);
        isGrounded = Physics2D.OverlapCircle(feet.transform.position, isGroundCheckRadius, groundLayer);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !jumpMade && !isSmashing){
            anim.SetBool("IsJump", true); 
            jumpMade = true;
            Invoke(nameof(Jump), jumpStartTime);
        }
        if (isGrounded && speedDown){
            speed += speedDownValue;
            speedDown = false;
        }
        if (!isGrounded && !speedDown){
            speed -= speedDownValue;
            speedDown = true;
        }
    }
    private void Walk(float vect){
        if (!isSmashing){
            if (vect > 0 && sprite.flipX){
                if (!isGrounded || jumpMade || isSquating){
                    Rotate();
                } else{
                    anim.SetInteger("RotateStat", 1);
                    anim.SetBool("IsWalk", true);
                }
            } else if (vect < 0 && !sprite.flipX){
                if (!isGrounded || jumpMade || isSquating){
                    Rotate();
                } else{
                    anim.SetInteger("RotateStat", -1);
                    anim.SetBool("IsWalk", true);
                }
            } else {
                if (vect == 0){
                    anim.SetBool("IsWalk", false);
                } else {
                    anim.SetBool("IsWalk", true);
                }
            }
            if(!isSquating || (isSquating && !isGrounded)){
                rb.velocity = new Vector2(vect * speed, rb.velocity.y);
            }
        }
    }
    private void Jump(){ 
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        jumpMade = false;
        anim.SetBool("IsJump", false); 
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && canBeAttacked)
        {
            GlobalVaribles.actEnemyNum = collision.gameObject.GetComponent<EnemyInicialisation>().num;
            GlobalVaribles.actEnemy = collision.gameObject.GetComponent<EnemyInicialisation>().typeOfEnemy;
            GlobalVaribles.lastPosBeforeBattle = transform.position;
            SceneManager.LoadScene("Battle");
        }
        if(collision.gameObject.tag == "Platform" && isGrounded){
            transform.parent = collision.transform;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Platform"){
            transform.parent = null;
        }
    }
    public void Rotate(){
        anim.SetInteger("RotateStat", 0); 
        sprite.flipX = !sprite.flipX;
    }
    private void Squat(bool isSquat){
        if (!isSmashing){
            if (isSquat){
                if (isGrounded){
                    anim.SetBool("IsSquat", true);
                    isSquating = true;
                    rb.velocity = Vector2.zero;
                    if(!jumpForceDown){
                        jumpForce -= jumpForceDownValue;
                        jumpForceDown = true;
                    }
                    col.size = squatColSize;
                    col.offset = squatColOffset;
                }
            } else {
                anim.SetBool("IsSquat", false);
                isSquating = false;
                if(jumpForceDown){
                    jumpForce += jumpForceDownValue;
                    jumpForceDown = false;
                }
                col.size = baseColSize;
                col.offset = baseColOffset;
            }
        }
    }
    private void DubinkaSmash(){
        anim.SetBool("IsDubinkaSmash", true);
        isSmashing = true;
        rb.velocity = Vector2.zero;
        Invoke(nameof(DubinkaSmashDone), whenDubinkaSmashDone);
    }
    private void DubinkaSmashDone(){
        anim.SetBool("IsDubinkaSmash", false);
        Collider2D[] hitObjs = Physics2D.OverlapCircleAll(dubinkaSmashArea.transform.position, dubinkaSmashRadius);
        foreach (Collider2D hitObj in hitObjs){
            if (hitObj.gameObject.tag == "Enemy"){
                GlobalVaribles.actEnemyNum = hitObj.gameObject.GetComponent<EnemyInicialisation>().num;
                GlobalVaribles.actEnemy = hitObj.gameObject.GetComponent<EnemyInicialisation>().typeOfEnemy;
                GlobalVaribles.lastPosBeforeBattle = transform.position;
                GlobalVaribles.isPlayerFirstSmash = true;
                SceneManager.LoadScene("Battle");
            }
        }
        isSmashing = false;
    }
    private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(feet.transform.position, isGroundCheckRadius);
        Gizmos.DrawSphere(dubinkaSmashArea.transform.position, dubinkaSmashRadius);
    }
    private void SetBaseConfig(){
        GlobalVaribles.hp = 12;
        GlobalVaribles.maxHp = 14;
        GlobalVaribles.cp = 2;
        GlobalVaribles.maxCp = 2;
        GlobalVaribles.fp = 5;
        GlobalVaribles.maxFp = 5;
        GlobalVaribles.numOfScene = 0;
        GlobalVaribles.sceneName = "World";
        GlobalVaribles.lastSceneNameBeforeBattle = "World";
        GlobalVaribles.lastPos = transform.position;
        GlobalVaribles.isPlayerWin = false;
        GlobalVaribles.cards = new List<(string, int)>(){
            ("input()", 0),
            ("--", 2),
            ("-=", 1),
            ("+=", 3),
            ("print()", 1)
        };
        GlobalVaribles.isBaseConfigDone = true;
    }
    private void NowCanBeAttacked(){
        canBeAttacked = true;
    }
}
