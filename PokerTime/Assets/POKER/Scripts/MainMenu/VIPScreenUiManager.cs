using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using LitJson;


public class VIPScreenUiManager : MonoBehaviour
{
    public GameObject[] privileges;
    private List<List<VIPPrivilegeData>> vipPrivilegeData = new List<List<VIPPrivilegeData>>();
    public Dropdown dropDown;
    public Text cardPriceText;
    public Image[] selectedCardImage;
    private VIPCard selectedCard = VIPCard.Bronze;
    public GameObject helpPanel;


    private void Start()
    {
        LoadDefaultValues();
        OnDropDownValueChange();
        ChangeSelectedCard(VIPCard.Bronze);
        FetchPrivilege();
    }

    private void FetchPrivilege()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        WebServices.instance.SendRequest(RequestType.GetVIPPrivilege, "{}", true, OnServerResponseFound);
    }


    public void OnDropDownValueChange()
    {
        UpdateCardValue(dropDown.value);
        ChangeSelectedCard(selectedCard);
    }


    private void ChangeSelectedCard(VIPCard cardType)
    {
        selectedCard = cardType;

        for (int i = 0; i < selectedCardImage.Length; i++)
        {
            selectedCardImage[i].gameObject.SetActive(false);
        }

        selectedCardImage[(int)cardType].gameObject.SetActive(true);

        cardPriceText.text = vipPrivilegeData[(int)cardType][dropDown.value].amount;
    }


    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);


        switch (eventName)
        {
            case "buy":
                {
                    //TODO need to call buy method
                }
                break;


            case "back":
                {
                    OnClickOnBack();
                }
                break;

            case "help":
                {
                    helpPanel.SetActive(true);
                }
                break;

            case "bronze":
                {
                    ChangeSelectedCard(VIPCard.Bronze);
                }
            break;

            case "silver":
                {
                    ChangeSelectedCard(VIPCard.Silver);
                }
                break;

            case "platinum":
                {
                    ChangeSelectedCard(VIPCard.Platinum);
                }
                break;

            default:
#if ERROR_LOG
            Debug.LogError("unhdnled eventName found in VIPScreenUiManager = " + eventName);
#endif
            break;
        }
    }


    private void UpdateCardValue(int dayIndex)
    {
        int totalCards = Enum.GetNames(typeof(VIPCard)).Length;

        for (int privilegeCounter = 0; privilegeCounter < privileges.Length; privilegeCounter++)
        {
            for (int cardCounter = 0; cardCounter < totalCards; cardCounter++)
            {
                if (privilegeCounter >= (int)VIP_Priillage.ExlusiveEmoji)
                {
                    privileges[privilegeCounter].transform.GetChild(cardCounter).GetComponent<Text>().text = "" + vipPrivilegeData[cardCounter][dayIndex].vipPrivilege[privilegeCounter].counterValue;
                }
                else
                {
                    privileges[privilegeCounter].transform.GetChild(cardCounter).gameObject.SetActive(vipPrivilegeData[cardCounter][dayIndex].vipPrivilege[privilegeCounter].isAvailable);
                }
            }
        }
    }


    private void LoadDefaultValues()
    {
        int totalCards = Enum.GetNames(typeof(VIPCard)).Length;
        int totalPrivillage = Enum.GetNames(typeof(VIP_Priillage)).Length;

        for (int i = 0; i < totalCards; i++)
        {
            List<VIPPrivilegeData> privilegeList = new List<VIPPrivilegeData>();

            for (int k = 0; k < 3; k++)
            {
                VIPPrivilegeData data = new VIPPrivilegeData();
                data.vipCard = (VIPCard)i;

                int day = 30;


                if (k == 0)
                {
                    day = 30;

                    if (data.vipCard == VIPCard.Bronze)
                    {
                        data.amount = "380";
                    }
                    else if (data.vipCard == VIPCard.Silver)
                    {
                        data.amount = "1580";
                    }
                    else
                    {
                        data.amount = "3880";
                    }
                }
                else if (k == 1)
                {
                    day = 90;

                    if (data.vipCard == VIPCard.Bronze)
                    {
                        data.amount = "980";
                    }
                    else if (data.vipCard == VIPCard.Silver)
                    {
                        data.amount = "3880";
                    }
                    else
                    {
                        data.amount = "9480";
                    }
                }
                else 
                {
                    day = 365;

                    if (data.vipCard == VIPCard.Bronze)
                    {
                        data.amount = "3180";
                    }
                    else if (data.vipCard == VIPCard.Silver)
                    {
                        data.amount = "11380";
                    }
                    else
                    {
                        data.amount = "27880";
                    }
                }



                data.day = day;
                data.vipPrivilege = new VIPPrivilege[totalPrivillage];

                for (int j = 0; j < totalPrivillage; j++)
                {
                    VIPPrivilege privilege = new VIPPrivilege();

                    if (j >= (int)VIP_Priillage.ExlusiveEmoji)
                    {
                        switch ((VIP_Priillage)j)
                        {
                            case VIP_Priillage.FreeEmoji:
                                {
                                    switch (day)
                                    {
                                        case 30:
                                            {
                                                if (data.vipCard == VIPCard.Bronze)
                                                {
                                                    privilege.counterValue = 200;
                                                }
                                                else if (data.vipCard == VIPCard.Silver)
                                                {
                                                    privilege.counterValue = 800;
                                                }
                                                else
                                                {
                                                    privilege.counterValue = 1200;
                                                }
                                            }
                                        break;

                                        case 90:
                                            {
                                                if (data.vipCard == VIPCard.Bronze)
                                                {
                                                    privilege.counterValue = 600;
                                                }
                                                else if (data.vipCard == VIPCard.Silver)
                                                {
                                                    privilege.counterValue = 2400;
                                                }
                                                else
                                                {
                                                    privilege.counterValue = 3600;
                                                }
                                            }
                                        break;

                                        default:
                                            {
                                                if (data.vipCard == VIPCard.Bronze)
                                                {
                                                    privilege.counterValue = 2400;
                                                }
                                                else if (data.vipCard == VIPCard.Silver)
                                                {
                                                    privilege.counterValue = 9600;
                                                }
                                                else
                                                {
                                                    privilege.counterValue = 14400;
                                                }
                                            }
                                        break;
                                    }



                                    
                                }
                                break;

                            case VIP_Priillage.FreeTimeBank:
                                {
                                    switch (day)
                                    {
                                        case 30:
                                            {
                                                if (data.vipCard == VIPCard.Bronze)
                                                {
                                                    privilege.counterValue = 15;
                                                }
                                                else if (data.vipCard == VIPCard.Silver)
                                                {
                                                    privilege.counterValue = 80;
                                                }
                                                else
                                                {
                                                    privilege.counterValue = 120;
                                                }
                                            }
                                            break;

                                        case 90:
                                            {
                                                if (data.vipCard == VIPCard.Bronze)
                                                {
                                                    privilege.counterValue = 45;
                                                }
                                                else if (data.vipCard == VIPCard.Silver)
                                                {
                                                    privilege.counterValue = 240;
                                                }
                                                else
                                                {
                                                    privilege.counterValue = 360;
                                                }
                                            }
                                            break;

                                        default:
                                            {
                                                if (data.vipCard == VIPCard.Bronze)
                                                {
                                                    privilege.counterValue = 180;
                                                }
                                                else if (data.vipCard == VIPCard.Silver)
                                                {
                                                    privilege.counterValue = 960;
                                                }
                                                else
                                                {
                                                    privilege.counterValue = 1440;
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;

                            default:

                            if ((VIP_Priillage)j == VIP_Priillage.ClubCreationLimit && data.vipCard == VIPCard.Bronze)
                            {
                                privilege.counterValue = 1;
                            }
                            else
                            {
                                privilege.counterValue = 3;
                            }

                            break;
                        }
                    }
                    else
                    {
                        if (data.vipCard == VIPCard.Bronze)
                        {
                            if (j <= (int)VIP_Priillage.MoreLoginRewards)
                            {
                                privilege.isAvailable = true;
                            }
                            else
                            {
                                privilege.isAvailable = false;
                            }
                        }
                        else if (data.vipCard == VIPCard.Silver)
                        {
                            if (j <= (int)VIP_Priillage.RivalDataDisplay)
                            {
                                privilege.isAvailable = true;
                            }
                            else
                            {
                                privilege.isAvailable = false;
                            }
                        }
                        else
                        {
                            privilege.isAvailable = true;
                        }
                    }


                    data.vipPrivilege[j] = privilege;
                }

                privilegeList.Add(data);
            }

            vipPrivilegeData.Add(privilegeList);
        }
    }



    private void ReadServerValues(JsonData data)
    {
        int totalPrivillage = Enum.GetNames(typeof(VIP_Priillage)).Length;

        for (int j = 0; j < data["response"].Count; j++)
        {
            int[] listIndex = GetListIndex(data["response"][j]["days"].ToString(), data["response"][j]["cardFeatureTitle"].ToString());

            vipPrivilegeData[listIndex[0]][listIndex[1]].amount = data["response"][j]["purchaseDiamond"].ToString();

            for (int i = 0; i < totalPrivillage; i++)
            {

                switch ((VIP_Priillage)i)
                {
  
                    case VIP_Priillage.AllInEquity:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].isAvailable = data["response"][j]["allInEquity"].ToString() == "Yes";
                        }
                        break;

                    case VIP_Priillage.RabitHunting:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].isAvailable = data["response"][j]["rabbitHunting"].ToString() == "Yes";
                        }
                        break;

                    case VIP_Priillage.MoreLoginRewards:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].isAvailable = data["response"][j]["moreLoginRewards"].ToString() == "Yes";
                        }
                        break;

                    case VIP_Priillage.ReportDetails:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].isAvailable = data["response"][j]["reportDetail"].ToString() == "Yes";
                        }
                        break;

                    case VIP_Priillage.RivalDataDisplay:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].isAvailable = data["response"][j]["rivalDataDisplay"].ToString() == "Yes";
                        }
                        break;

                    case VIP_Priillage.ClubData:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].isAvailable = data["response"][j]["clubData"].ToString() == "Yes";
                        }
                        break;

                    case VIP_Priillage.ExtraDisconnectProtection:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].isAvailable = data["response"][j]["extraDisconnectProtection"].ToString() == "Yes";
                        }
                        break;

                    case VIP_Priillage.ExlusiveEmoji:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].counterValue = (int)float.Parse(data["response"][j]["exclusiveEmoji"].ToString());
                        }
                        break;

                    case VIP_Priillage.ClubCreationLimit:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].counterValue = (int)float.Parse(data["response"][j]["clubCreationLimit"].ToString());
                        }
                        break;

                   
                    case VIP_Priillage.FreeEmoji:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].counterValue = (int)float.Parse(data["response"][j]["freeEmoji"].ToString());
                        }
                        break;

                    case VIP_Priillage.FreeTimeBank:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].counterValue = (int)float.Parse(data["response"][j]["freeTimeBank"].ToString());
                        }
                        break;


                    default:
                    break;
                }




            }
            

        }
    }

    private int[] GetListIndex(string dayString,string cardString)
    {
        int[] listIndex = new int[2];
        
        int day = (int)float.Parse(dayString);
        int totalCards = Enum.GetNames(typeof(VIPCard)).Length;
        VIPCard card = VIPCard.Bronze;

        for (int i = 0; i < totalCards; i++)
        {
            if(cardString == ""+(VIPCard)i)
            {
                card = (VIPCard)i;
                break;
            }
        }



        for (int i = 0; i < vipPrivilegeData.Count; i++)
        {
            for (int j = 0; j < vipPrivilegeData[i].Count; j++)
            {
                if (vipPrivilegeData[i][j].day == day && vipPrivilegeData[i][j].vipCard == card)
                {
                    listIndex[0] = i;
                    listIndex[1] = j;

                    i = j = 1000;
                    break;
                }
            }
        }

        return listIndex;
    }




    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuController.instance.ShowMessage(errorMessage);
            }

            return;
        }




        if (requestType == RequestType.GetVIPPrivilege)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["status"].Equals(true))
            {
                ReadServerValues(data);
            }
            else
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
    }



    public void OnClickOnBack()
    {
        if (helpPanel.activeInHierarchy)
        {
            helpPanel.SetActive(false);
        }
        else
        {
            MainMenuController.instance.ShowScreen(MainMenuScreens.MainMenu);
        }
        
    }



}

public class VIPPrivilegeData
{
    public VIPCard vipCard;
    public int day;
    public string amount;
    public VIPPrivilege[] vipPrivilege;
}

public class VIPPrivilege
{
    public bool isAvailable;
    public int counterValue;
}

[System.Serializable]
public enum VIPCard
{
    Bronze,  
    Silver,
    Platinum
}

[System.Serializable]
public enum VIP_Priillage
{
    AllInEquity,
    RabitHunting,
    MoreLoginRewards,
    ReportDetails,
    RivalDataDisplay,
    ClubData,
    ExtraDisconnectProtection,
    ExlusiveEmoji,
    ClubCreationLimit,
    FreeEmoji,
    FreeTimeBank
}



