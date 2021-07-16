using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : PersistableObject
{
    [SerializeField]
    Vector3 angularVelocity;

    private void FixedUpdate()
    {
        // 물체의 생성/파괴가 Fixed 기반이므로, 스폰 구역의 움직임까지도 Fixed로 넣었다
        transform.Rotate(angularVelocity * Time.deltaTime);
        
    }

}
