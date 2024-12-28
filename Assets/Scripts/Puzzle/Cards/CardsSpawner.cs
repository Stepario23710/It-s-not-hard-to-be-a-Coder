using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardsSpawner : MonoBehaviour
{
    [SerializeField] private Sprite[] cardsSprites;
    [SerializeField] private string[] cardsTypes;
    [SerializeField] private Sprite[] timersSprites;
    [SerializeField] private GameObject cardOrig;
    [SerializeField] private GameObject cardWithTimerOrig;
    [SerializeField] private int maxCardsQuantity;
    [SerializeField] private float distBeetweenCards;
    public float DistBeetweenCards => distBeetweenCards;
    public Sprite[] TimersSprites => timersSprites;
    public Vector2 cardDefaultPos => cardOrig.GetComponent<RectTransform>().localPosition;
    private Dictionary<string, Sprite> cardsObjects = new Dictionary<string, Sprite>();
    void Awake()
    {
        int cardsQuantity = GlobalVaribles.cards.Count;
        for (int i = 0; i < cardsSprites.Length; i++){
            cardsObjects.Add(cardsTypes[i], cardsSprites[i]);
        }
        for (int i = 0; i < cardsQuantity; i++){
            if (i == maxCardsQuantity){
                break;
            } else {
                AddCardToBattle(i);
            }
        }
    }
    private void AddCardToBattle(int koef){
        (string, int) card = GlobalVaribles.cards[koef];
        CardInicialisation createdCard;
        if (card.Item2 == 0){
            createdCard = Instantiate(cardOrig, gameObject.transform).GetComponent<CardInicialisation>();
        } else {
            createdCard = Instantiate(cardWithTimerOrig, gameObject.transform).GetComponent<CardInicialisation>();
        }
        createdCard.num = koef;
        createdCard.typeOfCard = card.Item1;
        createdCard.timer = card.Item2;
        createdCard.gameObject.GetComponent<Image>().sprite = cardsObjects[card.Item1];
        createdCard.gameObject.GetComponent<RectTransform>().localPosition = 
         new Vector2(cardOrig.GetComponent<RectTransform>().localPosition.x + distBeetweenCards * koef, 
          cardOrig.gameObject.GetComponent<RectTransform>().localPosition.y);
        UnityAction<CardInicialisation> action = new UnityAction<CardInicialisation>(gameObject.GetComponent<CardManager>().CardActivate);
        UnityEventTools.AddObjectPersistentListener<CardInicialisation>(createdCard.GetComponent<Button>().onClick, action, createdCard);
        if (card.Item2 != 0){
            createdCard.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = timersSprites[card.Item2 - 1];
        }
    }
    public void AddRemainingCardsToList() {
        GlobalVaribles.cards.Clear();
        for (int i = 0; i < transform.childCount; i++){
            var card = transform.GetChild(i).GetComponent<CardInicialisation>();
            GlobalVaribles.cards.Add((card.typeOfCard, card.timer));
        }
    }
}
