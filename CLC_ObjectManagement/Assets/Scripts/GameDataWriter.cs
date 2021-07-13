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

    // 색상 정보를 다루기 위한
    public void Write(Color value)
    {
        writer.Write(value.r);
        writer.Write(value.g);
        writer.Write(value.b);
        writer.Write(value.a);
    }

    // 직렬화된 랜덤 데이터를 받아 기록하는 메소드
    public void Write(Random.State value)
    {
        // 데이터를 직접 넘겨줄 수 없으므로 직렬화 타입을 활용
        writer.Write(JsonUtility.ToJson(value));
    }
}
