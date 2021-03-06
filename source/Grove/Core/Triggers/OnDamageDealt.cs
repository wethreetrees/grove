﻿namespace Grove.Triggers
{
  using System;
  using Events;
  using Infrastructure;

  public class OnDamageDealt : Trigger, IReceive<DamageDealtEvent>
  {
    private readonly Func<Parameters, bool> _predicate;

    private OnDamageDealt() {}

    public OnDamageDealt(Func<Parameters, bool> predicate)
    {
      _predicate = predicate;
    }

    public void Receive(DamageDealtEvent message)
    {
      if (_predicate(new Parameters(message.Receiver, this, message.Damage)))
      {
        Set(message);
      }
    }

    public class Parameters
    {
      private readonly Damage _damage;
      private readonly object _dealtTo;
      private readonly OnDamageDealt _trigger;

      public Parameters(object dealtTo, OnDamageDealt trigger, Damage damage)
      {
        _dealtTo = dealtTo;

        _trigger = trigger;
        _damage = damage;
      }

      public bool IsDealtToPlayer
      {
        get { return _dealtTo is Player; }
      }

      public bool IsDealtToCreature
      {
        get { return _dealtTo is Card; }
      }


      public bool IsCombat
      {
        get { return _damage.IsCombat; }
      }

      public bool IsDealtByEnchantedCreature
      {
        get
        {
          var owningCard = _trigger.Ability.OwningCard;
          return owningCard.AttachedTo == _damage.Source;
        }
      }

      public Card Source
      {
        get { return _damage.Source; }
      }

      public bool IsDealtToYou
      {
        get { return _dealtTo == _trigger.Controller; }
      }

      public bool IsDealtByOwningCard
      {
        get { return _trigger.OwningCard == _damage.Source; }
      }

      public bool IsDealtToOpponent
      {
        get { return _dealtTo == _trigger.Controller.Opponent; }
      }

      public bool IsDealtToOwningCard
      {
        get { return _dealtTo == _trigger.OwningCard; }
      }

      public bool IsDealtToEnchantedCreature
      {
        get { return _dealtTo == _trigger.Ability.OwningCard.AttachedTo; }
      }
    }
  }
}