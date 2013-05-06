﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Artifical.TimingRules;
  using Gameplay.Effects;
  using Gameplay.Misc;

  public class Sift : CardsSource
  {
    public override IEnumerable<CardFactory> GetCards()
    {
      yield return Card
        .Named("Sift")
        .ManaCost("{3}{U}")
        .Type("Sorcery")
        .Text("Draw three cards, then discard a card.")
        .FlavorText("Dwell longest on the thoughts that shine brightest.")
        .Cast(p =>
          {
            p.Effect = () => new DrawCards(count: 3, discardCount: 1);
            p.TimingRule(new FirstMain());
          });
    }
  }
}