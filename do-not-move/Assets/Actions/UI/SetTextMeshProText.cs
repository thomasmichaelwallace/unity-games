using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets textmeshpro text")]
    public class SetTextMeshProText : FsmStateAction
    {
        private TextMeshPro mesh;
        public FsmString text;
        public FsmGameObject textMeshPro;

        public override void Awake()
        {
            mesh = textMeshPro.Value.GetComponent<TextMeshPro>();
        }

        // Code that runs on entering the state.
        public override void OnEnter()
        {
            SetText();
        }

        // Code that runs every frame.
        public override void OnUpdate()
        {
            SetText();
        }

        private void SetText()
        {
            mesh.text = text.Value;
        }
    }
}