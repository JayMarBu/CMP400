using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIController : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] Canvas m_genCanvas;
    [SerializeField] Canvas m_renderCanvas;

    [Header("Gen Params")]
    [SerializeField] UISlider m_V_init;
    [SerializeField] UISlider m_P_init;
    [SerializeField] UISlider m_P_m;

    [SerializeField] UIRangePair m_Angle;

    [SerializeField] UIRangePair m_G_L;
    [SerializeField] UIRangePair m_G_A;

    [SerializeField] UISlider m_JFIterations;
    [SerializeField] UISlider m_BeaIterations;

    [Header("Render Params")]
    [SerializeField] UISlider m_diameterthinner;
    [SerializeField] UISlider m_tesslCount;
    [SerializeField] UISlider m_tesslSize;
    [SerializeField] UIRangePair m_tesslDepth;
    [SerializeField] TMP_Dropdown m_tesslMode;


    GenerationParameters m_params;
    [SerializeField] GenerationParameters m_defaultParams;

    public enum State
    {
        None,
        Gen,
        Render
    }

    public State m_state;

    private void Start()
    {
        m_genCanvas.enabled = false;
        m_renderCanvas.enabled = false;


        SetInitialValues();
    }

    private void OnEnable()
    {
        m_V_init.AddListener((float val) => { m_params.V_init = val; });

        m_P_init.AddListener((float val) => { m_params.P_init = val; });
        m_P_m.AddListener((float val) => { m_params.P_m = val; });

        m_Angle.AddListener1((float val) => { m_params.Angle.mean = val; });
        m_Angle.AddListener2((float val) => { m_params.Angle.std = val; });

        m_G_L.AddListener1((float val)=> { m_params.gasProperties.L.mean = val; });
        m_G_L.AddListener2((float val)=> { m_params.gasProperties.L.std = val; });
        m_G_A.AddListener1((float val) => { m_params.gasProperties.A.mean = val; });
        m_G_A.AddListener2((float val) => { m_params.gasProperties.A.std = val; });

        m_JFIterations.AddListener((float val) => { m_params.iterations = Mathf.RoundToInt(val); });
        m_BeaIterations.AddListener((float val) => { m_params.maxRecursionDepth = Mathf.RoundToInt(val); });

        m_diameterthinner.AddListener((float val) => { m_params.diameterThinner = val; });
        m_tesslCount.AddListener((float val) => { m_params.jitterPerUnit = val; });
        m_tesslSize.AddListener((float val) => { m_params.jitterSizeModifier = val; });

        m_tesslDepth.AddListener1((float val) => { m_params.jitterMinDepth = Mathf.RoundToInt(val); });
        m_tesslDepth.AddListener2((float val) => { m_params.jitterMaxDepth = Mathf.RoundToInt(val); });

        m_tesslMode.onValueChanged.AddListener((int val) => 
        {
            switch (val)
            {
                default:
                case 0:
                    m_params.jitterMode = TeselationMod.None;
                    break;
                case 1:
                    m_params.jitterMode = TeselationMod.Jitter;
                    break;
                case 2:
                    m_params.jitterMode = TeselationMod.Random_Offset;
                    break;
                
            }
        });
    }

    private void OnDisable()
    {
        m_V_init.ClearListeners();

        m_P_init.ClearListeners();
        m_P_m.ClearListeners();

        m_Angle.ClearListeners();

        m_G_L.ClearListeners();
        m_G_A.ClearListeners();

        m_JFIterations.ClearListeners();
        m_BeaIterations.ClearListeners();

        m_diameterthinner.ClearListeners();
        m_tesslCount.ClearListeners();
        m_tesslSize.ClearListeners();

        m_tesslDepth.ClearListeners();

        m_tesslMode.onValueChanged.RemoveAllListeners();
    }

    void SetInitialValues()
    {
        m_params = GenerationManager.Instance.Params;

        m_defaultParams = new GenerationParameters(m_params);

        m_V_init.Value              = m_params.V_init;
        m_P_init.Value              = m_params.P_init;
        m_P_m.Value                 = m_params.P_m;

        m_Angle.Value               = m_params.Angle;

        m_G_L.Value                 = m_params.gasProperties.L;
        m_G_A.Value                 = m_params.gasProperties.A;

        m_JFIterations.ValueInt     = m_params.iterations;
        m_BeaIterations.ValueInt    = m_params.maxRecursionDepth;

        m_diameterthinner.Value     = m_params.diameterThinner;
        m_tesslCount.Value          = m_params.jitterPerUnit;
        m_tesslSize.Value           = m_params.jitterSizeModifier;
        m_tesslDepth.Max            = m_params.jitterMaxDepth;
        m_tesslDepth.Min            = m_params.jitterMinDepth;
        m_tesslMode.value           = (int)m_params.jitterMode;
    }

    void SetState(State state)
    {
        m_state = state;

        switch (m_state)
        {
            case State.None:
                m_genCanvas.enabled = false;
                m_renderCanvas.enabled = false;
                break;
            case State.Gen:
                m_genCanvas.enabled = true;
                m_renderCanvas.enabled = false;
                break;
            case State.Render:
                m_genCanvas.enabled = false;
                m_renderCanvas.enabled = true;
                break;
        }
    }

    public void OnRenderParams()
    {
        if(m_state == State.Render)
        {
            SetState(State.None);
        }
        else
        {
            SetState(State.Render);
        }
    }

    public void OnGenParams()
    {
        if (m_state == State.Gen)
        {
            SetState(State.None);
        }
        else
        {
            SetState(State.Gen);
        }
    }

    public void OnReset()
    {
        m_params.SetParams(m_defaultParams);
    }
}
