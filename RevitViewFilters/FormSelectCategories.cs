using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ElementId = Autodesk.Revit.DB.ElementId;
using Category = Autodesk.Revit.DB.Category;

namespace RevitViewFilters
{
    public partial class FormSelectCategories : Form
    {
        public List<ElementId> checkedCategoriesIds;

        public FormSelectCategories(Autodesk.Revit.DB.Document doc, List<MyCategory> categories)
        {
            InitializeComponent();

            foreach(MyCategory mycat in categories)
            {
                checkedListBox1.Items.Add(mycat, CheckState.Checked);
            }
            
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            checkedCategoriesIds = new List<ElementId>();

            foreach (var checkedItem in checkedListBox1.CheckedItems)
            {
                MyCategory mycat =  (MyCategory)checkedItem;

                checkedCategoriesIds.Add(new ElementId(mycat.BuiltinCat));
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}
