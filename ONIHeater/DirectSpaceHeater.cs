using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace ONIHeater
{
  [SerializationConfig(MemberSerialization.OptIn)]
  public class DirectHeater : StateMachineComponent<DirectHeater.StatesInstance>, IGameObjectEffectDescriptor
  {
    public class StatesInstance : GameStateMachine<States, StatesInstance, DirectHeater, object>.GameInstance
    {
      public StatesInstance(DirectHeater master) : base(master)
      {
      }
    }

    public class States : GameStateMachine<States, StatesInstance, DirectHeater>
    {
      public class OnlineStates : State
      {
        public State heating;

        public State undermassliquid;

        public State undermassgas;
      }

      public State offline;

      public OnlineStates online;

      private StatusItem statusItemUnderMassLiquid;

      private StatusItem statusItemUnderMassGas;

      public override void InitializeStates(out BaseState default_state)
      {
        default_state = offline;
        serializable = SerializeType.Never;
        statusItemUnderMassLiquid = new StatusItem("statusItemUnderMassLiquid", BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_LIQUID.NAME, BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_LIQUID.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
        statusItemUnderMassGas = new StatusItem("statusItemUnderMassGas", BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_GAS.NAME, BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_GAS.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
        offline.EventTransition(GameHashes.OperationalChanged, online, (StatesInstance smi) => smi.master.operational.IsOperational);
        online.EventTransition(GameHashes.OperationalChanged, offline, (StatesInstance smi) => !smi.master.operational.IsOperational).DefaultState(online.heating).Update("spaceheater_online", delegate (StatesInstance smi, float dt)
        {
          switch (smi.master.MonitorHeating(dt))
          {
            case MonitorState.NotEnoughLiquid:
              smi.GoTo(online.undermassliquid);
              break;
            case MonitorState.NotEnoughGas:
              smi.GoTo(online.undermassgas);
              break;
            case MonitorState.ReadyToHeat:
              smi.GoTo(online.heating);
              break;
          }
        }, UpdateRate.SIM_4000ms);
        online.heating.Enter(delegate (StatesInstance smi)
        {
          smi.master.operational.SetActive(value: true);
        }).Exit(delegate (StatesInstance smi)
        {
          smi.master.operational.SetActive(value: false);
        });
        online.undermassliquid.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, statusItemUnderMassLiquid);
        online.undermassgas.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, statusItemUnderMassGas);
      }
    }

    private enum MonitorState
    {
      ReadyToHeat,
      NotEnoughLiquid,
      NotEnoughGas
    }

    public float minimumCellMass = 10;

    public int radius = 2;

    [MyCmpReq]
    private Operational operational;

    private List<int> monitorCells = new List<int>();

    protected override void OnSpawn()
    {
      base.OnSpawn();
      smi.StartSM();
    }

    private MonitorState MonitorHeating(float dt)
    {
      monitorCells.Clear();
      GameUtil.GetNonSolidCells(Grid.PosToCell(transform.GetPosition()), radius, monitorCells);
      int num = 0;
      for (int i = 0; i < monitorCells.Count; i++)
      {
        if (Grid.Mass[monitorCells[i]] > minimumCellMass)
          num++;
      }
      if (num == 0)
        return MonitorState.NotEnoughGas;
      return MonitorState.ReadyToHeat;
    }

    public List<Descriptor> GetDescriptors(GameObject go)
    {
      return new List<Descriptor>();
    }
  }

}
