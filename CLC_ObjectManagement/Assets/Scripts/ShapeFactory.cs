using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스크립트를 Asset으로 만드는 명령....;
[CreateAssetMenu]

// 별도의 오브젝트 형태로 존재하지 않으므로, monobehaviour 대신 Scriptable Object를 상속
public class ShapeFactory : ScriptableObject
{
    [SerializeField]
    Shape[] prefabs;

    [SerializeField]
    Material[] materials;

    // shape 오브젝트의 제활용 여부를 정하는 toggle
    [SerializeField]
    bool recycle;

    // 제거될 물체들을 재활용하기 위해 저장하는 공간
    List<Shape>[] pools;

    // shape id에 해당하는 도형을 가져온다
    public Shape Get(int shapeId = 0, int materialId = 0)
    {
        Shape instance;

        // 물체 생성시, 우선 recycle 여부와 pool을 검사
        if(recycle)
        {
            // pool이 비어있다면 pool을 생성
            if(pools == null)
            {
                CreatePools();
            }

            // 해당 Id 형태의 도형에 대한 pool(List<Shape>)를 넘겨줌
            List<Shape> pool = pools[shapeId];
            // 해당 List의 가장 마지막 원소를 추출
            int lastIndex = pool.Count - 1;

            // 받아온 pool 정보 내부에 무언가가 있다면, 
            if(lastIndex >= 0)
            {
                instance = pool[lastIndex];
                instance.gameObject.SetActive(true);
                pool.RemoveAt(lastIndex);
            }
            // 없으면 결국 새로 만들어야 한다
            else
            {
                instance = Instantiate(prefabs[shapeId]);
                instance.ShapeId = shapeId;
            }

        }
        // recycle하지 않는다면 그냥 생성한다
        else
        {
            instance = Instantiate(prefabs[shapeId]);
            instance.ShapeId = shapeId;
        }

        instance.SetMaterial(materials[materialId], materialId);
        return instance;
    }

    public Shape GetRandom()
    {
        return Get(
            Random.Range(0, prefabs.Length),
            Random.Range(0, materials.Length)
            );
    }

    void CreatePools()
    {
        pools = new List<Shape>[prefabs.Length];
        for(int i=0; i < pools.Length; i++)
        {
            pools[i] = new List<Shape>();
        }
    }

    public void Reclaim (Shape shapeToRecycle)
    {
        if(recycle)
        {
            if(pools == null)
            {
                CreatePools();
            }
            pools[shapeToRecycle.ShapeId].Add(shapeToRecycle);
            shapeToRecycle.gameObject.SetActive(false);
        }
        // 어차피 재활용할 거 아니면 그냥 제거하면 된다
        else
        {
            Destroy(shapeToRecycle.gameObject);
        }
    }
}
