using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class Game : MonoBehaviour
{
    // 소환할 물체의 정보
    public Transform prefab;

    // 물체 생성에 해당하는 키 코드
    public KeyCode createKey = KeyCode.C;

    // 게임을 새로 시작하는 키 코드
    public KeyCode newGameKey = KeyCode.N;

    // 정보를 저장하는 키 코드
    public KeyCode saveKey = KeyCode.S;

    // 저장한 정보를 로딩하는 키 코드
    public KeyCode loadKey = KeyCode.L;

    // 생성한 물체를 저장할 배열
    List<Transform> objects;

    // transform 정보를 저장할 위치
    string savePath;

    private void Awake()
    {
        objects = new List<Transform>();
        // persistent Data Path는 'file'이 아니라 'folder' 경로이다
        savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }

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
        else if(Input.GetKeyDown(saveKey))
        {
            Save();
        }
        else if(Input.GetKeyDown(loadKey))
        {
            Load();
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

        // 배열에 추가
        objects.Add(t);
    }

    // 다시 게임을 시작(Clear 용도)
    void BeginNewGame()
    {
        // Debug.Log("New Game Starts");
        for(int i=0; i < objects.Count; i++)
        {
            // 일단 물체를 제거
            Destroy(objects[i].gameObject);
        }

        // 배열의 내용은 한번에 정리
        objects.Clear();
    }

    void Save()
    {
        // 내용 기록을 위한 open
        using (
            // savePath 위치에 파일을 생성/기록하며, writer에 이진 방식으로 접근하겠다
            var writer = new BinaryWriter(File.Open(savePath, FileMode.Create))
            )
        {
            // 우선 원소의 수를 기록
            writer.Write(objects.Count);

            for(int i=0; i < objects.Count; i++)
            {
                Transform t = objects[i];
                writer.Write(t.localPosition.x);
                writer.Write(t.localPosition.y);
                writer.Write(t.localPosition.z);
            }
        }
        
    }

    void Load()
    {
        // 일단 기본적으로 새 게임을 시작해준다
        BeginNewGame();

        using(
            var reader = new BinaryReader(File.Open(savePath, FileMode.Open))
            )
        {
            // 4바이트만큼을 읽습니다
            int count = reader.ReadInt32();

            for(int i=0; i<count; i++)
            {
                // count와는 다르게, transform의 각 값은 float이므로 Read Single을 사용

                Vector3 p;
                p.x = reader.ReadSingle();
                p.y = reader.ReadSingle();
                p.z = reader.ReadSingle();

                Transform t = Instantiate(prefab);
                t.localPosition = p;
                objects.Add(t);
            }
        }
    }
}
