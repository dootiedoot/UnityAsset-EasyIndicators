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

using FlipWebApps.BeautifulTransitions.Scripts.Transitions;
using UnityEngine;
using UnityEngine.UI;

namespace FlipWebApps.BeautifulTransitions._Demo.Transitions.Scripts
{
    public class TransitionEvents : MonoBehaviour {
        public GameObject TransitionGameobject;
        public Text Status;

        public void TransitionIn()
        {
            TransitionHelper.TransitionIn(TransitionGameobject);
        }

        public void TransitionOut()
        {
            TransitionHelper.TransitionOut(TransitionGameobject);
        }

        public void TransitionInStarted()
        {
            Status.text = "Transition In Started";
        }

        public void TransitionInCompleted()
        {
            Status.text = "Transition In Completed";
        }

        public void TransitionOutStarted()
        {
            Status.text = "Transition Out Started";
        }

        public void TransitionOutCompleted()
        {
            Status.text = "Transition Out Completed";
        }

        public void ShowRatePage()
        {
            Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/56442");
        }
    }
}
