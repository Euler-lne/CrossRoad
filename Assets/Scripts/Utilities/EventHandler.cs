using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class EventHandler
{
    public static event Func<float, Settings.TerrainType> FrogJumped;
    public static Settings.TerrainType CallFrogJumped(float y)
    {
        return (Settings.TerrainType)(FrogJumped?.Invoke(y));
    }
    public static event Action<int> FrogJumpSucceed;
    public static void CallFrogJunpSucceed(int score)
    {
        FrogJumpSucceed?.Invoke(score);
    }
    public static event Action FrogDead;
    public static void CallFrogDead()
    {
        FrogDead?.Invoke();
    }

}