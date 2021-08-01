using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    // 재활용할 물체들을 저장할 별도의 씬
    Scene poolScene;



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
                instance.OriginFactory = this;
                instance.ShapeId = shapeId;

                // recycle을 하는 경우, 만든 object를 extra scene으로 옮긴다
                SceneManager.MoveGameObjectToScene(
                    instance.gameObject, poolScene
                    );
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

        // 이 작업은 에디터 상에서만 가능하다
        if(Application.isEditor)
        {
            // 해당 씬이 존재하는지 탐색, 그 값을 받아와야 한다 (recompile 과정에서 값 상실)
            poolScene = SceneManager.GetSceneByName(name);
            // recompile시, 이미 pool이 있다면 다시 초기화하기 않는다
            if (poolScene.isLoaded)
            {
                // recompile 과정에서 Scene과 prefab들의 연결이 끊어지는 것을 대비
                GameObject[] rootObjects = poolScene.GetRootGameObjects();

                for(int i=0; i < rootObjects.Length; i++)
                {
                    Shape pooledShape = rootObjects[i].GetComponent<Shape>();
                    if(!pooledShape.gameObject.activeSelf)
                    {
                        pools[pooledShape.ShapeId].Add(pooledShape);
                    }
                }
                return;
            }
        }

        // recycle을 하는 경우, 이를 위한 Scene을 만들어야 한다
        poolScene = SceneManager.CreateScene(name);
        // cf. name은 Object의 name을 의미한다. 여기서는 ShapeFactory
    }

    public void Reclaim (Shape shapeToRecycle)
    {
        // 생성되었던 factory와 재활용되는 factory가 다르면 오류 호출
        if(shapeToRecycle.OriginFactory != this)
        {
            Debug.LogError("Tried to reclaim shape with wrong factory");
            return;
        }

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
