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
using UnityEngine;

namespace FlipWebApps.BeautifulTransitions.Scripts.Transitions
{
    [HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]

    public class TransitionManager : MonoBehaviour {
        #region Singleton
        // Static singleton property
        public static TransitionManager Instance { get; private set; }
        public static bool IsActive { get { return Instance != null; } }

        void Awake()
        {
            // First we check if there are any other instances conflicting then destroy this and return
            if (Instance != null)
            {
                if (Instance != this)
                    Destroy(gameObject);
                return;             // return is my addition so that the inspector in unity still updates
            }

            // Here we save our singleton instance
            Instance = this as TransitionManager;

            // setup specifics for instantiated object only.
            ;
        }

        void OnDestroy()
        {
            // cleanup for instantiated object only.
            if (Instance == this) { }
        }
        #endregion Singleton

        public UnityEngine.GameObject[] DefaultSceneTransitions;


        public void TransitionOutAndLoadScene(string sceneName)
        {
            if (DefaultSceneTransitions.Length == 0)
                TransitionOutAndLoadScene(sceneName, new UnityEngine.GameObject[] { gameObject });
            else
                TransitionOutAndLoadScene(sceneName, DefaultSceneTransitions);
        }


        public void TransitionOutAndLoadScene(string sceneName, params UnityEngine.GameObject[] transitionGameObjects)
        {
            var transitionBases = new List<TransitionBase>();
            foreach (UnityEngine.GameObject transitionGameObject in transitionGameObjects)
            {
                transitionBases.AddRange(TransitionHelper.TransitionOut(transitionGameObject));
            }
            float delay = TransitionHelper.GetTransitionOutTime(transitionBases);
            LoadSceneDelayed(sceneName, delay);
        }


        public void LoadSceneDelayed(string sceneName, float delay)
        {
            StartCoroutine(LoadSceneDelayedCoroutine(sceneName, delay));
        }


        IEnumerator LoadSceneDelayedCoroutine(string sceneName, float delay)
        {
            if (!Mathf.Approximately(delay, 0))
                yield return new WaitForSeconds(delay);

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			Application.LoadLevel(sceneName);
#else
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
#endif
        }
    }
}
