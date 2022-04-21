# BSPLoader

## Reason
This tool has been made to simplify mapping for Unity and to show that mapping is not hard.

## Tutorial

Youtube:<br>
[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/GArfgC-AmEs/0.jpg)](https://www.youtube.com/watch?v=GArfgC-AmEs)

## How the mapping scheme works?

1. Create a map in your editor.
2. Compile/Export this map to the quake1 .bsp format
3. Click the "Load" button to load the .bsp map into unity.


## Installation
#### Mapping tools
I personally use the Trenchbroom for making maps, and to make a .bsp file, I use Necros’ Compiling GUI 1.03.<br>Here is a tutorial video of the installation:
Here is a tutorial video of the installation: https://youtu.be/gONePWocbqA

1. Download<br>
Dependencies:<br>
https://github.com/Dataram57/LibBSP<br>
https://github.com/Dataram57/Unity3D-BSP-Importer<br><br>
BSPLoader script:<br>
https://github.com/Dataram57/Unity/blob/main/Tools/BSPLoader/BSPLoader.cs<br><br>
2. Install the scripts by putting the files(unpacked) into Unity. <br>


## Setup
1. On the top menu bar go to Tools/Dataram_57/BSPLoader.<br><br>
![Image of Yaktocat](https://github.com/Dataram57/Unity/blob/main/Tools/BSPLoader/tuto1.gif)<br><br>
2. Fill the input fields with:
    - Path to the folder with .bsp maps. (example: C:\Quake\id1\maps\)
    - Name of the map that you want to load. (example: e3m4) (without .bsp)
    - Path where you want to save in assets your loaded mesh. (example: Assets/maps/)
    - Path where materials are stored. (example: Assets/Materials/)<br><br>
    (BSPLoader instead of loading the textures into your projects, It uses the texture name to load the material(from input4) with the same name.)
<br><br>
![Image of Yaktocat](https://github.com/Dataram57/Unity/blob/main/Tools/BSPLoader/tuto2.png)<br><br>
