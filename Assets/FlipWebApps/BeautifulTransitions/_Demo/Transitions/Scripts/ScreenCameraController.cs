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
using System.Collections.Generic;
using FlipWebApps.BeautifulTransitions.Scripts.Transitions;
using FlipWebApps.BeautifulTransitions.Scripts.Transitions.Camera;
using FlipWebApps.BeautifulTransitions.Scripts.Transitions.Screen;
using UnityEngine;

namespace FlipWebApps.BeautifulTransitions._Demo.Transitions.Scripts
{
    public class ScreenCameraController : MonoBehaviour
    {
        public FadeCamera FadeCamera;
        public WipeCamera WipeCamera;
        public FadeScreen FadeScreen;
        public WipeScreen WipeScreen;
        public Texture2D OverlayTexture;
        public Texture2D[] WipeTextures;

        Color _color = Color.white;
        bool _showTexture = true;
        int _effect;

        public void SetColorWhite()
        {
            SetColor(Color.white);
        }

        public void SetColorRed()
        {
            SetColor(Color.red);
        }

        public void SetColorBlue()
        {
            SetColor(Color.blue);
        }

        public void SetColorGreen()
        {
            SetColor(Color.green);
        }

        public void SetColorBlack()
        {
            SetColor(Color.black);
        }

        void SetColor(Color color)
        {
            _color = color;
            FadeCamera.InConfig.Color = _color;
            FadeCamera.OutConfig.Color = _color;
            WipeCamera.InConfig.Color = _color;
            WipeCamera.OutConfig.Color = _color;
            FadeScreen.InConfig.Color = _color;
            FadeScreen.OutConfig.Color = _color;
            WipeScreen.InConfig.Color = _color;
            WipeScreen.OutConfig.Color = _color;
        }

        public void SetEffect(int effect)
        {
            _effect = effect;
        }

        public void SetShowTexture(bool showTexture)
        {
            _showTexture = showTexture;
            var texture = _showTexture ? OverlayTexture : null;
            FadeCamera.InConfig.Texture = texture;
            FadeCamera.OutConfig.Texture = texture;
            WipeCamera.InConfig.Texture = texture;
            WipeCamera.OutConfig.Texture = texture;
            FadeScreen.InConfig.Texture = texture;
            FadeScreen.OutConfig.Texture = texture;
            WipeScreen.InConfig.Texture = texture;
            WipeScreen.OutConfig.Texture = texture;
        }

        public void SetWipeTexture()
        {
            if (_effect < 1 || _effect - 1 > WipeTextures.Length) return;

            var texture = WipeTextures[_effect - 1];
            WipeCamera.InConfig.MaskTexture = texture;
            WipeCamera.OutConfig.MaskTexture = texture;
            WipeScreen.InConfig.MaskTexture = texture;
            WipeScreen.OutConfig.MaskTexture = texture;
        }

        public void DemoScreen()
        {
            // make sure these are set in the material.
            SetColor(_color);
            SetShowTexture(_showTexture);

            if (_effect == 0)
            {
                StartCoroutine(DemoCameraInternal(FadeScreen));
            }
            else
            {
                StartCoroutine(DemoCameraInternal(WipeScreen));
            }
        }

        public void DemoCamera()
        {
            if (_effect == 0)
            {
                FadeCamera.enabled = true;
                WipeCamera.enabled = false;
                StartCoroutine(DemoCameraInternal(FadeCamera));
            }
            else
            {
                FadeCamera.enabled = false;
                WipeCamera.enabled = true;
                StartCoroutine(DemoCameraInternal(WipeCamera));
            }
        }

        public IEnumerator DemoCameraInternal(TransitionBase transitionBase)
        {
            // make sure these are set in the materials.
            SetColor(_color);
            SetShowTexture(_showTexture);
            SetWipeTexture();

            float transitionTime = TransitionHelper.GetTransitionOutTime(new List<TransitionBase> {transitionBase});
            transitionBase.InitTransitionOut();
            transitionBase.TransitionOut();
            yield return new WaitForSeconds(transitionTime + 0.5f);
            transitionBase.TransitionIn();
        }

        public void ShowRatePage()
        {
            Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/56442");
        }
    }
}
