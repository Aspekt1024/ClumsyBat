# ClumsyBat
A deceivingly cute and playful 2D side-scrolling avoidance game.  Reminiscent of Flappy Bird, Clumsy Bat includes upgradeable abilities, a variety of obstacles, and RPG and story elements. Designed for touch-screen mobile devices (Android and Apple), the player uses touch gestures to control the character to get to the end of each level unscathed. Boss levels included.

This is a product of two individuals:
    Dan Szabo       Programming, Design & Technical Artist
    Scott Robinson  Artist

# Installation:
1.  Clone this repoistory and open in Unit3D (This was last built in Unity 2017.1.1f1, and should also work in Unity5)
2.  Change the build settings to Android (Google Play Services will give compile errors if this isn't set)

# What's inside?
Scenes:
1.  'Play' is the starting point for players and holds the level map
2.  'Levels' describes the main level behaviour, and can be used to jump to specific levels
3.  'Boss' describes the boss level behaviour, and can be used to play-test boss encounters
4.  'LevelEditor' is just as it says! instructions for use below. This is used to create and play-test levels

Editors:
1.  Level Editor
2.  Boss Editor

# Level Editor
Level editing is done in edit mode

Press Space to access the Level Editor Inspector window. This will allow you to load, save and test levels.

Object Creation:
  Shift + 1   Moth
  Shift + 2   Stalactite
  Shift + 3   Mushroom
  Shift + 4   Spider
  Shift + 5   Unused
  Shift + 6   Event trigger
  
While an object is attached to the cursor:
  A       Rotate anti-clockwise
  D       Rotate clockwise
  S       Roate object 180 degrees
  G       Randomise Rotation
  Space   Reset object to initial rotation
  Escape  Delete the current object
  R-click Place the current object
  
Double-right click to attach an existing object to the cursor

While a cave piece is selected (not attached):
  Numpad 1-5    Change the cave section's shape
  Numpad +      Add a cave section after the selected one
  Numpad -      Remove the selected cave section (both top and bottom) - will also remove objects in this section
  
Special objects:
  Stalactites, Spiders and Moths each have unique attributes that can be altered in the inspector window.
  Stalactites are able to fall from the ceiling - a telegraph is shown in the editor to indicate the height it will be when the player crosses its position.
  Spiders can descend or swing - a telegraph is shown for the approximate swing path.
  Moths can be either Green, Blue or Gold (affecting abilities) and can be given a specific path to follow
  
# Boss Editor
The boss editor has not been developed for wide use, although you can still access the node editor to edit existing bosses easily. The boss editor can be accessed by Double-clicking on a StateMachine. There are multiple ways to do this.
Exiting State machines are stored in Assets/Resources/NPCs/Bosses/BossBehaviours
Alternatively, you can open the Boss.unity scene, click the Boss gameObject in the heirarchy, then double-click the Boss State Machine in the inspector.

Editing KingRockbreath1 will change the behaviour of the first boss. There are two parts to the editor, each with different right-click context menus.
  1.  The Root state machine - handles events and routing of states
  2.  Child states - handles the behaviour of the state
