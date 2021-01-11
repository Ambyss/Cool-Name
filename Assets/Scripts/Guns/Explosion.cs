using System;
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
        UnityEngine.Camera.main.GetComponent<Animator>().Play("Shake");
    }

    IEnumerator Babah()
    {
        yield return new WaitForSeconds(.4f);
        //Destroy(_explosionEffect);
        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            float distance = (transform.position - collider.transform.position).magnitude;
            try
            {
                collider.GetComponent<Enemy>().ExplosionDamage(_gunController.gun[4].damage/distance,
                    _gunController.gun[4].puffPower/distance, transform.position);
            }
            catch (Exception e)
            {
                collider.GetComponent<EnemyBig>().ExplosionDamage(_gunController.gun[4].damage/distance,
                    _gunController.gun[4].puffPower/distance, transform.position);
            }
        }
        else if (collider.CompareTag("Player"))
        {
            float distance = (transform.position - collider.transform.position).magnitude;
            float tempDamage = _gunController.gun[4].damage / distance * 10;
            int damage = (int)tempDamage;
            collider.GetComponent<PlayerController>().Damage(damage);
        }
    }
}
