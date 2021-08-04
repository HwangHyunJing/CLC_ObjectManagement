using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillationShapeBehavior : ShapeBehavior
{
    public override ShapeBehaviorType BehaviorType
    {
        get
        {
            return ShapeBehaviorType.Oscillation;
        }
    }

    public override void GameUpdate(Shape shape)
    {
        
    }

    public override void Save(GameDataWriter writer)
    {
        throw new System.NotImplementedException();
    }

    public override void Load(GameDataReader reader)
    {
        throw new System.NotImplementedException();
    }

    public override void Recycle()
    {
        ShapeBehaviorPool<OscillationShapeBehavior>.Reclaim(this);
    }
}
