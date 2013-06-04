﻿namespace Grove.Gameplay.Misc
{
  using System;
  using System.Collections.Generic;
  using Abilities;
  using Artifical;
  using Artifical.TimingRules;
  using CastingRules;
  using Characteristics;
  using Costs;
  using Damage;
  using Effects;
  using ManaHandling;
  using Modifiers;
  using States;
  using Triggers;
  using Zones;

  public class CardFactory
  {
    private readonly List<Action<CardParameters>> _init = new List<Action<CardParameters>>();

    public string Name { get; private set; }

    public Card CreateCard()
    {
      var p = new CardParameters();

      foreach (var action in _init)
      {
        action(p);
      }

      if (p.Colors.Count == 0)
      {
        p.Colors.AddRange(GetCardColorsFromManaCost(p.ManaCost));
      }

      if (p.CastInstructions.Count == 0)
      {
        var castParams = GetDefaultCastInstructionParameters(p);
        SetDefaultTimingRules(p, castParams);
        p.CastInstructions.Add(new CastInstruction(castParams));
      }

      return new Card(p);
    }

    private CastInstructionParameters GetDefaultCastInstructionParameters(CardParameters cp)
    {
      return new CastInstructionParameters
        {
          Cost = new PayMana(cp.ManaCost ?? Mana.Zero, ManaUsage.Spells, cp.HasXInCost),
          Text = string.Format("Cast {0}.", cp.Name),
          Effect = () => new PutIntoPlay(),
          Rule = GetDefaultCastingRule(cp.Type),
        };
    }

    private CastingRule GetDefaultCastingRule(CardType cardType)
    {
      if (cardType.Instant)
        return new Instant();
      if (cardType.Sorcery)
        return new Sorcery();
      if (cardType.Land)
        return new Land();
      if (cardType.Aura)
        return new Aura();

      return new Permanent();
    }

    public CardFactory HasXInCost()
    {
      _init.Add(p => p.HasXInCost = true);
      return this;
    }

    public CardFactory Protections(CardColor color)
    {
      _init.Add(p => p.Protections.AddProtectionFromColor(color));
      return this;
    }

    public CardFactory Cast(Action<CastInstructionParameters> set)
    {
      _init.Add(cp =>
        {
          var p = GetDefaultCastInstructionParameters(cp);

          set(p);

          p.Text = string.Format(p.Text, cp.Name);

          if (p.HasTimingRules == false)
          {
            SetDefaultTimingRules(cp, p);
          }

          cp.CastInstructions.Add(new CastInstruction(p));
        }
        );

      return this;
    }

    private static IEnumerable<CardColor> GetCardColorsFromManaCost(IManaAmount manaCost)
    {
      if (manaCost == null)
      {
        yield return CardColor.None;
        yield break;
      }

      if (manaCost.Converted == 0)
      {
        yield return CardColor.Colorless;
        yield break;
      }

      var existing = new HashSet<CardColor>();

      foreach (var mana in manaCost)
      {
        if (mana.Color.IsWhite && !existing.Contains(CardColor.White))
        {
          existing.Add(CardColor.White);
          yield return CardColor.White;
        }

        if (mana.Color.IsBlue && !existing.Contains(CardColor.Blue))
        {
          existing.Add(CardColor.Blue);
          yield return CardColor.Blue;
        }

        if (mana.Color.IsBlack && !existing.Contains(CardColor.Black))
        {
          existing.Add(CardColor.Black);
          yield return CardColor.Black;
        }

        if (mana.Color.IsRed && !existing.Contains(CardColor.Red))
        {
          existing.Add(CardColor.Red);
          yield return CardColor.Red;
        }

        if (mana.Color.IsGreen && !existing.Contains(CardColor.Green))
        {
          existing.Add(CardColor.Green);
          yield return CardColor.Green;
        }
      }

      if (existing.Count == 0)
        yield return CardColor.Colorless;
    }

    private static void SetDefaultTimingRules(CardParameters cp, CastInstructionParameters p)
    {
      if (cp.Type.Creature)
      {
        p.TimingRule(new Creatures());
      }
      else if (cp.Type.Land)
      {
        p.TimingRule(new Lands());
      }
    }

    public CardFactory Prevention(Func<DamagePrevention> factory)
    {
      _init.Add(p => p.DamagePreventions.AddPrevention(factory()));
      return this;
    }

    public CardFactory Protections(params string[] cardTypes)
    {
      _init.Add(p => p.Protections.AddProtectionFromCards(cardTypes));

      return this;
    }

    public CardFactory Echo(string manaCost)
    {
      var amount = manaCost.Parse();

      TriggeredAbility(p =>
        {
          p.Trigger(new OnStepStart(Step.Upkeep, onlyOnceWhenAfterItComesUnderYourControl: true));
          p.Text =
            "At the beginning of your upkeep, if this came under your control since the beginning of your last upkeep, sacrifice it unless you pay its echo cost.";
          p.Effect = () => new PayManaOrSacrifice(amount, "Pay echo?");
          p.TriggerOnlyIfOwningCardIsInPlay = true;
        });

      return this;
    }

    public CardFactory ActivatedAbility(Action<ActivatedAbilityParameters> set)
    {
      _init.Add(cp =>
        {
          var p = new ActivatedAbilityParameters();
          set(p);
          cp.ActivatedAbilities.Add(new ActivatedAbility(p));
        });
      return this;
    }

    public CardFactory ManaAbility(Action<ManaAbilityParameters> set)
    {
      _init.Add(cp =>
        {
          var p = new ManaAbilityParameters();
          p.Cost = new Tap();
          p.Priority = GetDefaultManaSourcePriority(cp);
          p.TapRestriction = true;

          set(p);

          cp.ActivatedAbilities.Add(new ManaAbility(p));
        });
      return this;
    }

    private int GetDefaultManaSourcePriority(CardParameters cp)
    {
      if (cp.Type.Creature)
        return ManaSourcePriorities.Creature;
      if (cp.Type.Land)
        return ManaSourcePriorities.Land;

      return ManaSourcePriorities.Land;
    }

    public CardFactory TriggeredAbility(Action<TriggeredAbilityParameters> set)
    {
      _init.Add(cp =>
        {
          var p = new TriggeredAbilityParameters();
          set(p);
          cp.TriggeredAbilities.Add(new TriggeredAbility(p));
        });
      return this;
    }

    public CardFactory StaticAbilities(params Static[] abilities)
    {
      _init.Add(cp =>
        {
          foreach (var ability in abilities)
          {
            cp.StaticAbilities.Add(ability);
          }
        });
      return this;
    }

    public CardFactory ContinuousEffect(Action<ContinuousEffectParameters> set)
    {
      _init.Add(cp =>
        {
          var p = new ContinuousEffectParameters();
          set(p);
          cp.ContinuousEffects.Add(new ContinuousEffect(p));
        });
      return this;
    }

    public CardFactory Colors(params CardColor[] colors)
    {
      _init.Add(p => p.Colors.AddRange(colors));
      return this;
    }

    public CardFactory IsLeveler()
    {
      _init.Add(p => { p.IsLeveler = true; });
      return this;
    }

    public CardFactory FlavorText(string flavorText)
    {
      _init.Add(p => { p.FlavorText = flavorText; });
      return this;
    }

    public CardFactory ManaCost(string manaCost)
    {
      _init.Add(p => { p.ManaCost = manaCost.Parse(); });
      return this;
    }

    public CardFactory Named(string name)
    {
      Name = name;
      _init.Add(p => { p.Name = name; });
      return this;
    }

    public CardFactory Cycling(string cost)
    {
      ActivatedAbility(p =>
        {
          p.Text = string.Format("Cycling {0} ({0}, Discard this card: Draw a card.)", cost);
          p.Cost = new AggregateCost(new PayMana(cost.Parse(), ManaUsage.Abilities), new Discard());
          p.Effect = () => new DrawCards(1);
          p.ActivationZone = Zone.Hand;
          p.TimingRule(new Cycling());
        });

      return this;
    }

    public CardFactory Power(int power)
    {
      _init.Add(p => { p.Power = power; });
      return this;
    }


    public CardFactory Text(string text)
    {
      _init.Add(p => { p.Text = text; });
      return this;
    }


    public CardFactory Toughness(int toughness)
    {
      _init.Add(p => { p.Toughness = toughness; });
      return this;
    }

    public CardFactory Type(string type)
    {
      _init.Add(p => { p.Type = type; });
      return this;
    }

    public CardFactory Leveler(string cost, EffectCategories category,
      params LevelDefinition[] levels)
    {
      ActivatedAbility(p =>
        {
          p.Text = String.Format("{0}: Put a level counter on this. Level up only as sorcery.", cost);
          p.Cost = new PayMana(cost.Parse(), ManaUsage.Abilities);
          p.Effect = () => new ApplyModifiersToSelf(() => new IncreaseLevel()) {Category = category};
          p.TimingRule(new LevelUp(cost.Parse(), levels));
          p.ActivateAsSorcery = true;
        });

      foreach (var level in levels)
      {
        var lvl = level;
        TriggeredAbility(p =>
          {
            p.Trigger(new OnLevelChanged(lvl.Min));
            p.Effect = () => new ApplyModifiersToSelf(
              () =>
                {
                  var modifier = new AddStaticAbility(lvl.StaticAbility);
                  modifier.AddLifetime(new LevelLifetime(lvl.Min, lvl.Max));
                  return modifier;
                },
              () =>
                {
                  var modifier = new SetPowerAndToughness(lvl.Power, lvl.Toughness);
                  modifier.AddLifetime(new LevelLifetime(lvl.Min, lvl.Max));
                  return modifier;
                }
              );
            p.UsesStack = false;
            p.TriggerOnlyIfOwningCardIsInPlay = true;
          });
      }

      _init.Add(cp => { cp.IsLeveler = true; });

      return this;
    }

    public CardFactory MayChooseToUntap()
    {
      _init.Add(p => p.MayChooseToUntap = true);
      return this;
    }

    public CardFactory OverrideScore(ScoreOverride score)
    {
      _init.Add(p => p.OverrideScore = score);
      return this;
    }
  }
}