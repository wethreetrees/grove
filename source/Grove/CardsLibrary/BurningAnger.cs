﻿namespace Grove.CardsLibrary
{
    using System.Collections.Generic;
    using AI.TargetingRules;
    using AI.TimingRules;
    using Costs;
    using Effects;
    using Modifiers;

    public class BurningAnger : CardTemplateSource
    {
        public override IEnumerable<CardTemplate> GetCards()
        {
            yield return Card
                .Named("Burning Anger")
                .ManaCost("{4}{R}")
                .Type("Enchantment — Aura")
                .Text("Enchant creature{EOL}Enchanted creature has \"{T}: This creature deals damage equal to its power to target creature or player.\"")
                .FlavorText("\"With rage as your forge, your hammer can smite anything.\"")
                .Cast(p =>
                {
                    p.Effect = () => new Attach(() =>
                    {
                        var ap = new ActivatedAbilityParameters
                        {
                            Text = "{T}: This creature deals damage equal to its power to target creature or player.",
                            Cost = new Tap(),
                            Effect = () => new DealDamageToTargets(P(e => e.Source.OwningCard.Power.Value))
                        };

                        ap.TargetSelector.AddEffect(trg => trg.Is.CreatureOrPlayer().On.Battlefield());

                        ap.TargetingRule(new EffectDealDamage(tp => tp.Card.Power.Value));

                        return new AddActivatedAbility(new ActivatedAbility(ap));
                    });

                    p.TargetSelector.AddEffect(trg => trg.Is.Card(c => c.Is().Creature && c.Power.HasValue && c.Power.Value > 0).On.Battlefield());

                    p.TimingRule(new OnFirstMain());
                    p.TargetingRule(new EffectCombatEnchantment());
                });
        }
    }
}
