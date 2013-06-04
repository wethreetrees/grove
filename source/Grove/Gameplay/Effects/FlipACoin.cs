﻿namespace Grove.Gameplay.Effects
{
  using Messages;

  public class FlipACoinReturnToHand : Effect
  {
    protected override void ResolveEffect()
    {
      var hasWon = FlipACoin();

      Publish(new PlayerHasFlippedACoin
        {
          Player = Controller,
          HasWon = hasWon
        });


      if (hasWon)
        return;

      Source.OwningCard.PutToHand();
    }
  }
}