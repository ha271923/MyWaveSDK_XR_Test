#pragma warning disable 0649
using UnityEngine;

namespace HTC.UnityPlugin.Utility.LiteTweener
{
    public class Vector4DampCurve
    {
        private readonly DampCurve cX = new DampCurve();
        private readonly DampCurve cY = new DampCurve();
        private readonly DampCurve cZ = new DampCurve();
        private readonly DampCurve cW = new DampCurve();
        private bool dirty = true;
        private bool normalizeSpeed = true;
        private float maxSpeed;

        public Vector4 StartValue
        {
            get { return new Vector4(cX.StartValue, cY.StartValue, cZ.StartValue, cW.StartValue); }
            set { cX.StartValue = value.x; cY.StartValue = value.y; cZ.StartValue = value.z; cW.StartValue = value.w; SetDirty(); }
        }

        public Vector4 EndValue
        {
            get { return new Vector4(cX.EndValue, cY.EndValue, cZ.EndValue, cW.EndValue); }
            set { cX.EndValue = value.x; cY.EndValue = value.y; cZ.EndValue = value.z; cW.EndValue = value.w; SetDirty(); }
        }

        public Vector4 StartSpeed
        {
            get { return new Vector4(cX.StartSpeed, cY.StartSpeed, cZ.StartSpeed, cW.StartSpeed); }
            set { cX.StartSpeed = value.x; cY.StartSpeed = value.y; cZ.StartSpeed = value.z; cW.StartSpeed = value.w; SetDirty(); }
        }

        public float StartTime
        {
            get { return cX.StartTime; }
            set { cX.StartTime = value; cY.StartTime = value; cZ.StartTime = value; cW.StartTime = value; }
        }

        public float EndTime
        {
            get { UpdateDirtyState(); return cX.EndTime; }
        }

        public float MaxSmoothTime
        {
            get { return cX.MaxSmoothTime; }
            set { cX.MaxSmoothTime = value; cY.MaxSmoothTime = value; cZ.MaxSmoothTime = value; cW.MaxSmoothTime = value; SetDirty(); }
        }

        public bool NormalizeSpeed { get { return normalizeSpeed; } set { if (normalizeSpeed != value) { normalizeSpeed = value; SetDirty(); } } }

        public Vector4 MaxSpeed
        {
            get { UpdateDirtyState(); return new Vector4(cX.MaxSpeed, cY.MaxSpeed, cZ.MaxSpeed, cW.MaxSpeed); }
        }

        public float Duration
        {
            get { UpdateDirtyState(); return cX.Duration; }
            set { cX.Duration = value; cY.Duration = value; cZ.Duration = value; cW.Duration = value; ResetDirty(); }
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
                    var speed = new Vector4(cX.EndValue - cX.StartValue, cY.EndValue - cY.StartValue, cZ.EndValue - cZ.StartValue, cW.EndValue - cW.StartValue).normalized * maxSpeed;
                    cX.MaxSpeed = speed.x;
                    cY.MaxSpeed = speed.y;
                    cZ.MaxSpeed = speed.z;
                    cW.MaxSpeed = speed.w;
                }
                else
                {
                    cX.MaxSpeed = maxSpeed;
                    cY.MaxSpeed = maxSpeed;
                    cZ.MaxSpeed = maxSpeed;
                    cW.MaxSpeed = maxSpeed;
                }

                var maxDuration = Mathf.Max(cX.Duration, cY.Duration, cZ.Duration, cW.Duration);
                cX.Duration = maxDuration;
                cY.Duration = maxDuration;
                cZ.Duration = maxDuration;
                cW.Duration = maxDuration;

                ResetDirty();
            }
        }

        public Vector4 Evaluate(float time)
        {
            UpdateDirtyState();
            return new Vector4(cX.Evaluate(time), cY.Evaluate(time), cZ.Evaluate(time), cW.Evaluate(time));
        }

        public Vector4 EvaluateSpeed(float time)
        {
            UpdateDirtyState();
            return new Vector4(cX.EvaluateSpeed(time), cY.EvaluateSpeed(time), cZ.EvaluateSpeed(time), cW.EvaluateSpeed(time));
        }

        public Vector4 Evaluate(float time, out Vector4 speed)
        {
            UpdateDirtyState();
            return new Vector4(cX.Evaluate(time, out speed.x), cY.Evaluate(time, out speed.y), cZ.Evaluate(time, out speed.z), cW.Evaluate(time, out speed.w));
        }
    }
}