using UnityEngine;

namespace Backgammon
{
    public class ObserveAnalysisManager : MonoBehaviour
    {
        [Header("MARKERS")]
        [SerializeField] Transform _pointMarkerPrefab = null;
        [SerializeField] Transform _pointConnectorPrefab = null;

        private Transform[] _pointMarkersArray = null;
        private Transform[] _pointConnectorsArray = null;

        private void Awake()
        {
            _pointMarkersArray = new Transform[8];
            _pointConnectorsArray = new Transform[4];

            for (int m = 0; m < _pointMarkersArray.Length; m++)
            {
                var marker = Instantiate(_pointMarkerPrefab, this.transform);
                _pointMarkersArray[m] = marker;
            }

            for (int c = 0; c < _pointConnectorsArray.Length; c++)
            {
                var connector = Instantiate(_pointConnectorPrefab, this.transform);
                _pointConnectorsArray[c] = connector;
            }
        }

        internal void Init()
        {
            Reset();
        }

        internal void SetMarkersToPoints(GameStateContext.MoveInfo[] moves)
        {
            var markerIndex = 0;
            var counterIndex = 0;
            var countersMovedArray = new int[8];

            foreach(GameStateContext.MoveInfo move in moves)
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
                var positionFrom = pointFrom == 25 ? Game.Context.BarManager.PlayerBar.GetActiveCounterPosition(multipleFrom) :
                    Game.Context.PointsManager.GetPlayerPointByID(pointFrom).GetActiveCounterPosition(multipleFrom);

                markerFrom.transform.position = positionFrom;

                var positionTo = pointTo == 0 ? Game.Context.HomeManager.PlayerHome.GetActiveCounterPosition(multipleTo) :
                    Game.Context.PointsManager.GetPlayerPointByID(pointTo).GetActiveCounterPosition(multipleTo);

                markerTo.transform.position = positionTo;

                markerFrom.gameObject.SetActive(true);
                markerTo.gameObject.SetActive(true);

                // CLAUCLATE MID AND END POINTS
                var midPoint = MidPoint(positionFrom, positionTo);
                var length = Length(positionFrom, positionTo);
                var direction = positionFrom - positionTo;

                var connector = _pointConnectorsArray[counterIndex++];
                connector.rotation = Quaternion.LookRotation(direction, Vector3.up);
                connector.localScale = new Vector3(connector.localScale.x, connector.localScale.y, length * .5f);
                connector.position = new Vector3(midPoint.x, markerFrom.position.y, midPoint.z);

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

        internal void Reset()
        {
            foreach (Transform marker in _pointMarkersArray)
                marker.gameObject.SetActive(false);

            foreach (Transform connector in _pointConnectorsArray)
                connector.gameObject.SetActive(false);
        }
    }
}