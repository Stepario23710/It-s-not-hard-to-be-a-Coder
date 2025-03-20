using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyBattleHpSystem : MonoBehaviour
{
    [SerializeField] protected UnityEngine.Vector2 textOfDamageOrHealOrigPos;
    [SerializeField] protected GameObject textOfDamageOrHealOrig;
    [SerializeField] protected GameObject canvas;
    [SerializeField] protected Color damageColor;
    [SerializeField] protected Color healColor;
    protected int maxHp;
    protected int hp;
    protected Animator anim;
    protected GameObject actTextOfDamageOrHeal;
    protected TextAnchor textOfDamageOrHealAlignment;
    public event Action AnimationEnded;
    public event Action EnemyDead;
    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        maxHp = UnityEngine.Random.Range(GlobalVaribles.actEnemy.minHp, GlobalVaribles.actEnemy.maxHp + 1);
        hp = maxHp;
        textOfDamageOrHealAlignment = TextAnchor.MiddleLeft;
        print(hp); //убрать
    }
    public void Damage(int value){
        if (value < 0){
            value = 0;
        }
        hp -= value;
        if (hp < 0){
            hp = 0;
        }
        InstantiateTextOfDamageOrHeal(damageColor, "-", value);
        anim.SetInteger("Stat", -1);
        print(hp); //убрать
    }
    public void Heal(int value){
        if (value < 0){
            value = 0;
        }
        hp += value;
        if (hp > maxHp){
            hp = maxHp;
        }
        InstantiateTextOfDamageOrHeal(healColor, "+", value);
        anim.SetInteger("Stat", 1);
    }
    public void EndAnim(){
        anim.SetInteger("Stat", 0);
        if (!DeathExamination()){
            AnimationEnded?.Invoke();
        }
    }
    protected bool DeathExamination(){
        if (hp <= 0){
            anim.SetBool("IsDeath", true);
            return true;
        } else {
            return false;
        }
    }
    protected void InstantiateTextOfDamageOrHeal(Color color, string minOrPl, int val){
        actTextOfDamageOrHeal = Instantiate(textOfDamageOrHealOrig, canvas.transform);
        actTextOfDamageOrHeal.GetComponent<RectTransform>().transform.localPosition = 
         new UnityEngine.Vector2(textOfDamageOrHealOrigPos.x, textOfDamageOrHealOrigPos.y);
        actTextOfDamageOrHeal.GetComponent<Text>().text = minOrPl + val.ToString();
        actTextOfDamageOrHeal.GetComponent<Text>().color = color;
        actTextOfDamageOrHeal.GetComponent<Text>().alignment = textOfDamageOrHealAlignment;
        actTextOfDamageOrHeal.GetComponent<Animator>().Play("TextOfDamageOrHealAnim");
        Invoke(nameof(DestroyTextOfDamageOrHeal), 1f);
    }
    protected void DestroyTextOfDamageOrHeal(){
        Destroy(actTextOfDamageOrHeal);
        actTextOfDamageOrHeal = null;
    }
    public virtual void Death(){
        EnemyDead?.Invoke();
        gameObject.SetActive(false);
        GlobalVaribles.isPlayerWin = true;
        GlobalVaribles.aliveEnemiesOnScenes[GlobalVaribles.numOfScene][GlobalVaribles.actEnemyNum] = false;
        GlobalVaribles.actEnemy = null;
        GlobalVaribles.actEnemyNum = -1;
        SceneManager.LoadScene(GlobalVaribles.lastSceneName);
    }
}
