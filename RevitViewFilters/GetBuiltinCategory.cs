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
    public static class GetBuiltinCategory
    {
        public static BuiltInCategory GetCategoryByRussianName(string Name)
        {
            switch (Name)
            {
                case "Аналитические модели балки":
                    return BuiltInCategory.OST_FramingAnalyticalGeometry;
                case "Аналитические модели изолированных фундаментов":
                    return BuiltInCategory.OST_IsolatedFoundationAnalytical;
                case "Аналитические модели колонн":
                    return BuiltInCategory.OST_ColumnAnalytical;
                case "Аналитические модели ленточных фундаментов":
                    return BuiltInCategory.OST_FootingAnalyticalGeometry;
                case "Аналитические модели перекрытий":
                    return BuiltInCategory.OST_FloorAnalytical;
                case "Аналитические модели раскосов":
                    return BuiltInCategory.OST_BraceAnalytical;
                case "Аналитические модели стен":
                    return BuiltInCategory.OST_WallAnalytical;
                case "Аналитические модели фундаментных плит":
                    return BuiltInCategory.OST_FoundationSlabAnalytical;
                case "Арматура воздуховодов":
                    return BuiltInCategory.OST_DuctAccessory;
                case "Арматура трубопроводов":
                    return BuiltInCategory.OST_PipeAccessory;
                case "Арматурная сетка несущей конструкции":
                    return BuiltInCategory.OST_FabricReinforcement;
                case "Армирование по площади несущей конструкции":
                    return BuiltInCategory.OST_AreaRein;
                case "Армирование по траектории":
                    return BuiltInCategory.OST_PathRein;
                case "Балочные системы":
                    return BuiltInCategory.OST_StructuralFramingSystem;
                case "Витражные системы":
                    return BuiltInCategory.OST_Curtain_Systems;
                case "Внутренние нагрузки несущих конструкций":
                    return BuiltInCategory.OST_InternalLoads;
                case "Внутренние линейные нагрузки":
                    return BuiltInCategory.OST_InternalLineLoads;
                case "Внутренние распределенные нагрузки":
                    return BuiltInCategory.OST_InternalAreaLoads;
                case "Внутренние сосредоточенные нагрузки":
                    return BuiltInCategory.OST_InternalPointLoads;
                case "Воздуховоды":
                    return BuiltInCategory.OST_DuctSystem;
                case "Воздуховоды по осевой":
                    return BuiltInCategory.OST_PlaceHolderDucts;
                case "Воздухораспределители":
                    return BuiltInCategory.OST_DuctTerminal;
                case "Генплан":
                    return BuiltInCategory.OST_Site;
                case "Границы участков":
                    return BuiltInCategory.OST_SiteProperty;
                case "	Основания":
                    return BuiltInCategory.OST_BuildingPad;
                case "Гибкие воздуховоды":
                    return BuiltInCategory.OST_FlexDuctCurves;
                case "Гибкие трубы":
                    return BuiltInCategory.OST_FlexPipeCurves;
                case "Датчики":
                    return BuiltInCategory.OST_DataDevices;
                case "Двери":
                    return BuiltInCategory.OST_Doors;
                case "Детали":
                    return BuiltInCategory.OST_Parts;
                case "Дорожки":
                    return BuiltInCategory.OST_Roads;
                case "Жесткие связи":
                    return BuiltInCategory.OST_AnalyticalRigidLinks;
                case "Зоны":
                    return BuiltInCategory.OST_Areas;
                case "Зоны ОВК":
                    return BuiltInCategory.OST_HVAC_Zones;
                case "Импосты витража":
                    return BuiltInCategory.OST_CurtainWallMullions;
                case "Кабельные лотки":
                    return BuiltInCategory.OST_CableTray;
                case "Каркас несущий":
                    return BuiltInCategory.OST_StructuralFraming;
                case "Колонны":
                    return BuiltInCategory.OST_Columns;
                case "Комплекты мебели":
                    return BuiltInCategory.OST_FurnitureSystems;
                case "Короба":
                    return BuiltInCategory.OST_Conduit;
                case "Крыши":
                    return BuiltInCategory.OST_Roofs;
                case "Желоба":
                    return BuiltInCategory.OST_Gutter;
                case "Лобовые доски":
                    return BuiltInCategory.OST_Fascia;
                case "Лестницы":
                    return BuiltInCategory.OST_Stairs;
                case "Материалы внутренней изоляции воздуховодов":
                    return BuiltInCategory.OST_DuctLinings;
                case "Материалы изоляции воздуховодов":
                    return BuiltInCategory.OST_DuctInsulations;
                case "Материалы изоляции труб":
                    return BuiltInCategory.OST_PipeInsulations;
                case "Мебель":
                    return BuiltInCategory.OST_Furniture;
                case "Нагрузки на конструкцию":
                    return BuiltInCategory.OST_Loads;
                case "Линейные нагрузки":
                    return BuiltInCategory.OST_LineLoads;
                case "Распределенные нагрузки":
                    return BuiltInCategory.OST_AreaLoads;
                case "Сосредоточенные нагрузки":
                    return BuiltInCategory.OST_PointLoads;
                case "Несущая арматура":
                    return BuiltInCategory.OST_Rebar;
                case "Несущие колонны":
                    return BuiltInCategory.OST_StructuralColumns;
                case "Области раскладки арматурных сеток":
                    return BuiltInCategory.OST_FabricAreas;
                case "Обобщенные модели":
                    return BuiltInCategory.OST_GenericModel;
                case "Оборудование":
                    return BuiltInCategory.OST_MechanicalEquipment;
                //case "Ограждение":
                //    return BuiltInCategory.INVALID;
                //case "	Балясины":
                //    return BuiltInCategory.INVALID;
                //case "	Верхние поручни":
                //    return BuiltInCategory.INVALID;
                //case "	Ограничения":
                //    return BuiltInCategory.INVALID;
                //case "	Опоры":
                //    return BuiltInCategory.INVALID;
                //case "	Перила":
                //    return BuiltInCategory.INVALID;
                case "Озеленение":
                    return BuiltInCategory.OST_Planting;
                case "Окна":
                    return BuiltInCategory.OST_Windows;
                case "Осветительная аппаратура":
                    return BuiltInCategory.OST_LightingDevices;
                case "Осветительные приборы":
                    return BuiltInCategory.OST_LightingFixtures;
                case "Пандус":
                    return BuiltInCategory.OST_Ramps;
                case "Панели витража":
                    return BuiltInCategory.OST_CurtainWallPanels;
                case "Парковка":
                    return BuiltInCategory.OST_Parking;
                case "Перекрытия":
                    return BuiltInCategory.OST_Floors;
                case "Помещения":
                    return BuiltInCategory.OST_Rooms;
                case "Потолки":
                    return BuiltInCategory.OST_Ceilings;
                case "Предохранительные устройства":
                    return BuiltInCategory.OST_SecurityDevices;
                case "Провода":
                    return BuiltInCategory.OST_Wire;
                case "Проемы для шахты":
                    return BuiltInCategory.OST_ShaftOpening;
                case "Пространства":
                    return BuiltInCategory.OST_MEPSpaces;
                case "Разрезы":
                    return BuiltInCategory.OST_Sections;
                //case "Разрезы":
                //    return BuiltInCategory.OST_Views;

                case "Ребра жесткости несущей конструкции":
                    return BuiltInCategory.OST_StructuralStiffener;
                case "Сантехнические приборы":
                    return BuiltInCategory.OST_PlumbingFixtures;
                case "Сборки":
                    return BuiltInCategory.OST_Assemblies;
                case "Сетки":
                    return BuiltInCategory.OST_Grids;
                case "Силовые электроприборы":
                    return BuiltInCategory.OST_ElectricalFixtures;
                case "Система коммутации":
                    return BuiltInCategory.OST_SwitchSystem;
                case "Системы воздуховодов":
                    return BuiltInCategory.OST_DuctSystem;
                case "Системы пожарной сигнализации":
                    return BuiltInCategory.OST_FireAlarmDevices;
                case "Соединения несущих конструкций":
                    return BuiltInCategory.OST_StructConnections;
                case "Соединительные детали воздуховодов":
                    return BuiltInCategory.OST_DuctFitting;
                case "Соединительные детали кабельных лотков":
                    return BuiltInCategory.OST_CableTrayFitting;
                case "Соединительные детали коробов":
                    return BuiltInCategory.OST_ConduitFitting;
                case "Соединительные детали труб":
                    return BuiltInCategory.OST_PipeFitting;
                case "Специальное оборудование":
                    return BuiltInCategory.OST_SpecialityEquipment;
                case "Спринклеры":
                    return BuiltInCategory.OST_Sprinklers;
                case "Стены":
                    return BuiltInCategory.OST_Walls;
                case "Телефонные устройства":
                    return BuiltInCategory.OST_TelephoneDevices;
                case "Топография":
                    return BuiltInCategory.OST_Topography;
                case "Трубопровод по осевой":
                    return BuiltInCategory.OST_PlaceHolderPipes;
                case "Трубопроводные системы":
                    return BuiltInCategory.OST_PipingSystem;
                case "Трубы":
                    return BuiltInCategory.OST_PipeCurves;
                case "Узлы аналитической модели":
                    return BuiltInCategory.OST_AnalyticalNodes;
                case "Уровни":
                    return BuiltInCategory.OST_Levels;
                case "Устройства вызова и оповещения":
                    return BuiltInCategory.OST_NurseCallDevices;
                case "Устройства связи":
                    return BuiltInCategory.OST_CommunicationDevices;

                case "Фасады":
                    return BuiltInCategory.OST_Elev;
                //case "Фасады":
                //    return BuiltInCategory.OST_Views;

                case "Фермы":
                    return BuiltInCategory.OST_StructuralTruss;
                case "Формообразующий элемент":
                    return BuiltInCategory.OST_Mass;
                case "Фрагменты":
                    return BuiltInCategory.OST_Callouts;
                case "Фундамент несущей конструкции":
                    return BuiltInCategory.OST_StructuralFoundation;
                case "Шкафы":
                    return BuiltInCategory.OST_Casework;
                case "Электрооборудование":
                    return BuiltInCategory.OST_ElectricalEquipment;
                case "Элементы узлов":
                    return BuiltInCategory.OST_DetailComponents;
                default:
                    return BuiltInCategory.INVALID;
            }

        }
    }
}
