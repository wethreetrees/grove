﻿namespace Grove.Gameplay
{
  using System;
  using System.Threading.Tasks;
  using System.Windows;
  using Infrastructure;
  using Misc;
  using Persistance;
  using UserInterface;
  using UserInterface.Shell;

  public class Match
  {
    private readonly Game.IFactory _gameFactory;
    private readonly MatchParameters _p;
    private readonly IShell _shell;
    private readonly ViewModelFactories _viewModels;
    private int? _looser;
    private bool _playerLeftMatch;
    private bool _rematch;

    public Match(MatchParameters p, IShell shell, ViewModelFactories viewModels, Game.IFactory gameFactory)
    {
      _p = p;
      _shell = shell;
      _viewModels = viewModels;
      _gameFactory = gameFactory;

      Application.Current.Exit += delegate { Stop(); };
    }

    public bool IsTournament { get { return _p.IsTournament; } }
    public bool WasStopped { get { return Game != null && Game.WasStopped; } }

    public Game Game { get; private set; }

    public bool IsFinished
    {
      get
      {
        return
          Player1WinCount == 2 ||
            Player2WinCount == 2;
      }
    }

    public int Player1WinCount { get; private set; }
    public int Player2WinCount { get; private set; }    

    protected Player Looser
    {
      get
      {
        if (_looser == null)
          return null;

        return Game.Players[_looser.Value];
      }
    }

    public bool InProgress { get { return Game != null && !IsFinished; } }

    public string Description
    {
      get
      {
        return String.Format("{0}({1}) vs {2}({3}), {4}. turn", Game.Players.Player1.Name, Game.Players.Player1.Life,
          Game.Players.Player2.Name, Game.Players.Player2.Life, Game.Turn.TurnCount);
      }
    }

    private void DisplayGameResults()
    {
      var viewModel = _viewModels.GameResults.Create();
      _shell.ShowModalDialog(viewModel);
      _playerLeftMatch = viewModel.PlayerLeftMatch;
    }

    private void DisplayMatchResults()
    {
      var viewModel = _viewModels.MatchResults.Create(canRematch: !IsTournament);
      _shell.ShowModalDialog(viewModel);
      _rematch = viewModel.ShouldRematch;
    }

    public void Start()
    {
      Game game;

      if (_p.IsSavedMatch)
      {
        game = _gameFactory.Create(GameParameters.Load(
          player1Controller: ControllerType.Human,
          player2Controller: ControllerType.Machine,
          savedGame: _p.SavedMatch.SavedGame));
      }
      else
      {
        game = _gameFactory.Create(GameParameters.Default(
          _p.Player1, _p.Player2));
      }

      var shouldPlayAnotherGame = RunGame(game);

      while (shouldPlayAnotherGame || _rematch)
      {
        if (_rematch)
        {
          Player1WinCount = 0;
          Player2WinCount = 0;
          _rematch = false;          
        }
        
        game = _gameFactory.Create(GameParameters.Default(
          _p.Player1, _p.Player2));

        shouldPlayAnotherGame = RunGame(game);        
      }            
    }

    private bool RunGame(Game game)
    {
      Game = game;      

      var playScreen = _viewModels.PlayScreen.Create();
      _shell.ChangeScreen(playScreen);

      var blocker = new ThreadBlocker();

      blocker.BlockUntilCompleted(() => Task.Factory.StartNew(() =>
        {
          Game.Start(looser: Looser);
          blocker.Completed();
        }, TaskCreationOptions.LongRunning));

      return ProcessGameResults();
    }

    private bool ProcessGameResults() {
      if (Game.WasStopped)
        return false;

      var looser = UpdateScore();
      SetLooser(looser);

      if (Game.WasStopped)
        return false;

      if (IsFinished)
      {
        DisplayMatchResults();

        if (Game.WasStopped)
          return false;

        if (_rematch && !_playerLeftMatch)
        {          
          return false;
        }

        return false;
      }

      DisplayGameResults();

      if (Game.WasStopped)
        return false;

      if (_playerLeftMatch)
      {
        Player2WinCount = 2;
        return false;
      }

      return true;
    }

    private void SetLooser(int? looser)
    {
      _looser = looser;
    }

    private int? UpdateScore()
    {
      if (Game.Players.BothHaveLost)
        return null;

      if (Game.Players.Player1.HasLost)
      {
        Player2WinCount++;
        return 0;
      }

      Player1WinCount++;
      return 1;
    }

    public void Rematch()
    {      
      Stop();      
      _rematch = true;
    }

    public void Stop()
    {
      _rematch = false;

      if (Game != null)
      {
        Game.Stop();
      }

      _shell.CloseAllDialogs();
    }

    public SavedMatch Save()
    {
      var savedMatch = new SavedMatch
        {
          Player1WinCount = Player1WinCount,
          Player2WinCount = Player2WinCount,
          SavedGame = Game.Save()
        };

      return savedMatch;
    }

    public interface IFactory
    {
      Match Create(MatchParameters p);
    }
  }
}