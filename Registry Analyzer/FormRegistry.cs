using System.Windows.Forms;

namespace Registry_Analyzer
{
    public partial class FormRegistry : Form
    {
        public ListView ListViewRegistry
        {
            get
            {
                return listViewRegistry;
            }
        }

        public Button ButtonSearch
        {
            get
            {
                return buttonSearch;
            }
        }

        public TextBox TextBoxSearch
        {
            get
            {
                return textBoxSearch;
            }
        }
        
        public FormRegistry()
        {
            InitializeComponent();
        }
    }
}
