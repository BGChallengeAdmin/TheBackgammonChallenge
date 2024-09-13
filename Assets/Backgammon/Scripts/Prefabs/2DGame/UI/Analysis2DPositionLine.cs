using System;
using System.Collections;
using UnityEngine;

public class Analysis2DPositionLine : MonoBehaviour
{
    [SerializeField] Transform _pointPositionPrefab;
    [SerializeField] Transform _circlePrefab;
    [SerializeField] Transform _trianglePrefab;

    private Transform[] _pointPositionsArray = null;
    private Transform _endLineCircleMarker = null;
    private Transform _endLineTriangleMarker = null;

    internal void SetLineBetweenPoints(Vector3 from, Vector3 to, float distance, Vector3 direction, bool shouldBeWhite = false)
    {
        NO_OF_STEPS = Convert.ToInt32(distance * .2f);

        _direction = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f);

        ConfigureLine(shouldBeWhite);

        this.from = from;
        dX = (to.x - from.x) * STEP_DELTA;
        dY = (to.y - from.y) * STEP_DELTA;

        _valuesSet = true;
    }

    internal void ConfigureLine(bool shouldBeWhite = false)
    {
        // LINE
        if (_pointPositionsArray is not null)
            foreach (Transform p in _pointPositionsArray)
                Destroy(p.gameObject);

        _pointPositionsArray = new Transform[NO_OF_STEPS];

        for (int p = 0; p < NO_OF_STEPS; p++)
        {
            var point = Instantiate(_pointPositionPrefab, this.transform);
            point.gameObject.SetActive(false);
            _pointPositionsArray[p] = point;
        }

        STEP_DELTA = 1f / NO_OF_STEPS;

        // CIRCLE MARKER
        if (_endLineCircleMarker is null)
            _endLineCircleMarker = Instantiate(_circlePrefab, this.transform);

        _endLineCircleMarker.gameObject.SetActive(false);

        _endLineCircleMarker.GetComponent<RectTransform>().sizeDelta = new Vector2(40f, 40f);

        // TRIANGLE MARKER
        if (_endLineTriangleMarker is null)
            _endLineTriangleMarker = Instantiate(_trianglePrefab, this.transform);

        _endLineTriangleMarker.gameObject.SetActive(false);

        _endLineTriangleMarker.GetComponent<RectTransform>().sizeDelta = new Vector2(40f, 50f);
        _endLineTriangleMarker.transform.rotation = _direction;

        // WHITE
        if (shouldBeWhite) SetLineToWhite();
    }

    internal void SetLineToWhite()
    {
        foreach (Transform p in _pointPositionsArray)
            p.GetComponent<SetToWhitePrefab>().SetToWhite();

        _endLineCircleMarker.GetComponent<SetToWhitePrefab>().SetToWhite();

        _endLineTriangleMarker.GetComponent<SetToWhitePrefab>().SetToWhite();
    }

    internal void DrawLineBetweenPoints()
    {
        _animating = true;

        _endLineCircleMarker.transform.localPosition = new Vector3(from.x, from.y, 0);
        _endLineCircleMarker.gameObject.SetActive(true);

        for (var p = 0; p < NO_OF_STEPS - NO_OF_STEPS_DRAW_OFFSET; p++)
        {
            var point = _pointPositionsArray[p];
            point.transform.localPosition = new Vector3((from.x + dX * p), (from.y + dY * p), 0);
            point.gameObject.SetActive(true);
        }

        _endLineTriangleMarker.transform.position = _pointPositionsArray[(NO_OF_STEPS - 7)].transform.position;
        _endLineTriangleMarker.gameObject.SetActive(true);

        _animating = false;
    }

    internal void AnimateLineBetweenPoints()
    {
        StartCoroutine(DrawLineCoroutine());
    }

    private IEnumerator DrawLineCoroutine()
    {
        var p = 0;

        _animating = true;

        _endLineCircleMarker.transform.localPosition = new Vector3(from.x, from.y, 0);
        _endLineCircleMarker.gameObject.SetActive(true);

        _endLineTriangleMarker.transform.localPosition = new Vector3(from.x, from.y, 0);
        _endLineTriangleMarker.gameObject.SetActive(true);

        while (p < NO_OF_STEPS - NO_OF_STEPS_DRAW_OFFSET)
        {
            var point = _pointPositionsArray[p];
            point.transform.localPosition = new Vector3((from.x + dX * p), (from.y + dY * p), 0);
            point.gameObject.SetActive(true);

            if (p < NO_OF_STEPS - 7)
                _endLineTriangleMarker.transform.position = point.transform.position;

            p++;

            yield return null;
        }

        _animating = false;
    }

    internal void Reset()
    {
        _animating = false;
        _valuesSet = false;

        foreach (var p in _pointPositionsArray)
            p.gameObject.SetActive(false);

        _endLineCircleMarker.gameObject.SetActive(false);

        _endLineTriangleMarker.gameObject.SetActive(false);
    }

    private int NO_OF_STEPS = 1;
    private int NO_OF_STEPS_DRAW_OFFSET = 2;
    private float STEP_DELTA = 1;

    private Vector3 from;
    private float dX = 0;
    private float dY = 0;

    private Quaternion _direction = Quaternion.identity;

    private float _animationTime = .025f;
    private bool _animating = false;
    private bool _valuesSet = false;

    internal bool Animating { get => _animating; }
    internal bool ValuesSet { get => _valuesSet; }
}
