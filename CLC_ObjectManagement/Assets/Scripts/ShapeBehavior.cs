using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// shpae의 행동 양식을 정해주는 클래스
public abstract class ShapeBehavior : MonoBehaviour
{
    // 3.9 - 1.7에서 추가
    public enum ShapeBehaviorType
    {
        Movement,
        Rotation
    }

    public abstract ShapeBehaviorType BehaviorType { get; }

    // 추상 메소드의 선언문
    public abstract void GameUpdate(Shape shape);



    public abstract void Save(GameDataWriter writer);
    public abstract void Load(GameDataReader reader);
    
}
