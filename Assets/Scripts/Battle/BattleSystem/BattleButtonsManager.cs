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
    [SerializeField] private GameObject[] subMenuesForActsOrig;
    [SerializeField] private GameObject[] subMenuesForCreaturesOrig;
    [SerializeField] private GameObject buttonsOfActsParent;
    [SerializeField] private PlayerActsManager playerActManager;
    [SerializeField] private Color notInteractableColorForText;
    private Dictionary<string, GameObject> menues;
    private GameObject actMenu;
    private GameObject actSubMenu;
    void Start()
    {
        menues = new Dictionary<string, GameObject>(){
            {"Act", menuesOrig[0]},
            //{"Items", menuesOrig[1]},
            {"Creatures", menuesOrig[2]},
            {"Dubinka", subMenuesForActsOrig[0]},
            {"Rogatka", subMenuesForActsOrig[1]},
            {"Convert", subMenuesForActsOrig[2]},
            {"Deer", subMenuesForCreaturesOrig[0]},
            {"Hare", subMenuesForCreaturesOrig[1]},
            //{"Cat-Snake", subMenuesForCreaturesOrig[2]},
        };//добавлять при необходимости
        playerActManager.gameObject.GetComponent<PlayerBattleHpSystem>().AnimationEnded += delegate{ActivateButtons(buttonsOfActsParent);};
        playerActManager.enemyHpSystem.AnimationEnded += delegate{ActivateButtons(buttonsOfActsParent);};
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
        GameObject menu;
        if (actMenu == null){
            menu = buttonsOfActsParent;
        } else {
            menu = actMenu;
        }
        DeactivateButtons(menu);
        menu = Instantiate(menues[menuInicialisation.nameOfYourMenu], transform);
        if (actMenu == null){
            actMenu = menu;
        } else {
            actSubMenu = menu;
        }
        Transform child;
        for(int i = 0; i < menu.transform.childCount; i++){
            child = menu.transform.GetChild(i);
            if (child.GetComponent<MenuButtonInicialisation>() != null){
                UnityAction<MenuButtonInicialisation> action = new UnityAction<MenuButtonInicialisation>(OpenMenu);
                UnityEventTools.AddObjectPersistentListener<MenuButtonInicialisation>(child.GetComponent<Button>().onClick, action,
                 child.GetComponent<MenuButtonInicialisation>());
            }
            if(child.GetComponent<BattleActInicialisation>() != null){
                if (playerActManager.fp < child.GetComponent<BattleActInicialisation>().needFp || 
                 playerActManager.cp < child.GetComponent<BattleActInicialisation>().needCp){
                    child.GetComponent<Button>().interactable = false;
                    child.GetChild(0).GetComponent<Text>().color = notInteractableColorForText;
                    child.GetChild(1).GetComponent<Text>().color = notInteractableColorForText;
                } else {
                    UnityAction<BattleActInicialisation> action = new UnityAction<BattleActInicialisation>(DoAct);
                    UnityEventTools.AddObjectPersistentListener<BattleActInicialisation>(child.GetComponent<Button>().onClick, action,
                     child.GetComponent<BattleActInicialisation>());
                }
            }
        }
    }
    private void DoAct(BattleActInicialisation actInicialisation){
        playerActManager.act = actInicialisation;
        playerActManager.Invoke("ActivateAct", 0.5f);
        CloseMenu();
        CloseMenu();
        DeactivateButtons(buttonsOfActsParent);
    }
    private void CloseMenu(){
        GameObject destMenu = null;
        GameObject menu = null;
        if(actSubMenu != null){
            destMenu = actSubMenu;
            menu = actMenu;
            actSubMenu = null;
        } else {
            if(actMenu != null){
                destMenu = actMenu;
                menu = buttonsOfActsParent;
                actMenu = null;
            }
        }
        if (destMenu != null){
            Destroy(destMenu);
            ActivateButtons(menu);
        }
    }
    private void ActivateButtons(GameObject menu){
        for(int i = 0; i < menu.transform.childCount; i++){
            menu.transform.GetChild(i).GetComponent<Button>().interactable = true;
            menu.transform.GetChild(i).GetChild(0).GetComponent<Text>().color = Color.white;
        }
    }
    private void DeactivateButtons(GameObject menu){
        for(int i = 0; i < menu.transform.childCount; i++){
            menu.transform.GetChild(i).GetComponent<Button>().interactable = false;
            menu.transform.GetChild(i).GetChild(0).GetComponent<Text>().color = notInteractableColorForText;
        }
    }
}
