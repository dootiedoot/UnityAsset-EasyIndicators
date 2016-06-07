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
using System.Collections;

namespace FlipWebApps.BeautifulTransitions.Scripts.DisplayItem
{
    /// <summary>
    /// UI Helper functions
    /// </summary>
    [HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
    internal class DisplayItemHelper
    {
        /// <summary>
        /// Sync the active state with the animation parameters.
        /// </summary>
        /// <param name="gameObject"></param>
        public static void SyncActiveStateAnimated(GameObject gameObject)
        {
            gameObject.GetComponent<Animator>().SetBool("Active", gameObject.activeSelf);
        }


        /// <summary>
        /// Set the active state and trigger animation update.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="value"></param>
        public static IEnumerator SetActiveAnimated(GameObject gameObject, bool value)
        {
            Animator animator = gameObject.GetComponent<Animator>();

            if (value)
            {
                gameObject.SetActive(true);
                animator.Play("NotActive");
                animator.SetBool("Active", true);
            }
            else
            {
                animator.SetBool("Active", false);
                bool closedStateReached = false;
                while (!closedStateReached)
                {
                    if (!animator.IsInTransition(0))
                        closedStateReached = animator.GetCurrentAnimatorStateInfo(0).IsName("NotActive");

                    yield return new WaitForEndOfFrame();
                }

                gameObject.SetActive(false);
            }
        }
    }
}