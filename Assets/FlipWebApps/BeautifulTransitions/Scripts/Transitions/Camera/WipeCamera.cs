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

namespace FlipWebApps.BeautifulTransitions.Scripts.Transitions.Camera
{
    [ExecuteInEditMode]
    [AddComponentMenu("Beautiful Transitions/Camera/Wipe")]
    [HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
    public class WipeCamera : TransitionCameraBase
    {
        [Header("Wipe Specific")]
        public InSettings InConfig;
        public OutSettings OutConfig;

        Material _material;

        public void Awake()
        {
            var shader = Shader.Find("Hidden/FlipWebApps/BeautifulTransitions/WipeCamera");
            if (shader != null && shader.isSupported)
                _material = new Material(shader);
            else
                Debug.Log("WipeCamera: Shader is not found or supported on this platform.");
        }


        // Postprocess the image
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_material != null)
            {
                _material.SetTexture("_OverlayTex", TransitionMode == TransitionModeType.In ? InConfig.Texture : OutConfig.Texture);
                _material.SetColor("_Color", TransitionMode == TransitionModeType.In ? InConfig.Color : OutConfig.Color);
                _material.SetTexture("_MaskTex", TransitionMode == TransitionModeType.In ? InConfig.MaskTexture : OutConfig.MaskTexture);
                _material.SetFloat("_Amount", Amount);
                if (TransitionMode == TransitionModeType.In ? InConfig.InvertMask : OutConfig.InvertMask)
                    _material.EnableKeyword("INVERT_MASK");
                else
                    _material.DisableKeyword("INVERT_MASK");

                // We can set skip idle rendering to only do image effect if a transition is in progress. This can however cause problems with
                // multiple image effects.
                if (SkipIdleRendering &&
                    ((TransitionMode == TransitionModeType.In && TransitionInConfig.TransitionType == TransitionHelper.TransitionType.none) ||
                    (TransitionMode == TransitionModeType.Out && TransitionOutConfig.TransitionType == TransitionHelper.TransitionType.none) ||
                    Amount == 0)) return;

                Graphics.Blit(source, destination, _material);
            }
        }


        [System.Serializable]
        public class InSettings
        {
            [Tooltip("Optional overlay texture to use.")]
            public Texture2D Texture;
            [Tooltip("Tint color.")]
            public Color Color = Color.white;
            [Tooltip("Gray scale wipe mask.")]
            public Texture2D MaskTexture;
            [Tooltip("Whether to invery the wipe mask.")]
            public bool InvertMask;
        }

        [System.Serializable]
        public class OutSettings
        {
            [Tooltip("Optional overlay texture to use.")]
            public Texture2D Texture;
            [Tooltip("Tint color.")]
            public Color Color = Color.white;
            [Tooltip("Gray scale wipe mask.")]
            public Texture2D MaskTexture;
            [Tooltip("Whether to invery the wipe mask.")]
            public bool InvertMask;
        }
    }
}
