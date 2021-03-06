﻿using Grove.AI;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Grove
{
  public class Settings
  {
    private static Settings _global;
        
    // This should be used only when access to Game context is not possible.
    public static Settings Readonly
    {
      get
      {
        if (_global == null)
        {
          _global = Settings.Load();
        }
        
        return _global;
      }
    }

    public string YourName = "You";
    public bool FullScreen = true;
    
    public int BasicLandVersions = 4;
    
    public string DefaultStarterPack = "M15";
    public string DefaultBoosterPack1 = "M15";
    public string DefaultBoosterPack2 = "M15";
    public string DefaultBoosterPack3 = "M15";

    public AiConfiguration Ai = new AiConfiguration
    {
      SearchDepth = 60,
      TargetCount = 2,
      Strategy = MultithreadStrategy.MultiThreaded2
    };

    public class AiConfiguration
    {
      public int SearchDepth;
      public int TargetCount;
      public MultithreadStrategy Strategy;
    }

    public enum MultithreadStrategy
    {
      SingleThreaded,
      MultiThreaded1,
      MultiThreaded2
    }   
    
    public SearchParameters GetSearchParameters()
    {
      return new SearchParameters(Ai.SearchDepth, Ai.TargetCount, GetPartitioningStrategy());
    }

    private SearchPartitioningStrategy GetPartitioningStrategy()
    {
#if DEBUG
      return SearchPartitioningStrategies.SingleThreaded;
#else
      switch (Ai.Strategy)
      {
        case (MultithreadStrategy.MultiThreaded1):
          return SearchPartitioningStrategies.MultiThreaded1;
        case (MultithreadStrategy.MultiThreaded2):
          return SearchPartitioningStrategies.MultiThreaded2;
      }
      return SearchPartitioningStrategies.SingleThreaded;
#endif
    }

    public static Settings Load()
    {
      const string filename = "settings.json";

      if (File.Exists(filename))
      {
        return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(filename));
      }

      return new Settings();
    }
  }
}