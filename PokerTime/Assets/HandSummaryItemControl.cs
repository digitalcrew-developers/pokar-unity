using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandSummaryItemControl : MonoBehaviour
{
    public TextMeshProUGUI ActionOneText;
    public TextMeshProUGUI PlayerNameText;
    public TextMeshProUGUI ActionTwoText;
    public TextMeshProUGUI BetText;

    public Image p_I1, p_I2; //normal game

    public Image p_image0, p_image1, p_image2, p_image3, p_image4;
    public Image c_image0, c_image1, c_image2, c_image3, c_image4;

    public Color GreenColor, RedColor;
    public string GreenColorString = "#33daaf";
    public string RedColorString = "#d45055";

    private HandSummary handSummary;

    public void Init(HandSummary _handSummary)
    {
        ColorUtility.TryParseHtmlString(GreenColorString, out GreenColor);
        ColorUtility.TryParseHtmlString(RedColorString, out RedColor);

        handSummary = _handSummary;

        ActionOneText.text = "" + handSummary.seatName;

        PlayerNameText.text = handSummary.userName;
        ActionTwoText.text = handSummary.handStrength;
        Debug.Log(handSummary.userName + " HandSummary " + handSummary.winAmount + ", " + handSummary.cards.Count + ", " + handSummary.communityCard.Count);
        if(handSummary.winAmount > 0)
        {
            BetText.text = "+" + handSummary.winAmount.ToString();
            BetText.color = GreenColor;
        }
        else
        {
            BetText.text =  handSummary.winAmount.ToString();
            BetText.color = RedColor;
        }

        //user cards 
        if(handSummary.handStrength != "Fold" && handSummary.cards.Count > 0)
        {
            if (!string.IsNullOrEmpty(handSummary.cards[0].ToString()))
            {
                CardData p1Card = CardsManager.instance.GetCardData(handSummary.cards[0].ToString());
                p_I1.sprite = p1Card.cardsSprite;
            }
            if (!string.IsNullOrEmpty(handSummary.cards[1].ToString()))
            {
                CardData p2Card = CardsManager.instance.GetCardData(handSummary.cards[1].ToString());
                p_I2.sprite = p2Card.cardsSprite;
            }
        }

        //community cards
        if (handSummary.communityCard.Count> 0)
        {
            if (!string.IsNullOrEmpty(handSummary.communityCard[0].ToString()))
            {
                CardData c1Card = CardsManager.instance.GetCardData(handSummary.communityCard[0].ToString());
                c_image0.sprite = c1Card.cardsSprite;
            }
            if (!string.IsNullOrEmpty(handSummary.communityCard[1].ToString()))
            {
                CardData c2Card = CardsManager.instance.GetCardData(handSummary.communityCard[1].ToString());
                c_image1.sprite = c2Card.cardsSprite;
            }
            if (!string.IsNullOrEmpty(handSummary.communityCard[2].ToString()))
            {
                CardData c3Card = CardsManager.instance.GetCardData(handSummary.communityCard[2].ToString());
                c_image2.sprite = c3Card.cardsSprite;
            }
            if (!string.IsNullOrEmpty(handSummary.communityCard[3].ToString()))
            {
                CardData c4Card = CardsManager.instance.GetCardData(handSummary.communityCard[3].ToString());
                c_image3.sprite = c4Card.cardsSprite;
            }
            if (!string.IsNullOrEmpty(handSummary.communityCard[4].ToString()))
            {
                CardData c5Card = CardsManager.instance.GetCardData(handSummary.communityCard[4].ToString());
                c_image4.sprite = c5Card.cardsSprite;
            }
        }
        

    }

}
