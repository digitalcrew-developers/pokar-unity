using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClubTableController : MonoBehaviour
{
    public static ClubTableController instance;

    [Header("NLH")]
    public Button RingGameTabButton_NLH;
    public Button SNGGameTabButton_NLH;
    public Button MTTGameTabButton_NLH;
    public GameObject RingGamePanel_NLH, SNGamePanel_NLH, MTTGamePanel_NLH;
    public List<GameObject> NLH_RingGameSettings = new List<GameObject>();
    public List<GameObject> NLH_SNGGameSettings = new List<GameObject>();
    public List<GameObject> NLH_MTTGameSettings = new List<GameObject>();

    public Toggle EVChop;
    public GameObject EVChopValueField;

    [Header("PLO")]
    public Button RingGameTabButton_PLO;
    public Button SNGGameTabButton_PLO;
    public Button MTTGameTabButton_PLO;
    public GameObject RingGamePanel_PLO, SNGamePanel_PLO, MTTGamePanel_PLO;
    public List<GameObject> PLO_RingGameSettings = new List<GameObject>();
    public List<GameObject> PLO_SNGGameSettings = new List<GameObject>();
    public List<GameObject> PLO_MTTGameSettings = new List<GameObject>();

    public Toggle EVChop_PLO;
    public GameObject EVChopValueField_PLO;

    [Header("MIXED GAME")]
    public Button RingGameTabButton_MIXED;
    public Button SNGGameTabButton_MIXED;
    public Button MTTGameTabButton_MIXED;
    public GameObject RingGamePanel_MIXED, SNGamePanel_MIXED, MTTGamePanel_MIXED;
    public List<GameObject> MIXED_RingGameSettings = new List<GameObject>();
    public List<GameObject> MIXED_SNGGameSettings = new List<GameObject>();
    public List<GameObject> MIXED_MTTGameSettings = new List<GameObject>();

    public Toggle EVChop_MIXED;
    public GameObject EVChopValueField_MIXED;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    private void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        RingGameTabButton_NLH.onClick.RemoveAllListeners();
        SNGGameTabButton_NLH.onClick.RemoveAllListeners();
        MTTGameTabButton_NLH.onClick.RemoveAllListeners();


        RingGameTabButton_NLH.onClick.AddListener(() => OpenScreen("Ring"));
        SNGGameTabButton_NLH.onClick.AddListener(() => OpenScreen("SNG"));
        MTTGameTabButton_NLH.onClick.AddListener(() => OpenScreen("MTT"));

        EVChop.onValueChanged.AddListener(delegate {
            ToggleValueChanged(EVChop);
        });
    }

    void ToggleValueChanged(Toggle change)
    {
        if (change.isOn)
        {
            EVChopValueField.SetActive(true);
        }
        else
        {
            EVChopValueField.SetActive(false);
        }
    }


    private void OpenScreen(string screenName)
    {
        Color c = new Color(1, 1, 1, 1);
        Color c1 = new Color(1, 1, 1, 0);

        switch (screenName)
        {
            case "Ring":
                RingGameTabButton_NLH.GetComponent<Image>().color = c;
                SNGGameTabButton_NLH.GetComponent<Image>().color = c1;
                MTTGameTabButton_NLH.GetComponent<Image>().color = c1;

                RingGamePanel_NLH.SetActive(true);
                SNGamePanel_NLH.SetActive(false);
                MTTGamePanel_NLH.SetActive(false);
                break;
            case "SNG":
                RingGameTabButton_NLH.GetComponent<Image>().color = c1;
                SNGGameTabButton_NLH.GetComponent<Image>().color = c;
                MTTGameTabButton_NLH.GetComponent<Image>().color = c1;

                RingGamePanel_NLH.SetActive(false);
                SNGamePanel_NLH.SetActive(true);
                MTTGamePanel_NLH.SetActive(false);
                break;
            case "MTT":
                RingGameTabButton_NLH.GetComponent<Image>().color = c1;
                SNGGameTabButton_NLH.GetComponent<Image>().color = c1;
                MTTGameTabButton_NLH.GetComponent<Image>().color = c;

                RingGamePanel_NLH.SetActive(false);
                SNGamePanel_NLH.SetActive(false);
                MTTGamePanel_NLH.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "save_nlh":
                {
                    
                }
                break;
            case "create_nlh":
                {

                }
                break;
            case "save_plo":
                {

                }
                break;
            case "create_plo":
                {

                }
                break;
            case "save_mixed":
                {

                }
                break;
            case "create_mixed":
                {

                }
                break;
        }
    }

    private void OnClickOnSave()
    {
        
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



        switch (requestType)
        {
            case RequestType.UpdateTemplateStatus:
                {
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    if (data["success"].ToString() == "1")
                    {
                        //createClubPopUp.SetActive(false);
                        //MainMenuController.instance.ShowMessage("Club created successfully");
                        //ClubListUiManager.instance.FetchList();
                    }
                    else
                    {
                        //MainMenuController.instance.ShowMessage(data["message"].ToString());
                    }
                }
                break;


            case RequestType.CreateTemplate:
                {
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    if (data["success"].ToString() == "1")
                    {
                        //joinClubPopUp.SetActive(false);
                        //MainMenuController.instance.ShowMessage("Club join request sent");
                    }
                    else
                    {
                        //MainMenuController.instance.ShowMessage(data["message"].ToString());
                    }
                }
                break;


            default:
#if ERROR_LOG
                Debug.LogError("Unhandled requestType found in  MenuHandller = " + requestType);
#endif
                break;
        }
    }
}