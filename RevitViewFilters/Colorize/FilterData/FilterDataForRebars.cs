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
#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Autodesk.Revit.DB;
#endregion

namespace RevitViewFilters
{
    public class FilterDataForRebars : IFilterData
    {
        const string rebarIsFamilyParamName = "Арм.ВыполненаСемейством";
        const string rebarHostParamName = "Метка основы";
        const string rebarMrkParamName = "Мрк.МаркаКонструкции";

        public Parameter rebarIsFamilyParam;
        public Parameter rebarHostParam;
        public Parameter rebarMrkParam;
        public Parameter markParam;

        public List<ElementId> catIdRebar;
        public List<ElementId> catIdConstructions;

        public List<string> values;

        public bool _rebarIsFamilyParamExists;

        public FilterDataForRebars(Document doc)
        {
            //определю, это шаблоне weandrevit или сторонний
            List<ElementId> rebarCategoryIds = new List<ElementId> { new ElementId(BuiltInCategory.OST_Rebar) };
            List<ElementId> paramsRebar = ParameterFilterUtilities.GetFilterableParametersInCommon(doc, rebarCategoryIds).ToList();
            _rebarIsFamilyParamExists = false;
            foreach (ElementId paramid in paramsRebar)
            {
                string paramName = ViewUtils.GetParamName(doc, paramid);
                if(paramName == rebarIsFamilyParamName)
                {
                    _rebarIsFamilyParamExists = true;
                    break;
                }
            }
        }

        public int ValuesCount => values.Count;

        private string _filterNamePrefix = "_rh_";
        public string FilterNamePrefix => _filterNamePrefix;

        public bool ApplyFilters(Document doc, View v, ElementId fillPatternId, bool colorLines, bool colorFill)
        {
            Debug.WriteLine("Start apply rebar filters for view: " + v.Name);
            for (int i = 0; i < values.Count; i++)
            {
                string val = values[i];
                Debug.WriteLine("Value: " + val);

                ParameterFilterElement filterConstr = FilterCreator.CreateConstrFilter(doc, catIdConstructions, markParam, val, _filterNamePrefix);
                ViewUtils.ApplyViewFilter(doc, v, filterConstr, fillPatternId, i, colorLines, colorFill);

                if (!_rebarIsFamilyParamExists)
                {
                    ParameterFilterElement filterRebarSingleMode = FilterCreator
                    .CreateRebarHostFilter(doc, catIdRebar, rebarIsFamilyParam, rebarHostParam, rebarMrkParam, val, _filterNamePrefix, RebarFilterMode.SingleMode);
                    ViewUtils.ApplyViewFilter(doc, v, filterRebarSingleMode, fillPatternId, i, colorLines, colorFill);
                    continue;
                }

#if R2017 || R2018
                ParameterFilterElement filterRebarStandardRebar = FilterCreator
                    .CreateRebarHostFilter(doc, catIdRebar, rebarIsFamilyParam, rebarHostParam, rebarMrkParam, val, _filterNamePrefix, RebarFilterMode.StandardRebarMode);
                ViewUtils.ApplyViewFilter(doc, v, filterRebarStandardRebar, fillPatternId, i, colorLines, colorFill);
                Debug.WriteLine("Filter created and applied to view: " + filterRebarStandardRebar.Name);

                ParameterFilterElement filterRebarIfcRebar = FilterCreator
                    .CreateRebarHostFilter(doc, new List<ElementId> { new ElementId(BuiltInCategory.OST_Rebar) }, rebarIsFamilyParam, rebarHostParam, rebarMrkParam, val, _filterNamePrefix, RebarFilterMode.IfcMode);
                ViewUtils.ApplyViewFilter(doc, v, filterRebarIfcRebar, fillPatternId, i, colorLines, colorFill);
                Debug.WriteLine("Filter created and applied to view: " + filterRebarIfcRebar.Name);
#else
                ParameterFilterElement filterRebarOrStyle = FilterCreator
                    .CreateRebarHostFilter(doc, catIdRebar, rebarIsFamilyParam, rebarHostParam, rebarMrkParam, val, _filterNamePrefix, RebarFilterMode.DoubleMode);
                ViewUtils.ApplyViewFilter(doc, v, filterRebarOrStyle, fillPatternId, i, colorLines, colorFill);
                Debug.WriteLine("Filter created and applied to view: " + filterRebarOrStyle.Name);
#endif
            }
            Debug.WriteLine("All filters applied");
            return true;
        }

        public MyDialogResult CollectValues(Document doc, View v)
        {
            Debug.WriteLine("Collect parameter values for view: " + v.Name);
            MyDialogResult result = new MyDialogResult(ResultType.ok, "");
            catIdRebar = new List<ElementId> {
                new ElementId(BuiltInCategory.OST_Rebar),
                new ElementId(BuiltInCategory.OST_AreaRein),
                new ElementId(BuiltInCategory.OST_PathRein)
            };

            catIdConstructions = new List<ElementId>{
                new ElementId(BuiltInCategory.OST_Walls),
                new ElementId(BuiltInCategory.OST_Floors),
                new ElementId(BuiltInCategory.OST_StructuralColumns),
                new ElementId(BuiltInCategory.OST_StructuralFraming),
                new ElementId(BuiltInCategory.OST_StructuralFoundation)
            };

            List<Element> rebars = new FilteredElementCollector(doc, v.Id)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.OST_Rebar)
                .ToElements()
                .ToList();
            Debug.WriteLine("Rebars count: " + rebars.Count);

            HashSet<string> hostMarksList = new HashSet<string>();

            foreach (Element e in rebars)
            {
                //для создания фильтров мне надо взять параметр Марки из любого элемента, можно даже из арматуры
                if (markParam == null)
                    markParam = e.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                Parameter hostParam = e.LookupParameter(rebarHostParamName);
                if (hostParam == null)
                {
                    hostParam = e.LookupParameter(rebarMrkParamName);
                    if (hostParam == null)
                    {
                        continue;
                    }
                }

                string hostMarkVal = hostParam.AsString();
                if(!string.IsNullOrEmpty(hostMarkVal))
                    hostMarksList.Add(hostMarkVal);

                if (rebarHostParam == null)
                {
                    rebarHostParam = e.LookupParameter(rebarHostParamName); ;
                }

                if (!_rebarIsFamilyParamExists)
                    continue;

                if (rebarMrkParam == null)
                {
                    rebarMrkParam = e.LookupParameter(rebarMrkParamName);
                }

                ElementType etype = doc.GetElement(e.GetTypeId()) as ElementType;
                Parameter armisfamparam = etype.LookupParameter(rebarIsFamilyParamName);
                if (armisfamparam == null)
                {
                    armisfamparam = e.LookupParameter(rebarIsFamilyParamName);
                    if (armisfamparam == null) continue;
                }
                if (rebarIsFamilyParam == null)
                    rebarIsFamilyParam = armisfamparam;
            }

            if (rebarIsFamilyParam == null)
            {
                result.ResultType = ResultType.warning;
                result.Message += "Не найден параметр Арм.ВыполненаСемейством. Фильтры будут созданы только по параметру Метка основы.";
            }

            values = hostMarksList.ToList();
            values.Sort();
            Debug.WriteLine("Collect values: " + values.Count);
            return result;
        }
    }
}
