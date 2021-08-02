using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// abstract 클래스는 자신의 객체를 생성할 수 없습니다
public abstract class SpawnZone : PersistableObject
{
    // zone에 대한 정보 - 가령 next sequential index 라던가 - 를 저장하기 위해 P.O.를 상속

    public abstract Vector3 SpawnPoint { get; }


    // shape의 이동에 대한 데이터를 하나로 묶음
    [System.Serializable]
    public struct SpawnConfiguration
    {
        public enum MovementDirection
        {
            Forward,
            Upward,
            Outward,
            Random
        }

        // shape factory가 다원화되면서, 이를 각각 받게 되었다
        public ShapeFactory[] factories;

        public MovementDirection movementDirection;
        // shape의 이동 속력
        public FloatRange speed;
        // shape의 회전 속력
        public FloatRange angularSpeed;
        // shape의 크기
        public FloatRange scale;

        public ColorRangeHSV color;

        // 동일한 색상의 도형만을 낼지 결정
        public bool uniformColor;
    }

    [SerializeField]
    SpawnConfiguration spawnConfig;

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


    [System.Serializable]
    public struct ColorRangeHSV
    {
        [FloatRangeSlider(0f, 1f)]
        public FloatRange hue, saturation, value;

        public Color RandomInRange
        {
            get
            {
                return Random.ColorHSV(
                    hue.min, hue.max, saturation.min, saturation.max,
                    value.min, value.max, 1f, 1f);
            }
        }
    }


    // Game.cs의 CreateShape 코드를 복사, 수정
    // 이제 Game.cs에서 SpawnZone.cs로 본격적으로 이관
    // public virtual void ConfigureSpawn(Shape shape)

    public virtual Shape SpawnShape()
    {
        int factoryIndex = Random.Range(0, spawnConfig.factories.Length);
        Shape shape = spawnConfig.factories[factoryIndex].GetRandom();

        Transform t = shape.transform;

        // 반지름이 1인 구 범위 내. (getter로 값 받아옴)
        t.localPosition = SpawnPoint;
        // 랜덤한 쿼터니언 성분을 리턴
        t.localRotation = Random.rotation;
        // Random. Range는 float를 리턴, transform.scale로 쓰려면 Vector를 곱해야 한다
        t.localScale = Vector3.one * spawnConfig.scale.RandomValueInRange;

        if(spawnConfig.uniformColor)
        {
            // 복합체의 경우 동일한 색상을 부여
            shape.SetColor(spawnConfig.color.RandomInRange);
        }
        else
        {
            for(int i = 0; i < shape.ColorCount; i++)
            {
                shape.SetColor(spawnConfig.color.RandomInRange, i);
            }
        }

        float angularSpeed = spawnConfig.angularSpeed.RandomValueInRange;
        if(angularSpeed != 0f)
        {
            // 제너릭 Add Behavior
            // 회전에 대한 스크립트 객체를 넘김
            var rotation = shape.AddBehavior<RotationShapeBehavior>();
            // 랜덤한 회전 속도를 부여
            rotation.AngularVelocity =
                Random.onUnitSphere * angularSpeed;
        }

        //
        

        float speed = spawnConfig.speed.RandomValueInRange;
        if(speed != 0f)
        {
            Vector3 direction;

            switch (spawnConfig.movementDirection)
            {
                case SpawnConfiguration.MovementDirection.Upward:
                    direction = transform.up;
                    break;
                case SpawnConfiguration.MovementDirection.Outward:
                    direction = (t.localPosition - transform.position).normalized;
                    break;
                case SpawnConfiguration.MovementDirection.Random:
                    direction = Random.onUnitSphere;
                    break;
                default:
                    direction = transform.forward;
                    break;
            }

            // 제너릭 Add Behavior
            var movement = shape.AddBehavior<MovementShapeBehavior>();
            // 랜덤 방향 대신에 주어진 성분을 사용
            movement.Velocity = direction * speed;
        }


        return shape;
    }
}
