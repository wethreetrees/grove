﻿namespace Grove.CardsLibrary
{
    using System.Collections.Generic;
    using AI.TargetingRules;
    using AI.TimingRules;
    using Costs;
    using Effects;
    using Triggers;

    public class RoguesGloves : CardTemplateSource
    {
        public override IEnumerable<CardTemplate> GetCards()
        {
            yield return Card
                .Named("Rogue's Gloves")
                .ManaCost("{2}")
                .Type("Artifact — Equipment")
                .Text("Whenever equipped creature deals combat damage to a player, you may draw a card.{EOL}{Equip}{2} {I}({2}: Attach to target creature you control. Equip only as a sorcery.){/I}")
                .FlavorText("Professional pilferers prefer proper preparation.")
                .Cast(p => p.TimingRule(new OnFirstMain()))
                .ActivatedAbility(p =>
                {
                    p.Text = "{2}: Attach to target creature you control. Equip only as a sorcery.";

                    p.Cost = new PayMana(2.Colorless(), ManaUsage.Abilities);

                    p.Effect = () => new Attach();

                    p.TargetSelector.AddEffect(trg => trg.Is.ValidEquipmentTarget().On.Battlefield());

                    p.TargetingRule(new EffectCombatEquipment());
                    p.TimingRule(new OnFirstMain());

                    p.ActivateAsSorcery = true;
                })
                .TriggeredAbility(p =>
                {
                    p.Text = "Whenever equipped creature deals combat damage to a player, you may draw a card.";

                    p.Trigger(new OnDamageDealt(
                        combatOnly: true,
                        useAttachedToAsTriggerSource: true,
                        playerFilter: (ply, trg, dmg) => ply != trg.OwningCard.Controller));

                    p.Effect = () => new DrawCards(1);

                    p.TriggerOnlyIfOwningCardIsInPlay = true;
                });
        }
    }
}
