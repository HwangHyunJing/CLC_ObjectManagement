using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    // 소환할 물체의 정보
    public Transform prefab;

    // 물체 생성에 해당하는 키 코드
    public KeyCode createKey = KeyCode.C;

    // 게임을 새로 시작하는 키 코드
    public KeyCode newGameKey = KeyCode.N;

    private void Update()
    {
        if(Input.GetKeyDown(createKey))
        {
            // Instantiate(prefab);
            CreateObject();
        }
        // 한 번에 여러 키가 입력되는 것을 막기 위해 else-if 구문으로 묶음
        else if(Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
        }
    }

    // 물체를 임의의 지점에 생성하는 메소드
    void CreateObject()
    {
        Transform t = Instantiate(prefab);

        // 반지름이 1인 구 범위 내. 5를 곱해 범위를 넓힘
        t.localPosition = Random.insideUnitSphere * 5f;
        // 랜덤한 쿼터니언 성분을 리턴
        t.localRotation = Random.rotation;
        // Random. Range는 float를 리턴하기 때문에, transform.scale로 쓰려면 Vector를 곱해야 한다
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
    }

    void BeginNewGame()
    {
        Debug.Log("New Game Starts");
    }
}
