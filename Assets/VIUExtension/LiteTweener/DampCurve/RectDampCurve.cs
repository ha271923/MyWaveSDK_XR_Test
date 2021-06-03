using UnityEngine;

namespace HTC.UnityPlugin.Utility.LiteTweener
{
    public class RectDampCurve
    {
        private readonly Vector2DampCurve cPos = new Vector2DampCurve() { NormalizeSpeed = true };
        private readonly Vector2DampCurve cSize = new Vector2DampCurve() { NormalizeSpeed = false };
        private bool dirty = true;

        public Rect StartValue
        {
            get { return new Rect(cPos.StartValue, cSize.StartValue); }
            set { cPos.StartValue = value.position; cSize.StartValue = value.size; SetDirty(); }
        }

        public Rect EndValue
        {
            get { return new Rect(cPos.EndValue, cSize.EndValue); }
            set { cPos.EndValue = value.position; cSize.EndValue = value.size; SetDirty(); }
        }

        public Rect StartSpeed
        {
            get { return new Rect(cPos.StartSpeed, cSize.StartSpeed); }
            set { cPos.StartSpeed = value.position; cSize.StartSpeed = value.size; SetDirty(); }
        }

        public float StartTime
        {
            get { return cPos.StartTime; }
            set { cPos.StartTime = value; cSize.StartTime = value; }
        }

        public float EndTime
        {
            get { UpdateDirtyState(); return cPos.EndTime; }
        }

        public float MaxSmoothTime
        {
            get { return cPos.MaxSmoothTime; }
            set { cPos.MaxSmoothTime = value; cSize.MaxSmoothTime = value; SetDirty(); }
        }

        public Rect MaxSpeed
        {
            get { return new Rect(cPos.MaxSpeed, cSize.MaxSpeed); }
        }

        public float Duration
        {
            get { UpdateDirtyState(); return cPos.Duration; }
            set { cPos.Duration = value; cSize.Duration = value; ResetDirty(); }
        }

        public void SetMaxSpeed(float value) { cPos.SetMaxSpeed(value); cSize.SetMaxSpeed(value); SetDirty(); }

        private void SetDirty() { dirty = true; }
        private void ResetDirty() { dirty = false; }

        private void UpdateDirtyState()
        {
            if (dirty)
            {
                var maxDuration = Mathf.Max(cPos.Duration, cSize.Duration);
                cPos.Duration = maxDuration;
                cSize.Duration = maxDuration;

                ResetDirty();
            }
        }

        public Rect Evaluate(float time)
        {
            UpdateDirtyState();
            return new Rect(cPos.Evaluate(time), cSize.Evaluate(time));
        }

        public Rect EvaluateSpeed(float time)
        {
            UpdateDirtyState();
            return new Rect(cPos.EvaluateSpeed(time), cSize.EvaluateSpeed(time));
        }

        public Rect Evaluate(float time, out Rect speed)
        {
            UpdateDirtyState();
            var result = new Rect(cPos.Evaluate(time, out var posSpeed), cSize.Evaluate(time, out var sizeSpeed));
            speed = new Rect(posSpeed, sizeSpeed);
            return result;
        }
    }
}