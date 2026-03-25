using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;
using SAModel.Direct3D;
using Color = System.Drawing.Color;

namespace SAModel.SAEditorCommon
{
    /// <summary>
    /// Class for creating log files.
    /// </summary>
    public static class Logger
	{
		/// <summary>Path to the log file.</summary>
		private static string LogFile;

		/// <summary>List of messages in the log.</summary>
		public static List<string> LogQueue { get; set; }

		/// <summary>
		/// Sets the log filename to match the currently running application.
		/// </summary>
		private static void GetLogFilename()
		{
			LogFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".log");
		}

		/// <summary>
		/// Initializes a logger that collects log information and writes it out in a file. The filename is EXEName.log.
		/// </summary>
		public static void Initialize()
		{
			GetLogFilename();
			LogQueue = new List<string>();
		}

		/// <summary>
		/// Add a message to the log queue.
		/// </summary>
		/// <param name="message">Text to log</param>
		public static void Add(string message)
		{
			LogQueue.Add(message);
		}

		/// <summary>
		/// Add a list of messages to the log queue.
		/// </summary>
		/// <param name="message">Strings list to log</param>
		public static void AddRange(List<string> messages)
		{
			LogQueue.AddRange(messages);
		}

		/// <summary>
		/// Clears log messages that haven't been written to the file yet.
		/// </summary>
		public static void ClearLogQueue()
		{
			LogQueue.Clear();
		}

		/// <summary>
		/// Writes all messages in the log queue to the file and clears the queue.
		/// </summary>
		public static void WriteLog()
		{
			if (string.IsNullOrEmpty(LogFile))
				GetLogFilename();
			try
			{
				if (!Directory.Exists(Path.GetDirectoryName(LogFile)))
					Directory.CreateDirectory(Path.GetDirectoryName(LogFile));
				File.AppendAllLines(LogFile, LogQueue);
				LogQueue.Clear();
			}
			catch { }
		}

		/// <summary>
		/// Clears the log file.
		/// </summary>
		public static void ClearLogFile()
		{
			if (string.IsNullOrEmpty(LogFile) || !File.Exists(LogFile))
				return;
			try
			{
				File.WriteAllText(LogFile, "");
			}
			catch { }
		}

		/// <summary>
		/// Deletes the log file.
		/// </summary>
		public static void DeleteLogFile()
		{
			if (string.IsNullOrEmpty(LogFile) || !File.Exists(LogFile))
				return;
			try
			{
				File.Delete(LogFile);
			}
			catch { }
		}

		/// <summary>
		/// Returns the log queue as a string.
		/// </summary>
		public static string GetLogString()
		{
			return string.Join(System.Environment.NewLine, LogQueue);
		}

		/// <summary>
		/// Opens the log file. If the log file doesn't exist, opens the SA Tools AppData folder.
		/// </summary>
		public static void OpenLog()
		{
			if (string.IsNullOrEmpty(LogFile))
				return;
			if (File.Exists(LogFile))
				System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("notepad", $"\"" + LogFile + "\"") { CreateNoWindow = false });
			else
				System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("explorer", $"\"" + Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools\"")) { CreateNoWindow = false });
		}
	}

	/// <summary>
	/// Class for OSD in 3D Editors.
	/// </summary>
	public static class OnScreenDisplay
	{
		public static Dictionary<string, int> MessageList { get; set; }
		private static List<OSDItem> OSDItems { get; set; }
		/// <summary>Sprite used to draw vertex indices in SAMDL.</summary>
		public static Sprite textSprite;
		private static Device d3ddevice;
		private static bool timer_freeze;
		private static RawColorBGRA logcolor;
		public static bool show_osd { get; set; }

		/// <summary>
		/// Initializes an OSD which displays colored text/log on screen.
		/// </summary>
		/// <param name="device">Direct3D device to draw on</param>
		public static void Initialize(Device device, RawColorBGRA color)
		{
			MessageList = new Dictionary<string, int>();
			OSDItems = new List<OSDItem>();
			textSprite = new Sprite(device);
			d3ddevice = device;
			logcolor = color;
			show_osd = true;
		}

		/// <summary>
		/// Draw pending log and OSD items. Call this before D3DDevice.Present().
		/// </summary>
		public static void ProcessMessages()
		{
			StringBuilder MessageString = new StringBuilder();
			// Update timers on render because the form's timer freezes when stuff is rendered
			UpdateTimer();
			// Create the full log string
			foreach (var key in MessageList.Keys.ToList())
			{
				MessageString.AppendFormat(key + "\n");
			}
			textSprite.Begin(SpriteFlags.AlphaBlend);
			// Process OSD items
			if (show_osd)
			{
				foreach (OSDItem osd in OSDItems.ToList())
				{
					SharpDX.Rectangle rec = EditorOptions.OnscreenFont.MeasureText(null, osd.text, FontDrawFlags.Right);
					EditorOptions.OnscreenFont.DrawText(textSprite, osd.text, osd.pos_x + 1 + rec.X, osd.pos_y + 1, Color.Black.ToRawColorBGRA());
					EditorOptions.OnscreenFont.DrawText(textSprite, osd.text, osd.pos_x + rec.X, osd.pos_y + 1, Color.Black.ToRawColorBGRA());
					EditorOptions.OnscreenFont.DrawText(textSprite, osd.text, osd.pos_x - 1 + rec.X, osd.pos_y - 1, Color.Black.ToRawColorBGRA());
					EditorOptions.OnscreenFont.DrawText(textSprite, osd.text, osd.pos_x + rec.X, osd.pos_y - 1, Color.Black.ToRawColorBGRA());
					EditorOptions.OnscreenFont.DrawText(textSprite, osd.text, osd.pos_x + rec.X, osd.pos_y, osd.color);
				}
			}
			// Process messages
			if (MessageList.Count > 0 && d3ddevice != null)
			{
				EditorOptions.OnscreenFont.DrawText(textSprite, MessageString.ToString(), 9, 9, Color.Black.ToRawColorBGRA());
				EditorOptions.OnscreenFont.DrawText(textSprite, MessageString.ToString(), 7, 7, Color.Black.ToRawColorBGRA());
				EditorOptions.OnscreenFont.DrawText(textSprite, MessageString.ToString(), 7, 9, Color.Black.ToRawColorBGRA());
				EditorOptions.OnscreenFont.DrawText(textSprite, MessageString.ToString(), 9, 7, Color.Black.ToRawColorBGRA());
				EditorOptions.OnscreenFont.DrawText(textSprite, MessageString.ToString(), 8, 8, logcolor);
			}
			textSprite.End();
			// Refresh messages after drawing
			MessageString.Clear();
		}

		/// <summary>
		/// Adds a message to display on the screen.
		/// </summary>
		/// <param name="message">Message text</param>
		/// <param name="timer">Duration of message in frames</param>
		public static void AddMessage(string message, int timer)
		{
			if (!MessageList.ContainsKey(message)) MessageList.Add(message, timer);
		}

		/// <summary>
		/// Clears current OSD message list.
		/// </summary>
		public static void ClearMessageList()
		{
			MessageList.Clear();
		}

		/// <summary>
		/// Class for individual messages that appear in the OSD.
		/// </summary>
		internal class OSDItem
		{
			public string text;
			public string id;
			public int timer;
			public int pos_x;
			public int pos_y;
			public RawColorBGRA color;
		}

		/// <summary>
		/// Adds or updates an OSD item.
		/// </summary>
		/// <param name="text">Item text</param>
		/// <param name="pos_x">Item X position</param>
		/// <param name="pos_y">Item Y position</param>
		/// <param name="color">Text color</param>
		/// <param name="identifier">Unique identifier, set to be able to update the item without recreating it</param>
		/// <param name="timer">Duration in frames, -1 for permanent, don't set to keep the previous value when updating an existing item</param>
		public static void UpdateOSDItem(string text, int pos_x, int pos_y, RawColorBGRA color, string identifier = "", int timer = 0)
		{
			foreach (OSDItem osd in OSDItems)
			{
				if (osd.id != "" && osd.id == identifier)
				{
					osd.text = text;
					if (timer != 0) osd.timer = timer;
					osd.pos_x = pos_x - 8;
					osd.pos_y = pos_y;
					osd.color = color;
					return;
				}
			}
			OSDItem newosd = new OSDItem();
			newosd.text = text;
			newosd.id = identifier;
			newosd.timer = timer;
			newosd.pos_x = pos_x - 8;
			newosd.pos_y = pos_y;
			newosd.color = color;
			OSDItems.Add(newosd);
		}

		/// <summary>
		/// Updates OSD/message timers and deletes old messages. Returns true if 3D view needs to be redrawn.
		/// </summary>
		public static bool UpdateTimer()
		{
			bool removeditems = false;
			foreach (var key in MessageList.Keys.ToList())
			{
				if (!timer_freeze) MessageList[key]--;
				if (MessageList[key] <= 0)
				{
					MessageList.Remove(key);
					removeditems = true;
				}
			}
			foreach (OSDItem osd in OSDItems.ToList())
			{
				if (osd.timer != -1)
				{
					osd.timer--;
					if (osd.timer <= 0)
					{
						OSDItems.Remove(osd);
						removeditems = true;
					}
				}
			}
			return removeditems;
		}
	}
}