using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObject
{
    // 외부에서 해당 성분에 접근하기 위한 조치
    public int ShapeId
    {
        get { return shapeId;   }
        set
        {
            if (shapeId == int.MinValue && value != int.MinValue)
            {
                shapeId = value;
            }
            else
            {
                Debug.LogError("Not allowed to cahnge shapeId.");
            }
        }
    }

    // prefab 물체 도형의 종류를 정의하는 id
    int shapeId = int.MinValue;
}
