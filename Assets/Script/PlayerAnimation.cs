using UnityEngine;
using System;
using System.Diagnostics;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    // 死亡アニメーションが終了したときに発火するイベントを追加
    public static event Action OnDeathAnimationComplete = delegate { };

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayJump(float velY)
    {
        animator.SetFloat("Jump", velY);
    }

    public void PlayRun(bool running)
    {
        animator.SetBool("Run", running);
    }

    public void PlayerDead(bool dead)
    {
        animator.SetBool("deadFlag", dead);
    }

    public void OnDeathAnimationCompleteMethod()
    {
        UnityEngine.Debug.Log("Death animation has completed!");
        OnDeathAnimationComplete.Invoke();
    }

}
