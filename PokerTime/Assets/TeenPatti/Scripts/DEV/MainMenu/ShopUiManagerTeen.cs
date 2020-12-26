using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.Linq;

public class ShopUiManagerTeen: MonoBehaviour
{
    public static ShopUiManagerTeen instance = null;

    public Button itemButton, diamondButton, pointButton;
    public Text coinsText, diamondText, pointText;
    public Transform container, itemContainer, hotPickContainer, vipCardContainer, goldContainer;

    //DEV_CODE
    public GameObject detailsTextBtn, recordsTextBtn;

    public GameObject vipCardPrefab;
    public GameObject[] coinsPrefab;
    public GameObject[] diamondPrefab;
    public GameObject[] featuredItemsPrefab;
    public Sprite[] vipCardSprites, purchaseCurrencySprite;


    [Header("HotPick")]
    public GameObject hotPickObj;
    
    [Header("VIP Card")]
    public GameObject vipCardObj;

    [Header("Gold")]
    public GameObject goldObj;


    private ShopDataTeen shopData = new ShopDataTeen();

    private bool isListDownloaded = false;
    private string screenToShow = "";


    private void OnEnable()
    {
        if(null== instance)
        {
            instance = this;
        }

        //if (!MainMenuControllerTeen.instance.bottomPanel.activeSelf && GameConstants.poker)
        //{
        //    MainMenuControllerTeen.instance.bottomPanel.SetActive(true);
        //    MainMenuControllerTeen.instance.bottomPanelTeen.SetActive(false);
        //}
        //else if (!MainMenuControllerTeen.instance.bottomPanel.activeSelf && !GameConstants.poker)
        //{
        //    MainMenuControllerTeen.instance.bottomPanelTeen.SetActive(true);
        //    MainMenuControllerTeen.instance.bottomPanel.SetActive(false);
        //}

        if (!MainMenuControllerTeen.instance.bottomPanel.activeSelf)
            MainMenuControllerTeen.instance.bottomPanel.SetActive(true);
    }

    private void Awake()
    {
        detailsTextBtn.SetActive(false);
        recordsTextBtn.SetActive(false);
    }

    public void ShowScreen(string screenName = "")
    {
        screenToShow = screenName;

        if (!isListDownloaded)
        {
            string requestData = "{\"shopCategoryId\":\"\"}";
            WebServices.instance.SendRequest(RequestType.GetShopValues, requestData, true, OnServerResponseFound);
            return;
        }

        UpdateAlltext(PlayerManager.instance.GetPlayerGameData());
       

        if (!string.IsNullOrEmpty(screenName))
        {
            OnClickOnButton(screenName);
        }
        else
        {
            OnClickOnButton("item");
        }
    }

    private void UpdateAlltext(PlayerGameDetails playerData)
    {
        coinsText.text = Utility.GetTrimmedAmount("" + playerData.coins);
        diamondText.text = Utility.GetTrimmedAmount("" + playerData.diamonds);
        pointText.text = Utility.GetTrimmedAmount("" + playerData.points);
    }

    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.MainMenu);
                }
                break;

            case "item":
                {
                    container.parent.parent.gameObject.SetActive(false);
                    itemContainer.parent.parent.gameObject.SetActive(true);

                    detailsTextBtn.SetActive(false);
                    recordsTextBtn.SetActive(false);
                    SpawnShopItems(ShopCategory.Item);
                }
                break;

            case "diamond":
                {
                    itemContainer.parent.parent.gameObject.SetActive(false);
                    container.parent.parent.gameObject.SetActive(true);

                    detailsTextBtn.SetActive(false);
                    recordsTextBtn.SetActive(false);
                    SpawnShopItems(ShopCategory.Diamond);
                }
                break;

            case "point":
                {
                    itemContainer.parent.parent.gameObject.SetActive(false);
                    container.parent.parent.gameObject.SetActive(true);

                    detailsTextBtn.SetActive(true);
                    recordsTextBtn.SetActive(true);
                    SpawnShopItems(ShopCategory.Point);
                }
                break;

            case "vip":
                MainMenuControllerTeen.instance.isVIPFromShop = true;
                MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.VIP_Privilege);
                break;

            default:
#if ERROR_LOG
            Debug.LogError("unhdnled eventName found in LobbyUiManager = " + eventName);
#endif
            break;
        }
    }

    private void SpawnShopItems(ShopCategory shopCategory)
    {
        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        for (int i = 3; i < itemContainer.childCount; i++)
        {
            Destroy(itemContainer.GetChild(i).gameObject);            
        }

        for (int i = 0; i < hotPickContainer.childCount; i++)
        {
            Destroy(hotPickContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < vipCardContainer.childCount; i++)
        {
            Destroy(vipCardContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < goldContainer.childCount; i++)
        {
            Destroy(goldContainer.GetChild(i).gameObject);
        }

        itemButton.interactable = true;
        diamondButton.interactable = true;
        pointButton.interactable = true;


        List<ShopItemTeen> shopList = null;

        if (shopCategory == ShopCategory.Item)
        {
            itemButton.interactable = false;
            shopList = new List<ShopItemTeen>(shopData.itemsList);
        }
        else if (shopCategory == ShopCategory.Diamond)
        {
            diamondButton.interactable = false;
            shopList = new List<ShopItemTeen>(shopData.diamondsList);
        }
        else
        {
            pointButton.interactable = false;
            shopList = new List<ShopItemTeen>(shopData.pointsList);
        }


        if (shopList.Count <= 0)
        {
            return;
        }

        if (shopCategory == ShopCategory.Item)
        {
            LoadShopItemData(shopList);
        }
        else if(shopCategory == ShopCategory.Diamond)
        {
            LoadDiamondData(shopList);
        }
        else
        {
            LoadPointData(shopList);
        }

        /*else
        {
            if (shopCategory == ShopCategory.Diamond)
                container.GetComponent<GridLayoutGroup>().padding.top = 0;

            if (shopCategory == ShopCategory.Point)
                container.GetComponent<GridLayoutGroup>().padding.top = 40;

            for (int i = 0; i < shopList.Count; i++)
            {
                switch (shopList[i].purchaseItem)
                {
                    case PurchaseItemTeen.Card:
                        {
                            GameObject gm = Instantiate(vipCardPrefab, container) as GameObject;
                            gm.transform.Find("Icon").GetComponent<Image>().sprite = vipCardSprites[(int)shopList[i].vipCard];

                            if (shopList[i].vipCard.ToString().Equals("Bronze"))
                                gm.transform.Find("Prize").GetComponent<Text>().text = "" + "Silver Card";

                            if (shopList[i].vipCard.ToString().Equals("Silver"))
                                gm.transform.Find("Prize").GetComponent<Text>().text = "" + "Black Card";

                            if (shopList[i].vipCard.ToString().Equals("Platinum"))
                                gm.transform.Find("Prize").GetComponent<Text>().text = "" + "Gold Card";

                            *//*gm.transform.Find("Prize").GetComponent<Text>().text = "" + shopList[i].vipCard + " Card";*//*
                            gm.transform.Find("Validity").GetComponent<Text>().text = "" + shopList[i].validity + " Days";

                            if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
                            {
                                gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                                gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
                            }
                            else
                            {
                                gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                                gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                                gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
                            }


                            gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
                            if (shopList[i].isOffer)
                            {
                                gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
                            }
                            ShopItemTeen itemData = shopList[i];
                            gm.transform.Find("Buy").GetComponent<Button>().onClick.AddListener(() => OnClickOnBuyButton(itemData));
                        }

                        break;



                    case PurchaseItemTeen.Diamond:
                        {
                            int index = 0;

                            if (float.Parse(shopList[i].getValue) > 2000)
                            {
                                index = 3;
                            }

                            if (float.Parse(shopList[i].getValue) > 1000)
                            {
                                index = 2;
                            }

                            if (float.Parse(shopList[i].getValue) > 500)
                            {
                                index = 1;
                            }


                            GameObject gm = Instantiate(diamondPrefab[index], container) as GameObject;
                            gm.transform.Find("Prize").GetComponent<Text>().text = "" + GetTrimmedAmount(shopList[i].getValue);

                            if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
                            {
                                gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                                gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
                            }
                            else
                            {
                                gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                                gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                                gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
                            }


                            gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
                            if (shopList[i].isOffer)
                            {
                                gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
                            }

                            ShopItemTeen itemData = shopList[i];
                            gm.transform.Find("Buy").GetComponent<Button>().onClick.AddListener(() => OnClickOnBuyButton(itemData));


                        }
                        break;

                    case PurchaseItemTeen.Coins:
                        {
                            GameObject gm = Instantiate(coinsPrefab, container) as GameObject;
                            gm.transform.Find("Prize").GetComponent<Text>().text = "" + GetTrimmedAmount(shopList[i].getValue);

                            if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
                            {
                                gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                                gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
                            }
                            else
                            {
                                gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                                gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                                gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
                            }


                            gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
                            if (shopList[i].isOffer)
                            {
                                gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
                            }

                            ShopItemTeen itemData = shopList[i];
                            gm.transform.Find("Buy").GetComponent<Button>().onClick.AddListener(() => OnClickOnBuyButton(itemData));

                        }
                        break;


                    case PurchaseItemTeen.Other:
                        {
                            GameObject gm = Instantiate(featuredItemsPrefab[(int)shopList[i].featureItem], container) as GameObject;
                            gm.transform.Find("Prize").GetComponent<Text>().text = "" + shopList[i].featureItem;

                            if (shopList[i].featureItem == ShopFeaturedItem.Rabit)
                            {
                                gm.transform.Find("Validity").GetComponent<Text>().text = "" + shopList[i].validity + " Days";
                            }
                            else
                            {
                                gm.transform.Find("Validity").GetComponent<Text>().text = "X" + shopList[i].getValue;
                            }



                            if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
                            {
                                gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                                gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
                            }
                            else
                            {
                                gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                                gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                                gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
                            }


                            gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
                            if (shopList[i].isOffer)
                            {
                                gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
                            }
                        }
                        break;

                    default:
                        {
#if ERROR_LOG
                            Debug.LogError("Unhandled case found in shop = " + shopList[i].purchaseItem);
#endif
                        }
                        break;
                }
            }
        }*/
    }

    private void LoadShopItemData(List<ShopItemTeen> shopList)
    {
        int coinPrefabCounter = 0;

        for (int i = 0; i < shopList.Count; i++)
        {
            switch (i)
            {
                //Cases for Hot Picks
                case 0:
                case 1:
                case 2:
                    switch (shopList[i].purchaseItem)
                    {
                        case PurchaseItemTeen.Card:
                            {
                                GameObject gm = Instantiate(vipCardPrefab, hotPickContainer) as GameObject;
                                gm.transform.Find("Icon").GetComponent<Image>().sprite = vipCardSprites[(int)shopList[i].vipCard];

                                if (shopList[i].vipCard.ToString().Equals("Bronze"))
                                    gm.transform.Find("Prize").GetComponent<Text>().text = "" + "Silver Card";

                                if (shopList[i].vipCard.ToString().Equals("Silver"))
                                    gm.transform.Find("Prize").GetComponent<Text>().text = "" + "Black Card";

                                if (shopList[i].vipCard.ToString().Equals("Platinum"))
                                    gm.transform.Find("Prize").GetComponent<Text>().text = "" + "Gold Card";

                                /*gm.transform.Find("Prize").GetComponent<Text>().text = "" + shopList[i].vipCard + " Card";*/
                                gm.transform.Find("Validity").GetComponent<Text>().text = "" + shopList[i].validity + " Days";

                                if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
                                }
                                else
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                                    gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
                                }


                                gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
                                if (shopList[i].isOffer)
                                {
                                    gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
                                }
                                ShopItemTeen itemData = shopList[i];
                                gm.transform.Find("Buy").GetComponent<Button>().onClick.AddListener(() => OnClickOnBuyButton(itemData));
                            }

                            break;



                        case PurchaseItemTeen.Diamond:
                            {
                                int index = 0;

                                if (float.Parse(shopList[i].getValue) > 2000)
                                {
                                    index = 3;
                                }

                                if (float.Parse(shopList[i].getValue) > 1000)
                                {
                                    index = 2;
                                }

                                if (float.Parse(shopList[i].getValue) > 500)
                                {
                                    index = 1;
                                }


                                GameObject gm = Instantiate(diamondPrefab[index], hotPickContainer) as GameObject;
                                gm.transform.Find("Prize").GetComponent<Text>().text = "" + GetTrimmedAmount(shopList[i].getValue);

                                if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
                                }
                                else
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                                    gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
                                }


                                gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
                                if (shopList[i].isOffer)
                                {
                                    gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
                                }

                                ShopItemTeen itemData = shopList[i];
                                gm.transform.Find("Buy").GetComponent<Button>().onClick.AddListener(() => OnClickOnBuyButton(itemData));


                            }
                            break;

                        case PurchaseItemTeen.Coins:
                            {
                                GameObject gm = Instantiate(coinsPrefab[coinPrefabCounter], hotPickContainer) as GameObject;
                                gm.transform.Find("Prize").GetComponent<Text>().text = "" + GetTrimmedAmount(shopList[i].getValue);

                                if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
                                }
                                else
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                                    gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
                                }


                                gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
                                if (shopList[i].isOffer)
                                {
                                    gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
                                }

                                ShopItemTeen itemData = shopList[i];
                                gm.transform.Find("Buy").GetComponent<Button>().onClick.AddListener(() => OnClickOnBuyButton(itemData));

                                if(coinPrefabCounter<3)
                                    coinPrefabCounter++;
                            }
                            break;


                        case PurchaseItemTeen.Other:
                            {
                                GameObject gm = Instantiate(featuredItemsPrefab[(int)shopList[i].featureItem], hotPickContainer) as GameObject;
                                gm.transform.Find("Prize").GetComponent<Text>().text = "" + shopList[i].featureItem;

                                if (shopList[i].featureItem == ShopFeaturedItemTeen.Rabit)
                                {
                                    gm.transform.Find("Validity").GetComponent<Text>().text = "" + shopList[i].validity + " Days";
                                }
                                else
                                {
                                    gm.transform.Find("Validity").GetComponent<Text>().text = "X" + shopList[i].getValue;
                                }



                                if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
                                }
                                else
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                                    gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
                                }


                                gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
                                if (shopList[i].isOffer)
                                {
                                    gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
                                }
                            }
                            break;

                        default:
                            {
#if ERROR_LOG
                                Debug.LogError("Unhandled case found in shop = " + shopList[i].purchaseItem);
#endif
                            }
                            break;
                    }
                    break;


                //Cases for VIP Cards
                case 3:
                case 4:
                case 5:
                    switch (shopList[i].purchaseItem)
                    {
                        case PurchaseItemTeen.Card:
                            {
                                GameObject gm = Instantiate(vipCardPrefab, vipCardContainer) as GameObject;
                                gm.transform.Find("Icon").GetComponent<Image>().sprite = vipCardSprites[(int)shopList[i].vipCard];

                                if (shopList[i].vipCard.ToString().Equals("Bronze"))
                                    gm.transform.Find("Prize").GetComponent<Text>().text = "" + "Silver Card";

                                if (shopList[i].vipCard.ToString().Equals("Silver"))
                                    gm.transform.Find("Prize").GetComponent<Text>().text = "" + "Black Card";

                                if (shopList[i].vipCard.ToString().Equals("Platinum"))
                                    gm.transform.Find("Prize").GetComponent<Text>().text = "" + "Gold Card";

                                /*gm.transform.Find("Prize").GetComponent<Text>().text = "" + shopList[i].vipCard + " Card";*/
                                gm.transform.Find("Validity").GetComponent<Text>().text = "" + shopList[i].validity + " Days";

                                if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
                                }
                                else
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                                    gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
                                }


                                gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
                                if (shopList[i].isOffer)
                                {
                                    gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
                                }
                                ShopItemTeen itemData = shopList[i];
                                gm.transform.Find("Buy").GetComponent<Button>().onClick.AddListener(() => OnClickOnBuyButton(itemData));
                            }

                            break;



                        case PurchaseItemTeen.Diamond:
                            {
                                int index = 0;

                                if (float.Parse(shopList[i].getValue) > 2000)
                                {
                                    index = 3;
                                }

                                if (float.Parse(shopList[i].getValue) > 1000)
                                {
                                    index = 2;
                                }

                                if (float.Parse(shopList[i].getValue) > 500)
                                {
                                    index = 1;
                                }


                                GameObject gm = Instantiate(diamondPrefab[index], vipCardContainer) as GameObject;
                                gm.transform.Find("Prize").GetComponent<Text>().text = "" + GetTrimmedAmount(shopList[i].getValue);

                                if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
                                }
                                else
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                                    gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
                                }


                                gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
                                if (shopList[i].isOffer)
                                {
                                    gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
                                }

                                ShopItemTeen itemData = shopList[i];
                                gm.transform.Find("Buy").GetComponent<Button>().onClick.AddListener(() => OnClickOnBuyButton(itemData));


                            }
                            break;

                        case PurchaseItemTeen.Coins:
                            {
                                GameObject gm = Instantiate(coinsPrefab[0], vipCardContainer) as GameObject;
                                gm.transform.Find("Prize").GetComponent<Text>().text = "" + GetTrimmedAmount(shopList[i].getValue);

                                if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
                                }
                                else
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                                    gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
                                }


                                gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
                                if (shopList[i].isOffer)
                                {
                                    gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
                                }

                                ShopItemTeen itemData = shopList[i];
                                gm.transform.Find("Buy").GetComponent<Button>().onClick.AddListener(() => OnClickOnBuyButton(itemData));

                            }
                            break;


                        case PurchaseItemTeen.Other:
                            {
                                GameObject gm = Instantiate(featuredItemsPrefab[(int)shopList[i].featureItem], vipCardContainer) as GameObject;
                                gm.transform.Find("Prize").GetComponent<Text>().text = "" + shopList[i].featureItem;

                                if (shopList[i].featureItem == ShopFeaturedItemTeen.Rabit)
                                {
                                    gm.transform.Find("Validity").GetComponent<Text>().text = "" + shopList[i].validity + " Days";
                                }
                                else
                                {
                                    gm.transform.Find("Validity").GetComponent<Text>().text = "X" + shopList[i].getValue;
                                }



                                if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
                                }
                                else
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                                    gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
                                }


                                gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
                                if (shopList[i].isOffer)
                                {
                                    gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
                                }
                            }
                            break;

                        default:
                            {
#if ERROR_LOG
                                Debug.LogError("Unhandled case found in shop = " + shopList[i].purchaseItem);
#endif
                            }
                            break;
                    }
                    break;

                default:
                    switch (shopList[i].purchaseItem)
                    {
                        case PurchaseItemTeen.Card:
                            {
                                GameObject gm = Instantiate(vipCardPrefab, goldContainer) as GameObject;
                                gm.transform.Find("Icon").GetComponent<Image>().sprite = vipCardSprites[(int)shopList[i].vipCard];

                                if (shopList[i].vipCard.ToString().Equals("Bronze"))
                                    gm.transform.Find("Prize").GetComponent<Text>().text = "" + "Silver Card";

                                if (shopList[i].vipCard.ToString().Equals("Silver"))
                                    gm.transform.Find("Prize").GetComponent<Text>().text = "" + "Black Card";

                                if (shopList[i].vipCard.ToString().Equals("Platinum"))
                                    gm.transform.Find("Prize").GetComponent<Text>().text = "" + "Gold Card";

                                /*gm.transform.Find("Prize").GetComponent<Text>().text = "" + shopList[i].vipCard + " Card";*/
                                gm.transform.Find("Validity").GetComponent<Text>().text = "" + shopList[i].validity + " Days";

                                if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
                                }
                                else
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                                    gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
                                }


                                gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
                                if (shopList[i].isOffer)
                                {
                                    gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
                                }
                                ShopItemTeen itemData = shopList[i];
                                gm.transform.Find("Buy").GetComponent<Button>().onClick.AddListener(() => OnClickOnBuyButton(itemData));
                            }

                            break;



                        case PurchaseItemTeen.Diamond:
                            {
                                int index = 0;

                                if (float.Parse(shopList[i].getValue) > 2000)
                                {
                                    index = 3;
                                }

                                if (float.Parse(shopList[i].getValue) > 1000)
                                {
                                    index = 2;
                                }

                                if (float.Parse(shopList[i].getValue) > 500)
                                {
                                    index = 1;
                                }


                                GameObject gm = Instantiate(diamondPrefab[index], goldContainer) as GameObject;
                                gm.transform.Find("Prize").GetComponent<Text>().text = "" + GetTrimmedAmount(shopList[i].getValue);

                                if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
                                }
                                else
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                                    gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
                                }


                                gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
                                if (shopList[i].isOffer)
                                {
                                    gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
                                }

                                ShopItemTeen itemData = shopList[i];
                                gm.transform.Find("Buy").GetComponent<Button>().onClick.AddListener(() => OnClickOnBuyButton(itemData));


                            }
                            break;

                        case PurchaseItemTeen.Coins:
                            {
                                GameObject gm = Instantiate(coinsPrefab[0], goldContainer) as GameObject;
                                gm.transform.Find("Prize").GetComponent<Text>().text = "" + GetTrimmedAmount(shopList[i].getValue);

                                if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
                                }
                                else
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                                    gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
                                }


                                gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
                                if (shopList[i].isOffer)
                                {
                                    gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
                                }

                                ShopItemTeen itemData = shopList[i];
                                gm.transform.Find("Buy").GetComponent<Button>().onClick.AddListener(() => OnClickOnBuyButton(itemData));

                            }
                            break;


                        case PurchaseItemTeen.Other:
                            {
                                GameObject gm = Instantiate(featuredItemsPrefab[(int)shopList[i].featureItem], goldContainer) as GameObject;
                                gm.transform.Find("Prize").GetComponent<Text>().text = "" + shopList[i].featureItem;

                                if (shopList[i].featureItem == ShopFeaturedItemTeen.Rabit)
                                {
                                    gm.transform.Find("Validity").GetComponent<Text>().text = "" + shopList[i].validity + " Days";
                                }
                                else
                                {
                                    gm.transform.Find("Validity").GetComponent<Text>().text = "X" + shopList[i].getValue;
                                }



                                if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
                                }
                                else
                                {
                                    gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                                    gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                                    gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
                                }


                                gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
                                if (shopList[i].isOffer)
                                {
                                    gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
                                }
                            }
                            break;

                        default:
                            {
#if ERROR_LOG
                                Debug.LogError("Unhandled case found in shop = " + shopList[i].purchaseItem);
#endif
                            }
                            break;
                    }
                    break;
            }
        }
    }

    private void LoadDiamondData(List<ShopItemTeen> shopList)
    {
        container.GetComponent<GridLayoutGroup>().padding.top = 0;

        /*container.GetComponent<GridLayoutGroup>().padding.top = 40;*/
        GameObject gm;
        for (int i = 0; i < shopList.Count; i++)
        {
            int index = 0;

            if (float.Parse(shopList[i].getValue) > 2000)
            {
                index = 3;
            }

            if (float.Parse(shopList[i].getValue) > 1000)
            {
                index = 2;
            }

            if (float.Parse(shopList[i].getValue) > 500)
            {
                index = 1;
            }

            gm = Instantiate(diamondPrefab[i], container) as GameObject;
            gm.transform.Find("Prize").GetComponent<Text>().text = "" + GetTrimmedAmount(shopList[i].getValue);

            if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
            {
                gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
            }
            else
            {
                gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
            }


            gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
            if (shopList[i].isOffer)
            {
                gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
            }

            ShopItemTeen itemData = shopList[i];
            gm.transform.Find("Buy").GetComponent<Button>().onClick.AddListener(() => OnClickOnBuyButton(itemData));

        }
        
    }

    private void LoadPointData(List<ShopItemTeen> shopList)
    {
        Debug.Log("Total Items for Points:" + shopList.Count);
        container.GetComponent<GridLayoutGroup>().padding.top = 40;

        GameObject gm;

        for (int i = 0; i < 3; i++)
        {
            gm = Instantiate(coinsPrefab[i], container) as GameObject;
            gm.transform.Find("Prize").GetComponent<Text>().text = "" + GetTrimmedAmount(shopList[i].getValue);

            if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
            {
                gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
            }
            else
            {
                gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
            }


            gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
            if (shopList[i].isOffer)
            {
                gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
            }

            ShopItemTeen itemData = shopList[i];
            gm.transform.Find("Buy").GetComponent<Button>().onClick.AddListener(() => OnClickOnBuyButton(itemData));
        }

        for (int i = 3; i < shopList.Count; i++)
        {
            gm = Instantiate(vipCardPrefab, container) as GameObject;
            gm.transform.Find("Icon").GetComponent<Image>().sprite = vipCardSprites[(int)shopList[i].vipCard];

            if (shopList[i].vipCard.ToString().Equals("Bronze"))
                gm.transform.Find("Prize").GetComponent<Text>().text = "" + "Silver Card";

            if (shopList[i].vipCard.ToString().Equals("Silver"))
                gm.transform.Find("Prize").GetComponent<Text>().text = "" + "Black Card";

            if (shopList[i].vipCard.ToString().Equals("Platinum"))
                gm.transform.Find("Prize").GetComponent<Text>().text = "" + "Gold Card";

            /*gm.transform.Find("Prize").GetComponent<Text>().text = "" + shopList[i].vipCard + " Card";*/
            gm.transform.Find("Validity").GetComponent<Text>().text = "" + shopList[i].validity + " Days";

            if (shopList[i].purchaseCurrency == PurchaseCurrencyTeen.Dollar)
            {
                gm.transform.Find("Buy/Icon").gameObject.SetActive(false);
                gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = "$" + GetTrimmedAmount(shopList[i].purchaseValue);
            }
            else
            {
                gm.transform.Find("Buy/Icon").gameObject.SetActive(true);
                gm.transform.Find("Buy/Icon").gameObject.GetComponent<Image>().sprite = purchaseCurrencySprite[(int)shopList[i].purchaseCurrency];
                gm.transform.Find("Buy/Text").gameObject.GetComponent<Text>().text = GetTrimmedAmount(shopList[i].purchaseValue);
            }


            gm.transform.Find("Offer").gameObject.SetActive(shopList[i].isOffer);
            if (shopList[i].isOffer)
            {
                gm.transform.Find("Offer/Text").gameObject.GetComponent<Text>().text = shopList[i].offerValue + "% OFF";
            }
            ShopItemTeen itemData = shopList[i];
            gm.transform.Find("Buy").GetComponent<Button>().onClick.AddListener(() => OnClickOnBuyButton(itemData));
        }

    }

    private void OnClickOnBuyButton(ShopItemTeen itemData)
    {
        PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
        bool isChangesMade = false;

        if (itemData.purchaseItem == PurchaseItemTeen.Diamond)
        {
            float purchaseValue = float.Parse(itemData.purchaseValue);


            switch (itemData.purchaseCurrency)
            {
                case PurchaseCurrencyTeen.Dollar:
                    {
                        playerData.diamonds += float.Parse(itemData.getValue);
                        isChangesMade = true;
                    }
                    break;

                case PurchaseCurrencyTeen.Coins:
                    {
                        if (playerData.coins >= float.Parse(itemData.purchaseValue))
                        {
                            playerData.coins -= purchaseValue;
                            if (playerData.coins < 0f) { playerData.coins = 0f; }
                            playerData.diamonds += float.Parse(itemData.getValue);
                            isChangesMade = true;
                        }
                        else
                        {
                            // NativeFunctionalityIntegration.SharedInstance.showToastMessage("Insufficient coins");
                            MainMenuControllerTeen.instance.ShowMessage("Insufficient Diamonds");
                        }
                    }
                break;

                case PurchaseCurrencyTeen.Point:
                    {
                        if (playerData.points >= float.Parse(itemData.purchaseValue))
                        {
                            playerData.points -= purchaseValue;
                            if (playerData.points < 0f) { playerData.points = 0f; }
                            playerData.diamonds += float.Parse(itemData.getValue);
                            isChangesMade = true;
                        }
                        else
                        {
                            // NativeFunctionalityIntegration.SharedInstance.showToastMessage("Insufficient points");
                            MainMenuControllerTeen.instance.ShowMessage("Insufficient Diamonds");
                        }
                    }
                break;

                default:
                break;
            }

        }
        else if (itemData.purchaseItem == PurchaseItemTeen.Coins)
        {
            float purchaseValue = float.Parse(itemData.purchaseValue);

            switch (itemData.purchaseCurrency)
            {
                case PurchaseCurrencyTeen.Dollar:
                    {
                        playerData.coins += float.Parse(itemData.getValue);
                        isChangesMade = true;
                    }
                    break;


                case PurchaseCurrencyTeen.Diamond:
                    {
                        if (playerData.diamonds >= purchaseValue)
                        {
                            playerData.diamonds -= purchaseValue;
                            if (playerData.diamonds < 0f) { playerData.diamonds = 0f; }
                            playerData.coins += float.Parse(itemData.getValue);
                            isChangesMade = true;
                        }
                        else
                        {
                            //NativeFunctionalityIntegration.SharedInstance.showToastMessage("Insufficient points");
                            MainMenuControllerTeen.instance.ShowMessage("Insufficient Diamonds");
                        }
                    }
                    break;


                case PurchaseCurrencyTeen.Point:
                    {
                        if (playerData.points >= purchaseValue)
                        {
                            playerData.points -= purchaseValue;
                            if (playerData.points < 0f) { playerData.points = 0f; }
                            playerData.coins += float.Parse(itemData.getValue);
                            isChangesMade = true;
                        }
                        else
                        {
                            //NativeFunctionalityIntegration.SharedInstance.showToastMessage("Insufficient points");
                            MainMenuControllerTeen.instance.ShowMessage("Insufficient Points");
                        }
                    }
                    break;

                default:
                break;
            }
        }
        else if (itemData.purchaseItem == PurchaseItemTeen.Card)
        {
            float purchaseValue = float.Parse(itemData.purchaseValue);

            switch (itemData.purchaseCurrency)
            {
                case PurchaseCurrencyTeen.Dollar:
                    {
                        if (itemData.vipCard.ToString().Equals("Bronze"))
                            Debug.Log("Purchasing Bronze card Using Dollar...");
                        if (itemData.vipCard.ToString().Equals("Silver"))
                            Debug.Log("Purchasing Silver card Using Dollar...");
                        if (itemData.vipCard.ToString().Equals("Platinum"))
                            Debug.Log("Purchasing Platinum card Using Dollar...");

                        isChangesMade = true;
                    }
                    break;


                case PurchaseCurrencyTeen.Diamond:
                    {
                        if (playerData.diamonds >= purchaseValue)
                        {
                            playerData.diamonds -= purchaseValue;
                            if (playerData.diamonds < 0f) { playerData.diamonds = 0f; }

                            if (itemData.vipCard.ToString().Equals("Bronze"))
                                Debug.Log("Purchasing Bronze card Using Diamonds...");
                            if (itemData.vipCard.ToString().Equals("Silver"))
                                Debug.Log("Purchasing Silver card Using Diamonds...");
                            if (itemData.vipCard.ToString().Equals("Platinum"))
                                Debug.Log("Purchasing Platinum card Using Diamonds...");

                            isChangesMade = true;
                        }
                        else
                        {
                            //NativeFunctionalityIntegration.SharedInstance.showToastMessage("Insufficient points");
                            MainMenuControllerTeen.instance.ShowMessage("Insufficient Diamonds");
                        }
                    }
                    break;


                case PurchaseCurrencyTeen.Point:
                    {
                        if (playerData.points >= purchaseValue)
                        {
                            playerData.points -= purchaseValue;
                            if (playerData.points < 0f) { playerData.points = 0f; }

                            if (itemData.vipCard.ToString().Equals("Bronze"))
                                Debug.Log("Purchasing Bronze card Using Points...");
                            if (itemData.vipCard.ToString().Equals("Silver"))
                                Debug.Log("Purchasing Silver card Using Points...");
                            if (itemData.vipCard.ToString().Equals("Platinum"))
                                Debug.Log("Purchasing Platinum card Using Points...");

                            isChangesMade = true;
                        }
                        else
                        {
                            //NativeFunctionalityIntegration.SharedInstance.showToastMessage("Insufficient points");
                            MainMenuControllerTeen.instance.ShowMessage("Insufficient Points to purchase Cards...");
                        }
                    }
                    break;

                default:
                    break;
            }
        }


        if (isChangesMade)
        {
            UpdateUserBalance(playerData);
        }
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuControllerTeen.instance.ShowMessage(errorMessage);
            }

            return;
        }

        if (requestType == RequestType.GetShopValues)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["status"].Equals(true))
            {
                for (int i = 0; i < data["response"].Count; i++)
                {
                    ShopItemTeen shopItem = new ShopItemTeen();
                    shopItem.shopId = data["response"][i]["shopId"].ToString();

                    shopItem.purchaseValue = data["response"][i]["purchaseValue"].ToString();
                    shopItem.shopSubTitle = data["response"][i]["shopSubTitle"].ToString();
                    shopItem.getValue = data["response"][i]["getValue"].ToString();
                    shopItem.validity = data["response"][i]["validityDays"].ToString();
                    shopItem.isOffer = data["response"][i]["isOffer"].ToString().Contains("Yes");
                    shopItem.offerValue = data["response"][i]["offerValue"].ToString();

                    switch (data["response"][i]["shopCategoryTitle"].ToString())
                    {
                        case "ITEM":
                            {
                                shopItem.shopCategory = ShopCategoryTeen.Item;
                            }
                            break;

                        case "DIAMOND":
                            {
                                shopItem.shopCategory = ShopCategoryTeen.Diamond;
                            }
                            break;

                        default:
                            {
                                shopItem.shopCategory = ShopCategoryTeen.Point;
                            }
                            break;
                    }


                    switch (data["response"][i]["fromPurchase"].ToString())
                    {
                        case "Doller":
                            {
                                shopItem.purchaseCurrency = PurchaseCurrencyTeen.Dollar;
                            }
                            break;

                        case "Coins":
                            {
                                shopItem.purchaseCurrency = PurchaseCurrencyTeen.Coins;
                            }
                            break;

                        case "Diamond":
                            {
                                shopItem.purchaseCurrency = PurchaseCurrencyTeen.Diamond;
                            }
                            break;

                        default:
                            {
                                shopItem.purchaseCurrency = PurchaseCurrencyTeen.Point;
                            }
                            break;
                    }


                    switch (data["response"][i]["getPurchase"].ToString())
                    {
                        case "Point":
                            {
                                shopItem.purchaseItem = PurchaseItemTeen.Point;
                            }
                            break;

                        case "Coins":
                            {
                                shopItem.purchaseItem = PurchaseItemTeen.Coins;
                            }
                            break;

                        case "Diamond":
                            {
                                shopItem.purchaseItem = PurchaseItemTeen.Diamond;
                            }
                            break;

                        case "Card":
                            {
                                shopItem.purchaseItem = PurchaseItemTeen.Card;
                            }
                            break;

                        default:
                            {
                                shopItem.purchaseItem = PurchaseItemTeen.Other;

                                switch (data["response"][i]["featuredItem"].ToString())
                                {
                                    case "Emoji":
                                        {
                                            shopItem.featureItem = ShopFeaturedItemTeen.Emoji;
                                        }
                                        break;

                                    case "Time Bank":
                                        {
                                            shopItem.featureItem = ShopFeaturedItemTeen.TimeBank;
                                        }
                                        break;

                                    default:
                                        {
                                            shopItem.featureItem = ShopFeaturedItemTeen.Rabit;
                                        }
                                        break;
                                }
                            }
                            break;
                    }



                    switch (data["response"][i]["cards"].ToString())
                    {
                        case "Platinum":
                            {
                                shopItem.vipCard = VIPCardTeen.Platinum;
                            }
                            break;

                        case "Silver":
                            {
                                shopItem.vipCard = VIPCardTeen.Silver;
                            }
                            break;

                        default:
                            {
                                shopItem.vipCard = VIPCardTeen.Bronze;
                            }
                            break;
                    }


                    if (shopItem.shopCategory == ShopCategoryTeen.Item)
                    {
                        shopData.itemsList.Add(shopItem);
                    }
                    else if (shopItem.shopCategory == ShopCategoryTeen.Diamond)
                    {
                        shopData.diamondsList.Add(shopItem);
                    }
                    else
                    {
                        shopData.pointsList.Add(shopItem);
                    }
                }



                shopData.itemsList.OrderBy(x => x.getValue);
                isListDownloaded = true;
                ShowScreen(screenToShow);
            }
            else
            {
                MainMenuControllerTeen.instance.ShowMessage(data["message"].ToString(), () =>
                {
                    MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.MainMenu);
                });
            }
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhandled server requestType found  " + requestType);
#endif

        }

    }


    public string GetTrimmedAmount(string currentValueInString)
    {
        double currentValue = double.Parse(currentValueInString);
        string updatedAmount = "";


        if (currentValue > 1000000000) // One Billion
        {
            updatedAmount = "" + (currentValue / 1000000000).ToString("0.00") + "B";
        }
        else if (currentValue > 1000000) // One Million
        {
            updatedAmount = "" + (currentValue / 1000000).ToString("0.00") + "M";
        }
        else if (currentValue > 1000) // One Thousand
        {
            updatedAmount = "" + (currentValue / 1000).ToString("0.00") + "K";
        }
        else
        {
            updatedAmount = currentValueInString;
        }

        if (updatedAmount.Contains(".00"))
        {
            updatedAmount = updatedAmount.Replace(".00","");
        }

        return updatedAmount;
    }


    public void UpdateUserBalance(PlayerGameDetails updatedData)
    {
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
            "\"silver\":\"0\"," +
            "\"coins\":\"" + (int)updatedData.coins + "\"," +
            "\"points\":\"" + (int)updatedData.points + "\"," +
            "\"diamond\":\"" + (int)updatedData.diamonds + "\"," +

            "\"rabbit\":\"0\"," +
            "\"emoji\":\"0\"," +
            "\"time\":\"0\"," +
            "\"day\":\"0\"," +
            "\"playerProgress\":\"\"}";

        MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Loading);

        WebServices.instance.SendRequest(RequestType.UpdateUserBalance, requestData, true, (requestType, serverResponse, isShowErrorMessage, errorMessage) =>
        {
            MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Loading);

            if (errorMessage.Length > 0)
            {
                MainMenuControllerTeen.instance.ShowMessage(errorMessage);
            }
            else
            {
                JsonData data = JsonMapper.ToObject(serverResponse);
                if (data["status"].Equals(true))
                {
                    PlayerManager.instance.SetPlayerGameData(updatedData);
                    UpdateAlltext(updatedData);
                    if (MenuHandllerTeen.instance != null)
                    {
                        MenuHandllerTeen.instance.UpdateAllText();
                    }
                    LobbyUiManager.instance.coinsText.text = Utility.GetTrimmedAmount("" + PlayerManager.instance.GetPlayerGameData().coins);
                }
                else
                {
                    MainMenuControllerTeen.instance.ShowMessage(data["message"].ToString());
                    if (MenuHandllerTeen.instance != null)
                    {
                        MenuHandllerTeen.instance.UpdateAllText();
                    }
                }
            }
        });
    }

}


[System.Serializable]
public class ShopDataTeen
{
    public List<ShopItemTeen> itemsList = new List<ShopItemTeen>();
    public List<ShopItemTeen> diamondsList = new List<ShopItemTeen>();
    public List<ShopItemTeen> pointsList = new List<ShopItemTeen>();
}


[System.Serializable]
public class ShopItemTeen
{
    public string shopId;
    public ShopCategoryTeen shopCategory;
    public PurchaseCurrencyTeen purchaseCurrency;
    public PurchaseItemTeen purchaseItem;
    public VIPCardTeen vipCard;

    public string purchaseValue;
    public string shopSubTitle;
    public ShopFeaturedItemTeen featureItem;
    public string getValue;
    public string validity;
    public bool isOffer;
    public string offerValue;
}


[System.Serializable]
public enum ShopCategoryTeen
{
    Item,
    Diamond,
    Point
}


[System.Serializable]
public enum PurchaseCurrencyTeen
{
    Diamond,
    Point,
    Coins,
    Dollar
}


[System.Serializable]
public enum PurchaseItemTeen
{
    Coins,
    Diamond,
    Point,
    Card,
    Other
}


[System.Serializable]
public enum ShopFeaturedItemTeen
{
    Rabit,
    Emoji,
    TimeBank
}