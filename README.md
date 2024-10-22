# Sonic Adventure Toolset
SA Tools is a toolkit for modifying and extracting data from the Sonic Adventure series of games. Before using these tools, we recommend having an understanding of basic 3d theory. If you're unfamiliar with the topic, read this overview [here](https://developer.mozilla.org/en-US/docs/Games/Techniques/3D_on_the_web/Basic_theory).

## Download SA Tools

[Download SA Tools - x64 version (recommended)](https://mm.reimuhakurei.net/SA%20Tools%20x64.7z)

[Download SA Tools - x86 version](https://mm.reimuhakurei.net/SA%20Tools%20x86.7z)

[.NET Desktop Runtime 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) - install the `Desktop` runtime matching the version of SA Tools (x86 or x64).

Development build (x64, manual updates only): 
[![Build status](https://ci.appveyor.com/api/projects/status/hobk9b466cvfrhov?svg=true)](https://ci.appveyor.com/project/PiKeyAr/sa-tools)

## Quick Links
The following links will direct you to pages on the [wiki](https://github.com/X-Hax/sa_tools/wiki) to help get you started with modding.
 - [Making Mods or Ripping Assets](https://github.com/X-Hax/sa_tools/wiki/SA-Tools-Hub#creating-a-project)
 - [Contributing or working with the SA Tools source code](https://github.com/X-Hax/sa_tools/wiki/Contributing-to-the-Tools)

## Contacting us with issues:
You are encouraged to first [check the wiki](https://github.com/X-Hax/sa_tools/wiki) to see if it can assist in your problem. If this is inadequate, please [check the issues](https://github.com/X-Hax/sa_tools/issues) here on Github. If your issue has not previously been posted, we encourage you to post the issue here. Your issue should include the following:
- A clear, concise description of the problem
- Steps to reproduce the problem. If you cannot get the problem to happen on-command, we can't fix it
- Media such as screenshots or video that help illustrate the issue.
### Real-time assistance & discussion
The toolkit developers (and other community members familiar with it) are frequently available at:
- [The official x-hax Discord server](https://discord.gg/gqJCF47)
- The official x-hax IRC server: irc://irc.badnik.zone/x-hax

## Building SA Tools from source
After cloning the repository, build in either Debug or Release mode for x86 or x64. After the build has completed, run the program `buildSATools.exe` in the solution's root folder. The files will be located in the `output` folder. You could specify a different location as a command line argument for `buildSATools.exe`, e.g. `buildSATools D:\SATools`.

Alternatively, you could open individual tools from Visual Studio by starting a new Debug instance. Only Debug builds are supported in this mode.

## External Resources
Below are two wikis that were created to help in both mod creation and documenting assets in the PC titles.
 - [SADX Modding Guide](https://github.com/X-Hax/SADXModdingGuide/wiki)
 - [SA2PC Modding Guide](https://github.com/X-Hax/SA2BModdingGuide/wiki)
