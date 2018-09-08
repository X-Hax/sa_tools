using System;
using System.Collections.Generic;

namespace ProjectManager
{
	class ProjectSettings
	{
		public List<String> OtherModsToRun { get; set; }
		public string PostBuildScript { get; set; }

		public ProjectSettings()
		{
			OtherModsToRun = new List<string>();
			PostBuildScript = "";
		}
	}
}
