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
using UnityEngine;
using UnityEngine.Assertions;

namespace FlipWebApps.BeautifulTransitions.Scripts.Transitions.GameObject
{
    [AddComponentMenu("Beautiful Transitions/GameObject/Move")]
    [HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
    public class TransitionMove : TransitionGameObjectBase
    {
        public enum MoveModeType
        {
            Global,
            Local,
            AnchoredPosition
        };

        public enum MoveType
        {
            FixedPosition,
            Delta
        };

        [Header("Move Specific")]
        public MoveModeType MoveMode = MoveModeType.Global;

        public InSettings InConfig;
        public OutSettings OutConfig;

        Vector3 _originalPosition;


        public new void Start()
        {
            // validation
            if (MoveMode == MoveModeType.AnchoredPosition)
                Assert.IsNotNull(Target.transform as RectTransform, "The target of TransitionMove must contain a RectTransform component (not just a standard Transform component) when using MoveMode of type AnchoredPosition");

            _originalPosition = GetPosition();
            base.Start();
        }

        #region TransitionBase Overrides
        public override void InitTransitionIn()
        {
            if (TransitionInConfig.TransitionType == TransitionHelper.TransitionType.none) return;
            TransitionMode = TransitionModeType.In;
            SetAmount(InConfig.StartPosition);
        }


        public override void TransitionIn ()
        {
            var startPosition = InConfig.StartPositionType == MoveType.FixedPosition
                ? InConfig.StartPosition
                : _originalPosition + InConfig.StartPosition;
            Transition(TransitionInConfig, startPosition, _originalPosition, TransitionModeType.In);
        }


        public override void TransitionOut () {
            var endPosition = OutConfig.EndPositionType == MoveType.FixedPosition
                ? OutConfig.EndPosition
                : _originalPosition + OutConfig.EndPosition;
            Transition(TransitionOutConfig, GetPosition(), endPosition, TransitionModeType.Out);
        }


        public override void SetAmount(Vector3 position)
        {
            SetPosition(position);
        }

        #endregion TransitionBase Overrides

        #region Position

        Vector3 GetPosition()
        {
            if (MoveMode == MoveModeType.Global)
                return Target.transform.position;
            else if (MoveMode == MoveModeType.Local)
                return Target.transform.localPosition;
            else //MovePositionType.AnchoredPosition
                return ((RectTransform)Target.transform).anchoredPosition;
        }

        void SetPosition(Vector3 position)
        {
            if (MoveMode == MoveModeType.Global)
                Target.transform.position = position;
            else if (MoveMode == MoveModeType.Local)
                Target.transform.localPosition = position;
            else //MovePositionType.AnchoredPosition
                ((RectTransform)Target.transform).anchoredPosition = position;
        }

        #endregion Position

        [Serializable]
        public class InSettings
        {
            [Tooltip("Movement type.")]
            public MoveType StartPositionType;
            [Tooltip("Starting position (end at the GameObjects initial position).")]
            public Vector3 StartPosition = new Vector3(0, 0, 0);
        }

        [Serializable]
        public class OutSettings
        {
            [Tooltip("Movement type.")]
            public MoveType EndPositionType;
            [Tooltip("End position (end at the GameObjects current position).")]
            public Vector3 EndPosition = new Vector3(0, 0, 0);
        }
    }
}
