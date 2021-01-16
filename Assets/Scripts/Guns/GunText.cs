using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunText : MonoBehaviour
{
    private TextMesh _text;
    private IEnumerator _coroutine;
    private Color _colorTransition;
    private Color _trueColor;
    
    void Awake()
    {
        _text = GetComponent<TextMesh>();
        _coroutine = Hide();
        _colorTransition = new Color(0, 0, 0, -0.03f);
        _trueColor = _text.color;
    }
    
    public void ChangeGun(string name)
    {
        print(name);
        _text.text = name;
        StopAllCoroutines();
        StartCoroutine(Hide());
    }

    IEnumerator Hide()
    {
        _text.color = _trueColor;
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            _text.color += _colorTransition;
            if (_text.color.a <= 0.05f)
                break;
        }
    }
}

