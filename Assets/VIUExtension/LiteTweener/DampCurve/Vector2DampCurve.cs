#pragma warning disable 0649
using UnityEngine;

namespace HTC.UnityPlugin.Utility.LiteTweener
{
    public class Vector2DampCurve
    {
        private readonly DampCurve cX = new DampCurve();
        private readonly DampCurve cY = new DampCurve();
        private bool dirty = true;
        private bool normalizeSpeed = true;
        private float maxSpeed;

        public Vector2 StartValue
        {
            get { return new Vector2(cX.StartValue, cY.StartValue); }
            set { cX.StartValue = value.x; cY.StartValue = value.y; SetDirty(); }
        }

        public Vector2 EndValue
        {
            get { return new Vector2(cX.EndValue, cY.EndValue); }
            set { cX.EndValue = value.x; cY.EndValue = value.y; SetDirty(); }
        }

        public Vector2 StartSpeed
        {
            get { return new Vector2(cX.StartSpeed, cY.StartSpeed); }
            set { cX.StartSpeed = value.x; cY.StartSpeed = value.y; SetDirty(); }
        }

        public float StartTime
        {
            get { return cX.StartTime; }
            set { cX.StartTime = value; cY.StartTime = value; }
        }

        public float EndTime
        {
            get { UpdateDirtyState(); return cX.EndTime; }
        }

        public float MaxSmoothTime
        {
            get { return cX.MaxSmoothTime; }
            set { cX.MaxSmoothTime = value; cY.MaxSmoothTime = value; SetDirty(); }
        }

        public bool NormalizeSpeed { get { return normalizeSpeed; } set { if (normalizeSpeed != value) { normalizeSpeed = value; SetDirty(); } } }

        public Vector2 MaxSpeed
        {
            get { UpdateDirtyState(); return new Vector2(cX.MaxSpeed, cY.MaxSpeed); }
        }

        public float Duration
        {
            get { UpdateDirtyState(); return cX.Duration; }
            set { cX.Duration = value; cY.Duration = value; ResetDirty(); }
        }

        public void SetMaxSpeed(float value) { maxSpeed = value; SetDirty(); }

        private void SetDirty() { dirty = true; }
        private void ResetDirty() { dirty = false; }

        private void UpdateDirtyState()
        {
            if (dirty)
            {
                if (normalizeSpeed)
                {
                    var speed = new Vector2(cX.EndValue - cX.StartValue, cY.EndValue - cY.StartValue).normalized * maxSpeed;
                    cX.MaxSpeed = speed.x;
                    cY.MaxSpeed = speed.y;
                }
                else
                {
                    cX.MaxSpeed = maxSpeed;
                    cY.MaxSpeed = maxSpeed;
                }

                var maxDuration = Mathf.Max(cX.Duration, cY.Duration);
                cX.Duration = maxDuration;
                cY.Duration = maxDuration;

                ResetDirty();
            }
        }

        public Vector2 Evaluate(float time)
        {
            UpdateDirtyState();
            return new Vector2(cX.Evaluate(time), cY.Evaluate(time));
        }

        public Vector2 EvaluateSpeed(float time)
        {
            UpdateDirtyState();
            return new Vector2(cX.EvaluateSpeed(time), cY.EvaluateSpeed(time));
        }

        public Vector2 Evaluate(float time, out Vector2 speed)
        {
            UpdateDirtyState();
            return new Vector2(cX.Evaluate(time, out speed.x), cY.Evaluate(time, out speed.y));
        }
    }
}