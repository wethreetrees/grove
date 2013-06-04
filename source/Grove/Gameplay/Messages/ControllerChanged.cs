﻿namespace Grove.Gameplay.Messages
{
  public class ControllerChanged
  {
    public ControllerChanged(Card card)
    {
      Card = card;
    }

    public Card Card { get; private set; }
  }
}