using HarmonyLib;

namespace ONIHeater
{
  [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
  internal class Patch
  {
    private static void Prefix()
    {
      ModUtil.AddBuildingToPlanScreen("Utilities", DirectHeaterConfig.ID);
    }

  }
  [HarmonyPatch(typeof(Db))]
  [HarmonyPatch("Initialize")]
  public class Db_Initialize_Patch
  {
    public static void Postfix()
    {
      Utils.AddBuildingToTechnology("TemperatureModulation", DirectHeaterConfig.ID);
    }
  }

  [HarmonyPatch(typeof(Localization), "Initialize")]
  public class StringLocalisationPatch
  {
    public static void Postfix()
    {
      Utils.Localize(typeof(ONIHeater.STRINGS));
    }
  }
}
