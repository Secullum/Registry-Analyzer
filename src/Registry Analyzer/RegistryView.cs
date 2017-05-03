using System.Windows.Forms;
using static Registry_Analyzer.RegistryModel;

namespace Registry_Analyzer
{
    class RegistryView
    {
        internal delegate void ButtonSearchClickedEventHandler(string term);

        internal delegate void UnregistryEventHandler(RegistryEntry entry);

        internal event ButtonSearchClickedEventHandler ButtonSearchClicked;

        internal event UnregistryEventHandler UnregistryClicked;

        private readonly FormRegistry form;

        internal RegistryView(FormRegistry form)
        {
            this.form = form;

            Initialize();
        }

        private void Initialize()
        {
            form.ListViewRegistry.Columns.Add("Nome da chave");
            form.ListViewRegistry.Columns.Add("Caminho apontado");
            form.ListViewRegistry.Columns.Add("Arquivo existe");

            AdjustColumnsSize();

            form.ListViewRegistry.Resize += (e, sender) => AdjustColumnsSize();

            form.ButtonSearch.Click += (sender, e) =>
            {
                ButtonSearchClicked?.Invoke(form.TextBoxSearch.Text.Trim());
            };

            form.ListViewRegistry.ContextMenu = new ContextMenu();

            form.ListViewRegistry.ContextMenu.Popup += (sender, e) =>
            {
                var enable = form.ListViewRegistry.SelectedItems.Count > 0;

                for (var i = 0; i < form.ListViewRegistry.ContextMenu.MenuItems.Count; i++)
                {
                    form.ListViewRegistry.ContextMenu.MenuItems[i].Enabled = enable;
                }
            };

            form.ListViewRegistry.ContextMenu.MenuItems.Add("Copiar chave");

            form.ListViewRegistry.ContextMenu.MenuItems[0].Click += (sender, e) =>
            {
                var subItems = form.ListViewRegistry.SelectedItems[0].SubItems;

                Clipboard.SetText(subItems[0].Text);
            };
            
            form.ListViewRegistry.ContextMenu.MenuItems.Add("Copiar caminho");

            form.ListViewRegistry.ContextMenu.MenuItems[1].Click += (sender, e) =>
            {
                var subItems = form.ListViewRegistry.SelectedItems[0].SubItems;

                Clipboard.SetText(subItems[1].Text);
            };

            form.ListViewRegistry.ContextMenu.MenuItems.Add("Desregistrar");

            form.ListViewRegistry.ContextMenu.MenuItems[2].Click += (sender, e) =>
            {
                if (ConfirmUnregistry())
                {
                    var subItems = form.ListViewRegistry.SelectedItems[0].SubItems;

                    UnregistryClicked?.Invoke(new RegistryEntry
                    {
                        Key = subItems[0].Text,
                        Path = subItems[1].Text,
                        Exists = subItems[2].Text == "Sim"
                    });
                }
            };
        }

        private void AdjustColumnsSize()
        {
            form.ListViewRegistry.Columns[2].Width = 90;
            form.ListViewRegistry.Columns[0].Width = 235;
            form.ListViewRegistry.Columns[1].Width = form.ListViewRegistry.Width - 350;
        }

        private bool ConfirmUnregistry()
        {
            var response = MessageBox.Show(form, "Desregistrar a entrada selecionada?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            return response == DialogResult.Yes;
        }

        internal void ClearGrid()
        {
            form.ListViewRegistry.Items.Clear();
        }

        internal void AddRow(RegistryEntry entry)
        {
            var item = new ListViewItem(entry.Key);

            item.SubItems.Add(entry.Path);
            item.SubItems.Add(entry.Exists ? "Sim" : "Não");

            form.ListViewRegistry.Items.Add(item);
        }
    }
}
