using TUNING;
using UnityEngine;

namespace ONIHeater
{
  public class DirectHeaterConfig : IBuildingConfig
  {
    public const string ID = "DirectHeater";

    public override BuildingDef CreateBuildingDef()
    {
      BuildingDef obj = BuildingTemplates.CreateBuildingDef(
        ID, 4, 1,
        "bolier_dry_kanim", 30, 30f, 
        BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 
        1600f, 
        BuildLocationRule.Anywhere, 
        noise: NOISE_POLLUTION.NOISY.TIER2, 
        decor: BUILDINGS.DECOR.BONUS.TIER1
       );
      obj.RequiresPowerInput = true;
      obj.EnergyConsumptionWhenActive = 1200f;
      obj.ExhaustKilowattsWhenActive = 16f;
      obj.SelfHeatKilowattsWhenActive = 4000f;
      obj.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 0));
      obj.ViewMode = OverlayModes.Power.ID;
      obj.AudioCategory = "SolidMetal";
      obj.OverheatTemperature = 9000.00f;
      obj.Floodable = false;
      obj.Overheatable = false;
      return obj;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
      go.AddOrGet<LoopingSounds>();
      go.AddOrGet<DirectHeater>();
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
      go.AddOrGet<LogicOperationalController>();
      go.AddOrGetDef<PoweredActiveController.Def>();
    }
  }
}
