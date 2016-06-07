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

using System.Collections.Generic;
using FlipWebApps.BeautifulTransitions.Scripts.Helper;
using UnityEngine;
using UnityEngine.Assertions;

namespace FlipWebApps.BeautifulTransitions.Scripts.ShakeCamera.Components {
    /// <summary>
    /// Shake the camera with teh specified settings.
    /// </summary>
    [AddComponentMenu("Beautiful Transitions/Shake Camera/Shake Camera")]
    [HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
    public class ShakeCamera : MonoBehaviour
    {
        #region Singleton
        // Static singleton property
        public static ShakeCamera Instance { get; private set; }
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
            Instance = this as ShakeCamera;

            // setup specifics for instantiated object only.
            Setup();
        }

        void OnDestroy()
        {
            // cleanup for instantiated object only.
            if (Instance == this) { }
        }
        #endregion Singleton

        /// <summary>
        /// The cameras to shake. If blank then either then a Camera on this gameobject or the main camera will be used instead.
        /// </summary>
        [Tooltip("The cameras to shake. If blank then either then a Camera on this gameobject or the main camera will be used instead.")]
        public List<Camera> Cameras;

        /// <summary>
        /// The duration to shake the camera for.
        /// </summary>
        [Tooltip("The duration to shake the camera for.")]
        public float Duration = 1f;

        /// <summary>
        /// The offset after which to start decaying (slowing down) the movement.
        /// </summary>
        [Tooltip("The offset after which to start decaying (slowing down) the movement.")]
        [Range(0, 1)]
        public float DecayStart = 0.75f;

        /// <summary>
        /// The shake movement range from the origin. Set any dimension to 0 to stop movement along that axis.
        /// </summary>
        [Tooltip("The shake movement range from the origin. Set any dimension to 0 to stop movement along that axis.")]
        public Vector3 Range = Vector3.one;


        /// <summary>
        /// Setup the cameras - called by Awake
        /// </summary>
        void Setup()
        {
            if (Cameras.Count < 1)
            {
                if (GetComponent<Camera>())
                    Cameras.Add(GetComponent<Camera>());

                if (Cameras.Count < 1)
                {
                    if (Camera.main)
                        Cameras.Add(Camera.main);
                }
            }

            Assert.AreNotEqual(0, Cameras.Count, "No Cameras assigned and not able to find any!");
        }


        /// <summary>
        /// Start shaking the cameras.
        /// </summary>
        public void Shake()
        {
            Shake(Duration, Range, DecayStart);
        }


        /// <summary>
        /// Start shaking the cameras.
        /// </summary>
        public void Shake(float duration, Vector3 range, float decayStart)
        {
            foreach (var camera in Cameras)
            {
                StartCoroutine(ShakeHelper.Shake(camera.transform, duration, range, decayStart));
            }
        }
    }
}