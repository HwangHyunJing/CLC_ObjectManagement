using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : MonoBehaviour
{
    [SerializeField]
    SpawnZone spawnZone;

    private void Start()
    {
        // 만들어둔 get을 통해 Game 인스턴스의 성분을 가져옴 (singleton)
        Game.Instance.SpawnZoneOfLevel1 = spawnZone;
    }

}
