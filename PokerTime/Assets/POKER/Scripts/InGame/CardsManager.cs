using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;

public class CardsManager: MonoBehaviour
{
    public static CardsManager instance;
    [SerializeField]
    private Sprite[] cardSprites;
    private Sprite cardBackSprite;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        cardSprites = Resources.LoadAll<Sprite>("cards");
        cardBackSprite = cardSprites[52];
    }



    public Sprite GetCardBackSideSprite()
    {
        return cardBackSprite;
    }

    #region Card_Prioirt_Calculation

    //public void FindCardSequence(int roundIndex, Action<string> callBackMethod)
    //{
    //    StartCoroutine(WaitAndFindBestCardSequence(roundIndex, callBackMethod));
    //}



    //private IEnumerator WaitAndFindBestCardSequence(int roundIndex, Action<string> callBackMethod)
    //{
    //    yield return new WaitForEndOfFrame();


    //    List<CardData> availableCards = new List<CardData>();
    //    List<CardData> userCards = new List<CardData>();

    //    CardData[] playersCard = InGameManagerScript.instance.GetMyPlayerObject().GetCards();
    //    CardSequence bestCardSequence = CardSequence.High_Card;

    //    for (int i = 0; i < playersCard.Length; i++)
    //    {
    //        availableCards.Add(playersCard[i]);
    //        userCards.Add(playersCard[i]);
    //    }


    //    switch (roundIndex)
    //    {
    //        case 0:
    //            {
    //                int totalCardsCount = 3;

    //                for (int i = 0; i < totalCardsCount; i++)
    //                {
    //                    availableCards.Add(openCards[i]);
    //                }

    //                bestCardSequence = GetCardSequence(availableCards);
    //            }
    //            break;

    //        case 1:
    //            {
    //                int cardToIgnore = 0;
    //                int totalCardsCount = openCards.Length - 1;

    //                for (int counter = 0; counter < totalCardsCount; counter++)
    //                {
    //                    availableCards.Clear();
    //                    availableCards.AddRange(userCards);

    //                    for (int i = 0; i < totalCardsCount; i++)
    //                    {
    //                        if (i != cardToIgnore)
    //                        {
    //                            availableCards.Add(openCards[i]);
    //                        }
    //                    }

    //                    Debug.Log("comparing cards = " + availableCards.Count + "  cardToIgnore = " + cardToIgnore);

    //                    CardSequence tempCardSequence = GetCardSequence(availableCards);

    //                    if (tempCardSequence < bestCardSequence)
    //                    {
    //                        bestCardSequence = tempCardSequence;
    //                    }

    //                    ++cardToIgnore;
    //                    yield return new WaitForEndOfFrame();
    //                }

    //            }

    //            break;

    //        case 2:
    //            {

    //                List<CardData> allCards = new List<CardData>();
    //                allCards.AddRange(userCards);

    //                for (int i = 0; i < openCards.Length; i++)
    //                {
    //                    allCards.Add(openCards[i]);
    //                }

    //                int totalCardsCount = allCards.Count;
    //                int numberOfCardsRequired = 5;
    //                int maxGap = totalCardsCount - (numberOfCardsRequired - 1);

    //                for (int counter = 1; counter <= maxGap; counter++)
    //                {
    //                    for (int i = 0; i < totalCardsCount; i++)
    //                    {
    //                        int count = 0, maxCount = 4, index = i + counter;

    //                        availableCards.Clear();
    //                        //availableCards.AddRange(userCards);
    //                        availableCards.Add(allCards[i]);
    //                        //4 
    //                        while (count < maxCount)
    //                        {
    //                            if (index >= totalCardsCount)
    //                            {
    //                                int additionalValue = index - totalCardsCount;
    //                                index = additionalValue;
    //                            }

    //                            availableCards.Add(allCards[index]);

    //                            ++index;
    //                            ++count;

    //                            yield return new WaitForEndOfFrame();
    //                        }

    //                        CardSequence tempCardSequence = GetCardSequence(availableCards);

    //                        if (tempCardSequence < bestCardSequence)
    //                        {
    //                            bestCardSequence = tempCardSequence;
    //                        }
    //                    }
    //                }

    //            }
    //            break;

    //        default:
    //        Debug.LogError("Unhandled round no found " + roundIndex);
    //        break;
    //    }

    //    if (callBackMethod != null)
    //    {
    //        callBackMethod("" + bestCardSequence);
    //    }
    //}


    public CardSequence GetCardSequence(List<CardData> availableCards)
    {
        availableCards = SortCard(availableCards);

        bool isAllInSequence = true, isAllIconSame = true;
        CardIcon firstCardIcon = availableCards[0].cardIcon;
        bool isAceAtFront = false;
        List<CardOccurrenceData> cardOccurrance = new List<CardOccurrenceData>();

        if (availableCards[0].cardNumber == CardNumber.ACE)
        {
            isAceAtFront = true;
        }


        for (int i = 0; i < availableCards.Count - 1; i++)
        {

            bool isMatchFound = false;
            for (int j = 0; j < cardOccurrance.Count; j++)
            {
                if (cardOccurrance[j].cardNumber == availableCards[i].cardNumber)
                {
                    ++cardOccurrance[j].occurranceCount;
                    isMatchFound = true;
                    j = 100;
                }
            }


            if (!isMatchFound)
            {
                CardOccurrenceData cardOccurrenceData = new CardOccurrenceData();
                cardOccurrenceData.cardNumber = availableCards[i].cardNumber;
                cardOccurrenceData.occurranceCount = 1;
                cardOccurrance.Add(cardOccurrenceData);
            }

            if (availableCards[i].cardNumber == CardNumber.ACE)
            {
                if (isAceAtFront)
                {
                    if (availableCards[i + 1].cardNumber != CardNumber.TWO)
                    {
                        isAllInSequence = false;
                    }
                }
                else // more than 1 Ace at the end
                {
                    isAllInSequence = false;
                }
            }
            else
            {
                if ((int)availableCards[i].cardNumber + 1 != (int)availableCards[i + 1].cardNumber)
                {
                    isAllInSequence = false;
                }
            }


            if (availableCards[i].cardIcon != firstCardIcon)
            {
                isAllIconSame = false;
            }
        }


        bool isMatchFound2 = false;
        for (int j = 0; j < cardOccurrance.Count; j++)
        {
            if (cardOccurrance[j].cardNumber == availableCards[availableCards.Count - 1].cardNumber)
            {
                ++cardOccurrance[j].occurranceCount;
                isMatchFound2 = true;
                break;
            }
        }


        if (!isMatchFound2)
        {
            CardOccurrenceData cardOccurrenceData = new CardOccurrenceData();
            cardOccurrenceData.cardNumber = availableCards[availableCards.Count - 1].cardNumber;
            cardOccurrenceData.occurranceCount = 1;
            cardOccurrance.Add(cardOccurrenceData);
        }

        if (availableCards[availableCards.Count - 1].cardIcon != firstCardIcon)
        {
            isAllIconSame = false;
        }


        if (isAllInSequence)
        {
            if (isAllIconSame)
            {
                if (IsContainsCard(availableCards, CardNumber.KING) && IsContainsCard(availableCards, CardNumber.ACE))
                {
                    return CardSequence.Royal_Flush; // All Card in Sequence and cardIcon is also same with highest Cards
                }
                else
                {
                    return CardSequence.Straight_Flush; // All Card in Sequence and cardIcon is also same without highest cards
                }
            }
        }


        int twoPairCount = 0;
        bool isThreePairAvailable = false;

        for (int i = 0; i < cardOccurrance.Count; i++)
        {

            if (cardOccurrance[i].occurranceCount >= 4)
            {
                return CardSequence.Four_of_a_Kind; // Four cards of same kind
            }

            if (cardOccurrance[i].occurranceCount >= 3)
            {
                isThreePairAvailable = true;
            }
            else if (cardOccurrance[i].occurranceCount >= 2)
            {
                ++twoPairCount;
            }

        }



        if (isThreePairAvailable && twoPairCount > 0)
        {
            return CardSequence.Full_House; // Three of a kind and one pair
        }


        if (isAllIconSame)
        {
            return CardSequence.Flush; // five cards of same Icon
        }

        if (isAllInSequence)
        {
            return CardSequence.Straight; // all cards in sequence but icon is not in sequence
        }

        if (isThreePairAvailable)
        {
            return CardSequence.Three_of_a_Kind; // Three of a kind without pair
        }

        if (twoPairCount > 1)
        {
            return CardSequence.Two_Pairs; // pair of two cards with same number 
        }

        if (twoPairCount > 0)
        {
            return CardSequence.Pair; // two cards with same number
        }

        return CardSequence.High_Card;
    }





    private List<CardData> SortCard(List<CardData> cards)
    {
        List<CardData> availableCards = new List<CardData>(cards);
        List<CardData> aceCards = new List<CardData>();

        for (int i = 0; i < availableCards.Count; i++)
        {
            if (availableCards[i].cardNumber == CardNumber.ACE)
            {
                aceCards.Add(availableCards[i]);
                availableCards.RemoveAt(i);
                --i;
            }
        }


        for (int counter = 0; counter < availableCards.Count; counter++)
        {
            for (int j = 0; j < availableCards.Count - 1; j++)
            {
                if (availableCards[j].cardNumber > availableCards[j + 1].cardNumber)
                {
                    CardData temp = availableCards[j];
                    availableCards[j] = availableCards[j + 1];
                    availableCards[j + 1] = temp;
                }
            }
        }



        if (IsContainsCard(availableCards, CardNumber.KING))
        {
            availableCards.AddRange(aceCards);
            return availableCards;
        }
        else
        {
            aceCards.AddRange(availableCards);
            return aceCards;
        }
    }


    private bool IsContainsCard(List<CardData> availableCards, CardNumber cardNumber)
    {
        for (int i = 0; i < availableCards.Count; i++)
        {
            if (availableCards[i].cardNumber == cardNumber)
            {
                return true;
            }
        }

        return false;
    }

    #endregion



    public CardData GetCardData(string serverGivenCardName)
    {
        CardData data = new CardData();

        switch (serverGivenCardName[0])
        {
            case 'T':
            data.cardNumber = CardNumber.TEN;
            break;

            case 'J':
            data.cardNumber = CardNumber.JACK;
            break;

            case 'Q':
            data.cardNumber = CardNumber.QUEEN;
            break;

            case 'K':
            data.cardNumber = CardNumber.KING;
            break;

            case 'A':
            data.cardNumber = CardNumber.ACE;
            break;

            default:
            int numberIndex = int.Parse(serverGivenCardName[0].ToString());
            data.cardNumber = (CardNumber)(numberIndex - 2);
            break;
        }


        switch (serverGivenCardName[1])
        {
            case 'c':
            data.cardIcon = CardIcon.CLUB;
            break;

            case 'd':
            data.cardIcon = CardIcon.DIAMOND;
            break;

            case 'h':
            data.cardIcon = CardIcon.HEART;
            break;

            case 's':
            data.cardIcon = CardIcon.SPADES;
            break;

            default:
            int numberIndex = int.Parse(serverGivenCardName[0].ToString());
            data.cardNumber = (CardNumber)(numberIndex - 2);
            break;
        }

        int totalCardNumbers = Enum.GetNames(typeof(CardNumber)).Length - 1; 
        int totalCardIcons = Enum.GetNames(typeof(CardIcon)).Length - 1; 


        int cardNumber = totalCardNumbers - (int)data.cardNumber; // reverse order
        int cardIcon = totalCardIcons - (int)data.cardIcon; // reverse order

        
        data.cardsSprite = cardSprites[(cardIcon * 13) + cardNumber];
        return data;
    }
 
}





[System.Serializable]
public class CardData
{
    public CardIcon cardIcon;
    public CardNumber cardNumber;
    public Sprite cardsSprite;
}

[System.Serializable]
public class CardOccurrenceData
{
    public int occurranceCount;
    public CardNumber cardNumber;
}


[System.Serializable]
public enum CardIcon
{
    CLUB,
    SPADES,
    DIAMOND,
    HEART
}


[System.Serializable]
public enum CardNumber
{
    TWO,
    THREE,
    FOUR,
    FIVE,
    SIX,
    SEVEN,
    EIGHT,
    NINE,
    TEN,
    JACK,
    QUEEN,
    KING,
    ACE
}


[System.Serializable]
public enum CardSequence
{
    Royal_Flush,
    Straight_Flush,
    Four_of_a_Kind,
    Full_House,
    Flush,
    Straight,
    Three_of_a_Kind,
    Two_Pairs,
    Pair,
    High_Card
}
