using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
            Debug.LogError("No Animator Component On " + gameObject.name);
    }

    void Update()
    {
        _animator.SetBool("isMove", Player.Instance.isMove);
    }
}
