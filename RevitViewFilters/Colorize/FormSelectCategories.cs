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

        public FormSelectCategories(Autodesk.Revit.DB.Document doc, List<ElementId> categoriesIds)
        {
            InitializeComponent();

            foreach(ElementId catId in categoriesIds)
            {
                Autodesk.Revit.DB.BuiltInCategory bic
                    = (Autodesk.Revit.DB.BuiltInCategory)catId.IntegerValue;
                checkedListBox1.Items.Add(bic, CheckState.Checked);
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
                Autodesk.Revit.DB.BuiltInCategory cat =  
                    (Autodesk.Revit.DB.BuiltInCategory)checkedItem;

                checkedCategoriesIds.Add(new ElementId(cat));
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}
