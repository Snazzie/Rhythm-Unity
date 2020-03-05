using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModSliderControl : MonoBehaviour
{
    [SerializeField]
    private float sliderMin = 0;
    [SerializeField]
    private float sliderMax = 2;

    public float Value { get; private set; } = 1;
    public Slider ChildSlider { get; private set; }
    public Action OnSliderValueChanged { get; set; }

    private InputField valueInput { get; set; }
    // Start is called before the first frame update


    void Start()
    {
        ChildSlider = transform.Find("Slider").transform.GetComponent<Slider>();
        ChildSlider.minValue = sliderMin;
        ChildSlider.maxValue = sliderMax;
        ChildSlider.value = Value;
        ChildSlider.onValueChanged.AddListener(delegate {
            Value = ChildSlider.value;
            OnSliderValueChanged.Invoke();
        });

        valueInput = transform.Find("ValueInput").transform.GetComponent<InputField>();
        valueInput.onValueChanged.AddListener(delegate { 
            Value = float.Parse(valueInput.text);
        });

    }

    void updateControls()
    {
        valueInput.text = Value.ToString();
        ChildSlider.value = Value;
    }

    // Update is called once per frame
    void Update()
    {
        updateControls();
    }
}
