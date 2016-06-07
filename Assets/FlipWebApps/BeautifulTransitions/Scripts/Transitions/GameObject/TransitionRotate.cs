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

using System.Collections;
using UnityEngine;

namespace FlipWebApps.BeautifulTransitions.Scripts.Transitions.GameObject
{
    [AddComponentMenu("Beautiful Transitions/GameObject/Rotate")]
    [HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
    public class TransitionRotate : TransitionGameObjectBase
    {
        public enum RotationModeType
        {
            Global,
            Local
        };

        [Header("Rotate Specific")]
        public RotationModeType RotationMode = RotationModeType.Local;

        public InSettings InConfig;
        public OutSettings OutConfig;

        Vector3 _originalRotation;

        public new void Awake()
        {
            base.Awake();

            _originalRotation = GetRotation();
        }

        #region TransitionBase Overrides

        public override void InitTransitionIn()
        {
            if (TransitionInConfig.TransitionType == TransitionHelper.TransitionType.none) return;
            TransitionMode = TransitionModeType.In;
            SetAmount(InConfig.StartRotation);
        }


        public override void TransitionIn () {
            Transition(TransitionInConfig, InConfig.StartRotation, _originalRotation, TransitionModeType.In);
        }


        public override void TransitionOut () {
            Transition(TransitionOutConfig, GetRotation(), OutConfig.EndRotation, TransitionModeType.Out);
        }


        public override void SetAmount(Vector3 rotation)
        {
            SetRotation(rotation);
        }

        #endregion TransitionBase Overrides

        #region Rotation

        Vector3 GetRotation()
        {
            if (RotationMode == RotationModeType.Global)
                return Target.transform.eulerAngles;
            else //if (RotationMode == RotationModeType.Local)
                return Target.transform.localEulerAngles;
        }

        void SetRotation(Vector3 position)
        {
            if (RotationMode == RotationModeType.Global)
                Target.transform.eulerAngles = position;
            else //if (RotationMode == RotationModeType.Local)
                Target.transform.localEulerAngles = position;
        }

        #endregion Rotation

        [System.Serializable]
        public class InSettings
        {
            [Tooltip("Start rotation (end at the GameObjects initial rotation).")]
            public Vector3 StartRotation = new Vector3(0, 0, 0);
        }

        [System.Serializable]
        public class OutSettings
        {
            [Tooltip("End rotation (starts at the GameObjects current position).")]
            public Vector3 EndRotation = new Vector3(0, 0, 0);
        }
    }
}
