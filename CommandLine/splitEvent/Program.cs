﻿using SplitTools.SAArc;
using System;

namespace splitEvent
{
	static class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.Write("Filename: ");
				args = new string[] { Console.ReadLine().Trim('"') };
			}
			foreach (string filename in args)
			{
				sa2Event.Split(filename);
			}
		}
	}
}