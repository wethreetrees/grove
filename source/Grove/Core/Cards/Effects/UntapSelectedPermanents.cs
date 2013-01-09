﻿namespace Grove.Core.Cards.Effects
{
  using System;
  using Decisions;

  public class UntapSelectedPermanents : Effect
  {
    public int MaxCount;
    public int MinCount;
    public string Text;
    public Func<Card, bool> Validator;

    protected override void ResolveEffect()
    {
      Game.Enqueue<SelectCardsToUntap>(Controller,
        p =>
          {
            p.Validator = Validator;
            p.MinCount = MinCount;
            p.MaxCount = MaxCount;
            p.Text = FormatText(Text);
          }
        );
    }
  }
}