using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    private Transform _player;
    public Transform _camera;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        //_camera = UnityEngine.Camera.main.GetComponent<Transform>();
        StartCoroutine(ChangeView());
        transform.rotation = Quaternion.Euler(Mathf.Atan((transform.position.z - _camera.position.z)/(_camera.position.y - transform.position.y)) * Mathf.Rad2Deg, 0, 0);
    }

    private void Update()
    {
        transform.position = new Vector3(_player.position.x, _player.position.y + 3, _player.position.z); 
    }

    IEnumerator ChangeView()
    {
        yield return new WaitForSeconds(.01f);
        transform.rotation = Quaternion.Euler(Mathf.Atan((transform.position.z - _camera.position.z)/(_camera.position.y - transform.position.y)) * Mathf.Rad2Deg, 0, 0);
    }
}
