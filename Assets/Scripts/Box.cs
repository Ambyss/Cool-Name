using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour
{
    private Gun _gun;
    public GameObject AmmoText;
    private GameObject _ammoText;
    private Vector3 _transitionVector;
    private Color _transitionColor;
    private Color _textColor;

    private void Start()
    {
        _gun = GameObject.FindWithTag("Gun").GetComponent<Gun>();
        _transitionVector = new Vector3(0, 0.15f, 0);
        _transitionColor = new Color(0, 0, 0, -.04f);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
            PickUp();
    }

    private void PickUp()
    {
        _gun.AddAmmo();
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        _ammoText = Instantiate(AmmoText, transform.position, Quaternion.identity);
        StartCoroutine(TextFloat());
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
        Transform _camera = UnityEngine.Camera.main.GetComponent<Transform>();
        _ammoText.transform.rotation = Quaternion.Euler(Mathf.Atan((transform.position.z - _camera.position.z)/(_camera.position.y - transform.position.y)) * Mathf.Rad2Deg, 0, 0);
        yield return new WaitForSeconds(2);
        Destroy(_ammoText);
        Destroy(gameObject);
    }

    IEnumerator TextFloat()
    {
        _ammoText.GetComponent<SpriteRenderer>().color += _transitionColor;
        yield return new WaitForSeconds(0.05f);
        _ammoText.transform.Translate(_transitionVector);
        StartCoroutine(TextFloat());
    }
}
