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

using UnityEngine;
using UnityEngine.UI;

namespace FlipWebApps.BeautifulTransitions.Scripts.Transitions.Screen
{
    [ExecuteInEditMode]
    public abstract class TransitionScreenBase : TransitionBase
    {
        protected Material _material;
        protected RawImage _rawImage;
        UnityEngine.GameObject _customTransitionGameObject;

        public virtual void Awake()
        {
            // if raw image is not present then create a new canvas
            _rawImage = GetComponent<RawImage>();
#if UNITY_EDITOR
            if (!Application.isPlaying && _rawImage == null) return; // don't create our own in edit mode!
#endif
            if (_rawImage == null)
            {
                _customTransitionGameObject = new UnityEngine.GameObject { name = "_BeautifulTransitions_" + this.GetType().Name };
                _customTransitionGameObject.SetActive(false);

                Canvas myCanvas = _customTransitionGameObject.AddComponent<Canvas>();
                myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                myCanvas.sortingOrder = 999;

                _rawImage = _customTransitionGameObject.AddComponent<RawImage>();
                _rawImage.enabled = false;
            }

            _material = _rawImage.material;
        }


        public override void InitTransitionIn()
        {
            base.InitTransitionIn();
            if (TransitionMode == TransitionModeType.In)
                SetRawImageEnabledState(true);
        }


        public override void TransitionIn()
        {
            SetRawImageEnabledState(true);
            base.TransitionIn();
        }


        public override void TransitionOut()
        {
            SetRawImageEnabledState(true);
            base.TransitionOut();
        }


        public override void TransitionInComplete(bool transitionCompleted)
        {
            base.TransitionInComplete(transitionCompleted);
            if (transitionCompleted) SetRawImageEnabledState(false);
        }


        protected void SetRawImageEnabledState(bool state)
        {
            _rawImage.enabled = state;
            if (_customTransitionGameObject != null)
                _customTransitionGameObject.SetActive(state);
        }
    }
}
