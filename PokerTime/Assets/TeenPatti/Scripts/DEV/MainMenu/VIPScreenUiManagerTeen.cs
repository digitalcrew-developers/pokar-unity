using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using LitJson;


public class VIPScreenUiManagerTeen : MonoBehaviour
{
    public GameObject[] privileges;
    private List<List<VIPPrivilegeDataTeen>> vipPrivilegeData = new List<List<VIPPrivilegeDataTeen>>();
    public Dropdown dropDown;
    public Text cardPriceText;
    public Image[] selectedCardImage;
    private VIPCardTeen selectedCard = VIPCardTeen.Bronze;
    public GameObject helpPanel;


    private void Start()
    {
        LoadDefaultValues();
        OnDropDownValueChange();
        ChangeSelectedCard(VIPCardTeen.Bronze);
        FetchPrivilege();
    }

    private void FetchPrivilege()
    {
        MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Loading);
        WebServices.instance.SendRequest(RequestType.GetVIPPrivilege, "{}", true, OnServerResponseFound);
    }


    public void OnDropDownValueChange()
    {
        UpdateCardValue(dropDown.value);
        ChangeSelectedCard(selectedCard);
    }


    private void ChangeSelectedCard(VIPCardTeen cardType)
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
                    ChangeSelectedCard(VIPCardTeen.Bronze);
                }
            break;

            case "silver":
                {
                    ChangeSelectedCard(VIPCardTeen.Silver);
                }
                break;

            case "platinum":
                {
                    ChangeSelectedCard(VIPCardTeen.Platinum);
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
        int totalCards = Enum.GetNames(typeof(VIPCardTeen)).Length;

        for (int privilegeCounter = 0; privilegeCounter < privileges.Length; privilegeCounter++)
        {
            for (int cardCounter = 0; cardCounter < totalCards; cardCounter++)
            {
                if (privilegeCounter >= (int)VIP_PriillageTeen.ExlusiveEmoji)
                {
                    privileges[privilegeCounter].transform.GetChild(cardCounter+1).GetChild(0).GetComponent<Text>().text = "" + vipPrivilegeData[cardCounter][dayIndex].vipPrivilege[privilegeCounter].counterValue;
                }
                else
                {
                    privileges[privilegeCounter].transform.GetChild(cardCounter+1).gameObject.SetActive(vipPrivilegeData[cardCounter][dayIndex].vipPrivilege[privilegeCounter].isAvailable);
                }
            }
        }
    }


    private void LoadDefaultValues()
    {
        int totalCards = Enum.GetNames(typeof(VIPCardTeen)).Length;
        int totalPrivillage = Enum.GetNames(typeof(VIP_PriillageTeen)).Length;

        for (int i = 0; i < totalCards; i++)
        {
            List<VIPPrivilegeDataTeen> privilegeList = new List<VIPPrivilegeDataTeen>();

            for (int k = 0; k < 3; k++)
            {
                VIPPrivilegeDataTeen data = new VIPPrivilegeDataTeen();
                data.vipCard = (VIPCardTeen)i;

                int day = 30;


                if (k == 0)
                {
                    day = 30;

                    if (data.vipCard == VIPCardTeen.Bronze)
                    {
                        data.amount = "380";
                    }
                    else if (data.vipCard == VIPCardTeen.Silver)
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

                    if (data.vipCard == VIPCardTeen.Bronze)
                    {
                        data.amount = "980";
                    }
                    else if (data.vipCard == VIPCardTeen.Silver)
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

                    if (data.vipCard == VIPCardTeen.Bronze)
                    {
                        data.amount = "3180";
                    }
                    else if (data.vipCard == VIPCardTeen.Silver)
                    {
                        data.amount = "11380";
                    }
                    else
                    {
                        data.amount = "27880";
                    }
                }



                data.day = day;
                data.vipPrivilege = new VIPPrivilegeTeen[totalPrivillage];

                for (int j = 0; j < totalPrivillage; j++)
                {
                    VIPPrivilegeTeen privilege = new VIPPrivilegeTeen();

                    if (j >= (int)VIP_PriillageTeen.ExlusiveEmoji)
                    {
                        switch ((VIP_PriillageTeen)j)
                        {
                            case VIP_PriillageTeen.FreeEmoji:
                                {
                                    switch (day)
                                    {
                                        case 30:
                                            {
                                                if (data.vipCard == VIPCardTeen.Bronze)
                                                {
                                                    privilege.counterValue = 200;
                                                }
                                                else if (data.vipCard == VIPCardTeen.Silver)
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
                                                if (data.vipCard == VIPCardTeen.Bronze)
                                                {
                                                    privilege.counterValue = 600;
                                                }
                                                else if (data.vipCard == VIPCardTeen.Silver)
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
                                                if (data.vipCard == VIPCardTeen.Bronze)
                                                {
                                                    privilege.counterValue = 2400;
                                                }
                                                else if (data.vipCard == VIPCardTeen.Silver)
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

                            case VIP_PriillageTeen.FreeTimeBank:
                                {
                                    switch (day)
                                    {
                                        case 30:
                                            {
                                                if (data.vipCard == VIPCardTeen.Bronze)
                                                {
                                                    privilege.counterValue = 15;
                                                }
                                                else if (data.vipCard == VIPCardTeen.Silver)
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
                                                if (data.vipCard == VIPCardTeen.Bronze)
                                                {
                                                    privilege.counterValue = 45;
                                                }
                                                else if (data.vipCard == VIPCardTeen.Silver)
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
                                                if (data.vipCard == VIPCardTeen.Bronze)
                                                {
                                                    privilege.counterValue = 180;
                                                }
                                                else if (data.vipCard == VIPCardTeen.Silver)
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

                            if ((VIP_PriillageTeen)j == VIP_PriillageTeen.ClubCreationLimit && data.vipCard == VIPCardTeen.Bronze)
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
                        if (data.vipCard == VIPCardTeen.Bronze)
                        {
                            if (j <= (int)VIP_PriillageTeen.MoreLoginRewards)
                            {
                                privilege.isAvailable = true;
                            }
                            else
                            {
                                privilege.isAvailable = false;
                            }
                        }
                        else if (data.vipCard == VIPCardTeen.Silver)
                        {
                            if (j <= (int)VIP_PriillageTeen.RivalDataDisplay)
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
        int totalPrivillage = Enum.GetNames(typeof(VIP_PriillageTeen)).Length;

        for (int j = 0; j < data["response"].Count; j++)
        {
            int[] listIndex = GetListIndex(data["response"][j]["days"].ToString(), data["response"][j]["cardFeatureTitle"].ToString());

            vipPrivilegeData[listIndex[0]][listIndex[1]].amount = data["response"][j]["purchaseDiamond"].ToString();

            for (int i = 0; i < totalPrivillage; i++)
            {

                switch ((VIP_PriillageTeen)i)
                {
  
                    case VIP_PriillageTeen.AllInEquity:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].isAvailable = data["response"][j]["allInEquity"].ToString() == "Yes";
                        }
                        break;

                    case VIP_PriillageTeen.RabitHunting:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].isAvailable = data["response"][j]["rabbitHunting"].ToString() == "Yes";
                        }
                        break;

                    case VIP_PriillageTeen.MoreLoginRewards:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].isAvailable = data["response"][j]["moreLoginRewards"].ToString() == "Yes";
                        }
                        break;

                    case VIP_PriillageTeen.ReportDetails:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].isAvailable = data["response"][j]["reportDetail"].ToString() == "Yes";
                        }
                        break;

                    case VIP_PriillageTeen.RivalDataDisplay:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].isAvailable = data["response"][j]["rivalDataDisplay"].ToString() == "Yes";
                        }
                        break;

                    case VIP_PriillageTeen.ClubData:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].isAvailable = data["response"][j]["clubData"].ToString() == "Yes";
                        }
                        break;

                    case VIP_PriillageTeen.ExtraDisconnectProtection:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].isAvailable = data["response"][j]["extraDisconnectProtection"].ToString() == "Yes";
                        }
                        break;

                    case VIP_PriillageTeen.ExlusiveEmoji:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].counterValue = (int)float.Parse(data["response"][j]["exclusiveEmoji"].ToString());
                        }
                        break;

                    case VIP_PriillageTeen.ClubCreationLimit:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].counterValue = (int)float.Parse(data["response"][j]["clubCreationLimit"].ToString());
                        }
                        break;

                   
                    case VIP_PriillageTeen.FreeEmoji:
                        {
                            vipPrivilegeData[listIndex[0]][listIndex[1]].vipPrivilege[i].counterValue = (int)float.Parse(data["response"][j]["freeEmoji"].ToString());
                        }
                        break;

                    case VIP_PriillageTeen.FreeTimeBank:
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
        int totalCards = Enum.GetNames(typeof(VIPCardTeen)).Length;
        VIPCardTeen card = VIPCardTeen.Bronze;

        for (int i = 0; i < totalCards; i++)
        {
            if(cardString == ""+(VIPCardTeen)i)
            {
                card = (VIPCardTeen)i;
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
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuControllerTeen.instance.ShowMessage(errorMessage);
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
                MainMenuControllerTeen.instance.ShowMessage(data["message"].ToString());
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
            if (MainMenuControllerTeen.instance.isVIPFromShop)
            {
                MainMenuControllerTeen.instance.isVIPFromShop = false;
                MainMenuControllerTeen.instance.SwitchToMainMenu(true, 0);
            }
            else if (MainMenuControllerTeen.instance.isVIPFromProfile)
            {
                MainMenuControllerTeen.instance.isVIPFromProfile = false;
                MainMenuControllerTeen.instance.SwitchToMainMenu(true, 4);
            }
            else
            {
                MainMenuControllerTeen.instance.SwitchToMainMenu(true);
            }
        }
        
    }



}

public class VIPPrivilegeDataTeen
{
    public VIPCardTeen vipCard;
    public int day;
    public string amount;
    public VIPPrivilegeTeen[] vipPrivilege;
}

public class VIPPrivilegeTeen
{
    public bool isAvailable;
    public int counterValue;
}

[System.Serializable]
public enum VIPCardTeen
{
    Bronze,
    Silver,
    Platinum
    /*Silver,
    Black,
    Golds*/
}

[System.Serializable]
public enum VIP_PriillageTeen
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