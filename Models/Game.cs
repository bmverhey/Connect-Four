using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Media;
using System.Collections.Generic;

namespace ConnectFour
{
    public class Game
    {
        #region Properties

        //holds the player name in a string in the position(s) they have played
        private string[,] _gameArrayPosition = new string[8, 7];
        public string[,] GameArrayPosition
        {
            get { return _gameArrayPosition; }
            set { _gameArrayPosition = value; }
        }

        //holds the color value as a string for each _game position
        private int[,] _winningArrayCoordinates = new int[4,2];
        public int[,] WinningArrayCoordinates
        {
            get { return _winningArrayCoordinates; }
            set { _winningArrayCoordinates = value;}
        }

        //holds the boolean of if a _game position is currently a valid move
        private Boolean[,] _gameArrayEnabled = new Boolean[8, 7] 
        {
            {true,false,false,false,false,false,false},
            {true,false,false,false,false,false,false},
            {true,false,false,false,false,false,false},
            {true,false,false,false,false,false,false},
            {true,false,false,false,false,false,false},
            {true,false,false,false,false,false,false},
            {true,false,false,false,false,false,false},
            {true,false,false,false,false,false,false},
        };
        public Boolean[,] GameArrayEnabled
        {
            get { return _gameArrayEnabled; }
            set { _gameArrayEnabled = value; }
        }

        //Is the current _game over
        private Boolean _isGameOver = false;
        public Boolean IsGameOver
        {
            get
            {
                _isGameOver = GameOverCheck(this.GameArrayPosition);
                if (_isGameOver == true)
                {
                    EndOfGameCleanUp();
                }
                return _isGameOver;
            }
            set { _isGameOver = value; }
        }

        //Has the _game been won
        private Boolean _isGameWon = false;
        public Boolean IsGameWon
        {
            get
            {
                GameWonCheck(this.GameArrayPosition, true);
                if (_isGameWon == true)
                {
                    IsGameOver = true;
                }
                return _isGameWon;
            }
            set { _isGameWon = value; }
        }

        //Ai Player Level
        private int _aiLevel = 3;
        public int AiLevel 
            {
                get 
                { return _aiLevel; }
                set
                { _aiLevel = value; }
            }


        #endregion //Properties

        #region Functions

        public Game GameMoveMake(string _player, int _column, int _row, Game _game)
        {
            if (_game.GameArrayPosition[_column, _row] == null)
            {
                _game.GameArrayPosition[_column, _row] = _player;
                _game.GameArrayEnabled[_column, _row] = false;
                if (_row < 6)
                {
                    _game.GameArrayEnabled[_column, (_row + 1)] = true;
                }

               
                GameWonCheck(_game.GameArrayPosition, true);
                GameOverCheck(_game.GameArrayPosition);
            }
            return _game;
        }

        public int GameLowestAvailableRowInColumn(int _column, int _row, Game _game)
        {
            int _arrayLowestAvailableRow = -1;

            //find what the lowest avaialble in that column 
            for (int r = 0; r < 7; r++)
            {
                if (_game.GameArrayEnabled[_column, r] == true)
                {
                    _arrayLowestAvailableRow = r;
                    break;
                }
            }
            //if the row available is above selected row do nothing
            if (_arrayLowestAvailableRow > _row)
            {
                _arrayLowestAvailableRow = -1;
            }

                return _arrayLowestAvailableRow;
        }

        public Game AiTurn(Game _game)
        {
            if (_isGameOver == false && _isGameWon == false)
            {
                int[] aiMove = new int[2];
                 aiMove = AiMoveGet(_game);
                int columnAi = Convert.ToInt32(aiMove[0]);
                int rowAi = Convert.ToInt32(aiMove[1]);
                _game = GameMoveMake("Ai", columnAi, rowAi, _game);
            }
            GameWonCheck(_game.GameArrayPosition, true);
            GameOverCheck(_game.GameArrayPosition);
            return _game;
        }
        private int[] AiMoveGet(Game _game)
        {

            #region aiSetup

            //hold column and row int of selected ai move
            int[] _arrayAiMoveArray = new int[2];

            //has an Ai move been selected
            Boolean _isAiMoveFoundBool = false;

            //to hold record of analysis of moves
            Boolean[] _arrayGoodMove = new Boolean[8] { true, true, true, true, true, true, true, true };
            
            //create an array to test moves that doesn't affect the original _game position array
            string[,] _arrayTestMoveArray = new string[8, 7];
            Array.Copy(_game.GameArrayPosition, _arrayTestMoveArray, 56);

            //create an array to test enabled that doesn't affect the original _game position array
            Boolean[,] _arrayTestEnabledArray = new Boolean[8, 7];
            Array.Copy(_game.GameArrayEnabled, _arrayTestEnabledArray, 56);

            #endregion //aiSetup

            #region playWinningMove

            //winning move
            if (_isAiMoveFoundBool == false && AiLevel > 1)
            {
                for (int c = 0; c < 8; c++)
                {
                    for (int r = 0; r < 7; r++)
                    {
                        Array.Copy(_game.GameArrayPosition, _arrayTestMoveArray, 56);
                        _arrayTestMoveArray[c, r] = "Ai";
                        if (GameWonCheck(_arrayTestMoveArray, false) == true && _game.GameArrayEnabled[c, r] == true)
                        {
                            _isAiMoveFoundBool = true;
                            _arrayAiMoveArray[0] = c;
                            _arrayAiMoveArray[1] = r;
                            break;

                        }
                    }
                    if (_isAiMoveFoundBool == true)
                    {
                        break;
                    }
                }
            }

            #endregion //playWinningMove

            #region blockWinningMove

            if (_isAiMoveFoundBool == false && AiLevel > 1)
            {
                for (int c = 0; c < 8; c++)
                {
                    for (int r = 0; r < 7; r++)
                    {
                        Array.Copy(_game.GameArrayPosition, _arrayTestMoveArray, 56);
                        _arrayTestMoveArray[c, r] = "Human";
                        if (GameWonCheck(_arrayTestMoveArray, false) == true && _game.GameArrayEnabled[c, r] == true)
                        {
                            _isAiMoveFoundBool = true;
                            _arrayAiMoveArray[0] = c;
                            _arrayAiMoveArray[1] = r;
                            break;

                        }
                    }
                    if (_isAiMoveFoundBool == true)
                    {
                        break;
                    }
                }
            }


            #endregion //blockWinningMove

            #region avoidHumanNextTurnSameColumnWin

            if (_isAiMoveFoundBool == false && AiLevel > 1)
            {
                for (int c = 0; c < 8; c++)
                {
                    for (int r = 0; r < 6; r++)
                    {
                        Array.Copy(_game.GameArrayPosition, _arrayTestMoveArray, 56);
                        _arrayTestMoveArray[c, r] = "Ai";
                        _arrayTestMoveArray[c, (r + 1)] = "Human";
                        if (GameWonCheck(_arrayTestMoveArray, false) == true && _game.GameArrayEnabled[c, r] == true)
                        {
                            _arrayGoodMove[c] = false; 
                        }
                    }
                }
            }

            #endregion //avoidHumanNextTurnSameColumnWin

            #region playForceWin

            //check for a single move that has 2+ immediate next move wins
            if (_isAiMoveFoundBool == false && AiLevel > 2)
            {
                for (int c = 0; c < 8; c++)
                {
                    for (int r = 0; r < 7; r++)
                    {
                        //secondMoveWinPlayCounter
                        if (_arrayGoodMove[c] == true && _gameArrayEnabled[c, r] == true)
                        {
                            int _secondMoveWinPlayCounter = 0;
                            for (int c2 = 0; c2 < 8; c2++)
                            {
                                for (int r2 = 0; r2 < 7; r2++)
                                {
                                    //reset test arrays
                                    Array.Copy(_game.GameArrayPosition, _arrayTestMoveArray, 56);
                                    Array.Copy(_game.GameArrayEnabled, _arrayTestEnabledArray, 56);
                                    //only test is first move is valid
                                    if (_arrayTestEnabledArray[c, r] == true)
                                    {
                                        //make first test move
                                        _arrayTestMoveArray[c, r] = "Ai";
                                        //disable first test move location
                                        _arrayTestEnabledArray[c, r] = false;
                                        //enable position able first test move location if able
                                        if (r < 6)
                                        {
                                            _arrayTestEnabledArray[c,(r+1)] = true;
                                        }
                                        //make second test move if able
                                        if (_arrayTestEnabledArray[c2, r2] == true)
                                        {
                                            _arrayTestMoveArray[c2, r2] = "Ai";
                                            //test moves and increment win counter if winning combo
                                            if (GameWonCheck(_arrayTestMoveArray, false) == true)
                                            {
                                                _secondMoveWinPlayCounter += 1;
                                            }
                                        }
                                    }
                                }
                            }
                            //make a move if there is more that one winning combination after the first move
                            if (_secondMoveWinPlayCounter > 1)
                            {
                                _isAiMoveFoundBool = true;
                                _arrayAiMoveArray[0] = c;
                                _arrayAiMoveArray[1] = r;
                                break;
                            }
                        }
                        //break the loop if move has been made
                        if (_isAiMoveFoundBool == true)
                        {
                            break;
                        }
                    }
                    //break the loop if move has been made
                    if (_isAiMoveFoundBool == true)
                    {
                        break;
                    }
                }
            }

            #endregion //playForceWind

            #region avoidOrBlockForceWin

            //block a force win

            if (_isAiMoveFoundBool == false && AiLevel > 2)
            {
                for (int c = 0; c < 8; c++)
                {
                    for (int r = 0; r < 7; r++)
                    {
                        //secondMoveWinPlayCounter
                        if (_arrayGoodMove[c] == true && _gameArrayEnabled[c, r] == true)
                        {
                            int _secondMoveWinPlayCounter = 0;
                            for (int c2 = 0; c2 < 8; c2++)
                            {
                                for (int r2 = 0; r2 < 7; r2++)
                                {
                                    //reset test arrays
                                    Array.Copy(_game.GameArrayPosition, _arrayTestMoveArray, 56);
                                    Array.Copy(_game.GameArrayEnabled, _arrayTestEnabledArray, 56);
                                    //only test is first move is valid
                                    if (_arrayTestEnabledArray[c, r] == true)
                                    {
                                        //make first test move
                                        _arrayTestMoveArray[c, r] = "Human";
                                        //disable first test move location
                                        _arrayTestEnabledArray[c, r] = false;
                                        //enable position above first test move location if able
                                        if (r < 6)
                                        {
                                            _arrayTestEnabledArray[c,(r+1)] = true;
                                        }
                                        //make second test move if able
                                        if (_arrayTestEnabledArray[c2, r2] == true)
                                        {
                                            _arrayTestMoveArray[c2, r2] = "Human";
                                            //test moves and increment win counter if winning combo
                                            if (GameWonCheck(_arrayTestMoveArray, false) == true)
                                            {
                                                _secondMoveWinPlayCounter += 1;
                                            }
                                        }
                                    }
                                }
                            }
                            //make a move if there is more that one winning combination after the first move
                            if (_secondMoveWinPlayCounter > 1 && _arrayGoodMove[c] == true && GameArrayEnabled[c,r] == true)
                            {
                                _isAiMoveFoundBool = true;
                                _arrayAiMoveArray[0] = c;
                                _arrayAiMoveArray[1] = r;
                                break;
                            }
                        }
                        //break the loop if move has been made
                        if (_isAiMoveFoundBool == true)
                        {
                            break;
                        }
                    }
                    //break the loop if move has been made
                    if (_isAiMoveFoundBool == true)
                    {
                        break;
                    }
                }
            }

            //avoid enabling a force win setup
            if (_isAiMoveFoundBool == false && AiLevel > 2)
            {
                for (int c = 0; c < 8; c++)
                {
                    for (int r = 0; r < 6; r++)
                    {
                        //secondMoveWinPlayCounter
                        if (_arrayGoodMove[c] == true && _gameArrayEnabled[c, r] == true)
                        {
                            int _secondMoveWinPlayCounter = 0;
                            for (int c2 = 0; c2 < 8; c2++)
                            {
                                for (int r2 = 0; r2 < 7; r2++)
                                {
                                    //reset test arrays
                                    Array.Copy(_game.GameArrayPosition, _arrayTestMoveArray, 56);
                                    Array.Copy(_game.GameArrayEnabled, _arrayTestEnabledArray, 56);
                                    //only test is first move is valid
                                    if (_arrayTestEnabledArray[c, r] == true)
                                    {
                                        //make first two test move
                                        _arrayTestMoveArray[c, r] = "Ai";
                                        _arrayTestMoveArray[c, (r +1)] = "Human";
                                        //disable first two test move location
                                        _arrayTestEnabledArray[c, r] = false;
                                        _arrayTestEnabledArray[c, (r+1)] = false;
                                        //enable position above first two test move location if able
                                        if (r < 5)
                                        {
                                            _arrayTestEnabledArray[c, (r + 2)] = true;
                                        }
                                        //make third test move if able
                                        if (_arrayTestEnabledArray[c2, r2] == true)
                                        {
                                            _arrayTestMoveArray[c2, r2] = "Human";
                                            //test moves and increment win counter if winning combo
                                            if (GameWonCheck(_arrayTestMoveArray, false) == true)
                                            {
                                                _secondMoveWinPlayCounter += 1;
                                            }
                                        }
                                    }
                                }
                            }
                            //set as not a good move if there is more that one winning combination after the first move
                            if (_secondMoveWinPlayCounter > 1 && _arrayGoodMove[c] == true && GameArrayEnabled[c, r] == true)
                            {
                                _arrayGoodMove[c] = false;
                                break;
                            }
                        }
                    }
                }
            }

            #endregion //avoidOrBlockForceWin

            #region randomMove

            //random move
            if (_isAiMoveFoundBool == false)
            {
                Random rand = new Random();
                int _intGoodMoves = 0;
                int _intPossibleMoves = 0;

                //count good moves
                for (int i = 0; i < 8; i++)
                {
                    if (_arrayGoodMove[i] == true)
                    {
                        _intGoodMoves += 1;
                    }
                }

                //count possible moves
                for (int c = 0; c < 8; c++)
                {
                    for (int r = 0; r < 7; r++)
                    {
                        if (_game.GameArrayEnabled[c,r] == true)
                        {
                            _intPossibleMoves += 1;
                        }
                    }
                }

                //find a good move
                if (_intGoodMoves > 0)
                {
                    int[,] _arrayGoodMoves = new int[_intGoodMoves,2];
                    int counter = 0;
                    for (int c = 0; c < 8; c++)
                    {
                        for (int r = 0; r < 7; r++)
                        {
                            if (_game.GameArrayEnabled[c, r] == true && _arrayGoodMove[c] == true)
                            {
                                _arrayGoodMoves[counter,0] = c;
                                _arrayGoodMoves[counter,1] = r;
                                counter += 1;
                            }
                        }
                    }
                    counter = rand.Next(0, counter);
                    _arrayAiMoveArray[0] = _arrayGoodMoves[counter,0];
                    _arrayAiMoveArray[1] = _arrayGoodMoves[counter,1];
                }
                //find a possible move
                else if (_intGoodMoves == 0 && _intPossibleMoves > 0)
                {
                    
                        int[,] _arrayPossibleMoves = new int[_intPossibleMoves, 2];
                        int counter = 0;
                        for (int c = 0; c < 8; c++)
                        {
                            for (int r = 0; r < 7; r++)
                            {
                                if (_game.GameArrayEnabled[c, r] == true)
                                {
                                    _arrayPossibleMoves[counter, 0] = c;
                                    _arrayPossibleMoves[counter, 1] = r;
                                    counter += 1;
                                }
                            }
                        }
                        counter = rand.Next(0, counter);
                        _arrayAiMoveArray[0] = _arrayPossibleMoves[counter, 0];
                        _arrayAiMoveArray[1] = _arrayPossibleMoves[counter, 1];
                    
                }
                //declare end of _game as there are no moves left
                else EndOfGameCleanUp();

            }

            #endregion randomMove

            return _arrayAiMoveArray;
        }
        public Boolean GameWonCheck(string[,] _gameArray, Boolean _update)
        {

            Boolean _isGameWonCheck = false;
            // Vertical
            for (int c = 0; c < 8; c++)
            {
                for (int r = 0; r < 4; r++)
                {
                    if (_gameArray[c, r] == _gameArray[c, (r + 1)]
                            && _gameArray[c, r] == _gameArray[c, (r + 2)]
                            && _gameArray[c, r] == _gameArray[c, (r + 3)]
                            && _gameArray[c, r] != null)
                    {
                        _isGameWonCheck = true;
                        if (_update == true)
                        {
                            WinningArrayCoordinates[0, 0] = c;
                            WinningArrayCoordinates[0, 1] = r;
                            WinningArrayCoordinates[1, 0] = c;
                            WinningArrayCoordinates[1, 1] = (r + 1);
                            WinningArrayCoordinates[2, 0] = c;
                            WinningArrayCoordinates[2, 1] = (r + 2);
                            WinningArrayCoordinates[3, 0] = c;
                            WinningArrayCoordinates[3, 1] = (r + 3);
                        }
                    };
                }
            }

            // Horizontal
            for (int c = 0; c < 5; c++)
            {
                for (int r = 0; r < 7; r++)
                {
                    if (_gameArray[c, r] == _gameArray[(c + 1), r]
                            && _gameArray[c, r] == _gameArray[(c + 2), r]
                            && _gameArray[c, r] == _gameArray[(c + 3), r]
                            && _gameArray[c, r] != null)
                    {
                        _isGameWonCheck = true;
                        if (_update == true)
                        {
                            WinningArrayCoordinates[0, 0] = c;
                            WinningArrayCoordinates[0, 1] = r;
                            WinningArrayCoordinates[1, 0] = (c + 1);
                            WinningArrayCoordinates[1, 1] = r;
                            WinningArrayCoordinates[2, 0] = (c + 2);
                            WinningArrayCoordinates[2, 1] = r;
                            WinningArrayCoordinates[3, 0] = (c + 3);
                            WinningArrayCoordinates[3, 1] = r;
                        }
                    };
                }
            }

            // Diagonal \
            for (int c = 0; c < 5; c++)
            {
                for (int r = 3; r < 7; r++)
                {
                    if (_gameArray[c, r] == _gameArray[(c + 1), (r - 1)]
                            && _gameArray[c, r] == _gameArray[(c + 2), (r - 2)]
                            && _gameArray[c, r] == _gameArray[(c + 3), (r - 3)]
                            && _gameArray[c, r] != null)
                    {
                        _isGameWonCheck = true;
                        if (_update == true)
                        {
                            {
                                WinningArrayCoordinates[0, 0] = c;
                                WinningArrayCoordinates[0, 1] = r;
                                WinningArrayCoordinates[1, 0] = (c + 1);
                                WinningArrayCoordinates[1, 1] = (r - 1);
                                WinningArrayCoordinates[2, 0] = (c + 2);
                                WinningArrayCoordinates[2, 1] = (r - 2);
                                WinningArrayCoordinates[3, 0] = (c + 3);
                                WinningArrayCoordinates[3, 1] = (r - 3);
                            }
                        }
                    };
                }
            }

            // Diagonal /
            for (int c = 0; c < 5; c++)
            {
                for (int r = 0; r < 4; r++)
                {
                    if (_gameArray[c, r] == _gameArray[(c + 1), (r + 1)]
                            && _gameArray[c, r] == _gameArray[(c + 2), (r + 2)]
                            && _gameArray[c, r] == _gameArray[(c + 3), (r + 3)]
                            && _gameArray[c, r] != null)
                    {
                        _isGameWonCheck = true;
                        if (_update == true)
                        {
                            WinningArrayCoordinates[0, 0] = c;
                            WinningArrayCoordinates[0, 1] = r;
                            WinningArrayCoordinates[1, 0] = (c + 1);
                            WinningArrayCoordinates[1, 1] = (r + 1);
                            WinningArrayCoordinates[2, 0] = (c + 2);
                            WinningArrayCoordinates[2, 1] = (r + 2);
                            WinningArrayCoordinates[3, 0] = (c + 3);
                            WinningArrayCoordinates[3, 1] = (r + 3);
                        }
                    };
                }
            }

            if (_update == true)
            {
                _isGameWon = _isGameWonCheck;
            }

            return _isGameWonCheck;
        }
        private Boolean GameOverCheck(string[,] _gameArray)
        {
            Boolean _isGameOver = false;
            if (GameWonCheck(_gameArray, true) == true)
            {
                _isGameOver = true;
                IsGameOver = true;
            }
            if (_isGameOver == false)
            {
                int _possibleMoves = 0;
                for (int c = 0; c < 8; c++)
                {
                    for (int r = 0; r < 7; r++)
                    {
                        _possibleMoves += 1;
                    }
                }
                if (_possibleMoves == 0)
                {
                    _isGameOver = true;
                    IsGameOver = true;
                }
            }
            return _isGameOver;
        }
        public void EndOfGameCleanUp()
        {
            for (int c = 0; c < 8; c++)
            {
                for (int r = 0; r < 7; r++)
                {
                    this.GameArrayEnabled[c, r] = false;
                }
            }
        }

        #endregion //Function
    }
}