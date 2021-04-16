#region License
/*Данный код опубликован под лицензией Creative Commons Attribution-ShareAlike.
Разрешено использовать, распространять, изменять и брать данный код за основу для производных в коммерческих и
некоммерческих целях, при условии указания авторства и если производные лицензируются на тех же условиях.
Код поставляется "как есть". Автор не несет ответственности за возможные последствия использования.
Зуев Александр, 2020, все права защищены.
This code is listed under the Creative Commons Attribution-ShareAlike license.
You may use, redistribute, remix, tweak, and build upon this work non-commercially and commercially,
as long as you credit the author by linking back and license your new creations under the same terms.
This code is provided 'as is'. Author disclaims any implied warranty.
Zuev Aleksandr, 2020, all rigths reserved.*/
#endregion
#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;
#endregion

namespace RevitViewFilters
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

    class CommandBatchDelete : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Debug.Listeners.Clear();
            Debug.Listeners.Add(new RbsLogger.Logger("BatchDeleteFilters"));
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<ParameterFilterElement> filters = new FilteredElementCollector(doc)
                .OfClass(typeof(ParameterFilterElement))
                .Cast<ParameterFilterElement>()
                .ToList();
            Debug.WriteLine("Filters found: " + filters.Count);

            List<string> filterNames = filters.Select(x => x.Name).ToList();
            filterNames.Sort();

            FormBatchDelete form = new FormBatchDelete();
            form.Items = filterNames;

            form.ShowDialog();

            if (form.DialogResult != DialogResult.OK)
            {
                Debug.WriteLine("Cancelled by user");
                return Result.Cancelled;
            }

            List<string> deleteFilterNames = form.CheckedItems;
            Debug.WriteLine("Filters name for deleting: " + deleteFilterNames.Count);

            List<ParameterFilterElement> filtersToDelete = filters
                .Where(i => deleteFilterNames.Contains(i.Name))
                .ToList();
            Debug.WriteLine("filters found: " + filtersToDelete.Count);

            List<ElementId> ids = filtersToDelete.Select(i => i.Id).ToList();

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Удаление фильтров: " + deleteFilterNames.Count.ToString());
                doc.Delete(ids);
                t.Commit();
            }

            form.Dispose();

            Debug.WriteLine("De;ete filters: " + ids.Count);
            TaskDialog.Show("Удаление фильтров", "Успешно удалено фильтров: " + ids.Count);

            return Result.Succeeded;
        }
    }
}
