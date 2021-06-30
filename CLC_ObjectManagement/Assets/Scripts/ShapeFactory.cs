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

    // shape id에 해당하는 도형을 가져온다
    public Shape Get(int shapeId)
    {
        // return Instantiate(prefabs[shapeID]);
        Shape instance = Instantiate(prefabs[shapeId]);
        instance.ShapeId = shapeId;
        return instance;
    }

    public Shape GetRandom()
    {
        return Get(Random.Range(0, prefabs.Length));
    }
}
