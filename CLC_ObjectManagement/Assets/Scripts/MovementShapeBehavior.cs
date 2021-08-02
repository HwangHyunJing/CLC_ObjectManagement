﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementShapeBehavior : ShapeBehavior
{
    public Vector3 Velocity { get; set; }

    public override void GmaeUpdate(Shape shape)
    {
        shape.transform.localPosition += Velocity * Time.deltaTime;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(Velocity);
    }

    public override void Load (GameDataReader reader)
    {
        Velocity = reader.ReadVector3();
    }
}
