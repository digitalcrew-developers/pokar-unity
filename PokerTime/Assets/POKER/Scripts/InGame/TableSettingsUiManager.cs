
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
     
public class TableSettingsUiManager : MonoBehaviour
{
    TableSetting ts = new TableSetting();
    public GameObject Voiceon, VoiceOff;
    public GameObject Texton, TextOff;
    public GameObject soundon, soundOff;
    public GameObject vibrationon, vibrationOff;
    public GameObject guseyournexton, guseyournextOff;
    public GameObject exactBettingon, exactBettingOff;
    public GameObject customizedActionbtnon, customizedActionbtnOff;
    public GameObject betSlider, RaiseSlider;

    RequestType tabletype;
    private int issound ,isVociemsg,isTextmsg, isvibrations,isguesyourNexthand,isexactBetting, iscustomizedActionbtn,israise;
    public void Start()
    {
        tabletype = RequestType.GetTableSettingData;
        WebServices.instance.SendRequest(RequestType.GetTableSettingData, "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"}", true, OnServerResponseFound);
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
        if (requestType ==tabletype)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                Debug.Log("Success data send");
                for (int i = 0; i < data["getData"].Count; i++)
                {
                   ts= new TableSetting();                    
                    ts.IsVociemsg =int.Parse (data["getData"][i]["voiceMessage"].ToString());
                    ts.IsTextmsg =int.Parse (data["getData"][i]["textMessage"].ToString());
                    ts.Issound = int.Parse(data["getData"][i]["soundEffects"].ToString());
                    ts.Isvibrations = int.Parse(data["getData"][i]["vibrations"].ToString());
                    ts.IsguesyourNexthand = int.Parse(data["getData"][i]["nextHand"].ToString());
                    ts.IsexactBetting = int.Parse(data["getData"][i]["exactBetting"].ToString());
                    ts.IscustomizedActionbtn = int.Parse(data["getData"][i]["bet"].ToString());
                    ts.Israise = int.Parse(data["getData"][i]["raise"].ToString());
                    if (tabletype == RequestType.GetTableSettingData)
                    {
                        LoadResetTableData();
                    }
                }

            }
            else
            {
               MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
    }   

    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    InGameUiManager.instance.DestroyScreen(InGameScreens.TableSettings);
                }
                break;
            case "voicemsg":
                {
                    if (isVociemsg==1)
                    {
                        isVociemsg = 0;
                        Voiceon.SetActive(false);
                        VoiceOff.SetActive(true);
                       
                      
                    }
                    else
                    {
                        isVociemsg = 1;
                        Voiceon.SetActive(true);
                        VoiceOff.SetActive(false);
                   
                    }

                }
                break;
            case "textmsg":
                {
                    if (isTextmsg==1)
                    {
                        isTextmsg = 0;
                        Texton.SetActive(false);
                        TextOff.SetActive(true);
                     
                    }
                    else
                    {
                        isTextmsg = 1;
                        Texton.SetActive(true);
                        TextOff.SetActive(false);
                      
                    }
                }
                break;
            case "soundEffect":
                {
                    if (issound==1)
                    {
                        issound = 0;
                        PlayerPrefs.SetInt("issound", issound);
                        soundon.SetActive(false);
                        soundOff.SetActive(true);
                    }
                    else
                    {
                        issound = 1;
                        PlayerPrefs.SetInt("issound", issound);
                        soundon.SetActive(true);
                        soundOff.SetActive(false);
                    }
                   
                }
                break;
            case "vibration":
                {
                    if (isvibrations==1)
                    {
                        isvibrations = 0;
                     
                        vibrationon.SetActive(false);
                        vibrationOff.SetActive(true);
                    }
                    else
                    {
                        isvibrations = 1;
                        vibrationon.SetActive(true);
                        vibrationOff.SetActive(false);
                     
                    }
                }
                break;
            case "guesyournext":
                {
                    if (isguesyourNexthand==1)
                    {
                        isguesyourNexthand = 0;
                        guseyournexton.SetActive(false);
                        guseyournextOff.SetActive(true);
                      
                    }
                    else
                    {
                        isguesyourNexthand = 1;
                        guseyournexton.SetActive(true);
                        guseyournextOff.SetActive(false);
                     
                    }
                }
                break;
            case "Exactbetting":
                {
                    if (isexactBetting==1)
                    {
                        isexactBetting = 0;
                        exactBettingon.SetActive(false);
                        exactBettingOff.SetActive(true);
                       
                    }
                    else
                    {
                        isexactBetting = 1;
                        exactBettingon.SetActive(true);
                        exactBettingOff.SetActive(false);
                       
                    }
                }
                break;
            case "customizedaction":
                {
                    if (iscustomizedActionbtn==1)
                    {
                        iscustomizedActionbtn = 0;
                        customizedActionbtnon.SetActive(false);
                        customizedActionbtnOff.SetActive(true);
                        RaiseSlider.SetActive(false);
                        betSlider.SetActive(false);

                    }
                    else
                    {
                        RaiseSlider.SetActive(true);
                        betSlider.SetActive(true);
                        iscustomizedActionbtn = 1;
                        customizedActionbtnon.SetActive(true);
                        customizedActionbtnOff.SetActive(false);
                    }
                }
                break;
            default:
                {
                    Debug.LogError("Unhandled eventName found in TableSettingsUiManager = " + eventName);
                }
                break;
        }
                AllPlayerPrefs();
    }
    public void AllPlayerPrefs()
    {
        
          string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                          "\"voiceMessage\":\"" + isVociemsg + "\"," +
                           "\"textMessage\":\"" + isTextmsg + "\"," +
                            "\"soundEffects\":\"" + issound + "\"," +
                              "\"vibrations\":\"" + isvibrations + "\"," +
                           "\"nextHand\":\"" + isguesyourNexthand + "\"," +
                            "\"exactBetting\":\"" + isexactBetting + "\"," +
                             "\"bet\":\"" + iscustomizedActionbtn + "\"," +
                          "\"raise\":\"" + israise + "\"}";
        tabletype = RequestType.UpdateTableSettings;
        WebServices.instance.SendRequest(RequestType.UpdateTableSettings, requestData, true, OnServerResponseFound);
    }
    void LoadResetTableData()
    {
        if (ts.IsVociemsg == 1)
        {         
            Voiceon.SetActive(true);
            VoiceOff.SetActive(false);
        }
        else
        {            
            Voiceon.SetActive(false);
            VoiceOff.SetActive(true);
        }
        if (ts.IsTextmsg == 1)
        {
            Texton.SetActive(true);
            TextOff.SetActive(false);
        }
        else
        {
            Texton.SetActive(false);
            TextOff.SetActive(true);
        }

        if (ts.Issound == 1) { 
            soundon.SetActive(true);
            soundOff.SetActive(false);
        }
        else
        
        {
            soundon.SetActive(false);
            soundOff.SetActive(true);
        }
        if (ts.Isvibrations == 1)
        {
            vibrationon.SetActive(true);
            vibrationOff.SetActive(false);
        }
        else
        {
            vibrationon.SetActive(false);
            vibrationOff.SetActive(true);

        }
        if (ts.IsguesyourNexthand == 1)
        {

            guseyournexton.SetActive(true);
            guseyournextOff.SetActive(false);
        }
        else
        {
            guseyournexton.SetActive(false);
            guseyournextOff.SetActive(true);
        }

        if (ts.IsexactBetting == 1)
        {
            exactBettingon.SetActive(true);
            exactBettingOff.SetActive(false);
        }
        else
        {
            exactBettingon.SetActive(false);
            exactBettingOff.SetActive(true);
        }

        if (ts.IscustomizedActionbtn == 1)
        {
            customizedActionbtnon.SetActive(true);
            customizedActionbtnOff.SetActive(false);
            RaiseSlider.SetActive(true);
            betSlider.SetActive(true);
        }
        else
        {
            RaiseSlider.SetActive( false);
            betSlider.SetActive(false);
            customizedActionbtnon.SetActive(false);
            customizedActionbtnOff.SetActive(true);
        }
        issound = ts.Issound;
        isVociemsg = ts.IsVociemsg;
        isTextmsg = ts.IsTextmsg;
        isvibrations = ts.Isvibrations;
        isguesyourNexthand = ts.IsguesyourNexthand;
        isexactBetting = ts.IsexactBetting;
        iscustomizedActionbtn = ts.IscustomizedActionbtn;
        israise=ts.Israise;
    }
}
[System.Serializable]
public class TableSetting
{
    public int Issound, IsVociemsg, IsTextmsg,Isvibrations, IsguesyourNexthand, IsexactBetting,IscustomizedActionbtn,Israise;
}