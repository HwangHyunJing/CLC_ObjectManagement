using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// shpae의 행동 양식을 정해주는 클래스
public abstract class ShapeBehavior : MonoBehaviour
{
    // 추상 메소드의 선언문
    public abstract void GmaeUpdate(Shape shape);

    public abstract void Save(GameDataWriter writer);
    public abstract void Load(GameDataReader reader);
    
}
