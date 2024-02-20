# UnityEditorWindowMinimize
A tool that helps minimize the Unity window

## Installation
Create a C# script in the Editor folder and copy the code into it.
## Usage
### Open: 
The window will be add to the menu Minimize/Open. You can open it at here, and choose some where place it.
### Add Window:
Click the 'SelectWindow' button, then click on the already opened window. When the window appears below the 'SelectWindow' button, it indicates successful addition.
### Minimize/Maximize Window: 
Click the button below to open and close (red means window is closed, green means window is normal, and blue means window is minimization). When the window is closed, the tool will attempt to open a window and output the name of the window to the console(Some windows cannot be opened, such as ShaderGraph, because the tool cant remember the edited target of shadergraph, after opening the corresponding window, the window will flash and disappear) .
### Set the position to hide the minimized window: 
By default, it should be hidden in the upper left corner of the editor. You can click the 'setpos' button, and a small window will appear. Move it to the position where you want to hide the minimized window and shrink it to the appropriate size. Click set to complete the process.
### Delete window: 
Click on the 'x' next to the button. If the window is minimized, it will be maximized before deletion.

Hope you love this small tool!
