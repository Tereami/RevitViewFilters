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
using System.Threading.Tasks;
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
            Document doc = commandData.Application.ActiveUIDocument.Document;
            View curView = doc.ActiveView;

            //проверю, можно ли менять фильтры у данного вида
            bool checkAllowFilters = ViewUtils.CheckIsChangeFiltersAvailable(doc, curView);
            if (!checkAllowFilters)
            {
                TaskDialog.Show("Ошибка", "Невозможно назначить фильтры, так как они определяются в шаблоне вида.");
                return Result.Failed;
            }

            //список все параметров у элементов на виде
            List<Element> elems = new FilteredElementCollector(doc, curView.Id)
                 .WhereElementIsNotElementType()
                 .ToElements()
                 .ToList();
            List<MyParameter> mparams = ViewUtils.GetAllFilterableParameters(doc, elems);


            FormSelectParameterForFilters form1 = new FormSelectParameterForFilters();
            form1.parameters = mparams;
            if (form1.ShowDialog() != System.Windows.Forms.DialogResult.OK) return Result.Cancelled;

            MyParameter mParam = form1.selectedParameter;
            string mParamName = mParam.Name;


            HashSet<MyParameter> values = new HashSet<MyParameter>(); //список значений параметра


            foreach (Element elem in elems)
            {
                if (elem.Category == null) continue;
                if (elem.Category.Id.IntegerValue == -2000500) continue; //отфильтровываю какие-то "Камеры"
                Parameter curParam = elem.LookupParameter(mParamName);
                if (curParam == null)
                {
                    ElementId typeElemId = elem.GetTypeId();
                    if (typeElemId == null) continue;
                    if (typeElemId == ElementId.InvalidElementId) continue;
                    Element typeElem = doc.GetElement(typeElemId);
                    curParam = typeElem.LookupParameter(mParamName);
                    if (curParam == null) continue;
                }

                if (!curParam.HasValue) continue;

                MyParameter mp = new MyParameter(curParam);
                if (!mp.HasValue) continue;

                values.Add(mp);
            }

            if (values.Count > 64)
            {
                message = "Значений больше 64! Генерация цветов невозможна";
                return Result.Failed;
            }

            List<ElementId> catsIds = mParam.FilterableCategoriesIds;  //список категорий, у которых есть этот параметр
            string catsName = ViewUtils.GetCategoriesName(doc, catsIds);
            //string function;

            switch (form1.criteriaType)
            {
                case CriteriaType.Equals:
                    break;
                case CriteriaType.StartsWith:
                    break;
                default:
                    break;
            }

            List<MyParameter> valuesList = values.ToList();

            if (form1.criteriaType == CriteriaType.StartsWith)
            {
                HashSet<MyParameter> valuesListStartsWith = new HashSet<MyParameter>();
                foreach (MyParameter mp in valuesList)
                {
                    if (mp.RevitStorageType != StorageType.String)
                    {
                        TaskDialog.Show("Ошибка", "Критерий \"Начинается с\" доступен только для текстовых параметров");
                        return Result.Failed;
                    }

                    string valTemp = mp.AsString();
                    int startChars = form1.startSymbols;

                    string val = "";
                    if (valTemp.Length < startChars)
                        val = valTemp;
                    else
                        val = valTemp.Substring(0, form1.startSymbols);

                    MyParameter mp2 = new MyParameter(mp.RevitParameter);
                    mp2.Set(val);
                    valuesListStartsWith.Add(mp2);
                }
                valuesList = valuesListStartsWith.ToList();
            }

            //Получу id сплошной заливки
            ElementId solidFillPatternId = new ElementId(4);



            using (Transaction t = new Transaction(doc))
            {
                t.Start("Колоризация вида");

                foreach (ElementId filterId in curView.GetFilters())
                {
                    ParameterFilterElement filter = doc.GetElement(filterId) as ParameterFilterElement;
                    if (filter.Name.StartsWith("_vc_"))
                    {
                        curView.RemoveFilter(filterId);
                    }
                }

                //if (form1.criteriaType == CriteriaType.Equals)

                for (int i = 0; i < valuesList.Count; i++)
                {
                    MyParameter mp = valuesList[i];
                    Parameter param = mp.RevitParameter;
                    FilterRule rule = null;
                    string filterName = "_vc_" + catsName + " " + mParam.Name;

                    if (form1.criteriaType == CriteriaType.Equals) filterName = filterName + " равно ";
                    if (form1.criteriaType == CriteriaType.StartsWith) filterName = filterName + " нач.с ";
                    filterName += mp.AsValueString();

                    ParameterFilterElement filter = RuleCreator.createSimpleFilter(doc, catsIds, filterName, mp, form1.criteriaType);
                    if (filter == null) continue;



                    curView.AddFilter(filter.Id);
                    OverrideGraphicSettings ogs = new OverrideGraphicSettings();

                    byte red = Convert.ToByte(ColorsCollection.colors[i].Substring(1, 2), 16);
                    byte green = Convert.ToByte(ColorsCollection.colors[i].Substring(3, 2), 16);
                    byte blue = Convert.ToByte(ColorsCollection.colors[i].Substring(5, 2), 16);

                    Color clr = new Color(red, green, blue);


                    ogs.SetProjectionLineColor(clr);
                    ogs.SetCutLineColor(clr);

#if R2017 || R2018
                    ogs.SetProjectionFillColor(clr);
                    ogs.SetProjectionFillPatternId(solidFillPatternId);
                    ogs.SetCutFillColor(clr);
                    ogs.SetCutFillPatternId(solidFillPatternId);
#else
                    ogs.SetSurfaceForegroundPatternColor(clr);
                    ogs.SetSurfaceForegroundPatternId(solidFillPatternId);
                    ogs.SetCutForegroundPatternColor(clr);
                    ogs.SetCutForegroundPatternId(solidFillPatternId);
#endif

                    doc.ActiveView.SetFilterOverrides(filter.Id, ogs);
                }

                t.Commit();
            }




            return Result.Succeeded;
        }
    }
}
