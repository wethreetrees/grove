﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Gameplay.Abilities;
  using Gameplay.Characteristics;
  using Gameplay.Misc;

  public class WeatherseedFaeries : CardsSource
  {
    public override IEnumerable<CardFactory> GetCards()
    {
      yield return Card
        .Named("Weatherseed Faeries")
        .ManaCost("{2}{U}")
        .Type("Creature Faerie")
        .Text("{Flying}, {protection from red}")
        .FlavorText("Two days after the forge was completed, the faeries were immune to its flames.")
        .Power(2)
        .Toughness(1)
        .SimpleAbilities(Static.Flying)
        .Protections(CardColor.Red);
    }
  }
}