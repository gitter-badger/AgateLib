﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AgateLib.ApplicationModels;
using AgateLib.Geometry;
using AgateLib.Platform.WinForms.ApplicationModels;
using AgateLib.Settings;

namespace AgateLib.Tests
{
	public class TestLauncherPresenter : ISettingsTracer
	{
		private frmLauncher frm;

		private bool runningTest = false;

		Dictionary<string, string> mSettings = new Dictionary<string, string>();
		string settingsFile;

		readonly char[] separator = new char[] { ' ' };

		public TestLauncherPresenter(frmLauncher frm)
		{
			this.frm = frm;

			frm.LaunchTest += Frm_LaunchTest;
			frm.FormClosed += HandleFormClosed;

			ReadSettingsNames();

			AgateLib.Settings.PersistantSettings.SettingsTracer = this;
			AgateLib.Settings.PersistantSettings.Debug = true;

			LoadTests();
		}

		private string[] CommandLineArguments
		{
			get
			{
				return frm.CommandLine.Split(separator,
					  StringSplitOptions.RemoveEmptyEntries);
			}
		}

		private void Frm_LaunchTest(object sender, TestEventArgs e)
		{
			LaunchTest(e.Info);
		}

		private void LaunchTest(TestInfo m)
		{
			IAgateTest obj = (IAgateTest)Activator.CreateInstance(m.Class);

			if (runningTest)
			{
				System.Diagnostics.Debug.Print("Bug in mono? A second test was launched while the first was still running.");
				return;
			}

			string[] args = { };

			frm.HideBeforeTest();
			foreach (var kvp in mSettings.ToArray())
			{
				if (kvp.Value == null)
					continue;

				string group, key;
				SplitName(kvp.Key, out group, out key);

				AgateLib.Core.Settings[group][key] = kvp.Value;
			}

			try
			{
				runningTest = true;
				LaunchTestModel(obj);
			}
			catch (TargetInvocationException e)
			{
				Exception ex_relevant = e.InnerException ?? e;
				string info = ex_relevant.Message;

				frm.TestExceptionMessage($"{ex_relevant.GetType().Name}\n{info}", $"AgateLib Test {m.Name} threw an exception.");

			}
			finally
			{
				runningTest = false;
				frm.ShowAfterTest();
			}
		}

		private void LaunchTestModel(IAgateTest test)
		{
			Core.State = new Configuration.State.AgateLibState();

			try
			{
				using (var setup = new AgateLib.Platform.WinForms.AgateSetupWinForms(CommandLineArguments))
				{
					setup.ApplicationName = $"{test.Category} :: {test.Name}";
					setup.AssetLocations.Path = "Assets";
					setup.AssetLocations.UserInterface = "UserInterface";
					setup.DesiredDisplayWindowResolution = new Size(800, 600);
					test.ModifySetup(setup);

					setup.InitializeAgateLib();

					test.Configuration = setup.Configuration;
					test.Run();
				}
			}
			catch (ExitGameException)
			{ }
		}

		private void SplitName(string p, out string group, out string key)
		{
			int period = p.LastIndexOf('.');

			if (period == -1)
				throw new DataException("Invalid key name");

			group = p.Substring(0, period);
			key = p.Substring(period + 1);
		}

		private void LoadTests()
		{
			TestCollection.AddTests(Assembly.GetAssembly(GetType()));

			frm.Tests = TestCollection.Tests;
		}

		void HandleFormClosed(object sender, FormClosedEventArgs e)
		{
			if (settingsFile == null)
			{
				System.Diagnostics.Debug.Print("No settings file to save to.");
				return;
			}

			using (StreamWriter w = new StreamWriter(settingsFile))
			{
				foreach (var setting in mSettings)
				{
					string text = setting.Key + "\t" + setting.Value;

					System.Diagnostics.Debug.Print(text);
					w.WriteLine(text);
				}
			}
		}

		#region --- ISettingsTracer implementation ---

		void AgateLib.Settings.ISettingsTracer.OnReadSetting(string groupName, string key, string value)
		{
			if (string.IsNullOrEmpty(groupName))
				throw new ArgumentException();

			string name = groupName + "." + key;

			StoreSetting(name, value);
		}
		void AgateLib.Settings.ISettingsTracer.OnWriteSetting(string groupName, string key, string value)
		{
			if (string.IsNullOrEmpty(groupName))
				throw new ArgumentException();

			string name = groupName + "." + key;

			StoreSetting(name, value);
		}

		void ReadSettingsNames()
		{
			//StreamReader r = null;
			//string targetDirectory = "../../../Tests/Assets/";
			//string filename = "settings_list.txt";

			//try
			//{
			//	settingsFile = System.IO.Path.GetFullPath(targetDirectory + filename);
			//	r = new StreamReader(targetDirectory + filename);
			//}
			//catch (DirectoryNotFoundException)
			//{
			//	settingsFile = filename;
			//	r = new StreamReader(filename);
			//}

			//using (r)
			//{
			//	while (r.EndOfStream == false)
			//	{
			//		string x = r.ReadLine().Trim();

			//		if (string.IsNullOrEmpty(x))
			//			continue;

			//		int index = x.IndexOf('\t');

			//		if (index == -1)
			//		{
			//			mSettings[x] = null;
			//		}
			//		else
			//			mSettings[x.Substring(0, index)] = x.Substring(index + 1);
			//	}
			//}
		}
		void StoreSetting(string name, string value)
		{
			if (mSettings.ContainsKey(name)) return;

			System.Diagnostics.Debug.Print("Storing setting " + name);

			mSettings.Add(name, value);
		}

		#endregion

	}
}
