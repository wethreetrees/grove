﻿namespace Grove.Core.Decisions.Scenario
{
  using System;
  using Mana;
  using Results;

  public class ScenarioPlayableSpell : PlayableSpell
  {
    public ScenarioPlayableSpell(Card card, ActivationParameters activationParameters, int index)
      : base(card, activationParameters, index) {}

    private Player Controller { get { return Card.Controller; } }

    public override bool CanPlay()
    {      
      if (Card == null)
        throw new InvalidOperationException("Did you forget to add card to players hand?");

      var manaCost = Card.GetSpellManaCost(Index);            

      if (ActivationParameters.X.HasValue)
        manaCost = manaCost.Add(ActivationParameters.X.Value);

      Controller.AddManaToManaPool(manaCost);
      var prerequisites = Card.CanCast();

      return prerequisites[Index].CanBeSatisfied;
    }
  }
}