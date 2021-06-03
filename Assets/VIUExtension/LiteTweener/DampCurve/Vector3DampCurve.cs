using UnityEngine;

namespace HTC.UnityPlugin.Utility.LiteTweener
{
    public class Vector3DampCurve
    {
        private readonly DampCurve cX = new DampCurve();
        private readonly DampCurve cY = new DampCurve();
        private readonly DampCurve cZ = new DampCurve();
        private bool dirty = true;
        private bool normalizeSpeed = true;
        private float maxSpeed;

        public Vector3 StartValue
        {
            get { return new Vector3(cX.StartValue, cY.StartValue, cZ.StartValue); }
            set { cX.StartValue = value.x; cY.StartValue = value.y; cZ.StartValue = value.z; SetDirty(); }
        }

        public Vector3 EndValue
        {
            get { return new Vector3(cX.EndValue, cY.EndValue, cZ.EndValue); }
            set { cX.EndValue = value.x; cY.EndValue = value.y; cZ.EndValue = value.z; SetDirty(); }
        }

        public Vector3 StartSpeed
        {
            get { return new Vector3(cX.StartSpeed, cY.StartSpeed, cZ.StartSpeed); }
            set { cX.StartSpeed = value.x; cY.StartSpeed = value.y; cZ.StartSpeed = value.z; SetDirty(); }
        }

        public float StartTime
        {
            get { return cX.StartTime; }
            set { cX.StartTime = value; cY.StartTime = value; cZ.StartTime = value; }
        }

        public float EndTime
        {
            get { UpdateDirtyState(); return cX.EndTime; }
        }

        public float MaxSmoothTime
        {
            get { return cX.MaxSmoothTime; }
            set { cX.MaxSmoothTime = value; cY.MaxSmoothTime = value; cZ.MaxSmoothTime = value; SetDirty(); }
        }

        public bool NormalizeSpeed { get { return normalizeSpeed; } set { if (normalizeSpeed != value) { normalizeSpeed = value; SetDirty(); } } }

        public Vector3 MaxSpeed
        {
            get { UpdateDirtyState(); return new Vector3(cX.MaxSpeed, cY.MaxSpeed, cZ.MaxSpeed); }
        }

        public float Duration
        {
            get { UpdateDirtyState(); return cX.Duration; }
            set { cX.Duration = value; cY.Duration = value; cZ.Duration = value; ResetDirty(); }
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
                    var speed = new Vector3(cX.EndValue - cX.StartValue, cY.EndValue - cY.StartValue, cZ.EndValue - cZ.StartValue).normalized * maxSpeed;
                    cX.MaxSpeed = speed.x;
                    cY.MaxSpeed = speed.y;
                    cZ.MaxSpeed = speed.z;
                }
                else
                {
                    cX.MaxSpeed = maxSpeed;
                    cY.MaxSpeed = maxSpeed;
                    cZ.MaxSpeed = maxSpeed;
                }

                var maxDuration = Mathf.Max(cX.Duration, cY.Duration, cZ.Duration);
                cX.Duration = maxDuration;
                cY.Duration = maxDuration;
                cZ.Duration = maxDuration;

                ResetDirty();
            }
        }

        public Vector3 Evaluate(float time)
        {
            UpdateDirtyState();
            return new Vector3(cX.Evaluate(time), cY.Evaluate(time), cZ.Evaluate(time));
        }

        public Vector3 EvaluateSpeed(float time)
        {
            UpdateDirtyState();
            return new Vector3(cX.EvaluateSpeed(time), cY.EvaluateSpeed(time), cZ.EvaluateSpeed(time));
        }

        public Vector3 Evaluate(float time, out Vector3 speed)
        {
            UpdateDirtyState();
            return new Vector3(cX.Evaluate(time, out speed.x), cY.Evaluate(time, out speed.y), cZ.Evaluate(time, out speed.z));
        }
    }
}