using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Model : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private string example = null;

    [Header("Parameters")]
    [SerializeField] private int exampleConfig = 0;

    [Header("Unity Events")]
    [SerializeField] private UnityEvent onEvent = null;
}