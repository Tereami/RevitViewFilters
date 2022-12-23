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
using Autodesk.Revit.UI.Selection;
#endregion

namespace RevitViewFilters
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    class CommandWallHatch : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Debug.Listeners.Clear();
            Debug.Listeners.Add(new RbsLogger.Logger("WallHatch"));
            AppBatchFilterCreation.assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            Document doc = commandData.Application.ActiveUIDocument.Document;
            View curView = doc.ActiveView;
            if (!(curView is ViewPlan))
            {
                message = "Запуск команды возможен только на плане";
                Debug.WriteLine(message);
                return Result.Failed;
            }

            if (curView.ViewTemplateId != null && curView.ViewTemplateId != ElementId.InvalidElementId)
            {
                message = "Для вида применен шаблон. Отключите шаблон вида перед запуском";
                Debug.WriteLine(message);
                return Result.Failed;
            }

            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            Debug.WriteLine("Selected elements: " + sel.GetElementIds().Count);
            if (sel.GetElementIds().Count == 0)
            {
                message = "Не выбраны стены.";
                return Result.Failed;
            }
            List<Wall> walls = new List<Wall>();
            foreach (ElementId id in sel.GetElementIds())
            {
                Wall w = doc.GetElement(id) as Wall;
                if (w == null) continue;
                walls.Add(w);
            }
            Debug.WriteLine("Walls count: " + walls.Count);
            if (walls.Count == 0)
            {
                message = "Не выбраны стены.";
                return Result.Failed;
            }

            SortedDictionary<double, List<Wall>> wallHeigthDict = new SortedDictionary<double, List<Wall>>();
            if (wallHeigthDict.Count > 10)
            {
                message = "Слишком много типов стен! Должно быть не более 10";
                Debug.WriteLine(message);
                return Result.Failed;
            }

            foreach (Wall w in walls)
            {
                Debug.WriteLine("Current wall id:" + w.Id.IntegerValue);
                double topElev = GetWallTopElev(doc, w, true);
                Debug.WriteLine("Top elevation: " + topElev.ToString("F1"));

                if (wallHeigthDict.ContainsKey(topElev))
                {
                    wallHeigthDict[topElev].Add(w);
                }
                else
                {
                    wallHeigthDict.Add(topElev, new List<Wall> { w });
                }
            }

            List<ElementId> catsIds = new List<ElementId> { new ElementId(BuiltInCategory.OST_Walls) };

            int i = 1;
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Отметки стен");

                foreach (ElementId filterId in curView.GetFilters())
                {
                    ParameterFilterElement filter = doc.GetElement(filterId) as ParameterFilterElement;
                    if (filter.Name.StartsWith("_cwh_"))
                    {
                        curView.RemoveFilter(filterId);
                    }
                }
                Debug.WriteLine("Old filters deleted");

                foreach (var kvp in wallHeigthDict)
                {
                    Debug.WriteLine("Current key: " + kvp.Key.ToString("F1"));
                    ElementId hatchId = GetHatchIdByNumber(doc, i);
                    ImageType image = GetImageTypeByNumber(doc, i);

                    double curHeigthMm = kvp.Key;
                    double curHeigthFt = curHeigthMm / 304.8;

                    List<Wall> curWalls = kvp.Value;

                    foreach (Wall w in curWalls)
                    {
                        w.get_Parameter(BuiltInParameter.ALL_MODEL_IMAGE).Set(image.Id);
                        w.LookupParameter("Рзм.ОтметкаВерха").Set(curHeigthFt);

                        double bottomElev = GetWallTopElev(doc, w, false);
                        double bottomElevFt = bottomElev / 304.8;
                        w.LookupParameter("Рзм.ОтметкаНиза").Set(bottomElevFt);
                    }

                    string filterName = "_cwh_" + "Стены Рзм.ОтметкаВерха равно " + curHeigthMm.ToString("F0");

                    MyParameter mp = new MyParameter(curWalls.First().LookupParameter("Рзм.ОтметкаВерха"));
                    ParameterFilterElement filter =
                        FilterCreator.createSimpleFilter(doc, catsIds, filterName, mp, CriteriaType.Equals);

                    curView.AddFilter(filter.Id);
                    OverrideGraphicSettings ogs = new OverrideGraphicSettings();

#if R2017 || R2018
                    ogs.SetProjectionFillPatternId(hatchId);
                    ogs.SetCutFillPatternId(hatchId);
#else
                    ogs.SetSurfaceForegroundPatternId(hatchId);
                    ogs.SetCutForegroundPatternId(hatchId);
#endif
                    curView.SetFilterOverrides(filter.Id, ogs);
                    i++;
                }
                t.Commit();
            }
            return Result.Succeeded;
        }

        private double GetWallTopElev(Document doc, Wall w, bool TopOrBottomElev)
        {
            Debug.WriteLine("Try to get wall top elevation, wall id: " + w.Id.IntegerValue);
            ElementId levelId = w.LevelId;
            if (levelId == null || levelId == ElementId.InvalidElementId)
                throw new Exception("У стены нет базового уровня! ID стены: " + w.Id.IntegerValue.ToString());

            Level lev = doc.GetElement(levelId) as Level;
            double levElev = lev.ProjectElevation;
            double baseOffset = w.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).AsDouble();

            double elev = levElev + baseOffset; // + wallHeigth;

            if (TopOrBottomElev)
            {
                double wallHeigth = w.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();
                elev += wallHeigth;
            }

            double elevmm = elev * 304.8;

            double elevRoundMm = Math.Round(elevmm);
            return elevRoundMm;
        }

        private ElementId GetHatchIdByNumber(Document doc, int number)
        {
            string hatchName = GetHatchNameByNumber(doc, number);
            Debug.WriteLine("Hatch name: " + hatchName);
            ElementId hatchId = GetHatchIdByName(doc, hatchName);
            return hatchId;
        }

        private string GetHatchNameByNumber(Document doc, int number)
        {
            if (number == 1) return "Грунт естественный";
            if (number == 2) return "08.Грунт.Гравий";
            if (number == 3) return "05.Кирпич.В разрезе (45град) 3.5мм";
            if (number == 4) return "02.Крест (45град) 2мм";
            if (number == 5) return "06.Древесина 2";
            if (number == 6) return "02.Крест (45град) 1мм";
            if (number == 7) return "01.Диагональ (135град) 1.5мм";
            if (number == 8) return "Грунт: песок плотный";
            if (number == 9) return "01.Диагональ (45град) 1.5мм";
            if (number == 10) return "05.Кирпич.В разрезе (135град) 3.5мм";

            return "Сплошная заливка";
        }

        private ElementId GetHatchIdByName(Document doc, string hatchName)
        {
            List<FillPatternElement> fpes = new FilteredElementCollector(doc)
                .OfClass(typeof(FillPatternElement))
                .Where(i => i.Name.Contains(hatchName))
                .Cast<FillPatternElement>()
                .ToList();
            if (fpes.Count == 0)
            {
                Debug.WriteLine("Unable to find hatch: " + hatchName);
                throw new Exception("Не удалось найти штриховку " + hatchName);
            }
            Debug.WriteLine("Hatch found: " + fpes.First().Id.IntegerValue);
            return fpes.First().Id;
        }

        private ImageType GetImageTypeByNumber(Document doc, int number)
        {
            string name = "ШтриховкаСтены_" + number.ToString() + ".png";
            Debug.WriteLine("Try to find image: " + name);

            List<ImageType> images = new FilteredElementCollector(doc)
                .OfClass(typeof(ImageType))
                .Cast<ImageType>()
                .Where(i => i.Name.Equals(name))
                .ToList();

            if (images.Count == 0)
            {
                List<ImageType> errImgs = new FilteredElementCollector(doc)
                    .OfClass(typeof(ImageType))
                    .Cast<ImageType>()
                    .Where(i => i.Name.Equals("Ошибка.png"))
                    .ToList();
                if (errImgs.Count == 0)
                {
                    Debug.WriteLine("No wall images in the project");
                    throw new Exception("Загрузите в проект картинки!");
                }

                ImageType errImg = errImgs.First();
                return errImg;
            }
            Debug.WriteLine("Image found, id: " + images.First().Id.IntegerValue);
            return images.First();
        }
    }
}
