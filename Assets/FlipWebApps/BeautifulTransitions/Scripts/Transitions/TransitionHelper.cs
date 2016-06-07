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

using System.Collections.Generic;
using UnityEngine;

namespace FlipWebApps.BeautifulTransitions.Scripts.Transitions
{
    [HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]

    public class TransitionHelper {
        /// <summary>
        /// See http://robertpenner.com/easing/easing_demo.html for examples of curves
        /// </summary>
        public enum TransitionType
        {
            none,
            easeInQuad,
            easeOutQuad,
            easeInOutQuad,
            easeInCubic,
            easeOutCubic,
            easeInOutCubic,
            easeInQuart,
            easeOutQuart,
            easeInOutQuart,
            easeInQuint,
            easeOutQuint,
            easeInOutQuint,
            easeInSine,
            easeOutSine,
            easeInOutSine,
            easeInExpo,
            easeOutExpo,
            easeInOutExpo,
            easeInCirc,
            easeOutCirc,
            easeInOutCirc,
            linear,
            spring,
            easeInBounce,
            easeOutBounce,
            easeInOutBounce,
            easeInBack,
            easeOutBack,
            easeInOutBack,
            easeInElastic,
            easeOutElastic,
            easeInOutElastic,
            AnimationCurve = 999
        }


        public static bool ContainsTransition(UnityEngine.GameObject gameObject)
        {
            var transitionBases = gameObject.GetComponents<TransitionBase>();
            return transitionBases.Length != 0;
        }


        public static List<TransitionBase> TransitionIn(UnityEngine.GameObject gameObject)
        {
            return TransitionIn(gameObject, false);
        }


        static List<TransitionBase> TransitionIn(UnityEngine.GameObject gameObject, bool isRecursiveCall) {
            var transitionBases = gameObject.GetComponents<TransitionBase>();
            var transitionList = new List<TransitionBase>();
            var callRecursive = false;

            // transition in transition items.
            foreach (var transitionBase in transitionBases) {
                // if first invoked on this gameobject, or don't need to trigger direct transition direct.
                if (transitionBase.isActiveAndEnabled && (isRecursiveCall == false || !transitionBase.TransitionInConfig.MustTriggerDirect)) {  
                    transitionBase.TransitionIn();
                    transitionList.Add(transitionBase);
                    // if we should transition children then set recursive flag
                    if (transitionBase.TransitionInConfig.TransitionChildren)                           
                        callRecursive = true;
                }
            }

            // if no transition items, or recursive call then process all child gameobjects
            if (transitionBases.Length == 0 || callRecursive)
            {
                for (var i = 0; i < gameObject.transform.childCount; i++)
                {
                    var transform = gameObject.transform.GetChild(i);
                    transitionList.AddRange(TransitionIn(transform.gameObject, true));
                }
            }

            return transitionList;
        }


        public static List<TransitionBase> TransitionOut(UnityEngine.GameObject gameObject)
        {
            return TransitionOut(gameObject, false);
        }


        static List<TransitionBase> TransitionOut(UnityEngine.GameObject gameObject, bool isRecursiveCall) {
            var transitionBases = gameObject.GetComponents<TransitionBase>();
            var transitionList = new List<TransitionBase>();
            var callRecursive = false;

            // transition out transition items.
            foreach (var transitionBase in transitionBases)
            {
                // if first invoked on this gameobject, or don't need to trigger direct transition direct.
                if (transitionBase.isActiveAndEnabled && (isRecursiveCall == false || !transitionBase.TransitionOutConfig.MustTriggerDirect))
                {
                    transitionBase.TransitionOut();
                    transitionList.Add(transitionBase);
                    // if we should transition children then set recursive flag
                    if (transitionBase.TransitionOutConfig.TransitionChildren)
                        callRecursive = true;
                }
            }

            // if no transition items, or recursive call then process all child gameobjects
            if (transitionBases.Length == 0 || callRecursive)
            {
                for (var i = 0; i < gameObject.transform.childCount; i++)
                {
                    var transform = gameObject.transform.GetChild(i);
                    transitionList.AddRange(TransitionOut(transform.gameObject, true));
                }
            }

            return transitionList;
        }


        public static float GetTransitionInTime(List<TransitionBase> transitionBases)
        {
            float transitionTime = 0;
            foreach (var transitionBase in transitionBases)
                transitionTime = Mathf.Max(transitionTime, transitionBase.TransitionInConfig.Delay + transitionBase.TransitionInConfig.Duration);
            return transitionTime;
        }


        public static float GetTransitionOutTime(List<TransitionBase> transitionBases)
        {
            float transitionTime = 0;
            foreach (var transitionBase in transitionBases)
                transitionTime = Mathf.Max(transitionTime, transitionBase.TransitionOutConfig.Delay + transitionBase.TransitionOutConfig.Duration);
            return transitionTime;
        }


        //void SetActiveAnimated(GameObject gameObject, bool active)
        //{
        //    var animator = gameObject.GetComponent<Animator>();
        //    if (animator != null)
        //    {
        //        animator.SetBool("Active", active);
        //    }
        //    gameObject.SetActive(active);
        //}


        //void SetActiveImmediate(GameObject gameObject, bool active)
        //{
        //    var animator = gameObject.GetComponent<Animator>();
        //    var layerIndex = animator.GetLayerIndex("Active");
        //    if (animator != null)
        //    {
        //        animator.Play(active ? "Active" : "Inactive", layerIndex, 1);
        //    }
        //    gameObject.SetActive(active);
        //}
    }
}
