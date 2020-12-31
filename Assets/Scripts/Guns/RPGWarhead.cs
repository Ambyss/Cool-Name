using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGWarhead : MonoBehaviour
{
    public GameObject _smoke;
    public GameObject _explosion;
    private WarheadSmoke _warheadSmoke;
    private bool _isHit;

    void Start()
    {
        _warheadSmoke = Instantiate(_smoke, transform.position, Quaternion.identity).GetComponent<WarheadSmoke>();
        _isHit = false;
    }

    void FixedUpdate()
    {
        _warheadSmoke.SetPosition(transform.position);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag("Player") && !collider.CompareTag("Gun") && !collider.CompareTag("GameController") )
        {
            StartCoroutine(Death());
            Instantiate(_explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    IEnumerator Death()
    {
        _warheadSmoke.StopEmmiting();
        yield return new WaitForSeconds(.3f);
        Destroy(_warheadSmoke);
        Destroy(gameObject);
    }
}
