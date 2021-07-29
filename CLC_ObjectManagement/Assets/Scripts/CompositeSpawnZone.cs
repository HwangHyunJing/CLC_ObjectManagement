using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeSpawnZone : SpawnZone
{
    // 한 스크립트로 복수의 구역들을 생성
    [SerializeField]
    SpawnZone[] spawnZones;

    // 배정된 각 공간을 '순서대로' 돌며 생성할지 여부
    [SerializeField]
    bool sequential;

    int nextSequentialIndex;

    // Spawn Zone에서 상속받은 속성
    [SerializeField]
    bool overrideConfig;

    public override Vector3 SpawnPoint
    {
        get
        {
            // 여러 묶음 중 한 구획을 랜덤으로 지정
            int index;

            if(sequential)
            {
                // 시퀀셜이 켜져있다면, 무조건 다음 순서의 index를 가져온다
                index = nextSequentialIndex++;

                // 인덱스가 끝까지 다다랐다면 다시 처음으로 돌린다
                if(nextSequentialIndex >= spawnZones.Length)
                {
                    // 우리가 사용할 값은 index이므로, 0이 되는 건 next Sequential Index가 맞다
                    nextSequentialIndex = 0;
                }
            }
            else
            {
                index = Random.Range(0, spawnZones.Length);
            }

            return spawnZones[index].SpawnPoint;
        }
    }

    public override void ConfigureSpawn(Shape shape)
    {
        if(overrideConfig)
        {
            // 하나의 configuration을 일괄적으로 받는 경우 (개별 설정의 번거로움 감소)
            base.ConfigureSpawn(shape);
        }
        else
        {
            int index;
            if (sequential)
            {
                index = nextSequentialIndex++;
                if (nextSequentialIndex >= spawnZones.Length)
                {
                    nextSequentialIndex = 0;
                }
            }
            else
            {
                index = Random.Range(0, spawnZones.Length);
            }
            spawnZones[index].ConfigureSpawn(shape);
        }   
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(nextSequentialIndex);
    }

    public override void Load(GameDataReader reader)
    {
        nextSequentialIndex = reader.ReadInt();
    }
}
