﻿namespace Grove.Artifical.TimingRules
{
  using System;
  using Gameplay;

  public class OwningCardHas : TimingRule
  {
    private readonly Func<Card, bool> _predicate;

    private OwningCardHas() {}

    public OwningCardHas(Func<Card, bool> predicate)
    {
      _predicate = predicate;
    }

    public override bool ShouldPlay(TimingRuleParameters p)
    {
      return _predicate(p.Card);
    }
  }
}