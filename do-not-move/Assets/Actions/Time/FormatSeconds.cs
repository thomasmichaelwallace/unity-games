using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Time)]
    [Tooltip("Gets seconds formatted as 00:00:00")]
    public class FormatSeconds : FsmStateAction
    {
        [UIHint(UIHint.Variable)] public FsmString formatted;
        public FsmFloat seconds;

        public override void OnUpdate()
        {
            var v = Mathf.Max(seconds.Value, 0);
            var s = Mathf.FloorToInt(v);
            var ms = Mathf.FloorToInt((v - s) * 1000);
            formatted.Value = $"{s}:{ms}";
        }
    }
}