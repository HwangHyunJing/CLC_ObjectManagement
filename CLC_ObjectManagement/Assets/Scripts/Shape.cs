using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObject
{
    // prefab 물체 도형의 종류를 정의하는 id
    int shapeId = int.MinValue;

    // 물체의 색상 정보를 빠르게 트레킹하기 위함
    Color color;
    MeshRenderer meshRenderer;

    // property block 사용을 위한 정적 타입
    static int colorPropertyId = Shader.PropertyToID("_Color");

    // 사용할 프로퍼티 블락
    static MaterialPropertyBlock shaderPropertyBlock;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public Vector3 AngularVelocity { get; set; }
    public Vector3 Velocity { get; set; }
    
    // private void FixedUpdate()
    public void GameUpdate()
    {
        // 각 물체가 제자리에서 돌도록 함
        transform.Rotate(AngularVelocity * Time.deltaTime);

        transform.localPosition += Velocity * Time.deltaTime;
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


    // Shape가 자신의 material 정보를 바꾸는 것을 막기 위해 set을 private으로 설정
    public int MaterialId { get; private set; }


    // 위에 setter의 기능을 대신하는 메소드
    public void SetMaterial(Material material, int materialId)
    {
        meshRenderer.material = material;
        MaterialId = materialId;
    }

    public void SetColor (Color color)
    {
        this.color = color;

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
        meshRenderer.SetPropertyBlock(shaderPropertyBlock);

        
    }

    // 도형의 정보에 대해 저장할 것들이 생기면, 여기서 하면 된다

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(color);
        writer.Write(AngularVelocity);
        writer.Write(Velocity);
    }

    public override void Load (GameDataReader reader)
    {
        base.Load(reader);
        SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
        AngularVelocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
        Velocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
    }
}
