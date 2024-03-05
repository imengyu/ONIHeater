using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ONIHeater
{
  public static class Utils
  {
    public static void AddBuildingToTechnology(string techId, string buildingId)
    {
      Db.Get().Techs.Get(techId).unlockedItemIDs.Add(buildingId);
    }
    public static void Localize(Type root)
    {
      ModUtil.RegisterForTranslation(root);
      Assembly executingAssembly = Assembly.GetExecutingAssembly();
      string name = executingAssembly.GetName().Name;
      string path = Path.Combine(Path.GetDirectoryName(executingAssembly.Location), "translations");
      Localization.Locale locale = Localization.GetLocale();
      if (locale != null)
      {
        try
        {
          string text = Path.Combine(path, locale.Code + ".po");
          if (File.Exists(text))
          {
            Debug.Log(name + ": Localize file found " + text);
            Localization.OverloadStrings(Localization.LoadStringsFile(text, isTemplate: false));
          }
        }
        catch
        {
          Debug.LogWarning(name + " Failed to load localization.");
        }
      }
      LocString.CreateLocStringKeys(root, "");
    }
  }

}
