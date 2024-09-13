using System;
using UnityEngine;

namespace Backgammon
{
    public static class PointFlow
    {
        public enum Point24Position
        {
            UpperLeft = 0x0,
            UpperRight = 0x1,
            LowerLeft = 0x2,
            LowerRight = 0x3
        }

        public static int GetPoint(int index, Point24Position player)
        {
            if (index < 0 || index > 23)
                return (-1); // invalid
            int point = -1;
            switch (player)
            {
                case Point24Position.LowerLeft:
                    {
                        point = index + 1;
                    }
                    break;
                case Point24Position.UpperLeft:
                    {
                        point = 24 - index;
                    }
                    break;
                case Point24Position.UpperRight:
                    {
                        int x = index % 12;
                        int y = index / 12;
                        point = (y == 0 ? 13 + x : 1 + x);
                    }
                    break;
                case Point24Position.LowerRight:
                    {
                        int x = index % 12;
                        int y = index / 12;
                        point = (y == 1 ? 24 - x : 12 - x);
                    }
                    break;
            }
            return (point);
        }

        public static int GetOpponentPoint(int index, Point24Position player)
        {
            if (index < 0 || index > 23)
                return (-1); // invalid
            return (GetPoint(23 - index, player)); // note - opponents points flow opposite to players
        }

        public static int GetPointIndex(int point, Point24Position player)
        {
            if (point < 1 || point > 24)
                return (-1); // invalid
            int index = point - 1; // LowerLeft (by default)
            switch (player)
            {
                case Point24Position.UpperLeft:
                    {
                        index = 23 - index;
                    }
                    break;
                case Point24Position.UpperRight:
                    {
                        int x = index % 12;
                        int y = index / 12;
                        index = (y == 0 ? 12 + x : x);
                    }
                    break;
                case Point24Position.LowerRight:
                    {
                        int x = index % 12;
                        int y = index / 12;
                        index = (y == 1 ? 23 - x : 11 - x);
                    }
                    break;
            }
            return (index);
        }

        public static int GetOpponentPointIndex(int point, Point24Position player)
        {
            if (point < 1 || point > 24)
                return (-1); // invalid
            return (GetPointIndex(25 - point, player)); // note - opponents points flow opposite to players
        }
    }

    [Serializable]
    public enum PlayerId
    {
        None,
        Player1,
        Player2
    }

    public enum PlayerColor
    {
        Black,
        White
    }

    public struct PlayerSetup
    {
        public PlayerId id
        {
            get;
            set;
        }

        public PlayerId opponentId
        {
            get { return (id == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1); }
        }

        public PlayerColor color
        {
            get;
            set;
        }

        public PlayerColor opponentColor
        {
            get { return (color == PlayerColor.Black ? PlayerColor.White : PlayerColor.Black); }
        }

        public PointFlow.Point24Position point24Position
        {
            get;
            set;
        }

        public int GetPlayer1PointIndex(int point)
        {
            return (id == PlayerId.Player1 ? PointFlow.GetPointIndex(point, point24Position) : PointFlow.GetOpponentPointIndex(point, point24Position));
        }

        public int GetPlayer2PointIndex(int point)
        {
            return (id == PlayerId.Player2 ? PointFlow.GetPointIndex(point, point24Position) : PointFlow.GetOpponentPointIndex(point, point24Position));
        }
    }

    [Serializable]
    public struct PointState
    {
        public int counters;
        public PlayerId occupier;

        public void PushCounter(PlayerId playerId)
        {
            if (occupier != PlayerId.None && occupier != playerId)
            {
                Debug.Log($"RETURN - {playerId} -> {occupier}");
                return; // as point occupied by opponent
            }

            ++counters;
            occupier = playerId;
        }

        public void PopCounter(PlayerId playerId)
        {
            if (counters == 0)
                return; // as point unoccupied
            if (occupier != PlayerId.None && occupier != playerId)
                return; // as point occupied by opponent
            --counters;
            if (counters == 0)
                occupier = PlayerId.None; // as point now unoccupied
        }
    }
}