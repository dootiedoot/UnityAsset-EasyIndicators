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
using UnityEditor;
using UnityEngine;

namespace FlipWebApps.BeautifulTransitions.Scripts.Transitions.Editor
{
    [CustomPropertyDrawer(typeof(TransitionBase.TransitionSettings))]
    public class TransitionSettingsDrawer : PropertyDrawer
    {
        bool showAdvanced = false;
        bool showEvents = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var transitionSettings = fieldInfo.GetValue(property.serializedObject.targetObject) as TransitionBase.TransitionSettings;

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel = 1;

                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("Delay"));
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("Duration"));
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("TransitionType"));
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

                if (transitionSettings.TransitionType == TransitionHelper.TransitionType.none)
                {
                    EditorGUI.indentLevel += 1;
                    EditorGUI.HelpBox(EditorGUI.IndentedRect(position), "This transition will be ignored!", MessageType.Info);
                    EditorGUI.indentLevel -= 1;
                }

                else if (transitionSettings.TransitionType == TransitionHelper.TransitionType.AnimationCurve)
                {
                    EditorGUI.indentLevel += 1;
                    Rect helpPosition = EditorGUI.IndentedRect(position);
                    helpPosition.height *= 2;
                    EditorGUI.HelpBox(helpPosition, "Custom animation curve with absolute values.\nClick the curve below to edit.", MessageType.Info);
                    position.y += helpPosition.height + EditorGUIUtility.standardVerticalSpacing;
                    position.height = EditorGUIUtility.singleLineHeight * 4;
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("AnimationCurve"), GUIContent.none);
                    EditorGUI.indentLevel -= 1;
                }

                else
                {
                    EditorGUI.indentLevel += 1;

                    var easingFunction = TweenMethods.GetEasingFunction(transitionSettings.TransitionType);
                    if (easingFunction == null)
                    {
                        // should never happen, but worth checking
                        EditorGUI.HelpBox(EditorGUI.IndentedRect(position), "Curve not found! Please report this error.", MessageType.Error);
                    }
                    else
                    {
                        EditorGUI.HelpBox(EditorGUI.IndentedRect(position), "Fixed Transition Curve.", MessageType.Info);
                        position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                        position.height = EditorGUIUtility.singleLineHeight * 5;

                        // set default texture color
                        var texture = new Texture2D((int) position.width, (int) position.height, TextureFormat.ARGB32,
                            false);
                        var colors = new Color[texture.width*texture.height];
                        for (var  i = 0; i < colors.Length; i++)
                            colors[i] = Color.black;

                        // First calculate min / max y as function might send values below 0 or above 1
                        var normalisedWidth = 1f/texture.width;
                        var minValue = float.MaxValue;
                        var maxValue = float.MinValue;
                        for (var i = 0; i < texture.width; i++)
                        {
                            var normalisedX = normalisedWidth * i;
                            var y = easingFunction(0, 1, normalisedX);
                            if (y < minValue) minValue = y;
                            if (y > maxValue) maxValue = y;
                        }

                        // plot values for all columns. graph, 0 and 1
                        var zeroRow = GetGraphRow(minValue, maxValue, 0, texture.height);
                        var oneRow = GetGraphRow(minValue, maxValue, 1, texture.height);
                        for (var i = 0; i < position.width; i++)
                        {
                            // lines at 0 and 1
                            PlotGraphPoint(i, zeroRow, texture.width, texture.height, colors, Color.gray);
                            PlotGraphPoint(i, oneRow, texture.width, texture.height, colors, Color.gray);

                            // graph value
                            var normalisedX = normalisedWidth * i;
                            var value = easingFunction(0, 1, normalisedX);
                            PlotGraphPosition(i, texture.width, minValue, maxValue, value, texture.height, colors, Color.green);
                        }
                        // Set and apply pixels
                        texture.SetPixels(colors);
                        texture.Apply();

                        // workaround given DrawPreviewTexture doesn't seem to work properly (disappears after a short while)!
                        GUIStyle style = new GUIStyle();
                        style.normal.background = texture;
                        EditorGUI.LabelField(position, GUIContent.none, style);

                        //EditorGUI.DrawPreviewTexture(position, texture);
                        EditorGUI.indentLevel -= 1;
                    }
                }

                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                position.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                showAdvanced = EditorGUI.Foldout(position, showAdvanced, new GUIContent("Advanced"));
                if (showAdvanced)
                {
                    position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("TransitionChildren"));
                    position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("MustTriggerDirect"));
                }

                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                position.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                showEvents = EditorGUI.Foldout(position, showEvents, new GUIContent("Events"));
                if (showEvents)
                {
                    position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                    position.height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("OnTransitionStart"));
                    Rect eventsPosition = EditorGUI.IndentedRect(position);
                    EditorGUI.PropertyField(eventsPosition, property.FindPropertyRelative("OnTransitionStart"));

                    position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                    position.height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("OnTransitionComplete"));
                    eventsPosition = EditorGUI.IndentedRect(position);
                    EditorGUI.PropertyField(eventsPosition, property.FindPropertyRelative("OnTransitionComplete"));
                }
            }
            EditorGUI.EndProperty();
        }


        static void PlotGraphPosition(int column, float graphWidth, float minValue, float maxValue, float value, float graphHeight, IList<Color> colors, Color color)
        {
            var row = GetGraphRow(minValue, maxValue, value, graphHeight);
            PlotGraphPoint(column, row, graphWidth, graphHeight, colors, color);
        }

        private static void PlotGraphPoint(int column, int row, float graphWidth, float graphHeight, IList<Color> colors, Color color)
        {
            if (row >= 0 && row < graphHeight)
                colors[column + row * (int)graphWidth] = color;
        }

        static int GetGraphRow(float minValue, float maxValue, float value, float graphHeight)
        {
            var graphYDistance = maxValue - minValue;
            var zeroOffsetRow = value - minValue;
            var row = (int)(graphHeight / graphYDistance * zeroOffsetRow);
            return row;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var transitionSettings = fieldInfo.GetValue(property.serializedObject.targetObject) as TransitionBase.TransitionSettings;
            // label
            var height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (property.isExpanded)
            {
                // main fields
                height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing)*3f;
                // transition type extra
                if (transitionSettings != null)
                {
                    // info message.
                    height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    // for display of the curve
                    if (transitionSettings.TransitionType != TransitionHelper.TransitionType.none)
                        height += EditorGUIUtility.singleLineHeight * 5;
                    
                    // other
                    if (transitionSettings.TransitionType == TransitionHelper.TransitionType.AnimationCurve)
                        height += EditorGUIUtility.singleLineHeight;

                    // get height for advanced properties
                    height += +EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    if (showAdvanced)
                    {
                        height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2f;
                    }

                    // get height for event properties.
                    height += +EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    if (showEvents)
                    {
                        height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("OnTransitionStart"));
                        height += EditorGUIUtility.singleLineHeight;
                        height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("OnTransitionComplete"));
                    }

                }
                // end spacing
                height += EditorGUIUtility.standardVerticalSpacing;
            }
            return height;
        }
    }
}
