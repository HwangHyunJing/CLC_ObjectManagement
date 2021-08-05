using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillationShapeBehavior : ShapeBehavior
{
    public Vector3 Offset { get; set; }
    public float Frequency { get; set; }

    // 이전 진동값을 기억했다가, 새롭게 구한 값에서 제함
    float previousOscillation;

    public override ShapeBehaviorType BehaviorType
    {
        get
        {
            return ShapeBehaviorType.Oscillation;
        }
    }

    public override void GameUpdate(Shape shape)
    {
        // 쉽게 말해서 2 파이 R (주파수 X 시간 = 파장; 이동 거리; 반지름)
        float oscillation = Mathf.Sin(2f * Mathf.PI * Frequency * Time.time);
        shape.transform.localPosition +=
            (oscillation - previousOscillation) * Offset;
        previousOscillation = oscillation;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(Offset);
        writer.Write(Frequency);
        writer.Write(previousOscillation);
    }

    public override void Load(GameDataReader reader)
    {
        Offset = reader.ReadVector3();
        Frequency = reader.ReadFloat();
        previousOscillation = reader.ReadFloat();
    }

    public override void Recycle()
    {
        // 새로 행동을 부르면서 값을 초기화
        previousOscillation = 0f;
        ShapeBehaviorPool<OscillationShapeBehavior>.Reclaim(this);
    }
}
