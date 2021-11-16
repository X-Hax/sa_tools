using System;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SAModel.DataToolbox
{
	public class TextBoxWriter : TextWriter
	{
		private TextBox MyControl;
		private StringBuilder buffer;

		public TextBoxWriter(TextBox control)
		{
			MyControl = control;
			buffer = new StringBuilder();
		}

		public void WriteOut()
		{
			if (ControlInvokeRequired(MyControl, () => WriteOut())) return;
			if (buffer.Length > 0)
			{
				MyControl.AppendText(buffer.ToString());
				buffer.Clear();
			}
		}

		public bool ControlInvokeRequired(Control c, Action a)
		{
			if (c.InvokeRequired) c.Invoke(new MethodInvoker(delegate { a(); }));
			else return false;

			return true;
		}

		public override void Write(string s)
		{
			if (ControlInvokeRequired(MyControl, () => Write(s))) return;
			buffer.Append(s);
		}

		public override void Write(char[] bufferx, int index, int count)
		{
			if (ControlInvokeRequired(MyControl, () => Write(bufferx, index, count))) return;
			char[] result = new char[count];
			Array.Copy(bufferx, index, result, 0, count);
			buffer.Append(result);
		}

		public override void Write(char c)
		{
			if (ControlInvokeRequired(MyControl, () => Write(c))) return;
			buffer.Append(c);
		}

		public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }

	}
}
