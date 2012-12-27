﻿namespace Grove.Core.Cards.Effects
{
  using System;
  using Decisions;
  using Decisions.Results;
  using Zones;

  public class SearchLibraryPutToHand : Effect, IProcessDecisionResults<ChosenCards>
  {
    public int MaxCount;
    public int MinCount;
    public bool DiscardRandomCardAfterwards;
    public string Text;
    public Func<Card, bool> Validator = delegate { return true; };

    public void ResultProcessed(ChosenCards results)
    {                       
      if (DiscardRandomCardAfterwards)
      {
        Controller.DiscardRandomCard();
      }

      Controller.ShuffleLibrary();
    }

    protected override void ResolveEffect()
    {
      Controller.RevealLibrary();
      
      Game.Enqueue<SelectCardsPutToHand>(
        controller: Controller,
        init: p =>
          {
            p.MinCount = MinCount;
            p.MaxCount = MaxCount;
            p.Validator = Validator;
            p.Zone = Zone.Library;
            p.Text = FormatText(Text);
            p.AiOrdersByDescendingScore = true;
            p.ProcessDecisionResults = this;
          });
    }
  }
}