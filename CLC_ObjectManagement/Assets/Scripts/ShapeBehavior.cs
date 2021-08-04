using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// shpae의 행동 양식을 정해주는 클래스
public abstract class ShapeBehavior
#if UNITY_EDITOR
    : ScriptableObject
#endif
{
    // scriptable object 상속받은 건, hot reload에서 해당 객체들의 플러쉬를 막기 위함

#if UNITY_EDITOR
    // reclaim 여부를 표시
    public bool IsReclaimed { get; set; }

    private void OnEnable()
    {
        // stack에 원소가 있을 경우, 해당 pool의 파괴를 막음
        if(IsReclaimed)
        {
            Recycle();
        }
    }
#endif

    // 3.9 - 1.7에서 추가
    public enum ShapeBehaviorType
    {
        Movement,
        Rotation,
        Oscillation
    }

    public static class ShapeBehaviorTypeMethods
    {
        public static ShapeBehavior GetInstance (this ShapeBehaviorType type)
        {
            switch (type)
            {
                case ShapeBehaviorType.Movement:
                    return ShapeBehaviorPool<MovementShapeBehavior>.Get();
                case ShapeBehaviorType.Rotation:
                    return ShapeBehaviorPool<RotationShapeBehavior>.Get();
                case ShapeBehaviorType.Oscillation:
                    return ShapeBehaviorPool<OscillationShapeBehavior>.Get();
            }

            UnityEngine.Debug.Log("Forgot to support " + type);
            return null;
        }
    }

    public abstract ShapeBehaviorType BehaviorType { get; }

    // 추상 메소드의 선언문
    public abstract void GameUpdate(Shape shape);

    public abstract void Recycle();

    public abstract void Save(GameDataWriter writer);
    public abstract void Load(GameDataReader reader);
    
    
}
