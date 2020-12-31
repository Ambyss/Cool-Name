using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public ParticleSystem _explosionEffect;
    private GunController _gunController;

    void Start()
    {
        _gunController = GameObject.Find("Gun").GetComponent<Gun>().GetGunController();
        Instantiate(_explosionEffect, transform.position, Quaternion.identity);
        StartCoroutine(Babah());
    }

    IEnumerator Babah()
    {
        yield return new WaitForSeconds(.4f);
        //Destroy(_explosionEffect);
        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider collider)
    {
        print(collider.tag);
        if (collider.CompareTag("Enemy"))
        {
            float distance = (transform.position - collider.transform.position).magnitude;
            collider.GetComponent<Enemy>().ExplosionDamage(_gunController.gun[4].damage/distance,
                _gunController.gun[4].puffPower/distance, transform.position);
        }
    }
}
