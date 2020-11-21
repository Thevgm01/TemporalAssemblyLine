using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoxSpawner_WallConveyor : BoxSpawner
{
    [SerializeField] Collider targetWall;
    int grabbableLayer = -1;

    protected override void Awake()
    {
        base.Awake();
        if (targetWall == null) Debug.LogWarning("Wall reference missing from wall conveyor.");
        grabbableLayer = box.layer;
    }

    public override void Spawn()
    {
        base.Spawn();

        var newBox = spawnedBoxes[spawnedBoxes.Count - 1];
        StartCoroutine("PassThroughTargetWall", newBox.GetComponent<Collider>());
    }

    IEnumerator PassThroughTargetWall(Collider box)
    {
        if (targetWall) yield return null;

        box.gameObject.layer = 0; // Prevent the player from grabbing the box inside the wall, potentially allowing them to move it around through the entire wall
        Physics.IgnoreCollision(box, targetWall, true);

        while (box && !box.bounds.Intersects(targetWall.bounds))
            yield return new WaitForSeconds(0.1f);
        while (box && box.bounds.Intersects(targetWall.bounds))
            yield return new WaitForSeconds(0.1f);

        if (box)
        {
            box.gameObject.layer = grabbableLayer;
            Physics.IgnoreCollision(box, targetWall, false);
        }
    }
}
