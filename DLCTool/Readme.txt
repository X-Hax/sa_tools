Sonic Adventure DLC Tool

This is a command line tool to extract and rebuild DLC files for Sonic Adventure (Dreamcast).

BASIC USAGE
Extracting a DLC file: DLCTool <file.vms> [-w]
A new folder will be created with DLC assets and metadata.

Building a DLC file from a folder: DLCTool <folder> [-d]
A DLC with a "_new" suffix will be created.

ADDITIONAL SWITCHES
-w: Extract raw binary section in addition to the model contained in it.
-d: Don't encrypt the DLC file. The game won't recognize it but the file itself will be easier to inspect in a HEX editor.
-t: Decrypt a raw binary file with Darksecond/Sappharad's code.

GETTING STARTED
Extract any of the official DLCs, replace the converted resources, edit the metadata and rebuild the file.
Each DLC file can store one model, up to 16 text strings per each language and optionally a soundbank with custom sounds and/or music.
A DLC file contains the following sections: 
1) A general VMU header with things like the title and the icon;
2) An object layout (item table) to tell which objects to load in which level;
3) A string table containing messages in different languages;
4) A PVM file containing textures;
5) (Optional) An MLT file containing sound effects or music;
6) A PRS-compressed binary containing a model.

When you extract a DLC file, sections 4 and 5 are extracted as individual files, and section 6 is extracted as a model in .SA1MDL (SA Tools) format.
Sections 1-3 are converted to a single INI file (metadata), which will be explained below.

EDITING METADATA
The metadata file contains stuff like the name of the DLC, the messages displayed when you touch objects, and the item table.
To add new objects to the game you need to edit the item table. Here's an example of an item with comments:

[Item 0] //Item index, increases for every next item.
Level=26 //Internal ID of the level where the object will be loaded. 1-10 are Action Stages, 26 is Station Square, 29 and 32 are the Egg Carrier, 33 is the Mystic Ruins etc.
Act=3 //Act ID where the object will be loaded.
ScaleX=10 //Scale of the model/sprite multiplied by 10, e.g. 10 means the object will not be scaled.
ScaleY=10
ScaleZ=10
ObjectType=TYPE_MODEL //Kind of object. Can be TYPE_MODEL (3D model), TYPE_SPRITE (flat model), TYPE_INVISIBLE (no geometry).
Flags=FLAG_MESSAGE, FLAG_WARP //Object flags (see below).
MessageID=3 //ID of text string to display when you touch the object.
CollectibleID=1 //IDs for collectible objects. Each collectible needs to have a different ID that ranges from 1 to the total number of objects.
TriggerRadius=20 //Distance required to trigger the object's action.
TextureID=1 //Force the model's material to a specific texture ID, useful for billboards but can work with 3D models too.
WarpLevelOrSoundbank=35 //Level ID to warp to or soundbank ID to play the sound when the object is triggered.
WarpActOrSoundID=2 //Act ID to warp to or sound ID to play when the object is triggered.
RotSpeedX=0 //Rotation speed.
RotSpeedY=10
RotSpeedZ=0
RotationX=49152 //Object rotation.
RotationY=4000
RotationZ=0
X=698 //Object position.
Y=0
Z=1600

OBJECT FLAGS
FLAG_SOLID - Enable collision.
FLAG_SOUND - Play a sound when the object is triggered.
FLAG_MESSAGE - Show a message when the object is triggered.
FLAG_COLLISION_ONLY - Disable all interactive elements and use the object as invisible collision.
FLAG_WARP - Teleport the character to a different level when the object is triggered.
FLAG_COLLECTIBLE - Make the object collectible.
FLAG_TIMER - Use the object to count the time and keep track of how many items have been collected.
FLAG_CHALLENGE - Use the object to start the timer.
Notes: 
For objects with FLAG_TIMER, the CollectibleID field stores the number of objects to collect. 
For objects with FLAG_CHALLENGE the same field stores the maximum time to complete the challenge. The format is number of seconds divided by 10, e.g. 60 is 10 minutes.
For objects with FLAG_WARP the WarpLevelOrSoundbank field stores the level ID and WarpActOrSoundID field stores the act ID.
For objects with FLAG_SOUND the WarpLevelOrSoundbank field stores the bank ID and WarpActOrSoundID field stores the sound ID. Bank 8 is the DLC soundbank, 15 is ADX music.

TIPS
It's recommended to prepare a 16-color bitmap for the icon. If the edited icon has more colors, it will be quantized with possible loss of quality.
Text strings must be 64 characters long or less. Japanese strings must be 32 characters or less. Use \n for newlines and TAB (actual tabulation) for text centering.
To save space, use as few textures as possible and compress them using the VQ4 PVR format. You can save textures in this format using the PVR plugin for Photoshop.
You can stop the stage's regular ADX music by playing bank 15 sound 110.

TROUBLESHOOTING
If the game doesn't pick up your DLC file, check the following:
1) The filename on the VMU must be "SONICADV_XXX", where XXX is any 3-digit number. For example, a file named "SONICADV_512" will be loaded by the game.
2) If you have used the -d switch, make sure you aren't uploading an unencrypted file to the VMU.
3) If the VMS file size isn't divisible by 512, append extra zeroes at the end of the file (the tool tries to do this automatically).

CREDITS
Sappharad: providing C# code to decrypt VMS files and cracking the DLC checksum
Darksecond: original Perl code to decrypt VMS files

BONUS SECTION
For nerds here's an overview of SA1 DLC structures:

 * VMU HEADER
 * OFFSET	SIZE	TYPE		DESCRIPTION
 * 0		16		string		DLC title
 * 10		32		string		DLC description
 * 30		16		string		Application title
 * 40		2		ushort		Number of icons
 * 42		2		ushort		Animation speed
 * 44		2		ushort		Eyecatch type (unused)
 * 46		2		ushort		CRC (unused)
 * 48		4		uint32		Size without the header
 * 4C		20		null		Reserved
 * 60		32		ushort		Icon palette, 16 colors
 * 80		512		byte		Icon graphics
 * 
 * SECTIONS HEADER (SIZE 64 BYTES) 
 * 280		4		uint32		Pointer to item layout table
 * 284		4		uint32		Item count
 * 288		4		uint32		Pointer to string table
 * 28C		4		uint32		String item count
 * 290		4		uint32		Pointer to PVM
 * 294		4		uint32		Number of PVMs (always 1)
 * 298		4		uint32		Number of textures in the PVM
 * 29C		4		uint32		Pointer to MLT
 * 2A0		4		uint32		Number of MLTs (either 0 or 1)
 * 2A4		4		uint32		Pointer to PRS
 * 2A8		4		uint32		Number of PRSes (always 1)
 * 2AC		4		uint32		Checksum
 * 2B0		16		null		Unused
 * 
 * ITEM LAYOUT TABLE HEADER (SIZE 12 BYTES)
 * 2C0		4		uint32		DLC ID (e.g. 504 in SONICADV_504)
 * 2C4		1		byte		Enable Sonic / Enable Tails
 * 2C5		1		byte		Enable Knuckles / Enable Gamma
 * 2C6		1		byte		Enable Amy / Enable Big
 * 2C7		1		byte		Unknown, probably unused
 * 2C8		4		uint32		Regional lock
 *
 * ITEM LAYOUT TABLE (ARRAY BEGINS AT 0x2CC, ITEM SIZE 30 BYTES)
 *  0		1		uint8		Level ID
 *  1		1		uint8		Act ID
 *  2		1		uint8		Scale X multiplied by 10
 *  3		1		uint8		Scale Y multiplied by 10
 *  4		1		uint8		Scale Z multiplied by 10
 *  5		1		uint8		Rotation speed X
 *  6		1		uint8		Rotation speed Y
 *  7		1		uint8		Rotation speed Z
 *  8		1		sint8		Item type (0: model, -128: sprite, -1: invisible)
 *  9		1		uint8		Texture ID
 *  A		2		ushort		Flags
 *  C		1		uint8		Object ID used for collectible items or the number of objects to collect
 *  D		1		byte		Unknown
 *  E		1		uint8		Message ID to show when touching the object
 *  F		1		uint8		Trigger distance
 *  10		1		uint8		Level ID to warp or soundbank ID (8 for MLT, 15 for ADX music)
 *  11		1		uint8		Act ID to warp to or sound/music ID to play when touching the object
 *  12		2		ushort		Rotation X
 *  14		2		ushort		Rotation Y
 *  16		2		ushort		Rotation Z
 *  18		2		short		Position X
 *  1A		2		short		Position Y
 *  1C		2		short		Position Z
 *
 * DLC OBJECT FLAGS
 * BIT_0	Unknown
 * BIT_4	Unknown
 * BIT_8	Solid
 * BIT_9	Play sound
 * BIT_10	Show message
 * BIT_11	Hide object and disable everything except collision
 * BIT_12	Warp
 * BIT_13	Collectible item
 * BIT_14	Timer item
 * BIT_15	Starts the challenge
 *
 * REGIONAL LOCK BITS
 *	-1	Disable regional lock
 *	1	Japan
 *	3	US
 *	4	Europe
 *	7	All regions