using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;


[RequireComponent(typeof(BoxCollider))]
public class XRPushButton : XRBaseInteractable
{
    public UnityEvent onPushedDown;
    [SerializeField] private float maximumPushDepth;
    [SerializeField] private float minimalPushDepth;
    private XRBaseInteractor pushInteractor = null;
    private bool previouslyPushed = false;
    private float oldPushPosition;

    protected override void OnEnable()
    {
        base.OnEnable();
        hoverEntered.AddListener(StartPush);
        hoverExited.AddListener(EndPush);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        hoverEntered.RemoveListener(StartPush);
        hoverExited.RemoveListener(EndPush);
    }


    private void Start()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        minimalPushDepth = transform.localPosition.y;
        //maximumPushDepth = transform.localPosition.y - (boxCollider.bounds.size.y * 0.55f);
    }


    private void StartPush(HoverEnterEventArgs arg0)
    {
        pushInteractor = arg0.interactor;
        oldPushPosition = GetLocalYPosition(arg0.interactor.transform.position);
        SetYPosition(maximumPushDepth);
    }

    private void EndPush(HoverExitEventArgs arg0)
    {
        pushInteractor = null;
        oldPushPosition = 0.0f;
        previouslyPushed = false;
        SetYPosition(minimalPushDepth);
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if (pushInteractor)
        {
            float newPushPosition = GetLocalYPosition(pushInteractor.transform.position);
            float pushDifference = oldPushPosition - newPushPosition;
            oldPushPosition = newPushPosition;

            float newPosition = transform.localPosition.y;
            SetYPosition(newPosition);
            CheckPress();

        }
    }

    private float GetLocalYPosition(Vector3 interactorPosition)
    {
        return transform.root.InverseTransformDirection(interactorPosition).y;
    }

    private void SetYPosition(float yPos)
    {
        Debug.Log("Setting y pos " + yPos);
        Vector3 newPosition = transform.localPosition;
        newPosition.y = Mathf.Clamp(yPos, maximumPushDepth, minimalPushDepth);
        transform.localPosition = newPosition;
    }

    private void CheckPress()
    {
        float inRange = Mathf.Clamp(transform.localPosition.y, maximumPushDepth, minimalPushDepth + 0.01f);
        bool isPushedDown = transform.localPosition.y == inRange;
        if (isPushedDown && !previouslyPushed)
            onPushedDown.Invoke();

        previouslyPushed = isPushedDown;
    }


}
