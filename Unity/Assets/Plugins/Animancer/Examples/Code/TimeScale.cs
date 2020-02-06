// Animancer // Copyright 2020 Kybernetik //

using UnityEngine;

namespace Animancer.Examples
{
    /// <summary>
    /// A simple Inspector slider to control <see cref="Time.timeScale"/>.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Time Scale")]
    public sealed class TimeScale : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField, Range(0, 1)]
        private float _Value = 0.5f;

        public float Value
        {
            get { return _Value; }
            set
            {
                _Value = value;

#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                    return;
#endif

                Time.timeScale = _Value;
            }
        }

        /************************************************************************************************************************/

        private void Awake()
        {
            Value = _Value;
        }

        /************************************************************************************************************************/

        private void OnValidate()
        {
            Value = _Value;
        }

        /************************************************************************************************************************/
    }
}
