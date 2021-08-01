using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 레벨 데이터 자체를 저장하기 위해 Persistable Object를 상속 받음
public class GameLevel : PersistableObject
{
    [SerializeField]
    SpawnZone spawnZone;

    // 이후 추가될 요소들을 생각해서, 반드시 저장되어야 할 것들을 의미
    // 예제에서는 Spawn Zone을 넘김
    [SerializeField]
    PersistableObject[] persistableObjects;

    public static GameLevel Current { get; private set; }

    private void OnEnable()
    {
        // 현재의 레벨을 받아옴 (singleton?)
        Current = this;

        // 데이터가 없을 경우 null을 받으면 에러가 나므로, 빈 데이터 값으로 대체
        if(persistableObjects == null)
        {
            persistableObjects = new PersistableObject[0];
        }
    }

    private void Start()
    {
        // 만들어둔 get을 통해 Game 인스턴스의 성분을 가져옴 (singleton)
        // Game.Instance.SpawnZoneOfLevel1 = spawnZone;
    }


    // Game.cs가 현재의 레벨을 참조해서 호출 >> 타 코드로 이관
    public Shape SpawnShape()
    {
        return spawnZone.SpawnShape();
    }

    public override void Save (GameDataWriter writer)
    {
        // 늘 했듯이, 일단 배열의 길이를 저장
        writer.Write(persistableObjects.Length);
        for(int i=0; i < persistableObjects.Length; i++)
        {
            // 그리고 각 원소(persistable Object)를 기록해준다
            persistableObjects[i].Save(writer);
        }
    }

    public override void Load (GameDataReader reader)
    {
        int saveCount = reader.ReadInt();
        for(int i=0; i < saveCount; i++)
        {
            persistableObjects[i].Load(reader);
        }
    }
}
