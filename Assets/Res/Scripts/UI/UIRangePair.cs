using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRangePair : MonoBehaviour
{
    [SerializeField, HideInInspector] UISlider m_meanSlider;
    [SerializeField, HideInInspector] UISlider m_stdSlider;
    [SerializeField, HideInInspector] TextMeshProUGUI m_label;

    public RangePair Value
    {
        get {return new RangePair(m_meanSlider.Value, m_stdSlider.Value);}
        set 
        {
            m_meanSlider.Value = value.mean;
            m_stdSlider.Value = value.std;
        }
    }

    public float Mean
    {
        get { return m_meanSlider.Value; }
        set { m_meanSlider.Value = value; }
    }

    public float Min
    {
        get { return Mean; }
        set { Mean = value; }
    }

    public float MaxMean
    {
        get { return m_meanSlider.Max; }
        set { m_meanSlider.Max = value; }
    }

    public float MinMean
    {
        get { return m_meanSlider.Min; }
        set { m_meanSlider.Min = value; }
    }

    public Slider MeanSlider
    {
        get { return m_meanSlider.slider; }
    }

    public float Std
    {
        get { return m_stdSlider.Value; }
        set { m_stdSlider.Value = value; }
    }

    public float Max
    {
        get { return Std; }
        set { Std = value; }
    }

    public float MaxStd
    {
        get { return m_stdSlider.Max; }
        set { m_stdSlider.Max = value; }
    }

    public float MinStd
    {
        get { return m_stdSlider.Min; }
        set { m_stdSlider.Min = value; }
    }

    public Slider StdSlider
    {
        get { return m_stdSlider.slider; }
    }

    private void OnValidate()
    {
        m_label.text = gameObject.name;
    }

    private void Start()
    {
        m_label.text = gameObject.name;
    }

    public void AddListener1(UnityEngine.Events.UnityAction<float> subscriber)
    {
        m_meanSlider.AddListener(subscriber);
    }

    public void AddListener2(UnityEngine.Events.UnityAction<float> subscriber)
    {
        m_stdSlider.AddListener(subscriber);
    }

    public void ClearListeners()
    {
        m_meanSlider.ClearListeners();
        m_stdSlider.ClearListeners();
    }
}
