using Backgammon;
using UnityEngine;
using UnityEngine.UI;

public class ConfigureBoardManualUI : MonoBehaviour
{
    [Header("BUTTONS")]
    [SerializeField] Button _blackCountersButton = null;
    [SerializeField] Button _whiteCountersButton = null;
    [SerializeField] Button _toBarButton = null;
    [SerializeField] Button _undoButton = null;
    [SerializeField] Button _resetButton = null;
    [SerializeField] Button _exitButton = null;
    [SerializeField] Button _doneButton = null;
    [SerializeField] Button _selectGoesFirstRandomButton = null;
    [SerializeField] Button _selectGoesFirstPlayerButton = null;
    [SerializeField] Button _selectGoesFirstBGBlitzButton = null;

    [Header("TRANSFORMS")]
    [SerializeField] Transform _counterSelectTransform = null;
    [SerializeField] Transform _placementSelectTransform = null;
    [SerializeField] Transform _resetSelectTransform = null;
    [SerializeField] Transform _doneSelectTransform = null;

    private ButtonHighlight _setGoesFirstRandomHighlight;
    private ButtonHighlight _setGoesFirstPlayerHighlight;
    private ButtonHighlight _setGoesFirstBGBlitzHighlight;

    protected void OnEnable()
    {
        _ifClickedUndo = false;
        _ifClickedDone = false;

        SetUndoInteractive(false);

        _blackCountersButton.onClick.AddListener(() => OnSelectBlackCounters());
        _whiteCountersButton.onClick.AddListener(() => OnSelectWhiteCounters());
        _toBarButton.onClick.AddListener(() => OnClickToBar());
        _undoButton.onClick.AddListener(() => OnClickUndo());
        _resetButton.onClick.AddListener(() => OnClickReset());
        _exitButton.onClick.AddListener(() => OnClickExit());
        _doneButton.onClick.AddListener(() => OnClickDone());

        _selectGoesFirstRandomButton.onClick.AddListener(() => OnClickGoesFirstRandom());
        _selectGoesFirstPlayerButton.onClick.AddListener(() => OnClickGoesFirstPlayer());
        _selectGoesFirstBGBlitzButton.onClick.AddListener(() => OnClickGoesFirstBGBlitz());

        _setGoesFirstRandomHighlight = _selectGoesFirstRandomButton.GetComponent<ButtonHighlight>();
        _setGoesFirstPlayerHighlight = _selectGoesFirstPlayerButton.GetComponent<ButtonHighlight>();
        _setGoesFirstBGBlitzHighlight = _selectGoesFirstBGBlitzButton.GetComponent<ButtonHighlight>();
        
        OnClickGoesFirstRandom();

        SetBothCountersDeselected();

        if (Game2D.Context.IfPlayerIsBlack) OnSelectBlackCounters();
        else OnSelectWhiteCounters();
    }

    protected void OnDisable()
    {
        _blackCountersButton.onClick.RemoveAllListeners();
        _whiteCountersButton.onClick.RemoveAllListeners();
        _toBarButton.onClick.RemoveAllListeners();
        _undoButton.onClick.RemoveAllListeners();
        _resetButton.onClick.RemoveAllListeners();
        _exitButton.onClick.RemoveAllListeners();
        _doneButton.onClick.RemoveAllListeners();

        _selectGoesFirstRandomButton.onClick.RemoveAllListeners();
        _selectGoesFirstPlayerButton.onClick.RemoveAllListeners();
        _selectGoesFirstBGBlitzButton.onClick.RemoveAllListeners();
    }

    internal void SetActive(bool active)
    {
        this.gameObject.SetActive(active);
    }

    internal void SetToBarInteractiveActive(bool active)
    {
        _toBarButton.interactable = active;
    }

    internal void SetUndoInteractive(bool active)
    {
        _undoButton.interactable = active;
    }

    // BUTTON VISUALS
    private void SetBothCountersDeselected()
    {
        _blackCountersButton.GetComponentInParent<Transform>().localScale = new Vector3(.8f, .8f, 1);
        _whiteCountersButton.GetComponentInParent<Transform>().localScale = new Vector3(.8f, .8f, 1);
    }

    internal void SwapCounterToPlaceType()
    {
        SetBothCountersDeselected();
        _placingPlayerCounters = !_placingPlayerCounters;

        if (_placingPlayerCounters) OnSelectBlackCounters();
        else OnSelectWhiteCounters();
    }

    internal void SetReadyToCommence(bool active)
    {
        SetBothCountersDeselected();
        SetToBarInteractiveActive(!active);
    }

    // BUTTONS CLICKS
    internal void OnSelectBlackCounters()
    {
        SetBothCountersDeselected();
        _blackCountersButton.GetComponentInParent<Transform>().localScale = Vector3.one;
        _placingPlayerCounters = true;
    }

    internal void OnSelectWhiteCounters()
    {
        SetBothCountersDeselected();
        _whiteCountersButton.GetComponentInParent<Transform>().localScale = Vector3.one;
        _placingPlayerCounters = false;
    }

    private void OnClickToBar()
    {
        _ifClickedToBar = true;
    }    

    private void OnClickUndo()
    {
        if (IfLastPlacedPlayerCounter)
        {
            if (Game2D.Context.IfPlayerIsBlack) OnSelectBlackCounters();
            else OnSelectWhiteCounters();
        }
        else
        {
            if (Game2D.Context.IfPlayerIsBlack) OnSelectWhiteCounters(); 
            else OnSelectBlackCounters();
        }

        _ifClickedUndo = true;
    }

    private void OnClickReset()
    {
        _ifClickedReset = true;
    }

    private void OnClickExit()
    {
        _ifClickedExit = true;
    }

    private void OnClickDone()
    {
        _ifClickedDone = true;
    }

    private void OnClickGoesFirstRandom() 
    {
        SetAllButtonHighlightsOff();
        _setGoesFirstRandomHighlight.SetColourOrDefault(Color.cyan);

        Game2D.SetStartingPlayerToRandom();
    }

    private void OnClickGoesFirstPlayer() 
    {
        SetAllButtonHighlightsOff();
        _setGoesFirstPlayerHighlight.SetColourOrDefault(Color.cyan);

        Game2D.SetStartingPlayerWhoGoesFirst(true);
    }

    private void OnClickGoesFirstBGBlitz()
    {
        SetAllButtonHighlightsOff();
        _setGoesFirstBGBlitzHighlight.SetColourOrDefault(Color.cyan);

        Game2D.SetStartingPlayerWhoGoesFirst(false);
    }

    private void SetAllButtonHighlightsOff()
    {
        _setGoesFirstRandomHighlight.SetColourOrDefault();
        _setGoesFirstPlayerHighlight.SetColourOrDefault();
        _setGoesFirstBGBlitzHighlight.SetColourOrDefault();
    }    

    private bool _placingPlayerCounters = true;
    private bool _lastPlacedPlayerCounters = true;
    private bool _ifClickedToBar = false;
    private bool _ifClickedUndo = false;
    private bool _ifClickedReset = false;
    private bool _ifClickedExit = false;
    private bool _ifClickedDone = false;

    internal bool PlacingPlayerCounters { get => _placingPlayerCounters; }
    internal bool IfLastPlacedPlayerCounter { get => _lastPlacedPlayerCounters; set => _lastPlacedPlayerCounters = value; }
    internal bool IfClickedToBar { get => _ifClickedToBar; set => _ifClickedToBar = value; }
    internal bool IfClickedUndo { get => _ifClickedUndo; set => _ifClickedUndo = value; }
    internal bool IfClickedReset { get => _ifClickedReset; set => _ifClickedReset = value; }
    internal bool IfClickedExit { get => _ifClickedExit; set => _ifClickedExit = value; }
    internal bool IfClickedDone { get => _ifClickedDone; }
}
