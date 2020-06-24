using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

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

        public FilterDataForRebars(Document doc)
        {

        }

        public int ValuesCount => values.Count;

        private string _filterNamePrefix = "_rh_";
        public string FilterNamePrefix => _filterNamePrefix;

        public bool ApplyFilters(Document doc, View v, ElementId fillPatternId)
        {
            for (int i = 0; i < values.Count; i++)
            {
                string val = values[i];
                ParameterFilterElement filterRebar = FilterCreator.CreateRebarHostFilter(doc, rebarIsFamilyParam, rebarHostParam, rebarMrkParam, val, _filterNamePrefix);
                ViewUtils.ApplyViewFilter(doc, v, filterRebar, fillPatternId, i);

                ParameterFilterElement filterConstr = FilterCreator.CreateConstrFilter(doc, catIdConstructions, markParam, val, _filterNamePrefix);
                ViewUtils.ApplyViewFilter(doc, v, filterConstr, fillPatternId, i);
            }
            return true;
        }

        public string CollectValues(Document doc, View v)
        {
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

            HashSet<string> hostMarksList = new HashSet<string>();


            foreach (Element e in rebars)
            {
                ElementType etype = doc.GetElement(e.GetTypeId()) as ElementType;
                Parameter armisfamparam = etype.LookupParameter(rebarIsFamilyParamName);
                if (armisfamparam == null)
                {
                    armisfamparam = e.LookupParameter(rebarIsFamilyParamName);
                    if (armisfamparam == null) continue;
                }
                if (rebarIsFamilyParam == null)
                    rebarIsFamilyParam = armisfamparam;

                string hostMarkVal = "";

                Parameter hostParam;
                if (armisfamparam.AsInteger() == 0)
                {
                    hostParam = e.LookupParameter(rebarHostParamName);
                    if (hostParam == null) continue;
                }
                else
                {
                    hostParam = e.LookupParameter(rebarMrkParamName);
                    if (hostParam == null) continue;
                }
                hostMarkVal = hostParam.AsString();

                if (rebarHostParam == null)
                {
                    rebarHostParam = e.LookupParameter(rebarHostParamName); ;
                }
                if (rebarMrkParam == null)
                {
                    rebarMrkParam = e.LookupParameter(rebarMrkParamName);
                }


                //для создания фильтров мне надо взять параметр Марки из любого элемента, можно даже из арматуры
                if (markParam == null)
                    markParam = e.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);


                hostMarksList.Add(hostMarkVal);
            }



            values = hostMarksList.ToList();
            values.Sort();
            return string.Empty;
        }
    }
}
