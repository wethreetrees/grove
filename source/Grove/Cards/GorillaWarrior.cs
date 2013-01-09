﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Dsl;

  public class GorillaWarrior : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return Card
        .Named("Gorilla Warrior")
        .ManaCost("{2}{G}")
        .Type("Creature Ape Warrior")
        .FlavorText(
          "The gorilla beat its chest and threw great handfuls of leaves into the air. It howled challenge and showed its teeth. The mechanical soldier, not understanding, simply killed it.")
        .Power(3)
        .Toughness(2);
    }
  }
}