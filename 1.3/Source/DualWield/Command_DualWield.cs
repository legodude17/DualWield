using UnityEngine;
using Verse;

namespace DualWield
{
    public class Command_DualWield : Command_VerbTarget
    {
        private readonly Thing offHandThing;
        private readonly Verb offHandVerb;

        public Command_DualWield(Thing offHandThing)
        {
            this.offHandThing = offHandThing;
            if (this.offHandThing.TryGetComp<CompEquippable>() is CompEquippable ce) offHandVerb = ce.PrimaryVerb;
        }

        public override float GetWidth(float maxWidth) => base.GetWidth(maxWidth);

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            var res = base.GizmoOnGUI(topLeft, maxWidth, parms);
            GUI.color = offHandThing.DrawColor;
            var material = !disabled ? null : TexUI.GrayscaleGUI;
            var tex = offHandThing.def.uiIcon;
            if (tex == null) tex = BaseContent.BadTex;
            var rect = new Rect(topLeft.x, topLeft.y + 10, GetWidth(maxWidth), 75f);
            Widgets.DrawTextureFitted(rect, tex, iconDrawScale * 0.85f, iconProportions, iconTexCoords, iconAngle, material);
            GUI.color = Color.white;
            return res;
        }

        public override void GizmoUpdateOnMouseover()
        {
            base.GizmoUpdateOnMouseover();
            offHandVerb.verbProps.DrawRadiusRing(offHandVerb.caster.Position);
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            if (!offHandVerb.IsMeleeAttack && offHandVerb.verbProps.range > verb.verbProps.range)
            {
                var targeter = Find.Targeter;
                if (offHandVerb.CasterIsPawn && targeter.targetingSource != null && targeter.targetingSource.GetVerb.verbProps == offHandVerb.verbProps)
                {
                    var casterPawn = offHandVerb.CasterPawn;
                    if (!targeter.IsPawnTargeting(casterPawn)) targeter.targetingSourceAdditionalPawns.Add(casterPawn);
                }
                else
                    Find.Targeter.BeginTargeting(offHandVerb);
            }
        }
    }
}