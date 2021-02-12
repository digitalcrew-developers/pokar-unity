using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TopUpScriptTeenPatti : MonoBehaviour
{
    public Text sliderAmountText, balanceText, minText, maxText;
    public Slider slider;
    private float initialBalance = 0;
    public Button TopUpButton;

    private void Start()
    {
        //TopUpButton.onClick.RemoveAllListeners();
        //TopUpButton.onClick.AddListener(() => OnClickOnButton(true));
    }

    public void Init(float userInitialBalance)
    {
        initialBalance = userInitialBalance;
        slider.minValue = GlobalGameManager.instance.GetRoomData().minBuyIn;
        slider.maxValue = PlayerManager.instance.GetPlayerGameData().coins;

        minText.text = "" + (int)slider.minValue;
        maxText.text = "" + (int)slider.maxValue;
        slider.value = slider.minValue;
    }


    public void OnSlideValueChange()
    {
        sliderAmountText.text = "" + (int)slider.value;
    }

    public void OnClickOnButton(bool isTopUp)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        if (isTopUp)
        {
            int addedBalance = (int)(slider.value - initialBalance);
            SocketController.instance.SendTopUpRequest(addedBalance);
            InGameManagerTeenPatti.instance.ToggleTopUpDone(true);
        }

        InGameUiManagerTeenPatti.instance.DestroyScreen(InGameScreensTeenPatti.TopUp);
    }
}
