using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// abstract 클래스는 자신의 객체를 생성할 수 없습니다
public abstract class SpawnZone : PersistableObject
{
    // zone에 대한 정보 - 가령 next sequential index 라던가 - 를 저장하기 위해 P.O.를 상속

    public abstract Vector3 SpawnPoint { get; }

    
}
