﻿namespace Grove.Core.Ai
{
  public interface ISearchNode
  {
    Game Game { get; }
    Player Controller { get; }
    int ResultCount { get; }
    void SetResult(int index);

    void GenerateChoices();
  }
}