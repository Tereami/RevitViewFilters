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
using System.IO;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;
#endregion

namespace RevitViewFilters
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

    class CommandCreate : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            AppBatchFilterCreation.assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string dllPath = Path.GetDirectoryName(AppBatchFilterCreation.assemblyPath);

            OpenFileDialog openCsvDialog = new OpenFileDialog();
            openCsvDialog.Filter = "CSV file|*.csv";
            openCsvDialog.Title = "Выберите файл CSV (v2018.10.17)";
            openCsvDialog.Multiselect = false;
            openCsvDialog.InitialDirectory = dllPath;

            if (openCsvDialog.ShowDialog() != DialogResult.OK)
                return Result.Cancelled; ;

            //считываю файл
            string path = openCsvDialog.FileName;
            List<string[]> data = ReadDataFromCSV.Read(path);

            string msg = "";
            int filterCount = 0;

            //одна строка в файле - один фильтр
            foreach (string[] line in data)
            {
                FilterSourceInfo filterSource = new FilterSourceInfo(line);
                string filterName = filterSource.FilterName;

                //Добавляю категории
                List<ElementId> catIds = new List<ElementId>();
                foreach (string stringCat in filterSource.Categories)
                {
                    BuiltInCategory cat = GetBuiltinCategory.GetCategoryByRussianName(stringCat);
                    catIds.Add(new ElementId(cat));
                }


                //добавляю критерии фильтрации
                List<FilterRule> filterRules = new List<FilterRule>();

                foreach (string[] sourceRule in filterSource.SourceRules)
                {
                    string paramName = sourceRule[0];
                    string function = sourceRule[1];
                    string value = sourceRule[2];


                    BuiltInCategory cat = GetBuiltinCategory.GetCategoryByRussianName(filterSource.Categories[0]);
                    if (cat == BuiltInCategory.OST_Sections || cat == BuiltInCategory.OST_Elev || cat == BuiltInCategory.OST_Callouts)
                        cat = BuiltInCategory.OST_Views;

                    FilteredElementCollector collector = new FilteredElementCollector(doc).OfCategory(cat);

                    Parameter param = null;
                    try
                    {
                        foreach (Element elem in collector)
                        {
                            param = elem.LookupParameter(paramName);
                            if (param == null)
                                continue;
                            break;
                        }
                    }
                    catch { }

                    if (collector.Count() == 0 || param == null)
                    {
                        message = "Ошибка при создании фильтра: " + filterName;
                        message += "\nУстановите как минимум один элемент в категории: " + filterSource.Categories[0];
                        message += "\nТребуемый параметр: " + paramName;
                        return Result.Failed;
                    }

                    FilterRule rule = FilterCreator.CreateRule2(param, function, value);
                    filterRules.Add(rule);
                }

                try
                {
                    using (Transaction t = new Transaction(doc))
                    {
                        t.Start("Создание фильтра" + filterName);
                        ParameterFilterElement filter = ParameterFilterElement.Create(doc, filterName, catIds);
#if R2017 || R2018
                        
                        filter.SetRules(filterRules);
#else
                        ElementParameterFilter epf = new ElementParameterFilter(filterRules);
                        filter.SetElementFilter(epf);
#endif
                        filterCount++;
                        t.Commit();
                    }
                }
                catch
                {
                    msg += filterName + "\n";
                }

            }
            string finalMessage = "Создано фильтров: " + filterCount.ToString() + "\n";
            if (msg.Length != 0)
            {
                finalMessage += "Не удалось создать: \n" + msg;
            }

            TaskDialog.Show("Batch filter create", finalMessage);
            return Result.Succeeded;
        }
    }
}
