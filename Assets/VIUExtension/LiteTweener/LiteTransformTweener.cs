#pragma warning disable 0649
using HTC.UnityPlugin.LiteCoroutineSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.Utility.LiteTweener
{
    public class LiteTransformTweener : MonoBehaviour
    {
        [Serializable]
        public struct TweenTarget
        {
            public Transform transformObj;
            public TransformStruct transformStruct;
            public TransformStructMask mask;
            public bool useLocal;
            public UnityEvent onTargetReached;

            public TransformStruct ToLocalTarget(Transform transform)
            {
                var target = TransformStruct.FromTransformLocal(transform);

                if (transformObj != null)
                {
                    if (useLocal)
                    {
                        target.CopyFromTransformLocal(transformObj, mask);
                    }
                    else
                    {
                        if (transform.parent == null)
                        {
                            target.CopyFromTransformWorld(transformObj, mask);
                        }
                        else
                        {
                            target.CopyFromTransformRelative(transform.parent, transformObj, mask);
                        }
                    }
                }
                else
                {
                    if (transform.parent == null || useLocal)
                    {
                        target.CopyFromTransformStruct(transformStruct, mask);
                    }
                    else
                    {
                        var worldTrans = TransformStruct.FromTransformWorld(transform);
                        worldTrans.CopyFromTransformStruct(transformStruct, mask);
                        // transform to local space
                        if (mask.UseAnyPos) { worldTrans.pos = transform.parent.InverseTransformPoint(worldTrans.pos); }
                        if (mask.UseAnyRot) { worldTrans.rot = (Quaternion.Euler(worldTrans.rot) * Quaternion.Inverse(transform.parent.rotation)).eulerAngles; }

                        target.CopyFromTransformStruct(worldTrans, mask);
                    }
                }

                return target;
            }
        }

        [Serializable]
        public struct DampCurveParam
        {
            public float maxMoveSpeed;
            public float maxRotateSpeed;
            public float maxScaleSpeed;
            public float smoothTime;
        }

        [Serializable]
        public struct SmoothDampParam
        {
            public float maxMoveSpeed;
            public float maxRotateSpeed;
            public float maxScaleSpeed;
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
            public float moveSpeed;
            public float rotateSpeed;
            public float scaleSpeed;
        }

        [SerializeField]
        private LiteTweenerUpdateMode updateMode;
        [SerializeField]
        private LiteTweenerAnimateMode animateMode;
        [SerializeField]
        private DampCurveParam dampCurveParam = new DampCurveParam() { maxMoveSpeed = 1000f, maxRotateSpeed = 1440f, maxScaleSpeed = 50f, smoothTime = 0.3f };
        [SerializeField]
        private SmoothDampParam smoothDampParam = new SmoothDampParam() { maxMoveSpeed = 1000f, maxRotateSpeed = 1440f, maxScaleSpeed = 50f, smoothTime = 0.3f };
        [SerializeField]
        private EaseParam easeParam = new EaseParam() { duration = 0.5f };
        [SerializeField]
        private LinearParam linearParam = new LinearParam() { moveSpeed = 5f, rotateSpeed = 180f, scaleSpeed = 1f };
        [SerializeField]
        private UnityEvent onAnyTargetReached;

        [SerializeField]
        private List<TweenTarget> targets;

        private LiteCoroutine coroutine;
        private TransformStruct currentLocalTarget;
        private TransformStructMask currentMask;
        private UnityEvent currentTargetReachedCallback;
        private TransformStruct currentSpeed;

        private TransformDampCurve dampCurve = new TransformDampCurve();
        private Quaternion currentTargetRot;
        private float currentAngleSpeed;
        private float tweenEndTime;

        public LiteTweenerAnimateMode Mode { get { return animateMode; } set { animateMode = value; } }
        public UnityEvent OnAnyTargetReached { get { return onAnyTargetReached; } }
        public List<TweenTarget> Targets { get { return targets; } }

        public void TweenToTransformLocal(Transform t) { TweenToTarget(new TweenTarget() { transformObj = t, mask = TransformStructMask.All, useLocal = true }); }

        public void TweenToTransformWorld(Transform t) { TweenToTarget(new TweenTarget() { transformObj = t, mask = TransformStructMask.All, useLocal = false }); }

        public void SnapToTransform(Transform t) { SnapToTarget(new TweenTarget() { transformObj = t, mask = TransformStructMask.All }); }

        public void TweenToIndex(int index) { TweenToTarget(targets[index]); }

        public void SnapToIndex(int index) { SnapToTarget(targets[index]); }

        public void TweenToTarget(TweenTarget target)
        {
            if (!target.mask.UseAny) { return; }

            currentLocalTarget = target.ToLocalTarget(transform);
            currentMask = target.mask;
            currentTargetReachedCallback = target.onTargetReached;

            switch (animateMode)
            {
                case LiteTweenerAnimateMode.DampCurve:
                    dampCurve.StartValue = TransformStruct.FromTransformLocal(transform);
                    dampCurve.EndValue = currentLocalTarget;
                    dampCurve.StartTime = Time.time;
                    dampCurve.StartSpeed = currentSpeed;
                    dampCurve.MaxSmoothTime = dampCurveParam.smoothTime;
                    dampCurve.SetMaxSpeed(dampCurveParam.maxMoveSpeed, dampCurveParam.maxRotateSpeed, dampCurveParam.maxScaleSpeed);
                    break;
                case LiteTweenerAnimateMode.SmoothDamp:
                    break;
                case LiteTweenerAnimateMode.Ease:
                    currentTargetRot = Quaternion.Euler(currentLocalTarget.rot);
                    tweenEndTime = Time.time + easeParam.duration;
                    break;
                case LiteTweenerAnimateMode.Linear:
                    currentTargetRot = Quaternion.Euler(currentLocalTarget.rot);
                    var posDiff = currentLocalTarget.pos - transform.localPosition;
                    var moveDuration = posDiff.magnitude / linearParam.moveSpeed;
                    var angleDiff = Quaternion.Angle(currentTargetRot, transform.localRotation);
                    var rotateDuration = angleDiff / linearParam.rotateSpeed;
                    var scaleDiff = currentLocalTarget.scale - transform.localScale;
                    var scaleDuration = scaleDiff.magnitude / linearParam.scaleSpeed;
                    var maxDuration = Mathf.Max(moveDuration, rotateDuration, scaleDuration);
                    currentSpeed.pos = posDiff / maxDuration;
                    currentSpeed.rot = (currentLocalTarget.rot - transform.localEulerAngles) / maxDuration;
                    currentAngleSpeed = angleDiff / maxDuration;
                    currentSpeed.scale = scaleDiff / maxDuration;
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

            target.ToLocalTarget(transform).ApplyTransformLocal(transform, target.mask);
            currentTargetReachedCallback = null;

            ResetCurrentSpeed();
        }

        private void ResetCurrentSpeed()
        {
            currentSpeed = new TransformStruct();
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
                var now = Time.time;

                switch (animateMode)
                {
                    case LiteTweenerAnimateMode.DampCurve:
                        if (now < dampCurve.EndTime)
                        {
                            dampCurve.Evaluate(now, out currentSpeed).ApplyTransformLocal(transform, currentMask);
                            reachTarget = false;
                        }
                        break;
                    case LiteTweenerAnimateMode.SmoothDamp:
                        if (!ReachTarget() || currentSpeed.pos != Vector3.zero || currentAngleSpeed > 0.1f || currentSpeed.scale != Vector3.zero)
                        {
                            var delta = Time.deltaTime;

                            if (currentMask.UseAnyPos) { transform.localPosition = Vector3.SmoothDamp(transform.localPosition, currentLocalTarget.pos, ref currentSpeed.pos, smoothDampParam.smoothTime, smoothDampParam.maxMoveSpeed, delta); }

                            if (currentMask.UseAnyRot)
                            {
                                var euler = transform.localEulerAngles;
                                if (currentMask.UseRotX) { euler.x = Mathf.SmoothDampAngle(euler.x, currentLocalTarget.rot.x, ref currentSpeed.rot.x, smoothDampParam.smoothTime, smoothDampParam.maxRotateSpeed, delta); }
                                if (currentMask.UseRotY) { euler.y = Mathf.SmoothDampAngle(euler.y, currentLocalTarget.rot.y, ref currentSpeed.rot.y, smoothDampParam.smoothTime, smoothDampParam.maxRotateSpeed, delta); }
                                if (currentMask.UseRotZ) { euler.z = Mathf.SmoothDampAngle(euler.z, currentLocalTarget.rot.z, ref currentSpeed.rot.z, smoothDampParam.smoothTime, smoothDampParam.maxRotateSpeed, delta); }
                                transform.localEulerAngles = euler;
                            }

                            if (currentMask.UseAnyPos) { transform.localScale = Vector3.SmoothDamp(transform.localScale, currentLocalTarget.scale, ref currentSpeed.scale, smoothDampParam.smoothTime, smoothDampParam.maxScaleSpeed, delta); }

                            reachTarget = false;
                        }
                        break;
                    case LiteTweenerAnimateMode.Ease:
                        if (now < tweenEndTime)
                        {
                            var deltaTime = Time.deltaTime;
                            if (easeParam.duration < Mathf.Epsilon || easeParam.duration <= deltaTime)
                            {
                                if (currentMask.UseAnyPos) { currentSpeed.pos = (currentLocalTarget.pos - transform.localPosition) / deltaTime; }
                                if (currentMask.UseAnyRot) { currentSpeed.rot = (currentLocalTarget.rot - transform.localEulerAngles) / deltaTime; }
                                if (currentMask.UseAnyScale) { currentSpeed.scale = (currentLocalTarget.scale - transform.localScale) / deltaTime; }
                            }
                            else
                            {
                                var easeTime = 1f - Mathf.Pow(0.001f, deltaTime / easeParam.duration);
                                if (currentMask.UseAnyPos) { transform.localPosition = Vector3.Lerp(transform.localPosition, currentLocalTarget.pos, easeTime); }
                                if (currentMask.UseAnyRot) { transform.localRotation = Quaternion.Slerp(transform.localRotation, currentTargetRot, easeTime); }
                                if (currentMask.UseAnyScale) { transform.localScale = Vector3.Lerp(transform.localScale, currentLocalTarget.scale, easeTime); }
                                reachTarget = false;
                            }
                        }
                        break;
                    case LiteTweenerAnimateMode.Linear:
                        if (now < tweenEndTime)
                        {
                            var delta = Time.deltaTime;
                            if (currentMask.UseAnyPos) { transform.localPosition = transform.localPosition + currentSpeed.pos * delta; }
                            if (currentMask.UseAnyRot) { transform.localRotation = Quaternion.RotateTowards(transform.localRotation, currentTargetRot, currentAngleSpeed * delta); }
                            if (currentMask.UseAnyScale) { transform.localScale = transform.localScale + currentSpeed.scale * delta; }
                            reachTarget = false;
                        }
                        break;
                }

                if (reachTarget) { break; }
            }

            ResetCurrentSpeed();

            currentLocalTarget.ApplyTransformLocal(transform, currentMask);

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

        private bool ReachTarget()
        {
            if (transform.localPosition != currentLocalTarget.pos) { return false; }
            if (Quaternion.Angle(transform.localRotation, Quaternion.Euler(currentLocalTarget.rot)) > 0.1f) { return false; }
            if (transform.localScale != currentLocalTarget.scale) { return false; }
            return true;
        }
    }
}