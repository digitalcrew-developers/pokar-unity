using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    public bool isOn;

    public Color onColorBg;
    public Color offColorBg;

    public Sprite onImage;
    public Sprite offImage;

    public Image toggleBgImage;
    public RectTransform toggle;

    public GameObject handle;
    private RectTransform handleTransform;

    private float handleSize;
    private float onPosX;
    private float offPosX;

    public float handleOffset;

    public GameObject onIcon;
    public GameObject offIcon;


    public float speed;
    static float t = 0.0f;

    private bool switching = false;

    public delegate void ToggleValue(bool val);
    public event ToggleValue ToggleValueChanged;

    void Awake()
    {
        handleTransform = handle.GetComponent<RectTransform>();
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleSize = handleRect.sizeDelta.x;
        float toggleSizeX = toggle.sizeDelta.x;
        onPosX = (toggleSizeX / 2) - (handleSize / 2) - handleOffset;
        offPosX = onPosX * -1;

    }


    void Start()
    {
        if (isOn)
        {
            //toggleBgImage.color = onColorBg;
            toggleBgImage.sprite = onImage;
            handleTransform.localPosition = new Vector3(onPosX, 0f, 0f);
            onIcon.gameObject.SetActive(true);
            offIcon.gameObject.SetActive(false);
        }
        else
        {
            //toggleBgImage.color = offColorBg;
            toggleBgImage.sprite = offImage;
            handleTransform.localPosition = new Vector3(offPosX, 0f, 0f);
            onIcon.gameObject.SetActive(false);
            offIcon.gameObject.SetActive(true);
        }
    }

    void Update()
    {

        if (switching)
        {
            Toggle(isOn);
        }
    }

    public void DoYourStaff()
    {
        //Debug.Log(isOn);
        ToggleValueChanged?.Invoke(isOn);
    }

    public void Switching()
    {
        switching = true;
    }

    public void ToggleTrue()
    {
        toggleBgImage.sprite = onImage;
        onIcon.SetActive(true);
        offIcon.SetActive(false);
        //toggleBgImage.sprite = SmoothColor(onColorBg, offColorBg, offImage);
        //toggleBgImage.color = SmoothColor(onColorBg, offColorBg);
        Transparency(onIcon, 1f, 0f);
        Transparency(offIcon, 0f, 1f);
        handleTransform.localPosition = /*SmoothMove(handle, onPosX, offPosX);*/ new Vector3(Mathf.Lerp(onPosX, offPosX , /*t +=*/ speed * Time.deltaTime), 0f, 0f);
    }

    public void ToggleFalse()
    {
        toggleBgImage.sprite = offImage;
        onIcon.SetActive(false);
        offIcon.SetActive(true);
        //toggleBgImage.sprite = SmoothColor(onColorBg, offColorBg, offImage);
        //toggleBgImage.color = SmoothColor(onColorBg, offColorBg);
        Transparency(onIcon, 0f, 1f);
        Transparency(offIcon, 1f, 0f);
        handleTransform.localPosition = /*SmoothMove(handle, onPosX, offPosX);*/ new Vector3(Mathf.Lerp(offPosX, onPosX, /*t +=*/ speed * Time.deltaTime), 0f, 0f);
    }

    public void Toggle(bool toggleStatus)
    {
        if (!onIcon.active || !offIcon.active)
        {
            onIcon.SetActive(true);
            offIcon.SetActive(true);
        }

        if (toggleStatus)
        {
            toggleBgImage.sprite = SmoothColor(onColorBg, offColorBg, offImage);
            //toggleBgImage.color = SmoothColor(onColorBg, offColorBg);
            Transparency(onIcon, 1f, 0f);
            Transparency(offIcon, 0f, 1f);
            handleTransform.localPosition = SmoothMove(handle, onPosX, offPosX);
        }
        else
        {
            toggleBgImage.sprite = SmoothColor(onColorBg, offColorBg, onImage);
            //toggleBgImage.color = SmoothColor(offColorBg, onColorBg);
            Transparency(onIcon, 0f, 1f);
            Transparency(offIcon, 1f, 0f);
            handleTransform.localPosition = SmoothMove(handle, offPosX, onPosX);
        }

    }


    Vector3 SmoothMove(GameObject toggleHandle, float startPosX, float endPosX)
    {

        Vector3 position = new Vector3(Mathf.Lerp(startPosX, endPosX, t += speed * Time.deltaTime), 0f, 0f);
        StopSwitching();
        return position;
    }

    Sprite SmoothColor(Color startCol, Color endCol, Sprite bgImage)
    {
        Color resultCol;
        resultCol = Color.Lerp(startCol, endCol, t += speed * Time.deltaTime);

        //return resultCol;
        return bgImage;
    }

    CanvasGroup Transparency(GameObject alphaObj, float startAlpha, float endAlpha)
    {
        CanvasGroup alphaVal;
        alphaVal = alphaObj.gameObject.GetComponent<CanvasGroup>();
        alphaVal.alpha = Mathf.Lerp(startAlpha, endAlpha, t += speed * Time.deltaTime);
        return alphaVal;
    }

    void StopSwitching()
    {
        if (t > 1.0f)
        {
            switching = false;

            t = 0.0f;
            switch (isOn)
            {
                case true:
                    isOn = false;
                    DoYourStaff();
                    break;

                case false:
                    isOn = true;
                    DoYourStaff();
                    break;
            }

        }
    }

}