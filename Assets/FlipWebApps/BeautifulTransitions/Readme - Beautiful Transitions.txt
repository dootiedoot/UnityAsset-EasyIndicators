Beautiful Transitions v2.1.1

Thank you for using Beautiful Transitions. 

If you have any thoughts, comments, suggestions or otherwise then please contact us through our website or 
drop me a mail directly on mark_a_hewitt@yahoo.co.uk

Please consider rating this asset on the asset store.

Regards,
Mark Hewitt

For more details please visit: http://www.flipwebapps.com/ 
For tutorials visit: http://www.flipwebapps.com/

- - - - - - - - - -

QUICK START

	1. If you have an older version installed:
		1.1. Make a backup of your project
		1.2. Delete the old /FlipWebApps/BeautifulTransitions folder to cater for possible conflicts.
	2. Check out the demo scenes under /FlipWebApps/BeautifulTransitions/_Demo

NOTE: Unity 5.0 - 5.1 does not support the dropdown used in the ScreenCamera demo scene. The transitions work, but you will have to preview them manually.

- - - - - - - - - -

CHANGE LOG

v2.2
	Improvements
	- Exposed Transition Start and Complete events through the inspector so you can easily hook up other code.
	- Transitions are now shown on the component menu
	- Demo: Added Events Demo
	- Demo: Added SceneSwap Demo

v2.1.1
	Fixes
	- Under certain build conditions the shaders would not be included. Moved shaders to a resources folder and 
	improved load validation.

v2.1
	Improvements
	- Shake Camera: Added ShakeCamera component and shake helper for shaking other gameobjects.
	- Code refactor improvements (some shared script files moved to the Helper folder)

	Fixes
	- Added demo titles

v2.0
	Improvements
	- All previous transition curves are now built in, removing the dependency on iTween and giving improved performance.
	- Updated inspector gui with visual curves.
	- Code refactor improvements (if you experience any build errors with your own derived classes that you don't know how to fix then let us know)

	Fixes
	- Fixed unpredictable handling of concurrent transition calls and multiple calls to the same transition

v1.2
	Improvements
	- Added the ability to define your own animation curves.
	- Added Property Tool Tips. 
	- Added Custom Property Editor for improved UI. 
	- Added help link to components

	Fixes
	- Demo scene camera demo correctly only runs the specified transition.
	- iTween bug where start transition value was not always set until after 1 frame.
	- A disabled component would wrongly be run when placed as a nested transition.

v1.1
	Improvements
	- New Rotate UI / Game Object transition. 
	- Transition core code refactor.

	Fixes
	- Demo Black tint was setting wrong color.

v1.0
	Improvements
	- Rewritten core for easier extensibility
	- Added Screen transitions including fade and multiple wipe transitions
	- Added Camera transitions including fade and multiple wipe transitions
	- Added the possibility to easily create your own custom transitions by uploading a new Alpha texture
	- Added new demo for screen and camera transitions.

v0.8
	First public release