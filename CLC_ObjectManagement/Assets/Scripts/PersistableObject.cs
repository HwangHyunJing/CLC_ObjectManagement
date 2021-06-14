using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 해당 클래스가 object내에 중복으로 존재하지 않도록 하는 코드
[DisallowMultipleComponent]

public class PersistableObject : MonoBehaviour
{
    public virtual void Save (GameDataWriter writer)
    {
        writer.Write(transform.localPosition);
        writer.Write(transform.localRotation);
        writer.Write(transform.localScale);
    }

    public virtual void Load(GameDataReader reader)
    {
        transform.localPosition = reader.ReadVector3();
        transform.localRotation = reader.ReadQuaternion();
        transform.localScale = reader.ReadVector3();
    }
}
