﻿namespace Grove.Core.Controllers.Machine
{
  using System.Linq;
  using Ai;

  public class DeclareBlockers : Controllers.DeclareBlockers
  {
    protected override void ExecuteQuery()
    {
      Result = new BlockStrategy
        (
        Game.Combat.Attackers.Select(x => x.Card),
        Controller.Battlefield.CreaturesThatCanBlock,
        Controller.Life
        );
    }
  }
}