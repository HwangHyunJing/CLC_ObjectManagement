using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// abstract 클래스는 자신의 객체를 생성할 수 없습니다
public abstract class SpawnZone : MonoBehaviour
{
    public abstract Vector3 SpawnPoint { get; }

}
