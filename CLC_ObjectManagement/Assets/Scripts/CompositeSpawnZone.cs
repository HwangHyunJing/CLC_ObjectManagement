using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeSpawnZone : SpawnZone
{
    // 한 스크립트로 복수의 구역들을 생성
    [SerializeField]
    SpawnZone[] spawnZones;

    public override Vector3 SpawnPoint
    {
        get
        {
            // 여러 묶음 중 한 구획을 랜덤으로 지정
            int index = Random.Range(0, spawnZones.Length);
            return spawnZones[index].SpawnPoint;
        }
    }
}
