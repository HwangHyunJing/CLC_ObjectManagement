using System.Collections;
using System.Collections.Generic;
// using System.IO;
using UnityEngine;

// 물체의 생성에만 관여. 저장/로드는 Persistable Object. cs가 담당
public class Game : PersistableObject
{
    // 소환할 물체의 정보
    // public Transform prefab;
    // public PersistableObject prefab;
    // 이제 PersistableObject형 prefab류 대신, ShapeFactory형 shapes를 사용
    public ShapeFactory shapeFactory;

    // 물체 생성에 해당하는 키 코드
    public KeyCode createKey = KeyCode.C;

    // 게임을 새로 시작하는 키 코드
    public KeyCode newGameKey = KeyCode.N;

    // 정보를 저장하는 키 코드
    public KeyCode saveKey = KeyCode.S;

    // 저장한 정보를 로딩하는 키 코드
    public KeyCode loadKey = KeyCode.L;

    // 생성한 물체를 저장할 배열
    // List<Transform> objects;
    List<Shape> shapes;

    // transform 정보를 저장할 위치
    // string savePath;
    public PersistableStorage storage;

    private void Awake()
    {
        shapes = new List<Shape>();
        // persistent Data Path는 'file'이 아니라 'folder' 경로이다
        // savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }

    private void Update()
    {
        if(Input.GetKeyDown(createKey))
        {
            // Instantiate(prefab);
            // CreateObject();
            CreateShape();
        }
        // 한 번에 여러 키가 입력되는 것을 막기 위해 else-if 구문으로 묶음
        else if(Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
        }
        else if(Input.GetKeyDown(saveKey))
        {
            // Save();
            storage.Save(this);
        }
        else if(Input.GetKeyDown(loadKey))
        {
            // Load();
            BeginNewGame();
            storage.Load(this);
        }
    }

    // 물체를 임의의 지점에 생성하는 메소드
    // 의미를 확실하게 하기 위해 o 대신 instance로 단어를 변화
    void CreateShape()
    {
        // Transform t = Instantiate(prefab);
        // PersistableObject o = Instantiate(prefab);
        Shape instance = shapeFactory.GetRandom();
        Transform t = instance.transform;

        // 반지름이 1인 구 범위 내. 5를 곱해 범위를 넓힘
        t.localPosition = Random.insideUnitSphere * 5f;
        // 랜덤한 쿼터니언 성분을 리턴
        t.localRotation = Random.rotation;
        // Random. Range는 float를 리턴하기 때문에, transform.scale로 쓰려면 Vector를 곱해야 한다
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);

        // 배열에 추가
        // objects.Add(o);
        shapes.Add(instance);
    }

    // objects -> shapes
    // 다시 게임을 시작(Clear 용도)
    void BeginNewGame()
    {
        // Debug.Log("New Game Starts");
        for(int i=0; i < shapes.Count; i++)
        {
            // 일단 물체를 제거
            Destroy(shapes[i].gameObject);
        }

        // 배열의 내용은 한번에 정리
        shapes.Clear();
    }

    // objects -> shapes

    public override void Save(GameDataWriter writer)
    {
        // 배열의 길이를 기록
        writer.Write(shapes.Count);
        for(int i=0; i < shapes.Count; i++)
        {
            shapes[i].Save(writer);
        }
    }

    // objects -> shapes

    public override void Load(GameDataReader reader)
    {
        int count = reader.ReadInt();
        for(int i=0; i < count; i++)
        {
            Shape o = shapeFactory.GetRandom();
            o.Load(reader);
            shapes.Add(o);
        }
    }

}
