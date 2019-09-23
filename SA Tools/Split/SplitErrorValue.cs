using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA_Tools.Split
{
	public enum SplitERRORVALUE
	{
		Success = 0,
		NoProject = -1,
		InvalidProject = -2,
		NoSourceFile = -3,
		NoDataMapping = -4,
		InvalidDataMapping = -5,
		UnhandledException = -6,
		InvalidConfig = -7
	}
}
