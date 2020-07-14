using System.IO;
using System.Linq;
using System.Collections.Generic;
using SharpDX.Direct3D9;
using SonicRetro.SAModel.Direct3D;
using Color = System.Drawing.Color;
using System.Text;
using SharpDX.Mathematics.Interop;

namespace SonicRetro.SAModel.SAEditorCommon
{
	public class Logger
	{
		private bool filecreated;
		private string file;
		public List<string> LogQueue { get; set; }
		/// <summary>
		/// Initializes a logger that collects log information and writes it out in a specified file.
		/// </summary>
		/// <param name="filename">Path to the log file</param>
		public Logger(string filename)
		{
			file = filename;
			LogQueue = new List<string>();
		}
		/// <summary>
		/// Add a message to the log queue.
		/// </summary>
		/// <param name="message">Text to log</param>
		public void Add(string message)
		{
			LogQueue.Add(message);
		}
		/// <summary>
		/// Add a list of messages to the log queue.
		/// </summary>
		/// <param name="message">Strings list to log</param>
		public void AddRange(List<string> messages)
		{
			LogQueue.AddRange(messages);
		}
		/// <summary>
		/// Clears log messages that haven't been written to the file yet.
		/// </summary>
		public void ClearLogQueue()
		{
			LogQueue.Clear();
		}
		/// <summary>
		/// Writes all messages in the log queue to the file and clears the queue.
		/// </summary>
		public void WriteLog()
		{
			File.AppendAllLines(file, LogQueue);
			LogQueue.Clear();
		}
		/// <summary>
		/// Clears the log file.
		/// </summary>
		public void ClearLogFile()
		{
			File.WriteAllText(file, "");
		}
		/// <summary>
		/// Deletes the log file.
		/// </summary>
		public void DeleteLogFile()
		{
			File.Delete(file);
		}
	}

	public class OnScreenDisplay
	{
		public Dictionary<string, int> MessageList { get; set; }
		private List<OSDItem> OSDItems { get; set; }
		Sprite textSprite;
		public Device d3ddevice;
		public bool timer_freeze;

		/// <summary>
		/// Initializes an OSD which displays colored text/log on screen.
		/// </summary>
		/// <param name="device">Direct3D device to draw on</param>
		public OnScreenDisplay(Device device)
		{
			MessageList = new Dictionary<string, int>();
			OSDItems = new List<OSDItem>();
			textSprite = new Sprite(device);
			d3ddevice = device;
		}

		/// <summary>
		/// Draw pending log and OSD items. Call this before D3DDevice.Present().
		/// </summary>
		public void ProcessMessages()
		{
			StringBuilder MessageString = new StringBuilder();

			//Process log messages
			foreach (var key in MessageList.Keys.ToList())
			{
				if (d3ddevice != null && !timer_freeze) MessageList[key]--;
				if (MessageList[key] <= 0) MessageList.Remove(key);
				else MessageString.AppendFormat(key + "\n");
			}

			//Process OSD items
			foreach (OSDItem osd in OSDItems)
			{
				if (d3ddevice != null && !timer_freeze && osd.timer != -1)
				{
					osd.timer--;
					if (osd.timer <= 0) OSDItems.Remove(osd);
				}
				textSprite.Begin(SpriteFlags.AlphaBlend);
				EditorOptions.OnscreenFont.DrawText(textSprite, osd.text, osd.pos_x + 1, osd.pos_y + 1, Color.Black.ToRawColorBGRA());
				EditorOptions.OnscreenFont.DrawText(textSprite, osd.text, osd.pos_x, osd.pos_y, osd.color);
				textSprite.End();
			}

			//Draw stuff
			if (MessageList.Count > 0 && d3ddevice != null)
			{
				textSprite.Begin(SpriteFlags.AlphaBlend);
				EditorOptions.OnscreenFont.DrawText(textSprite, MessageString.ToString(), 9, 9, Color.Black.ToRawColorBGRA());
				EditorOptions.OnscreenFont.DrawText(textSprite, MessageString.ToString(), 8, 8, Color.FromArgb(245, 220, 220, 240).ToRawColorBGRA());
				textSprite.End();
			}

			//Refresh messages after drawing
			MessageString.Clear();
		}
		/// <summary>
		/// Adds a message to display on the screen.
		/// </summary>
		/// <param name="message">Message text</param>
		/// /// <param name="timer">Duration of message in frames</param>
		public void AddMessage(string message, int timer)
		{
			if (!MessageList.ContainsKey(message)) MessageList.Add(message, timer);
		}
		internal class OSDItem
		{
			public string text;
			public int timer;
			public int pos_x;
			public int pos_y;
			public RawColorBGRA color;
		}
		/// <summary>
		/// Adds a text item to the OSD.
		/// </summary>
		/// <param name="text">Item text</param>
		/// <param name="timer">Duration in frames, -1 for permanent</param>
		/// <param name="pos_x">Item X position</param>
		/// <param name="pos_y">Item Y position</param>
		/// <param name="color">Text color</param>
		public void AddOSDItem(string text, int timer, int pos_x, int pos_y, RawColorBGRA color)
		{
			OSDItem newosd = new OSDItem();
			newosd.text = text;
			newosd.timer = timer;
			newosd.pos_x = pos_x;
			newosd.pos_y = pos_y;
			newosd.color = color;
			OSDItems.Add(newosd);
		}
	}
}
