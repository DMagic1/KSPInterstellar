using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FNPlugin {
	class FNSolarPanelWasteHeatModule : FNResourceSuppliableModule {
		protected float wasteheat_production_f = 0;
		[KSPField(isPersistant = false, guiActive = true, guiName = "Heat Production")]
		public string heatProductionStr = ":";

		public override void OnStart(PartModule.StartState state) {
			String[] resources_to_supply = {FNResourceManager.FNRESOURCE_WASTEHEAT};
			this.resources_to_supply = resources_to_supply;

			base.OnStart (state);

			if (state == StartState.Editor) { return; }
			this.part.force_activate();

			ModuleDeployableSolarPanel solarPanel = (ModuleDeployableSolarPanel)this.part.Modules["ModuleDeployableSolarPanel"];
			if (solarPanel != null) {
				solarPanel.powerCurve = PluginHelper.getSatFloatCurve ();
			}
		}

		public override void OnUpdate() {
			heatProductionStr = wasteheat_production_f.ToString ("0.00") + " KW";
		}

		public override void OnFixedUpdate() {
			base.OnFixedUpdate ();

			ModuleDeployableSolarPanel solarPanel = (ModuleDeployableSolarPanel)this.part.Modules["ModuleDeployableSolarPanel"];
			if (solarPanel != null) {
				float solar_rate = solarPanel.flowRate*TimeWarp.fixedDeltaTime;
				float heat_rate = solar_rate * 0.5f/1000.0f;

				if (getResourceBarRatio (FNResourceManager.FNRESOURCE_WASTEHEAT) >= 0.98 && solarPanel.panelState == ModuleDeployableSolarPanel.panelStates.EXTENDED) {
					solarPanel.Retract ();
					if (FlightGlobals.ActiveVessel == vessel) {
						ScreenMessages.PostScreenMessage ("Warning Dangerous Overheating Detected: Solar Panel retraction occuring NOW!", 5.0f, ScreenMessageStyle.UPPER_CENTER);
					}
					return;
				}

				wasteheat_production_f = supplyFNResource(heat_rate,FNResourceManager.FNRESOURCE_WASTEHEAT)/TimeWarp.fixedDeltaTime*1000.0f;
			}


		}
	}
}
