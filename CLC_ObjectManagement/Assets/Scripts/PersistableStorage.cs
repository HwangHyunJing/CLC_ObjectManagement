using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PersistableStorage : MonoBehaviour
{
    // 저장 경로
    string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }

    public void Save(PersistableObject o, int version)
    {
        using (
            var writer = new BinaryWriter(File.Open(savePath, FileMode.Create))
            )
        {
            writer.Write(-version);
            o.Save(new GameDataWriter(writer));
        }
    }

    public void Load(PersistableObject o)
    {
        /*
        using(
            var reader = new BinaryReader(File.Open(savePath, FileMode.Open))
            )
        {
            o.Load(new GameDataReader(reader, -reader.ReadInt32()));
        }*/

        // 코루틴으로 인해 파일이 조기에 닫히는 문제로, 여는 방식을 변경
        byte[] data = File.ReadAllBytes(savePath);
        // Binary류 메소드는 단순 배열인 data(byte[])를 읽을 수 없으므로 스트림으로 전환
        var reader = new BinaryReader(new MemoryStream(data));
        o.Load(new GameDataReader(reader, -reader.ReadInt32()));
    }
}
