using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SAModel.SAEditorCommon.Import
{
	public static class NodeTable
	{
		/// <summary>
		/// Imports a nodetable (a single-level instance heirarchy layout) file.
		/// </summary>
		/// <param name="filePath">full path to file (with extension) to import data from.</param>
		/// <param name="errorFlag">Set to TRUE if an error occured.</param>
		/// <param name="errorMsg">Suggested error message to show to the user.</param>
		public static void ImportFromFile(string filePath, out bool errorFlag, out string errorMsg, EditorItemSelection selectionManager)
		{
			if (!File.Exists(filePath))
			{
				errorFlag = true;
				errorMsg = "File not found!";
				return;
			}

			Stream nodeTable = File.OpenRead(filePath);
			StreamReader nodeTableStream = new StreamReader(nodeTable);
			string folderPath = Path.GetDirectoryName(filePath);

			string version = nodeTableStream.ReadLine();

			string[] versionSplit = version.Split(' ');

			#region Version Validity Check
			if (versionSplit.Length <= 1 || versionSplit[0] != "ver") // invalid file
			{
				errorFlag = true;
				errorMsg = "Invalid file! (Version Check Failed)";
				return;
			}

			// versionNumber is the last string in the split sequence, with the ; character as the delimiter
			string[] versionNumberSplit = versionSplit[1].Split(';');
			string versionNumber = versionNumberSplit[0];

			if ((versionNumber != "1") && (versionNumber != "1.5") && (versionNumber != "1.6"))
			{
				errorFlag = true;
				errorMsg = "Invalid Nodetable version number was supplied";
				nodeTable.Close();
				return;
			}
			#endregion

			// get node count from next line
			string[] nodeCountLines = nodeTableStream.ReadLine().Split(' ');
			if ((nodeCountLines[0] != "node") || (nodeCountLines[1] != "count"))
			{
				errorFlag = true;
				errorMsg = "Error in node count!";
				nodeTable.Close();
				return;
			}

			nodeCountLines = nodeCountLines[2].Split(';');

			if (!Int32.TryParse(nodeCountLines[0], NumberStyles.Any, new CultureInfo("en-US"), out int nodeCount))
			{
				errorFlag = true;
				errorMsg = "Error parsing node count!";
				nodeTable.Close();
				return;
			}

			nodeTableStream.ReadLine(); // aligning

			List<KeyValuePair<int, Attach>> instanceMgr = new List<KeyValuePair<int, Attach>>();

			if (versionNumber == "1.5")
			{
				for (int n = 0; n < nodeCount; n++)
				{
					string nodeInput = nodeTableStream.ReadLine();
					string[] nodeDescriptorSplit = nodeInput.Split(' ');
					string[] nodeIndexSplit = nodeDescriptorSplit[1].Split(';');


					if (!Int32.TryParse(nodeIndexSplit[0], NumberStyles.Any, new CultureInfo("en-US"), out int nodeIndex))
					{
						errorFlag = true;
						errorMsg = String.Format("Error parsing node label for node {0}.", n);
						nodeTableStream.Close();
						return;
					}

					#region Position Read/Parse
					Vertex position;
					string[] positionSplit = nodeTableStream.ReadLine().Split(' ');

					if (positionSplit[0] != "pos")
					{
						errorFlag = true;
						errorMsg = String.Format("Error retrieving position values for node {0}", n);
						nodeTableStream.Close();
						return;
					}
					positionSplit[3] = positionSplit[3].Split(';')[0];

					if ((float.TryParse(positionSplit[1], NumberStyles.Any, new CultureInfo("en-US"), out float xPos)) && (float.TryParse(positionSplit[2], NumberStyles.Any, new CultureInfo("en-US"), out float yPos)) && (float.TryParse(positionSplit[3], NumberStyles.Any, new CultureInfo("en-US"), out float zPos)))
					{
						position = new Vertex(xPos, yPos, zPos);
					}
					else
					{
						errorFlag = true;
						errorMsg = String.Format("Error parsing position values for node {0}", n);
						nodeTableStream.Close();
						return;
					}
					#endregion

					#region Rotation Read/Parse
					Rotation rotation;
					string[] rotationSplit = nodeTableStream.ReadLine().Split(' ');

					if (rotationSplit[0] != "rot")
					{
						errorFlag = true;
						errorMsg = String.Format("Error retrieving rotation values for node {0}", n);
						nodeTableStream.Close();
						return;
					}
					rotationSplit[3] = rotationSplit[3].Split(';')[0];

					if (float.TryParse(rotationSplit[1], NumberStyles.Any, new CultureInfo("en-US"), out float xRot) && float.TryParse(rotationSplit[2], NumberStyles.Any, new CultureInfo("en-US"), out float yRot) && float.TryParse(rotationSplit[3], NumberStyles.Any, new CultureInfo("en-US"), out float zRot))
					{

						rotation = new Rotation(Rotation.DegToBAMS(xRot), Rotation.DegToBAMS(yRot), Rotation.DegToBAMS(zRot));
					}
					else
					{
						errorFlag = true;
						errorMsg = String.Format("Error parsing rotation values for node {0}", n);
						nodeTableStream.Close();
						return;
					}
					#endregion

					#region Creating LevelItem
					string modelFilePath = String.Concat(folderPath, "/", nodeIndex, ".obj");
					if (!File.Exists(modelFilePath))
					{
						errorFlag = true;
						errorMsg = String.Format("File not found: {0}", modelFilePath);
						nodeTableStream.Close();
						return;
					}

					if (nodeDescriptorSplit[0] == "node")
					{
						LevelItem levelItem = new LevelItem(modelFilePath, position, rotation, LevelData.LevelItemCount, selectionManager);
						instanceMgr.Add(new KeyValuePair<int, Attach>(nodeIndex, levelItem.CollisionData.Model.Attach));
					}
					else if (nodeDescriptorSplit[0] == "instance")
					{
						Attach instanceBaseAttach = instanceMgr.Find(item => item.Key == nodeIndex).Value;
						LevelItem levelItem = new LevelItem(instanceBaseAttach, position, rotation, LevelData.LevelItemCount, selectionManager);
					}
					#endregion

					nodeTableStream.ReadLine(); // aligning
				}
			}
			else if (versionNumber == "1")
			{
				// version 1 does not support instances. Just read and construct.
				throw new NotImplementedException();
			}
			else if (versionNumber == "1.6")
			{
				for (int n = 0; n < nodeCount; n++)
				{
					string nodeInput = nodeTableStream.ReadLine();
					string[] nodeDescriptorSplit = nodeInput.Split(' ');
					string[] nodeIndexSplit = nodeDescriptorSplit[1].Split(';');


					if (!Int32.TryParse(nodeIndexSplit[0], NumberStyles.Any, new CultureInfo("en-US"), out int nodeIndex))
					{
						errorFlag = true;
						errorMsg = String.Format("Error parsing node label for node {0}.", n);
						nodeTableStream.Close();
						return;
					}

					#region Position Read/Parse
					Vertex position;
					string[] positionSplit = nodeTableStream.ReadLine().Split(' ');

					if (positionSplit[0] != "pos")
					{
						errorFlag = true;
						errorMsg = String.Format("Error retrieving position values for node {0}", n);
						nodeTableStream.Close();
						return;
					}
					positionSplit[3] = positionSplit[3].Split(';')[0];
					if ((float.TryParse(positionSplit[1], NumberStyles.Any, new CultureInfo("en-US"), out float xPos)) && (float.TryParse(positionSplit[2], NumberStyles.Any, new CultureInfo("en-US"), out float yPos)) && (float.TryParse(positionSplit[3], NumberStyles.Any, new CultureInfo("en-US"), out float zPos)))
					{
						position = new Vertex(xPos, yPos, zPos);
					}
					else
					{
						errorFlag = true;
						errorMsg = String.Format("Error parsing position values for node {0}", n);
						nodeTableStream.Close();
						return;
					}
					#endregion

					#region Rotation Read/Parse
					Rotation rotation;
					string[] rotationSplit = nodeTableStream.ReadLine().Split(' ');

					if (rotationSplit[0] != "rot")
					{
						errorFlag = true;
						errorMsg = String.Format("Error retrieving rotation values for node {0}", n);
						nodeTableStream.Close();
						return;
					}
					rotationSplit[3] = rotationSplit[3].Split(';')[0];

					if (float.TryParse(rotationSplit[1], NumberStyles.Any, new CultureInfo("en-US"), out float xRot) && float.TryParse(rotationSplit[2], NumberStyles.Any, new CultureInfo("en-US"), out float yRot) && float.TryParse(rotationSplit[3], NumberStyles.Any, new CultureInfo("en-US"), out float zRot))
					{

						rotation = new Rotation(Rotation.DegToBAMS(xRot), Rotation.DegToBAMS(yRot), Rotation.DegToBAMS(zRot));
					}
					else
					{
						errorFlag = true;
						errorMsg = String.Format("Error parsing rotation values for node {0}", n);
						nodeTableStream.Close();
						return;
					}
					#endregion

					#region SurfaceFlags Read/Parse
					string surfaceFlags = "";
					string surfaceFlagsLine = nodeTableStream.ReadLine();
					string[] surfaceFlagsSplit = surfaceFlagsLine.Split(' ');

					if (surfaceFlagsSplit[0] == "surfaceflags")
					{
						surfaceFlags = surfaceFlagsSplit[1].Split(';')[0];
						surfaceFlagsSplit = surfaceFlags.Split('X');
						surfaceFlags = surfaceFlagsSplit[1];
					}
					#endregion

					#region Creating LevelItem
					string modelFilePath = String.Concat(folderPath, "/", nodeIndex, ".obj");
					if (!File.Exists(modelFilePath))
					{
						errorFlag = true;
						errorMsg = String.Format("File not found: {0}", modelFilePath);
						nodeTableStream.Close();
						return;
					}

					if (nodeDescriptorSplit[0] == "node")
					{
						LevelItem levelItem = new LevelItem(modelFilePath, position, rotation, LevelData.LevelItemCount, selectionManager, true)
						{
							Flags = surfaceFlags
						};
						instanceMgr.Add(new KeyValuePair<int, Attach>(nodeIndex, levelItem.CollisionData.Model.Attach));
					}
					else if (nodeDescriptorSplit[0] == "instance")
					{
						Attach instanceBaseAttach = instanceMgr.Find(item => item.Key == nodeIndex).Value;
						LevelItem levelItem = new LevelItem(instanceBaseAttach, position, rotation, LevelData.LevelItemCount, selectionManager)
						{
							Flags = surfaceFlags
						};
					}
					#endregion

					nodeTableStream.ReadLine(); // aligning
				}
			}

			nodeTable.Close();

			errorFlag = false;
			errorMsg = "Import successful!";
		}

		/// <summary>
		/// Exports a nodetable file (a single-level instance heirarchy layout) file.
		/// </summary>
		/// <param name="filePath">Full file path to export to</param>
		/// <param name="errorFlag">Set to TRUE if an error occured.</param>
		/// <param name="errorMsg">Suggested error message to show the user.</param>
		public static void ExportToFile(string filePath, out bool errorFlag, out string errorMsg)
		{
			try
			{
				string outputFolderPath = Path.GetDirectoryName(filePath);
				File.CreateText(filePath);

				//foreach
			}
			catch (Exception e)
			{
				errorFlag = true;
				errorMsg = e.Message;
				return;
			}

			errorFlag = false;
			errorMsg = "";
		}
	}
}
