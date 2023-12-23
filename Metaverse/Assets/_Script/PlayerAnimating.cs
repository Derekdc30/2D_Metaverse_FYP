using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimating : MonoBehaviour
{
    private Animator _animator;
    private void Awake() {
        _animator = GetComponent<Animator>();
    }
    public void SetMoving(bool value){
        _animator.SetBool("Moving", value);
    }
}
