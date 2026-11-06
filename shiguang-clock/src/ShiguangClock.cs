using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ShiguangClock
{
    internal enum ThemeKind
    {
        Dark,
        Light
    }

    internal sealed class Palette
    {
        public Color BgTop;
        public Color BgBottom;
        public Color GlowWarm;
        public Color GlowCool;
        public Color Text;
        public Color Muted;
        public Color Faint;
        public Color Accent;
        public Color AccentSoft;
        public Color Second;
        public Color DialTop;
        public Color DialBottom;
        public Color DialBorder;
        public Color Tick;
        public Color Track;
        public Color Hover;
        public Color CloseHover;
        public Color Panel;
        public Color PanelBorder;

        public static Palette CreateDark()
        {
            Palette p = new Palette();
            p.BgTop = Color.FromArgb(12, 20, 36);
            p.BgBottom = Color.FromArgb(8, 13, 24);
            p.GlowWarm = Color.FromArgb(38, 231, 184, 114);
            p.GlowCool = Color.FromArgb(24, 93, 142, 190);
            p.Text = Color.FromArgb(245, 241, 232);
            p.Muted = Color.FromArgb(151, 164, 185);
            p.Faint = Color.FromArgb(91, 106, 130);
            p.Accent = Color.FromArgb(231, 184, 114);
            p.AccentSoft = Color.FromArgb(54, 45, 31);
            p.Second = Color.FromArgb(255, 121, 104);
            p.DialTop = Color.FromArgb(18, 29, 47);
            p.DialBottom = Color.FromArgb(10, 17, 30);
            p.DialBorder = Color.FromArgb(48, 63, 86);
            p.Tick = Color.FromArgb(131, 147, 171);
            p.Track = Color.FromArgb(33, 47, 68);
            p.Hover = Color.FromArgb(25, 39, 60);
            p.CloseHover = Color.FromArgb(193, 69, 61);
            p.Panel = Color.FromArgb(21, 33, 52);
            p.PanelBorder = Color.FromArgb(45, 60, 82);
            return p;
        }

        public static Palette CreateLight()
        {
            Palette p = new Palette();
            p.BgTop = Color.FromArgb(249, 245, 237);
            p.BgBottom = Color.FromArgb(235, 226, 212);
            p.GlowWarm = Color.FromArgb(34, 181, 126, 55);
            p.GlowCool = Color.FromArgb(18, 86, 122, 148);
            p.Text = Color.FromArgb(30, 39, 52);
            p.Muted = Color.FromArgb(104, 111, 123);
            p.Faint = Color.FromArgb(151, 142, 128);
            p.Accent = Color.FromArgb(172, 112, 42);
            p.AccentSoft = Color.FromArgb(242, 228, 207);
            p.Second = Color.FromArgb(198, 79, 59);
            p.DialTop = Color.FromArgb(252, 249, 242);
            p.DialBottom = Color.FromArgb(243, 235, 222);
            p.DialBorder = Color.FromArgb(214, 199, 178);
            p.Tick = Color.FromArgb(146, 130, 109);
            p.Track = Color.FromArgb(221, 208, 190);
            p.Hover = Color.FromArgb(239, 230, 217);
            p.CloseHover = Color.FromArgb(193, 69, 61);
            p.Panel = Color.FromArgb(252, 248, 241);
            p.PanelBorder = Color.FromArgb(222, 209, 190);
            return p;
        }

        public static Palette Lerp(Palette a, Palette b, float t)
        {
            Palette p = new Palette();
            p.BgTop = Mix(a.BgTop, b.BgTop, t);
            p.BgBottom = Mix(a.BgBottom, b.BgBottom, t);
            p.GlowWarm = Mix(a.GlowWarm, b.GlowWarm, t);
            p.GlowCool = Mix(a.GlowCool, b.GlowCool, t);
            p.Text = Mix(a.Text, b.Text, t);
            p.Muted = Mix(a.Muted, b.Muted, t);
            p.Faint = Mix(a.Faint, b.Faint, t);
            p.Accent = Mix(a.Accent, b.Accent, t);
            p.AccentSoft = Mix(a.AccentSoft, b.AccentSoft, t);
            p.Second = Mix(a.Second, b.Second, t);
            p.DialTop = Mix(a.DialTop, b.DialTop, t);
            p.DialBottom = Mix(a.DialBottom, b.DialBottom, t);
            p.DialBorder = Mix(a.DialBorder, b.DialBorder, t);
            p.Tick = Mix(a.Tick, b.Tick, t);
            p.Track = Mix(a.Track, b.Track, t);
            p.Hover = Mix(a.Hover, b.Hover, t);
            p.CloseHover = Mix(a.CloseHover, b.CloseHover, t);
            p.Panel = Mix(a.Panel, b.Panel, t);
            p.PanelBorder = Mix(a.PanelBorder, b.PanelBorder, t);
            return p;
        }

        private static Color Mix(Color a, Color b, float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            return Color.FromArgb(
                (int)Math.Round(a.A + (b.A - a.A) * t),
                (int)Math.Round(a.R + (b.R - a.R) * t),
                (int)Math.Round(a.G + (b.G - a.G) * t),
                (int)Math.Round(a.B + (b.B - a.B) * t));
        }
    }

    internal sealed class AppSettings
    {
        public ThemeKind Theme = ThemeKind.Dark;
        public bool AlwaysOnTop;
        public bool Use24Hour = true;
        public Rectangle Bounds = Rectangle.Empty;

        private static string SettingsPath
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "ShiguangClock",
                    "settings.ini");
            }
        }

        public static AppSettings Load()
        {
            AppSettings s = new AppSettings();
            try
            {
                if (!File.Exists(SettingsPath))
                {
                    return s;
                }

                string[] lines = File.ReadAllLines(SettingsPath, Encoding.UTF8);
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    int eq = line.IndexOf('=');
                    if (eq <= 0)
                    {
                        continue;
                    }

                    string key = line.Substring(0, eq).Trim();
                    string value = line.Substring(eq + 1).Trim();
                    if (string.Equals(key, "theme", StringComparison.OrdinalIgnoreCase))
                    {
                        s.Theme = string.Equals(value, "light", StringComparison.OrdinalIgnoreCase)
                            ? ThemeKind.Light : ThemeKind.Dark;
                    }
                    else if (string.Equals(key, "alwaysOnTop", StringComparison.OrdinalIgnoreCase))
                    {
                        bool parsed;
                        if (bool.TryParse(value, out parsed))
                        {
                            s.AlwaysOnTop = parsed;
                        }
                    }
                    else if (string.Equals(key, "use24Hour", StringComparison.OrdinalIgnoreCase))
                    {
                        bool parsed;
                        if (bool.TryParse(value, out parsed))
                        {
                            s.Use24Hour = parsed;
                        }
                    }
                    else if (string.Equals(key, "bounds", StringComparison.OrdinalIgnoreCase))
                    {
                        s.Bounds = ParseBounds(value);
                    }
                }
            }
            catch
            {
                // A clock should always start, even if its preference file is damaged.
            }
            return s;
        }

        public void Save()
        {
            try
            {
                string dir = Path.GetDirectoryName(SettingsPath);
                Directory.CreateDirectory(dir);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("theme=" + (Theme == ThemeKind.Light ? "light" : "dark"));
                sb.AppendLine("alwaysOnTop=" + AlwaysOnTop.ToString());
                sb.AppendLine("use24Hour=" + Use24Hour.ToString());
                sb.AppendLine(string.Format(
                    CultureInfo.InvariantCulture,
                    "bounds={0},{1},{2},{3}",
                    Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height));
                File.WriteAllText(SettingsPath, sb.ToString(), new UTF8Encoding(false));
            }
            catch
            {
                // Preferences are optional; never interrupt the app for this.
            }
        }

        private static Rectangle ParseBounds(string value)
        {
            string[] parts = value.Split(',');
            if (parts.Length != 4)
            {
                return Rectangle.Empty;
            }

            int x;
            int y;
            int w;
            int h;
            if (int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out x)
                && int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out y)
                && int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out w)
                && int.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out h)
                && w >= 400 && h >= 300)
            {
                return new Rectangle(x, y, w, h);
            }
            return Rectangle.Empty;
        }
    }

    internal static class ClockText
    {
        private static readonly string[] Weekdays =
        {
            "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"
        };

        private static readonly string[] LunarMonths =
        {
            "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "冬", "腊"
        };

        private static readonly string[] LunarDays =
        {
            "初一", "初二", "初三", "初四", "初五", "初六", "初七", "初八", "初九", "初十",
            "十一", "十二", "十三", "十四", "十五", "十六", "十七", "十八", "十九", "二十",
            "廿一", "廿二", "廿三", "廿四", "廿五", "廿六", "廿七", "廿八", "廿九", "三十"
        };

        public static string Weekday(DateTime time)
        {
            return Weekdays[(int)time.DayOfWeek];
        }

        public static string TimeOfDay(DateTime time)
        {
            int h = time.Hour;
            if (h < 5) return "深夜";
            if (h < 8) return "清晨";
            if (h < 11) return "上午";
            if (h < 13) return "中午";
            if (h < 18) return "下午";
            if (h < 23) return "晚上";
            return "深夜";
        }

        public static string Lunar(DateTime time)
        {
            try
            {
                ChineseLunisolarCalendar calendar = new ChineseLunisolarCalendar();
                if (time < calendar.MinSupportedDateTime || time > calendar.MaxSupportedDateTime)
                {
                    return "";
                }

                int lunarYear = calendar.GetYear(time);
                int lunarMonth = calendar.GetMonth(time);
                int lunarDay = calendar.GetDayOfMonth(time);
                int leapMonth = calendar.GetLeapMonth(lunarYear);
                bool isLeap = leapMonth > 0 && lunarMonth == leapMonth;
                if (leapMonth > 0 && lunarMonth >= leapMonth)
                {
                    lunarMonth--;
                }

                int sexagenaryYear = calendar.GetSexagenaryYear(time);
                string stems = "甲乙丙丁戊己庚辛壬癸";
                string branches = "子丑寅卯辰巳午未申酉戌亥";
                int stemIndex = (sexagenaryYear - 1) % 10;
                int branchIndex = (sexagenaryYear - 1) % 12;

                return string.Format(
                    "农历 {0}{1}年 {2}{3}月{4}",
                    stems.Substring(stemIndex, 1),
                    branches.Substring(branchIndex, 1),
                    isLeap ? "闰" : "",
                    LunarMonths[lunarMonth - 1],
                    LunarDays[lunarDay - 1]);
            }
            catch
            {
                return "";
            }
        }

        public static int IsoWeek(DateTime date)
        {
            int day = (int)date.DayOfWeek;
            if (day == 0)
            {
                day = 7;
            }
            DateTime thursday = date.AddDays(4 - day);
            DateTime januaryFirst = new DateTime(thursday.Year, 1, 1);
            return 1 + (int)((thursday - januaryFirst).TotalDays / 7.0);
        }

        public static string UtcOffset()
        {
            TimeSpan offset = DateTimeOffset.Now.Offset;
            int totalMinutes = (int)offset.TotalMinutes;
            string sign = totalMinutes < 0 ? "-" : "+";
            totalMinutes = Math.Abs(totalMinutes);
            return string.Format(
                CultureInfo.InvariantCulture,
                "UTC{0}{1:00}:{2:00}",
                sign, totalMinutes / 60, totalMinutes % 60);
        }

        public static string LocalZone()
        {
            try
            {
                string name = TimeZoneInfo.Local.StandardName;
                if (!string.IsNullOrEmpty(name))
                {
                    return name;
                }
            }
            catch
            {
            }
            return "本地时间";
        }
    }

    internal static class NativeMethods
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(
            IntPtr window, int attribute, ref int value, int size);
    }

    internal static class DrawingHelpers
    {
        public static GraphicsPath RoundedRect(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return path;
            }

            float d = Math.Min(radius * 2f, Math.Min(rect.Width, rect.Height));
            if (d <= 0.5f)
            {
                path.AddRectangle(rect);
                return path;
            }

            path.AddArc(rect.X, rect.Y, d, d, 180f, 90f);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270f, 90f);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0f, 90f);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90f, 90f);
            path.CloseFigure();
            return path;
        }

        public static StringFormat Typographic()
        {
            StringFormat format = (StringFormat)StringFormat.GenericTypographic.Clone();
            format.FormatFlags |= StringFormatFlags.NoWrap;
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Near;
            return format;
        }

        public static void DrawSpacedString(
            Graphics g, string text, Font font, Brush brush,
            float x, float y, float spacing, StringFormat format)
        {
            float cursor = x;
            for (int i = 0; i < text.Length; i++)
            {
                string character = text.Substring(i, 1);
                g.DrawString(character, font, brush, cursor, y, format);
                cursor += g.MeasureString(character, font, PointF.Empty, format).Width + spacing;
            }
        }

        public static float MeasureSpacedString(
            Graphics g, string text, Font font, float spacing, StringFormat format)
        {
            float width = 0f;
            for (int i = 0; i < text.Length; i++)
            {
                width += g.MeasureString(text.Substring(i, 1), font, PointF.Empty, format).Width;
            }
            if (text.Length > 1)
            {
                width += spacing * (text.Length - 1);
            }
            return width;
        }
    }

    internal sealed class ClockForm : Form
    {
        private readonly AppSettings settings;
        private readonly ClockSurface surface;
        private readonly Timer clockTimer;
        private readonly Timer introTimer;
        private int lastPaintedSecond;
        private Rectangle normalBounds;
        private Rectangle boundsBeforeFullscreen;
        private bool previousTopMost;
        private bool fullscreen;
        private bool previewMode;
        private bool isClosing;

        public bool PreviewMode
        {
            get { return previewMode; }
            set { previewMode = value; }
        }

        public bool Fullscreen
        {
            get { return fullscreen; }
        }

        public ClockForm(AppSettings appSettings)
        {
            settings = appSettings;
            Text = "拾光时钟";
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.FromArgb(10, 16, 28);
            DoubleBuffered = true;
            KeyPreview = true;
            AutoScaleMode = AutoScaleMode.None;
            MinimumSize = new Size(920, 580);
            ShowIcon = true;

            try
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch
            {
            }

            surface = new ClockSurface(this, settings.Theme);
            surface.Use24Hour = settings.Use24Hour;
            surface.Dock = DockStyle.Fill;
            Controls.Add(surface);
            TopMost = settings.AlwaysOnTop;

            clockTimer = new Timer();
            clockTimer.Interval = 200;
            clockTimer.Tick += ClockTimerTick;

            introTimer = new Timer();
            introTimer.Interval = 16;
            introTimer.Tick += IntroTimerTick;

            Shown += FormShown;
            FormClosing += FormClosingHandler;
            Resize += FormResize;
            LocationChanged += FormLocationChanged;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= 0x00020000; // CS_DROPSHADOW
                return cp;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            float scale;
            using (Graphics g = CreateGraphics())
            {
                scale = g.DpiX / 96f;
            }

            MinimumSize = new Size((int)Math.Round(920 * scale), (int)Math.Round(580 * scale));
            int defaultWidth = (int)Math.Round(1120 * scale);
            int defaultHeight = (int)Math.Round(680 * scale);
            Rectangle area = Screen.FromPoint(Cursor.Position).WorkingArea;
            int width = Math.Min(defaultWidth, area.Width - 40);
            int height = Math.Min(defaultHeight, area.Height - 40);
            width = Math.Max(width, MinimumSize.Width);
            height = Math.Max(height, MinimumSize.Height);

            bool restored = false;
            if (!settings.Bounds.IsEmpty && IsVisibleOnAnyScreen(settings.Bounds))
            {
                Rectangle saved = settings.Bounds;
                if (saved.Width >= MinimumSize.Width && saved.Height >= MinimumSize.Height)
                {
                    Bounds = saved;
                    normalBounds = saved;
                    restored = true;
                }
            }

            if (!restored)
            {
                Bounds = new Rectangle(
                    area.Left + (area.Width - width) / 2,
                    area.Top + (area.Height - height) / 2,
                    width,
                    height);
                normalBounds = Bounds;
            }

            ApplyWindowChrome();
        }

        private void ApplyWindowChrome()
        {
            try
            {
                int preference = 2; // DWMWCP_ROUND
                NativeMethods.DwmSetWindowAttribute(Handle, 33, ref preference, sizeof(int));
            }
            catch
            {
            }
        }

        private static bool IsVisibleOnAnyScreen(Rectangle bounds)
        {
            Screen[] screens = Screen.AllScreens;
            for (int i = 0; i < screens.Length; i++)
            {
                Rectangle target = Rectangle.Intersect(screens[i].WorkingArea, bounds);
                if (target.Width >= 120 && target.Height >= 120)
                {
                    return true;
                }
            }
            return false;
        }

        private void FormShown(object sender, EventArgs e)
        {
            UpdateMaximizedBounds();
            lastPaintedSecond = DateTime.Now.Second;
            clockTimer.Start();
            if (previewMode)
            {
                Opacity = 1.0;
            }
            else
            {
                Opacity = 0.0;
                introTimer.Start();
            }
            surface.Focus();
        }

        private void IntroTimerTick(object sender, EventArgs e)
        {
            if (previewMode)
            {
                Opacity = 1.0;
                introTimer.Stop();
                return;
            }

            Opacity = Math.Min(1.0, Opacity + 0.16);
            if (Opacity >= 1.0)
            {
                introTimer.Stop();
            }
        }

        private void ClockTimerTick(object sender, EventArgs e)
        {
            if (!Visible || WindowState == FormWindowState.Minimized)
            {
                return;
            }

            DateTime now = DateTime.Now;
            if (now.Second != lastPaintedSecond)
            {
                lastPaintedSecond = now.Second;
                surface.Invalidate();
            }
        }

        private void FormResize(object sender, EventArgs e)
        {
            if (!fullscreen && WindowState == FormWindowState.Normal)
            {
                normalBounds = Bounds;
            }
            surface.Invalidate();
        }

        private void FormLocationChanged(object sender, EventArgs e)
        {
            if (!fullscreen && WindowState == FormWindowState.Normal)
            {
                normalBounds = Bounds;
                UpdateMaximizedBounds();
            }
        }

        private void UpdateMaximizedBounds()
        {
            if (WindowState == FormWindowState.Normal)
            {
                MaximizedBounds = Screen.FromControl(this).WorkingArea;
            }
        }

        private void FormClosingHandler(object sender, FormClosingEventArgs e)
        {
            if (isClosing)
            {
                return;
            }
            isClosing = true;
            clockTimer.Stop();
            introTimer.Stop();

            if (!previewMode)
            {
                settings.Theme = surface.ThemeKind;
                settings.AlwaysOnTop = TopMost;
                settings.Use24Hour = surface.Use24Hour;
                if (!fullscreen && WindowState == FormWindowState.Normal)
                {
                    normalBounds = Bounds;
                }
                settings.Bounds = normalBounds;
                settings.Save();
            }
        }

        public void ToggleTheme()
        {
            ThemeKind next = surface.ThemeKind == ThemeKind.Dark ? ThemeKind.Light : ThemeKind.Dark;
            surface.SetTheme(next, !previewMode);
            settings.Theme = next;
        }

        public void SetTheme(ThemeKind theme, bool animate)
        {
            surface.SetTheme(theme, animate);
            settings.Theme = theme;
        }

        public void ToggleTopMost()
        {
            TopMost = !TopMost;
            settings.AlwaysOnTop = TopMost;
            surface.Invalidate();
        }

        public void ToggleMaximize()
        {
            if (fullscreen)
            {
                ToggleFullscreen();
                return;
            }

            if (WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Normal;
                if (!normalBounds.IsEmpty && IsVisibleOnAnyScreen(normalBounds))
                {
                    Bounds = normalBounds;
                }
            }
            else
            {
                normalBounds = Bounds;
                UpdateMaximizedBounds();
                WindowState = FormWindowState.Maximized;
            }
            surface.Invalidate();
        }

        public void ToggleFullscreen()
        {
            if (!fullscreen)
            {
                boundsBeforeFullscreen = WindowState == FormWindowState.Normal ? Bounds : normalBounds;
                previousTopMost = TopMost;
                fullscreen = true;
                WindowState = FormWindowState.Normal;
                surface.Fullscreen = true;
                Bounds = Screen.FromControl(this).Bounds;
                TopMost = true;
            }
            else
            {
                fullscreen = false;
                surface.Fullscreen = false;
                Bounds = boundsBeforeFullscreen;
                WindowState = FormWindowState.Normal;
                TopMost = previousTopMost;
            }
            surface.Invalidate();
        }

        public void CopyCurrentTime()
        {
            DateTime now = DateTime.Now;
            string lunar = ClockText.Lunar(now);
            string line = string.Format(
                "{0}年{1}月{2}日 {3} {4} {5:00}:{6:00}:{7:00}",
                now.Year, now.Month, now.Day,
                ClockText.Weekday(now),
                lunar,
                now.Hour, now.Minute, now.Second).Replace("  ", " ");
            try
            {
                Clipboard.SetText(line.Trim());
                surface.ShowToast("已复制当前日期和时间");
            }
            catch
            {
                surface.ShowToast("复制失败，请重试");
            }
        }

        public void ShowHelp()
        {
            string message =
                "拾光时钟  ·  时间与日期\n\n" +
                "双击标题栏    最大化 / 还原\n" +
                "F11                 全屏显示\n" +
                "Esc                 退出全屏\n" +
                "T                    切换深色 / 浅色主题\n" +
                "H                    切换 12 / 24 小时制\n" +
                "P                    窗口置顶开关\n" +
                "Ctrl + C          复制当前日期和时间\n\n" +
                "右键窗口可打开快捷菜单。\n" +
                "设置会自动保存，下次启动时恢复。";
            MessageBox.Show(
                this,
                message,
                "关于拾光时钟",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public void ShowContextMenu(Point location)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.ShowImageMargin = false;
            menu.Font = new Font("Microsoft YaHei UI", 9.5f, FontStyle.Regular);
            menu.BackColor = surface.IsLight ? Color.FromArgb(252, 248, 241) : Color.FromArgb(23, 34, 51);
            menu.ForeColor = surface.IsLight ? Color.FromArgb(30, 39, 52) : Color.FromArgb(238, 234, 225);

            ToolStripMenuItem themeItem = new ToolStripMenuItem(
                surface.ThemeKind == ThemeKind.Dark ? "切换到浅色主题" : "切换到深色主题");
            themeItem.Click += delegate { ToggleTheme(); };

            ToolStripMenuItem formatItem = new ToolStripMenuItem("使用 24 小时制");
            formatItem.Checked = surface.Use24Hour;
            formatItem.Click += delegate
            {
                surface.Use24Hour = !surface.Use24Hour;
                surface.Invalidate();
            };

            ToolStripMenuItem topItem = new ToolStripMenuItem("窗口置顶");
            topItem.Checked = TopMost;
            topItem.Click += delegate { ToggleTopMost(); };

            ToolStripMenuItem copyItem = new ToolStripMenuItem("复制日期和时间");
            copyItem.Click += delegate { CopyCurrentTime(); };

            ToolStripMenuItem fullItem = new ToolStripMenuItem("全屏显示");
            fullItem.Click += delegate { ToggleFullscreen(); };

            ToolStripMenuItem helpItem = new ToolStripMenuItem("快捷键说明");
            helpItem.Click += delegate { ShowHelp(); };

            ToolStripMenuItem exitItem = new ToolStripMenuItem("退出拾光时钟");
            exitItem.Click += delegate { Close(); };

            menu.Items.Add(themeItem);
            menu.Items.Add(formatItem);
            menu.Items.Add(topItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(copyItem);
            menu.Items.Add(fullItem);
            menu.Items.Add(helpItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(exitItem);
            menu.Show(this, location);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C))
            {
                CopyCurrentTime();
                return true;
            }
            if (keyData == Keys.F11)
            {
                ToggleFullscreen();
                return true;
            }
            if (keyData == Keys.Escape && fullscreen)
            {
                ToggleFullscreen();
                return true;
            }
            if (keyData == Keys.T)
            {
                ToggleTheme();
                return true;
            }
            if (keyData == Keys.H)
            {
                surface.Use24Hour = !surface.Use24Hour;
                surface.Invalidate();
                return true;
            }
            if (keyData == Keys.P)
            {
                ToggleTopMost();
                return true;
            }
            if (keyData == Keys.F1)
            {
                ShowHelp();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;
            const int HTCLIENT = 1;
            const int HTLEFT = 10;
            const int HTRIGHT = 11;
            const int HTTOP = 12;
            const int HTTOPLEFT = 13;
            const int HTTOPRIGHT = 14;
            const int HTBOTTOM = 15;
            const int HTBOTTOMLEFT = 16;
            const int HTBOTTOMRIGHT = 17;

            if (m.Msg == WM_NCHITTEST && !fullscreen && WindowState == FormWindowState.Normal)
            {
                base.WndProc(ref m);
                if (m.Result.ToInt32() == HTCLIENT)
                {
                    int raw = m.LParam.ToInt32();
                    int screenX = (short)(raw & 0xFFFF);
                    int screenY = (short)((raw >> 16) & 0xFFFF);
                    Point p = PointToClient(new Point(screenX, screenY));
                    int grip = (int)Math.Round(7 * DeviceDpi / 96.0);
                    bool left = p.X <= grip;
                    bool right = p.X >= ClientSize.Width - grip;
                    bool top = p.Y <= grip;
                    bool bottom = p.Y >= ClientSize.Height - grip;

                    if (left && top) m.Result = (IntPtr)HTTOPLEFT;
                    else if (right && top) m.Result = (IntPtr)HTTOPRIGHT;
                    else if (left && bottom) m.Result = (IntPtr)HTBOTTOMLEFT;
                    else if (right && bottom) m.Result = (IntPtr)HTBOTTOMRIGHT;
                    else if (left) m.Result = (IntPtr)HTLEFT;
                    else if (right) m.Result = (IntPtr)HTRIGHT;
                    else if (top) m.Result = (IntPtr)HTTOP;
                    else if (bottom) m.Result = (IntPtr)HTBOTTOM;
                }
                return;
            }
            base.WndProc(ref m);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                clockTimer.Dispose();
                introTimer.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    internal sealed class ClockSurface : Control
    {
        private const int ButtonClose = 0;
        private const int ButtonMaximize = 1;
        private const int ButtonMinimize = 2;
        private const int ButtonPin = 3;
        private const int ButtonTheme = 4;

        private readonly ClockForm host;
        private readonly Timer themeTimer;
        private readonly Timer toastTimer;
        private readonly ToolTip toolTip;
        private readonly RectangleF[] buttonRects;
        private Palette palette;
        private Palette themeFrom;
        private Palette themeTo;
        private DateTime themeStarted;
        private DateTime toastUntil;
        private string toastText;
        private ThemeKind themeKind;
        private int hoverButton;
        private float uiScale;
        private bool draggingWindow;
        private bool dragMoved;
        private Point dragCursorStart;
        private Point dragWindowStart;
        private bool fullscreen;
        private bool use24Hour;

        public ThemeKind ThemeKind
        {
            get { return themeKind; }
        }

        public bool Use24Hour
        {
            get { return use24Hour; }
            set
            {
                use24Hour = value;
                Invalidate();
            }
        }

        public bool IsLight
        {
            get { return themeKind == ThemeKind.Light; }
        }

        public bool Fullscreen
        {
            get { return fullscreen; }
            set
            {
                fullscreen = value;
                hoverButton = -1;
                toolTip.Hide(this);
                Invalidate();
            }
        }

        public ClockSurface(ClockForm form, ThemeKind initialTheme)
        {
            host = form;
            themeKind = initialTheme;
            palette = initialTheme == ThemeKind.Light ? Palette.CreateLight() : Palette.CreateDark();
            themeFrom = palette;
            themeTo = palette;
            use24Hour = true;
            uiScale = 1f;
            hoverButton = -1;
            buttonRects = new RectangleF[5];

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);
            TabStop = false;

            themeTimer = new Timer();
            themeTimer.Interval = 15;
            themeTimer.Tick += ThemeTimerTick;

            toastTimer = new Timer();
            toastTimer.Interval = 1700;
            toastTimer.Tick += ToastTimerTick;

            toolTip = new ToolTip();
            toolTip.InitialDelay = 300;
            toolTip.ReshowDelay = 80;
            toolTip.ShowAlways = true;
            toolTip.SetToolTip(this, "拾光时钟");
        }

        public void SetTheme(ThemeKind kind, bool animate)
        {
            if (kind == themeKind && animate)
            {
                return;
            }

            themeKind = kind;
            themeFrom = palette;
            themeTo = kind == ThemeKind.Light ? Palette.CreateLight() : Palette.CreateDark();
            if (!animate)
            {
                themeTimer.Stop();
                palette = themeTo;
                Invalidate();
                return;
            }

            themeStarted = DateTime.Now;
            themeTimer.Start();
            Invalidate();
        }

        public void SetUse24Hour(bool value)
        {
            use24Hour = value;
            Invalidate();
        }

        private void ThemeTimerTick(object sender, EventArgs e)
        {
            double elapsed = (DateTime.Now - themeStarted).TotalMilliseconds;
            float t = (float)Math.Min(1.0, elapsed / 240.0);
            float eased = t * t * (3f - 2f * t);
            palette = Palette.Lerp(themeFrom, themeTo, eased);
            if (t >= 1f)
            {
                palette = themeTo;
                themeTimer.Stop();
            }
            Invalidate();
        }

        public void ShowToast(string message)
        {
            toastText = message;
            toastUntil = DateTime.Now.AddMilliseconds(1650);
            toastTimer.Stop();
            toastTimer.Start();
            Invalidate();
        }

        private void ToastTimerTick(object sender, EventArgs e)
        {
            toastTimer.Stop();
            toastUntil = DateTime.MinValue;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            uiScale = g.DpiX / 96f;
            float u = uiScale;
            float w = ClientSize.Width;
            float h = ClientSize.Height;
            if (w <= 0 || h <= 0)
            {
                return;
            }

            DateTime now = DateTime.Now;
            PaintBackground(g, w, h, u);
            if (!fullscreen)
            {
                PaintHeader(g, w, h, u);
            }
            else
            {
                for (int i = 0; i < buttonRects.Length; i++)
                {
                    buttonRects[i] = RectangleF.Empty;
                }
                hoverButton = -1;
            }

            float pad = fullscreen ? 58f * u : 34f * u;
            float headerHeight = 84f * u;
            float footerHeight = 46f * u;
            float contentTop = fullscreen ? 44f * u : headerHeight + 28f * u;
            float contentBottom = h - (fullscreen ? 46f * u : footerHeight + 20f * u);
            float contentLeft = pad;
            float contentRight = w - pad;

            if (fullscreen)
            {
                float maxContent = Math.Min(w - 2f * pad, 1700f * u);
                contentLeft = (w - maxContent) / 2f;
                contentRight = contentLeft + maxContent;
            }

            float contentWidth = Math.Max(1f, contentRight - contentLeft);
            float contentHeight = Math.Max(1f, contentBottom - contentTop);
            float gap = fullscreen ? 66f * u : 52f * u;
            float analogMax = Math.Min(contentHeight, contentWidth * 0.44f);
            analogMax = Math.Min(analogMax, (fullscreen ? 690f : 620f) * u);
            float analogSize = Math.Max(220f * u, analogMax);
            float analogLeft = contentLeft;
            float analogTop = contentTop + (contentHeight - analogSize) / 2f;
            RectangleF analogRect = new RectangleF(analogLeft, analogTop, analogSize, analogSize);

            float rightX = analogRect.Right + gap;
            float rightWidth = Math.Max(220f * u, contentRight - rightX);
            PaintAnalog(g, analogRect, now, u);
            PaintDigital(g, new RectangleF(rightX, contentTop, rightWidth, contentHeight), now, u);
            PaintFooter(g, w, h, u, now);

            if (toastUntil > DateTime.Now && !string.IsNullOrEmpty(toastText))
            {
                PaintToast(g, w, h, u);
            }
        }

        private void PaintBackground(Graphics g, float w, float h, float u)
        {
            using (LinearGradientBrush background = new LinearGradientBrush(
                new RectangleF(0, 0, w, h), palette.BgTop, palette.BgBottom, 90f))
            {
                g.FillRectangle(background, 0, 0, w, h);
            }

            using (GraphicsPath warmPath = new GraphicsPath())
            {
                warmPath.AddEllipse(w * 0.45f, -h * 0.55f, w * 0.8f, h * 1.1f);
                using (PathGradientBrush warm = new PathGradientBrush(warmPath))
                {
                    warm.CenterColor = palette.GlowWarm;
                    warm.SurroundColors = new Color[] { Color.FromArgb(0, palette.GlowWarm) };
                    g.FillPath(warm, warmPath);
                }
            }

            using (GraphicsPath coolPath = new GraphicsPath())
            {
                coolPath.AddEllipse(-w * 0.35f, h * 0.42f, w * 0.8f, h * 0.9f);
                using (PathGradientBrush cool = new PathGradientBrush(coolPath))
                {
                    cool.CenterColor = palette.GlowCool;
                    cool.SurroundColors = new Color[] { Color.FromArgb(0, palette.GlowCool) };
                    g.FillPath(cool, coolPath);
                }
            }

            using (Pen arcPen = new Pen(Color.FromArgb(IsLight ? 26 : 32, palette.Accent), 1f * u))
            {
                float diameter = Math.Min(w, h) * 1.35f;
                g.DrawEllipse(
                    arcPen,
                    w * 0.67f - diameter / 2f,
                    h * 0.19f - diameter / 2f,
                    diameter,
                    diameter);
            }

            using (Pen hairline = new Pen(Color.FromArgb(IsLight ? 24 : 30, palette.Text), 1f))
            {
                g.DrawLine(hairline, 0, h - 1, w, h - 1);
            }
        }

        private void PaintHeader(Graphics g, float w, float h, float u)
        {
            float pad = 34f * u;
            float logoSize = 34f * u;
            RectangleF logo = new RectangleF(pad, (84f * u - logoSize) / 2f, logoSize, logoSize);

            using (GraphicsPath logoPath = DrawingHelpers.RoundedRect(logo, 10f * u))
            using (SolidBrush logoFill = new SolidBrush(palette.Panel))
            using (Pen logoBorder = new Pen(palette.PanelBorder, 1f * u))
            {
                g.FillPath(logoFill, logoPath);
                g.DrawPath(logoBorder, logoPath);
            }

            PointF center = new PointF(logo.Left + logo.Width / 2f, logo.Top + logo.Height / 2f);
            float ring = 9.2f * u;
            using (Pen ringPen = new Pen(palette.Accent, 1.5f * u))
            {
                g.DrawEllipse(ringPen, center.X - ring, center.Y - ring, ring * 2f, ring * 2f);
            }
            using (Pen handPen = new Pen(palette.Text, 1.7f * u))
            {
                handPen.StartCap = LineCap.Round;
                handPen.EndCap = LineCap.Round;
                g.DrawLine(
                    handPen,
                    center.X,
                    center.Y,
                    center.X - ring * 0.45f,
                    center.Y - ring * 0.38f);
                g.DrawLine(
                    handPen,
                    center.X,
                    center.Y,
                    center.X + ring * 0.62f,
                    center.Y - ring * 0.52f);
            }
            using (SolidBrush centerDot = new SolidBrush(palette.Accent))
            {
                g.FillEllipse(centerDot, center.X - 1.6f * u, center.Y - 1.6f * u, 3.2f * u, 3.2f * u);
            }

            float brandX = logo.Right + 13f * u;
            using (Font brandFont = new Font("Microsoft YaHei UI", 12.5f, FontStyle.Bold, GraphicsUnit.Point))
            using (Font subFont = new Font("Segoe UI", 7.2f, FontStyle.Regular, GraphicsUnit.Point))
            using (SolidBrush brandBrush = new SolidBrush(palette.Text))
            using (SolidBrush subBrush = new SolidBrush(palette.Muted))
            using (StringFormat format = DrawingHelpers.Typographic())
            {
                g.DrawString("拾光时钟", brandFont, brandBrush, brandX, 17f * u, format);
                DrawingHelpers.DrawSpacedString(
                    g, "SHIGUANG CLOCK", subFont, subBrush,
                    brandX + 1f * u, 43f * u, 1.25f * u, format);
                float subWidth = DrawingHelpers.MeasureSpacedString(
                    g, "SHIGUANG CLOCK", subFont, 1.25f * u, format);
                using (Pen subLine = new Pen(Color.FromArgb(95, palette.Faint), 1f * u))
                {
                    g.DrawLine(
                        subLine,
                        brandX + subWidth + 10f * u,
                        49f * u,
                        brandX + subWidth + 18f * u,
                        49f * u);
                }
                g.DrawString(
                    "时间与日期", subFont, subBrush,
                    brandX + subWidth + 24f * u, 43f * u, format);
            }

            float buttonSize = 32f * u;
            float gap = 4f * u;
            float top = (84f * u - buttonSize) / 2f;
            float x = w - pad - buttonSize;
            for (int i = 0; i < 5; i++)
            {
                buttonRects[i] = new RectangleF(x, top, buttonSize, buttonSize);
                x -= buttonSize + gap;
            }

            for (int i = 0; i < 5; i++)
            {
                PaintHeaderButton(g, i, buttonRects[i], u);
            }

            using (Pen separator = new Pen(Color.FromArgb(IsLight ? 34 : 39, palette.Text), 1f))
            {
                g.DrawLine(separator, pad, 84f * u, w - pad, 84f * u);
            }
        }

        private void PaintHeaderButton(Graphics g, int index, RectangleF rect, float u)
        {
            if (rect.IsEmpty)
            {
                return;
            }

            bool hovered = hoverButton == index;
            bool activePin = index == ButtonPin && host.TopMost;
            Color iconColor = hovered || activePin ? palette.Text : palette.Muted;
            if (index == ButtonClose && hovered)
            {
                iconColor = Color.White;
            }
            if (activePin && !hovered)
            {
                iconColor = palette.Accent;
            }

            if (hovered || activePin)
            {
                Color fill = index == ButtonClose && hovered
                    ? palette.CloseHover
                    : palette.Hover;
                if (activePin && !hovered)
                {
                    fill = palette.AccentSoft;
                }
                using (GraphicsPath path = DrawingHelpers.RoundedRect(rect, 9f * u))
                using (SolidBrush brush = new SolidBrush(fill))
                {
                    g.FillPath(brush, path);
                }
            }

            using (Pen pen = new Pen(iconColor, 1.65f * u))
            using (SolidBrush brush = new SolidBrush(iconColor))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;
                float cx = rect.Left + rect.Width / 2f;
                float cy = rect.Top + rect.Height / 2f;

                if (index == ButtonClose)
                {
                    float d = 5.5f * u;
                    g.DrawLine(pen, cx - d, cy - d, cx + d, cy + d);
                    g.DrawLine(pen, cx + d, cy - d, cx - d, cy + d);
                }
                else if (index == ButtonMinimize)
                {
                    g.DrawLine(pen, cx - 6.5f * u, cy + 5f * u, cx + 6.5f * u, cy + 5f * u);
                }
                else if (index == ButtonMaximize)
                {
                    if (host.WindowState == FormWindowState.Maximized && !fullscreen)
                    {
                        float s = 9f * u;
                        g.DrawRectangle(pen, cx - s / 2f - 2f * u, cy - s / 2f - 2f * u, s, s);
                        using (Pen backPen = new Pen(palette.Hover, 3f * u))
                        {
                            g.DrawLine(
                                backPen,
                                cx - s / 2f + 4f * u,
                                cy - s / 2f - 2f * u,
                                cx + s / 2f + 2f * u,
                                cy - s / 2f - 2f * u);
                        }
                        g.DrawRectangle(pen, cx - s / 2f + 2f * u, cy - s / 2f + 2f * u, s, s);
                    }
                    else
                    {
                        float s = 10f * u;
                        g.DrawRectangle(pen, cx - s / 2f, cy - s / 2f, s, s);
                    }
                }
                else if (index == ButtonPin)
                {
                    float head = 6f * u;
                    g.DrawLine(pen, cx - head, cy - 7.5f * u, cx + head, cy - 7.5f * u);
                    PointF[] pinBody =
                    {
                        new PointF(cx - 4.6f * u, cy - 5.7f * u),
                        new PointF(cx + 4.6f * u, cy - 5.7f * u),
                        new PointF(cx + 1.7f * u, cy + 1.4f * u),
                        new PointF(cx - 1.7f * u, cy + 1.4f * u)
                    };
                    g.FillPolygon(brush, pinBody);
                    g.DrawLine(pen, cx, cy + 1.4f * u, cx, cy + 8.2f * u);
                }
                else if (index == ButtonTheme)
                {
                    if (themeKind == ThemeKind.Dark)
                    {
                        float sunRadius = 4.2f * u;
                        g.DrawEllipse(pen, cx - sunRadius, cy - sunRadius, sunRadius * 2f, sunRadius * 2f);
                        for (int ray = 0; ray < 8; ray++)
                        {
                            double angle = Math.PI * 2.0 * ray / 8.0;
                            float x1 = cx + (float)Math.Cos(angle) * 7.2f * u;
                            float y1 = cy + (float)Math.Sin(angle) * 7.2f * u;
                            float x2 = cx + (float)Math.Cos(angle) * 9.6f * u;
                            float y2 = cy + (float)Math.Sin(angle) * 9.6f * u;
                            g.DrawLine(pen, x1, y1, x2, y2);
                        }
                    }
                    else
                    {
                        float moonSize = 12f * u;
                        RectangleF moonRect = new RectangleF(
                            cx - moonSize / 2f - 1f * u,
                            cy - moonSize / 2f,
                            moonSize,
                            moonSize);
                        GraphicsPath moon = new GraphicsPath();
                        moon.AddEllipse(moonRect);
                        GraphicsPath cut = new GraphicsPath();
                        cut.AddEllipse(
                            moonRect.X + 4.5f * u,
                            moonRect.Y - 2f * u,
                            moonSize,
                            moonSize);
                        Region moonRegion = new Region(moon);
                        moonRegion.Exclude(cut);
                        g.FillRegion(brush, moonRegion);
                        moonRegion.Dispose();
                        cut.Dispose();
                        moon.Dispose();
                    }
                }
            }
        }

        private void PaintAnalog(Graphics g, RectangleF rect, DateTime now, float u)
        {
            float diameter = Math.Min(rect.Width, rect.Height);
            float cx = rect.Left + rect.Width / 2f;
            float cy = rect.Top + rect.Height / 2f;
            float radius = diameter / 2f;

            using (GraphicsPath glowPath = new GraphicsPath())
            {
                glowPath.AddEllipse(
                    cx - radius - 18f * u,
                    cy - radius - 18f * u,
                    diameter + 36f * u,
                    diameter + 36f * u);
                using (PathGradientBrush glow = new PathGradientBrush(glowPath))
                {
                    glow.CenterColor = Color.FromArgb(IsLight ? 18 : 32, palette.Accent);
                    glow.SurroundColors = new Color[] { Color.FromArgb(0, palette.Accent) };
                    g.FillPath(glow, glowPath);
                }
            }

            RectangleF shadowRect = new RectangleF(
                cx - radius + 3f * u,
                cy - radius + 7f * u,
                diameter,
                diameter);
            using (SolidBrush shadow = new SolidBrush(Color.FromArgb(IsLight ? 25 : 72, 0, 0, 0)))
            {
                g.FillEllipse(shadow, shadowRect);
            }

            RectangleF dial = new RectangleF(cx - radius, cy - radius, diameter, diameter);
            using (LinearGradientBrush dialBrush = new LinearGradientBrush(
                dial, palette.DialTop, palette.DialBottom, 115f))
            {
                g.FillEllipse(dialBrush, dial);
            }
            using (Pen border = new Pen(palette.DialBorder, 1.1f * u))
            {
                g.DrawEllipse(border, dial);
            }
            using (Pen innerRing = new Pen(Color.FromArgb(IsLight ? 52 : 65, palette.Accent), 0.9f * u))
            {
                float inner = radius * 0.955f;
                g.DrawEllipse(innerRing, cx - inner, cy - inner, inner * 2f, inner * 2f);
            }

            using (StringFormat centered = new StringFormat())
            {
                centered.Alignment = StringAlignment.Center;
                centered.LineAlignment = StringAlignment.Center;

                using (StringFormat typographic = DrawingHelpers.Typographic())
                using (Font numberFont = new Font(
                    "Georgia",
                    Math.Max(8f, radius * 0.092f * 72f / g.DpiY),
                    FontStyle.Regular,
                    GraphicsUnit.Point))
                {
                    DrawCardinal(g, "12", numberFont, cx, cy - radius * 0.705f, centered, true);
                    DrawCardinal(g, "3", numberFont, cx + radius * 0.705f, cy, centered, false);
                    DrawCardinal(g, "6", numberFont, cx, cy + radius * 0.705f, centered, false);
                    DrawCardinal(g, "9", numberFont, cx - radius * 0.705f, cy, centered, false);

                    float brandY = cy + radius * 0.43f;
                    using (Font brandFont = new Font(
                        "Segoe UI",
                        Math.Max(5.5f, radius * 0.030f * 72f / g.DpiY),
                        FontStyle.Regular,
                        GraphicsUnit.Point))
                    using (SolidBrush brandBrush = new SolidBrush(palette.Faint))
                    {
                        float brandWidth = DrawingHelpers.MeasureSpacedString(
                            g, "SHIGUANG", brandFont, 1.5f * u, typographic);
                        DrawingHelpers.DrawSpacedString(
                            g, "SHIGUANG", brandFont, brandBrush,
                            cx - brandWidth / 2f, brandY, 1.5f * u, typographic);
                    }

                    using (Pen dotPen = new Pen(Color.FromArgb(110, palette.Accent), 1f * u))
                    {
                        g.DrawLine(dotPen, cx - 8f * u, brandY + 12f * u, cx + 8f * u, brandY + 12f * u);
                    }
                }
            }

            float hour = now.Hour % 12;
            float minute = now.Minute;
            float second = now.Second;
            float hourAngle = (hour + minute / 60f + second / 3600f) * 30f;
            float minuteAngle = (minute + second / 60f) * 6f;
            float secondAngle = second * 6f;

            PointF hourEnd = PointOnCircle(cx, cy, radius * 0.47f, hourAngle);
            PointF minuteEnd = PointOnCircle(cx, cy, radius * 0.68f, minuteAngle);
            PointF secondEnd = PointOnCircle(cx, cy, radius * 0.80f, secondAngle);
            PointF secondTail = PointOnCircle(cx, cy, radius * 0.17f, secondAngle + 180f);

            using (Pen shadowPen = new Pen(Color.FromArgb(IsLight ? 24 : 90, 0, 0, 0), radius * 0.042f))
            {
                shadowPen.StartCap = LineCap.Round;
                shadowPen.EndCap = LineCap.Round;
                PointF hourShadow = new PointF(hourEnd.X + 2f * u, hourEnd.Y + 3f * u);
                PointF minuteShadow = new PointF(minuteEnd.X + 2f * u, minuteEnd.Y + 3f * u);
                g.DrawLine(shadowPen, cx + 2f * u, cy + 3f * u, hourShadow.X, hourShadow.Y);
                g.DrawLine(shadowPen, cx + 2f * u, cy + 3f * u, minuteShadow.X, minuteShadow.Y);
            }

            using (Pen hourPen = new Pen(palette.Text, Math.Max(4f, radius * 0.032f)))
            {
                hourPen.StartCap = LineCap.Round;
                hourPen.EndCap = LineCap.Round;
                g.DrawLine(hourPen, cx, cy, hourEnd.X, hourEnd.Y);
            }
            using (Pen minutePen = new Pen(palette.Text, Math.Max(2.5f, radius * 0.021f)))
            {
                minutePen.StartCap = LineCap.Round;
                minutePen.EndCap = LineCap.Round;
                g.DrawLine(minutePen, cx, cy, minuteEnd.X, minuteEnd.Y);
            }
            using (Pen secondPen = new Pen(palette.Second, Math.Max(1.4f, radius * 0.010f)))
            {
                secondPen.StartCap = LineCap.Round;
                secondPen.EndCap = LineCap.Round;
                g.DrawLine(secondPen, secondTail.X, secondTail.Y, secondEnd.X, secondEnd.Y);
            }
            using (SolidBrush capShadow = new SolidBrush(Color.FromArgb(70, 0, 0, 0)))
            {
                g.FillEllipse(
                    capShadow,
                    cx - 6.2f * u + 2f * u,
                    cy - 6.2f * u + 3f * u,
                    12.4f * u,
                    12.4f * u);
            }
            using (SolidBrush capOuter = new SolidBrush(palette.Text))
            {
                float cap = 6.2f * u;
                g.FillEllipse(capOuter, cx - cap, cy - cap, cap * 2f, cap * 2f);
            }
            using (SolidBrush capInner = new SolidBrush(palette.Accent))
            {
                float cap = 3.1f * u;
                g.FillEllipse(capInner, cx - cap, cy - cap, cap * 2f, cap * 2f);
            }
        }

        private void DrawCardinal(
            Graphics g, string text, Font font, float x, float y,
            StringFormat format, bool accent)
        {
            using (SolidBrush brush = new SolidBrush(accent ? palette.Accent : palette.Muted))
            {
                g.DrawString(text, font, brush, x, y, format);
            }
        }

        private static PointF PointOnCircle(float cx, float cy, float radius, float degrees)
        {
            double angle = (degrees - 90.0) * Math.PI / 180.0;
            return new PointF(
                cx + (float)Math.Cos(angle) * radius,
                cy + (float)Math.Sin(angle) * radius);
        }

        private void PaintDigital(Graphics g, RectangleF area, DateTime now, float u)
        {
            if (area.Width < 40f || area.Height < 40f)
            {
                return;
            }

            using (StringFormat typographic = DrawingHelpers.Typographic())
            using (Font labelFont = new Font(
                "Microsoft YaHei UI", fullscreen ? 11.5f : 10f, FontStyle.Regular, GraphicsUnit.Point))
            using (Font periodFont = new Font(
                "Microsoft YaHei UI", fullscreen ? 10f : 9f, FontStyle.Regular, GraphicsUnit.Point))
            using (SolidBrush accentBrush = new SolidBrush(palette.Accent))
            using (SolidBrush mutedBrush = new SolidBrush(palette.Muted))
            using (SolidBrush textBrush = new SolidBrush(palette.Text))
            using (SolidBrush faintBrush = new SolidBrush(palette.Faint))
            using (SolidBrush secondBrush = new SolidBrush(palette.Second))
            {
                float labelY = fullscreen
                    ? area.Top + Math.Max(0f, (area.Height - 340f * u) / 2f)
                    : area.Top + 2f * u;
                g.DrawString("现在", labelFont, accentBrush, area.Left, labelY, typographic);
                float labelWidth = g.MeasureString("现在", labelFont, PointF.Empty, typographic).Width;
                using (Pen linePen = new Pen(Color.FromArgb(120, palette.Accent), 1f * u))
                {
                    g.DrawLine(
                        linePen,
                        area.Left + labelWidth + 12f * u,
                        labelY + 7.5f * u,
                        area.Left + labelWidth + 35f * u,
                        labelY + 7.5f * u);
                }
                g.DrawString(
                    ClockText.TimeOfDay(now),
                    periodFont,
                    mutedBrush,
                    area.Left + labelWidth + 43f * u,
                    labelY + 0.5f * u,
                    typographic);

                using (Font zoneFont = new Font("Microsoft YaHei UI", 8.5f, FontStyle.Regular, GraphicsUnit.Point))
                {
                    string offset = ClockText.UtcOffset();
                    SizeF offsetSize = g.MeasureString(offset, zoneFont, PointF.Empty, typographic);
                    g.DrawString(
                        offset,
                        zoneFont,
                        faintBrush,
                        area.Right - offsetSize.Width,
                        labelY + 1.5f * u,
                        typographic);
                }

                string mainTime = use24Hour
                    ? now.ToString("HH:mm", CultureInfo.InvariantCulture)
                    : now.ToString("h:mm", CultureInfo.InvariantCulture);
                float maxWidth = Math.Max(80f, area.Width - 92f * u);
                Font timeFont = null;
                float timePoint = fullscreen ? 150f : 112f;
                float minimumPoint = fullscreen ? 64f : 54f;
                while (timePoint >= minimumPoint)
                {
                    Font candidate = new Font(
                        "Segoe UI Light", timePoint, FontStyle.Regular, GraphicsUnit.Point);
                    SizeF candidateSize = g.MeasureString(
                        mainTime, candidate, PointF.Empty, typographic);
                    if (candidateSize.Width <= maxWidth)
                    {
                        timeFont = candidate;
                        break;
                    }
                    candidate.Dispose();
                    timePoint -= 2f;
                }
                if (timeFont == null)
                {
                    timeFont = new Font("Segoe UI Light", 52f, FontStyle.Regular, GraphicsUnit.Point);
                }

                float timeY = labelY + 34f * u;
                SizeF timeSize = g.MeasureString(mainTime, timeFont, PointF.Empty, typographic);
                g.DrawString(mainTime, timeFont, textBrush, area.Left, timeY, typographic);

                using (Font secondsFont = new Font(
                    "Segoe UI Light", Math.Max(18f, timePoint * 0.29f), FontStyle.Regular, GraphicsUnit.Point))
                using (Font secondsUnit = new Font(
                    "Microsoft YaHei UI", 8.5f, FontStyle.Regular, GraphicsUnit.Point))
                {
                    string seconds = now.ToString("ss", CultureInfo.InvariantCulture);
                    SizeF secondsSize = g.MeasureString(seconds, secondsFont, PointF.Empty, typographic);
                    float secondsX = area.Left + timeSize.Width + 9f * u;
                    float secondsY = timeY + timeSize.Height - secondsSize.Height - 5f * u;
                    if (secondsX + secondsSize.Width > area.Right)
                    {
                        secondsX = area.Right - secondsSize.Width;
                    }
                    g.DrawString(seconds, secondsFont, secondBrush, secondsX, secondsY, typographic);
                    if (use24Hour)
                    {
                        g.DrawString(
                            "秒",
                            secondsUnit,
                            mutedBrush,
                            secondsX + secondsSize.Width - 18f * u,
                            secondsY + secondsSize.Height - 2f * u,
                            typographic);
                    }
                    else
                    {
                        g.DrawString(
                            now.ToString("tt", CultureInfo.CurrentCulture),
                            secondsUnit,
                            mutedBrush,
                            secondsX + secondsSize.Width - 20f * u,
                            secondsY + secondsSize.Height - 2f * u,
                            typographic);
                    }
                }

                float dateY = timeY + timeSize.Height + 14f * u;
                using (Font dateFont = new Font(
                    "Microsoft YaHei UI", fullscreen ? 28f : 21f, FontStyle.Regular, GraphicsUnit.Point))
                using (Font pillFont = new Font(
                    "Microsoft YaHei UI", fullscreen ? 11.5f : 10f, FontStyle.Regular, GraphicsUnit.Point))
                {
                    string dateText = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}年{1}月{2}日",
                        now.Year, now.Month, now.Day);
                    g.DrawString(dateText, dateFont, textBrush, area.Left, dateY, typographic);
                    SizeF dateSize = g.MeasureString(dateText, dateFont, PointF.Empty, typographic);

                    string weekday = ClockText.Weekday(now);
                    SizeF weekdaySize = g.MeasureString(weekday, pillFont, PointF.Empty, typographic);
                    float pillWidth = weekdaySize.Width + 20f * u;
                    float pillHeight = (fullscreen ? 31f : 27f) * u;
                    float pillX = area.Left + dateSize.Width + 16f * u;
                    float pillY = dateY + 5f * u;
                    if (pillX + pillWidth <= area.Right)
                    {
                        RectangleF pill = new RectangleF(pillX, pillY, pillWidth, pillHeight);
                        using (GraphicsPath pillPath = DrawingHelpers.RoundedRect(pill, pillHeight / 2f))
                        using (SolidBrush pillFill = new SolidBrush(palette.AccentSoft))
                        using (Pen pillBorder = new Pen(Color.FromArgb(IsLight ? 70 : 85, palette.Accent), 1f * u))
                        {
                            g.FillPath(pillFill, pillPath);
                            g.DrawPath(pillBorder, pillPath);
                        }
                        using (StringFormat centered = new StringFormat())
                        {
                            centered.Alignment = StringAlignment.Center;
                            centered.LineAlignment = StringAlignment.Center;
                            g.DrawString(weekday, pillFont, accentBrush, pill, centered);
                        }
                    }
                    else
                    {
                        g.DrawString(
                            weekday,
                            pillFont,
                            accentBrush,
                            area.Left,
                            dateY + dateSize.Height + 4f * u,
                            typographic);
                    }

                    string lunar = ClockText.Lunar(now);
                    if (!string.IsNullOrEmpty(lunar))
                    {
                        using (Font lunarFont = new Font(
                            "Microsoft YaHei UI", fullscreen ? 12f : 10.5f, FontStyle.Regular, GraphicsUnit.Point))
                        using (SolidBrush lunarBrush = new SolidBrush(palette.Muted))
                        {
                            g.DrawString(
                                lunar,
                                lunarFont,
                                lunarBrush,
                                area.Left,
                                dateY + dateSize.Height + 9f * u,
                                typographic);
                        }
                    }
                }

                if (!fullscreen)
                {
                    float metaLineY = area.Bottom - 92f * u;
                    using (Pen metaLine = new Pen(Color.FromArgb(IsLight ? 38 : 42, palette.Text), 1f))
                    {
                        g.DrawLine(metaLine, area.Left, metaLineY, area.Right, metaLineY);
                    }

                    double dayFraction = (
                        now.Hour * 3600.0 + now.Minute * 60.0 + now.Second + now.Millisecond / 1000.0
                    ) / 86400.0;
                    double displayPercent = Math.Floor(dayFraction * 1000.0) / 10.0;

                    float progressLabelY = area.Bottom - 64f * u;
                    using (Font progressFont = new Font(
                        "Microsoft YaHei UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point))
                    using (Font progressValueFont = new Font(
                        "Segoe UI Semibold", 10f, FontStyle.Regular, GraphicsUnit.Point))
                    {
                        g.DrawString("今日已过", progressFont, mutedBrush, area.Left, progressLabelY, typographic);
                        string percent = displayPercent.ToString("0.0", CultureInfo.InvariantCulture) + "%";
                        SizeF percentSize = g.MeasureString(
                            percent, progressValueFont, PointF.Empty, typographic);
                        g.DrawString(
                            percent,
                            progressValueFont,
                            accentBrush,
                            area.Right - percentSize.Width,
                            progressLabelY - 1f * u,
                            typographic);
                    }

                    float barY = area.Bottom - 37f * u;
                    float barHeight = 4f * u;
                    RectangleF track = new RectangleF(area.Left, barY, area.Width, barHeight);
                    using (GraphicsPath trackPath = DrawingHelpers.RoundedRect(track, barHeight / 2f))
                    using (SolidBrush trackBrush = new SolidBrush(palette.Track))
                    {
                        g.FillPath(trackBrush, trackPath);
                    }
                    float fillWidth = (float)(area.Width * dayFraction);
                    if (fillWidth > 0f)
                    {
                        RectangleF fill = new RectangleF(area.Left, barY, Math.Max(barHeight, fillWidth), barHeight);
                        using (GraphicsPath fillPath = DrawingHelpers.RoundedRect(fill, barHeight / 2f))
                        using (SolidBrush fillBrush = new SolidBrush(palette.Accent))
                        {
                            g.FillPath(fillBrush, fillPath);
                        }
                        using (SolidBrush dotBrush = new SolidBrush(palette.Accent))
                        {
                            float dot = 3.2f * u;
                            g.FillEllipse(
                                dotBrush,
                                area.Left + fillWidth - dot,
                                barY + barHeight / 2f - dot,
                                dot * 2f,
                                dot * 2f);
                        }
                    }
                }

                timeFont.Dispose();
            }
        }

        private void PaintFooter(Graphics g, float w, float h, float u, DateTime now)
        {
            float pad = 34f * u;
            using (Font footerFont = new Font("Microsoft YaHei UI", 8.5f, FontStyle.Regular, GraphicsUnit.Point))
            using (SolidBrush footerBrush = new SolidBrush(palette.Faint))
            using (StringFormat typographic = DrawingHelpers.Typographic())
            {
                if (fullscreen)
                {
                    string fullscreenStats = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} 年  ·  第 {1} 天  ·  第 {2} 周",
                        now.Year,
                        now.DayOfYear,
                        ClockText.IsoWeek(now));
                    g.DrawString(fullscreenStats, footerFont, footerBrush, pad, h - 34f * u, typographic);
                    string hint = "Esc  退出全屏";
                    SizeF hintSize = g.MeasureString(hint, footerFont, PointF.Empty, typographic);
                    g.DrawString(hint, footerFont, footerBrush, w - pad - hintSize.Width, h - 34f * u, typographic);
                    return;
                }

                g.DrawString(
                    "F11 全屏    ·    T 主题    ·    H 12/24 小时    ·    P 置顶    ·    Ctrl+C 复制",
                    footerFont,
                    footerBrush,
                    pad,
                    h - 36f * u,
                    typographic);
                string stats = string.Format(
                    CultureInfo.InvariantCulture,
                    "第 {0} 天  ·  第 {1} 周",
                    now.DayOfYear,
                    ClockText.IsoWeek(now));
                SizeF statsSize = g.MeasureString(stats, footerFont, PointF.Empty, typographic);
                g.DrawString(stats, footerFont, footerBrush, w - pad - statsSize.Width, h - 36f * u, typographic);
            }
        }

        private void PaintToast(Graphics g, float w, float h, float u)
        {
            using (Font font = new Font("Microsoft YaHei UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point))
            using (StringFormat typographic = DrawingHelpers.Typographic())
            {
                SizeF textSize = g.MeasureString(toastText, font, PointF.Empty, typographic);
                float width = textSize.Width + 32f * u;
                float height = 34f * u;
                RectangleF rect = new RectangleF(
                    (w - width) / 2f,
                    h - (fullscreen ? 60f : 88f) * u,
                    width,
                    height);
                using (GraphicsPath path = DrawingHelpers.RoundedRect(rect, height / 2f))
                using (SolidBrush background = new SolidBrush(Color.FromArgb(240, palette.Text)))
                using (SolidBrush textBrush = new SolidBrush(palette.BgTop))
                using (StringFormat centered = new StringFormat())
                {
                    centered.Alignment = StringAlignment.Center;
                    centered.LineAlignment = StringAlignment.Center;
                    g.FillPath(background, path);
                    g.DrawString(toastText, font, textBrush, rect, centered);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (draggingWindow)
            {
                Point current = Cursor.Position;
                int dx = current.X - dragCursorStart.X;
                int dy = current.Y - dragCursorStart.Y;
                if (!dragMoved && (Math.Abs(dx) > 2 || Math.Abs(dy) > 2))
                {
                    dragMoved = true;
                }
                if (dragMoved)
                {
                    host.Location = new Point(dragWindowStart.X + dx, dragWindowStart.Y + dy);
                }
                return;
            }

            int hit = HitButton(e.Location);
            if (hit != hoverButton)
            {
                hoverButton = hit;
                if (hit >= 0)
                {
                    toolTip.Show(ButtonHint(hit), this, (int)buttonRects[hit].Left - 18, (int)buttonRects[hit].Bottom + 8, 2600);
                }
                else
                {
                    toolTip.Hide(this);
                }
                Invalidate();
            }

            if (hit >= 0)
            {
                Cursor = Cursors.Hand;
            }
            else if (!fullscreen && e.Y <= 84f * uiScale && !IsOverTopButton(e.Location))
            {
                Cursor = Cursors.SizeAll;
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            int hit = HitButton(e.Location);
            if (hit >= 0)
            {
                HandleButton(hit);
                return;
            }

            if (fullscreen)
            {
                if (e.Clicks >= 2)
                {
                    host.ToggleFullscreen();
                }
                return;
            }

            if (e.Y <= 84f * uiScale)
            {
                if (e.Clicks >= 2)
                {
                    host.ToggleMaximize();
                    return;
                }
                if (host.WindowState == FormWindowState.Normal)
                {
                    draggingWindow = true;
                    dragMoved = false;
                    dragCursorStart = Cursor.Position;
                    dragWindowStart = host.Location;
                    Capture = true;
                    Cursor = Cursors.SizeAll;
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left && draggingWindow)
            {
                draggingWindow = false;
                dragMoved = false;
                Capture = false;
                Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (!draggingWindow && hoverButton >= 0)
            {
                hoverButton = -1;
                toolTip.Hide(this);
                Invalidate();
            }
            if (!draggingWindow)
            {
                Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            base.OnMouseCaptureChanged(e);
            if (!Capture && draggingWindow)
            {
                draggingWindow = false;
                dragMoved = false;
            }
        }

        protected override void OnContextMenuStripChanged(EventArgs e)
        {
            base.OnContextMenuStripChanged(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Right)
            {
                host.ShowContextMenu(e.Location);
            }
        }

        private bool IsOverTopButton(Point point)
        {
            return HitButton(point) >= 0;
        }

        private int HitButton(Point point)
        {
            for (int i = 0; i < buttonRects.Length; i++)
            {
                if (!buttonRects[i].IsEmpty && buttonRects[i].Contains(point))
                {
                    return i;
                }
            }
            return -1;
        }

        private string ButtonHint(int index)
        {
            if (index == ButtonClose) return "关闭";
            if (index == ButtonMaximize) return "最大化 / 还原";
            if (index == ButtonMinimize) return "最小化";
            if (index == ButtonPin) return "窗口置顶  ·  P";
            return "切换主题  ·  T";
        }

        private void HandleButton(int index)
        {
            toolTip.Hide(this);
            if (index == ButtonClose)
            {
                host.Close();
                return;
            }
            else if (index == ButtonMaximize)
            {
                host.ToggleMaximize();
            }
            else if (index == ButtonMinimize)
            {
                host.WindowState = FormWindowState.Minimized;
            }
            else if (index == ButtonPin)
            {
                host.ToggleTopMost();
            }
            else if (index == ButtonTheme)
            {
                host.ToggleTheme();
            }
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                themeTimer.Dispose();
                toastTimer.Dispose();
                toolTip.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string previewPath = null;
            bool previewLight = false;
            bool previewTopMost = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (string.Equals(args[i], "--preview", StringComparison.OrdinalIgnoreCase)
                    && i + 1 < args.Length)
                {
                    previewPath = args[i + 1];
                    i++;
                }
                else if (string.Equals(args[i], "--light", StringComparison.OrdinalIgnoreCase))
                {
                    previewLight = true;
                }
                else if (string.Equals(args[i], "--topmost", StringComparison.OrdinalIgnoreCase))
                {
                    previewTopMost = true;
                }
            }

            AppSettings settings = AppSettings.Load();
            if (previewPath != null)
            {
                settings.Bounds = Rectangle.Empty;
                settings.Theme = previewLight ? ThemeKind.Light : ThemeKind.Dark;
                settings.AlwaysOnTop = previewTopMost;
                settings.Use24Hour = true;

                using (ClockForm preview = new ClockForm(settings))
                {
                    preview.PreviewMode = true;
                    preview.StartPosition = FormStartPosition.Manual;
                    preview.Location = new Point(24, 24);
                    preview.ClientSize = new Size(1120, 680);
                    preview.Show();
                    preview.Refresh();
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(180);
                    Application.DoEvents();
                    using (Bitmap bitmap = new Bitmap(preview.Width, preview.Height))
                    {
                        preview.DrawToBitmap(
                            bitmap,
                            new Rectangle(0, 0, preview.Width, preview.Height));
                        bitmap.Save(previewPath, ImageFormat.Png);
                    }
                    preview.Close();
                }
                return;
            }

            Application.Run(new ClockForm(settings));
        }
    }
}
