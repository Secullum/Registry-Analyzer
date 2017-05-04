using System.Windows.Forms;
using static Registry_Analyzer.RegistryModel;

namespace Registry_Analyzer
{
    class RegistryPresenter
    {
        private readonly RegistryModel model = new RegistryModel();
        private readonly RegistryView view;

        internal RegistryPresenter(RegistryView view)
        {
            this.view = view;
            
            view.ButtonSearchClicked += UpdateView;
            view.UnregistryClicked += Unregistry;
            view.CopyKeyClicked += CopyKey;
            view.CopyPathClicked += CopyPath;
        }

        private void UpdateView(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return;
            }

            view.ClearGrid();

            model.Search(term).ForEach(view.AddRow);
        }

        private void Unregistry(RegistryEntry entry)
        {
            var response = MessageBox.Show("Desregistrar a entrada selecionada?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (response == DialogResult.Yes)
            {
                model.Unregistry(entry);

                view.RemoveRow(entry);
            }
        }

        private void CopyKey(string key)
        {
            Clipboard.SetText(key);
        }

        private void CopyPath(string path)
        {
            Clipboard.SetText(path);
        }
    }
}
