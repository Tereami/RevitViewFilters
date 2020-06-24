using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace RevitViewFilters
{
    public static class DocumentGetter
    {
        public static List<MyParameter> GetParameterValues(Document doc, List<Element> elems, string ParamName, int startSymbols)
        {
            HashSet<MyParameter> values = new HashSet<MyParameter>(); //список значений параметра
            foreach (Element elem in elems)
            {
                Parameter curParam = elem.LookupParameter(ParamName);
                if (curParam == null)
                {
                    ElementId typeElemId = elem.GetTypeId();
                    if (typeElemId == null) continue;
                    if (typeElemId == ElementId.InvalidElementId) continue;
                    Element typeElem = doc.GetElement(typeElemId);
                    curParam = typeElem.LookupParameter(ParamName);
                    if (curParam == null) continue;
                }

                if (!curParam.HasValue) continue;

                MyParameter mp = new MyParameter(curParam);
                if (!mp.HasValue) continue;

                if (startSymbols > 0)
                {
                    if (mp.RevitStorageType != StorageType.String)
                    {
                        throw new Exception("Критерий \"Начинается с\" доступен только для текстовых параметров");
                    }

                    string valTemp = mp.AsString();
                    string val = "";
                    if (valTemp.Length < startSymbols)
                        val = valTemp;
                    else
                        val = valTemp.Substring(0, startSymbols);

                    mp.Set(val);

                }
                values.Add(mp);
            }

            List<MyParameter> listParams = values.ToList();
            listParams.Sort();
            return listParams;

        }



        public static ElementId GetSolidFillPatternId(Document doc)
        {
            FillPatternElement fpe = new FilteredElementCollector(doc)
                .OfClass(typeof(FillPatternElement))
                .Cast<FillPatternElement>()
                .First(a => a.GetFillPattern().IsSolidFill);
            return fpe.Id;
        }

        public static ParameterFilterElement GetFilterByName(Document doc, string filterName)
        {
            List<ParameterFilterElement> filters = new FilteredElementCollector(doc)
                    .OfClass(typeof(ParameterFilterElement))
                    .Cast<ParameterFilterElement>()
                    .Where(f => f.Name == filterName)
                    .ToList();
            if (filters.Count == 0)
                return null;

            return filters.First();
        }
    }


}
