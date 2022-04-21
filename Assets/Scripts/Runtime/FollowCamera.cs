using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    private void FixedUpdate()
    {
        if(target == null) { return; }
        
        Vector3 pos = target.position;
        pos.y = transform.position.y;

        transform.position = Vector3.Lerp(transform.position, pos, .1f);
    }
}
