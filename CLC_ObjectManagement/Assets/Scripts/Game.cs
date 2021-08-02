using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

// 물체의 생성에만 관여. 저장/로드는 Persistable Object. cs가 담당
public class Game : PersistableObject
{
    // 소환할 물체의 정보
    // 이제 PersistableObject형 prefab류 대신, ShapeFactory형 shapes를 사용
    [SerializeField]
    ShapeFactory[] shapeFactories;

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
    [SerializeField]
    PersistableStorage storage;

    // 저장 버전
    [SerializeField]
    const int saveVersion = 6;

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

    // 완전한 랜덤이 아니라, 메인 state를 기반으로 한 random(이 무슨)
    Random.State mainRandomState;

    // Load를 할 때 seed를 다시 설정할지 여부
    [SerializeField]
    bool reseedOnLoad;

    // 변화된 값을 slider에 적용시켜주기 위한 참조. 그냥 inspector상에서 넘겨줌
    [SerializeField]
    Slider creationSpeedSlider;
    [SerializeField]
    Slider destructionSpeedSlider;

    // 물체가 생성되는 랜덤한 지점 (구하는 건 해당 스크립트가 해줌)
    public SpawnZone SpawnZoneOfLevel1 { get; set; }

    // public static Game Instance { get; private set; }

    private void OnEnable()
    {
        // hot reload 직후 on enabled가 다시 호출되면서 id가 갱신되는 현상을 막음
        if(shapeFactories[0].FactoryId != 0)
        {
            for (int i = 0; i < shapeFactories.Length; i++)
            {
                shapeFactories[i].FactoryId = i;
            }
        }
        
    }

    private void Start()
    {
        // 코드 시작시 일단 랜덤 상태를 부여받음
        mainRandomState = Random.state;

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

        BeginNewGame();
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
            StartCoroutine(LoadLevel(loadedLevelBuildIndex));
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

        
    }

    private void FixedUpdate()
    {
        // 생성 및 파괴의 경우 프레임에 영향을 받지 않도록 하기 위해 Fixed가 담당

        for(int i=0; i < shapes.Count; i++)
        {
            shapes[i].GameUpdate();
        }

        creationProgress += Time.deltaTime * CreationSpeed;
        // progress가 1값에 도달할 때 마다 creation을 실행한다
        while (creationProgress >= 1f)
        {
            creationProgress -= 1f;
            CreateShape();
        }

        destructionProgress += Time.deltaTime * DestructionSpeed;
        while (destructionProgress >= 1f)
        {
            destructionProgress -= 1f;
            DestroyShape();
        }
    }

    // 물체를 임의의 지점에 생성하는 메소드
    // 의미를 확실하게 하기 위해 o 대신 instance로 단어를 변화
    void CreateShape()
    {
        // 물체의 생성 자체를 spawn쪽 코드가 직접 리턴 하면서, instance가 필요 없어진다
        // Shape instance = shapeFactory.GetRandom();

        // GameLevel.Current.ConfigureSpawn(instance);
        GameLevel.Current.SpawnShape();

        // 배열에 추가
        shapes.Add(GameLevel.Current.SpawnShape());
    }

    // 다시 게임을 시작(Clear 용도)
    void BeginNewGame()
    {
        // 새 게임을 시작하면 메인 rdm 상태를 주고
        Random.state = mainRandomState;
        // 시간에 따른 시드값을 새로 생성
        int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledTime;

        mainRandomState = Random.state;

        // 메인 state와 새로운 seed를 이용해 새로운 state를 만든다
        Random.InitState(seed);

        // 슬라이더와 값의 데이터 초기화 (슬라이더는 serialize 방식으로 참조)
        creationSpeedSlider.value = CreationSpeed = 0f;
        destructionSpeedSlider.value = DestructionSpeed = 0f;

        // Debug.Log("New Game Starts");
        for(int i=0; i < shapes.Count; i++)
        {
            // 다시 가져오는 건 shapeFactory가 주체로 호출할 수 없다
            // Reclaim 기능이 생산이 아니라 기록 로드에 가까워서 그런건가?
            // shapeFactory.Reclaim(shapes[i]);
            shapes[i].Recycle();
        }

        // 배열의 내용은 한번에 정리
        shapes.Clear();
    }

    void DestroyShape()
    {
        if (shapes.Count > 0)
        {
            int index = Random.Range(0, shapes.Count);
            // Destroy(shapes[index].gameObject);
            // 바로 파괴하는 대신, 재활용을 판단 및 처리
            // shapeFactory.Reclaim(shapes[index]);
            shapes[index].Recycle();

            int lastIndex = shapes.Count - 1;
            shapes[index] = shapes[lastIndex];

            shapes.RemoveAt(lastIndex);
        }
    }


    // objects -> shapes

    public override void Save(GameDataWriter writer)
    {
        // 저장 정보의 버전은 다른 곳에서 먼저 기록합니다

        // 배열의 길이를 기록
        writer.Write(shapes.Count);
        // 랜덤에 대한 정보를 기록
        writer.Write(Random.state);
        // 생성 및 파괴 값을 저장
        writer.Write(CreationSpeed);
        writer.Write(creationProgress);
        writer.Write(DestructionSpeed);
        writer.Write(destructionProgress);
        // 마지막으로 로드되었던 레벨의 index까지 저장한다
        writer.Write(loadedLevelBuildIndex);
        // 레벨 자체에 대한 정보를 기록
        GameLevel.Current.Save(writer);

        for(int i=0; i < shapes.Count; i++)
        {
            writer.Write(shapes[i].OriginFactory.FactoryId);
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
        if (version > saveVersion)
        {
            Debug.LogError("Unsupported future save version " + version);
            return;
        }
        StartCoroutine(LoadGame(reader));

    }



    // 게임을 로드할 때 기능이 너무 많아서 분리
    IEnumerator LoadGame (GameDataReader reader)
    {
        // 버전 확인 우선
        int version = reader.Version;

        // 배열의 길이를 읽어옴
        int count = version <= 0 ? -version : reader.ReadInt();

        // seed 및 state관련 저장 여부를 검사
        if (version >= 3)
        {
            // 기존에 저장된 seed값을 읽어들임
            Random.State state = reader.ReadRandomState();

            // load에서 시드 재설정을 원하지 않는다면
            if (!reseedOnLoad)
            {
                // 로드된 seed 상태를 사용
                Random.state = state;
            }

            creationSpeedSlider.value = CreationSpeed = reader.ReadFloat();
            creationProgress = reader.ReadFloat();
            destructionSpeedSlider.value = DestructionSpeed = reader.ReadFloat();
            destructionProgress = reader.ReadFloat();
        }

        // 버전이 2보다 낮아 레벨 로드 미지원인 경우, 디폴트로 레벨 1을 로드
        yield return LoadLevel(version < 2 ? 1 : reader.ReadInt());

        // 레벨 자체의 데이터까지 지원하는 버전의 경우, 해당 정보를 로드
        if(version >= 3)
        {
            GameLevel.Current.Load(reader);
        }

        for (int i = 0; i < count; i++)
        {
            int factoryId = version >= 5 ? reader.ReadInt() : 0;
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactories[factoryId].Get(shapeId, materialId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }

    IEnumerator LoadLevel(int levelBuildIndex)
    {
        // 로딩되지 않은 scene에 대한 명령을 비활성화시키기 위함
        // 보통 여기서 로딩창을 띄운다
        enabled = false;

        // 자신의 현재 scene을 종료 (0 제외)
        if (loadedLevelBuildIndex > 0)
        {
            Debug.Log("previous: " + loadedLevelBuildIndex + ", now: " + levelBuildIndex);

            // 지금 로드되어있는 레벨의 인덱스를 제거
            yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
        }

        // 로드 메소드를 실행하면서 코루틴도 돌리는 방법; 씬의 로딩이 끝날때까지를 측정
        yield return SceneManager.LoadSceneAsync(
            levelBuildIndex, LoadSceneMode.Additive);

        // 로드할 뿐 아니라, 해당 Scene이 active하도록 해야 한다
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
        // 새로운 씬이 성공적으로 로드될 때 마다, 몇 번째 index인지 업데이트
        loadedLevelBuildIndex = levelBuildIndex;

        enabled = true;
    }
}
