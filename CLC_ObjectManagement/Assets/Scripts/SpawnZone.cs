using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// abstract 클래스는 자신의 객체를 생성할 수 없습니다
public abstract class SpawnZone : PersistableObject
{
    // zone에 대한 정보 - 가령 next sequential index 라던가 - 를 저장하기 위해 P.O.를 상속

    public abstract Vector3 SpawnPoint { get; }

    // spawn zone을 기준으로 shape들이 움직이는 방향에 대한 열거형
    public enum SpawnMovementDirection
    {
        Forward,
        Upward,
        Outward,
        Random
    }
    [SerializeField]
    SpawnMovementDirection spawnMovementDirection;

    // shape가 움직이는 속력에 대한 최소/최대값
    [SerializeField]
    FloatRange spawnSpeed;

    [System.Serializable]
    // 속력 범위 설정을 깔끔하게 하기 위한 구조체
    public struct FloatRange
    {
        public float min, max;

        // 실제로 사용될 속력값
        public float RandomValueInRange
        {
            get
            {
                return Random.Range(min, max);
            }
        }
    }

    // Game.cs의 CreateShape 코드를 복사, 수정
    // 값의 configure를 Game.cs가 아니라 여기서 담당
    public virtual void ConfigureSpawn(Shape shape)
    {
        Transform t = shape.transform;

        // 반지름이 1인 구 범위 내. (getter로 값 받아옴)
        t.localPosition = SpawnPoint;
        // 랜덤한 쿼터니언 성분을 리턴
        t.localRotation = Random.rotation;
        // Random. Range는 float를 리턴하기 때문에, transform.scale로 쓰려면 Vector를 곱해야 한다
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);

        // 생성시 임의의 색상을 부여
        shape.SetColor(Random.ColorHSV(
            0f, 1f, .5f, 1f, .25f, 1f, 1f, 1f));
        // 랜덤한 회전 속도를 부여
        shape.AngularVelocity = Random.onUnitSphere * Random.Range(0f, 90f);

        //
        Vector3 direction;
        if(spawnMovementDirection == SpawnMovementDirection.Upward)
        {
            // spawnzone에 종속되어 움직이므로, Vector3 형이 아니라 transform 성분이 맞다
            direction = transform.up;
        }
        else if (spawnMovementDirection == SpawnMovementDirection.Outward)
        {
            // 정확히 zone의 중심에 spawn되는 게 아니므로 가능한 방식
            direction = (t.localPosition - transform.position).normalized;
        }
        else if(spawnMovementDirection == SpawnMovementDirection.Random)
        {
            direction = Random.onUnitSphere;
        }
        else
        {
            direction = transform.forward;
        }
        // 랜덤 방향 대신에 주어진 성분을 사용
        shape.Velocity = direction * spawnSpeed.RandomValueInRange;
    }
}
