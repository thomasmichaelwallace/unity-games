namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Logic)]
    [Tooltip("Tests if float within a range")]
    public class FloatWithin : FsmStateAction
    {
        public FsmBool everyFrame;
        public FsmFloat lowerBound;
        public FsmEvent sendEvent;
        public FsmFloat upperBound;
        public FsmFloat value;

        // Code that runs on entering the state.
        public override void OnEnter()
        {
            TestInRange();
        }

        // Code that runs every frame.
        public override void OnUpdate()
        {
            if (!everyFrame.Value) return;
            TestInRange();
        }

        private void TestInRange()
        {
            var f = value.Value;
            if (f > lowerBound.Value && f < upperBound.Value) Fsm.Event(sendEvent);
        }
    }
}