// Animancer // Copyright 2020 Kybernetik //

using System;
using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> which extracts a particular <see cref="AnimationCurve"/> from an
    /// <see cref="AnimationClip"/> in the Unity Editor so that it can be accessed at runtime.
    /// </summary>
    [CreateAssetMenu(menuName = Strings.MenuPrefix + "Exposed Curve", order = Strings.AssetMenuOrder + 9)]
    public class ExposedCurve : ScriptableObject
    {
        /************************************************************************************************************************/

        [SerializeField, Tooltip("The animation to extract the curve from")]
        private AnimationClip _Clip;

        /// <summary>[<see cref="SerializeField"/>] The animation to extract the curve from.</summary>
        public AnimationClip Clip
        {
            get { return _Clip; }
            set
            {
                if (value != null)
                    Validate.NotLegacy(value);

                _Clip = value;
            }
        }

        /// <summary>Returns the <see cref="Clip"/>.</summary>
        /// <exception cref="NullReferenceException">Thrown if the `curve` is null.</exception>
        public static implicit operator AnimationClip(ExposedCurve curve)
        {
            return curve.Clip;
        }

        /************************************************************************************************************************/

#if UNITY_EDITOR
        [SerializeField, Tooltip("[Editor-Only] The name of the curve to extract")]
        private string _PropertyName;

        /// <summary>[Editor-Only] [<see cref="SerializeField"/>] The name of the curve to extract.</summary>
        public string PropertyName
        {
            get { return _PropertyName; }
            set
            {
                _PropertyName = value;
            }
        }
#endif

        /************************************************************************************************************************/

        [SerializeField]
        private AnimationCurve _Curve;

        /// <summary>[<see cref="SerializeField"/>]
        /// The <see cref="AnimationCurve"/> that has been extracted from the <see cref="Clip"/>.
        /// </summary>
        public AnimationCurve Curve
        {
            get { return _Curve; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _Curve = value;
            }
        }

        /// <summary>
        /// Returns the <see cref="Curve"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">Thrown if the `curve` is null.</exception>
        public static implicit operator AnimationCurve(ExposedCurve curve)
        {
            return curve.Curve;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Returns the value of the <see cref="Curve"/> at the current <see cref="AnimancerState.Time"/> of the state
        /// registered with the <see cref="Clip"/> as its key.
        /// </summary>
        public float Evaluate(AnimancerComponent animancer)
        {
            return Evaluate(animancer.States[_Clip]);
        }

        /// <summary>
        /// Returns the value of the <see cref="Curve"/> at the current <see cref="AnimancerState.Time"/>.
        /// </summary>
        public float Evaluate(AnimancerState state)
        {
            if (state == null)
                return 0;

            return Evaluate(state.Time % state.Length);
        }

        /// <summary>
        /// Returns the value of the <see cref="Curve"/> at the specified `time`.
        /// </summary>
        public float Evaluate(float time)
        {
            return _Curve.Evaluate(time);
        }

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        private void OnEnable()
        {
            UnityEditor.EditorCurveBinding binding;
            if (TryGetCurveBinding(out binding))
            {
                _Curve = UnityEditor.AnimationUtility.GetEditorCurve(_Clip, binding);
            }
            else
            {
                _Curve = null;
            }
        }

        /************************************************************************************************************************/

        private void OnValidate()
        {
            OnEnable();
        }

        /************************************************************************************************************************/

        private bool TryGetCurveBinding(out UnityEditor.EditorCurveBinding binding)
        {
            if (_Clip != null &&
               !string.IsNullOrEmpty(_PropertyName))
            {
                var bindings = UnityEditor.AnimationUtility.GetCurveBindings(_Clip);
                for (int i = 0; i < bindings.Length; i++)
                {
                    binding = bindings[i];
                    if (binding.propertyName == _PropertyName)
                        return true;
                }
            }

            binding = new UnityEditor.EditorCurveBinding();
            return false;
        }

        /************************************************************************************************************************/

        [UnityEditor.CustomEditor(typeof(ExposedCurve), true)]
        private class Editor : Animancer.Editor.ScriptableObjectEditor { }

        /************************************************************************************************************************/
#endif
        /************************************************************************************************************************/
    }
}
