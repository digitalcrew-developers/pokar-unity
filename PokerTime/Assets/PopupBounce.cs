using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupBounce : MonoBehaviour
{
    private Animator _animator;
    private bool animationPlayed = false;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        PlayMainMenuAnimations();
    }

    public void PlayMainMenuAnimations()
    {
        if (!animationPlayed)
        {
            _animator.SetTrigger("or");
            animationPlayed = true;
        }
    }
}
