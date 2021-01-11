using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WavesController : MonoBehaviour
{
    private GameObject[] _spawners;
    private int _currentWave; // TODO: Bind this and SpawnEnemies()
    private int _numOfEnemies;
    private BoxCollider _collider;
    public Text exitText;
    private IEnumerator _exitRoutine;
    private PlayerController _player;
    private int _comboWave;
    private int _comboWaveMax;
    private float _comboPercent;
    private int _score;
    private Text[] _scoreNumbers = new Text[8];
    private bool _isPaused;
    public Canvas PauseCanvas;
    public Canvas GameCanvas;
    public Canvas DeathCanvas;
    private ComboVizualizer _comboVizualizer;
    public Gun Gun;
    public GameObject NewGunText;
    private bool _isTextFade;
    private Color _fadeColor;
    public Text NewWaveText;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _currentWave = 1;
    }

    private void Start()
    {
        NewGunText.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        _fadeColor = new Color(0, 0, 0, -0.02f);
        _isTextFade = false;
        Time.timeScale = 1;
        PauseCanvas.enabled = false;
        _isPaused = false;
        exitText.enabled = false;
        _exitRoutine = ExitFromLab();
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _comboWave = 1;
        _comboPercent = 0f;
        _score = 0;
        GameObject tempScoreLine = GameObject.Find("ScoreLine");
        for (int i = _scoreNumbers.Length - 1, k = 0; i >= 0; i--)
        {
            _scoreNumbers[k] = tempScoreLine.transform.GetChild(i).GetComponent<Text>();
            k++;
        }
        UpdateScore();
        _comboWaveMax = _comboWave;
        _comboVizualizer = GetComponent<ComboVizualizer>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _comboPercent += 90;
        _comboPercent -= _comboWave * 0.05f;
        if (_comboPercent > 100)
        {
            _comboWave++;
            _comboPercent = _comboPercent - 100;
        }
        else if (_comboPercent < 0)
        {
            if (_comboWave == 1)
            {
                _comboPercent = 0;
            }
            else
            {
                _comboWave--;
                _comboPercent = 100;
            }
        }
        if (_comboWave > _comboWaveMax)
            _comboWaveMax = _comboWave;
        _comboVizualizer.UpdateCombo(_comboPercent, _comboWave);
        
        // Open Guns
        if (_comboWave >= 20)
        {
            Gun.OpenGun(6);
        }
        else if (_comboWave >= 16)
        {
            Gun.OpenGun(5);
        }
        else if (_comboWave >= 12)
        {
            Gun.OpenGun(4);
        }
        else if (_comboWave >= 8)
        {
            Gun.OpenGun(3);
        }
        else if (_comboWave >= 5)
        {
            Gun.OpenGun(2);
        }
        else if (_comboWave >= 3)
        {
            Gun.OpenGun(1);
        }
        
        // New Gun Text
        if (_isTextFade)
        {
            NewGunText.GetComponent<Image>().color += _fadeColor;
        }
    }
    
    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            exitText.enabled = true;
            StartExit();
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            exitText.enabled = false;
            StopAllCoroutines();
        }
    }

    private IEnumerator ExitFromLab()
    {
        _player.Damage(3);
        yield return new WaitForSeconds(.1f);
        StartExit();
    }

    private void StartExit()
    {        
        StartCoroutine(ExitFromLab());
    }
    
    public void InitSpawn()
    {
        _spawners = GameObject.FindGameObjectsWithTag("Spawner");
        SpawnEnemies();
    }

    public void InitCollider(int width, int height, int k)
    {
        _collider.center = new Vector3((width - 1) * k / 2, 0, (height - 1) * k / 2);
        _collider.size = new Vector3((width - 1) * k, 5, (height - 1) * k);
    }
    
    public void SpawnEnemies()
    {
        int randomnum = _currentWave * _currentWave + _currentWave * Random.Range(0, 4);
        for (int i = 0; i < _spawners.Length; i++)
        {
            _spawners[i].GetComponent<EnemySpawner>().SpawnEnemies(randomnum);
            InitNumOfEnemies(randomnum);
        }
    }
    
    private void NextWave()
    {
        _currentWave++;
        SpawnEnemies();
    }

    private void InitNumOfEnemies(int number)
    {
        _numOfEnemies += number;
    }

    public void EnemyKilled()
    {
        _comboPercent += 30;
        _numOfEnemies -= 1;
        if (_numOfEnemies == 0)
            NextWave();
        _score += 8367; // TODO: Score depends from enemy
        UpdateScore();
    }

    private void UpdateScore()
    {
        for (int i = 0; i < _scoreNumbers.Length - 1; i++)
        {
            _scoreNumbers[i].text = ((int)(_score % Mathf.Pow(10, i + 1))/(int)(Mathf.Pow(10, i))).ToString();
        }
    }

    public void Pause()
    {
        if (_isPaused)
        {
            Time.timeScale = 1;
            GameCanvas.enabled = true;
            PauseCanvas.enabled = false;
        }
        else
        {
            Time.timeScale = 0;
            GameCanvas.enabled = false;
            PauseCanvas.enabled = true;
        }
        _isPaused = !_isPaused;
    }

    public void Death()
    {
        DeathCanvas.GetComponent<DeathCanvas>().Death(_score, _comboWaveMax);
        Time.timeScale = 0;
        // TODO: Save stats
    }

    public void NewGun()
    {
        NewGunText.GetComponent<Image>().color = Color.white;
        print(NewGunText.GetComponent<Image>().color);
        StartCoroutine(NewGunAppear());
    }

    IEnumerator NewGunAppear()
    {
        _isTextFade = true;
        yield return new WaitForSeconds(2);
        NewGunText.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        _isTextFade = false;
    }
}
