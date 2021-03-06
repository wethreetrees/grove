﻿namespace Grove.AI.CostRules
{
  using System.Linq;

  public class XIsOpponentsLandCount : CostRule
  {
    public override int CalculateX(CostRuleParameters p)
    {
      return p.Controller.Opponent.Battlefield.Lands.Count();
    }
  }
}