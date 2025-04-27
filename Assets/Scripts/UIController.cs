using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider resolutionSlider;
    [SerializeField] private TMP_Dropdown functionDropdown;
    [SerializeField] private Toggle hasTransitionToggle;
    [SerializeField] private GameObject transitionPanel;
    [SerializeField] private TMP_InputField transitionDurationInput;
    [SerializeField] private TMP_InputField functionDuration;
    [SerializeField] private TMP_Dropdown functionTransitionDropdown;

    [Header("References")]
    [SerializeField] private Graph graph;

    private void Awake() 
    {
        resolutionSlider.value = graph.Resolution;
        resolutionSlider.wholeNumbers = true;
        resolutionSlider.onValueChanged.AddListener(OnResolutionChanged);

        SetDropdownOptionsByEnum<FunctionType>(functionDropdown);
        functionDropdown.value = (int)graph.FunctionType;
        functionDropdown.onValueChanged.AddListener(OnFunctionChangedInUI);
        graph.OnFunctionChanged += OnFunctionChanged;

        SetDropdownOptionsByEnum<TransitionMode>(functionTransitionDropdown);
        functionTransitionDropdown.value = (int)graph.FunctionTransition;
        functionTransitionDropdown.onValueChanged.AddListener(OnFunctionTransitionChanged);

        hasTransitionToggle.isOn = graph.HasTransition;
        hasTransitionToggle.onValueChanged.AddListener(OnHasTransitionChanged);
        transitionPanel.gameObject.SetActive(graph.HasTransition);

        transitionDurationInput.contentType = TMP_InputField.ContentType.DecimalNumber;
        transitionDurationInput.text = graph.TransitionDuration.ToString();
        transitionDurationInput.onEndEdit.AddListener(OnTransitionDurationChanged);

        functionDuration.contentType = TMP_InputField.ContentType.DecimalNumber;
        functionDuration.text = graph.FunctionDuration.ToString();
        functionDuration.onEndEdit.AddListener(OnFunctionDurationChanged);
    }

    private void OnFunctionChanged(FunctionType function)
    {
        functionDropdown.SetValueWithoutNotify((int)function);
    }

    private void OnFunctionDurationChanged(string text)
    {
        graph.FunctionDuration = Convert.ToSingle(text);
    }

    private void OnTransitionDurationChanged(string text)
    {
        graph.TransitionDuration = Convert.ToSingle(text);
    }

    private void OnHasTransitionChanged(bool hasTransition)
    {
        graph.HasTransition = hasTransition;
        transitionPanel.gameObject.SetActive(hasTransition);
    }

    private void OnFunctionTransitionChanged(int index)
    {
        graph.FunctionTransition = (TransitionMode)index;
    }

    private void OnFunctionChangedInUI(int index)
    {
        graph.FunctionType = (FunctionType)index;
    }

    private void SetDropdownOptionsByEnum<T>(TMP_Dropdown dropdown) where T : Enum 
    {
        dropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (var function in Enum.GetValues(typeof(T)))
        {
            options.Add(new TMP_Dropdown.OptionData(function.ToString()));
        }

        dropdown.AddOptions(options);
    }

    private void OnResolutionChanged(float resolution)
    {
        graph.Resolution = (int)resolution;
    }
}
