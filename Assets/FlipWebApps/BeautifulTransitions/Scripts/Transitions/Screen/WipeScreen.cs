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

namespace FlipWebApps.BeautifulTransitions.Scripts.Transitions.Screen
{
    [AddComponentMenu("Beautiful Transitions/Screen/Wipe")]
    [HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
    [ExecuteInEditMode]
    public class WipeScreen : TransitionScreenBase
    {
        [Header("Wipe Specific")]
        public InSettings InConfig;
        public OutSettings OutConfig;

        bool _isCustomMaterialSetup = false;

        public override void Awake()
        {
            base.Awake();
// return if editor and no attached _rawImage
#if UNITY_EDITOR
            if (!Application.isPlaying && _rawImage == null) return;
#endif
            if (_material == null || _material.name == "Default UI Material")
            {
                var shader = Shader.Find("FlipWebApps/BeautifulTransitions/WipeScreen");
                if (shader != null && shader.isSupported)
                {
                    _rawImage.material = _material = new Material(shader);
                    _isCustomMaterialSetup = true;
                }
                else
                    Debug.Log("WipScreen: Shader is not found or supported on this platform.");
            }
        }


        public override void InitTransitionIn()
        {
            base.InitTransitionIn();
            if (TransitionMode == TransitionModeType.In)
            {
                SetConfiguration(InConfig.Texture, InConfig.Color, InConfig.MaskTexture, InConfig.InvertMask);
            }
        }

        public override void TransitionIn()
        {
            SetConfiguration(InConfig.Texture, InConfig.Color, InConfig.MaskTexture, InConfig.InvertMask);
            base.TransitionIn();
        }


        public override void TransitionOut()
        {
            SetConfiguration(OutConfig.Texture, OutConfig.Color, OutConfig.MaskTexture, OutConfig.InvertMask);
            base.TransitionOut();
        }


        void SetConfiguration(Texture2D texture, Color color, Texture2D maskTexture, bool invertMask)
        {
            _rawImage.texture = texture;
            if (_isCustomMaterialSetup)
            {
                _material.SetColor("_Color", color);
                _material.SetTexture("_MaskTex", maskTexture);
                if (invertMask)
                    _material.EnableKeyword("INVERT_MASK");
                else
                    _material.DisableKeyword("INVERT_MASK");
            }
        }


        public override void SetAmount(float amount)
        {
            base.SetAmount(amount);
// return if editor and no attached _rawImage
#if UNITY_EDITOR
            if (!Application.isPlaying && _rawImage == null) return;
#endif

            if (_isCustomMaterialSetup)
                _material.SetFloat("_Amount", Amount);
        }


        void OnValidate()
        {
            if (_material == null) return;

            SetAmount(Amount);
            SetConfiguration(InConfig.Texture, InConfig.Color, InConfig.MaskTexture, InConfig.InvertMask);
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
