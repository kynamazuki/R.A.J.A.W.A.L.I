Asset Version: 2.9.6

Changelog
---------

Additions/Improvements:

- Updated and improved the minimap so that it can display either a rendered map or a   prebaked map.

- Added Game State Condition component to enable something to run only if the current   game state is one of the specified values.

- Reworked the vehicle input framework to enable it to work with AI scripts.

- PlayerInput_InputSystem_SpaceshipControls: Added ability to enable auto-roll for specified device types.

- Gimbal Controller: Added getters/setters for max horizontal and vertical angular   velocities.   

- VehicleEngines3D: Made 'Max Steering Forces', 'Max Movement Forces', and 'Max Boost Forces' settable.   

- HUD Manager: Now searches through inactive child gameobjects for HUD components, so you can disable HUD components in the editor when they clutter the screen.   

- HUD Component: Added 'Unplugged' checkbox to enable temporary disabling of HUD components.   

- Added access to Healing properties for all weapon types.   

- Loadout Display Manager: improved code and added a list of created modules.   

- Module Mount: Removed unnecessary UnmountAllModules function.   

- SCKPlayerInput_InputSystem_ShipLanderControls: Now shows the control bindings as part of launch/land prompt.

- Aim Controller: Added setting to limit aim angle, so weapons don't fire backward when an object is in front of the camera.

- Ship Lander: Added option to auto land when the ship is above a landable surface.

- VehicleEngines3D: added getter/setter for Movement Input Response Speed.   

- Loadout Camera Controller:   Added camera collisions.   

- VehicleEngines3D: Made boost resource handlers list public.

- Gimballed Vehicle Controller: Added unity event called when follower transforms are updated.   

- Steering Follow Animator: Now extends the Vehicle Control Animation base class so works with other control animations.   

- Trackable: Made the Trackable Type settable.

- HUD Cursor: now parented to the camera by default, prevents jitter issues with World Space canvas.

- Game State Post Process Enabler: Now doesn't throw error if Post Process Volume is missing (e.g. when using URP).

- HUD Cursor: Added functions to aim along a specified direction, or at a specific world position.

- PlayerInput_InputSystem_SpaceshipControls: Added checkbox for whether this script should control the HUD Cursor.

- General Input: Added input start delay (default 0.5 seconds) to get rid of input buffered during level loading.

- Engines Exhaust Controller: Corrected to use modulated engine input values.   

- Vehicle Engines 3D Rumble: Corrected to use modulated boost input values.   

- Engine Audio Controller: Corrected to use modulated engine input values.   

- Engine Control Threshold Trigger: Corrected to use modulated engine input values.   

- Space Fighter Camera Controller: Uses modulated engine boost inputs when controlling FOV.   

- PlayerInput_InputSystem_SpaceshipControls: Gamepad input now triggers reset of HUD cursor position.

- PlayerInput_InputSystem_EnterExitControls: Now gets controls info from the Input System to display prompts on the UI.


Bugfixes:

- PlayerInput_InputSystem_SpaceshipControls: Fixed bug with steering when no HUD Cursor is active.

- Engines Exhaust Controller: Fixed bug with boost visual effects not working correctly at low throttle.

- Fixed bug with unmounting modules not working properly.

- Resource Container: Fixed bug with OnFilled and OnEmpty events not being called.

- VehicleEngines3D/Engines: Fixed bug with modulators being applied twice.