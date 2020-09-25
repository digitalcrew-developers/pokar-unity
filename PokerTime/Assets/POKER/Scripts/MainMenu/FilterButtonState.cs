using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FilterState
{
    Ascending, //Down
    Descending, //Up
    None //None
}

public enum ClubMemberFilters
{
    Fee,
    SpinUpBuyIn,
    Winnings,
    Hand,
    LastLogin,
    LastPlayed
}

public class FilterButtonState : MonoBehaviour
{
    private FilterState state;
    public Image Up, Down;
    public string FilterName;

    private string hexDisableColorString = "#646464";
    private string hexEnableColorString = "#1dffec";

    private Color EnableColor, DisableColor;

    public delegate void FilterButtonStateUpdate(FilterState stateType, string stateName);
    public event FilterButtonStateUpdate OnStateChange;


    private void Start()
    {
        ColorUtility.TryParseHtmlString(hexEnableColorString, out EnableColor);
        ColorUtility.TryParseHtmlString(hexDisableColorString, out DisableColor);
        UpdateState(FilterState.None);
    }

    public void SwitchState()
    {
        if(state == FilterState.Ascending)
        {
            UpdateState(FilterState.Descending);
        }
        else if(state == FilterState.Descending)
        {
            UpdateState(FilterState.Ascending);
        }
        else
        {
            UpdateState(FilterState.Ascending);
        }
        OnStateChange?.Invoke(state, FilterName);
    }

    public void UpdateState(FilterState _state)
    {
        state = _state;
        switch (state)
        {
            case FilterState.Ascending:
                Up.color = EnableColor;
                Down.color = DisableColor;
                break;
            case FilterState.Descending:
                Up.color = DisableColor;
                Down.color = EnableColor;
                break;
            case FilterState.None:
                Up.color = DisableColor;
                Down.color = DisableColor;
                break;
            default:
                break;
        }
    }

    public FilterState GetState()
    {
        return state;
    }

    public string GetStateName()
    {
        return FilterName;
    }
}
