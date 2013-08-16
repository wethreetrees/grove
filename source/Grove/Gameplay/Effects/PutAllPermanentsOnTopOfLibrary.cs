﻿namespace Grove.Gameplay.Effects
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Artifical.Decisions;
  using Decisions;
  using Decisions.Results;
  using Infrastructure;

  public class PutAllPermanentsOnTopOfLibrary : Effect
  {
    private readonly Func<Card, bool> _filter;

    private PutAllPermanentsOnTopOfLibrary() {}

    public PutAllPermanentsOnTopOfLibrary(Func<Card, bool> filter)
    {
      _filter = filter;
    }

    protected override void ResolveEffect()
    {
      PutPlayersPermanentsOnTopOfLibrary(Players.Active);
      PutPlayersPermanentsOnTopOfLibrary(Players.Passive);
    }

    private void PutPlayersPermanentsOnTopOfLibrary(Player player)
    {
      var permanents = player.Battlefield
        .Where(x => _filter(x))
        .ToList();

      var orderAndPutOnTop = new OrderAndPutOnTopOfLibrary(player);

      Enqueue<Decisions.OrderCards>(
        controller: player,
        init: p =>
          {
            p.Cards = permanents;
            p.ProcessDecisionResults = orderAndPutOnTop;
            p.ChooseDecisionResults = orderAndPutOnTop;
            p.Title = "Order cards from top to bottom";
          });
    }

    [Copyable]
    public class OrderAndPutOnTopOfLibrary : IProcessDecisionResults<Ordering>,
      IChooseDecisionResults<List<Card>, Ordering>
    {
      private readonly Player _controller;
      private List<Card> _candidates;

      private OrderAndPutOnTopOfLibrary() {}

      public OrderAndPutOnTopOfLibrary(Player controller)
      {
        _controller = controller;
      }

      public Ordering ChooseResult(List<Card> candidates)
      {
        _candidates = candidates;
        return QuickDecisions.OrderTopCards(candidates, _controller);
      }

      public void ProcessResults(Ordering results)
      {
        _candidates.ShuffleInPlace(results.Indices);

        for (var i = _candidates.Count - 1; i >= 0; i--)
        {
          _candidates[i].PutOnTopOfLibrary();
        }
      }
    }
  }
}