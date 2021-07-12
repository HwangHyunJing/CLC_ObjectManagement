using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawnZone : SpawnZone
{
    [SerializeField]
    bool surfaceOnly;

    public override Vector3 SpawnPoint
    {
        get
        {
            Vector3 p;
            p.x = Random.Range(-.5f, .5f);
            p.y = Random.Range(-.5f, .5f);
            p.z = Random.Range(-.5f, .5f);

            if(surfaceOnly)
            {
                // random 대신에 양 극단의 값만 사용하도록 함
                int axis = Random.Range(0, 3);
                p[axis] = p[axis] < 0f ? -.5f : .5f;
            }

            return transform.TransformPoint(p);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        // 해당 스크립트를 지닌 물체의 transform 성분을 가져옴
        Gizmos.matrix = transform.localToWorldMatrix;
        // 중심부는 0, 크기는 1
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
