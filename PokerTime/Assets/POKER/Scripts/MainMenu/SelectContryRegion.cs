using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GetCountry
{
    public string countryCode;
    public string CountryName, countryFlag;

}

public class SelectContryRegion : MonoBehaviour
{
    [SerializeField]
    private List<GetCountry> allCountries;
    public GameObject CountryPrefeb, prefebParent;

    public void Start()
    {
        WebServices.instance.SendRequest(RequestType.GetCountryList, "", true, OnServerResponseFound);


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



        if (requestType == RequestType.GetCountryList)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["status"].Equals(true))
            {
                ReadAllCountries(data);
            }
            else
            {
                MainMenuController.instance.ShowMessage("Unable to get data");
            }

        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhadnled response found in  = " + requestType);
#endif
        }

    }
    private void ReadAllCountries(JsonData data)
    {
       
        for (int i = 0; i < data["getData"].Count; i++)
        {
            GetCountry getCountry = new GetCountry();

            getCountry.countryCode = data["getData"][i]["countryCode"].ToString();
            getCountry.CountryName = data["getData"][i]["countryName"].ToString();
            getCountry.countryFlag = data["getData"][i]["countryFlag"].ToString();

            GameObject sc = Instantiate(CountryPrefeb, prefebParent.transform);
            sc.GetComponent<SelectCountry>().countryName.text = getCountry.CountryName;
            sc.GetComponent<SelectCountry>().countryURL = getCountry.countryFlag;
            sc.GetComponent<SelectCountry>().countryID = data["getData"][i]["countryCode"].ToString();
            allCountries.Add(getCountry);
        }
       
    }


    public void OnCloseCountryRegion()
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.SelectRegion);
    }
    public void ConfirmBTN()
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.SelectRegion);
    }
}
