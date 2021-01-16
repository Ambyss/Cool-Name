using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

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
        new GunsBasis(){name = "Pistol", fireRate = 3, damage = .5f, ammo = 2, isOpen = true, puffPower = 2},
        new GunsBasis(){name = "Uzi", fireRate = 20, damage = .2f, ammo = 100, isOpen = true, puffPower = .2f},
        new GunsBasis(){name = "Shotgun", fireRate = 2, damage = 2f, ammo = 20, isOpen = true, puffPower = 5},
        new GunsBasis(){name = "Auto", fireRate = 12, damage = .7f, ammo = 50, isOpen = true, puffPower = .5f},
        new GunsBasis(){name = "RPG", fireRate = 2, damage = 5f, ammo = 10, isOpen = true, puffPower = 10f},
        new GunsBasis(){name = "Granate", fireRate = 2, damage = 5f, ammo = 10, isOpen = true, puffPower = 10f},
        new GunsBasis(){name = "Mines", fireRate = 10, damage = 5f, ammo = 15, isOpen = true, puffPower = 10f},
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
    private WavesController _wavesController;
    private AudioSystem _audioSystem;
    public AudioClip Pistol;
    public AudioClip Shotgun;
    public AudioClip Auto;
    public AudioClip Uzi;
    public AudioClip RPG;


    void Start()
    {
        _audioSystem = GameObject.FindWithTag("Audio").GetComponent<AudioSystem>();
        coroutine = Firing();
        granateCoroutine = GranateForce();
        _player = transform.parent.GetComponent<Transform>();
        _isFire = true;
        _line = GetComponent<LineRenderer>();
        _currentAmmoText.text = _gunController.gun[_gunController.currentGun].ammo.ToString();
        _ammoSpawner = GameObject.Find("WavesController").GetComponent<AmmoSpawner>();
        _wavesController = GameObject.Find("WavesController").GetComponent<WavesController>();
        _startGranateForce = 5;
        _granateForce = _startGranateForce;
        GetCurrentGunName();
        _isMinesPlaces = true;
    }

    void FixedUpdate()
    {
        print(_gunController.gun[_gunController.currentGun].ammo);

    }
    
    private IEnumerator Firing()
    {
        while (true)
        {
            if (_gunController.gun[_gunController.currentGun].ammo == 0)
            {
                print("Oh no");
                yield return new WaitForSeconds(.1f);
                continue;
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
    }

    private float SimpleShot()
    {
        if (_gunController.gun[_gunController.currentGun].name == "Pistol")
            _audioSystem.PlayAudio(Pistol);
        else if (_gunController.gun[_gunController.currentGun].name == "Auto")
            _audioSystem.PlayAudio(Auto);
        else if (_gunController.gun[_gunController.currentGun].name == "Uzi")
            _audioSystem.PlayAudio(Uzi);
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
                try
                {
                    _raycastHit.collider.GetComponent<Enemy>().Damage(_gunController.gun[_gunController.currentGun].damage, _gunController.gun[_gunController.currentGun].puffPower);
                }
                catch (Exception e)
                {
                    _raycastHit.collider.GetComponent<EnemyBig>().Damage(_gunController.gun[_gunController.currentGun].damage, _gunController.gun[_gunController.currentGun].puffPower);
                }
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
        _audioSystem.PlayAudio(Shotgun);
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
                    try
                    {
                        _raycastHit.collider.GetComponent<Enemy>().Damage(_gunController.gun[_gunController.currentGun].damage, _gunController.gun[_gunController.currentGun].puffPower);
                    }
                    catch (Exception e)
                    {
                        _raycastHit.collider.GetComponent<EnemyBig>().Damage(_gunController.gun[_gunController.currentGun].damage, _gunController.gun[_gunController.currentGun].puffPower);
                    }
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
        _gunController.gun[_gunController.currentGun].ammo--;
        _currentAmmoText.text = _gunController.gun[_gunController.currentGun].ammo.ToString();
        _audioSystem.PlayAudio(RPG);
        float angle = Mathf.Acos((_player.forward.x) / (_player.forward.magnitude)) * Mathf.Rad2Deg;
        if (_player.forward.z < 0)
            angle = 360 - angle;
        Vector3 target = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
        Instantiate(_RPGWarhead, transform.position, Quaternion.LookRotation(target)).GetComponent<Rigidbody>().AddForce(target * 1500);
        print(_gunController.gun[_gunController.currentGun].ammo);
        return _gunController.gun[_gunController.currentGun].fireRate;
    }
    
    private void GranadeShot()
    {
        _gunController.gun[_gunController.currentGun].ammo--;
        _currentAmmoText.text = _gunController.gun[_gunController.currentGun].ammo.ToString();
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
            if (_gunController.gun[_gunController.currentGun].name == "Granate" && _gunController.gun[_gunController.currentGun].ammo > 0)
            {
                StartCoroutine(granateCoroutine);
            }
            else if (_gunController.gun[_gunController.currentGun].name == "Mines" && _gunController.gun[_gunController.currentGun].ammo > 0)
            {
                _isMinesPlaces = false;
                Instantiate(_mine, transform.position, Quaternion.identity);
                _gunController.gun[_gunController.currentGun].ammo--;
                _currentAmmoText.text = _gunController.gun[_gunController.currentGun].ammo.ToString();
            }
            else
            {
                StartCoroutine(ReloadBuffer(1/_gunController.gun[_gunController.currentGun].fireRate));
                StartCoroutine(coroutine);
            }
        }
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

        while (true)
        {
            _gunController.ChangeGun();
            if (_gunController.gun[_gunController.currentGun].isOpen)
                break;
        }
        
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
        _gunController.gun[4].ammo += 12;
        _gunController.gun[5].ammo += 10;
        _gunController.gun[6].ammo += 8;

        _currentAmmoText.text = _gunController.gun[_gunController.currentGun].ammo.ToString();
        _ammoSpawner.AmmoPicked();
        for (int i = 0; i < _gunController.gun.Count; i++)
        {
            if (_gunController.gun[_gunController.currentGun].ammo > 0)
                _gunController.gun[_gunController.currentGun].isOpen = true;
        }
    }

    public GunController GetGunController()
    {
        return _gunController;
    }

    public void GetCurrentGunName()
    {
        GunText.ChangeGun(_gunController.gun[_gunController.currentGun].name);
    }

    public void OpenGun(int numOfGun)
    {
        if (!_gunController.gun[numOfGun].isOpen)
        {
            _gunController.gun[numOfGun].isOpen = true;
            _wavesController.NewGun();
        }
    }
}

