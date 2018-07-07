# Sonic Adventure Toolset

# Quick Start Guide

This branch currently does not have a release! You will need to work entirely from source.

## Building SA Tools
- First, you'll need [Visual Studio 2017](https://visualstudio.microsoft.com/vs/). Earlier versions won't work, since the codebase takes advantage of newer C# features.

- After Visual Studio is installed, clone the repository by using the "Clone or download" button on the repo home page.

- When the clone is complete, open Visual Studio. In the menu toolbar, use "File->Project/Solution". In the dialog box that opens, navigate to the folder containing the repository, and select the "SA Tools.sln" file to open the solution.

- You'll need to disable Load Lock exception handling, otherwise you won't be able to run the codebase from the IDE. In the menu toolbar, select "Debug->Windows->Exception Settings". A docked window at the bottom of the screen should appear. Expand the "Managed Debugging Assistants" entry, then scroll down until you see "Loader Lock". Ensure that "Loader Lock" is unchecked. NOTE: Sometimes Visual Studio will un-do this on you. If you get a loader lock exception when trying to debug in the editor, you'll need to repeat this step.

- To build, go to the menu-toolbar and select "Build->Build Solution". Wait a moment, then check the Output window to see if the build has succeeded or not. If it has, you're ready to go.

## Creating a new project.
The point of entry for all new mods is now the Project Manager. To boot it up, open the SA Tools solution ("SA Tools.sln") in visual studio. In the Solution Explorer tab (on the right hand side of the screen), scroll until you find "Project Manager". Press F5 to run.

### First-Time Project Manager config.
The first time you run Project Manager, it will need some information from you. Specifically, it needs to know where your games are. It's ok if you only have one of the games installed. Click the 'browse' button for the game you want to locate (SADXPC, or SA2PC) and then use the folder browser to specify the folder that contains either sonic.exe or sa2app.exe.

### New Project
In the project menu, select 'New Project'. In the dialog that appears, give your project a name. Don't use any characters that would be invalid for a folder name. Select whether you want it to be SADXPC or SA2PC, then click 'create'. The program will spend a noticable amount of time splitting all of the data out from the game's files. SA2 Note: not everything will be split this way. For some things, you will need to split the data manually (GUI for this not yet implemented).

## Editing the Project
When the project is done being created, you will be greeted by the Project Actions screen. Here you can select a tool to modify the project with, or build the project.

### Tools:
- SADXLVL2 is for modifying game stages. You can modify or generate completely new:
 - level geometry
 - SET (object) layouts
 - CAM (camera) layouts
 - MI (Mission) layouts
 - Splines
- SADXTweaker2 is for modifying various game-data. You can change:
 - (make a complete article for sadtweaker 2)
- SAMDL is for replacing and editing properties of in-game models. It will soon have features geared specifically towards character mods.


## Building A project

## About Building Projects
SADX:
There are two kinds of ways to 'build' a mod for SADXPC: ini mods (simple) and dll mods (advanced). Ini mods are simple data replacements. Dll mods are custom code models that can also have data embedded in them. DLL mods have the most power and can do anything that INI mods can do.

What kinds of things can be done for each type?
Ini:
- Level modifications
- Simple object replacements (enemy models, prop models and )
- Texture lists
- No code compiling required!

Dll:
- custom code
- complete character mods (face morphers, joint welds)
- Anything the Ini mods can do, and more.
- Code compiling required.

Either:
- Texture edits
- SET edits
- CAM edits

SA2:
More info coming soon

## Building a SADXPC Project (ini)
The build process is still, unfortunately, very manual. We will be improving this before release. In this example, we'll build an INI mod for SADXPC.

First step is to create the mod folder. This is different from the project folder. The project folder is for your source files. The mod folder is what you'll be giving way to players for them to use. Use the same name for the mod folder that you used when creating it. The path should be: sadx/mods/(your project name)/

Then, make a trivial edit to something that originally comes from sonic.exe (like Emerald Coast). Then, make a trivial edit of something that belongs to a DLL file (like an adventure field). Do not try messing with chrmodels. Character model edits are still in the realm of DLL editing only. Make a note of which DLL file your edit belongs to, it will be relevant later.

After your edits are made, Load up Project Manager. Select Open Project and specify your project. The Project Actions dialog should appear. To generate the ini for your dll-derived data:

- click the 'Build DLL Derived Data' button.
- DLLModGenerator will open in a new window.
- Click: File->Open
  - Supply the dialog with the file: sadx/Projects/(your project name)/(dll data name).ini example: ADV00MODELS_data.ini
- Ensure that everything you want to be exported is checked.
- Click the 'Export INI' button.
- Export the ini to sadx\Mods\(your project name)\(dll data name).ini
- the data should now be exported correctly.
- close the DLLModGenerator window.

Next, we'll do an extremely similar process, but for exporting the data that descends from sonic.exe. Back in the Project Actions window:

- Click the 'Build EXE Derived Data' button.
- StructConverter will open in a new window.
- Click: File->Open
  - Supply the dialog with the file: sadx\Projects\(your project name)\sonic_data.ini
- Ensure that everything you want to be exported is checked.
- Click the 'Export INI' button.
- Export the ini to sadx\Mods\(your project name)/exeData.ini
- The data should now be exported correctly.
- close the StructConverter window

We're almost done. The final step is to create our mod.ini. Create: sadx\Mods\(your project name)\mod.ini
Mod.ini has a very simple key/value format. Every line represents one key/value pair, separated by an '=' character.

    Name=Test Project
    Description=Testing the new SA Tools pipeline
    Author=You
    EXEData=exeData.ini

We'll need to add one more line to reference our dll-derived ini data file. The dll data references use the format <assembly name>data, so if we were to export a piece of ADV00MODELS.dll, we would supply the key/value pair ADV00MODELSData=Adv00Models.ini

That's it! Save your mod.ini file and then load up SADXModManager and enjoy your mod!
