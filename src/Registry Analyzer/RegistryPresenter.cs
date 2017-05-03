using static Registry_Analyzer.RegistryModel;

namespace Registry_Analyzer
{
    class RegistryPresenter
    {
        private readonly RegistryModel model = new RegistryModel();
        private readonly RegistryView view;
        private string lastTerm = string.Empty;

        internal RegistryPresenter(RegistryView view)
        {
            this.view = view;
            
            view.ButtonSearchClicked += UpdateView;
            view.UnregistryClicked += Unregistry;
        }

        private void UpdateView(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return;
            }

            view.ClearGrid();

            model.Search(term).ForEach(view.AddRow);

            lastTerm = term;
        }

        private void Unregistry(RegistryEntry entry)
        {
            model.Unregistry(entry);

            UpdateView(lastTerm);
        }
    }
}
