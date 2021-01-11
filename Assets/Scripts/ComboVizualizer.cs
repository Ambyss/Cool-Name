using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboVizualizer : MonoBehaviour
{
    public GameObject Point;
    public Text Wave;
    
    public void UpdateCombo(float percent, int wave)
    {
        Point.transform.localPosition = new Vector3(percent, 0, -1);
        Wave.text = wave.ToString();
    }
}
