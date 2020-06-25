using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitViewFilters
{
    class FilterDataSimple : IFilterData
    {

        public int ValuesCount => valuesList.Count;

        private string _filterNamePrefix = "_vc_";
        public string FilterNamePrefix => _filterNamePrefix;

        List<MyParameter> valuesList;

        private string catsName;
        private string paramName;
        List<ElementId> catsIds;
        List<Element> elems;
        int startSymbols;
        CriteriaType _criteriaType;

        public FilterDataSimple(Document doc, List<Element> Elems, MyParameter MyParam, int StartSymbols, CriteriaType CritType)
        {
            
            paramName = MyParam.Name;
            catsIds = MyParam.FilterableCategoriesIds;
            elems = Elems;
            
            _criteriaType = CritType;
            if (CritType == CriteriaType.Equals)
                startSymbols = 0;
            else
                startSymbols = StartSymbols;
        }

        public bool ApplyFilters(Document doc, View v, ElementId fillPatternId)
        {
            for (int i = 0; i < valuesList.Count; i++)
            {
                MyParameter mp = valuesList[i];
                Parameter param = mp.RevitParameter;
                string filterName = _filterNamePrefix + catsName + " " + paramName;

                if (_criteriaType == CriteriaType.Equals)
                    filterName = filterName + " равно ";
                else if (_criteriaType == CriteriaType.StartsWith)
                    filterName = filterName + " нач.с ";
                filterName += mp.AsValueString();

                ParameterFilterElement filter = FilterCreator.createSimpleFilter(doc, catsIds, filterName, mp, _criteriaType);
                if (filter == null) continue;


                ViewUtils.ApplyViewFilter(doc, v, filter, fillPatternId, i);

            }
            return true;

        }

        public MyResult CollectValues(Document doc, View v)
        {
            MyResult result = new MyResult(ResultType.ok, "");
            if (catsIds.Count > 1)
            {
                FormSelectCategories formSelCats = new FormSelectCategories(doc, catsIds);
                if (formSelCats.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    result.ResultType = ResultType.cancel;
                    return result;
                }
                    
                catsIds = formSelCats.checkedCategoriesIds;

                //перефильтрую элементы после того как перевыбрал категории
                elems = elems.Where(e => catsIds.Contains(e.Category.Id)).ToList();
            }

            catsName = ViewUtils.GetCategoriesName(doc, catsIds);

            valuesList = DocumentGetter.GetParameterValues(doc, elems, paramName, startSymbols);

            return result;
        }
    }
}
