using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoveringAnimationScript : MonoBehaviour
{
    public Animator Animator;

    public void FlipOff()
    {
        Animator.SetBool("Flipped", false);
    }
}
