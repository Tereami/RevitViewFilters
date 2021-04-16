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
using System.Text;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace RevitViewFilters
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    class CommandViewColoring : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Debug.Listeners.Clear();
            Debug.Listeners.Add(new RbsLogger.Logger("ViewColoring"));

            Document doc = commandData.Application.ActiveUIDocument.Document;
            View curView = doc.ActiveView;

            //проверю, можно ли менять фильтры у данного вида
            bool checkAllowFilters = ViewUtils.CheckIsChangeFiltersAvailable(doc, curView);
            if (!checkAllowFilters)
            {
                Debug.WriteLine("View is depended by view template");
                TaskDialog.Show("Ошибка", "Невозможно назначить фильтры, так как они определяются в шаблоне вида.");
                return Result.Failed;
            }

            //список все параметров у элементов на виде
            List<Element> elems = new FilteredElementCollector(doc, curView.Id)
                 .WhereElementIsNotElementType()
                 .ToElements()
                 .Where(e => e != null)
                 .Where(e => e.IsValidObject)
                 .Where(e => e.Category != null)
                 .Where(e => e.Category.Id.IntegerValue != -2000500)
                 .ToList();
            Debug.WriteLine("Elements on view: " + elems.Count);    
            List<MyParameter> mparams = ViewUtils.GetAllFilterableParameters(doc, elems);
            Debug.WriteLine("Filterable parameters found: " + mparams.Count);

            FormSelectParameterForFilters form1 = new FormSelectParameterForFilters();
            form1.parameters = mparams;
            if (form1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                Debug.WriteLine("Cancelled by user");
                return Result.Cancelled;
            }

            IFilterData filterData = null;
            if(form1.colorizeMode == ColorizeMode.ResetColors)
            {
                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Очистка фильтров");
                    ClearFilters(doc, curView);
                    t.Commit();
                }
                return Result.Succeeded;
            }
            else if (form1.colorizeMode == ColorizeMode.ByParameter)
            {
                int startSymbols = 0;
                if (form1.criteriaType == CriteriaType.StartsWith)
                {
                    startSymbols = form1.startSymbols;
                    Debug.WriteLine("Filter criteria start symbols:" + startSymbols);
                }
                Debug.WriteLine("Colorize by parameter: " + form1.selectedParameter.Name);
                filterData = new FilterDataSimple(doc, elems, form1.selectedParameter, form1.startSymbols, form1.criteriaType);
            }
            else if (form1.colorizeMode == ColorizeMode.CheckHostmark)
            {
                Debug.WriteLine("Colorize for rebar host checking");
                filterData = new FilterDataForRebars(doc);
            }

            MyDialogResult collectResult = filterData.CollectValues(doc, curView);
            if(collectResult.ResultType == ResultType.cancel)
            {
                Debug.WriteLine("Cancelled by user");
                return Result.Cancelled;
            }
            else if(collectResult.ResultType == ResultType.error)
            {
                message = collectResult.Message;
                Debug.WriteLine(message);
                return Result.Failed;
            }
            else if (collectResult.ResultType == ResultType.warning)
            {
                Debug.WriteLine(collectResult.Message);
                TaskDialog.Show("Внимание", collectResult.Message);
            }

            Debug.WriteLine("Values:" + filterData.ValuesCount);
            if (filterData.ValuesCount > 64)
            {
                message = "Значений больше 64! Генерация цветов невозможна";
                return Result.Failed;
            }

            //Получу id сплошной заливки
            ElementId solidFillPatternId = DocumentGetter.GetSolidFillPatternId(doc);


            using (Transaction t = new Transaction(doc))
            {
                t.Start("Колоризация вида");

                ClearFilters(doc, curView);

                filterData.ApplyFilters(doc, curView, solidFillPatternId, form1.colorLines, form1.colorFill);

                t.Commit();
            }
            Debug.WriteLine("Coloring completed");
            return Result.Succeeded;
        }

        private void ClearFilters(Document doc, View view)
        {
            Debug.WriteLine("Clear coloring filters on view: " + view.Name);
            foreach (ElementId filterId in view.GetFilters())
            {
                ParameterFilterElement filter = doc.GetElement(filterId) as ParameterFilterElement;
                if (filter.Name.StartsWith("_"))
                {
                    view.RemoveFilter(filterId);
                }
            }
            Debug.WriteLine("Clear filters completed");
        }
    }
}
