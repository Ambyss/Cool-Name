using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

internal class GunsBasis
{
    public string name;
    public float fireRate;
    public float damage;
    public int ammo;
}

public class Gun : MonoBehaviour
{
    private List<GunsBasis> gun = new List<GunsBasis>();
    private IEnumerator coroutine;
    private Ray _ray;
    private RaycastHit _raycastHit;
    private Transform _player;
    public ParticleSystem wallPuff;
    public ParticleSystem EnemyPuff;

    void Start()
    {
        gun.Add(new GunsBasis(){name = "Pistol", fireRate = 3, damage = 1, ammo = 1000});
        coroutine = Firing();
        _player = transform.parent.GetComponent<Transform>();
    }

    private IEnumerator Firing()
    {
        while (true)
        {
            yield return new WaitForSeconds(1/Shot());
        }
    }

    private float Shot()
    {
        _ray = new Ray(transform.position, _player.forward);
        Physics.Raycast(_ray, out _raycastHit);
        if (_raycastHit.collider)
        {
            if (_raycastHit.collider.CompareTag("Enemy"))
            {
                _raycastHit.collider.GetComponent<Enemy>().Damage(1, 3); // TODO: Damage depends from type of weapon
                Debug.DrawLine(_ray.origin, _raycastHit.point, Color.blue, .1f);
                Instantiate(EnemyPuff, _raycastHit.point, Quaternion.identity);
            }
            else if (_raycastHit.collider.CompareTag("Wall"))
            {
                // TODO: create puff
                Debug.DrawLine(_ray.origin, _raycastHit.point, Color.blue, .1f);
                Instantiate(wallPuff, _raycastHit.point, Quaternion.identity);
            }
            // TODO: else to hit to playe in coop
        }
        
        return 3f; // TODO Return time to type of current weapon
    }
    
    public void Fire()
    {
        StartCoroutine(coroutine);
    }
    
    public void StopFire()
    {
        StopCoroutine(coroutine);
    }
}

