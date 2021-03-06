﻿namespace Grove.AI.TargetingRules
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class CostSacrificeToDrawCards : TargetingRule
  {
    private readonly Func<Card, bool> _filter;

    public CostSacrificeToDrawCards(Func<Card, bool> filter = null)
    {
      _filter = filter ?? delegate { return true; };
    }

    private CostSacrificeToDrawCards() {}

    protected override IEnumerable<Targets> SelectTargets(TargetingRuleParameters p)
    {
      var candidates = new List<Card>();

      if (Turn.Step == Step.DeclareBlockers && Stack.IsEmpty)
      {
        candidates.AddRange(
          p.Candidates<Card>()
            .Where(x => _filter(x))
            .Where(x => Combat.CanBeDealtLeathalCombatDamage(x))
            .Where(x => !Combat.CanKillAny(x)));
      }

      candidates.AddRange(
        p.Candidates<Card>()
          .Where(x => _filter(x))
          .Where(x => Stack.CanBeDestroyedByTopSpell(x)));

      return Group(candidates, p.TotalMinTargetCount(), add: (trg, trgs) => trgs.AddCost(trg));
    }
  }
}