﻿namespace Grove.Modifiers
{
  using System.Linq;
  using Grove.Events;
  using Grove.Infrastructure;

  public class Add11ForEachOtherCreature : Modifier, ICardModifier, IReceive<ZoneChangedEvent>,
    IReceive<TypeChangedEvent>, IReceive<ControllerChangedEvent>
  {
    private IntegerIncrement _integerIncrement = new IntegerIncrement();
    private Strenght _strenght;

    public override void Apply(Strenght strenght)
    {
      _strenght = strenght;
      _strenght.AddPowerModifier(_integerIncrement);
      _strenght.AddToughnessModifier(_integerIncrement);
    }

    public void Receive(ControllerChangedEvent message)
    {
      if (message.Card.Is().Creature || message.Card == SourceCard)
      {
        _integerIncrement.Value = GetCreatureCount();
      }
    }

    public void Receive(TypeChangedEvent message)
    {
      if (message.Card.Controller != SourceCard.Controller ||
        !message.Card.Is().Creature)
      {
        return;
      }

      _integerIncrement.Value = GetCreatureCount();
    }

    public void Receive(ZoneChangedEvent message)
    {
      if (message.Card.Controller != SourceCard.Controller ||
        !message.Card.Is().Creature)
      {
        return;
      }

      if (message.From == Zone.Battlefield)
      {
        _integerIncrement--;
      }

      else if (message.To == Zone.Battlefield)
      {
        _integerIncrement++;
      }
    }

    protected override void Unapply()
    {
      _strenght.RemovePowerModifier(_integerIncrement);
      _strenght.RemoveToughnessModifier(_integerIncrement);
    }

    protected override void Initialize()
    {
      _integerIncrement.Initialize(ChangeTracker);
      _integerIncrement.Value = GetCreatureCount();
    }

    private int GetCreatureCount()
    {
      return SourceCard.Controller.Battlefield.Count(card => card != Owner && card.Is().Creature);
    }
  }
}