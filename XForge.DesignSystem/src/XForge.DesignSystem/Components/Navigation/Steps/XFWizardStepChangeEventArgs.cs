namespace XForge.DesignSystem.Components.Navigation.Steps
{
    public sealed class XFWizardStepChangeEventArgs
    {
        public XFWizardStepChangeEventArgs(string currentId, string targetId, int currentIndex, int targetIndex)
        {
            CurrentId = currentId;
            TargetId = targetId;
            CurrentIndex = currentIndex;
            TargetIndex = targetIndex;
        }

        public string CurrentId { get; }

        public string TargetId { get; }

        public int CurrentIndex { get; }

        public int TargetIndex { get; }

        public bool Cancel { get; set; }
    }
}
