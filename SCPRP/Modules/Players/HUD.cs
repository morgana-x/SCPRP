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
                string tag = $"         <align=left><color={p.GetJobInfo().HexColour()}>{p.GetJobInfo().Name}</color>   <color=#55ff55>${p.GetMoney()}</color></align>";
                var moneytag = new RueI.API.Elements.BasicElement(14f, tag);

              //  var bgtag = new RueI.API.Elements.BasicElement(35f,"<align=left><color #44444420><size=50>■</size></color></align>");
                RueI.API.RueDisplay.Get(p).Show(moneytag, 0.7f);
               //RueI.API.RueDisplay.Get(p).Show(bgtag, 0.7f);
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
