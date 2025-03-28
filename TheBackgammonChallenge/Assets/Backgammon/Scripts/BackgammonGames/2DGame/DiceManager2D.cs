using UnityEngine;

namespace Backgammon
{
    public class DiceManager2D : MonoBehaviour
    {
        internal void Init(float scaledCounterSize)
        {
            //Game2D.Context.DiceRollsUI.SetDiceUIScale(scaledCounterSize);
            //Game2D.Context.DiceRollsUI.SetAsBlackOrWhite(Game2D.Context.IfPlayerIsBlack, scaledCounterSize);
            //Game2D.Context.DiceRollsUI.ResetPlayerDiceRollsText();

            Game2DStrategy.Context.DiceRollsUI.SetDiceUIScale(scaledCounterSize);
            Game2DStrategy.Context.DiceRollsUI.SetAsBlackOrWhite(Game2D.Context.IfPlayerIsBlack, scaledCounterSize);
            Game2DStrategy.Context.DiceRollsUI.ResetPlayerDiceRollsText();

            _dice = new int[4];
        }

        internal void SetDiceValues(int dice1, int dice2)
        {
            _dice1 = dice1;
            _dice2 = dice2;

            if (dice1 == dice2)
            {
                for (int d = 0; d < 4; d++) _dice[d] = dice1;
                _diceAvailable = 4;
                _doubleWasRolled = true;
            }
            else
            {
                _dice[0] = 0;
                _dice[1] = dice1;
                _dice[2] = dice2;
                _dice[3] = 0;

                _diceAvailable = 2;
                _doubleWasRolled = false;

                _IfDiceWereAdded = false;
                _IfDiceWereAddedInDouble = false;
                _numberOfDoublesUsed = 0;
            }
        }

        // ------------------------------------------- HELPER METHODS -------------------------------------------

        internal int TestWhichDicePlayed(int pointFrom, int pointTo)
        {
            var number = 1;
            var step = pointFrom - pointTo;

            if (_doubleWasRolled)
            {
                var index = 0;
                var diceUsed = step / Dice1;

                // BEARING OFF - STEP < DICE VALUE
                if ((step % Dice1) > 0)
                {
                    diceUsed += 1; 
                }

                // FIND THE INDEX OF THE FIRST NON '0'
                for (int i = 0; i < 4; i++)
                {
                    if (_dice[i] > 0)
                    {
                        index = i;
                        break;
                    }
                }

                // REMOVE THE NUMBER OF DICE USED
                for (int d = index; d < (index + diceUsed); d++) { _dice[d] = 0; }

                number = diceUsed;
                _diceAvailable -= diceUsed;
                _IfDiceWereAddedInDouble = _IfDiceWereAddedInDouble ? true : (diceUsed > 1 ? true : false);

                _numberOfDoublesUsed = number > _numberOfDoublesUsed ? number : _numberOfDoublesUsed;
            }
            else
            {
                // NORMAL PLAY
                if (step == Dice1)
                {
                    _dice[1] = 0;
                    _diceAvailable -= 1;
                }
                else if (step == Dice2)
                {
                    _dice[2] = 0;
                    _diceAvailable -= 1;
                }
                else if (step == (Dice1 + Dice2))
                {
                    _dice[1] = 0;
                    _dice[2] = 0;
                    _diceAvailable -= 2;
                    _IfDiceWereAdded = true;

                    number = 2;
                }
                // BEARING OFF
                else if (step < Dice1 && step > Dice2)
                {
                    _dice[1] = 0;
                    _diceAvailable -= 1;
                }
                else if (step < Dice2 && step > Dice1)
                {
                    _dice[2] = 0;
                    _diceAvailable -= 1;
                }
                else if (step > Dice1 && step > Dice2)
                {
                    _dice[1] = 0;
                    _dice[2] = 0;
                    _diceAvailable -= 2;
                    _IfDiceWereAdded = true;

                    number = 2;
                }
                else if (step < Dice1 && step < Dice2)
                {
                    if (Dice1 < Dice2)
                    {
                        _dice[1] = 0;
                        _diceAvailable -= 1;
                    }
                    else
                    {
                        _dice[2] = 0;
                        _diceAvailable -= 1;
                    }
                }
            }

            return number;
        }

        // ----------------------------------------- GETTERS && SETTERS -----------------------------------------

        private int[] _dice;
        private int _diceAvailable = 0;
        private int _dice1 = 0;
        private int _dice2 = 0;
        private bool _doubleWasRolled = false;
        private bool _IfDiceWereAdded = false;
        private bool _IfDiceWereAddedInDouble = false;
        private int _numberOfDoublesUsed = 0;

        public int DiceAvailable { get => _diceAvailable; }
        public int Dice1 { get => _dice1; }
        public int Dice2 { get => _dice2; }
        public bool Dice1Played { get => (_dice[1] == 0); }
        public bool Dice2Played { get => (_dice[2] == 0); }
        public bool DoubleWasRolled { get => _doubleWasRolled; }
        public bool IfDiceWereAdded { get => _IfDiceWereAdded; }
        public bool IfDiceWereAddedInDouble { get => _IfDiceWereAddedInDouble; }
        public int NumberOfDoublesUsed { get => _numberOfDoublesUsed; }
    }
}