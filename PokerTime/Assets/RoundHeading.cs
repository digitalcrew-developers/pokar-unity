using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundHeading : MonoBehaviour
{
    public GameObject PreflopCards;
    public Text RoundName, roundAmount;
    public Image image0, image1, image2, image3, image4;
    private string roundName;

    public void Init(string round, HandDetails handDetails)
    {
        PreflopCards.SetActive(false);
        switch (round)
        {
            case "preflop":
                //PreflopCards.SetActive(true);
                RoundName.text = "Preflop";
                roundAmount.text = handDetails.PREFLOP[0].currentPot.ToString();
                if (handDetails.PREFLOP[0].betType != "Fold Card" && PrefsManager.GetPlayerData().userName == handDetails.PREFLOP[0].userName)
                {
                    if (!string.IsNullOrEmpty(handDetails.PREFLOP[0].playerCards[0]))
                    {
                        CardData cardData1 = CardsManager.instance.GetCardData(handDetails.PREFLOP[0].playerCards[0]);
                        image0.sprite = cardData1.cardsSprite;

                    }
                    if (!string.IsNullOrEmpty(handDetails.PREFLOP[0].playerCards[1]))
                    {
                        CardData cardData2 = CardsManager.instance.GetCardData(handDetails.PREFLOP[0].playerCards[1]);
                        image1.sprite = cardData2.cardsSprite;

                    }
                }
                
                image0.gameObject.SetActive(true);
                image1.gameObject.SetActive(true);
                image2.gameObject.SetActive(false);
                image3.gameObject.SetActive(false);
                image4.gameObject.SetActive(false);
                break;
            case "postflop":
                RoundName.text = "Flop";
                roundAmount.text = handDetails.POSTFLOP[0].currentPot.ToString();
                if (!string.IsNullOrEmpty(handDetails.POSTFLOP[0].openCards[0]))
                {
                    CardData cardData1 = CardsManager.instance.GetCardData(handDetails.POSTFLOP[0].openCards[0]);
                    image0.sprite = cardData1.cardsSprite;

                }
                if (!string.IsNullOrEmpty(handDetails.POSTFLOP[0].openCards[1]))
                {
                    CardData cardData2 = CardsManager.instance.GetCardData(handDetails.POSTFLOP[0].openCards[1]);
                    image1.sprite = cardData2.cardsSprite;

                }
                if (!string.IsNullOrEmpty(handDetails.POSTFLOP[0].openCards[2]))
                {
                    CardData cardData3 = CardsManager.instance.GetCardData(handDetails.POSTFLOP[0].openCards[2]);
                    image2.sprite = cardData3.cardsSprite;
                }
                image0.gameObject.SetActive(true);
                image1.gameObject.SetActive(true);
                image2.gameObject.SetActive(true);
                image3.gameObject.SetActive(false); 
                image4.gameObject.SetActive(false);


                break;
            case "turn":
                RoundName.text = "Turn";
                roundAmount.text = handDetails.POSTTURN[0].currentPot.ToString();
                if (!string.IsNullOrEmpty(handDetails.POSTTURN[0].openCards[0]))
                {
                    CardData cardData1_1 = CardsManager.instance.GetCardData(handDetails.POSTTURN[0].openCards[0]);
                    image0.sprite = cardData1_1.cardsSprite;
                }
                if (!string.IsNullOrEmpty(handDetails.POSTTURN[0].openCards[1]))
                {
                    CardData cardData2_1 = CardsManager.instance.GetCardData(handDetails.POSTTURN[0].openCards[1]);
                    image1.sprite = cardData2_1.cardsSprite;
                }
                if (!string.IsNullOrEmpty(handDetails.POSTTURN[0].openCards[2]))
                {
                    CardData cardData3_1 = CardsManager.instance.GetCardData(handDetails.POSTTURN[0].openCards[2]);
                    image2.sprite = cardData3_1.cardsSprite;
                }
                if (!string.IsNullOrEmpty(handDetails.POSTTURN[0].openCards[3]))
                {
                    CardData cardData4_1 = CardsManager.instance.GetCardData(handDetails.POSTTURN[0].openCards[3]);
                    image3.sprite = cardData4_1.cardsSprite;
                }
                image0.gameObject.SetActive(true);
                image1.gameObject.SetActive(true);
                image2.gameObject.SetActive(true);
                image3.gameObject.SetActive(true);
                image4.gameObject.SetActive(false);

                break;
            case "river":
                RoundName.text = "River";
                roundAmount.text = handDetails.POSTRIVER[0].currentPot.ToString();
                if (!string.IsNullOrEmpty(handDetails.POSTRIVER[0].openCards[0]))
                {
                    CardData cardData1_2 = CardsManager.instance.GetCardData(handDetails.POSTRIVER[0].openCards[0]);
                    image0.sprite = cardData1_2.cardsSprite;
                }
                if (!string.IsNullOrEmpty(handDetails.POSTRIVER[0].openCards[1]))
                {
                    CardData cardData2_2 = CardsManager.instance.GetCardData(handDetails.POSTRIVER[0].openCards[1]);
                    image1.sprite = cardData2_2.cardsSprite;
                }
                if (!string.IsNullOrEmpty(handDetails.POSTRIVER[0].openCards[2]))
                {
                    CardData cardData3_2 = CardsManager.instance.GetCardData(handDetails.POSTRIVER[0].openCards[2]);
                    image2.sprite = cardData3_2.cardsSprite;
                }
                if (!string.IsNullOrEmpty(handDetails.POSTRIVER[0].openCards[3]))
                {
                    CardData cardData4_2 = CardsManager.instance.GetCardData(handDetails.POSTRIVER[0].openCards[3]);
                    image3.sprite = cardData4_2.cardsSprite;
                }
                if (!string.IsNullOrEmpty(handDetails.POSTRIVER[0].openCards[4]))
                {
                    CardData cardData5_2 = CardsManager.instance.GetCardData(handDetails.POSTRIVER[0].openCards[4]);
                    image4.sprite = cardData5_2.cardsSprite;
                }

                image0.gameObject.SetActive(true);
                image1.gameObject.SetActive(true);
                image2.gameObject.SetActive(true);
                image3.gameObject.SetActive(true);
                image4.gameObject.SetActive(true);


                break;
            case "showdown":
                RoundName.text = "Showdown";
                roundAmount.text = handDetails.SHOWDOWN[0].currentPot.ToString();
                if (!string.IsNullOrEmpty(handDetails.SHOWDOWN[0].openCards[0]))
                {
                    CardData cardData1_3 = CardsManager.instance.GetCardData(handDetails.SHOWDOWN[0].openCards[0]);
                    image0.sprite = cardData1_3.cardsSprite;
                }
                if (!string.IsNullOrEmpty(handDetails.SHOWDOWN[0].openCards[1]))
                {
                    CardData cardData2_3 = CardsManager.instance.GetCardData(handDetails.SHOWDOWN[0].openCards[1]);
                    image1.sprite = cardData2_3.cardsSprite;
                }
                if (!string.IsNullOrEmpty(handDetails.SHOWDOWN[0].openCards[2]))
                {
                    CardData cardData3_3 = CardsManager.instance.GetCardData(handDetails.SHOWDOWN[0].openCards[2]);
                    image2.sprite = cardData3_3.cardsSprite;
                }
                if (!string.IsNullOrEmpty(handDetails.SHOWDOWN[0].openCards[3]))
                {
                    CardData cardData4_3 = CardsManager.instance.GetCardData(handDetails.SHOWDOWN[0].openCards[3]);
                    image3.sprite = cardData4_3.cardsSprite;
                }
                if (!string.IsNullOrEmpty(handDetails.SHOWDOWN[0].openCards[4]))
                {
                    CardData cardData5_3 = CardsManager.instance.GetCardData(handDetails.SHOWDOWN[0].openCards[4]);
                    image4.sprite = cardData5_3.cardsSprite;
                }

                image0.gameObject.SetActive(true);
                image1.gameObject.SetActive(true);
                image2.gameObject.SetActive(true);
                image3.gameObject.SetActive(true);
                image4.gameObject.SetActive(true);

                break;
            default:
                break;
        }
    }

    public void ShowComCards(List<comCard> cards, int runit)
    {

        if (runit == 0)
            RoundName.text = "2nd Time";
        else if (runit == 1)
            RoundName.text = "3nd Time";
        roundAmount.text = "";

        image0.gameObject.SetActive(true);
        image1.gameObject.SetActive(true);
        image2.gameObject.SetActive(true);
        image3.gameObject.SetActive(true);
        image4.gameObject.SetActive(true);
        CardData cardData1_3 = CardsManager.instance.GetCardData(cards[runit].c0);
        image0.sprite = cardData1_3.cardsSprite;
        CardData cardData2_3 = CardsManager.instance.GetCardData(cards[runit].c1);
        image1.sprite = cardData2_3.cardsSprite;
        CardData cardData3_3 = CardsManager.instance.GetCardData(cards[runit].c2);
        image2.sprite = cardData3_3.cardsSprite;
        CardData cardData4_3 = CardsManager.instance.GetCardData(cards[runit].c3);
        image3.sprite = cardData4_3.cardsSprite;
        CardData cardData5_3 = CardsManager.instance.GetCardData(cards[runit].c4);
        image4.sprite = cardData5_3.cardsSprite;
    }
}
