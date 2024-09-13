using UnityEngine;
using UnityEngine.UI;

public class AnalysisOrContinueUI : MonoBehaviour
{
    [Header("TRANSFORMS")]
    [SerializeField] Transform _playerDiceTransform;
    [SerializeField] Transform _markersContainerTransform;
    [SerializeField] Transform _buttonContainerTransform;
    [SerializeField] Transform _analysisButtonContainerTransform;

    [Header("BUTTONS")]
    [SerializeField] Button _analysisButton;
    [SerializeField] Button _continueButton;

    [Header("MARKERS")]
    [SerializeField] Analysis2DPositionLine _pointPositionLinePrefab = null;
    private Analysis2DPositionLine[] _pointLinesArrays = null;

    [Header("RANK1 INDICATORS")]
    [SerializeField] Transform _playerRank1ContainerTransform = null;
    [SerializeField] Transform _playerRank1Transform = null;
    [SerializeField] Transform _playerProRank1ContainerTransform = null;
    [SerializeField] Transform _playerProRank1Transform = null;
    [SerializeField] Transform _opponentRank1ContainerTransform = null;
    [SerializeField] Transform _opponentRank1Transform = null;

    private void Awake()
    {
        _analysisButton.onClick.AddListener(() => OnAnalysisButtonClick());
        _continueButton.onClick.AddListener(() => OnContinueButtonClick());
    }

    private void OnDestroy()
    {
        _analysisButton.onClick.RemoveAllListeners();
        _continueButton.onClick.RemoveAllListeners();
    }

    internal void Init()
    {
        _buttonContainerTransform.localPosition = _playerDiceTransform.localPosition + new Vector3(0f, -80f, 0f);

        foreach(Transform line in _markersContainerTransform)
            if (line is not null)
                Destroy(line.gameObject);

        _pointLinesArrays = new Analysis2DPositionLine[3];

        for (var l = 0; l < _pointLinesArrays.Length; l++)
        {
            var line = Instantiate(_pointPositionLinePrefab, _markersContainerTransform);
            line.ConfigureLine();
            _pointLinesArrays[l] = line;
        }
    }

    internal void SetActive(bool active)
    {
        this.gameObject.SetActive(active);

        if (active) Reset();
        else ResetLines();
    }

    // MARKERS

    internal void SetMarkersToIndicateRank1(bool playerOrPro, bool opponent = false)
    {
        Vector3 pointFromTravel = _buttonContainerTransform.localPosition +
                                    _analysisButtonContainerTransform.localPosition;
        Vector3 pointToTravel;

        if (!opponent && playerOrPro)
        {
            pointToTravel = _playerRank1ContainerTransform.localPosition +
                                _playerRank1Transform.localPosition + _offset;

            _pointLinesArrays[0].SetLineBetweenPoints(pointFromTravel, pointToTravel,
                                                      Length(pointToTravel, pointFromTravel),
                                                      Direction(pointToTravel, pointFromTravel),
                                                      true);
            _pointLinesArrays[0].AnimateLineBetweenPoints();
        }
        else if (!opponent && !playerOrPro)
        {
            pointToTravel = _playerProRank1ContainerTransform.localPosition +
                                _playerProRank1Transform.localPosition + _offset;

            _pointLinesArrays[1].SetLineBetweenPoints(pointFromTravel, pointToTravel,
                                                      Length(pointToTravel, pointFromTravel),
                                                      Direction(pointToTravel, pointFromTravel),
                                                      true);
            _pointLinesArrays[1].AnimateLineBetweenPoints();
        }
        else
        {
            pointToTravel = _opponentRank1ContainerTransform.localPosition +
                                _opponentRank1Transform.localPosition + _offset;

            _pointLinesArrays[2].SetLineBetweenPoints(pointFromTravel, pointToTravel,
                                                      Length(pointToTravel, pointFromTravel),
                                                      Direction(pointToTravel, pointFromTravel),
                                                      true);
            _pointLinesArrays[2].AnimateLineBetweenPoints();
        }
    }

    private float Length(Vector3 start, Vector3 end)
    {
        return Vector3.Distance(start, end);
    }

    private Vector3 Direction(Vector3 start, Vector3 end)
    {
        return (end - start);
    }

    // BUTTON CLICKS

    private void OnAnalysisButtonClick() 
    {
        _ifClicked = true;
        _ifAnalysis = true;
    }

    private void OnContinueButtonClick() 
    {
        _ifClicked = true;
        _ifContinue = true;
    }

    private void Reset()
    {
        _ifClicked = false;
        _ifContinue = false;
        _ifAnalysis = false;
    }

    internal void ResetLines()
    {
        foreach (var line in _pointLinesArrays)
            line.Reset();
    }

    private bool _ifClicked = false;
    private bool _ifAnalysis = false;
    private bool _ifContinue = false;

    private Vector3 _offset = new Vector3(0f, 75f, 0f);

    internal bool IfClicked { get => _ifClicked; }
    internal bool IfAnalysis { get => _ifAnalysis; }
    internal bool IfContinue { get => _ifContinue; }
}
