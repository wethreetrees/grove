﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai.TargetingRules;
  using Core.Ai.TimingRules;
  using Core.Costs;
  using Core.Dsl;
  using Core.Mana;

  public class BarrinMasterWizard : CardsSource
  {
    public override IEnumerable<CardFactory> GetCards()
    {
      yield return Card
        .Named("Barrin, Master Wizard")
        .ManaCost("{1}{U}{U}")
        .Type("Legendary Creature Human Wizard")
        .Text("{2}, Sacrifice a permanent: Return target creature to its owner's hand.")
        .FlavorText(
          "'Knowledge is no more expensive than ignorance, and at least as satisfying.'{EOL}—Barrin, master wizard")
        .Power(1)
        .Toughness(1)
        .ActivatedAbility(p =>
          {
            p.Text = "{2}, Sacrifice a permanent: Return target creature to its owner's hand.";
            p.Cost = new AggregateCost(
              new PayMana(2.Colorless(), ManaUsage.Abilities),
              new Sacrifice());
            p.Effect = () => new Core.Effects.ReturnToHand();
            p.TargetSelector
              .AddCost(trg =>
                {
                  trg.Is.Card(controlledBy: ControlledBy.SpellOwner).On.Battlefield();
                  trg.Text = "Select a permanent to sacrifice.";
                })
              .AddEffect(trg =>
                {
                  trg.Is.Card(c => c.Is().Creature).On.Battlefield();
                  trg.Text = "Select a creature to bounce.";
                });

            p.TargetingRule(new SacrificeToBounce());
            p.TimingRule(new TargetRemoval());
          });
    }
  }
}