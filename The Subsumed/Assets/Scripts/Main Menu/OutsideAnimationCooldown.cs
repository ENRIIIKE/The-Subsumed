using System.Collections;
using UnityEngine;

public class OutsideAnimationCooldown : MonoBehaviour
{
    public Animator rat;
    private void Start()
    {
        StartCoroutine(SetAnimation());
    }
    public IEnumerator SetAnimation()
    {
        rat.SetTrigger("Start");
        yield return new WaitForSeconds(12f);
        StartCoroutine(SetAnimation());
    }
}
