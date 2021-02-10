using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GetCountryTeen
{
    public string countryCode;
    public string CountryName, countryFlag;
}

public class SelectCountryRegionTeen : MonoBehaviour
{
    public static SelectCountryRegionTeen instance;
    [SerializeField]
    private List<GetCountryTeen> allCountries;
    public GameObject CountryPrefeb, prefebParent;

    public InputField countryNameInputField;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void Start()
    {
        WebServices.instance.SendRequest(RequestType.GetCountryList, "", true, OnServerResponseFound);

        countryNameInputField.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    private void ValueChangeCheck()
    {
        for(int i=0; i< AllCountries.Count; i++)
        {
            if (string.IsNullOrEmpty(countryNameInputField.text))
            {
                AllCountries[i].SetActive(true);
            }
            else
            {
                AllCountries[i].SetActive(false);
            }
        }
        IEnumerable<GameObject> results = AllCountries.Where(s => s.name.ToLower().Contains(countryNameInputField.text));
        var ls = results.ToList();

        for (int i = 0; i < ls.Count; i++)
        {
            ls[i].SetActive(true);
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



        if (requestType == RequestType.GetCountryList)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["status"].Equals(true))
            {
                ReadAllCountries(data);
            }
            else
            {
                MainMenuControllerTeen.instance.ShowMessage("Unable to get data");
            }

        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhadnled response found in  = " + requestType);
#endif
        }

    }

    private List<GameObject> AllCountries = new List<GameObject>();
    private void ReadAllCountries(JsonData data)
    {
       
        for (int i = 0; i < data["getData"].Count; i++)
        {
            GetCountryTeen getCountry = new GetCountryTeen();

            getCountry.countryCode = data["getData"][i]["countryCode"].ToString();
            getCountry.CountryName = data["getData"][i]["countryName"].ToString();
            getCountry.countryFlag = data["getData"][i]["countryFlag"].ToString();

            GameObject sc = Instantiate(CountryPrefeb, prefebParent.transform);
            sc.name = getCountry.CountryName;
            sc.GetComponent<SelectCountryTeen>().countryName.text = getCountry.CountryName;
            sc.GetComponent<SelectCountryTeen>().countryURL = getCountry.countryFlag;
            sc.GetComponent<SelectCountryTeen>().countryID = data["getData"][i]["countryCode"].ToString();
            allCountries.Add(getCountry);
            AllCountries.Add(sc);
        }
       
    }


    public void OnCloseCountryRegion()
    {
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.SelectRegion);
    }
    public void ConfirmBTN()
    {
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.SelectRegion);
    }
}
