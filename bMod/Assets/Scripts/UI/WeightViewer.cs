using UnityEngine;
using UnityEngine.UI;

public class WeightViewer : MonoBehaviour
{
    public Slider slider;
    public Text txt;
    public Boss boss;
    
    void Start()
    {
        // slider.value = boss.pushWeight;
        txt.text = slider.value.ToString();
    }

    public void OnChange()
    {
        // boss.pushWeight = slider.value;
        txt.text = slider.value.ToString();
    }
}