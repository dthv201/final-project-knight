using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class FindUnusedAssets
{
    [MenuItem("Tools/Build Report/List Possibly Unused Assets")]
    public static void ListPossiblyUnused()
    {
        // 1) טען את רשימת הקבצים שהיו בשימוש (CSV האחרון בתיקיית BuildReports)
        string reportsDir = Path.Combine(Application.dataPath, "../BuildReports");
        if (!Directory.Exists(reportsDir))
        {
            EditorUtility.DisplayDialog("Unused Assets", "לא נמצאה תיקיית BuildReports. תריץ קודם Export Used Assets CSV.", "OK");
            return;
        }

        var latestCsv = Directory.GetFiles(reportsDir, "UsedAssets_*.csv")
                                 .OrderByDescending(f => f)
                                 .FirstOrDefault();
        if (latestCsv == null)
        {
            EditorUtility.DisplayDialog("Unused Assets", "לא נמצא CSV. תריץ קודם Export Used Assets CSV.", "OK");
            return;
        }

        var usedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in File.ReadAllLines(latestCsv).Skip(1))
        {
            // עמודה שלישית היא המסלול
            var firstComma = line.IndexOf(',');
            if (firstComma < 0) continue;
            var secondComma = line.IndexOf(',', firstComma + 1);
            if (secondComma < 0) continue;
            string path = line.Substring(secondComma + 1).Trim();
            // ננרמל ל-Assets/...
            if (!path.StartsWith("Assets/")) continue;
            usedPaths.Add(path.Replace('\\', '/'));
        }

        // 2) אסוף את כל ה-Assets בפרויקט
        var all = AssetDatabase.GetAllAssetPaths()
                               .Where(p => p.StartsWith("Assets/"))
                               .Where(p => !AssetDatabase.IsValidFolder(p))
                               .Where(p => !p.Contains("/Editor/"))         // לא לגעת בסקריפטים/כלים של Editor
                               .Where(p => !p.Contains("/Gizmos/"))
                               .Where(p => !p.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)) // סקריפטים לא יופיעו בבילד אבל חשובים
                               .Where(p => !p.EndsWith(".asmdef", StringComparison.OrdinalIgnoreCase))
                               .Where(p => !p.EndsWith(".cginc", StringComparison.OrdinalIgnoreCase))
                               .Where(p => !p.EndsWith(".shadergraph", StringComparison.OrdinalIgnoreCase))
                               .ToList();

        // 3) סנן את מה שמשתמשים בו
        var candidates = all.Where(p => !usedPaths.Contains(p)).ToList();

        // 4) שמירה לקובץ טקסט + פתיחה
        string outDir = Path.Combine(Application.dataPath, "../BuildReports");
        Directory.CreateDirectory(outDir);
        string outPath = Path.Combine(outDir, $"PossiblyUnused_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

        File.WriteAllLines(outPath, new[]{
            "Possibly Unused Assets (השוואה ל-UsedAssets CSV האחרון):",
            "— שים לב: זו רשימת מועמדים בלבד. בדוק ידנית לפני מחיקה.",
            "",
        }.Concat(candidates));

        AssetDatabase.Refresh();
        EditorUtility.RevealInFinder(outPath);
        EditorUtility.DisplayDialog("Unused Assets", $"נוצר קובץ:\n{outPath}\n\nאלו קבצים שנראים לא בשימוש.", "OK");
    }
}
