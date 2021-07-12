using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SpawnZone : MonoBehaviour
{
    // 랜덤 구역의 표면에서만 생성되도록 하는 옵션
    [SerializeField]
    bool surfaceOnly;

    public Vector3 SpawnPoint
    {
        get
        {
            // 자기 자신의 transformation local 값을 world 상에서 쓰도록 함
            // 즉, 입력을 inspector의 본인 transform 성분으로 하는 방식
            return transform.TransformPoint(
                surfaceOnly ? Random.onUnitSphere : Random.insideUnitSphere);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        // gizmo가 Spawn Zone 오브젝트의 transform 성분을 따르도록 함
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, 1f);
    }
}
