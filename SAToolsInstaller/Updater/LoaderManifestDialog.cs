using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAToolsHub.Updater
{
	public class LoaderManifestDialog : ProgressDialog
	{
		private readonly string updatePath;
		private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

		public LoaderManifestDialog(string updatePath)
			: base("Update Progress", true)
		{
			this.updatePath = updatePath;

			Shown += OnShown;
			CancelEvent += OnCancelEvent;
		}

		private void OnCancelEvent(object sender, EventArgs eventArgs)
		{
			tokenSource.Cancel();
		}

		private void OnShown(object sender, EventArgs eventArgs)
		{
			DialogResult = DialogResult.OK;

			SetTaskCount(1);
			SetProgress(1);

			CancellationToken token = tokenSource.Token;
			string originalPath = Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).FullName).FullName;
			DialogResult result;
			do
			{
				result = DialogResult.Cancel;

				try
				{
					// poor man's await Task.Run (not available in .net 4.0)
					using (var task = new Task(() =>
					{
						string newManPath = Path.Combine(updatePath, "satools.manifest");
						string oldManPath = Path.Combine(originalPath, "satools.manifest");

						SetTaskAndStep("Parsing manifest...");
						if (token.IsCancellationRequested)
						{
							return;
						}

						List<ModManifestEntry> newManifest = ModManifest.FromFile(newManPath);

						SetTaskAndStep("Applying manifest...");
						if (token.IsCancellationRequested)
						{
							return;
						}

						if (File.Exists(oldManPath))
						{
							List<ModManifestEntry> oldManifest = ModManifest.FromFile(oldManPath);
							List<string> oldFiles = oldManifest.Except(newManifest)
								.Select(x => x.FilePath)
								.ToList();

							foreach (string file in oldFiles)
							{
								if (File.Exists(file))
								{
									File.Delete(file);
								}
							}

							RemoveEmptyDirectories(oldManifest, newManifest);
						}

						foreach (ModManifestEntry file in newManifest)
						{
							string dir = Path.GetDirectoryName(file.FilePath);
							if (!string.IsNullOrEmpty(dir))
							{
								string newDir = dir;
								if (!Directory.Exists(newDir))
								{
									Directory.CreateDirectory(newDir);
								}
							}

							string dest = file.FilePath;

							if (File.Exists(dest))
							{
								File.Delete(dest);
							}

							File.Copy(Path.Combine(updatePath, file.FilePath), dest);
						}

						File.Copy(newManPath, oldManPath, true);
						Process.Start(Path.Combine(originalPath, Path.GetFileName(Application.ExecutablePath)), $"cleanupdate \"{updatePath}\"");
					}, token))
					{
						task.Start();

						while (!task.IsCompleted && !task.IsCanceled)
						{
							Application.DoEvents();
						}

						task.Wait(token);
					}
				}
				catch (AggregateException ae)
				{
					ae.Handle(ex =>
					{
						result = MessageBox.Show(this, $"Failed to update:\r\n{ex.Message}",
							"Update Failed", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
						return true;
					});
				}
			} while (result == DialogResult.Retry);

			Close();
		}

		private void RemoveEmptyDirectories(IEnumerable<ModManifestEntry> oldManifest, IEnumerable<ModManifestEntry> newManifest)
		{
			foreach (string dir in ModManifest.GetOldDirectories(oldManifest, newManifest))
			{
				if (Directory.Exists(dir))
				{
					// Note that this is very intentionally not recursive. If there are
					// any files left over somehow, this SHOULD be considered an error,
					// as the goal is to exclusively remove empty directories.
					// - SF94
					Directory.Delete(dir);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			tokenSource.Dispose();
			base.Dispose(disposing);
		}
	}
}