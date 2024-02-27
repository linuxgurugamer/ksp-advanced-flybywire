## Advanced Fly-By-Wire
### Kerbal Space Program input system overhaul mod for KSP 1.1 and later

[Download latest version](https://github.com/linuxgurugamer/ksp-advanced-flybywire/releases)

[Licensed under the MIT License](https://github.com/linuxgurugamer/ksp-advanced-flybywire/blob/master/LICENSE)

You can also find the mod on:
- [Official forums](https://forum.kerbalspaceprogram.com/index.php?/topic/175359-19x-afbw-revived-joystick-controller-mod/)
- Spacedock [Windows](https://spacedock.info/mod/1869) [Linux](https://spacedock.info/mod/1870/)

Original development by nlight: [mod page](https://forum.kerbalspaceprogram.com/index.php?/topic/85711-11-afbw-v17-beta-joystick-controller-mod/) [Github](https://github.com/AlexanderDzhoganov/ksp-advanced-flybywire)

#### Installation
1. Download and install prerequisite mods:
   * [Click Through Blocker](https://forum.kerbalspaceprogram.com/index.php?/topic/170747-19x-click-through-blocker/)
   * [ToolbarController](https://forum.kerbalspaceprogram.com/index.php?/topic/169509-141-toolbar-controller-for-modders/)
1. Simply extract the ZIP file into the root installation folder for your game.
   In Windows, the KSP folder will usually be directly in `%ProgramFiles%` or, if you are using Steam, `%ProgramFiles%\Steam\steamapps\common\Kerbal Space Program`.

##### OSX
Use `brew install sdl` then follow the instructions above.

### What is this?
This is a mod for [Kerbal Space Program](http://kerbalspaceprogram.com), a spaceship building/space exploration game by Squad.
It dramatically enhances the stock input system with a bunch of fixes and many new features.

### How is it better than stock?

- Edit your control setup at any time during flight, no need to go back to the main menu to change bindings.
- Supports almost all controller types through the SDL wrapper.
- Native support for the Xbox 360 controller (and PS3 controller using MotionJoy) through the XInput wrapper. A built-in example preset is available.
- Unlimited number of controller buttons and axes
- Full keyboard & mouse support
- Lower latency and better analog precision than KSP's stock input
- Smart dead-zone detection and analog calibration 
- Remaps analog inputs to achieve better precision
- Acceleration-based discrete inputs for precise keyboard flight
- Supports key combinations with an infinite number of keys
- Multiple presets per controller
- Extremely simple to configure and use
- Works alongside the stock input system. The mod will not override or break your current setup.

### How to use
![screenshot](http://i.imgur.com/NgIVDIQ.png)

The mod adds a new button to the mod toolbar.

Click the game controller icon or press Shift + L during flight to bring up Fly-By-Wire's main configuration screen.

From there you will see a list of detected controllers. You can click on "Enable" to enable a controller from the list - two new buttons will appear - "Presets" and "Configure".

"Presets" will open up the preset editor which is very similar to KSP's stock bindings editor. Using the preset editor you can modify your controller layout at any time.

"Configure" will open up the controller configuration screen. It allows you to set some configuration values as well as calibrate the controller if necessary.

After setting up your controller you should save your game (by using quicksave or exiting to space center), which will automatically save your controller configuration as well.

### Operating system compatibility
Fully tested and compatible with KSP x64 on Windows and Linux operating systems.
XInput support unavailable on Linux.

### Performance considerations
The mod is extremely lightweight both on performance and memory. It only does a bit of arithmetic and remapping of incoming inputs which should have
no noticeable effect on CPU usage. Memory usage is in the order of a few megabytes.

### Bug reports
Please report any bugs using [GitHub's issue tracker](https://github.com/linuxgurugamer/ksp-advanced-flybywire/issues).

### Changelog

[Click here for changelog](https://github.com/linuxgurugamer/ksp-advanced-flybywire/ksp-advanced-flybywire/master/CHANGELOG)

