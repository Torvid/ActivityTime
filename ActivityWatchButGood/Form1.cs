// I dedicate this work to the public domain.
// CC0, no right reserved, do as you will.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
//using System.Windows.Automation;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;
using UIAutomationClient;
using System.Windows.Automation;
using TreeScope = UIAutomationClient.TreeScope;
//using UIA;
//using UIAutomationBlockingCoreLib;

// Overview

// No style points here, just features.

// All program data aside from the exe is stored in %appdata%/ActivityTime/

// programs.csv
//      text file containing all tracked programs.
//      new programs are automatically added to this file.

// "session" binary files, (1670681019.bin) and the like.
//      Every time the program starts, a new session file is created with
//      filename being the unix timestamp the program started.
//      the file is a sequence of alternating uint64 like so:
//      [hash, time, hash, time, hash, time, ...]
//      the hash is the program ID.
//      the time is how long that program was focused.

// use VisualUIAVerifyNative.exe to navigate windows UI elements to find the edit bar

// TODO: test it under heavy load, like gigabytes of logged data. Make sure it's fast.
// TODO: look into how stable the "timer" is. Maybe it drifts over time.
// TODO: make the dog say snarky things about what programs you are using? xD

// TODO: bug, make it so that when the day rolls over, the date automatically changes

// TODO: make the datetime picker actually just show the month for moth mode, only show year in year mode, etc.

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

    // Extracts the domain name part of a string.
    string CleanupURL(string result)
    {
        if (result == null)
            return "";

        if (result == "")
            return "";

        if (result.Contains(" "))
            return "";

        if (!result.Contains("."))
            return "";

        Uri uriResult;
        if (!Uri.TryCreate(result, UriKind.Absolute, out uriResult))
            result = "http://" + result;
        if (!Uri.TryCreate(result, UriKind.Absolute, out uriResult))
            return "";

        if (uriResult.Scheme == "file")
            return "web browser file";

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

    [DllImport("psapi.dll")]
    static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, StringBuilder lpBaseName, int nSize);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
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
        ArtDesignAndComposition,
        Entertainment,
        Work,
        NewsAndOpinion,
        Personal,
        ReferenceAndLearning,
        Shopping,
        SocialMedia,
        SoftwareDevelopment,
        System,
        Miscellaneous,
        Utilities,
    }

    // How productive a category is
    public Productivity[] CategoryProductivity = new Productivity[]
    {
        Productivity.Neutral,
        Productivity.VeryProductive,
        Productivity.Productive,
        Productivity.VeryProductive,
        Productivity.VeryDistracting,
        Productivity.VeryProductive,
        Productivity.VeryDistracting,
        Productivity.Neutral,
        Productivity.Productive,
        Productivity.VeryDistracting,
        Productivity.VeryDistracting,
        Productivity.VeryProductive,
        Productivity.Neutral,
        Productivity.Neutral,
        Productivity.Neutral,
    };

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

            this.name = splitText[0].Trim();
            this.hash = HashString(this.name);
            this.prettyName = splitText[1].Trim();

            this.productivity = Productivity.Neutral;
            Enum.TryParse<Productivity>(splitText[2].Trim(), out this.productivity);

            this.category = Category.Uncategorized;
            Enum.TryParse<Category>(splitText[3].Trim(), out this.category);
        }
    }

    // Global variables
    string SessionFilePath;
    string ProgramsFilePath;
    string userDataPath;
    TimeView timeView = TimeView.Daily;
    int timelineSubsteps = 24;
    Dictionary<ulong, Activity> activities;
    ulong currentFocusedProgram = 1337;
    Activity[] top10Activities = new Activity[10];
    List<string> ActivityNamesForListbox;
    List<ulong> ActivityNamesForListboxIndexes;
    bool initialized = false;

    // constants
    Color[] ProductivityColors = {
        Color.FromArgb(0, 85, 196),
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

    List<Productivity> productivityTimelinePoints = new List<Productivity>();
    // time spent focusing on something that's not a activity, like the desktop.
    TimeSpan outsideTime = TimeSpan.Zero;
    Category[] categories;
    TimeSpan totalTime = TimeSpan.Zero;

    List<(ulong, DateTimeOffset, DateTimeOffset)> rawData = new List<(ulong, DateTimeOffset, DateTimeOffset)>();

    Productivity filteredProductivity;
    Category filteredCategory;
    FilterType filterType;

    enum FilterType
    {
        None,
        Category,
        Productivity,
    }

    // fills "secondsActive" of all activities based on the input timerange
    void ReloadUI()
    {
        if (!initialized)
            return;

        if (filterType == FilterType.None)
            filterLabel.Text = "Filter: None";
        else if (filterType == FilterType.Category)
            filterLabel.Text = "Filter: " + filteredCategory.ToString();
        else if (filterType == FilterType.Productivity)
            filterLabel.Text = "Filter: " + filteredProductivity.ToString();

        foreach (Activity t in activities.Values)
        {
            t.totalSecondsActive = TimeSpan.Zero;
        }
        outsideTime = TimeSpan.Zero;
        totalTime = TimeSpan.Zero;

        int ProductivityCount = Enum.GetNames(typeof(Productivity)).Length;
        TimeSpan[,] TimeHours = new TimeSpan[ProductivityCount, timelineSubsteps];

        int CategoryCount = Enum.GetNames(typeof(Category)).Length;
        TimeSpan[] TimeCategory = new TimeSpan[CategoryCount];
        categories = new Category[CategoryCount];
        for (int i = 0; i < categories.Length; i++)
        {
            categories[i] = (Category)i;
        }

        rawData.Clear();
        string[] sessionFiles = Directory.GetFiles(userDataPath, "*.bin");
        foreach (string s in sessionFiles)
        {
            ulong unixStartTime = (ulong)int.Parse(Path.GetFileNameWithoutExtension(s));
            DateTimeOffset startTime = DateTimeOffset.FromUnixTimeSeconds((long)unixStartTime);
            byte[] data = File.ReadAllBytes(s);
            for (int i = 0; i < data.Length; i += 16)
            {
                ulong hash = BitConverter.ToUInt64(data, i);
                ulong count = BitConverter.ToUInt64(data, i + 8);
                DateTimeOffset FocusStart = startTime;
                startTime += TimeSpan.FromSeconds(count);
                DateTimeOffset FocusEnd = startTime;
                rawData.Add((hash, FocusStart, FocusEnd));
            }
        }


        foreach (var entry in rawData)
        {
            ulong hash = entry.Item1;
            DateTimeOffset FocusStart = entry.Item2;
            DateTimeOffset FocusEnd = entry.Item3;
            TimeSpan overlap = GetTimeOverlap(FocusStart, FocusEnd, ViewStart, ViewEnd);
            if (overlap == TimeSpan.Zero)
                continue;

            totalTime += overlap;

            if (hash == 0)
            {
                outsideTime += overlap;
            }
            else
            {
                activities[hash].totalSecondsActive += overlap;

                TimeCategory[(int)activities[hash].category] += overlap;

                TimeSpan viewTimeSpan = ViewEnd - ViewStart;
                viewTimeSpan = new TimeSpan(viewTimeSpan.Ticks / timelineSubsteps);

                DateTimeOffset HourCurrent = ViewStart;
                // get overlap with the individual days of the week
                for (int j = 0; j < timelineSubsteps; j++)
                {
                    DateTimeOffset HourStart = HourCurrent;
                    HourCurrent += viewTimeSpan;
                    DateTimeOffset HourEnd = HourCurrent;

                    TimeSpan hourOverlap = GetTimeOverlap(FocusStart, FocusEnd, HourStart, HourEnd);
                    TimeHours[(int)activities[hash].productivity, j] += hourOverlap;
                    HourStart = HourEnd;
                }
            }
        }

        if ((int)Math.Floor(totalTime.TotalHours) == 0)
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
        Panel[] categoryProgressPanels = new Panel[5] {
            categoryProgress0Panel,
            categoryProgress1Panel,
            categoryProgress2Panel,
            categoryProgress3Panel,
            categoryProgress4Panel };

        for (int i = 0; i < TimeCategory.Length; i++)
        {
            Total += TimeCategory[i];
        }
        for (int i = 0; i < 5; i++)
        {
            double percentage = TimeCategory[i].TotalSeconds / Total.TotalSeconds;
            categoryPctLabels[i].Text = Math.Round(percentage * 100) + "%";
            categoryNameLabels[i].Text = categories[i].ToString();
            categoryProgressPanels[i].Width = (int)(237.0 * percentage);
            categoryProgressPanels[i].BackColor = ProductivityColors[(int)CategoryProductivity[(int)categories[i]]];
        }

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
                if (t.totalSecondsActive.TotalSeconds > largest && t.totalSecondsActive > TimeSpan.FromSeconds(10))
                {
                    largest = (int)t.totalSecondsActive.TotalSeconds;
                    skipList[i] = j;
                    top10Activities[i] = t;
                    break;
                }
                j++;
            }
        }

        // Clear the graphs
        timelineChart.Series["VeryDistracting"].Points.Clear();
        timelineChart.Series["Distracting"].Points.Clear();
        timelineChart.Series["Neutral"].Points.Clear();
        timelineChart.Series["Productive"].Points.Clear();
        timelineChart.Series["VeryProductive"].Points.Clear();
        histogramChart.Series["Entries"].Points.Clear();

        foreach (Activity activity in top10Activities)
        {
            if (activity == null)
                continue;
            DateTime dt = new DateTime(2012, 1, 1) + activity.totalSecondsActive;
            int i = histogramChart.Series["Entries"].Points.AddXY(activity.prettyName, dt);
            histogramChart.Series["Entries"].Points[i].Color = ProductivityColors[(int)activity.productivity];
        }
        productivityTimelinePoints.Clear();
        
        for (int i = 0; i < timelineSubsteps; i++)
        {
            for (int j = 0; j < ProductivityCount; j++)
            {
                productivityTimelinePoints.Add((Productivity)(ProductivityCount - j - 1));
                string productivity = ((Productivity)j).ToString();
                string hour = (i + 1).ToString();
                int totalSeconds = ((int)TimeHours[j, i].TotalSeconds);
                if ((Productivity)j == Productivity.Distracting || (Productivity)j == Productivity.VeryDistracting)
                    totalSeconds = (-(int)TimeHours[j, i].TotalSeconds);

                TimeSpan dt = TimeSpan.FromSeconds(totalSeconds);
                string value = Math.Floor(dt.TotalMinutes).ToString();
                timelineChart.Series[productivity].Points.AddXY(hour, value);
            }
        }

        RefreshListBox();
    }
    bool noRefreshFlag = false;
    void RefreshListBox()
    {
        if (noRefreshFlag)
            return;
        ActivityNamesForListbox.Clear();
        ActivityNamesForListboxIndexes.Clear();
        foreach (KeyValuePair<ulong, Activity> entry in activities)
        {
            // filter anything that's not been logged for more than 10 seconds
            if (entry.Value.totalSecondsActive < TimeSpan.FromSeconds(10))
                continue;

            if (filterType == FilterType.Category && entry.Value.category != filteredCategory)
                continue;

            if (filterType == FilterType.Productivity && entry.Value.productivity != filteredProductivity)
                continue;

            ActivityNamesForListbox.Add(entry.Value.prettyName);
            ActivityNamesForListboxIndexes.Add(entry.Key);
        }
        noRefreshFlag = true;
        int selected = activitiesListBox.SelectedIndex;
        if (ActivityNamesForListbox.Count <= selected)
            selected = -1;
        activitiesListBox.DataSource = null;
        activitiesListBox.DataSource = ActivityNamesForListbox;
        activitiesListBox.SelectedIndex = selected;
        noRefreshFlag = false;
    }

    public Form1()
    {
        InitializeComponent();
        productivityComboBox.DataSource = Enum.GetValues(typeof(Productivity));
        categoryComboBox.DataSource = Enum.GetValues(typeof(Category));
        timeViewComboBox.DataSource = Enum.GetValues(typeof(TimeView));

        dateTimePicker1.Value = DateTime.Now;
        
    }

    ulong sessionStartTimestamp;

    // Program start
    private void Form1_Load(object sender, EventArgs e)
    {
        string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        userDataPath = $@"{appdataPath}\ActivityTime";
        if (!Directory.Exists(userDataPath))
            Directory.CreateDirectory(userDataPath);

        // Create session file
        sessionStartTimestamp = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();
        SessionFilePath = $@"{userDataPath}\{sessionStartTimestamp}.bin";
        if (!File.Exists(SessionFilePath))
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
        ReloadUI();

        // this is slow and unreprdictable so we do it in a different thread.<
        Thread BrowserActivityUpdaterThread = new Thread(GetBrowserMapping);
        BrowserActivityUpdaterThread.Start();
    }

    enum BrowserType
    {
        Firefox,
        Chrome,
        Brave,
        Edge,
    }

    class BrowserData
    {
        public string name;
        public BrowserType browserType;
        public IntPtr window;
        public IUIAutomationElement elementCOM;
        public System.Windows.Automation.AutomationElement element;
        //public string domainName;
    }
    ConcurrentDictionary<IntPtr, BrowserData> BrowserMapping = new ConcurrentDictionary<IntPtr, BrowserData>();

    // https://learn.microsoft.com/en-us/windows/win32/winauto/uiauto-automation-element-propids
    int UIA_ControlTypePropertyId = 30003;
    int UIA_ClassNamePropertyId = 30012;
    int UIA_AutomationIdPropertyId = 30011;

    int UIA_NamePropertyId = 30005;

    // https://learn.microsoft.com/en-us/windows/win32/winauto/uiauto-controltype-ids
    int UIA_EditControlTypeId = 50004;

    // https://learn.microsoft.com/en-us/windows/win32/winauto/uiauto-controlpattern-ids
    int UIA_ValuePatternId = 10002;

    IUIAutomationElement GetEditElementViaTreeNavigation(CUIAutomation automation, BrowserData data)
    {
        IUIAutomationElement element = null;
        try
        {
            StatsLog2("    ElementFromHandle()");
            Thread.Sleep(250);
            element = automation.ElementFromHandle(data.window);
        }
        catch { };

        if (element == null)
        {
            StatsLog2("    ElementFromHandle() failed.");
            return null;
        }

        try
        {
            StatsLog2("    FindFirst()");
            Thread.Sleep(250);
            if(data.browserType == BrowserType.Firefox)
                element = element.FindFirst(TreeScope.TreeScope_Descendants, automation.CreatePropertyCondition(UIA_AutomationIdPropertyId, "urlbar-input"));
            if (data.browserType == BrowserType.Brave)
                element = element.FindFirst(TreeScope.TreeScope_Descendants, automation.CreatePropertyCondition(UIA_NamePropertyId, "Address and search bar"));
            if (data.browserType == BrowserType.Chrome)
                element = element.FindFirst(TreeScope.TreeScope_Descendants, automation.CreatePropertyCondition(UIA_NamePropertyId, "Address and search bar"));
            if (data.browserType == BrowserType.Edge)
                element = element.FindFirst(TreeScope.TreeScope_Descendants, automation.CreatePropertyCondition(UIA_NamePropertyId, "Address and search bar"));
        }
        catch { };

        if (element == null)
        {
            StatsLog2("    FindFirst() failed null.");
            return null;
        }

        if(element.CurrentControlType != UIA_EditControlTypeId)
        {
            StatsLog2("    FindFirst() failed Not an edit control.");
        }
        Process proc = Process.GetProcessById((int)element.CurrentProcessId);
        StatsLog2("    element found, belongs to: " + proc.ProcessName);
        StatsLog2("    element found, name is: " + element.CurrentName);


        IUIAutomationValuePattern value = null;
        try
        {
            StatsLog2("    GetCurrentPattern()");
            Thread.Sleep(250);
            value = (IUIAutomationValuePattern)element.GetCurrentPattern(UIA_ValuePatternId);
        }
        catch { }

        if (value == null)
        {
            StatsLog2("    GetCurrentPattern() failed null.");
            return null;
        }

        if (value.CurrentValue == "")
        {
            StatsLog2("    GetCurrentPattern() failed emptystring.");
            return null;
        }

        return element;
    }

    IUIAutomationElement GetEditElementViaDeadReckoningCOM(CUIAutomation automation, BrowserData data)
    {
        int magicOffsetX = 400;
        int magicOffsetY = 55;
        RECT rect = new RECT();
        GetWindowRect(data.window, ref rect);
        tagPOINT testPoint = new tagPOINT();
        testPoint.x += rect.Left + magicOffsetX;
        testPoint.y += rect.Top + magicOffsetY;

        Thread.Sleep(250);
        IUIAutomationElement element = null;
        try
        {
            StatsLog2("    ElementFromPoint()");
            Thread.Sleep(250);
            element = automation.ElementFromPoint(testPoint);
        }
        catch { }

        if (element == null)
        {
            StatsLog2("    ElementFromPoint() failed.");
            return null;
        }

        IUIAutomationValuePattern value = null;
        try
        {
            StatsLog2("    GetCurrentPattern()");
            Thread.Sleep(250);
            value = (IUIAutomationValuePattern)element.GetCurrentPattern(UIA_ValuePatternId);
        }
        catch { }

        if (value == null)
        {
            StatsLog2("    GetCurrentPattern() failed null.");
            return null;
        }

        if (value.CurrentValue == "")
        {
            StatsLog2("    GetCurrentPattern() failed emptystring.");
            return null;
        }

        return element;
    }

    AutomationElement GetEditElementViaDeadReckoning(BrowserData data)
    {
        int magicOffsetX = 400;
        int magicOffsetY = 55;
        RECT rect = new RECT();
        System.Windows.Point testPoint = new System.Windows.Point(0, 0);
        testPoint.X += rect.Left + magicOffsetX;
        testPoint.Y += rect.Top + magicOffsetY;

        StatsLog2("    FromPoint()");
        Thread.Sleep(250);
        AutomationElement element = AutomationElement.FromPoint(testPoint);
        if (element == null)
        {
            StatsLog2("    FromPoint() failed null");
            return null;
        }

        Process proc = Process.GetProcessById((int)element.Current.ProcessId);
        StatsLog2("    element found, belongs to: " + proc.ProcessName);

        if (element.Current.LocalizedControlType != "edit")
        {
            StatsLog2("    FromPoint() failed wrong element type");
            return null;
        }

        Thread.Sleep(250);
        StatsLog2("    GetCurrentPattern()");
        ValuePattern v = ((ValuePattern)element.GetCurrentPattern(ValuePattern.Pattern));

        if (v == null)
        {
            StatsLog2("    GetCurrentPattern() failed null");
            return null;
        }

        if (v.Current.Value == null)
        {
            StatsLog2("    GetCurrentPattern() failed value null");
            return null;
        }

        if (v.Current.Value == "")
        {
            StatsLog2("    GetCurrentPattern() failed emptystring");
            return null;
        }

        return element;
    }

    void GetBrowserMapping()
    {
        CUIAutomation automation = new CUIAutomation();
        while (true)
        {
            //statsString = "";
            foreach (var v in BrowserMapping)
            {
                if (v.Value != null && v.Value.elementCOM == null)
                {
                    StatsLog2("");
                    StatsLog2("--------");
                    StatsLog2("START Check mapping for program: " + v.Value.name + ", window: " + v.Value.window.ToString());

                    BrowserData data = v.Value;
                    IUIAutomationElement elementCOM = null;
                    AutomationElement element = null;

                    // Tree naviation
                    if (elementCOM == null)
                    {
                        StatsLog2("Tree navigation");
                        elementCOM = GetEditElementViaTreeNavigation(automation, data);
                    }
                    if (elementCOM == null)
                        StatsLog2("Tree navigation FAILED.");

                    // COM API Dead Reckoning
                    if (elementCOM == null)
                    {
                        StatsLog2("Dead Reckoning via COM API");
                        elementCOM = GetEditElementViaDeadReckoningCOM(automation, data);
                    }
                    if (elementCOM == null)
                        StatsLog2("Dead Reckoning via COM API FAILED");
                    
                    // Managed API Dead Reckoning
                    if (elementCOM == null)
                    {
                        StatsLog2("Dead Reckoning via Managed API ");
                        element = GetEditElementViaDeadReckoning(data);
                    }
                    if (elementCOM == null && element == null)
                        StatsLog2("Dead Reckoning via Managed API FAILED");


                    if (elementCOM == null && element == null)
                    {
                        StatsLog2("Everything failed. Trying again...");
                        continue;
                    }
                    StatsLog2("Success.");
                    v.Value.elementCOM = elementCOM;
                    v.Value.element = element;
                }
            }
            statsString2 = statsString;
        }
    }
    void StatsLog(string message)
    {
        //statsString += message + "\n";
    }
    void StatsLog2(string message)
    {
        statsString += message + "\n";
    }


    // This function gets the exe name of whatver window the user has selected.
    // Activities are either exe names, or domain names.
    string GetFocusedActivityName()
    {
        StatsLog("GetFocusedActivityName()");
        // If the mouse has been still for 10 minutes, we assume the user is afk and pause logging.
        TicksWithMouseStill++;
        if (CursorPosition != System.Windows.Forms.Cursor.Position)
        {
            CursorPosition = System.Windows.Forms.Cursor.Position;
            TicksWithMouseStill = 0;
        }
        if (TicksWithMouseStill > (10 * 60 * 60))
        {
            StatsLog("Mouse was still for 10 minutes, logging paused.");
            return "";
        }

        // get window handle
        StatsLog("Get window handle.");
        IntPtr currentWindow = GetForegroundWindow();
        if (currentWindow == IntPtr.Zero)
        {
            StatsLog("Get window failed.");
            return "";
        }

        StatsLog("Get program name.");
        uint processID = 0;
        uint threadID = GetWindowThreadProcessId(currentWindow, out processID);

        //uint pid = GetWindowThreadProcessId(currentWindow, IntPtr.Zero);
        Process proc = Process.GetProcessById((int)processID);

        string exeName = proc.ProcessName;

        // result is the name of the exe.
        string result = exeName;

        StatsLog("Get program name: " + exeName);
        if (exeName == "firefox" || exeName == "chrome" || exeName == "brave" || exeName == "msedge")
        {
            StatsLog("program is web browser");
            if (!BrowserMapping.ContainsKey(currentWindow))
            {
                StatsLog("program missing in keymap, adding it.");
                BrowserData data = new BrowserData();
                data.name = exeName;
                data.window = currentWindow;

                if (exeName == "firefox")
                    data.browserType = BrowserType.Firefox;
                if (exeName == "chrome")
                    data.browserType = BrowserType.Chrome;
                if (exeName == "brave")
                    data.browserType = BrowserType.Brave;
                if (exeName == "msedge")
                    data.browserType = BrowserType.Edge;

                BrowserMapping.TryAdd(currentWindow, data);
            }

            if (BrowserMapping.ContainsKey(currentWindow) && BrowserMapping[currentWindow] != null)
            {
                IUIAutomationElement element = BrowserMapping[currentWindow].elementCOM;
                if (element != null)
                {
                    IUIAutomationValuePattern val = null;
                    try
                    {
                        val = (IUIAutomationValuePattern)element.GetCurrentPattern(UIA_ValuePatternId);
                    }
                    catch { }
                    if (val != null && val.CurrentValue != "")
                    {
                        result = CleanupURL(val.CurrentValue);
                    }
                }
            }
        }
        return result;
    }
    
    string statsString;
    string statsString2;

    int TicksWithMouseStill = 0;
    System.Drawing.Point CursorPosition = new System.Drawing.Point(0, 0);
    private void tick_Tick(object sender, EventArgs e)
    {
        string focusedActivityName = GetFocusedActivityName();
        statsLabel.Text = statsString2;
        ulong hash = 0;
        activeAppLabel.Text = "???";
        if (focusedActivityName != "")
        {
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
        
        byte[] data2 = File.ReadAllBytes(SessionFilePath);
        ulong fileTimestamp = sessionStartTimestamp;
        for (int i = 0; i < data2.Length; i += 16)
        {
            fileTimestamp += BitConverter.ToUInt64(data2, i + 8);
        }
        ulong currentTimestamp = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();
        long d = ((long)currentTimestamp - (long)fileTimestamp);
        //Console.WriteLine(d);
        if (Math.Abs((double)d) > (60 * 5)) // If we've drifted by 5 minutes, reset the counter
        {
            currentFocusedProgram = 0;
            ulong non = 0;
            AppendAllBytes(SessionFilePath, BitConverter.GetBytes(non));
            AppendAllBytes(SessionFilePath, BitConverter.GetBytes((ulong)d));
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
        data = BitConverter.GetBytes(count);
        WriteLastBytes(SessionFilePath, data);
        if(focusedActivityName != "")
        {
            activeAppLabel.Text = focusedActivityName + ": " + TimeSpan.FromSeconds(count).ToString();
        }
    }


    private void histogramChart_MouseClick(object sender, MouseEventArgs e)
    {
        var r = histogramChart.HitTest(e.X, e.Y);

        if (r.ChartElementType == ChartElementType.DataPoint)
        {
            DataPoint p = (DataPoint)r.Object;
            int index = r.PointIndex;

            filterType = FilterType.None;
            ReloadUI();
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
    private void timelineChart_MouseClick(object sender, MouseEventArgs e)
    {
        var r = timelineChart.HitTest(e.X, e.Y);

        if (r.ChartElementType == ChartElementType.DataPoint)
        {
            DataPoint p = (DataPoint)r.Object;
            int index = r.PointIndex;

            filteredProductivity = productivityTimelinePoints[index];
            filterType = FilterType.Productivity;
            ReloadUI();
        }
    }

    private void histogramChart_MouseMove(object sender, MouseEventArgs e)
    {
        var r = histogramChart.HitTest(e.X, e.Y);

        if (r.ChartElementType == ChartElementType.DataPoint)
            histogramChart.Cursor = Cursors.Hand;
        else
            histogramChart.Cursor = Cursors.Default;
    }

    private void timelineChart_MouseMove(object sender, MouseEventArgs e)
    {
        var r = timelineChart.HitTest(e.X, e.Y);

        if (r.ChartElementType == ChartElementType.DataPoint)
            timelineChart.Cursor = Cursors.Hand;
        else
            timelineChart.Cursor = Cursors.Default;
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
        TimeChanged();
    }

    void TimeChanged()
    {
        dateTimePicker1.ShowUpDown = false;
        timeView = (TimeView)timeViewComboBox.SelectedIndex;
        switch (timeView)
        {
            case TimeView.Daily:
                chooseDayLabel.Text = "Choose Day";
                dateTimePicker1.Format = DateTimePickerFormat.Short;
                timeByHourLabel.Text = "Focus by hour";
                timelineSubsteps = 24;

                ViewStart = dateTimePicker1.Value;
                ViewEnd = dateTimePicker1.Value;
                ViewStart = new DateTime(ViewStart.Year, ViewStart.Month, ViewStart.Day, 0, 0, 0);
                ViewEnd = new DateTime(ViewEnd.Year, ViewEnd.Month, ViewEnd.Day, 0, 0, 0) + TimeSpan.FromDays(1);
                break;
            case TimeView.Weekly:
                chooseDayLabel.Text = "Choose Week";
                dateTimePicker1.Format = DateTimePickerFormat.Short;
                timeByHourLabel.Text = "Focus by day";
                timelineSubsteps = 7;

                ViewStart = dateTimePicker1.Value - TimeSpan.FromDays((int)dateTimePicker1.Value.DayOfWeek);
                ViewEnd = dateTimePicker1.Value + TimeSpan.FromDays(7 - (int)dateTimePicker1.Value.DayOfWeek);
                ViewStart = new DateTime(ViewStart.Year, ViewStart.Month, ViewStart.Day, 0, 0, 0);
                ViewEnd = new DateTime(ViewEnd.Year, ViewEnd.Month, ViewEnd.Day, 0, 0, 0);
                break;
            case TimeView.Monthly:
                chooseDayLabel.Text = "Choose Month";
                dateTimePicker1.Format = DateTimePickerFormat.Custom;
                dateTimePicker1.CustomFormat = "yyyy-MM";
                timeByHourLabel.Text = "Focus by day";
                timelineSubsteps = DateTime.DaysInMonth(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month);

                ViewStart = dateTimePicker1.Value;
                ViewEnd = dateTimePicker1.Value;
                ViewStart = new DateTime(ViewStart.Year, ViewStart.Month, 1, 0, 0, 0);
                ViewEnd = new DateTime(ViewEnd.Year, ViewEnd.Month, 1, 0, 0, 0) + TimeSpan.FromDays(timelineSubsteps);
                break;
            case TimeView.Yearly:
                chooseDayLabel.Text = "Choose Year";
                dateTimePicker1.Format = DateTimePickerFormat.Custom;
                dateTimePicker1.CustomFormat = "yyyy";
                dateTimePicker1.ShowUpDown = true;
                timeByHourLabel.Text = "Focus by day";
                timelineSubsteps = 52;

                ViewStart = dateTimePicker1.Value;
                ViewEnd = dateTimePicker1.Value;
                ViewStart = new DateTime(ViewStart.Year, 1, 1, 0, 0, 0);
                ViewEnd = new DateTime(ViewEnd.Year + 1, 1, 1, 0, 0, 0);
                break;
            default:
                break;
        }
        ReloadUI();
    }
    private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
    {
        TimeChanged();
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
        filterType = FilterType.None;
        ReloadUI();
    }

    private void categoryName4Label_Click(object sender, EventArgs e)
    {
        filteredCategory = categories[4];
        filterType = FilterType.Category;
        ReloadUI();
    }

    private void categoryName3Label_Click(object sender, EventArgs e)
    {
        filteredCategory = categories[3];
        filterType = FilterType.Category;
        ReloadUI();
    }

    private void categoryName2Label_Click(object sender, EventArgs e)
    {
        filteredCategory = categories[2];
        filterType = FilterType.Category;
        ReloadUI();
    }

    private void categoryName1Label_Click(object sender, EventArgs e)
    {
        filteredCategory = categories[1];
        filterType = FilterType.Category;
        ReloadUI();
    }

    private void categoryName0Label_Click(object sender, EventArgs e)
    {
        filteredCategory = categories[0];
        filterType = FilterType.Category;
        ReloadUI();
    }

    private void Form1_Activated(object sender, EventArgs e)
    {
        ReloadUI();
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        Environment.Exit(0);
    }

    private void refreshStripMenuItem_Click(object sender, EventArgs e)
    {
        ReloadUI();
    }

    private void statsForNerdsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        // TODO: log currently active app
        // Log file delta compared to realtime
    }

    private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ReloadUI();
    }

    private void startsForNerdsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        statsPanel.Visible = !statsPanel.Visible;
    }
}
