#pragma warning disable 0649
using HTC.UnityPlugin.LiteCoroutineSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.Utility.LiteTweener
{
    public class TestDampCurve : MonoBehaviour
    {
        private DampCurve curve;

        public bool byMaxSpeed;
        public float vStart = 1f;
        public float vEnd = 5f;
        public float sStart = 1f;
        public float smooth = 5f;
        public float tStart = 0.1f;
        public float duration = 3f;
        public float maxSpeed = 1f;

        private void Start()
        {
            curve = new DampCurve();
        }

        private void Update()
        {
            curve.StartValue = vStart;
            curve.EndValue = vEnd;
            curve.StartSpeed = sStart;
            curve.MaxSmoothTime = smooth;
            curve.StartTime = tStart;

            if (byMaxSpeed)
            {
                curve.MaxSpeed = maxSpeed;
            }
            else
            {
                curve.Duration = duration;
            }

            curve.DebugDraw(transform.position, transform.right, transform.up);
        }
    }
}