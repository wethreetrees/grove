﻿namespace Grove.Gameplay.Effects
{
  using Decisions;
  using Decisions.Results;
  using ManaHandling;

  public class PayManaOrLooseLife : Effect, IProcessDecisionResults<BooleanResult>
  {
    private readonly int _lifeAmount;
    private readonly IManaAmount _manaAmount;
    private readonly DynParam<Player> _player;

    private PayManaOrLooseLife() {}

    public PayManaOrLooseLife(int lifeAmount, IManaAmount manaAmount, DynParam<Player> player)
    {
      _lifeAmount = lifeAmount;
      _manaAmount = manaAmount;
      _player = player;

      RegisterDynamicParameters(player);
    }

    public void ProcessResults(BooleanResult results)
    {
      if (results.IsTrue)
        return;

      _player.Value.Life -= _lifeAmount;
    }

    protected override void ResolveEffect()
    {
      Enqueue<PayOr>(Controller, p =>
        {
          p.ManaAmount = _manaAmount;
          p.Text = "Pay mana?";
          p.ProcessDecisionResults = this;
        });
    }
  }
}