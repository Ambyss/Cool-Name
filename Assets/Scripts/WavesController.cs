using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

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
    private float _comboPercent;
    [SerializeField] private Text _comboPercentText;
    [SerializeField] private Text _comboWaveText;
    private int _score;
    private Text[] _scoreNumbers = new Text[8];
    private bool _isPaused;
    public Canvas PauseCanvas;
    public Canvas GameCanvas;


    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        PauseCanvas.enabled = false;
        _isPaused = false;
        _currentWave = 1;
        exitText.enabled = false;
        _exitRoutine = ExitFromLab();
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _comboWave = 1;
        _comboPercent = 0f;
        _comboPercentText.text = _comboPercent.ToString();
        _comboWaveText.text = _comboWave.ToString();
        _score = 0;
        GameObject tempScoreLine = GameObject.Find("ScoreLine");
        for (int i = _scoreNumbers.Length - 1, k = 0; i >= 0; i--)
        {
            _scoreNumbers[k] = tempScoreLine.transform.GetChild(i).GetComponent<Text>();
            k++;
        }
        UpdateScore();
    }

    private void FixedUpdate()
    {
        _comboPercent -= _comboWave * 0.1f;
        if (_comboPercent > 100)
        {
            _comboWave++;
            _comboPercent = _comboPercent - 100;
            _comboWaveText.text = _comboWave.ToString();
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
                _comboWaveText.text = _comboWave.ToString();
            }
        }
        _comboPercentText.text = ((int)_comboPercent).ToString();
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
        int randomnum = 1; // TODO: Calculate num of enemies    
        for (int i = 0; i < _spawners.Length; i++)
        {
            _spawners[i].GetComponent<EnemySpawner>().SpawnEnemies(randomnum); // TODO: Random Number
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
}
