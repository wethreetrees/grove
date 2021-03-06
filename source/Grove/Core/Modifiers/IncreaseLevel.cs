﻿namespace Grove.Modifiers
{
  public class IncreaseLevel : Modifier, ICardModifier
  {
    private readonly IntegerIncrement _levelIntegerIncrement = new IntegerIncrement(1);
    private Level _level;

    public override void Apply(Level level)
    {
      _level = level;
      _levelIntegerIncrement.Initialize(ChangeTracker);
      _level.AddModifier(_levelIntegerIncrement);
    }

    protected override void Unapply()
    {
      _level.RemoveModifier(_levelIntegerIncrement);
    }
  }
}