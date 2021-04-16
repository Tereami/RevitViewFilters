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

using System.Windows.Media.Imaging;
#endregion

namespace RevitViewFilters
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

    public class AppBatchFilterCreation : IExternalApplication
    {
        public static string assemblyPath = "";

        Result IExternalApplication.OnStartup(UIControlledApplication application)
        {
            assemblyPath = typeof(AppBatchFilterCreation).Assembly.Location;

            string tabName = "BIM-STARTER TEST";
            try { application.CreateRibbonTab(tabName); }
            catch { }


            RibbonPanel panelFilters = application.CreateRibbonPanel(tabName, "Фильтры");
            PushButton btnBatchCreate = panelFilters.AddItem(new PushButtonData(
                "BatchCreate",
                "Создать\nфильтры",
                assemblyPath,
                "RevitViewFilters.CommandCreate")
                ) as PushButton;

            PushButton btnBatchDelete = panelFilters.AddItem(new PushButtonData(
                "BatchDelete",
                "Удалить\nфильтры",
                assemblyPath,
                "RevitViewFilters.CommandBatchDelete")
                ) as PushButton;


            PushButton btnColoring = panelFilters.AddItem(new PushButtonData(
                "ViewColoring",
                "Колоризация\nвида",
                assemblyPath,
                "RevitViewFilters.CommandViewColoring")
                ) as PushButton;

            PushButton btnWallHatch = panelFilters.AddItem(new PushButtonData(
                "WallHatch",
                "Штриховки\nстен",
                assemblyPath,
                "RevitViewFilters.CommandWallHatch")
                ) as PushButton;


            return Result.Succeeded;
        }

        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

    }
}
