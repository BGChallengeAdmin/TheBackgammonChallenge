using System.Collections;
using UnityEngine;

namespace Backgammon
{
    public class ObserveAnalysisManager2D : MonoBehaviour
    {
        [Header("MARKERS")]
        [SerializeField] Transform _pointMarkerPrefab = null;
        [SerializeField] Transform _pointConnectorPrefab = null;
        [SerializeField] Analysis2DPositionLine _pointPositionPrefab = null;

        private Transform[] _pointMarkersArray = null;
        private Transform[] _pointConnectorsArray = null;
        private Analysis2DPositionLine[] _pointLinesArrays = null;

        private void Awake()
        {
            _pointMarkersArray = new Transform[8];
            _pointConnectorsArray = new Transform[4];
            _pointLinesArrays = new Analysis2DPositionLine[4];

            for (int p = 0; p < 4; p++)
            {
                var line = Instantiate(_pointPositionPrefab, this.transform);
                line.ConfigureLine();
                _pointLinesArrays[p] = line;
            }
        }

        // POINT POSITIONS

        internal void SetPointPositionsToPoints(GameStateContext2D.MoveInfo[] moves, bool showingOpponent = false, bool replayingPro = false)
        {
            var lineIndex = 0;
            var markerIndex = 0;
            var countersMovedArray = new int[8];
            
            foreach (GameStateContext2D.MoveInfo move in moves)
            {
                if (move.pointFrom == 0) break;

                var line = _pointLinesArrays[lineIndex++];

                var pointFrom = move.pointFrom;
                var pointTo = move.pointTo;

                countersMovedArray[markerIndex++] = pointFrom;
                countersMovedArray[markerIndex++] = pointTo;

                // TEST FOR PREVIOUS POINTS TO ADJUST MARKER POSITION
                var multipleFrom = 0;
                var multipleTo = replayingPro ? -1 : 0;

                for (int i = 0; i < countersMovedArray.Length; i += 2)
                {
                    if (countersMovedArray[i] == pointFrom) multipleFrom++;
                    if (countersMovedArray[i + 1] == pointTo)
                    {
                        if (replayingPro && multipleTo == 0) multipleTo++;
                        multipleTo++;
                    }
                }

                // CONFIGURE LINE
                var positionFrom = pointFrom == 25 ? Game2D.Context.BarManager.PlayerBar.GetActiveCounterPosition(multipleFrom) :
                    Game2D.Context.PointsManager.GetAdjustedPointPositionMultiple(
                        Game2D.Context.PointsManager.GetPlayerPointByID(pointFrom), multipleFrom);

                var positionTo = pointTo == 0 ? Game2D.Context.HomeManager.PlayerHome.GetActiveCounterPosition(multipleTo) :
                    Game2D.Context.PointsManager.GetAdjustedPointPositionMultiple(
                        Game2D.Context.PointsManager.GetPlayerPointByID(pointTo), multipleTo);

                if (showingOpponent)
                {
                    positionFrom = pointFrom == 25 ? Game2D.Context.BarManager.OpponentBar.GetActiveCounterPosition(multipleFrom) :
                    Game2D.Context.PointsManager.GetAdjustedPointPositionMultiple(
                        Game2D.Context.PointsManager.GetOpponentPointByID(pointFrom), multipleFrom);

                    positionTo = pointTo == 0 ? Game2D.Context.HomeManager.OpponentHome.GetActiveCounterPosition(multipleTo) :
                        Game2D.Context.PointsManager.GetAdjustedPointPositionMultiple(
                            Game2D.Context.PointsManager.GetOpponentPointByID(pointTo), multipleTo);
                }

                line.SetLineBetweenPoints(positionFrom, positionTo, Length(positionFrom, positionTo), Direction(positionTo, positionFrom));
            }

            StartCoroutine(AnimatePointPositionLines());
        }

        private IEnumerator AnimatePointPositionLines()
        {
            foreach (var line in _pointLinesArrays)
            {
                if (!line.ValuesSet) break;

                line.DrawLineBetweenPoints();

                while (line.Animating) yield return null;

                yield return new WaitForSeconds(.75f);
            }
        }

        // MARKERS AND CONNECTORS

        internal void Init(float scaledCounterSize)
        {
            _scaledCounterSize = .75f * scaledCounterSize;

            // CLEAR UP ANY PREVIOUS MARKERS
            foreach (Transform c in _pointConnectorsArray) if (c is not null) Destroy(c.gameObject);
            foreach (Transform m in _pointMarkersArray) if (m is not null) Destroy(m.gameObject);
            
            // CREATE CONNECTORS FIRST - MARKERS WILL BE OVERLAYED IN DRAW ORDER
            for (int c = 0; c < _pointConnectorsArray.Length; c++)
            {
                var connector = Instantiate(_pointConnectorPrefab, this.transform);
                _pointConnectorsArray[c] = connector;
            }

            for (int m = 0; m < _pointMarkersArray.Length; m++)
            {
                var marker = Instantiate(_pointMarkerPrefab, this.transform);
                marker.GetComponent<RectTransform>().sizeDelta = new Vector2(_scaledCounterSize, _scaledCounterSize);
                _pointMarkersArray[m] = marker;
            }

            Reset();
        }

        internal void SetMarkersToPoints(GameStateContext2D.MoveInfo[] moves)
        {
            var markerIndex = 0;
            var connectorIndex = 0;
            var countersMovedArray = new int[8];

            foreach (GameStateContext2D.MoveInfo move in moves)
            {
                if (move.pointFrom == 0) break;

                var pointFrom = move.pointFrom;
                var pointTo = move.pointTo;

                countersMovedArray[markerIndex] = pointFrom;
                var markerFrom = _pointMarkersArray[markerIndex++];

                countersMovedArray[markerIndex] = pointTo;
                var markerTo = _pointMarkersArray[markerIndex++];

                // TEST FOR PREVIOUS POINTS TO ADJUST MARKER POSITION
                var multipleFrom = 0;
                var multipleTo = 0;

                for (int i = 0; i < countersMovedArray.Length; i += 2)
                {
                    if (countersMovedArray[i] == pointFrom) multipleFrom++;
                    if (countersMovedArray[i + 1] == pointTo) multipleTo++;
                }

                // ACTIVATE MARKERS
                var positionFrom = pointFrom == 25 ? Game2D.Context.BarManager.PlayerBar.GetActiveCounterPosition(multipleFrom) :
                    Game2D.Context.PointsManager.GetAdjustedPointPositionMultiple(
                        Game2D.Context.PointsManager.GetPlayerPointByID(pointFrom), multipleFrom);

                markerFrom.transform.localPosition = positionFrom;

                var positionTo = pointTo == 0 ? Game2D.Context.HomeManager.PlayerHome.GetActiveCounterPosition(multipleTo) :
                    Game2D.Context.PointsManager.GetAdjustedPointPositionMultiple(
                        Game2D.Context.PointsManager.GetPlayerPointByID(pointTo), multipleTo);

                markerTo.transform.localPosition = positionTo;

                markerFrom.gameObject.SetActive(true);
                markerTo.gameObject.SetActive(true);

                // CLAUCLATE MID AND END POINTS
                var midPoint = MidPoint(positionFrom, positionTo);
                var length = Length(positionFrom, positionTo);
                var direction = positionFrom - positionTo;

                var connector = _pointConnectorsArray[connectorIndex++];
                connector.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f);
                connector.GetComponent<RectTransform>().sizeDelta = new Vector2(_scaledCounterSize, length);
                connector.transform.localPosition = positionFrom;

                connector.gameObject.SetActive(true);
            }
        }

        private Vector3 MidPoint(Vector3 start, Vector3 end)
        {
            return Vector3.Lerp(start, end, 0.5f);
        }

        private float Length(Vector3 start, Vector3 end)
        {
            return Vector3.Distance(start, end);
        }

        private Vector3 Direction(Vector3 start, Vector3 end)
        {
            return (end - start);
        }

        internal void Reset()
        {
            foreach (Transform marker in _pointMarkersArray)
                marker.gameObject.SetActive(false);

            foreach (Transform connector in _pointConnectorsArray)
                connector.gameObject.SetActive(false);

            foreach (var line in _pointLinesArrays)
                line.Reset();
        }

        // ------------------------------------ GETTERS && SETTERS ------------------------------------

        internal bool CurrentlyActive { get => _pointMarkersArray[0].gameObject.activeInHierarchy; }

        private float _scaledCounterSize = 1f;
    }
}