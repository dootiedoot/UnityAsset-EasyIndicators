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
using UnityEngine;
using UnityEngine.Assertions;

namespace FlipWebApps.BeautifulTransitions.Scripts.Transitions.GameObject
{
    [AddComponentMenu("Beautiful Transitions/GameObject/Animation")]
    [HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
    public class TransitionAnimation : TransitionGameObjectBase
    {
        [Header("Animation Specific")]
        public InSettings InConfig;
        public OutSettings OutConfig;

        Animator _animator;

        public new void Awake()
        {
            base.Awake();

            _animator = Target.GetComponent<Animator>();
            Assert.IsNotNull(_animator, "Ensure that there is an Animator on the gameobject used by TransitionAnimation");

            _animator.enabled = false;
        }


        /// <summary>
        /// Enable the animator and set Speed to 0 so it isn't played.
        /// </summary>
        public override void InitTransitionIn()
        {
            base.InitTransitionIn();
            _animator.enabled = true;
            _animator.speed = 0;
        }


        public override void TransitionIn () {
            TransitionMode = TransitionModeType.In;
            Transition(TransitionInConfig.Delay, "TransitionIn", "TransitionIn", InConfig.Speed, TransitionInComplete);
        }


        public override void TransitionOut () {
            TransitionMode = TransitionModeType.Out;
            Transition(TransitionOutConfig.Delay, "TransitionOut", "TransitionOut", OutConfig.Speed, TransitionOutComplete);
        }


        public void Transition(float delay, string trigger, string targetState, float speed, Action<bool> doneCallback)
        {
            StartCoroutine(TransitionInternal(delay, trigger, targetState, speed, doneCallback));
        }


        IEnumerator TransitionInternal(float delay, string trigger, string targetState, float speed, Action<bool> doneCallback)
        {
            if (delay != 0) yield return new WaitForSeconds(delay);

            _animator.enabled = true;
            _animator.SetTrigger(trigger);
            _animator.speed = speed;

            //TODO this assumes that we don't interrupt the transition or animation - can get problems otherwise.
            bool stateReached = false;
            while (!stateReached)
            {
                yield return new WaitForEndOfFrame();
                if (!_animator.IsInTransition(0))
                    stateReached = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 || !_animator.GetCurrentAnimatorStateInfo(0).IsName(targetState);
            }

            if (doneCallback != null)
                doneCallback(true);
        }


        [System.Serializable]
        public class InSettings
        {
            [Tooltip("The Animator Speed.")]
            public float Speed = 1;
        }

        [System.Serializable]
        public class OutSettings
        {
            [Tooltip("The Animator Speed.")]
            public float Speed = 1;
        }
    }
}
