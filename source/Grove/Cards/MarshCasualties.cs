﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai.TargetingRules;
  using Core.Costs;
  using Core.Dsl;
  using Core.Effects;
  using Core.Mana;
  using Core.Modifiers;

  public class MarshCasualties : CardsSource
  {
    public override IEnumerable<CardFactory> GetCards()
    {
      yield return Card
        .Named("Marsh Casualties")
        .ManaCost("{B}{B}")
        .Type("Sorcery")
        .Text(
          "{Kicker} {3}{EOL}Creatures target player controls get -1/-1 until end of turn. If Marsh Casualties was kicked, those creatures get -2/-2 until end of turn instead.")
        .Cast(p =>
          {
            p.Effect = () => new ApplyModifiersToPermanents(
              filter: (effect, card) => card.Is().Creature,
              modifiers: () => new AddPowerAndToughness(-1, -1) {UntilEot = true}) {ToughnessReduction = 1};

            p.TargetSelector.AddEffect(trg => trg.Is.Player());
            p.TargetingRule(new Opponent());
          })
        .Cast(p =>
          {
            p.Text = p.KickerDescription;
            p.Cost = new PayMana("{3}{B}{B}".ParseMana(), ManaUsage.Spells);
            p.Effect = () => new ApplyModifiersToPermanents(
              filter: (effect, card) => card.Is().Creature,
              modifiers: () => new AddPowerAndToughness(-2, -2) {UntilEot = true}) {ToughnessReduction = 2};

            p.TargetSelector.AddEffect(trg => trg.Is.Player());
            p.TargetingRule(new Opponent());
          });
    }
  }
}