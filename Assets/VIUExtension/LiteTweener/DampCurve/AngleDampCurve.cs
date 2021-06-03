using UnityEngine;

namespace HTC.UnityPlugin.Utility.LiteTweener
{
    public class AngleDampCurve
    {
        private readonly DampCurve c = new DampCurve();
        private bool dirty = true;

        public float StartValue
        {
            get { UpdateDirtyState(); return c.StartValue; }
            set { c.StartValue = value; SetDirty(); }
        }

        public float EndValue
        {
            get { UpdateDirtyState(); return c.EndValue; }
            set { c.EndValue = value; SetDirty(); }
        }

        public float StartSpeed
        {
            get { return c.StartSpeed; }
            set { c.StartSpeed = value; }
        }

        public float StartTime
        {
            get { return c.StartTime; }
            set { c.StartTime = value; ; }
        }

        public float EndTime
        {
            get { UpdateDirtyState(); return c.EndTime; }
        }

        public float MaxSmoothTime
        {
            get { return c.MaxSmoothTime; }
            set { c.MaxSmoothTime = value; }
        }

        public float MaxSpeed
        {
            get { UpdateDirtyState(); return c.MaxSpeed; }
        }

        public float Duration
        {
            get { UpdateDirtyState(); return c.Duration; }
            set { c.Duration = value; }
        }

        public void SetMaxSpeed(float value) { c.MaxSpeed = value; }

        private void SetDirty() { dirty = true; }
        private void ResetDirty() { dirty = false; }

        private void UpdateDirtyState()
        {
            if (dirty)
            {
                c.StartValue = Mathf.Repeat(c.StartValue, 360f);
                var positiveAngle = c.EndValue = Mathf.Repeat(c.EndValue, 360f);
                var positiveAngleDuration = c.Duration;

                c.EndValue -= 360f;
                var negativeAngleDuration = c.Duration;

                if (positiveAngleDuration < negativeAngleDuration)
                {
                    c.EndValue = positiveAngle;
                }

                ResetDirty();
            }
        }

        public float Evaluate(float time)
        {
            UpdateDirtyState();
            return c.Evaluate(time);
        }

        public float EvaluateSpeed(float time)
        {
            UpdateDirtyState();
            return c.EvaluateSpeed(time);
        }

        public float Evaluate(float time, out float speed)
        {
            UpdateDirtyState();
            return c.Evaluate(time, out speed);
        }
    }
}