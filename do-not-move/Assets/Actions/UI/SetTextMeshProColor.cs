using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets textmeshpro text color")]
    public class SetTextMeshProColor : FsmStateAction
    {
        public FsmColor color;
        private TextMeshPro mesh;
        public FsmGameObject textMeshPro;

        public override void Awake()
        {
            mesh = textMeshPro.Value.GetComponent<TextMeshPro>();
        }

        // Code that runs on entering the state.
        public override void OnEnter()
        {
            SetColor();
        }

        // Code that runs every frame.
        public override void OnUpdate()
        {
            SetColor();
        }

        private void SetColor()
        {
            mesh.color = color.Value;
        }
    }
}