using System.Collections;
using System.Collections.Generic;
// using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// 물체의 생성에만 관여. 저장/로드는 Persistable Object. cs가 담당
public class Game : PersistableObject
{
    // 소환할 물체의 정보
    // 이제 PersistableObject형 prefab류 대신, ShapeFactory형 shapes를 사용
    public ShapeFactory shapeFactory;

    // 물체 생성에 해당하는 키 코드
    public KeyCode createKey = KeyCode.C;

    // 물체를 제거하는 키 코드
    public KeyCode destroyKey = KeyCode.X;

    // 게임을 새로 시작하는 키 코드
    public KeyCode newGameKey = KeyCode.N;

    // 정보를 저장하는 키 코드
    public KeyCode saveKey = KeyCode.S;

    // 저장한 정보를 로딩하는 키 코드
    public KeyCode loadKey = KeyCode.L;


    // 생성한 물체를 저장할 배열
    List<Shape> shapes;

    // transform 정보를 저장할 위치
    public PersistableStorage storage;

    // 저장 버전
    const int saveVersion = 2;

    // Creation의 속도
    public float CreationSpeed { get; set; }
    // creation의 진행 정도 (축적되는 값)
    float creationProgress=0f;

    // Destruction의 속도
    public float DestructionSpeed { get; set; }
    // destruction의 진행 정도 (축적되는 값)
    float destructionProgress=0f;

    // 전체 레벨의 수
    public int levelCount;

    // 지금 load가 되어있는 씬의 index
    int loadedLevelBuildIndex;

    // 물체가 생성되는 랜덤한 지점 (구하는 건 해당 스크립트가 해줌)
    public SpawnZone spawnZone;

    private void Start()
    {
        shapes = new List<Shape>();
        // persistent Data Path는 'file'이 아니라 'folder' 경로이다

        if(Application.isEditor)
        {

            for(int i=0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.name.Contains("Level "))
                {
                    // 확실하게 켜두기 위한 명령어
                    SceneManager.SetActiveScene(loadedScene);
                    // build setting상에서, 지금 로드된 씬의 index를 제공
                    loadedLevelBuildIndex = loadedScene.buildIndex;
                    // 이후 불필요한 추가적인 로드를 막음
                    return;
                }
            }
        }

        StartCoroutine(LoadLevel(1));
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
        else if(Input.GetKeyDown(destroyKey))
        {
            DestroyShape();
        }
        else if(Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
        }
        else if(Input.GetKeyDown(saveKey))
        {
            // Save();
            storage.Save(this, saveVersion);
        }
        else if(Input.GetKeyDown(loadKey))
        {
            // Load();
            BeginNewGame();
            storage.Load(this);
        }
        // 숫자키를 통해 레벨을 입력
        else
        {
            // 유효하지 않은 레벨을 걸러냄
            for(int i=1; i <= levelCount; i++)
            {
                if(Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    // 새로운 레벨이 로드되면, 게임도 리셋
                    BeginNewGame();
                    StartCoroutine(LoadLevel(i));
                    return;
                }
            }
        }

        creationProgress += Time.deltaTime * CreationSpeed;
        // progress가 1값에 도달할 때 마다 creation을 실행한다
        while(creationProgress >= 1f)
        {
            creationProgress -= 1f;
            CreateShape();
        }

        destructionProgress += Time.deltaTime * DestructionSpeed;
        while(destructionProgress >= 1f)
        {
            destructionProgress -= 1f;
            DestroyShape();
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

        // 반지름이 1인 구 범위 내.
        t.localPosition = spawnZone.SpawnPoint;
        // 랜덤한 쿼터니언 성분을 리턴
        t.localRotation = Random.rotation;
        // Random. Range는 float를 리턴하기 때문에, transform.scale로 쓰려면 Vector를 곱해야 한다
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);

        // 생성시 임의의 색상을 부여
        instance.SetColor(Random.ColorHSV(
            0f, 1f, .5f, 1f, .25f, 1f, 1f, 1f));
        // 배열에 추가
        shapes.Add(instance);
    }

    // objects -> shapes
    // 다시 게임을 시작(Clear 용도)
    void BeginNewGame()
    {
        // Debug.Log("New Game Starts");
        for(int i=0; i < shapes.Count; i++)
        {
            // 다시 가져오는 건 shapeFactory가 주체로 호출할 수 없다
            // Reclaim 기능이 생산이 아니라 기록 로드에 가까워서 그런건가?
            shapeFactory.Reclaim(shapes[i]);
        }

        // 배열의 내용은 한번에 정리
        shapes.Clear();
    }

    // objects -> shapes

    public override void Save(GameDataWriter writer)
    {
        // 저장 정보의 버전을 기록
        // writer.Write(-saveVersion);
        // 배열의 길이를 기록
        writer.Write(shapes.Count);
        // 마지막으로 로드되었던 레벨의 index까지 저장한다
        writer.Write(loadedLevelBuildIndex);

        for(int i=0; i < shapes.Count; i++)
        {
            // 해당 도형의 shape id 정보를 기록
            writer.Write(shapes[i].ShapeId);
            writer.Write(shapes[i].MaterialId);
            // writer 인자 정보에 각 shape들의 transform 정보를 넘김
            shapes[i].Save(writer);
        }
    }

    // objects -> shapes

    public override void Load(GameDataReader reader)
    {
        // 저장된 정보의 세이브 방식을 읽어옴
        int version = reader.Version;

        // version이 count의 정보를 받았다면, 해당 값은 음수가 된다
        // saveVersion의 초기화값은 1이 되므로, 그 경우 에러를 호출해야 한다
        if(version > saveVersion)
        {
            Debug.LogError("Unsupported future save version " + version);
            return;
        }

        // 배열의 길이를 읽어옴
        int count = version <= 0 ? -version : reader.ReadInt();
        // 버전이 2보다 낮아 레벨 로드 미지원인 경우, 디폴트로 레벨 1을 로드
        StartCoroutine(LoadLevel(version < 2 ? 1 : reader.ReadInt()));

        for(int i=0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactory.Get(shapeId, materialId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }

    void DestroyShape()
    {
        if(shapes.Count > 0)
        {
            int index = Random.Range(0, shapes.Count);
            // Destroy(shapes[index].gameObject);
            // 바로 파괴하는 대신, 재활용을 판단 및 처리
            shapeFactory.Reclaim(shapes[index]);

            int lastIndex = shapes.Count - 1;
            shapes[index] = shapes[lastIndex];

            shapes.RemoveAt(lastIndex);
        }
    }

    IEnumerator LoadLevel(int levelBuildIndex)
    {
        // 로딩되지 않은 scene에 대한 명령을 비활성화시키기 위함
        // 보통 여기서 로딩창을 띄운다
        enabled = false;

        // 0번 main을 제외한 다른 Scene을 로드하려고 한다면
        if(loadedLevelBuildIndex > 0)
        {
            // 지금 로드되어있는 레벨의 인덱스를 제거
            yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
        }

        // 로드 메소드를 실행하면서 코루틴도 돌리는 방법; 씬의 로딩이 끝날때까지를 측정
        yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);

        // 로드할 뿐 아니라, 해당 Scene이 active하도록 해야 한다
        // SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level 1"));
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
        // 새로운 씬이 성공적으로 로드될 때 마다, 몇 번째 index인지 업데이트
        loadedLevelBuildIndex = levelBuildIndex;

        enabled = true;
    }
}
