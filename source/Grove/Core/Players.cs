﻿namespace Grove.Core
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using Infrastructure;
  using Targeting;

  [Copyable]
  public class Players : IEnumerable<IPlayer>, IHashable
  {
    private readonly TrackableList<Player> _extraTurns;
    private IPlayer _player1;
    private IPlayer _player2;
    private IPlayer _starting;

    public Players(ChangeTracker changeTracker)
    {
      _extraTurns = new TrackableList<Player>(changeTracker, orderImpactsHashcode: true);
    }

    private Players() {}

    public IPlayer Active { get { return Player1.IsActive ? Player1 : Player2; } }

    public bool AnotherMulliganRound
    {
      get
      {
        return
          (Player1.CanMulligan) ||
            (Player2.CanMulligan);
      }
    }

    public IPlayer Attacking { get { return Active; } }
    public bool BothHaveLost { get { return Player1.HasLost && Player2.HasLost; } }
    public IPlayer Computer { get { return Player1.IsHuman ? Player2 : Player1; } }
    public IPlayer Defending { get { return Passive; } }
    public IPlayer Human { get { return Player1.IsHuman ? Player1 : Player2; } }
    public IPlayer this[int num] { get { return num == 0 ? Player1 : Player2; } }
    public IPlayer Max { get { return Player1.IsMax ? Player1 : Player2; } }
    public IPlayer Min { get { return GetOpponent(Max); } }
    public IPlayer Passive { get { return GetOpponent(Active); } }
    public IPlayer Player1
    {
      get { return _player1; }
      set
      {
        _player1 = value;
        _player1.IsMax = true;
      }
    }

    public IPlayer Player2
    {
      get { return _player2; }
      set
      {
        _player2 = value;
        _player2.IsMax = false;
      }
    }

    public int Score { get { return Player1.Score + Player2.Score; } }

    public IPlayer Starting
    {
      get { return _starting; }
      set
      {
        _starting = value;

        value.IsActive = true;
        GetOpponent(value).IsActive = false;
      }
    }

    public IPlayer WithPriority { get { return _player1.HasPriority ? _player1 : _player2; } }

    public IPlayer WithoutPriority { get { return _player1.HasPriority ? _player2 : _player1; } }

    public IEnumerator<IPlayer> GetEnumerator()
    {
      yield return Player1;
      yield return Player2;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public IEnumerable<ITarget> GetTargets()
    {
      return this.SelectMany(x => x.GetTargets());
    }

    public int CalculateHash(HashCalculator calc)
    {
      return HashCalculator.Combine(
        calc.Calculate(Player1),
        calc.Calculate(Player2),
        calc.Calculate(_extraTurns));
    }

    public void SetAiVisibility(IPlayer player)
    {
      Player1.SetAiVisibility(player);
      Player2.SetAiVisibility(player);
    }

    public void ChangeActivePlayer()
    {
      if (_extraTurns.Count > 0)
      {
        var nextActive = _extraTurns.PopLast();
        var opponent = GetOpponent(nextActive);

        nextActive.IsActive = true;
        opponent.IsActive = false;

        return;
      }

      Player1.IsActive = !Player1.IsActive;
      Player2.IsActive = !Player2.IsActive;
    }

    public IPlayer GetOpponent(IPlayer player)
    {
      return player == Player1 ? Player2 : Player1;
    }

    public void ScheduleExtraTurns(Player player, int count)
    {
      for (var i = 0; i < count; i++)
      {
        _extraTurns.Add(player);
      }
    }

    public void SetPriority(IPlayer player)
    {
      player.HasPriority = true;
      GetOpponent(player).HasPriority = false;
    }

    public bool AnyHasLost()
    {
      return Player1.HasLost || Player2.HasLost;
    }

    public IEnumerable<Card> Permanents()
    {
      return this.SelectMany(x => x.Battlefield);
    }

    public void MoveDeadCreaturesToGraveyard()
    {
      foreach (var player in this)
      {
        player.MoveCreaturesWithLeathalDamageOrZeroTougnessToGraveyard();
      }

      RespectLegendaryRule();
    }

    public void RemoveDamageFromPermanents()
    {
      foreach (var player in this)
      {
        player.RemoveDamageFromPermanents();
      }
    }

    public void RemoveRegenerationFromPermanents()
    {
      foreach (var player in this)
      {
        player.RemoveRegenerationFromPermanents();
      }
    }

    private void RespectLegendaryRule()
    {
      var duplicateLegends = this
        .SelectMany(x => x.Battlefield.Legends)
        .GetDuplicates(card => card.Name).ToArray();

      foreach (var legend in duplicateLegends)
      {        
        legend.Sacrifice();        
      }
    }

    public void DestroyPermanents(Func<Card, bool> filter = null)
    {
      filter = filter ?? delegate { return true; };

      foreach (var permanent in Permanents().ToList())
      {
        if (filter(permanent))
        {
          permanent.Destroy();
        }
      }
    }
  }
}