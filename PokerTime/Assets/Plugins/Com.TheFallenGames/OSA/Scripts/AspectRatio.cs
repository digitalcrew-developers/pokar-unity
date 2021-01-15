using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AspectRatio : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        CalcAspect();
    }


    void CalcAspect()
    {
        decimal r = (decimal)Screen.height / Screen.width;
        Debug.Log(Screen.width + " screen " + Screen.height);
        string _r = r.ToString("F2");
        string ratio = _r.Substring(0, 4);
        Debug.Log("Ratio is : " + ratio);
        float rat = float.Parse(ratio);
        if(rat >= 2)
        {
            GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
        }
        else
        {
            GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
        }
        Debug.Log("matchWidthOrHeight : " + GetComponent<CanvasScaler>().matchWidthOrHeight);
        /*switch (ratio)
        {
            case "0.60": case "2.11": //4:3
                gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
                break;
                case "1.33":  //4:3
                    this.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
                    break;
                case "1.50": //3:2
                    this.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
                    break;

                case "1.78": //16:9 && 1920 * 1080 && 1280 * 720
                    this.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
                    break;
                case "1.60": //16:10
                    this.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
                    break;
                case "1.25": //5:4
                    this.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
                    break;
                case "2.00": //18:9 && 2160 * 1080
                    this.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 1f;
                    break;
                case "2.11": //2280 * 1080
                    this.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 1f;
                    break;
                case "2.06": //2960 * 1440
                    this.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 1f;
                    break;
                case "1.67"://800 * 400
                    this.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
                    break;
                case "1.70"://17:10
                    this.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
                    break;
                case "2.17": //IphoneX
                    this.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
                    break;
                //case "2.11": //Vivo V9
                //    this.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
                //    break;
                //case "2.04": //One+6
                //    this.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
                //    break;
                //case "2.00": //Honor 7X & 9X
                //    this.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
                //    break;
        }*/
        //if (double.Parse(ratio) > 2) {
        //	this.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 1f;
        //	ratioMobiles = true;
        //   if (Application.loadedLevelName == "Texas")
        //       megaJackpot.GetComponent<RectTransform>().anchoredPosition = new Vector2(151f, 51.3f);
        //}
    }
}
