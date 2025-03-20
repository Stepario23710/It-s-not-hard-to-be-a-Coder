using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private float lifeObjGunRadius;
    [SerializeField] private Vector2 squatColOffset, squatColSize;
    [SerializeField] private GameObject feet;
    [SerializeField] private GameObject dubinkaSmashArea;
    [SerializeField] private GameObject usingLifeObj;
    [SerializeField] private GameObject lifeObjsOnScene;
    [SerializeField] private GameObject keyE;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask lifeObjGunLayer;
    private float moveVector;
    private bool isGrounded, isSquating, speedDown, jumpForceDown, jumpMade, canBeAttacked, isSmashing, isNearLifeCodeStation;
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
        if (GlobalVaribles.actLifeObjNum != -1){
            RememberLifeObj(lifeObjsOnScene.transform.GetChild(GlobalVaribles.actLifeObjNum).gameObject);
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
    void FixedUpdate()
    {
        if (GlobalVaribles.isCodeImplementing == true){
            keyE.transform.parent.gameObject.SetActive(false);
            rb.bodyType = RigidbodyType2D.Static;
            return;
        } else {
            keyE.transform.parent.gameObject.SetActive(true);
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
        moveVector = Input.GetAxisRaw("Horizontal");
        if (!isSmashing){
            Walk(moveVector);
        }
    }
    void Update()
    {
        if (GlobalVaribles.isCodeImplementing == true){
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && !isSmashing && isGrounded && !isSquating && !jumpMade){
            if (isNearLifeCodeStation){
                GlobalVaribles.lastPos = transform.position;
                lifeObjsOnScene.transform.parent.GetComponent<LifeSpawner>().SaveLifeObjs();
                SceneManager.LoadScene("LifeCodeStation");
            }
        }
        if (isGrounded){
            anim.SetBool("IsInAir", false); 
            if (isSquating){
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
        }
        if (!isGrounded){
            anim.SetBool("IsInAir", true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)){
            Time.timeScale = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)){
            Time.timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.F) && !isSmashing && isGrounded && !isSquating && !jumpMade){
            DubinkaSmash();
        }
        if (Input.GetKeyDown(KeyCode.Q) && !isSmashing && isGrounded && !isSquating && !jumpMade){
            UseLifeObjGun();
        }
        if (Input.GetKeyDown(KeyCode.S) && !isSmashing){
            Squat(true);
        } else if (Input.GetKeyUp(KeyCode.S)){
            Squat(false);
        }
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
        if (vect > 0 && transform.rotation.y != 0){
            if (!isGrounded || jumpMade || isSquating){
                Rotating();
            } else{
                anim.SetInteger("RotateStat", 1);
                anim.SetBool("IsWalk", true);
            }
        } else if (vect < 0 && transform.rotation.y == 0){
            if (!isGrounded || jumpMade || isSquating){
                Rotating();
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
            GlobalVaribles.lastSceneName = "World";
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
    public void Rotating(){
        anim.SetInteger("RotateStat", 0); 
        if (transform.rotation.y != 0){
            transform.rotation = Quaternion.Euler(0, 0, 0);
        } else{
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
    private void Squat(bool isSquat){
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
        Gizmos.color = Color.green;
        float dir = 1;
        if (transform.rotation.y != 0){
            dir = -1;
        }
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + dir * lifeObjGunRadius, transform.position.y));
    }
    private void SetBaseConfig(){
        GlobalVaribles.hp = 12;
        GlobalVaribles.maxHp = 14;
        GlobalVaribles.numOfScene = 0;
        GlobalVaribles.lastSceneName = "World";
        GlobalVaribles.lastPos = transform.position;
        GlobalVaribles.isPlayerWin = false;
        GlobalVaribles.isBaseConfigDone = true;
    }
    private void NowCanBeAttacked(){
        canBeAttacked = true;
    }
    private void UseLifeObjGun(){
        Vector2 dir = Vector2.right;
        if (transform.rotation.y != 0){
            dir = Vector2.left;
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, lifeObjGunRadius, lifeObjGunLayer);
        if (hit.collider != null){
            RememberLifeObj(hit.collider.gameObject);
        }
    }
    private void RememberLifeObj(GameObject go){
        GlobalVaribles.actLifeObjType = go.GetComponent<LifeObjInicialisation>().typeOfLifeObj;
        GlobalVaribles.actLifeObjNum = go.GetComponent<LifeObjInicialisation>().numOfLifeObjOnScene;
        Sprite spr = go.GetComponent<SpriteRenderer>().sprite;
        Color col = go.GetComponent<SpriteRenderer>().color;
        usingLifeObj.GetComponent<Image>().sprite = spr;
        usingLifeObj.GetComponent<Image>().color = col;
        usingLifeObj.GetComponent<Image>().SetNativeSize();
        RectTransform rt = usingLifeObj.GetComponent<RectTransform>();
        while (rt.sizeDelta.x > 160 || rt.sizeDelta.y > 160){
            rt.sizeDelta = new Vector2(rt.sizeDelta.x / 10 * 9, rt.sizeDelta.y / 10 * 9);
        }
    }
    void OnTriggerEnter2D(Collider2D col){
        if (col.gameObject.tag == "LCS"){
            GlobalVaribles.actLCSNum = col.gameObject.GetComponent<LCSInicialisation>().numOfLCSOnScene;
            isNearLifeCodeStation = true;
            keyE.SetActive(true);
        }
    }
    void OnTriggerExit2D(Collider2D col){
        if (col.gameObject.tag == "LCS"){
            isNearLifeCodeStation = false;
            keyE.SetActive(false);
        }
    }
}
