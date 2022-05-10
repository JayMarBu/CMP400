using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    [SerializeField, HideInInspector] TextMeshProUGUI m_label;
    [SerializeField, HideInInspector] TextMeshProUGUI m_value;
    [SerializeField, HideInInspector] Slider m_slider;
    [SerializeField] Vector2 m_range = new Vector2();
    [SerializeField] bool m_int = false;

    public Slider slider
    {
        get { return m_slider; }
    }

    public float Value
    {
        get { return m_slider.value; }
        set { m_slider.value = value; }
    }

    public int ValueInt
    {
        get { return Mathf.RoundToInt(m_slider.value); }
        set { m_slider.value = value; }
    }

    public float Max
    {
        get 
        {
            m_slider.maxValue = m_range.y;
            return m_range.y; 
        }
        set 
        {
            m_range.y = value;
            m_slider.maxValue = value;
        }
    }

    public float Min
    {
        get
        {
            m_slider.minValue = m_range.x;
            return m_range.x;
        }
        set
        {
            m_range.x = value;
            m_slider.minValue = value;
        }
    }

    private void OnValidate()
    {
        m_label.text = gameObject.name;
        m_slider.maxValue = m_range.y;
        m_slider.minValue = m_range.x;
    }

    private void Start()
    {
        m_label.text = gameObject.name;
        m_slider.maxValue = m_range.y;
        m_slider.minValue = m_range.x;
    }

    private void Update()
    {
        if(m_int)
            m_value.text = Mathf.RoundToInt(m_slider.value).ToString();
        else
            m_value.text = m_slider.value.ToString();
    }

    public void AddListener(UnityEngine.Events.UnityAction<float> subscriber)
    {
        m_slider.onValueChanged.AddListener(subscriber);
    }

    public void ClearListeners()
    {
        m_slider.onValueChanged.RemoveAllListeners();
    }
}
