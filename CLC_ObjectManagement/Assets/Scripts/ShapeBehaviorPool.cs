using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeBehaviorPool<T> where T : ShapeBehavior, new()
{
    static Stack<T> stack = new Stack<T>();

    public static T Get()
    {
        // 목록이 존재하면 가장 위에있는 행동을 리턴
        if(stack.Count > 0)
        {
            T behavior = stack.Pop();

#if UNITY_EDITOR
            behavior.IsReclaimed = false;
#endif

            return behavior;
        }

#if UNITY_EDITOR
        // 없다면 새로운 stack을 생성
        return ScriptableObject.CreateInstance<T>();
#else
        return new T();
#endif
    }

    
    public static void Reclaim (T behavior)
    {
        // reclaim이 recycle입니다... 넣는 게 맞음
#if UNITY_EDITOR
        behavior.IsReclaimed = true;
#endif
        stack.Push(behavior);
    }
}
