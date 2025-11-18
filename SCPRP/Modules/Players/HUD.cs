using SCPRP.Extensions;
using System;

namespace SCPRP.Modules.Players
{
    internal class HUD : BaseModule
    {
        public override void Load()
        {
            
        }

        DateTime nextHUD = DateTime.Now;
        public override void Tick()
        {
            if (DateTime.Now < nextHUD) { return;  }
            nextHUD = DateTime.Now.AddSeconds(0.5f);
            foreach (var p in LabApi.Features.Wrappers.Player.GetAll())
            {
                var moneytag = new RueI.API.Elements.BasicElement(0.5f, $"      <color=#55ff55><align=left>${p.GetMoney()}</align></color>");
                RueI.API.RueDisplay.Get(p).Show(moneytag, 0.7f);
            }
        }

        public static void ShowHint(LabApi.Features.Wrappers.Player pl, string text, float duration = 5f)
        {
            var moneytag = new RueI.API.Elements.BasicElement(5f,text);
            RueI.API.RueDisplay.Get(pl).Show(moneytag, duration);
        }

        public override void Unload()
        {
            
        }
    }
}
