using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class OnEnableDo : MonoBehaviour
{
    public UnityEvent OnEnableEvnet;
    void OnEnable()
    {
        OnEnableEvnet.Invoke();
    }
}