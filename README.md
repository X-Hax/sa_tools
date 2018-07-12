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

For reference, the project's source data will be saved to /game_path/Projects/project_name/ You shouldn't need to mess with things in this folder for most mods, with the primary exception being new content like textures and audio that would normally go in the /system/ or /gd_PC/ folders. Anything you put into those folders will get copied to the proper location by the build process.

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

## Auto-Build
If you're creating an INI mod and don't need any C++ code at all, use this. Click on either Auto-Build, or 'Build and Run'. The two options are identical aside from one difference: if you click 'Build and Run', the game will start as soon as the build is complete.

## Manual Build
If you're making a more complicated mod (like one that uses C++ code), use the 'manual build' option. It will open a window that allows you to select which data files will be exported. You can export to either INI or C++. If you choose the C++ option, the C++ files will be exported to the /game/Projects/project_name/source/ folder. Otherwise, you will be prompted for the location to save the ini files. Usually this is /game/mods/project_name/

You will then need to add the ini files as entries to /game/mods/project_name/mod.ini

    Name=Test Project
    Description=Testing the new SA Tools pipeline
    Author=You
    EXEData=exeData.ini
