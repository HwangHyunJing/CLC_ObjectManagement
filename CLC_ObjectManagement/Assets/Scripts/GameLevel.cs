using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 레벨 데이터 자체를 저장하기 위해 Persistable Object를 상속 받음
public class GameLevel : PersistableObject
{
    [SerializeField]
    SpawnZone spawnZone;

    public static GameLevel Current { get; private set; }

    private void OnEnable()
    {
        // 현재의 레벨을 받아옴 (singleton?)
        Current = this;
    }

    private void Start()
    {
        // 만들어둔 get을 통해 Game 인스턴스의 성분을 가져옴 (singleton)
        // Game.Instance.SpawnZoneOfLevel1 = spawnZone;
    }

    public Vector3 SpawnPoint
    {
        // 기존에 setter 방식 대신에 getter로 넘기게 된다
        get
        {
            return spawnZone.SpawnPoint;
        }
    }

    public override void Save (GameDataWriter write)
    {

    }

    public override void Load (GameDataReader reader)
    {

    }
}
