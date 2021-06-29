using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataWriter
{
    BinaryWriter writer;

    // 생성자
    public GameDataWriter (BinaryWriter writer)
    {
        this.writer = writer;
    }

    // transform 성분을 저장하기 위함
    public void Write (float value)
    {
        writer.Write(value);
    }

    // list의 count를 저장하기 위함
    public void Write (int value)
    {
        writer.Write(value);
    }

    // rotation 값 저장
    public void Write(Quaternion value)
    {
        writer.Write(value.x);
        writer.Write(value.y);
        writer.Write(value.z);
        writer.Write(value.w);
    }

    // position 및 local scale값 저장
    public void Write(Vector3 value)
    {
        writer.Write(value.x);
        writer.Write(value.y);
        writer.Write(value.z);
    }
}
