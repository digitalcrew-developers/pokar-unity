using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class JackpotManager : MonoBehaviour
{
    public static JackpotManager instance;

    public string JackpotAmount = "";
    public static bool isJackpotActivated = false;
    public GameObject popUpText;
    public GameObject TipsObj;
    public GameObject TopUpJackpotPrefab;
    public Transform TopUpJackpotContainer;
    public TMP_Text TotalJackpotAmountText;
    public TextMeshProUGUI ChipsAvailableText;
    public TMP_InputField JackpotAmountInputField;
    public Button JackpotTopUpConfimButton;
    public ToggleController JackpotToggleController;
    public GameObject JackpotTopUpPopup, JackpotExplanationPanel, JackpotPayoutPanel, TopUpPanel, TopRecordPanel;
    public Button JackpotExplanationButton, JackpotPayoutButton, TopUpButton, TopupTabButton, TopupRecordTabButton;
    private bool isExplanationScreenOpen = true;
    public bool IsJackpotEnabled { get => IsJackpotEnabled; }
    private bool isJackpotEnabled { get; set; }
    public GameObject JackpotPanel, TurnOffJackpotPanel;

    private void OnEnable()
    {
        if (instance == null)
            instance = this;

        Initialize();
    }

    private void Initialize()
    {
        JackpotExplanationButton.onClick.RemoveAllListeners();
        JackpotPayoutButton.onClick.RemoveAllListeners();
        TopUpButton.onClick.RemoveAllListeners();
        TopupTabButton.onClick.RemoveAllListeners();
        TopupRecordTabButton.onClick.RemoveAllListeners();

        TopUpButton.onClick.AddListener(OpenTopupPopup);

        JackpotExplanationButton.onClick.AddListener(OpenJackpotExplanationPanel);
        JackpotPayoutButton.onClick.AddListener(OpenJackpotPayoutPanel);

        TopupTabButton.onClick.AddListener(OpenTopUpPanel);
        TopupRecordTabButton.onClick.AddListener(OpenTopRecordPanel);

        //Default Open TopupPanel
        OpenTopUpPanel();
        RequestJackpotDetails();

        JackpotToggleController.ToggleValueChanged += JackpotToggleController_ToggleValueChanged;

        JackpotTopUpConfimButton.onClick.RemoveAllListeners();
        JackpotTopUpConfimButton.onClick.AddListener(SendTopUpJackpotRequest);

        ChipsAvailableText.text = ClubDetailsUIManager.instance.CLubChips.text;
    }

    public void RequestJackpotDetails()
    {
        string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"}";
        WebServices.instance.SendRequest(RequestType.GetJackpotDetailByClubId, requestData, true, OnServerResponseFound);
    }

    private void SendTopUpJackpotRequest()
    {
        float amount = 0;
        float availableAmount = 0;
        float.TryParse(JackpotAmountInputField.text, out amount);
        float.TryParse(ChipsAvailableText.text, out availableAmount);
        //Debug.Log("Jackpot Amount: " + amount);
        //Debug.Log("Available Amount: " + availableAmount);

        if (amount > availableAmount)
        {
            //Debug.Log("Insufficient Amount...");
            StartCoroutine(ShowPopUp("Insufficient amount", 1.29f));
        }
        else if (amount == 0)
        {
            StartCoroutine(ShowPopUp("Please enter valid amount", 1.29f));
        }
        else
        {
            string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
                                     "\"userId\":\"" + MemberListUIManager.instance.GetClubOwnerObject().userId + "\"," +
                                     "\"jackpotAmount\":\"" + amount + "\"}";

            WebServices.instance.SendRequest(RequestType.TopUpJackpot, requestData, true, OnServerResponseFound);
        }
    }

    private void JackpotToggleController_ToggleValueChanged(bool val)
    {
        if (isJackpotActivated)
        {
            ClubDetailsUIManager.instance.SetJackpotStatus(val);
            //Debug.Log("jackpot status :" + val.ToString());
            string b = string.Empty;
            if (val) { b = "1"; } else { b = "0"; }
            //Debug.Log(ClubDetailsUIManager.instance.GetClubUniqueId());

            string requestData = "{\"uniqueClubId\":\"" + ClubDetailsUIManager.instance.GetClubUniqueId() + "\"," +
                                "\"clubName\":\"" + ClubDetailsUIManager.instance.GetClubName() + "\"," +
                                "\"clubStatus\":\"" + "1" + "\"," +
                                "\"jackpotToggle\":\"" + b + "\"," +
                                "\"layout\":\"" + ClubDetailsUIManager.instance.GetLayout() //to-do. get layout from club details ui manager
                                + "\"}";

            WebServices.instance.SendRequest(RequestType.UpdateClub, requestData, true, OnServerResponseFound);

            if (!val)
            {
                TurnOffJackpotPanel.SetActive(true);
                TurnOffJackpotPanel.transform.Find("BG1/Heading/Close").GetComponent<Button>().onClick.RemoveAllListeners();
                TurnOffJackpotPanel.transform.Find("BG1/Heading/Close").GetComponent<Button>().onClick.AddListener(() => OnCloseTurnOffJackpotPanel());
                TurnOffJackpotPanel.transform.Find("BG1/BG2/CenterArea/Text (TMP)").GetComponent<TMP_Text>().text = "ID: " + ClubDetailsUIManager.instance.GetClubId();
                TurnOffJackpotPanel.transform.Find("BG1/BG2/CenterArea/BtnConfirm").GetComponent<Button>().onClick.AddListener(() => CheckToConfirm(val));
            }
            else if (val)
            {
                TurnOffJackpotPanel.SetActive(false);
                string req = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
                             "\"status\":\"" + "Active" + "\"}";

                WebServices.instance.SendRequest(RequestType.OnOffJackpot, req, true, OnServerResponseFound);
            }
        }
        else
        {
            if (val)
            {
                JackpotTopUpPopup.SetActive(true);
                OpenTopUpPanel();
            }
        }
    }

    private void OnCloseTurnOffJackpotPanel()
    {
        //JackpotToggleController.isOn = true;
        //JackpotToggleController.DoYourStaff();
        JackpotToggleController.isOn = true;
        JackpotToggleController.Toggle(true);
        TurnOffJackpotPanel.SetActive(false);
    }

    private void CheckToConfirm(bool val)
    {
        if (TurnOffJackpotPanel.transform.Find("BG1/BG2/CenterArea/InputField (TMP)").GetComponent<TMP_InputField>().text.Length <= 0)
        {
            StartCoroutine(ShowPopUp("Enter Club ID", 1.29f));
        }
        else if (!TurnOffJackpotPanel.transform.Find("BG1/BG2/CenterArea/InputField (TMP)").GetComponent<TMP_InputField>().text.Equals(ClubDetailsUIManager.instance.GetClubId()))
        {
            StartCoroutine(ShowPopUp("ID incorrect", 1.29f));
        }
        else
        {
            TipsObj.SetActive(true);
            TipsObj.transform.Find("BG1/BG2/ConfirmDeleteMember").GetComponent<Button>().onClick.RemoveAllListeners();
            TipsObj.transform.Find("BG1/BG2/ConfirmDeleteMember").GetComponent<Button>().onClick.AddListener(() => OnConfirmDisableJackpot());
        }
    }

    private void OpenTopUpPanel()
    {
        //to-do.. get current chips

        Color c = new Color(1, 1, 1, 1);
        TopupTabButton.GetComponent<Image>().color = c;

        Color c1 = new Color(1, 1, 1, 0);
        TopupRecordTabButton.GetComponent<Image>().color = c1;

        TopUpPanel.SetActive(true);
        TopRecordPanel.SetActive(false);
    }

    private void OpenJackpotPayoutPanel()
    {
        isExplanationScreenOpen = false;
        Color c = new Color(1, 1, 1, 0);
        JackpotExplanationButton.GetComponent<Image>().color = c;

        Color c1 = new Color(1, 1, 1, 1);
        JackpotPayoutButton.GetComponent<Image>().color = c1;

        JackpotExplanationPanel.SetActive(false);
        JackpotPayoutPanel.SetActive(true);
    }

    private void OpenTopupPopup()
    {
        JackpotTopUpPopup.SetActive(true);
        OpenTopUpPanel();
    }

    private void OpenJackpotExplanationPanel()
    {
        isExplanationScreenOpen = true;
        Color c = new Color(1, 1, 1, 1);
        JackpotExplanationButton.GetComponent<Image>().color = c;

        Color c1 = new Color(1, 1, 1, 0);
        JackpotPayoutButton.GetComponent<Image>().color = c1;

        JackpotExplanationPanel.SetActive(true);
        JackpotPayoutPanel.SetActive(false);
    }

    public void OnCloseTopUpPanel()
    {
        if (!isJackpotActivated)
        {
            JackpotToggleController.isOn = false;
            //JackpotToggleController.Toggle(false);
            JackpotToggleController.Toggle(false);
            JackpotTopUpPopup.SetActive(false);
        }
        else
        {
            JackpotToggleController.isOn = true;
            //JackpotToggleController.Toggle(false);
            JackpotToggleController.Toggle(true);
            JackpotTopUpPopup.SetActive(false);
        }
    }

    private void OpenTopRecordPanel()
    {
        Color c = new Color(1, 1, 1, 0);
        TopupTabButton.GetComponent<Image>().color = c;

        Color c1 = new Color(1, 1, 1, 1);
        TopupRecordTabButton.GetComponent<Image>().color = c1;

        TopUpPanel.SetActive(false);
        TopRecordPanel.SetActive(true);

        WebServices.instance.SendRequest(RequestType.GetTopUpDetailsByClubId, "{\"clubId\":" + ClubDetailsUIManager.instance.GetClubId() + "}", true, OnServerResponseFound);
    }

    private void OnConfirmDisableJackpot()
    {
        string req = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
                     "\"status\":\"" + "Inactive" + "\"}";

        WebServices.instance.SendRequest(RequestType.OnOffJackpot, req, true, OnServerResponseFound);
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
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
            case RequestType.UpdateClub:
                {
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    if (data["status"].Equals(true))
                    {
                        if (ClubAdminManager.instance.DisbandClub.activeInHierarchy)//api response is for disband club
                        {
                            MainMenuController.instance.ShowMessage("Club has been disbanded", () => {
                                ClubDetailsUIManager.instance.OnClickOnButton("back");
                                for (int i = 0; i < ClubListUiManager.instance.container.childCount; i++)
                                {
                                    Destroy(ClubListUiManager.instance.container.GetChild(i).gameObject);
                                }
                                ClubListUiManager.instance.FetchList(true);
                            });
                        }
                    }
                    else
                    {
                        MainMenuController.instance.ShowMessage(data["message"].ToString());
                    }
                }
                break;
            case RequestType.GetJackpotDetailByClubId:
                {
                    Debug.Log("Response => GetJackpotDetailsByClubId : " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    //Debug.Log("Status: " + data["success"]);
                    if (data["status"].Equals(true))
                    {
                        if (!isJackpotActivated)
                            isJackpotActivated = true;

                        int a = data["data"][0]["jackpotAmount"].ToString().Length;
                        int b = TotalJackpotAmountText.text.Length;

                        string str = "";
                        for (int i = 0; i < (b - a); i++)
                        {
                            if (i == 1)
                            {
                                str += ",";
                                continue;
                            }
                            else if (i == 5)
                            {
                                str += ",";
                                continue;
                            }
                            str += "0";
                        }

                        str += data["data"][0]["jackpotAmount"].ToString();

                        JackpotAmount = str;
                        //Debug.Log("Jackpot Amount: " + JackpotAmount);
                        TotalJackpotAmountText.text = JackpotAmount;

                        //Debug.Log("Jackpot is active");
                        if (data["data"][0]["jackpotStatus"].ToString().Equals("Active"))
                        {
                            JackpotToggleController.isOn = true;
                            //JackpotToggleController.DoYourStaff();
                            JackpotToggleController.Toggle(true);
                        }
                        else
                        {
                            JackpotToggleController.isOn = false;
                            //JackpotToggleController.DoYourStaff();
                            JackpotToggleController.Toggle(false);
                        }
                    }
                    else
                    {
                        Debug.Log("No Jackpot is available...");
                        JackpotToggleController.isOn = false;
                        //JackpotToggleController.DoYourStaff();
                        JackpotToggleController.Toggle(false);
                    }
                }
                break;

            case RequestType.TopUpJackpot:
                {
                    Debug.Log("Response => TopUpJackpot : " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    if (data["status"].Equals(true)/*["message"].ToString() == "Jackpot topup successfully"*/)
                    {
                        JackpotToggleController.isOn = true;
                        JackpotToggleController.Toggle(true);
                        JackpotTopUpPopup.SetActive(false);
                        JackpotAmountInputField.text = "";

                        StartCoroutine(ShowPopUp("Topped Up Successfully", 1.29f));
                    }
                }
                break;

            case RequestType.OnOffJackpot:
                {
                    Debug.Log("Response => OnOffJackpot : " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    if (data["status"].Equals(true))
                    {
                        if (TipsObj.activeSelf)
                            TipsObj.SetActive(false);

                        if (TurnOffJackpotPanel.activeSelf)
                        {
                            TurnOffJackpotPanel.SetActive(false);
                            TurnOffJackpotPanel.transform.Find("BG1/BG2/CenterArea/InputField (TMP)").GetComponent<TMP_InputField>().text = "";
                            ClubDetailsUIManager.instance.jackpotData.SetActive(false);
                            ClubDetailsUIManager.instance.otherDetails.transform.localPosition = new  Vector3(-51, 0, 0);
                        }

                        ClubDetailsUIManager.instance.FetchJackpotDetails();
                    }
                }
                break;

            case RequestType.GetTopUpDetailsByClubId:
                {
                    Debug.Log("Response => GetTopUpDetailsByClubId : " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    if (data["success"].Equals(1))
                    {
                        for (int i = 0; i < TopUpJackpotContainer.childCount; i++)
                        {
                            Destroy(TopUpJackpotContainer.GetChild(0).gameObject);
                        }

                        for (int i = 0; i < data["data"].Count; i++)
                        {
                            GameObject gm = Instantiate(TopUpJackpotPrefab, TopUpJackpotContainer) as GameObject;
                            gm.transform.Find("Data").GetComponent<TMP_Text>().gameObject.SetActive(false);
                            gm.transform.Find("Name").GetComponent<TMP_Text>().text = MemberListUIManager.instance.GetClubOwnerObject().userAlias + " topped up";
                            //gm.transform.Find("Name").GetComponent<RectTransform>().position = new Vector3(18, 12, 0);
                            gm.transform.Find("Time").GetComponent<TMP_Text>().text = data["data"][i]["created"].ToString().Substring(0, 10) + " " + data["data"][i]["created"].ToString().Substring(11, 5);
                            gm.transform.Find("Image/Coins").GetComponent<TMP_Text>().text = "-" + data["data"][i]["jackpotAmount"].ToString();
                        }
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

    IEnumerator ShowPopUp(string msg, float delay)
    {
        popUpText.SetActive(true);
        popUpText.transform.GetChild(0).GetComponent<Text>().text = msg;
        yield return new WaitForSeconds(delay);
        popUpText.SetActive(false);
    }
}