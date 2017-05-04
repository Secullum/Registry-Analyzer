using System;
using System.IO;
using System.Windows.Forms;
using static Registry_Analyzer.RegistryModel;

namespace Registry_Analyzer
{
    class RegistryView
    {
        internal delegate void ButtonSearchClickedEventHandler(string term);

        internal delegate void UnregistryEventHandler(RegistryEntry entry);

        internal delegate void CopyKeyEventHandler(string key);

        internal delegate void CopyPathEventHandler(string path);

        internal event ButtonSearchClickedEventHandler ButtonSearchClicked;

        internal event UnregistryEventHandler UnregistryClicked;

        internal event CopyKeyEventHandler CopyKeyClicked;

        internal event CopyPathEventHandler CopyPathClicked;

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

            form.TextBoxSearch.KeyPress += (sender, e) =>
            {
                if (e.KeyChar == (char)Keys.Return)
                {
                    form.ButtonSearch.PerformClick();
                }
            };

            form.ListViewRegistry.ContextMenu = new ContextMenu();
            
            form.ListViewRegistry.SelectedIndexChanged += (sender, e) =>
            {
                var list = form.ListViewRegistry;
                
                for (var i = 0; i < list.ContextMenu.MenuItems.Count; i++)
                {
                    list.ContextMenu.MenuItems[i].Enabled = list.SelectedItems.Count > 0;
                }
            };

            form.ListViewRegistry.ContextMenu.MenuItems.Add("Copiar chave");
            
            form.ListViewRegistry.ContextMenu.MenuItems[0].Click += (sender, e) =>
            {
                CopyKeyClicked?.Invoke(SelectedItem(0));
            };
            
            form.ListViewRegistry.ContextMenu.MenuItems.Add("Copiar caminho");

            form.ListViewRegistry.ContextMenu.MenuItems[1].Click += (sender, e) =>
            {
                CopyPathClicked?.Invoke(SelectedItem(1));
            };

            form.ListViewRegistry.ContextMenu.MenuItems.Add("Desregistrar");

            form.ListViewRegistry.ContextMenu.MenuItems[2].Click += (sender, e) =>
            {
                UnregistryClicked?.Invoke(new RegistryEntry
                {
                    Key = SelectedItem(0),
                    Path = SelectedItem(1)
                });
            };
        }

        private string SelectedItem(int columnIndex)
        {
            return form.ListViewRegistry.SelectedItems[0].SubItems[columnIndex].Text;
        }

        private void AdjustColumnsSize()
        {
            form.ListViewRegistry.Columns[2].Width = 90;
            form.ListViewRegistry.Columns[0].Width = 235;
            form.ListViewRegistry.Columns[1].Width = form.ListViewRegistry.Width - 350;
        }
        
        internal void ClearGrid()
        {
            form.ListViewRegistry.Items.Clear();
        }

        internal void AddRow(RegistryEntry entry)
        {
            var item = new ListViewItem(entry.Key);

            item.SubItems.Add(entry.Path);
            item.SubItems.Add(File.Exists(entry.Path) ? "Sim" : "Não");

            form.ListViewRegistry.Items.Add(item);
        }

        internal void RemoveRow(RegistryEntry entry)
        {
            var item = form.ListViewRegistry.FindItemWithText(entry.Key);

            form.ListViewRegistry.Items.Remove(item);
        }
    }
}
