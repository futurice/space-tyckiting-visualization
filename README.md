# space-tyckiting-visualization

Visualizes games played with space-tyckiting

https://github.com/futurice/space-tyckiting

Some features require Unity Pro to function.

## How to run (WebGL)
Embed the build to a webpage, and from the page you can run scripts to call functions in the game.
http://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html

To load a game data file, call
SendMessage ('Canvas', 'LoadGame', 'www.url.com/path/to/json');
or
SendMessage ('Canvas', 'LoadGameJson', '[{"valid json data"}]');

To set the time scale, call
SendMessage ('Canvas', 'SetTimeScale', '2');
Replace last parameter with desired time scale.

You might need to wait for the player to load before the functions can be called.

## How to run (Desktop)

Open built project from command line and supply game data json as an argument "path"

path=example.json

Game speed is can be controlled with optional argument "timescale"

timescale=2

For example on OSX, run from terminal with

open space-tyckiting.app --args path=test1.json timescale=2

Note that the path needs to without sub directories, or be absolute.

To change resolution and controller settings, start the application with shift (windows) or alt (mac) key pressed down.

## In-game instructions

Press "Start" to start or restart game

Keys 1-3 change camera setup, Escape to quit

Arrows move the camera, shift speeds up movement.


## Attirubtions

Game plane hexagonal mesh by Fredrik Holmström from https://github.com/fholm/unityassets
