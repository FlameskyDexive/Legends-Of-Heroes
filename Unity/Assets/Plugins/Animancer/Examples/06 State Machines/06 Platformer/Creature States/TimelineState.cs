// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0618 // Type or member is obsolete (for PlayableAssetState in Animancer Lite).
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using UnityEngine;

namespace Animancer.Examples.StateMachines.Platformer
{
    /// <summary>
    /// A <see cref="CreatureState"/> that plays a <see cref="UnityEngine.Playables.PlayableAsset"/> (such as a
    /// <see cref="UnityEngine.Timeline.TimelineAsset"/>)
    /// then returns to idle.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Platformer - Timeline State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.StateMachines.Platformer/TimelineState")]
    public sealed class TimelineState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private PlayableAssetState.Transition _Animation;
        [SerializeField] private bool _DestroyWhenDone;

        /************************************************************************************************************************/

        private void Awake()
        {
            if (_DestroyWhenDone)
            {
                _Animation.Events.Sequence.OnEnd = () =>
                {
                    Creature.Idle.ForceEnterState();
                    Creature.Animancer.States.Destroy(_Animation);
                    Destroy(this);
                };
            }
            else
            {
                _Animation.Events.Sequence.OnEnd = Creature.ForceEnterIdleState;
            }
        }

        /************************************************************************************************************************/

        private void OnEnable()
        {
            Creature.Animancer.Play(_Animation);
        }

        /************************************************************************************************************************/
    }
}
