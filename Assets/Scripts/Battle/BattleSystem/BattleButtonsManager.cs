using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleButtonsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] menuesOrig;
    [SerializeField] private GameObject buttonsOfActsParent;
    [SerializeField] private PlayerActsManager playerActManager;
    [SerializeField] private Color notInteractableColorForText;
    private Dictionary<string, GameObject> menues;
    private GameObject actMenu;
    void Start()
    {
        menues = new Dictionary<string, GameObject>(){
            {"Act", menuesOrig[0]},
            //{"Items", menuesOrig[1]},
            {"SpecialAct", menuesOrig[2]}
        };
        playerActManager.gameObject.GetComponent<PlayerBattleHpSystem>().AnimationEnded += delegate{ActivateButtons();};
        playerActManager.enemyHpSystem.AnimationEnded += delegate{ActivateButtons();};
        if (GlobalVaribles.isPlayerFirstSmash){
            DoAct(null);
        }
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
           CloseMenu();
        }
    }
    public void OpenMenu(MenuButtonInicialisation menuInicialisation){
        DeactivateButtons();
        GameObject menu = Instantiate(menues[menuInicialisation.nameOfYourMenu], transform);
        actMenu = menu;
        Transform child;
        for(int i = 0; i < menu.transform.childCount; i++){
            child = menu.transform.GetChild(i);
            UnityAction<BattleActInicialisation> action = new UnityAction<BattleActInicialisation>(DoAct);
            UnityEventTools.AddObjectPersistentListener<BattleActInicialisation>(child.GetComponent<Button>().onClick, action,
            child.GetComponent<BattleActInicialisation>());
        }
    }
    private void DoAct(BattleActInicialisation actInicialisation){
        playerActManager.act = actInicialisation;
        playerActManager.Invoke("ActivateAct", 0.5f);
        CloseMenu();
        DeactivateButtons();
    }
    private void CloseMenu(){
        GameObject destMenu = null;
        if(actMenu != null){
            destMenu = actMenu;
            actMenu = null;
        }
        if (destMenu != null){
            Destroy(destMenu);
            ActivateButtons();
        }
    }
    private void ActivateButtons(){
        for(int i = 0; i < buttonsOfActsParent.transform.childCount; i++){
            buttonsOfActsParent.transform.GetChild(i).GetComponent<Button>().interactable = true;
            buttonsOfActsParent.transform.GetChild(i).GetChild(0).GetComponent<Text>().color = Color.white;
        }
    }
    private void DeactivateButtons(){
        for(int i = 0; i < buttonsOfActsParent.transform.childCount; i++){
            buttonsOfActsParent.transform.GetChild(i).GetComponent<Button>().interactable = false;
            buttonsOfActsParent.transform.GetChild(i).GetChild(0).GetComponent<Text>().color = notInteractableColorForText;
        }
    }
}
