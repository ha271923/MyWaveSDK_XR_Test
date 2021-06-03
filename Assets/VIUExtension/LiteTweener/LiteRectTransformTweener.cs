#pragma warning disable 0649
using HTC.UnityPlugin.LiteCoroutineSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.Utility.LiteTweener
{
    public class LiteRectTransformTweener : MonoBehaviour
    {
        [Serializable]
        public struct TweenTarget
        {
            public RectTransform rectObj;
            public Rect rect;
            public UnityEvent onTargetReached;

            public Rect ToTarget(RectTransform rectTransform)
            {
                return rectObj != null ? GetTargetRect(rectTransform, rectObj) : rect;
            }
        }

        [Serializable]
        public struct DampCurveParam
        {
            public float maxSpeed;
            public float smoothTime;
        }

        [Serializable]
        public struct SmoothDampParam
        {
            public float maxSpeed;
            public float smoothTime;
        }

        [Serializable]
        public struct EaseParam
        {
            public float duration;
        }

        [Serializable]
        public struct LinearParam
        {
            public float speed;
        }

        [SerializeField]
        private LiteTweenerUpdateMode updateMode;
        [SerializeField]
        private LiteTweenerAnimateMode animateMode;
        [SerializeField]
        private DampCurveParam dampCurveParam = new DampCurveParam() { maxSpeed = 1000f, smoothTime = 0.3f };
        [SerializeField]
        private SmoothDampParam smoothDampParam = new SmoothDampParam() { maxSpeed = 500f, smoothTime = 0.3f };
        [SerializeField]
        private EaseParam easeParam = new EaseParam() { duration = 0.5f };
        [SerializeField]
        private LinearParam linearParam = new LinearParam() { speed = 500f };
        [SerializeField]
        private UnityEvent onAnyTargetReached = new UnityEvent();

        [SerializeField]
        private List<TweenTarget> targets;

        private LiteCoroutine coroutine;
        private Rect currentTargetRect;
        private UnityEvent currentTargetReachedCallback;
        private Rect currentSpeed;

        private RectDampCurve dampCurve = new RectDampCurve();
        private float tweenEndTime;

        private RectTransform _rectTransform;
        private RectTransform rectTransform { get { return _rectTransform == null ? (_rectTransform = (RectTransform)transform) : _rectTransform; } }

        public event UnityAction OnAnyTargetReached { add { onAnyTargetReached.AddListener(value); } remove { onAnyTargetReached.RemoveListener(value); } }

        private static Vector3[] rectCorners = new Vector3[4];
        public static Rect GetTargetRect(RectTransform origin, RectTransform target)
        {
            target.GetWorldCorners(rectCorners);
            var min = origin.InverseTransformPoint(rectCorners[0]);
            var max = origin.InverseTransformPoint(rectCorners[2]);
            var targetRect = Rect.MinMaxRect(min.x, min.y, max.x, max.y);

            // try set size
            var currentRect = origin.rect;
            var currentLocalPos = origin.localPosition;
            origin.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetRect.width);
            origin.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetRect.height);

            // record position after size changed
            var posAfterSizeChanged = origin.rect.position;

            // back to original size
            origin.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentRect.width);
            origin.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentRect.height);
            origin.localPosition = currentLocalPos;

            return new Rect(origin.localPosition + (Vector3)(targetRect.position - posAfterSizeChanged), targetRect.size);
        }

        public static void SetTargetRect(RectTransform target, Rect dstRect)
        {
            target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dstRect.width);
            target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dstRect.height);
            target.localPosition = dstRect.position;
        }

        public static void SetTargetRect(RectTransform target, RectTransform dst)
        {
            var dstRect = GetTargetRect(target, dst);
            SetTargetRect(target, dstRect);
        }

        public void TweenToTransform(RectTransform t) { TweenToTarget(new TweenTarget() { rectObj = t }); }

        public void SnapToTransform(RectTransform t) { SnapToTarget(new TweenTarget() { rectObj = t }); }

        public void TweenToRect(Rect t) { TweenToTarget(new TweenTarget() { rect = t }); }

        public void SnapToRect(Rect t) { SnapToTarget(new TweenTarget() { rect = t }); }

        public void TweenToIndex(int index) { TweenToTarget(targets[index]); }

        public void SnapToIndex(int index) { SnapToTarget(targets[index]); }

        public void TweenToTarget(TweenTarget target)
        {
            currentTargetRect = target.ToTarget(rectTransform);
            currentTargetReachedCallback = target.onTargetReached;

            switch (animateMode)
            {
                case LiteTweenerAnimateMode.DampCurve:
                    var currentRectPos = transform.localPosition;
                    var currentRectSize = rectTransform.rect.size;
                    if (GeoApproxEqual(currentRectPos, currentTargetRect.position)) { currentRectPos = currentTargetRect.position; }
                    if (ArithApproxEqual(currentRectSize.x, currentTargetRect.size.x)) { currentRectSize.x = currentTargetRect.size.x; }
                    if (ArithApproxEqual(currentRectSize.y, currentTargetRect.size.y)) { currentRectSize.y = currentTargetRect.size.y; }
                    dampCurve.StartValue = new Rect(currentRectPos, currentRectSize);
                    dampCurve.EndValue = currentTargetRect;
                    dampCurve.StartTime = Time.time;
                    dampCurve.StartSpeed = currentSpeed;
                    dampCurve.MaxSmoothTime = dampCurveParam.smoothTime;
                    dampCurve.SetMaxSpeed(dampCurveParam.maxSpeed);
                    break;
                case LiteTweenerAnimateMode.SmoothDamp:
                    break;
                case LiteTweenerAnimateMode.Ease:
                    tweenEndTime = Time.time + easeParam.duration;
                    break;
                case LiteTweenerAnimateMode.Linear:
                    var currentRect = new Rect(transform.localPosition, rectTransform.rect.size);
                    var posDiff = currentTargetRect.position - currentRect.position;
                    var sizeDiff = currentTargetRect.size - currentRect.size;
                    var maxDuration = Mathf.Max(posDiff.magnitude / linearParam.speed, sizeDiff.x / linearParam.speed, sizeDiff.y / linearParam.speed);
                    currentSpeed = new Rect(posDiff / maxDuration, sizeDiff / maxDuration);
                    tweenEndTime = Time.time + maxDuration;
                    break;
                case LiteTweenerAnimateMode.Snap:
                    break;
            }

            if (coroutine.IsNullOrDone()) { LiteCoroutine.StartCoroutine(ref coroutine, GotoCoroutine()); }
        }

        public void SnapToTarget(TweenTarget target)
        {
            if (!coroutine.IsNullOrDone()) { coroutine.Stop(); }

            if (target.rect != null)
            {
                SetTargetRect(rectTransform, target.rectObj);
            }
            else
            {
                SetTargetRect(rectTransform, target.rect);
            }

            currentTargetReachedCallback = null;

            ResetCurrentSpeed();
        }

        private void ResetCurrentSpeed()
        {
            currentSpeed = Rect.zero;
        }

        private IEnumerator GotoCoroutine()
        {
            while (true)
            {
                switch (updateMode)
                {
                    case LiteTweenerUpdateMode.LateUpdate: yield return new WaitForLateUpdate(); break;
                    case LiteTweenerUpdateMode.FixedUpdate: yield return new WaitForFixedUpdate(); break;
                    case LiteTweenerUpdateMode.EndOfFrameUpdate: yield return new WaitForEndOfFrame(); break;
                    default: yield return null; break;
                }

                var reachTarget = true;
                Rect currentRect;
                var now = Time.time;

                switch (animateMode)
                {
                    case LiteTweenerAnimateMode.DampCurve:
                        if (now < dampCurve.EndTime)
                        {
                            SetTargetRect(rectTransform, dampCurve.Evaluate(now, out currentSpeed));
                            reachTarget = false;
                        }
                        break;
                    case LiteTweenerAnimateMode.SmoothDamp:
                        currentRect = new Rect(transform.localPosition, rectTransform.rect.size);
                        if (!RectApproximatelyEqual(currentRect, currentTargetRect) || currentSpeed.position.sqrMagnitude > 0.01f || currentSpeed.size.sqrMagnitude > 0.1f)
                        {
                            var delta = Time.deltaTime;

                            var posCurrent = transform.localPosition;
                            var posTarget = currentTargetRect.position;
                            var posSpeed = (Vector3)currentSpeed.position;
                            var newPos = Vector3.SmoothDamp(posCurrent, posTarget, ref posSpeed, smoothDampParam.smoothTime, smoothDampParam.maxSpeed, delta);
                            currentSpeed.position = posSpeed;

                            var sizeCurrent = rectTransform.rect.size;
                            var sizeTarget = currentTargetRect.size;
                            var sizeSpeed = (Vector3)currentSpeed.size;
                            var newSize = Vector3.SmoothDamp(sizeCurrent, sizeTarget, ref sizeSpeed, smoothDampParam.smoothTime, smoothDampParam.maxSpeed, delta);
                            currentSpeed.size = sizeSpeed;

                            SetTargetRect(rectTransform, new Rect(newPos, newSize));
                            reachTarget = false;
                        }
                        break;
                    case LiteTweenerAnimateMode.Ease:
                        if (now < tweenEndTime)
                        {
                            currentRect = new Rect(transform.localPosition, rectTransform.rect.size);
                            var deltaTime = Time.deltaTime;
                            if (easeParam.duration < Mathf.Epsilon || easeParam.duration <= deltaTime)
                            {
                                currentSpeed = new Rect((currentTargetRect.position - currentRect.position) / deltaTime, (currentTargetRect.size - currentRect.size) / deltaTime);
                            }
                            else
                            {
                                var easeTime = 1f - Mathf.Pow(0.001f, deltaTime / easeParam.duration);
                                var newRect = new Rect(Vector2.Lerp(currentRect.position, currentTargetRect.position, easeTime), Vector2.Lerp(currentRect.size, currentTargetRect.size, easeTime));
                                currentSpeed = new Rect((newRect.position - currentRect.position) / deltaTime, (newRect.size - currentRect.size) / deltaTime);
                                SetTargetRect(rectTransform, newRect);
                                reachTarget = false;
                            }
                        }
                        break;
                    case LiteTweenerAnimateMode.Linear:
                        if (now < tweenEndTime)
                        {
                            currentRect = new Rect(transform.localPosition, rectTransform.rect.size);
                            var delta = Time.deltaTime;
                            var newPos = currentRect.position + currentSpeed.position * delta;
                            var newSize = currentRect.size + currentSpeed.size * delta;
                            SetTargetRect(rectTransform, new Rect(newPos, newSize));
                            reachTarget = false;
                        }
                        break;
                }

                if (reachTarget) { break; }
            }

            ResetCurrentSpeed();

            SetTargetRect(rectTransform, currentTargetRect);

            if (currentTargetReachedCallback != null)
            {
                currentTargetReachedCallback.Invoke();
                currentTargetReachedCallback = null;
            }

            if (onAnyTargetReached != null)
            {
                onAnyTargetReached.Invoke();
            }
        }

        private static bool RectApproximatelyEqual(Rect v1, Rect v2)
        {
            return GeoApproxEqual(v1.position, v2.position) && ArithApproxEqual(v1.size, v2.size);
        }

        private static bool GeoApproxEqual(Vector2 v1, Vector2 v2)
        {
            return (v1 - v2).sqrMagnitude < 0.01f;
        }

        private static bool ArithApproxEqual(Vector2 v1, Vector2 v2)
        {
            return ArithApproxEqual(v1.x, v2.x) && ArithApproxEqual(v1.y, v2.y);
        }

        private static bool ArithApproxEqual(float v1, float v2)
        {
            return Mathf.Abs(v1 - v2) < 0.1f;
        }
    }
}