using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField]
    private RectTransform _background;
    [SerializeField]
    private RectTransform _trigger;
    [SerializeField]
    private Canvas _canvas;
    [SerializeField]
    private float _triggerAllowableDistance = 100;

    private Camera _inputCamera;

    public Vector2 InputDirection { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        _background.gameObject.SetActive(true);
        _background.position = eventData.position;
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _background.gameObject.SetActive(false);
        InputDirection = Vector2.zero;
    }

    private void Awake()
    {
        _inputCamera = _canvas.worldCamera;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 backgroundPosition = _background.position;
        Vector2 inputDirection = eventData.position - backgroundPosition;
        float inputMagnitude = inputDirection.magnitude;
        _trigger.position = backgroundPosition +
                inputDirection.normalized  * FormatInputMagnitude(inputMagnitude);
        InputDirection = inputDirection.normalized * GetInputMagnitudeNormalized(inputMagnitude);
    }

    private float FormatInputMagnitude(float inputMagnitude)
    {
        return inputMagnitude > _triggerAllowableDistance ? _triggerAllowableDistance : inputMagnitude;
    }

    private float GetInputMagnitudeNormalized(float inputMagnitude)
    {
        if (inputMagnitude > _triggerAllowableDistance)
        {
            return 1;
        }
        else
        {
            return inputMagnitude / _triggerAllowableDistance;
        }
    }
}
