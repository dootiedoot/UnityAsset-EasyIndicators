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
    [AddComponentMenu("Beautiful Transitions/GameObject/Move Target")]
    [HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
    public class TransitionMoveTraget : TransitionGameObjectBase
    {
        [Header("Move Specific Settings")]
        public InSettings MoveInConfig;
        public OutSettings MoveOutConfig;

        Vector3 _originalPosition;


        public new void Start()
        {
            _originalPosition = Target.transform.position;

            base.Start();
        }


        public override void InitTransitionIn()
        {
            if (TransitionInConfig.TransitionType == TransitionHelper.TransitionType.none) return;
            TransitionMode = TransitionModeType.In;
            SetAmount(MoveInConfig.StartTarget.transform.position);
        }


        public override void TransitionIn () {
            Transition(TransitionInConfig, MoveInConfig.StartTarget.transform.position, _originalPosition, TransitionModeType.In);
        }


        public override void TransitionOut () {
            Transition(TransitionOutConfig, Target.transform.position, MoveOutConfig.EndTarget.transform.position, TransitionModeType.Out);
        }


        public override void SetAmount(Vector3 position)
        {
            Target.transform.position = position;
        }


        [System.Serializable]
        public class InSettings
        {
            [Tooltip("GameObject used as a starting position (end at the GameObjects initial position).")]
            public UnityEngine.GameObject StartTarget;
        }

        [System.Serializable]
        public class OutSettings
        {
            [Tooltip("GameObject used as the ending position (starts at the GameObjects current position).")]
            public UnityEngine.GameObject EndTarget;
        }
    }
}
