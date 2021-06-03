using System;
using UnityEngine;

namespace HTC.UnityPlugin.Utility.LiteTweener
{
    [Serializable]
    public struct TransformStruct
    {
        public Vector3 pos;
        public Vector3 rot;
        public Vector3 scale;

        public TransformStruct(Vector3 pos, Vector3 rot, Vector3 scale)
        {
            this.pos = pos;
            this.rot = rot;
            this.scale = scale;
        }

        public void CopyFromTransformLocal(Transform t, TransformStructMask mask)
        {
            if (mask.UseAnyPos) { ApplyPos(t.localPosition, ref mask); }
            if (mask.UseAnyRot) { ApplyRot(t.localEulerAngles, ref mask); }
            if (mask.UseAnyScale) { ApplyScale(t.localScale, ref mask); }
        }

        public void CopyFromTransformWorld(Transform t, TransformStructMask mask)
        {
            if (mask.UseAnyPos) { ApplyPos(t.position, ref mask); }
            if (mask.UseAnyRot) { ApplyRot(t.eulerAngles, ref mask); }
            if (mask.UseAnyScale) { ApplyScale(t.localScale, ref mask); }
        }

        public void CopyFromTransformRelative(Transform origin, Transform t, TransformStructMask mask)
        {
            if (mask.UseAnyPos) { ApplyPos(origin.InverseTransformPoint(t.position), ref mask); }
            if (mask.UseAnyRot) { ApplyRot((t.rotation * Quaternion.Inverse(origin.rotation)).eulerAngles, ref mask); }
            if (mask.UseAnyScale) { ApplyScale(t.localScale, ref mask); }
        }

        public void CopyFromTransformStruct(TransformStruct t, TransformStructMask mask)
        {
            if (mask.UseAnyPos) { ApplyPos(t.pos, ref mask); }
            if (mask.UseAnyRot) { ApplyRot(t.rot, ref mask); }
            if (mask.UseAnyScale) { ApplyScale(t.scale, ref mask); }
        }

        public void CopyFromTransformStructRelative(Transform origin, TransformStruct t, TransformStructMask mask)
        {
            if (mask.UseAnyPos) { ApplyPos(origin.InverseTransformPoint(t.pos), ref mask); }
            if (mask.UseAnyRot) { ApplyRot(Quaternion.Euler(t.rot) * Quaternion.Inverse(origin.rotation).eulerAngles, ref mask); }
            if (mask.UseAnyScale) { ApplyScale(t.scale, ref mask); }
        }

        public static TransformStruct FromTransformLocal(Transform t) { return new TransformStruct(t.localPosition, t.localEulerAngles, t.localScale); }

        public static TransformStruct FromTransformWorld(Transform t) { return new TransformStruct(t.position, t.eulerAngles, t.lossyScale); }

        public void ApplyTransformLocal(Transform t, TransformStructMask mask)
        {
            if (mask.UseAllPos) { t.localPosition = pos; }
            else if (mask.UseAnyPos) { var originPos = t.localPosition; t.localPosition = new Vector3(mask.UsePosX ? pos.x : originPos.x, mask.UsePosY ? pos.y : originPos.y, mask.UsePosZ ? pos.z : originPos.z); }

            if (mask.UseAllRot) { t.localEulerAngles = rot; }
            else if (mask.UseAnyRot) { var originRot = t.localEulerAngles; t.localEulerAngles = new Vector3(mask.UseRotX ? rot.x : originRot.x, mask.UseRotY ? rot.y : originRot.y, mask.UseRotZ ? rot.z : originRot.z); }

            if (mask.UseAllScale) { t.localScale = scale; }
            else if (mask.UseAnyScale) { var originScale = t.localScale; t.localScale = new Vector3(mask.UseScaleX ? scale.x : originScale.x, mask.UseRotY ? scale.y : originScale.y, mask.UseRotZ ? scale.z : originScale.z); }
        }

        public void ApplyTransformWorld(Transform t, TransformStructMask mask)
        {
            if (mask.UseAllPos) { t.position = pos; }
            else if (mask.UseAnyPos) { var originPos = t.position; t.position = new Vector3(mask.UsePosX ? pos.x : originPos.x, mask.UsePosY ? pos.y : originPos.y, mask.UsePosZ ? pos.z : originPos.z); }

            if (mask.UseAllRot) { t.eulerAngles = rot; }
            else if (mask.UseAnyRot) { var originRot = t.eulerAngles; t.eulerAngles = new Vector3(mask.UseRotX ? rot.x : originRot.x, mask.UseRotY ? rot.y : originRot.y, mask.UseRotZ ? rot.z : originRot.z); }

            if (mask.UseAllScale) { t.localScale = scale; }
            else if (mask.UseAnyScale) { var originScale = t.localScale; t.localScale = new Vector3(mask.UseScaleX ? scale.x : originScale.x, mask.UseRotY ? scale.y : originScale.y, mask.UseRotZ ? scale.z : originScale.z); }
        }

        private void ApplyPos(Vector3 value, ref TransformStructMask mask)
        {
            if (mask.UseAllPos) { pos = value; return; }
            if (mask.UsePosX) { pos.x = value.x; }
            if (mask.UsePosY) { pos.y = value.y; }
            if (mask.UsePosZ) { pos.z = value.z; }
        }

        private void ApplyRot(Vector3 value, ref TransformStructMask mask)
        {
            if (mask.UseAllRot) { rot = value; return; }
            if (mask.UseRotX) { rot.x = value.x; }
            if (mask.UseRotY) { rot.y = value.y; }
            if (mask.UseRotZ) { rot.z = value.z; }
        }

        private void ApplyScale(Vector3 value, ref TransformStructMask mask)
        {
            if (mask.UseAllScale) { scale = value; return; }
            if (mask.UseScaleX) { scale.x = value.x; }
            if (mask.UseScaleY) { scale.y = value.y; }
            if (mask.UseScaleZ) { scale.z = value.z; }
        }
    }

    [Flags]
    public enum TransformStructMaskEnum
    {
        PosX,
        PosY,
        PosZ,
        RotX,
        RotY,
        RotZ,
        ScaleX,
        ScaleY,
        ScaleZ,
    }

    [Serializable]
    public struct TransformStructMask
    {
        [FlagsFromEnum(typeof(TransformStructMaskEnum))]
        private uint mask;

        public TransformStructMask(params TransformStructMaskEnum[] fields)
        {
            mask = 0u;
            foreach (var f in fields) { this[f] = true; }
        }

        public bool this[TransformStructMaskEnum i] { get { return EnumUtils.GetFlag(mask, (int)i); } set { EnumUtils.SetFlag(ref mask, (int)i, value); } }

        public bool UsePosX { get { return (mask & posX) > 0; } set { if (value) { mask |= posX; } else { mask &= ~posX; } } }
        public bool UsePosY { get { return (mask & posY) > 0; } set { if (value) { mask |= posY; } else { mask &= ~posY; } } }
        public bool UsePosZ { get { return (mask & posZ) > 0; } set { if (value) { mask |= posZ; } else { mask &= ~posZ; } } }
        public bool UseRotX { get { return (mask & rotX) > 0; } set { if (value) { mask |= rotX; } else { mask &= ~rotX; } } }
        public bool UseRotY { get { return (mask & rotY) > 0; } set { if (value) { mask |= rotY; } else { mask &= ~rotY; } } }
        public bool UseRotZ { get { return (mask & rotZ) > 0; } set { if (value) { mask |= rotZ; } else { mask &= ~rotZ; } } }
        public bool UseScaleX { get { return (mask & scaleX) > 0; } set { if (value) { mask |= scaleX; } else { mask &= ~scaleX; } } }
        public bool UseScaleY { get { return (mask & scaleY) > 0; } set { if (value) { mask |= scaleY; } else { mask &= ~scaleY; } } }
        public bool UseScaleZ { get { return (mask & scaleZ) > 0; } set { if (value) { mask |= scaleZ; } else { mask &= ~scaleZ; } } }
        public bool UseAnyPos { get { return (mask & allPos) > 0; } }
        public bool UseAllPos { get { return (mask & allPos) == allPos; } }
        public bool UseAnyRot { get { return (mask & allRot) > 0; } }
        public bool UseAllRot { get { return (mask & allRot) == allRot; } }
        public bool UseAnyScale { get { return (mask & allScale) > 0; } }
        public bool UseAllScale { get { return (mask & allScale) == allScale; } }
        public bool UseAny { get { return (mask & all) > 0; } }
        public bool UseAll { get { return (mask & all) == all; } }

        public static TransformStructMask PosX = new TransformStructMask() { mask = posX };
        public static TransformStructMask PosY = new TransformStructMask() { mask = posY };
        public static TransformStructMask PosZ = new TransformStructMask() { mask = posZ };
        public static TransformStructMask RotX = new TransformStructMask() { mask = rotX };
        public static TransformStructMask RotY = new TransformStructMask() { mask = rotY };
        public static TransformStructMask RotZ = new TransformStructMask() { mask = rotZ };
        public static TransformStructMask ScaleX = new TransformStructMask() { mask = scaleX };
        public static TransformStructMask ScaleY = new TransformStructMask() { mask = scaleY };
        public static TransformStructMask ScaleZ = new TransformStructMask() { mask = scaleZ };
        public static TransformStructMask AllPos = new TransformStructMask() { mask = allPos };
        public static TransformStructMask AllRot = new TransformStructMask() { mask = allRot };
        public static TransformStructMask AllScale = new TransformStructMask() { mask = allScale };
        public static TransformStructMask All = new TransformStructMask() { mask = all };

        private const uint posX = 1 << (int)TransformStructMaskEnum.PosX;
        private const uint posY = 1 << (int)TransformStructMaskEnum.PosY;
        private const uint posZ = 1 << (int)TransformStructMaskEnum.PosZ;
        private const uint rotX = 1 << (int)TransformStructMaskEnum.RotX;
        private const uint rotY = 1 << (int)TransformStructMaskEnum.RotY;
        private const uint rotZ = 1 << (int)TransformStructMaskEnum.RotZ;
        private const uint scaleX = 1 << (int)TransformStructMaskEnum.ScaleX;
        private const uint scaleY = 1 << (int)TransformStructMaskEnum.ScaleY;
        private const uint scaleZ = 1 << (int)TransformStructMaskEnum.ScaleZ;
        private const uint allPos = posX | posY | posZ;
        private const uint allRot = rotX | rotY | rotZ;
        private const uint allScale = scaleX | scaleY | scaleZ;
        private const uint all = allPos | allRot | allScale;

        public static TransformStructMask operator &(TransformStructMask a, TransformStructMask b) { return new TransformStructMask() { mask = a.mask & b.mask }; }
        public static TransformStructMask operator |(TransformStructMask a, TransformStructMask b) { return new TransformStructMask() { mask = a.mask | b.mask }; }
        public static TransformStructMask operator ^(TransformStructMask a, TransformStructMask b) { return new TransformStructMask() { mask = a.mask ^ b.mask }; }
    }

    public class TransformDampCurve
    {
        private readonly Vector3DampCurve cPos = new Vector3DampCurve() { NormalizeSpeed = true };
        private readonly EulerAnglesDampCurve cRot = new EulerAnglesDampCurve();
        private readonly Vector3DampCurve cScale = new Vector3DampCurve() { NormalizeSpeed = true };
        private bool dirty = true;
        private bool normalizeSpeed = true;

        public TransformStruct StartValue
        {
            get { return new TransformStruct(cPos.StartValue, cRot.StartValue, cScale.StartValue); }
            set { cPos.StartValue = value.pos; cRot.StartValue = value.rot; cScale.StartValue = value.scale; SetDirty(); }
        }

        public TransformStruct EndValue
        {
            get { return new TransformStruct(cPos.EndValue, cRot.EndValue, cScale.EndValue); }
            set { cPos.EndValue = value.pos; cRot.EndValue = value.rot; cScale.EndValue = value.scale; SetDirty(); }
        }

        public TransformStruct StartSpeed
        {
            get { return new TransformStruct(cPos.StartSpeed, cRot.StartSpeed, cScale.StartSpeed); }
            set { cPos.StartSpeed = value.pos; cRot.StartSpeed = value.rot; cScale.StartSpeed = value.scale; SetDirty(); }
        }

        public float StartTime
        {
            get { return cPos.StartTime; }
            set { cPos.StartTime = value; cRot.StartTime = value; cScale.StartTime = value; }
        }

        public float EndTime
        {
            get { UpdateDirtyState(); return cPos.EndTime; }
        }

        public float MaxSmoothTime
        {
            get { return cPos.MaxSmoothTime; }
            set { cPos.MaxSmoothTime = value; cRot.MaxSmoothTime = value; cScale.MaxSmoothTime = value; SetDirty(); }
        }

        public bool NormalizeSpeed { get { return normalizeSpeed; } set { if (normalizeSpeed != value) { normalizeSpeed = value; SetDirty(); } } }

        public TransformStruct MaxSpeed
        {
            get { UpdateDirtyState(); return new TransformStruct(cPos.MaxSpeed, cRot.MaxSpeed, cScale.MaxSpeed); }
        }

        public float Duration
        {
            get { UpdateDirtyState(); return cPos.Duration; }
            set { cPos.Duration = value; cRot.Duration = value; cScale.Duration = value; ResetDirty(); }
        }

        public void SetMaxSpeed(float posSpeed, float angleSpeed, float scaleSpeed) { cPos.SetMaxSpeed(posSpeed); cRot.SetMaxSpeed(angleSpeed); cScale.SetMaxSpeed(scaleSpeed); SetDirty(); }

        private void SetDirty() { dirty = true; }
        private void ResetDirty() { dirty = false; }

        private void UpdateDirtyState()
        {
            if (dirty)
            {
                var maxDuration = Mathf.Max(cPos.Duration, cRot.Duration, cScale.Duration);
                cPos.Duration = maxDuration;
                cRot.Duration = maxDuration;
                cScale.Duration = maxDuration;

                ResetDirty();
            }
        }

        public TransformStruct Evaluate(float time)
        {
            UpdateDirtyState();
            return new TransformStruct(cPos.Evaluate(time), cRot.Evaluate(time), cScale.Evaluate(time));
        }

        public TransformStruct EvaluateSpeed(float time)
        {
            UpdateDirtyState();
            return new TransformStruct(cPos.EvaluateSpeed(time), cRot.EvaluateSpeed(time), cScale.EvaluateSpeed(time));
        }

        public TransformStruct Evaluate(float time, out TransformStruct speed)
        {
            UpdateDirtyState();
            var result = new TransformStruct(cPos.Evaluate(time, out var posSpeed), cRot.Evaluate(time, out var rotSpeed), cScale.Evaluate(time, out var scaleSpeed));
            speed = new TransformStruct(posSpeed, rotSpeed, scaleSpeed);
            return result;
        }
    }
}