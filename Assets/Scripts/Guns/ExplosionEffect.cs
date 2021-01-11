using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Death());
        UnityEngine.Camera.main.GetComponent<Animator>().SetTrigger("shake");
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
