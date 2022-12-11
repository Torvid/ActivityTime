﻿// I dedicate this work to the public domain.
// CC0, no right reserved, do as you will.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ActivityWatchButGood
{
    // Program overview

    // programs.csv
    //      text file containing all tracked programs.
    //      new programs are automatically added to this file.

    // "session" binary files, (1670681019.bin) and the like.
    //      Every time the program starts, a new session file is created with
    //      filename being the unix timestamp the program started.
    //      

    // TODO: test it under heavy load, like gigabytes of logged data. Make sure it's fast.

    public partial class Form1 : Form
    {
        // First some utlity functions!

        // appends bytes to a file
        public void AppendAllBytes(string path, byte[] bytes)
        {
            using (var stream = new FileStream(path, FileMode.Append))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        // Overwrites the last N bytes in a file
        public void WriteLastBytes(string path, byte[] bytes)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Write))
            {
                stream.Seek((int)(stream.Length - bytes.Length), SeekOrigin.Begin);
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        // Reads the last N bytes in a file.
        public void ReadLastBytes(string path, byte[] bytes)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                //if(stream.Length == 0)
                //{
                //    Crash("Stream length was 0 reading file. tell torvid.");
                //}
                stream.Seek((int)(stream.Length - bytes.Length), SeekOrigin.Begin);
                stream.Read(bytes, 0, bytes.Length);
            }
        }

        // hashes a string using sha256. probably overkill, we really just want to avoid collisions.
        public static ulong HashString(string text)
        {
            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                // hash the string
                byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text.ToLower()));

                // sha256 produces 32 bytes, but we want 8 bytes, so we xor them together.
                ulong result = BitConverter.ToUInt64(hashBytes, 0) ^
                               BitConverter.ToUInt64(hashBytes, 8) ^
                               BitConverter.ToUInt64(hashBytes, 16) ^
                               BitConverter.ToUInt64(hashBytes, 24);
                return result;
            }
        }

        // Display an error messsage to the user then close the program.
        public void Crash(string message)
        {
            MessageBox.Show(message, "Fatal Error");
            //Environment.Exit(0); // die immedietly
        }

        // Extracts the domain name part of a string.
        string CleanupURL(string result)
        {
            if (result == null)
                return "";

            if (result == "")
                return "";

            Uri uriResult;
            if (!Uri.TryCreate(result, UriKind.Absolute, out uriResult))
                result = "http://" + result;
            if (!Uri.TryCreate(result, UriKind.Absolute, out uriResult))
                return "";

            result = uriResult.Host;

            if (result.StartsWith("www."))
                return result.Replace("www.", "");

            return result;
        }

        // some win32 functions not exposed to .NET
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref RECT rectangle);

        public enum TimeView
        {
            Daily,
            Weekly,
            Monthly,
            Yearly,
        }

        public enum Productivity
        {
            VeryProductive,
            Productive,
            Neutral,
            Distracting,
            VeryDistracting,
        }

        public enum Category
        {
            Uncategorized,
            Business,
            CommunicationAndScheduling,
            DesignAndComposition,
            Art,
            Entertainment,
            FocusWork,
            Miscellaneous,
            NewsAndOpinion,
            OtherWork,
            Personal,
            ReferenceAndLearning,
            Shopping,
            SocialNetworking,
            SoftwareDevelopment,
            Utilities,
        }

        enum TimelineStepSize
        {
            hourly,
            daily,
            weekly,
        }

        public class Activity
        {
            // hash of activity name string
            public ulong hash;

            // exe name or website domain name, ie ("mspaint", "devenv", "google.com", "discord", "furaffinity.net", ...)
            public string name;

            // pretty name for the UI. ("Paint", "Microsoft Visual Studio", "Google", "Discord", "Furaffinity", ....)
            public string prettyName;

            // ("Neutral", "VeryProductive", "Neutral", "Distracting", "VeryDistracting")
            public Productivity productivity;

            // ("Utilities", "SoftwareDevelopment", "ReferenceAndLearning", "CommunicationAndScheduling", "Entertainment")
            public Category category;

            // value that's updated with the amount of seconds this activity was active depending on the current view (daily, weekly, monthly, etc)
            public TimeSpan totalSecondsActive;

            // serialize
            public override string ToString()
            {
                return "\n" + $@"{name}, {prettyName}, {productivity}, {category}";
            }

            // deserialize
            public Activity(string text)
            {
                string[] splitText = text.Split(',');

                this.name           = splitText[0].Trim();
                this.hash           = HashString(this.name);
                this.prettyName     = splitText[1].Trim();

                this.productivity   = Productivity.Neutral;
                Enum.TryParse<Productivity>(splitText[2].Trim(), out this.productivity);

                this.category       = Category.Uncategorized;
                Enum.TryParse<Category>(splitText[3].Trim(), out this.category);
            }
        }

        // Global variables
        string SessionFilePath;
        string ProgramsFilePath;
        string userDataPath;
        TimeView timeView = TimeView.Daily;
        TimelineStepSize timelineStepSize = TimelineStepSize.hourly;
        Dictionary<ulong, Activity> activities;
        ulong currentFocusedProgram = 1337;
        Activity[] top10Activities = new Activity[10];
        List<string> ActivityNamesForListbox;
        List<ulong> ActivityNamesForListboxIndexes;
        bool initialized = false;

        // constants
        Color[] ProductivityColors = { Color.FromArgb(0, 85, 196),
            Color.FromArgb(61, 128, 224),
            Color.FromArgb(177, 193, 191),
            Color.FromArgb(220, 104, 90),
            Color.FromArgb(214, 24, 0),
        };

        DateTimeOffset ViewStart;
        DateTimeOffset ViewEnd;
        DateTimeOffset Max(DateTimeOffset a, DateTimeOffset b)
        {
            return a > b ? a : b;
        }
        DateTimeOffset Min(DateTimeOffset a, DateTimeOffset b)
        {
            return a < b ? a : b;
        }

        // Find how much of these two time regions overlap.
        TimeSpan GetTimeOverlap(DateTimeOffset aStart, DateTimeOffset aEnd, DateTimeOffset bStart, DateTimeOffset bEnd)
        {
            TimeSpan result = (Min(aEnd, bEnd) - Max(aStart, bStart));
            if (result < TimeSpan.Zero)
                result = TimeSpan.Zero;
            return result;
        }

        // time spent focusing on something that's not a activity, like the desktop.
        TimeSpan outsideTime = TimeSpan.Zero;
        TimeSpan totalTime = TimeSpan.Zero;
        // fills "secondsActive" of all activities based on the input timerange
        void ReloadUI()
        {
            if (!initialized)
                return;

            foreach(Activity t in activities.Values)
            {
                t.totalSecondsActive = TimeSpan.Zero;
            }
            outsideTime = TimeSpan.Zero;
            totalTime = TimeSpan.Zero;

            int ProductivityCount = Enum.GetNames(typeof(Productivity)).Length;
            TimeSpan[,] TimeHours = new TimeSpan[ProductivityCount, 24];

            int CategoryCount = Enum.GetNames(typeof(Category)).Length;
            TimeSpan[] TimeCategory = new TimeSpan[CategoryCount];
            Category[] categories = new Category[CategoryCount];
            for (int i = 0; i < categories.Length; i++)
            {
                categories[i] = (Category)i;
            }

            string[] sessionFiles = Directory.GetFiles(userDataPath, "*.bin");
            foreach(string s in sessionFiles)
            {
                ulong unixStartTime = (ulong)int.Parse(Path.GetFileNameWithoutExtension(s));
                DateTimeOffset startTime = DateTimeOffset.FromUnixTimeSeconds((long)unixStartTime);

                byte[] data = File.ReadAllBytes(s);
                for (int i = 0; i < data.Length; i += 16)
                {
                    ulong hash  = BitConverter.ToUInt64(data, i);
                    ulong count = BitConverter.ToUInt64(data, i + 8);
                    DateTimeOffset FocusStart = startTime;
                    DateTimeOffset FocusEnd = startTime + TimeSpan.FromSeconds(count);

                    // find the part of FocusStart->FocusEnd that falls inside the range of ViewStart->ViewEnd
                    TimeSpan overlap = GetTimeOverlap(FocusStart, FocusEnd, ViewStart, ViewEnd);

                    startTime += TimeSpan.FromSeconds(count);
                    totalTime += overlap;

                    if (hash == 0)
                    {
                        outsideTime += overlap;
                    }
                    else
                    {
                        activities[hash].totalSecondsActive += overlap;

                        TimeCategory[(int)activities[hash].category] += overlap;

                        // get overlap with the individual days of the week
                        for (int j = 0; j < 24; j++)
                        {
                            DateTimeOffset HourStart = new DateTime(ViewStart.Year, ViewStart.Month, ViewStart.Day, j, 0, 0);
                            DateTimeOffset HourEnd;
                            if (j == 23)
                                HourEnd = new DateTime(ViewStart.Year, ViewStart.Month, ViewStart.Day + 1, 0, 0, 0);
                            else
                                HourEnd = new DateTime(ViewStart.Year, ViewStart.Month, ViewStart.Day, j + 1, 0, 0);
                            TimeSpan hourOverlap = GetTimeOverlap(FocusStart, FocusEnd, HourStart, HourEnd);
                            TimeHours[(int)activities[hash].productivity, j] += hourOverlap;
                        }
                    }
                }
            }

            if((int)Math.Floor(totalTime.TotalHours) == 0)
                timeLoggedLabel.Text = $@"{totalTime.Minutes}m";
            else
                timeLoggedLabel.Text = $@"{(int)Math.Floor(totalTime.TotalHours)}h {totalTime.Minutes}m";

            // Sort
            Array.Sort(TimeCategory, categories);
            Array.Sort(TimeCategory);
            Array.Reverse(TimeCategory);
            Array.Reverse(categories);
            TimeSpan Total = TimeSpan.Zero;
            Label[] categoryPctLabels = new Label[5] {
                categoryPct0Label,
                categoryPct1Label,
                categoryPct2Label,
                categoryPct3Label,
                categoryPct4Label };
            Label[] categoryNameLabels = new Label[5] {
                categoryName0Label,
                categoryName1Label,
                categoryName2Label,
                categoryName3Label,
                categoryName4Label };

            for (int i = 0; i < TimeCategory.Length; i++)
            {
                Total += TimeCategory[i];
            }
            for (int i = 0; i < 5; i++)
            {
                double percentage = TimeCategory[i].TotalSeconds / Total.TotalSeconds;
                categoryPctLabels[i].Text = Math.Round(percentage * 100) + "%";
                categoryNameLabels[i].Text = categories[i].ToString();
            }

            // get top N entires by category
            //for (int i = 0; i < 24; i++)
            //{
            //    for (int j = 0; j < 5; j++)
            //    {
            //        TimeHours[j, ]
            //    }
            //}

            // get top N entries in by application
            List<int> skipList = new List<int>();
            for (int i = 0; i < top10Activities.Length; i++)
            {
                top10Activities[i] = null;
                skipList.Add(-1);
            }
            for (int i = 0; i < top10Activities.Length; i++)
            {
                int largest = 0;
                int j = 0;
                foreach (Activity t in activities.Values)
                {
                    if (skipList.Contains(j))
                    {
                        j++;
                        continue;
                    }
                    if (t.totalSecondsActive.TotalSeconds > largest)
                    {
                        largest = (int)t.totalSecondsActive.TotalSeconds;
                        skipList[i] = j;
                        top10Activities[i] = t;
                        break;
                    }
                    j++;
                }
            }

            ClearGraphs();
            foreach (Activity activity in top10Activities)
            {
                if (activity == null || activity.totalSecondsActive == TimeSpan.Zero)
                    continue;
                DateTime dt = new DateTime(2012, 1, 1) + activity.totalSecondsActive;
                int i = histogramChart.Series["Entries"].Points.AddXY(activity.prettyName, dt);
                histogramChart.Series["Entries"].Points[i].Color = ProductivityColors[(int)activity.productivity];
            }

            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j < ProductivityCount; j++)
                {
                    string productivity = ((Productivity)j).ToString();
                    string hour = i.ToString();
                    string totalSeconds = ((int)TimeHours[j, i].TotalSeconds / 60).ToString();
                    if ((Productivity)j == Productivity.Distracting || (Productivity)j == Productivity.VeryDistracting)
                        totalSeconds = (-(int)TimeHours[j, i].TotalSeconds / 60).ToString();
                    timelineChart.Series[productivity].Points.AddXY(hour, totalSeconds);
                }
            }

            RefreshListBox();
        }

        void RefreshListBox()
        {
            ActivityNamesForListbox.Clear();
            ActivityNamesForListboxIndexes.Clear();
            foreach (KeyValuePair<ulong, Activity> entry in activities)
            {
                //if (entry.Value.totalSecondsActive < TimeSpan.FromSeconds(10))
                //    continue;

                ActivityNamesForListbox.Add(entry.Value.prettyName);
                ActivityNamesForListboxIndexes.Add(entry.Key);
            }

            activitiesListBox.DataSource = null;
            activitiesListBox.DataSource = ActivityNamesForListbox;
        }


        public Form1()
        {
            InitializeComponent();
            productivityComboBox.DataSource = Enum.GetValues(typeof(Productivity));
            categoryComboBox.DataSource = Enum.GetValues(typeof(Category));
            timeViewComboBox.DataSource = Enum.GetValues(typeof(TimeView));

            dateTimePicker1.Value = DateTime.Now;
        }
        void ClearGraphs()
        {
            timelineChart.Series["VeryDistracting"].Points.Clear();
            timelineChart.Series["Distracting"].Points.Clear();
            timelineChart.Series["Neutral"].Points.Clear();
            timelineChart.Series["Productive"].Points.Clear();
            timelineChart.Series["VeryProductive"].Points.Clear();
            histogramChart.Series["Entries"].Points.Clear();
        }

        // Program start
        private void Form1_Load(object sender, EventArgs e)
        {
            string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            userDataPath = $@"{appdataPath}\ActivityWatchButBetter";
            if (!Directory.Exists(userDataPath))
                Directory.CreateDirectory(userDataPath);

            // Create session file
            string startupTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            SessionFilePath = $@"{userDataPath}\{startupTimestamp}.bin";
            if(!File.Exists(SessionFilePath))
                File.Create(SessionFilePath).Close();

            // Load tracked programs
            ProgramsFilePath = $@"{userDataPath}\programs.csv";
            activities = new Dictionary<ulong, Activity>();
            ActivityNamesForListbox = new List<string>();
            ActivityNamesForListboxIndexes = new List<ulong>();
            if (File.Exists(ProgramsFilePath))
            {
                string programsString = File.ReadAllText(ProgramsFilePath);
                foreach (string s in programsString.Split('\n'))
                {
                    string[] splitText = s.Split(',');
                    if (splitText.Length != 4)
                        continue;
                    ulong hash = HashString(splitText[0].Trim());
                    if (activities.ContainsKey(hash))
                        continue;
                    activities.Add(hash, new Activity(s));
                }
            }
            else
            {
                // No programs file found, create one.
                // TODO: fill this out with a ton of good defaults!
                File.WriteAllText(ProgramsFilePath, "devenv, Microsoft Visual Studio, VeryProductive, SoftwareDevelopment");
            }

            initialized = true;
            // fill out the current thing
            ReloadUI();
        }

        private void WalkControlElements(AutomationElement rootElement, TreeNode treeNode)
        {
            // Conditions for the basic views of the subtree (content, control, and raw) 
            // are available as fields of TreeWalker, and one of these is used in the 
            // following code.
            AutomationElement elementNode = TreeWalker.ControlViewWalker.GetFirstChild(rootElement);

            while (elementNode != null)
            {
                TreeNode childTreeNode = treeNode.Nodes.Add(elementNode.Current.ControlType.LocalizedControlType);
                WalkControlElements(elementNode, childTreeNode);
                elementNode = TreeWalker.ControlViewWalker.GetNextSibling(elementNode);
            }
        }

        // Mapping of windows to the automation element that has the control.
        Dictionary<IntPtr, AutomationElement> BrowserMapping = new Dictionary<IntPtr, AutomationElement>();
        Dictionary<IntPtr, bool> BrowserMappingSeeking = new Dictionary<IntPtr, bool>();

        // This function gets the exe name of whatver window the user has selected.
        // Activities are either exe names, or domain names.
        string GetFocusedActivityName()
        {
            // get window handle
            IntPtr currentWindow = GetForegroundWindow();
            if (currentWindow == IntPtr.Zero)
                return "";

            uint processID = 0;
            uint threadID = GetWindowThreadProcessId(currentWindow, out processID);

            //uint pid = GetWindowThreadProcessId(currentWindow, IntPtr.Zero);
            Process proc = Process.GetProcessById((int)processID);

            string exeName = proc.ProcessName;

            // result is the name of the exe.
            string result = exeName;

            // Unless it's a web browser!
            // We use the windows "screen reader" UI-browsing features to find the "edit" navigation element for every browser.
            // This code is probably very prone to breaking as web browsers change over time, but is also not very hard to maintain!
            // It's just simple tree-search and it's meant to be human-browsable for the visually impared.
            bool isBrowser = false;
            int magicOffsetX = 0;
            int magicOffsetY = 0;
            if (exeName == "firefox")
            {
                magicOffsetX = 400;
                magicOffsetY = 50;
                isBrowser = true;
                //AutomationElement element = AutomationElement.FromHandle(currentWindow);
                //element = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Navigation"));
                //if (element != null)
                //{
                //    element = element.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.LocalizedControlTypeProperty, "edit"));
                //    if (element != null)
                //    {
                //        result = ((ValuePattern)element.GetCurrentPattern(ValuePattern.Pattern)).Current.Value;
                //        result = CleanupURL(result);
                //    }
                //}
            }
            else if (exeName == "chrome")
            {
                magicOffsetX = 400;
                magicOffsetY = 60;
                isBrowser = true;
                //AutomationElement element = AutomationElement.FromHandle(currentWindow);
                //element = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Google Chrome"));
                //element = element.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.LocalizedControlTypeProperty, "edit"));
                //if (element != null)
                //{
                //    result = ((ValuePattern)element.GetCurrentPattern(ValuePattern.Pattern)).Current.Value;
                //    result = CleanupURL(result);
                //}
            }
            else if (exeName == "brave")
            {
                magicOffsetX = 400;
                magicOffsetY = 57;
                isBrowser = true;
            }
            else if (exeName == "msedge")
            {
                magicOffsetX = 400;
                magicOffsetY = 60;
                isBrowser = true;
                //AutomationElement element = AutomationElement.FromHandle(currentWindow);
                //element = element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));
                //if (element != null)
                //{
                //    result = ((ValuePattern)element.GetCurrentPattern(ValuePattern.Pattern)).Current.Value;
                //    result = CleanupURL(result);
                //}
            }


            if(isBrowser)
            {
                if (!BrowserMappingSeeking.ContainsKey(currentWindow))
                {
                    BrowserMappingSeeking.Add(currentWindow, false);

                    RECT rect = new RECT();
                    GetWindowRect(currentWindow, ref rect);
                    System.Windows.Point testPoint = new System.Windows.Point(0, 0);
                    testPoint.X += rect.Left + magicOffsetX;
                    testPoint.Y += rect.Top + magicOffsetY;

                    AutomationElement element = AutomationElement.FromPoint(testPoint);

                    if (element != null && element.Current.LocalizedControlType == "edit")
                    {
                        BrowserMapping.Add(currentWindow, element);
                    }
                    else
                    {
                        BrowserMappingSeeking.Remove(currentWindow);
                    }
                }
                if (BrowserMapping.ContainsKey(currentWindow))
                {
                    AutomationElement element = BrowserMapping[currentWindow];
                    //if (element != null && element.Current.LocalizedControlType == "edit")
                    {
                        result = ((ValuePattern)element.GetCurrentPattern(ValuePattern.Pattern)).Current.Value;
                        result = CleanupURL(result);
                    }
                }
            }
            return result;
        }

        private void tick_Tick(object sender, EventArgs e)
        {
            string focusedActivityName = GetFocusedActivityName();
            ulong hash = 0;
            if (focusedActivityName != "")
            {
                //Console.WriteLine(focusedActivityName);
                hash = HashString(focusedActivityName);

                // check if this name exists in the program list.
                Activity activity = null;
                if (activities.ContainsKey(hash))
                {
                    activity = activities[hash];
                }
                else
                {
                    // If the activity is not in the list, we create it and add it to the list.
                    activity = new Activity($@"{focusedActivityName}, {focusedActivityName}, Neutral, Uncategorized");
                    activities.Add(hash, activity);
                    File.AppendAllText(ProgramsFilePath, activity.ToString());
                }
            }

            // If the activity changed, append two bytes
            // first is the hash of the new activity
            // second is 0, to be incremetned every second that activity is still focused
            // a hash of 0 indicates no program, like the windows desktop.
            if (currentFocusedProgram != hash)
            {
                currentFocusedProgram = hash;
                AppendAllBytes(SessionFilePath, BitConverter.GetBytes(hash));
                AppendAllBytes(SessionFilePath, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            }

            // add 1 to the last number, as one second has passed.
            byte[] data = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            ReadLastBytes(SessionFilePath, data);
            ulong count = BitConverter.ToUInt64(data, 0);
            count++;
            //Console.WriteLine(count);
            data = BitConverter.GetBytes(count);
            WriteLastBytes(SessionFilePath, data);
        }

        private void timelineChart_Click(object sender, EventArgs e)
        {

        }


        private void histogramChart_MouseClick(object sender, MouseEventArgs e)
        {
            var r = histogramChart.HitTest(e.X, e.Y);

            if (r.ChartElementType == ChartElementType.DataPoint)
            {
                DataPoint p = (DataPoint)r.Object;
                int index = r.PointIndex;
                //Console.WriteLine(top10[index]?.prettyName);

                for (int i = 0; i < ActivityNamesForListboxIndexes.Count; i++)
                {
                    if(ActivityNamesForListboxIndexes[i] == top10Activities[index].hash)
                    {
                        activitiesListBox.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void histogramChart_MouseMove(object sender, MouseEventArgs e)
        {
            var r = histogramChart.HitTest(e.X, e.Y);

            if (r.ChartElementType == ChartElementType.DataPoint)
            {
                histogramChart.Cursor = Cursors.Hand;
            }
            else
            {
                histogramChart.Cursor = Cursors.Default;
            }
        }

        private void histogramChart_Click(object sender, EventArgs e)
        {

        }

        private void activitiesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (activitiesListBox.SelectedIndex == -1)
                return;
            Activity selectedActivity = activities[ActivityNamesForListboxIndexes[activitiesListBox.SelectedIndex]];
            nameTextBox.Text = selectedActivity.prettyName;
            productivityComboBox.SelectedIndex = (int)selectedActivity.productivity;
            categoryComboBox.SelectedIndex = (int)selectedActivity.category;
            activityNameLabel.Text = selectedActivity.name;
            countLabel.Text = selectedActivity.totalSecondsActive.ToString();
            ReloadUI();
        }

        // any of these changed, apply it to the current activity and resave
        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (activitiesListBox.SelectedIndex == -1)
                return;
            if (!((TextBox)sender).Modified)
                return;
            Activity selectedActivity = activities[ActivityNamesForListboxIndexes[activitiesListBox.SelectedIndex]];
            selectedActivity.prettyName = nameTextBox.Text.Replace(",", "").Replace("\n", "");
            ResaveCSV();
            ReloadUI();
        }
        private void productivenessComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (activitiesListBox.SelectedIndex == -1)
                return;
            Activity selectedActivity = activities[ActivityNamesForListboxIndexes[activitiesListBox.SelectedIndex]];
            selectedActivity.productivity = (Productivity)productivityComboBox.SelectedIndex;
            ResaveCSV();
            ReloadUI();
        }
        private void categoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (activitiesListBox.SelectedIndex == -1)
                return;
            Activity selectedActivity = activities[ActivityNamesForListboxIndexes[activitiesListBox.SelectedIndex]];
            selectedActivity.category = (Category)categoryComboBox.SelectedIndex;
            ResaveCSV();
            ReloadUI();
        }

        void ResaveCSV()
        {
            string result = "";
            foreach(Activity activity in activities.Values)
            {
                result += activity.ToString();
            }
            File.WriteAllText(ProgramsFilePath, result);
        }

        private void timeViewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //TODO: make the datetime picker actually just show the month for moth mode, only show year in year mode, etc.

            dateTimePicker1.ShowUpDown = false;
            timeView = (TimeView)timeViewComboBox.SelectedIndex;
            switch (timeView)
            {
                case TimeView.Daily:
                    chooseDayLabel.Text = "Choose Day";
                    dateTimePicker1.Format = DateTimePickerFormat.Short;
                    timeByHourLabel.Text = "Focus by hour";
                    break;
                case TimeView.Weekly:
                    chooseDayLabel.Text = "Choose Week";
                    dateTimePicker1.Format = DateTimePickerFormat.Short;
                    timeByHourLabel.Text = "Focus by day";
                    break;
                case TimeView.Monthly:
                    chooseDayLabel.Text = "Choose Month";
                    dateTimePicker1.Format = DateTimePickerFormat.Custom;
                    dateTimePicker1.CustomFormat = "yyyy-MM";
                    timeByHourLabel.Text = "Focus by day";
                    break;
                case TimeView.Yearly:
                    chooseDayLabel.Text = "Choose Year";
                    dateTimePicker1.Format = DateTimePickerFormat.Custom;
                    dateTimePicker1.CustomFormat = "yyyy";
                    dateTimePicker1.ShowUpDown = true;
                    timeByHourLabel.Text = "Focus by day";
                    break;
                default:
                    break;
            }
            ReloadUI();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            switch (timeView)
            {
                case TimeView.Daily:
                    timelineStepSize = TimelineStepSize.hourly;
                    ViewStart = dateTimePicker1.Value;
                    ViewEnd = dateTimePicker1.Value;
                    ViewStart = new DateTime(ViewStart.Year, ViewStart.Month, ViewStart.Day, 0, 0, 0);
                    ViewEnd = new DateTime(ViewEnd.Year, ViewEnd.Month, ViewEnd.Day+1, 0, 0, 0);
                    break;
                case TimeView.Weekly:
                    timelineStepSize = TimelineStepSize.daily;
                    int daysInWeek = 7;
                    ViewStart = dateTimePicker1.Value + TimeSpan.FromDays(daysInWeek - (int)dateTimePicker1.Value.DayOfWeek);
                    ViewEnd = dateTimePicker1.Value - TimeSpan.FromDays((int)dateTimePicker1.Value.DayOfWeek);
                    ViewStart = new DateTime(ViewStart.Year, ViewStart.Month, ViewStart.Day, 0, 0, 0);
                    ViewEnd = new DateTime(ViewEnd.Year, ViewEnd.Month, ViewEnd.Day, 0, 0, 0);
                    break;
                case TimeView.Monthly:
                    timelineStepSize = TimelineStepSize.daily;
                    ViewStart = dateTimePicker1.Value;
                    ViewEnd = dateTimePicker1.Value;
                    ViewStart = new DateTime(ViewStart.Year, ViewStart.Month, 0, 0, 0, 0);
                    ViewEnd = new DateTime(ViewEnd.Year, ViewEnd.Month+1, 0, 0, 0, 0);
                    break;
                case TimeView.Yearly:
                    timelineStepSize = TimelineStepSize.weekly;
                    ViewStart = dateTimePicker1.Value;
                    ViewEnd = dateTimePicker1.Value;
                    ViewStart = new DateTime(ViewStart.Year, 0, 0, 0, 0, 0);
                    ViewEnd = new DateTime(ViewEnd.Year+1, 0, 0, 0, 0, 0);
                    break;
                default:
                    break;
            }
            ReloadUI();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ReloadUI();
        }

        private void productivityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
