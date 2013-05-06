﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Artifical;
  using Artifical.TargetingRules;
  using Artifical.TimingRules;
  using Gameplay.Effects;
  using Gameplay.Misc;

  public class Rescind : CardsSource
  {
    public override IEnumerable<CardFactory> GetCards()
    {
      yield return Card
        .Named("Rescind")
        .ManaCost("{1}{U}{U}")
        .Type("Instant")
        .Text("Return target permanent to its owner's hand.{EOL}Cycling {2}({2}, Discard this card: Draw a card.)")
        .Cycling("{2}")
        .Cast(p =>
          {
            p.Effect = () => new ReturnToHand {Category = EffectCategories.Bounce};
            p.TargetSelector.AddEffect(trg => trg.Is.Card().On.Battlefield());

            p.TargetingRule(new Bounce());
            p.TimingRule(new TargetRemoval());
          });
    }
  }
}