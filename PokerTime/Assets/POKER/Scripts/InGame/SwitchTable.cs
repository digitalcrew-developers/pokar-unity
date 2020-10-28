using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchTable : MonoBehaviour
{
    public List<GameObject> PreviewImages = new List<GameObject>();
    public List<GameObject> TableImages = new List<GameObject>();
    public Button PreviousBtn, NextBtn;

    private int counter = 0;
    public TMPro.TextMeshProUGUI CountText;

    public TMPro.TextMeshProUGUI ButtonText;
    public GameObject InUse, Use;
    public Button ConfirmButton;

    private void Start()
    {
        counter = PlayerPrefs.GetInt("TableCount");

        int c1 = PreviewImages.Count - 1;
        CountText.text = counter + "/" + c1;

        UpdateButtonState();

        PreviousBtn.onClick.RemoveAllListeners();
        NextBtn.onClick.RemoveAllListeners();

        PreviousBtn.onClick.AddListener(Decrement);
        NextBtn.onClick.AddListener(Increment);
        ConfirmButton.onClick.AddListener(SwitchConfirm);
    }

    private void Increment()
    {
        counter++;
        if (counter > PreviewImages.Count - 1)
        {
            counter = 0;
        }
        UpdateButtonState();

        int c1 = PreviewImages.Count - 1;
        CountText.text = counter + "/" + c1;
    }

    private void Decrement()
    {
        counter--;
        if (counter < 0)
        {
            counter = PreviewImages.Count - 1;
        }
        UpdateButtonState();

        int c1 = PreviewImages.Count - 1;
        CountText.text = counter + "/" + c1;
    }

    private void UpdateButtonState()
    {
        foreach (GameObject g in PreviewImages)
        {
            g.SetActive(false);
        }
        PreviewImages[counter].SetActive(true);
        
        if (counter == PlayerPrefs.GetInt("TableCount"))
        {
            ButtonText.text = "In Use";
            InUse.SetActive(true);
            Use.SetActive(false);
            ConfirmButton.enabled = false;
        }
        else
        {
            ButtonText.text = "Use";
            InUse.SetActive(false);
            Use.SetActive(true);
            ConfirmButton.enabled = true;
        }
    }

    public void SwitchConfirm()
    {
        SwitchTables(counter);
        ClosePanel();
    }

    public void ClosePanel()
    {
        PlayerPrefs.SetInt("TableCount", counter);
        gameObject.SetActive(false);
    }

    private void SwitchTables(int counter)
    {
        PlayerPrefs.SetInt("TableCount", counter);

        ButtonText.text = "In Use";
        InUse.SetActive(true);
        Use.SetActive(false);
        ConfirmButton.enabled = false;

        foreach (GameObject g in TableImages)
        {
            g.SetActive(false);
        }
        TableImages[counter].SetActive(true);
        
    }
}