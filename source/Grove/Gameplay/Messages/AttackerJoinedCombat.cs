﻿namespace Grove.Gameplay.Messages
{
  public class AttackerJoinedCombat
  {
    public Attacker Attacker { get; set; }
    public bool WasDeclared { get; set; }
  }
}