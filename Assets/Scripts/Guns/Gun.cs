using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GunsBasis
{
    public string name;
    public float fireRate;
    public float damage;
    public int ammo;
    public bool isOpen;
    public float puffPower;
}

public class GunController
{

    public int currentGun = 0;
    public List<GunsBasis> gun = new List<GunsBasis>()
    {
        new GunsBasis(){name = "Pistol", fireRate = 3, damage = .5f, ammo = 1000, isOpen = true, puffPower = 2},
        new GunsBasis(){name = "Uzi", fireRate = 20, damage = .2f, ammo = 100, isOpen = false, puffPower = .2f},
        new GunsBasis(){name = "Shotgun", fireRate = 2, damage = 2f, ammo = 20, isOpen = false, puffPower = 5},
        new GunsBasis(){name = "Auto", fireRate = 12, damage = .7f, ammo = 50, isOpen = false, puffPower = .5f},
        new GunsBasis(){name = "RPG", fireRate = 2, damage = 5f, ammo = 10, isOpen = false, puffPower = 10f},
        new GunsBasis(){name = "Granate", fireRate = 2, damage = 5f, ammo = 10, isOpen = false, puffPower = 10f},
        new GunsBasis(){name = "Mines", fireRate = 10, damage = 5f, ammo = 15, isOpen = false, puffPower = 10f},
    };

    public void ChangeGun()
    {
        currentGun++;
        if (currentGun > gun.Count - 1)
            currentGun = 0;
    }
}

public class Gun : MonoBehaviour
{
    private IEnumerator coroutine;
    private IEnumerator granateCoroutine;

    private Ray _ray;
    private RaycastHit _raycastHit;
    private Transform _player;
    public ParticleSystem wallPuff;
    public ParticleSystem EnemyPuff;
    private bool _isFire;
    private LineRenderer _line;
    private GunController _gunController = new GunController();
    [SerializeField] private Text _currentAmmoText;
    private AmmoSpawner _ammoSpawner;
    [SerializeField] private GameObject[] _guns;
    [SerializeField] private GameObject _granate;
    [SerializeField] private GameObject _RPGWarhead;
    [SerializeField] private GameObject _mine;
    private float _granateForce;
    private float _startGranateForce;
    public TrajectoryRenderer trajectoryRenderer;
    public GunText GunText;
    private bool _isMinesPlaces;

    void Start()
    {
        coroutine = Firing();
        granateCoroutine = GranateForce();
        _player = transform.parent.GetComponent<Transform>();
        _isFire = true;
        _line = GetComponent<LineRenderer>();
        _currentAmmoText.text = _gunController.gun[_gunController.currentGun].ammo.ToString();
        _ammoSpawner = GameObject.Find("WavesController").GetComponent<AmmoSpawner>();
        _startGranateForce = 5;
        _granateForce = _startGranateForce;
        GetCurrentGunName();
        _isMinesPlaces = true;
    }

    private IEnumerator Firing()
    {
        while (true)
        {
            if (_gunController.gun[_gunController.currentGun].ammo <= 0)
            {
                print("Oh no");
                break;
            }
            if (_gunController.gun[_gunController.currentGun].name == "Shotgun" && _gunController.gun[_gunController.currentGun].ammo > 0)
            {
                yield return new WaitForSeconds(1 / ShotgunShot());
            }
            else if (_gunController.gun[_gunController.currentGun].name == "RPG" &&
                     _gunController.gun[_gunController.currentGun].ammo > 0)
            {
                yield return new WaitForSeconds(1/RPGShot());
            }
            else if (_gunController.gun[_gunController.currentGun].ammo > 0)
            {
                yield return new WaitForSeconds(1/SimpleShot());
            }
            _gunController.gun[_gunController.currentGun].ammo--;
            _currentAmmoText.text = _gunController.gun[_gunController.currentGun].ammo.ToString();
        }
        print("No ammo!");
        yield return new WaitForSeconds(0.4f);
        StopFire();
    }

    private float SimpleShot()
    {
        float angle = Mathf.Acos((_player.forward.x) / (_player.forward.magnitude)) * Mathf.Rad2Deg;
        if (_player.forward.z < 0)
            angle = 360 - angle;
        angle += Random.Range(-5f, 5f);
        Vector3 target = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
        _ray = new Ray(transform.position, target);
        Physics.Raycast(_ray, out _raycastHit);
        _line.SetPosition(0, transform.position);
        _line.SetPosition(1, _raycastHit.point);
        for (int i = 2; i < _line.positionCount; i+= 2)
        {
            _line.SetPosition(i, transform.position);
            _line.SetPosition(i+1, _raycastHit.point);
        }
        StartCoroutine(BulletTrack());
        if (_raycastHit.collider)
        {
            if (_raycastHit.collider.CompareTag("Enemy"))
            {
                _raycastHit.collider.GetComponent<Enemy>().Damage(_gunController.gun[_gunController.currentGun].damage, _gunController.gun[_gunController.currentGun].puffPower);
                Instantiate(EnemyPuff, _raycastHit.point, Quaternion.identity);
            }
            else if (_raycastHit.collider.CompareTag("Wall"))
            {
                // Puff
                Instantiate(wallPuff, _raycastHit.point, Quaternion.identity);
            }
            // TODO: else to hit to play in coop
        }
        
        return _gunController.gun[_gunController.currentGun].fireRate; // TODO Return time to type of current weapon
    }
    
    private float ShotgunShot()
    {
        for (int i = 0; i < _line.positionCount/2; i++)
        {
            float angle = Mathf.Acos((_player.forward.x) / (_player.forward.magnitude)) * Mathf.Rad2Deg;
            if (_player.forward.z < 0)
                angle = 360 - angle;
            angle += Random.Range(-5f, 5f);
            Vector3 target = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
            _ray = new Ray(transform.position, target);
            Physics.Raycast(_ray, out _raycastHit);
            _line.SetPosition(i * 2, transform.position);
            _line.SetPosition(i * 2 + 1, _raycastHit.point);
            StartCoroutine(BulletTrack());
            if (_raycastHit.collider)
            {
                if (_raycastHit.collider.CompareTag("Enemy"))
                {
                    _raycastHit.collider.GetComponent<Enemy>().Damage(_gunController.gun[_gunController.currentGun].damage, _gunController.gun[_gunController.currentGun].puffPower);
                    Instantiate(EnemyPuff, _raycastHit.point, Quaternion.identity);
                }
                else if (_raycastHit.collider.CompareTag("Wall"))
                {
                    // Puff
                    Instantiate(wallPuff, _raycastHit.point, Quaternion.identity);
                }
                // TODO: else to hit to play in coop
            }
        }
        return _gunController.gun[_gunController.currentGun].fireRate;
    }
    
    private float RPGShot()
    {
        float angle = Mathf.Acos((_player.forward.x) / (_player.forward.magnitude)) * Mathf.Rad2Deg;
        if (_player.forward.z < 0)
            angle = 360 - angle;
        Vector3 target = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
        Instantiate(_RPGWarhead, transform.position, transform.rotation).GetComponent<Rigidbody>().AddForce(target * 1500);
        return _gunController.gun[_gunController.currentGun].fireRate;
    }
    
    private void GranadeShot()
    {
        float angle = Mathf.Acos((_player.forward.x) / (_player.forward.magnitude)) * Mathf.Rad2Deg;
        if (_player.forward.z < 0)
            angle = 360 - angle;
        Vector3 target = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 1.2f, Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        Instantiate(_granate, transform.position, Quaternion.identity).GetComponent<Rigidbody>().AddForce(target * _granateForce, ForceMode.VelocityChange);
        
        trajectoryRenderer.ShowTrajectory(new Vector3(0, -100, 0), Vector3.zero);
        _granateForce = _startGranateForce;
    }

    IEnumerator GranateForce()
    {
        while (true)
        {
            float angle = Mathf.Acos((_player.forward.x) / (_player.forward.magnitude)) * Mathf.Rad2Deg;
            if (_player.forward.z < 0)
                angle = 360 - angle;
            Vector3 target = new Vector3();
            target = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 1.2f, Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
            if (_granateForce < 15)
                _granateForce += .1f;
            trajectoryRenderer.ShowTrajectory(transform.position, target * _granateForce);
            yield return new WaitForSeconds(.01f);
        }
    }
    
    public void Fire()
    {
        if (_isFire)
        {
            if (_gunController.gun[_gunController.currentGun].name == "Granate")
            {
                StartCoroutine(granateCoroutine);
            }
            else if (_gunController.gun[_gunController.currentGun].name == "Mines")
            {
                _isMinesPlaces = false;
                Instantiate(_mine, transform.position, Quaternion.identity);
            }
            else
            {
                StartCoroutine(ReloadBuffer(.3f)); // TODO: current reload buffer
                StartCoroutine(coroutine);
            }
        }
        // TODO: else: claz, ammo null
    }
    
    public void StopFire()
    {
        StopCoroutine(coroutine);
        _isMinesPlaces = true;
        if (_gunController.gun[_gunController.currentGun].name == "Granate")
        {
            GranadeShot();
            StopCoroutine(granateCoroutine);
        }
    }
    
    private IEnumerator ReloadBuffer(float waitTime)
    {
        _isFire = false;
        yield return new WaitForSeconds(waitTime);
        _isFire = true;
    }

    private IEnumerator BulletTrack()
    {
        _line.enabled = true;
        yield return new WaitForSeconds(.02f);
        _line.enabled = false;
    }
    
    public void ChangeGun()
    {
        _gunController.ChangeGun();
        _currentAmmoText.text = _gunController.gun[_gunController.currentGun].ammo.ToString();

        for (int i = 0; i < _guns.Length; i++)
        {
            _guns[i].SetActive(false);
        }

        _guns[_gunController.currentGun].SetActive(true);
        GetCurrentGunName();
    }

    public void AddAmmo()
    {
        _gunController.gun[0].ammo += 30;
        _gunController.gun[1].ammo += 100;
        _gunController.gun[2].ammo += 20;
        _gunController.gun[3].ammo += 50;
        _currentAmmoText.text = _gunController.gun[_gunController.currentGun].ammo.ToString();
        _ammoSpawner.AmmoPicked();
    }

    public GunController GetGunController()
    {
        return _gunController;
    }

    public void GetCurrentGunName()
    {
        GunText.ChangeGun(_gunController.gun[_gunController.currentGun].name);
    }
}

