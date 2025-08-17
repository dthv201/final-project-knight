using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class BuildLogUsedAssetsExporter
{
    // מיקום ה-Editor.log לפי מערכת הפעלה
    static string GetEditorLogPath()
    {
#if UNITY_EDITOR_WIN
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(localAppData, "Unity", "Editor", "Editor.log");
#else
        // macOS
        string home = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        return Path.Combine(home, "Library", "Logs", "Unity", "Editor.log");
#endif
    }

    [MenuItem("Tools/Build Report/Export Used Assets CSV")]
    public static void ExportUsedAssetsCsv()
    {
        string logPath = GetEditorLogPath();
        if (!File.Exists(logPath))
        {
            EditorUtility.DisplayDialog("Build Report", "לא נמצא Editor.log. תריץ Build קודם ואז נסה שוב.", "OK");
            return;
        }

       string[] lines;
    using (var fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    using (var sr = new StreamReader(fs))
    {
        var list = new List<string>();
        while (!sr.EndOfStream)
            list.Add(sr.ReadLine());
        lines = list.ToArray();
    }

        const string header = "Used Assets and files from the Resources folder, sorted by uncompressed size:";
        int start = Array.FindIndex(lines, l => l.StartsWith(header));
        if (start < 0)
        {
            EditorUtility.DisplayDialog("Build Report", "לא נמצא מקטע Used Assets בלוג. ודא שסיימת Build מלא.", "OK");
            return;
        }

        // איסוף שורות עד קטע ריק/כותרת חדשה
        var used = new List<(long sizeBytes, string path)>();
        for (int i = start + 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) break;

            // פורמט טיפוסי:  1.2 mb  5.6% Assets/Textures/Stone.tga
            // לפעמים בג׳יבייבטים/קילו. נטפל בכל.
            try
            {
                // פיצול כפול רווחים → הגודל הוא הטוקן הראשון, השאר הסוף הוא הנתיב
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                // הגודל מופיע בשני טוקנים (value + unit)
                double val = double.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
                string unit = parts[1].ToLowerInvariant();
                long mul = unit.StartsWith("kb") ? 1024L :
                           unit.StartsWith("mb") ? 1024L * 1024L :
                           unit.StartsWith("gb") ? 1024L * 1024L * 1024L : 1L;
                long bytes = (long)(val * mul);

                // הנתיב מתחיל אחרי האחוזים → נמצא ה-index של הטוקן שמתחיל ב"Assets/"
                int assetIdx = Array.FindIndex(parts, p => p.StartsWith("Assets/") || p.StartsWith("Packages/") || p.StartsWith("Library/") || p.StartsWith("Resources/") || p.StartsWith("StreamingAssets/"));
                if (assetIdx < 0) continue;
                string path = string.Join(" ", parts.Skip(assetIdx));

                used.Add((bytes, path));
            }
            catch { /* דלג על שורה לא צפויה */ }
        }

        if (used.Count == 0)
        {
            EditorUtility.DisplayDialog("Build Report", "לא אותרו נכסים במקטע. ייתכן שהפורמט בלוג שונה בגרסה שלך.", "OK");
            return;
        }

        string outDir = "BuildReports";
        Directory.CreateDirectory(outDir);
        string csvPath = Path.Combine(outDir, $"UsedAssets_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

        var sb = new StringBuilder();
        sb.AppendLine("SizeBytes,SizeReadable,Path");
        foreach (var u in used.OrderByDescending(u => u.sizeBytes))
            sb.AppendLine($"{u.sizeBytes},{FormatBytes(u.sizeBytes)},{u.path.Replace(',', ' ')}");

        File.WriteAllText(csvPath, sb.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh();

        EditorUtility.RevealInFinder(csvPath);
        EditorUtility.DisplayDialog("Build Report", $"נוצר קובץ:\n{csvPath}\n\nאלה הקבצים שנכנסו לבילד.", "OK");
    }

    static string FormatBytes(long b)
    {
        double d = b;
        string[] u = { "B", "KB", "MB", "GB" };
        int i = 0;
        while (d >= 1024 && i < u.Length - 1) { d /= 1024; i++; }
        return $"{d:0.##} {u[i]}";
    }
}
