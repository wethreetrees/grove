﻿namespace Grove.CardsLibrary
{
    using System.Collections.Generic;
    using AI;
    using AI.TimingRules;
    using Effects;
    using Modifiers;

    public class SanctifiedCharge : CardTemplateSource
    {
        public override IEnumerable<CardTemplate> GetCards()
        {
            yield return Card
                .Named("Sanctified Charge")
                .ManaCost("{4}{W}")
                .Type("Instant")
                .Text("Creatures you control get +2/+1 until end of turn. White creatures you control also gain first strike until end of turn. (They deal combat damage before creatures without first strike.)")
                .FlavorText("You need only raise your spear to receive this blessing.")
                .Cast(p =>
                {
                    p.Effect = () => new CompoundEffect(
                        new ApplyModifiersToPermanents(
                            selector: (e, c) => e.Source.OwningCard.Controller == c.Controller && c.Is().Creature,
                            modifiers: () => new AddPowerAndToughness(2, 1) { UntilEot = true }).SetTags(EffectTag.IncreasePower, EffectTag.IncreaseToughness),
                        new ApplyModifiersToPermanents(
                            selector: (e, c) => e.Source.OwningCard.Controller == c.Controller && c.Is().Creature && c.HasColor(CardColor.White),
                            modifiers: () => new AddStaticAbility(Static.FirstStrike) { UntilEot = true }));

                    p.TimingRule(new Any(new AfterYouDeclareBlockers(), new AfterOpponentDeclaresBlockers()));
                });
        }
    }
}
