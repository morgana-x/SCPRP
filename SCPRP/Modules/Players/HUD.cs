using SCPRP.Extensions;
using System;

namespace SCPRP.Modules.Players
{
    internal class HUD : BaseModule
    {
        RueI.API.Elements.DynamicElement MainTag;
        public override void Load()
        {
            MainTag = new RueI.API.Elements.DynamicElement(14f, (x) =>
            {
                LabApi.Features.Wrappers.Player p = LabApi.Features.Wrappers.Player.Get(x);
                if (p == null) return "";
                return $"         <align=left><color={p.GetJobInfo().HexColour()}>{p.GetJobInfo().Name}</color>   <color=#55ff55>${p.GetMoney()}</color></align>";
            });
        }

        DateTime nextHUD = DateTime.Now;


        public override void Tick()
        {
            if (DateTime.Now < nextHUD) { return;  }
            nextHUD = DateTime.Now.AddSeconds(0.5f);
            foreach (var p in LabApi.Features.Wrappers.Player.GetAll())
            {
                if (p == null) continue;
                if (!p.IsReady) continue;
                if (p.IsDummy) continue;
                try
                {
                    RueI.API.RueDisplay.Get(p).Show(MainTag, 0.6f);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public static void ShowHint(LabApi.Features.Wrappers.Player pl, string text, float duration = 5f)
        {
            var moneytag = new RueI.API.Elements.BasicElement(100f,text);
            RueI.API.RueDisplay.Get(pl).Show(moneytag, duration);
        }

        public override void Unload()
        {
            
        }
    }
}
