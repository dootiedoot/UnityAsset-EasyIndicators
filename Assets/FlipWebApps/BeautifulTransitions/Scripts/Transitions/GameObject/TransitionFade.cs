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
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FlipWebApps.BeautifulTransitions.Scripts.Transitions.GameObject
{
    [AddComponentMenu("Beautiful Transitions/GameObject/Fade")]
    [HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
    public class TransitionFade : TransitionGameObjectBase
    {
        [Header("Fade Specific")]
        public InSettings FadeInConfig;
        public OutSettings FadeOutConfig;

        //[Header("Linked Items (not on target)")]
        CanvasGroup[] CanvasGroups = new CanvasGroup[0];
        Image[] Images = new Image[0];
        Text[] Texts = new Text[0];

        float _originalTransparency;


        public new void Awake()
        {
            base.Awake();

            // get the components to work on target
            var canvasGroup = Target.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                CanvasGroups = CanvasGroups.Concat(Enumerable.Repeat(canvasGroup, 1)).ToArray();
            }
            else {
                var image = Target.GetComponent<Image>();
                if (image != null)
                    Images = Images.Concat(Enumerable.Repeat(image, 1)).ToArray();

                var text = Target.GetComponent<Text>();
                if (text != null)
                    Texts = Texts.Concat(Enumerable.Repeat(text, 1)).ToArray();
            }

            _originalTransparency = GetCurrentTransparency();
        }

        public override void InitTransitionIn()
        {
            if (TransitionInConfig.TransitionType == TransitionHelper.TransitionType.none) return;
            TransitionMode = TransitionModeType.In;
            SetAmount(FadeInConfig.StartTransparency);
        }


        public override void TransitionIn () {
            Transition(TransitionInConfig, FadeInConfig.StartTransparency, _originalTransparency, TransitionModeType.In);
        }


        public override void TransitionOut() {
            Transition(TransitionOutConfig, GetCurrentTransparency(), FadeOutConfig.EndTransparency, TransitionModeType.Out);
        }


        float GetCurrentTransparency()
        {
            if (CanvasGroups.Length > 0)
                return CanvasGroups[0].alpha;
            if (Images.Length > 0)
                return Images[0].color.a;
            if (Texts.Length > 0)
                return Texts[0].color.a;

            return 1;
        }


        public override void SetAmount(float amount)
        {
            base.SetAmount(amount);
            foreach (var canvas in CanvasGroups)
                canvas.alpha = amount;
            foreach (var image in Images)
                image.color = new Color(image.color.r, image.color.g, image.color.b, amount);
            foreach (var text in Texts)
                text.color = new Color(text.color.r, text.color.g, text.color.b, amount);
        }


        [System.Serializable]
        public class InSettings
        {
            [Tooltip("Normalised transparency at the start of the transition (ends at the GameObjects initial transparency).")]
            public float StartTransparency = 0;
        }

        [System.Serializable]
        public class OutSettings
        {
            [Tooltip("Normalised transparency at the end of the transition (starts at the GameObjects current transparency).")]
            public float EndTransparency = 0;
        }
    }
}
