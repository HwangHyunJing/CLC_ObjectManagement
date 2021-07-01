using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObject
{
    // prefab 물체 도형의 종류를 정의하는 id
    int shapeId = int.MinValue;


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


    // Shape가 자신의 material 정보를 바꾸는 것을 막기 위해 set을 private으로 설정
    public int MaterialId { get; private set; }

    // 위에 setter의 기능을 대신하는 메소드
    public void SetMaterial(Material material, int materialId)
    {
        GetComponent<MeshRenderer>().material = material;
        MaterialId = materialId;
    }
}
