using UnityEngine;

namespace HTC.UnityPlugin.Utility.LiteTweener
{
    public class DampCurve
    {
        private enum DirtyState
        {
            UpToDate,
            DurationDirty,
            MaxSpeedDirty,
        }

        public const float MIN_MAXSPEED = 0.0000001f;
        public const float DEFAULT_MAXSPEED = 10f;
        public const float DEFAULT_SMOOTHTIME = 0.1f;

        public float StartValue { get { return v0; } set { if (v0 != value) { v0 = value; SetDirty(); } } }
        public float EndValue { get { return v3; } set { if (v3 != value) { v3 = value; SetDirty(); } } }
        public float StartSpeed { get { return s0; } set { if (s0 != value) { s0 = value; SetDirty(); } } }
        public float EndSpeed { get { return 0f; } }
        public float StartTime { get { return t0; } set { if (t0 != value) { var dt = value - t0; t0 = value; t1 += dt; t2 += dt; t3 += dt; } } }
        public float EndTime { get { if (dirtyState == DirtyState.DurationDirty) { UpdateDirtyState(); } return t3; } }
        public float MaxSmoothTime { get { return maxSmoothTime; } set { value = Mathf.Max(value, 0f); if (maxSmoothTime != value) { maxSmoothTime = value; SetDirty(); } } }

        public float MaxSpeed
        {
            get
            {
                if (dirtyState == DirtyState.MaxSpeedDirty) { UpdateDirtyState(); }
                return maxSpeed;
            }
            set
            {
                maxSpeed = Mathf.Max(Mathf.Abs(value), MIN_MAXSPEED);
                SetDurationDirty();
            }
        }

        public float Duration
        {
            get
            {
                if (dirtyState == DirtyState.DurationDirty) { UpdateDirtyState(); }
                return t3 - t0;
            }
            set
            {
                t3 = t0 + Mathf.Max(0f, value);
                SetMaxSpeedDirty();
            }
        }

        private DirtyState dirtyState = DirtyState.MaxSpeedDirty;
        private float v0;
        private float v1;
        private float v2;
        private float v3;
        private float s0;
        private float s1;
        private float s2 { get { return s1; } set { s1 = value; } }
        private const float s3 = 0f;
        private float t0;
        private float t1;
        private float t2;
        private float t3;

        private float maxSpeed = DEFAULT_MAXSPEED; // > 0
        private float maxSmoothTime = DEFAULT_SMOOTHTIME; // >= 0

        private void SetDirty() { if (dirtyState == DirtyState.UpToDate) { dirtyState = DirtyState.MaxSpeedDirty; } }
        private void SetDurationDirty() { dirtyState = DirtyState.DurationDirty; }
        private void SetMaxSpeedDirty() { dirtyState = DirtyState.MaxSpeedDirty; ; }
        private void ResetDirty() { dirtyState = DirtyState.UpToDate; }

        private void UpdateDirtyState()
        {
            switch (dirtyState)
            {
                case DirtyState.DurationDirty: CalculateCurveByMaxSpeed(); break;
                case DirtyState.MaxSpeedDirty: CalculateCurveByDuration(); break;
            }
        }

        private static bool SignEqual(float a, float b) { return (a == 0f || b == 0f) ? true : (a > 0f == b > 0f); }
        private static float Sqr(float v) { return v * v; }
        private static float QuadraticFormula(bool hiDelta, float a, float b, float c)
        {
            var delta = Mathf.Sqrt(Sqr(b) - 4f * a * c);
            if (!hiDelta) { delta = -delta; }
            return (-b + delta) / (2f * a);
        }

        public void CalculateCurveByMaxSpeed()
        {
            if ((Mathf.Approximately(v0, v3) && Mathf.Approximately(s0, 0f)) || float.IsInfinity(maxSpeed))
            {
                v1 = v0;
                v2 = v3;
                s1 = s0;
                t3 = t2 = t1 = t0;
            }
            else
            {
                var distance = v3 - v0;
                var sMax = Mathf.Sign(distance) * maxSpeed;
                var dMaxSmooth0 = (s0 + sMax) * maxSmoothTime * 0.5f;
                var dMaxSmooth2 = sMax * maxSmoothTime * 0.5f;
                var dMaxSmooth = dMaxSmooth0 + dMaxSmooth2;

                if (!SignEqual(dMaxSmooth, distance) || Mathf.Abs(dMaxSmooth) <= Mathf.Abs(distance))
                {
                    var dUniform = distance - dMaxSmooth0 - dMaxSmooth2;
                    var tUniform = dUniform / sMax;

                    v1 = v0 + dMaxSmooth0;
                    v2 = v1 + dUniform;

                    s1 = sMax;

                    t1 = t0 + maxSmoothTime;
                    t2 = t1 + tUniform;
                    t3 = t2 + maxSmoothTime;
                }
                else
                {
                    var scale = QuadraticFormula(distance > 0f, 2f * maxSmoothTime * sMax, s0 * maxSmoothTime, -2f * distance);
                    var tSmooth = maxSmoothTime * scale;
                    sMax *= scale;

                    v1 = v0 + (s0 + sMax) * tSmooth * 0.5f;
                    v2 = v1;

                    s1 = sMax;

                    t1 = t0 + tSmooth;
                    t2 = t1;
                    t3 = t2 + tSmooth;
                }
            }

            ResetDirty();
        }

        public void CalculateCurveByDuration()
        {
            var distance = v3 - v0;
            var duration = t3 - t0;

            if (duration <= 2f * maxSmoothTime)
            {
                var tSmooth = duration * 0.5f;
                var sMax = (distance / tSmooth) - (s0 * 0.5f);

                v1 = v0 + (s0 + sMax) * tSmooth * 0.5f;
                v2 = v1;

                s1 = sMax;
                maxSpeed = Mathf.Abs(sMax);

                t1 = t0 + tSmooth;
                t2 = t1;
            }
            else
            {
                var sMax = (2f * distance - s0 * maxSmoothTime) / (2f * (duration - maxSmoothTime));
                var tSmooth = duration - 2f * maxSmoothTime;

                v1 = v0 + (s0 + sMax) * maxSmoothTime * 0.5f;
                v2 = v1 + sMax * tSmooth;

                s1 = sMax;
                maxSpeed = Mathf.Abs(sMax);

                t1 = t0 + maxSmoothTime;
                t2 = t1 + tSmooth;
            }

            ResetDirty();
        }

        public float Evaluate(float time)
        {
            UpdateDirtyState();

            if (time >= t3) { return v3; }
            if (time >= t2) { return v2 + (s2 + EvaluateSpeed(time)) * (time - t2) * 0.5f; }
            if (time >= t1) { return v1 + s1 * (time - t1); }
            if (time >= t0) { return v0 + (s0 + EvaluateSpeed(time)) * (time - t0) * 0.5f; }
            return v0;
        }

        public float EvaluateSpeed(float time)
        {
            UpdateDirtyState();

            if (time >= t3) { return 0f; }
            if (time >= t2) { return (s3 - s2) * (time - t2) / (t3 - t2) + s2; }
            if (time >= t1) { return s1; }
            if (time >= t0) { return (s1 - s0) * (time - t0) / (t1 - t0) + s0; }
            return s0;
        }

        public float Evaluate(float time, out float speed)
        {
            UpdateDirtyState();

            if (time >= t3) { speed = 0f; return v3; }
            if (time >= t2) { speed = (s3 - s2) * (time - t2) / (t3 - t2) + s2; return v2 + (s2 + speed) * (time - t2) * 0.5f; }
            if (time >= t1) { speed = s1; return v1 + s1 * (time - t1); }
            if (time >= t0) { speed = (s1 - s0) * (time - t0) / (t1 - t0) + s0; return v0 + (s0 + speed) * (time - t0) * 0.5f; }
            speed = s0; return v0;
        }

        public void DebugDraw(Vector3 origin, Vector3 timeUnit, Vector3 speedUnit)
        {
            UpdateDirtyState();

            var p0 = origin + s0 * speedUnit;
            var p1 = origin + s1 * speedUnit + (t1 - t0) * timeUnit;
            var p2 = origin + s2 * speedUnit + (t2 - t0) * timeUnit;
            var p3 = origin + (t3 - t0) * timeUnit;
            SafeDrawLine(origin, p0, Color.red);
            SafeDrawLine(origin, p3, Color.blue);
            SafeDrawLine(p0, p1, Color.green);
            SafeDrawLine(p1, p2, Color.green);
            SafeDrawLine(p2, p3, Color.green);

            for (float t = t0, step = 0.02f, tmax = Mathf.Min(1000f, t3 - step + Mathf.Epsilon); t <= tmax; t += step)
            {
                var ps = origin + (Evaluate(t) - v0) * speedUnit + (t - t0) * timeUnit;
                var te = Mathf.Min(t3, t + step);
                var pe = origin + (Evaluate(te) - v0) * speedUnit + (te - t0) * timeUnit;
                SafeDrawLine(ps, pe, Color.yellow);
            }
        }

        private static void SafeDrawLine(Vector3 a, Vector3 b, Color color)
        {
            if ((a - b).sqrMagnitude < 10000f)
            {
                Debug.DrawLine(a, b, color);
            }
        }
    }
}