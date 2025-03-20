using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerBattleHpSystem : EnemyBattleHpSystem 
{
    [SerializeField] private Text playerHPText;
    protected override void Start()
    {
        anim = GetComponent<Animator>();
        maxHp = GlobalVaribles.maxHp;
        hp = GlobalVaribles.hp;
        playerHPText.text = hp.ToString() + "/" + maxHp.ToString();
        textOfDamageOrHealAlignment = TextAnchor.MiddleRight;
        base.EnemyDead += SaveHp;
    }
    void Update()
    {
        playerHPText.text = hp.ToString() + "/" + maxHp.ToString();
    }
    public override void Death(){
        gameObject.SetActive(false);
        GlobalVaribles.isPlayerWin = false;
        GlobalVaribles.actEnemy = null;
        GlobalVaribles.actEnemyNum = -1;
        SceneManager.LoadScene(GlobalVaribles.lastSceneName);
    }
    private void SaveHp(){
        GlobalVaribles.hp = hp;
    }
}
