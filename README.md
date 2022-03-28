# UnityAsset-EasyIndicators

# Easy Indicators: https://goo.gl/57z7cB (Online Manual)

## A simple solution to On-Screen & Off-Screen indicators for Unity3D

Navigate to the scripts folder containing the IndicatorViewer script. This script will be used to see the indicators. Add it to a gameobject acting as the viewer.

Once added, a set of configurations will show. The default settings will be sufficient. However, the Default Indicator Panel needs a prefab that represents a indicator UI panel. This will be the default indicator UI for all targets.

Navigate to the Preset Panels Prefabs folder and drag-n-drop any of the preset prefabs to the Default Indicator Panel setting. In this case, we will use the Demo4 - RuntimeDemo_Panel.

Now we need a target to track. Simply navigate to scripts folder containing the IndicatorTarget script and add it to the target gameobject.

That is it, just press Play! You should see the target with the On-Screen indicator if it is within the main camera’s view. If the target moves out the view, it should point towards it with the Off-Screen indicator. Example with a capsule below:

# Main Components
## IndicatorViewer
The IndicatorViewer will allow the camera see the indicators. It can be attached to any gameobject to act as the manager. This script is responsible for calling each gameobject with the IndicatorTarget script to update their tracking. The parameter and settings of the IndicatorViewer will default to all targets unless it is overridden in the target’s IndicatorTarget script.


HOW-TO: Simply attach this script to a gameobject and assign the indicator panel prefab. Targets can also be added through code from this script.

## IndicatorTarget
The IndicatorTarget will allow the gameobject to be tracked by the viewer. This script is responsible for calculating the position, scaling, distance, and any necessities of the indicator. It will also create and control the IndicatorPanel associated with the target. The target can be any kind of type such as a button, NPC, flag, etc. The parameters for the IndicatorTarget will override the default ones from the viewer so use this to apply custom indicators for specific targets.


HOW-TO: Simply attach this script to the target to start tracking it.

## IndicatorPanel
The IndicatorPanel will allow the IndicatorTarget and utilities to control displaying of the indicators. This script is responsible containing reference to the On-Screen & Off-Screen indicator gameobject as well as contain the transition animations call methods. See any demo and their panel prefab for example.


HOW-TO: Simply grab the Indicator_Panel Template prefab under the Preset Panels Prefabs folder and add UI under the off-screen and/or on-screen panels. Then assign the indicator panel to the default viewer panel or any target. If you choose to not use the off-screen or on-screen indicator simply delete that panel from the template.

# Utilities
## Indicator Color
To automatically assign color to any indicators, simply add the IndicatorColor script from the Utilities folder to the gameobject that has a IndicatorTarget component on it and simply adjust the settings. This will set every graphic color in the indicator panel’s children to the assigned color. Color can also be assigned during runtime through code.

NOTE: You can also use this script to just color a gameobject and it’s children that does not have an indicator.

## Distance Tracker
The distance tracker script will create a text that will display the distance from the viewer. Example:

HOW-TO: Simply add the IndicatorDistanceTracker script from the Utilities folder to a gameobject that has a IndicatorTarget component on it and simply adjust the settings. Simply enable/disable the script to stop displaying distance.

## Target Camera
The target camera will allow the target’s real-time movement and animation in a icon-style image. A example of this can be seen in the Super Smash Bros. game series when someone is knocked outside of the viewing boundaries and their character can be seen inside a circle indicator.

HOW-TO: Simply add the IndicatorTargetCamera script from the Utilities folder to a gameobject that has a IndicatorTarget component on it and simply adjust the settings. Simply enable/disable the script to stop camera tracking.

# Scripting
Not much to say about scripting at the moment. The level of coding is about beginner-to-intermediate if you want to dive deep into the source code. Nonetheless, the code is documented and most important functions such as track and untracking targets are straightforward. Demo4 - Runtime is a great demo for seeing scripting in action as it contains majority of the public methods. Utility scripts can be turned on/off simply by enabling/disabling their scripts with the Enable method.

```C#
IndicatorViewer
IndicatorViewer.TrackTarget(GameObject target)
Will create a IndicatorTarget script on the target. If target already has an IndicatorTarget script attached, then the target will be added to the tracking list if not added already.
IndicatorViewer.UnTrackTarget(GameObject target)
Will remove target from the tracking list.
IndicatorViewer.GetIndicatorTarget(GameObject target)
Will return the IndicatorTarget component of the target.
IndicatorViewer.SetTracking(Bool trackOnScreen, Bool trackOffScreen)
Set which whether On-Screen and/or Off-Screen tracking is enabled/disabled.
IndicatorViewer.StartTracking()
will start tracking regardless if On/Off-Screen tracking is disabled. Enabled by default.
IndicatorViewer.StopTracking()
will stop tracking regardless if On/Off-Screen tracking is enabled.
IndicatorViewer.IsTracking
Return whether or not the viewer is currently tracking regardless if On/Off-Screen tracking is enabled.
IndicatorViewer.TrackOnScreen
Get or Set the status of On-Screen tracking. Enabled by default.
IndicatorViewer.TrackOffScreen
Get or Set the status of Off-Screen tracking. Enabled by default.
```

# Demos
## Super Smash Bros Example
<img src='./media/13 - m5rxKGP.gif'>

## Off-screen tracker Example
<img src='./media/14 - Y1TNHw6.gif'>

## Stress test Example
<img src='./media/ezgif-5-32aaac91a9.gif'>
