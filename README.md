# Sonic Adventure Toolset

# Quick Start Guide

Whether you're programming or just using the tools, the first thing you'll need to do is separate the game's data from the executable. This is done using a program called Split.

## Using Split
- download the latest release from the [here](http://mm.reimuhakurei.net/SA%20Tools.7z)

- Decide which game your project is for. In this example, we'll be using SADX PC, but the steps should be similar for other titles.

- Extract it to a known location, preferably somewhere near but not within your specific game folder. I recommend calling it 'SATools', and this is what we'll refer to it by here.

- In the SA.Tools folder you created, find the folder name that matches the game that you're modifying. Again, we'll be using SADXPC. Copy everything within the SADXPC folder, and paste it into the SADX game folder (the same folder that contains sonic.exe)

- locate the 'splitall.bat' file. It will go through every data file it knows of (like sonic.exe, chrmodels.dll, etc) and extract whatever it can. This process will take awhile, and you'll be able to watch its process inside of a cmd box. If the box disappears nearly instantly after opening, something has gone wrong. Try executing it from the CLI so that you can see any error messages that show up.


## Building SA Tools
- First, you'll need [Visual Studio 2017](https://visualstudio.microsoft.com/vs/). Earlier versions won't work, since the codebase takes advantage of newer C# features.

- After Visual Studio is installed, clone the repository by using the "Clone or download" button on the repo home page.

- When the clone is complete, open Visual Studio. In the menu toolbar, use "File->Project/Solution". In the dialog box that opens, navigate to the folder containing the repository, and select the "SA Tools.sln" file to open the solution.

- You'll need to disable Load Lock exception handling, otherwise you won't be able to run the codebase from the IDE. In the menu toolbar, select "Debug->Windows->Exception Settings". A docked window at the bottom of the screen should appear. Expand the "Managed Debugging Assistants" entry, then scroll down until you see "Loader Lock". Ensure that "Loader Lock" is unchecked.

- To build, go to the menu-toolbar and select "Build->Build Solution". Wait a moment, then check the Output window to see if the build has succeeded or not. If it has, you're ready to start programming.
