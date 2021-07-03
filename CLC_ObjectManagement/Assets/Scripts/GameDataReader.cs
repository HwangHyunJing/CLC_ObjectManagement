using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataReader
{
    public int Version { get; }

    BinaryReader reader;

    // 생성자
    public GameDataReader (BinaryReader reader, int version)
    {
        this.reader = reader;
        this.Version = version;
    }

    public float ReadFloat()
    {
        return reader.ReadSingle();
    }

    public int ReadInt()
    {
        return reader.ReadInt32();
    }

    // Quaternion값을 읽어드림
    public Quaternion ReadQuaternion()
    {
        Quaternion value;

        value.x = reader.ReadSingle();
        value.y = reader.ReadSingle();
        value.z = reader.ReadSingle();
        value.w = reader.ReadSingle();

        return value;
    }

    // position, local scale 값을 읽어드림
    public Vector3 ReadVector3()
    {
        Vector3 value;
        value.x = reader.ReadSingle();
        value.y = reader.ReadSingle();
        value.z = reader.ReadSingle();

        return value;
    }

    // 색상 정보를 읽어들이는 메소드
    public Color ReadColor()
    {
        Color value;
        value.r = reader.ReadSingle();
        value.g = reader.ReadSingle();
        value.b = reader.ReadSingle();
        value.a = reader.ReadSingle();

        return value;
    }
}
