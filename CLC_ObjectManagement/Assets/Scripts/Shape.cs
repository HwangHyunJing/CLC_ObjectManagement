using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Shape : PersistableObject
{
    // prefab 물체 도형의 종류를 정의하는 id
    int shapeId = int.MinValue;

    // 물체의 색상 정보를 빠르게 트레킹하기 위함
    // Color color; >> colors가 대체
    // MeshRenderer meshRenderer; >> meshRendereres가 대체

    // 자식 shape가 있는 경우 이에 접근하기 위함
    [SerializeField]
    MeshRenderer[] meshRenderers;

    Color[] colors;

    // property block 사용을 위한 정적 타입
    static int colorPropertyId = Shader.PropertyToID("_Color");

    // 사용할 프로퍼티 블락
    static MaterialPropertyBlock shaderPropertyBlock;

    List<ShapeBehavior> behaviorList = new List<ShapeBehavior>();



    private void Awake()
    {
        // 하나의 shape 스크립트는 단일 shape 객체만 관리
        // 전체 mesh의 수와 일치하게 색상 블락 array를 만듦
        colors = new Color[meshRenderers.Length];
    }

    // 별도의 스크립트에서 담당하면서 해당 데이터를 필요 없어짐
    // public Vector3 AngularVelocity { get; set; }
    // public Vector3 Velocity { get; set; }
    
    // private void FixedUpdate()
    public void GameUpdate()
    {
        // behavior 목록만큼 행동을 하도록 함
        for(int i = 0; i < behaviorList.Count; i++)
        {
            behaviorList[i].GameUpdate(this);
        }
    }

    // 외부에서 해당 성분에 접근하기 위한 조치
    public int ShapeId
    {
        get { return shapeId;   }
        set
        {
            if (shapeId == int.MinValue && value != int.MinValue)
            {
                shapeId = value;
            }
            else
            {
                Debug.LogError("Not allowed to cahnge shapeId.");
            }
        }
    }

    // 한 씬에 존재하는 여러 factory중에 자신이 속해있던 factory에 대한 정보
    public ShapeFactory OriginFactory
    {
        get
        {
            return originFactory;
        }
        set
        {
            if (originFactory == null)
            {
                originFactory = value;
            }
            else
            {
                Debug.LogError("Not allowed to change origin factory.");
            }
        }
    }

    ShapeFactory originFactory;

    // Shape가 자신의 material 정보를 바꾸는 것을 막기 위해 set을 private으로 설정
    public int MaterialId { get; private set; }


    // 위에 setter의 기능을 대신하는 메소드
    public void SetMaterial(Material material, int materialId)
    {
        // meshRenderer.material = material;
        for(int i=0; i<meshRenderers.Length; i++)
        {
            meshRenderers[i].material = material;
        }
        MaterialId = materialId;
    }

    public void SetColor (Color color)
    {
        // this.color = color;

        // 직접적인 material 변경(카피)를 막음
        // var propertyBlock = new MaterialPropertyBlock();
        if(shaderPropertyBlock == null)
        {
            // Material Property Block은 static 방식의 초기화가 안된다
            // static 방식의 초기화는 위에 colorPropertyId를 보면 된다
            // 그래서 이렇게 하는 거
            shaderPropertyBlock = new MaterialPropertyBlock();
        }
        shaderPropertyBlock.SetColor(colorPropertyId, color);
        
        for(int i=0; i<meshRenderers.Length; i++)
        {
            colors[i] = color;
            meshRenderers[i].SetPropertyBlock(shaderPropertyBlock);

        }

    }

    // 도형의 정보에 대해 저장할 것들이 생기면, 여기서 하면 된다

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        // 저장하려는 색상의 수 자체를 받음
        writer.Write(colors.Length);
        for(int i = 0; i < colors.Length; i++)
        {
            writer.Write(colors[i]);
        }
        // writer.Write(AngularVelocity);
        // writer.Write(Velocity);

        // 저장된 행동의 수를 우선 읽고
        writer.Write(behaviorList.Count);
        for(int i = 0; i < behaviorList.Count; i++)
        {
            // float값이 아니라 enum 데이터이므로 int 캐스팅이 맞다
            writer.Write((int)behaviorList[i].BehaviorType);
            behaviorList[i].Save(writer);
        }
    }

    public override void Load (GameDataReader reader)
    {
        base.Load(reader);

        // 복합체의 색상까지 로드하는 버전(5) 이라면
        if(reader.Version >= 5)
        {
            LoadColors(reader);
        }
        // 아닌 경우는 스크립트를 지닌 부모 객체에 대해서만 색상을 로드한다
        else
        {
            SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
        }
 
        // AngularVelocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
        // Velocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
        
        // 6 이상의 버전에서는 각 shape의 행동 양식을 저장
        if(reader.Version >= 6)
        {
            int behaviorCount = reader.ReadInt();
            for(int i = 0; i < behaviorCount; i++)
            {
                AddBehavior((ShapeBehavior.ShapeBehaviorType)reader.ReadInt()).Load(reader);
            }
        }
        // 그 이전 버전의 경우(4, 5) 기존 방식대로 벡터 자체를 읽어온다
        else if(reader.Version >= 4)
        {
            AddBehavior<RotationShapeBehavior>().AngularVelocity =
                reader.ReadVector3();
            AddBehavior<MovementShapeBehavior>().Velocity = reader.ReadVector3();
        }
    }

    // 복합체에 색상을 각자 부여하는 메소드
    public void SetColor (Color color, int index)
    {
        if(shaderPropertyBlock == null)
        {
            shaderPropertyBlock = new MaterialPropertyBlock();
        }
        shaderPropertyBlock.SetColor(colorPropertyId, color);
        colors[index] = color;
        meshRenderers[index].SetPropertyBlock(shaderPropertyBlock);
    }

    // 현재 지니고 있는 색상이 얼마나 되는지 받아옴
    public int ColorCount
    {
        get
        {
            return colors.Length;
        }
    }

    void LoadColors (GameDataReader reader)
    {
        int count = reader.ReadInt();
        // 저장된 색상의 수(count)와 사용하기 위해 저장한 색상의 수(colors.Length)를 비교
        int max = count <= colors.Length ? count : colors.Length; // 더 적은 값을 취함
        
        // 일단 저장된 정보까지는 정상적으로 로드
        int i = 0;
        for(; i < max; i++)
        {
            SetColor(reader.ReadColor(), i);
        }

        // 아직 저장 공간에 적용되지 못한 색상이 남았다면 읽어서 버퍼만 처리
        if(count > colors.Length)
        {
            for(; i < count; i++)
            {
                reader.ReadColor();
            }
        }
        // array에 사용하지 못한 색상이 있다면 배열의 남은 칸은 white(디폴트)로 대체
        else if(count < colors.Length)
        {
            for(; i < colors.Length; i++)
            {
                SetColor(Color.white, i);
            }
        }
    }


    // origin factory에 대한 reclaim 기능
    public void Recycle()
    {
        // Add Behavior의 Add Component로 인한 스크립트 덤핑을 막기 위함
        for(int i=0; i < behaviorList.Count; i++)
        {
            Destroy(behaviorList[i]);
        }
        behaviorList.Clear();

        OriginFactory.Reclaim(this);
    }

    // 새로운 행동을 추가하는 메소드 (제너릭)
    // T는 shape behavior이므로, 반드시 shape behavior를 상속하는 형에 대해서만 작동해야 한다
    public T AddBehavior<T>() where T : ShapeBehavior
    {
        T behavior = gameObject.AddComponent<T>();
        behaviorList.Add(behavior);
        return behavior;
    }

    private ShapeBehavior AddBehavior (ShapeBehavior.ShapeBehaviorType type)
    {
        switch(type)
        {
            case ShapeBehavior.ShapeBehaviorType.Movement:
                return AddBehavior<MovementShapeBehavior>();
            case ShapeBehavior.ShapeBehaviorType.Rotation:
                return AddBehavior<RotationShapeBehavior>();
        }

        // switch의 defualt에 해당하는 부분
        Debug.LogError("Forgor to support");
        return null;
    }
}
