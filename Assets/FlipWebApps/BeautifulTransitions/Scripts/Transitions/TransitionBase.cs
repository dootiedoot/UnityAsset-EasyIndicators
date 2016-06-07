//----------------------------------------------
// Flip Web Apps: Beautiful Transitions
// Copyright © 2016 Flip Web Apps / Mark Hewitt
//
// Please direct any bugs/comments/suggestions to http://www.flipwebapps.com
// 
// The copyright owner grants to the end user a non-exclusive, worldwide, and perpetual license to this Asset
// to integrate only as incorporated and embedded components of electronic games and interactive media and 
// distribute such electronic game and interactive media. End user may modify Assets. End user may otherwise 
// not reproduce, distribute, sublicense, rent, lease or lend the Assets. It is emphasized that the end 
// user shall not be entitled to distribute or transfer in any way (including, without, limitation by way of 
// sublicense) the Assets in any other way than as integrated components of electronic games and interactive media. 

// The above copyright notice and this permission notice must not be removed from any files.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//----------------------------------------------

using System;
using System.Collections;
using FlipWebApps.BeautifulTransitions.Scripts.Helper;
using UnityEngine;
using UnityEngine.Events;

namespace FlipWebApps.BeautifulTransitions.Scripts.Transitions
{
    public abstract class TransitionBase : MonoBehaviour {
        [Tooltip("Whether to set up ready for transitioning in.")]
        public bool InitForTransitionIn = true;
        [Tooltip("Whether to automatically run the transition in effect on start.")]
        public bool AutoRun;

        [Tooltip("Normalised progress of the transition effect.")]
        [Range(0.0f, 1.0f)]
        [HideInInspector]
        public float Amount;
        //public float TargetAmount;
        public TransitionSettings TransitionInConfig;
        public TransitionSettings TransitionOutConfig;

        public enum TransitionModeType {None, In, Out}
        public TransitionModeType TransitionMode { get; set; }

        /// <summary>
        /// Normalised progress of the current transtion.
        /// </summary>
        public float Progress { get; set; }

        // A Guid to identify the active transition. If a coroutine finds that then doesn't match the passed value then it should exit
        // as a new transition has been activated.
        string _activeTransitionId = "";

        // Values for the currently running transition.
        enum TargetTypeEnum { Float, Vector3 }
        TargetTypeEnum _targetType { get; set; }
        TransitionSettings _transitionSettings;
        float _startFloat, _endFloat;
        Vector3 _startVector3, _endVector3;
        TweenMethods.EasingFunction _easingFunction;

        public virtual void Start()
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying) return;
            #endif

            if (InitForTransitionIn || AutoRun)
            {
                InitTransitionIn();
            }

            if (AutoRun)
            {
                TransitionIn();
            }
        }


        public virtual void InitTransitionIn()
        {
            if (TransitionInConfig.TransitionType == TransitionHelper.TransitionType.none) return;
            TransitionMode = TransitionModeType.In;
            SetAmount(1);
        }


        public virtual void TransitionIn()
        {
            Transition(TransitionInConfig, Amount, 0, TransitionModeType.In);
        }


        public virtual void InitTransitionOut()
        {
            if (TransitionOutConfig.TransitionType == TransitionHelper.TransitionType.none) return;
            TransitionMode = TransitionModeType.Out;
            SetAmount(0);
        }


        public virtual void TransitionOut()
        {
            Transition(TransitionOutConfig, Amount, 1, TransitionModeType.Out);
        }


        public void Transition(TransitionSettings transitionSettings, float startAmount, float endAmount, TransitionModeType transitionMode)
        {
            // if this transition should not run then return immediately
            if (transitionSettings.TransitionType == TransitionHelper.TransitionType.none) return;

            // save transition values
            _targetType = TargetTypeEnum.Float;
            _startFloat = startAmount;
            _endFloat = endAmount;

            // start transition
            Transition(transitionSettings, transitionMode);
        }


        public void Transition(TransitionSettings transitionSettings, Vector3 startAmount, Vector3 endAmount, TransitionModeType transitionMode)
        {
            // if this transition should not run then return immediately
            if (transitionSettings.TransitionType == TransitionHelper.TransitionType.none) return;

            // save transition values
            _targetType = TargetTypeEnum.Vector3;
            _startVector3 = startAmount;
            _endVector3 = endAmount;

            // start transition
            Transition(transitionSettings, transitionMode);
        }


        void Transition(TransitionSettings transitionSettings, TransitionModeType transitionMode)
        {
            if (transitionMode == TransitionModeType.In && TransitionInConfig.OnTransitionStart != null)
                TransitionInConfig.OnTransitionStart.Invoke();
            else if (transitionMode == TransitionModeType.Out && TransitionOutConfig.OnTransitionStart != null)
                TransitionOutConfig.OnTransitionStart.Invoke();

            // set new transition id to stop all old transitions.
            _activeTransitionId = Guid.NewGuid().ToString();
            _transitionSettings = transitionSettings;
            TransitionMode = transitionMode;

            // get the easing function - will be null for unsupported types.
            _easingFunction = TweenMethods.GetEasingFunction(_transitionSettings.TransitionType);

            // if delay and duration are both zero then just set end state, otherwise run the transition.
            if (Mathf.Approximately(_transitionSettings.Delay, 0) && Mathf.Approximately(_transitionSettings.Duration, 0))
            {
                SetProgress(1);
                SetAmount(_endFloat);
            }
            else
            {
                SetProgress(0);
                StartCoroutine(TransitionInternal(_activeTransitionId, transitionMode));
            }
        }


        /// <summary>
        /// Coroutine to handle the transition over time.
        /// </summary>
        /// <param name="currentTransitionId"></param>
        /// <param name="transitionMode"></param>
        /// <returns></returns>
        IEnumerator TransitionInternal(string currentTransitionId, TransitionModeType transitionMode)
        {
            bool currentTransitionisActiveTransition = true;
            // delay
            if (_transitionSettings.Delay != 0) yield return new WaitForSeconds(_transitionSettings.Delay);

            var normalisedFactor = 1/ _transitionSettings.Duration;
            while (Progress < 1)
            {
                // exit if a newer transition has been started
                currentTransitionisActiveTransition = currentTransitionId == _activeTransitionId;
                if (!currentTransitionisActiveTransition) break;

                // update progress
                Progress += normalisedFactor * Time.deltaTime;
                SetProgress(Progress);

                // update amount based upon the target type
                if (_targetType == TargetTypeEnum.Float)
                {
                    SetAmount(TransitionValue(_startFloat, _endFloat, Progress));
                }
                else
                {
                    SetAmount(new Vector3(
                        TransitionValue(_startVector3.x, _endVector3.x, Progress),
                        TransitionValue(_startVector3.y, _endVector3.y, Progress),
                        TransitionValue(_startVector3.z, _endVector3.z, Progress)));
                }

                yield return 0;
            }

            // set end value if this transition is still running.
            currentTransitionisActiveTransition = currentTransitionId == _activeTransitionId;
            if (currentTransitionisActiveTransition)
            {
                if (_targetType == TargetTypeEnum.Float)
                    SetAmount(_endFloat);
                else
                    SetAmount(_endVector3);
            }

            // finally call callback
            if (transitionMode == TransitionModeType.In)
                TransitionInComplete(currentTransitionisActiveTransition);
            else if (transitionMode == TransitionModeType.Out)
                TransitionOutComplete(currentTransitionisActiveTransition);
        }


        /// <summary>
        /// based upon the current setup, transition a float between start and end.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        float TransitionValue(float start, float end, float value)
        {
            if (_transitionSettings.TransitionType == TransitionHelper.TransitionType.AnimationCurve)
            {
                return EaseAnimationCurve(start, end, value);
            }
            else if (_easingFunction != null)
            {
                return _easingFunction(start, end, value);
            }
            return end;
        }


        /// <summary>
        /// Override this with your custom set amount function
        /// </summary>
        /// <param name="amount"></param>
        public virtual void SetAmount(float amount)
        {
            Amount = amount;
        }


        /// <summary>
        /// Override this with your custom set amount function
        /// </summary>
        /// <param name="amount"></param>
        public virtual void SetAmount(Vector3 amount)
        {
        }


        /// <summary>
        /// Override this if you need to update based upon the progress (0..1)
        /// </summary>
        /// <param name="progress"></param>
        public virtual void SetProgress(float progress)
        {
            Progress = progress;
        }


        #region Transition Complete Callbacks

        /// <summary>
        /// Called when an in transition has been completed (or interupted)
        /// </summary>
        public virtual void TransitionInComplete(bool transitionCompleted)
        {
            TransitionMode = TransitionModeType.Out;
            if (TransitionInConfig.OnTransitionComplete != null)
                TransitionInConfig.OnTransitionComplete.Invoke();
        }


        /// <summary>
        /// Called when an out transition has been completed (or interupted)
        /// </summary>
        public virtual void TransitionOutComplete(bool transitionCompleted)
        {
            if (TransitionOutConfig.OnTransitionComplete != null)
                TransitionOutConfig.OnTransitionComplete.Invoke();
        }

        #endregion transition complete callbacks

        #region Additional Ease Functions

        public float EaseAnimationCurve(float start, float end, float value)
        {
            var curveStart = _transitionSettings.AnimationCurve.keys[0].time;
            var curveLength =
                _transitionSettings.AnimationCurve.keys[_transitionSettings.AnimationCurve.keys.Length - 1].time -
                curveStart;
            return _transitionSettings.AnimationCurve.Evaluate(curveStart + curveLength * value);
        }

        #endregion Additional Ease Functions

        /// <summary>
        /// Transition setting calss exposed through the editor
        /// </summary>
        [System.Serializable]
        public class TransitionSettings
        {
            [Tooltip("Whether to automatically check and run transitions on child GameObjects.")]
            public bool TransitionChildren = false;
            [Tooltip("Whether this must be transitioned specifically. If not set it will run automatically when a parent transition is run that has the TransitionChildren property set.")]
            public bool MustTriggerDirect = false;
            [Tooltip("Time in seconds before this transition should be started.")]
            public float Delay;
            [Tooltip("How long this transition will / should run for.")]
            public float Duration = 0.3f;
            [Tooltip("How the transition should be run.")]
            public TransitionHelper.TransitionType TransitionType = TransitionHelper.TransitionType.linear;
            [Tooltip("A custom curve to show how the transition should be run.")]
            public AnimationCurve AnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            [Tooltip("Methods that should be called when the transition is started.")]
            public UnityEvent OnTransitionStart;
            [Tooltip("Methods that should be called when the transition has completed.")]
            public UnityEvent OnTransitionComplete;
        }
    }
}
