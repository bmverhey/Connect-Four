using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ConnectFour
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region constructor

        public MainWindowViewModel()
        {
            _modelGame = new Game();
            RefreshUiComponents(ModelGame);
            CommandMoveButton = new RelayCommand(p => ExecuteCommandMoveButton(p.ToString(), ModelGame));
            CommandNewGame = new RelayCommand(p => ExecuteCommandNewGame());
            CommandExit = new RelayCommand(p => ExecuteCommandExit());
            CommandAiLevelChange = new RelayCommand(p => ExecuteCommandAiLevelChange(p.ToString()));
        }

        #endregion //constructor

        #region Properties

        public RelayCommand CommandMoveButton { get; set; }
        public RelayCommand CommandExit { get; set; }
        public RelayCommand CommandNewGame { get; set; }
        public RelayCommand CommandAiLevelChange { get; set; }

        private Game _modelGame;
        public Game ModelGame
        {
            get { return _modelGame; }
            set
            {
                _modelGame = RefreshUiComponents(_modelGame);
                _modelGame = value;
            }
        }

        List<String> _listColor = new List<String>();
        public List<String> ListColor
        {
            get { return _listColor; }
            set { _listColor = value; }
        }

        List<String> _listBorderColor = new List<String>();
        public List<String> ListBorderColor
        {
            get { return _listBorderColor; }
            set { _listBorderColor = value; }
        }

        private String[,] _gameArrayColor = new String[8, 7]
        {
            {"Transparent","Transparent","Transparent","Transparent","Transparent","Transparent","Transparent"},
            {"Transparent","Transparent","Transparent","Transparent","Transparent","Transparent","Transparent"},
            {"Transparent","Transparent","Transparent","Transparent","Transparent","Transparent","Transparent"},
            {"Transparent","Transparent","Transparent","Transparent","Transparent","Transparent","Transparent"},
            {"Transparent","Transparent","Transparent","Transparent","Transparent","Transparent","Transparent"},
            {"Transparent","Transparent","Transparent","Transparent","Transparent","Transparent","Transparent"},
            {"Transparent","Transparent","Transparent","Transparent","Transparent","Transparent","Transparent"},
            {"Transparent","Transparent","Transparent","Transparent","Transparent","Transparent","Transparent"},
        };
        public String[,] GameArrayColor
        {
            get { return _gameArrayColor; }
            set { _gameArrayColor = value; }
        }

        private String[,] _gameArrayBorderColor = new String[8, 7]
        {
            {"Black","Black","Black","Black","Black","Black","Black"},
            {"Black","Black","Black","Black","Black","Black","Black"},
            {"Black","Black","Black","Black","Black","Black","Black"},
            {"Black","Black","Black","Black","Black","Black","Black"},
            {"Black","Black","Black","Black","Black","Black","Black"},
            {"Black","Black","Black","Black","Black","Black","Black"},
            {"Black","Black","Black","Black","Black","Black","Black"},
            {"Black","Black","Black","Black","Black","Black","Black"},
        };
        public String[,] GameArrayBorderColor
        {
            get { return _gameArrayBorderColor; }
            set { _gameArrayBorderColor = value; }
        }

        #endregion //Properties

        #region Functions

        public Game RefreshUiComponents(Game _game)
        {
            string _borderColorWin = "Lime";
            string _borderColorNormal = "Black";

            //highlight winning combination
            if (ModelGame.IsGameWon == true)
            {

                for (int i = 0; i < 4; i++)
                {
                    int c = ModelGame.WinningArrayCoordinates[i, 0];
                    int r = ModelGame.WinningArrayCoordinates[i, 1];
                    GameArrayBorderColor[c, r] = _borderColorWin;
                }
            }
            else
            {
                for (int c = 0; c < 8; c++)
                {
                    for (int r = 0; r < 7; r++)
                    {
                        GameArrayBorderColor[c, r] = _borderColorNormal;
                    }
                }
            }

            //translate moves to color
            for (int c = 0; c < 8; c++)
            {
                for (int r = 0; r < 7; r++)
                {
                    if (ModelGame.GameArrayPosition[c, r] == "Human")
                    {
                        GameArrayColor[c, r] = "LightSkyBlue";
                    }
                    else if (ModelGame.GameArrayPosition[c, r] == "Ai")
                    {
                        GameArrayColor[c, r] = "Black";
                    }
                    else
                    {
                        GameArrayColor[c, r] = "Transparent";
                    }
                }
            }

            //Color List Build
            ListColor.Clear();
            for (int c = 0; c < 8; c++)
            {
                for (int r = 0; r < 7; r++)
                {
                    ListColor.Add(GameArrayColor[c, r]);
                }
            }

            //Color Border List Build
            ListBorderColor.Clear();
            for (int c = 0; c < 8; c++)
            {
                for (int r = 0; r < 7; r++)
                {
                    ListBorderColor.Add(GameArrayBorderColor[c, r]);
                }
            }

            NotifyPropertyChanged("ListColor");
            NotifyPropertyChanged("ListBorderColor");
            return _game;
        }

        #endregion //functions

        #region iINotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion //INotifyPropertyChanged

        #region UserEventsOrCommands

        public void ExecuteCommandMoveButton(string _parameter, Game _game)
        {
            string s = _parameter.ToString();
            string[] _separator = { "," };
            string[] arrayValues = s.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
            int columnHuman = Convert.ToInt32(arrayValues[0]);
            int rowHuman = Convert.ToInt32(arrayValues[1]);
            if (_game.GameLowestAvailableRowInColumn(columnHuman, rowHuman, _game) > -1)
            {
                rowHuman = _game.GameLowestAvailableRowInColumn(columnHuman, rowHuman, _game);
                if (_game.GameArrayEnabled[columnHuman, rowHuman] == true && _game.IsGameOver == false && _game.IsGameWon == false)
                {
                    ModelGame = ModelGame.GameMoveMake("Human", columnHuman, rowHuman, _game);
                    ModelGame.AiTurn(_game);
                }
            }
            ModelGame = RefreshUiComponents(_game);
        }

        public void ExecuteCommandNewGame()
        {
            int _saveGameAiLevel = ModelGame.AiLevel;
            Game _gameNew = new Game();
            ModelGame = _gameNew;
            RefreshUiComponents(_gameNew);
            ModelGame.AiLevel = _saveGameAiLevel;
        }

        public void ExecuteCommandExit()
        {
            System.Environment.Exit(0);
        }

        private void ExecuteCommandAiLevelChange(string AiLevel)
        {
            ModelGame.AiLevel = Convert.ToInt32(AiLevel);
        }

        #endregion
    }
}