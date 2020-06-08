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
    public static class RuleCreator
    {
        public static ParameterFilterElement createSimpleFilter(Document doc, List<ElementId> catsIds, string filterName, MyParameter mp, CriteriaType ctype)
        {
            List<ParameterFilterElement> filters = new FilteredElementCollector(doc)
                 .OfClass(typeof(ParameterFilterElement))
                 .Cast<ParameterFilterElement>()
                 .ToList();

            FilterRule rule = null;
            List<ParameterFilterElement> checkFilters = filters.Where(f => f.Name == filterName).ToList();
            ParameterFilterElement filter = null;
            if (checkFilters.Count != 0)
            {
                filter = checkFilters[0];
            }
            else
            {
                rule = RuleCreator.CreateRule(mp, ctype);


                if (rule == null) return null;

                List<FilterRule> filterRules = new List<FilterRule> { rule };
                try
                {
                    filter = ParameterFilterElement.Create(doc, filterName, catsIds);
#if R2017 || R2018
                    filter.SetRules(filterRules);
#else
                    filter.SetElementFilter(new ElementParameterFilter(filterRules));
#endif
                }
                catch { }

            }

            return filter;
        }


        public static FilterRule CreateRule(MyParameter mp, CriteriaType ctype)
        {
            Parameter param = mp.RevitParameter;
            FilterRule rule = null;
            if (ctype == CriteriaType.Equals)
            {
                switch (param.StorageType)
                {
                    case StorageType.None:
                        break;
                    case StorageType.Integer:
                        rule = ParameterFilterRuleFactory.CreateEqualsRule(param.Id, mp.AsInteger());
                        break;
                    case StorageType.Double:
                        rule = ParameterFilterRuleFactory.CreateEqualsRule(param.Id, mp.AsDouble(), 0.0001);
                        break;
                    case StorageType.String:
                        string val = mp.AsString();
                        if (val == null) break;
                        rule = ParameterFilterRuleFactory.CreateEqualsRule(param.Id, val, true);
                        break;
                    case StorageType.ElementId:
                        rule = ParameterFilterRuleFactory.CreateEqualsRule(param.Id, mp.AsElementId());
                        break;
                    default:
                        break;
                }
            }

            if (ctype == CriteriaType.StartsWith)
            {
                switch (param.StorageType)
                {
                    case StorageType.None:
                        break;
                    case StorageType.String:
                        string val = mp.AsString();
                        if (val == null) break;
                        rule = ParameterFilterRuleFactory.CreateBeginsWithRule(param.Id, val, true);
                        break;
                    default:
                        break;
                }
            }

            if (rule == null) throw new Exception("Не удалось создать правило фильтра");
            return rule;

        }



        public static FilterRule CreateRule2(Parameter Param, string Function, string Value)
        {
            ElementId paramId = Param.Id;
            switch (Param.StorageType)
            {
                case StorageType.String:
                    FilterRule stringRule = CreateRule(paramId, Function, Value);
                    return stringRule;

                case StorageType.Integer:
                    int intValue = 0;
                    if (Value.Equals("Да") || Value.Equals("да"))
                    {
                        intValue = 1;
                        goto Create;
                    }
                    if (Value.Equals("Нет") || Value.Equals("нет"))
                    {
                        intValue = 0;
                        goto Create;
                    }
                    int i = 0;
                    bool check = int.TryParse(Value, out i);
                    if (!check)
                    {
                        throw new Exception("Ошибка при обработке параметра: " + Param.Definition.Name + " = " + Value);
                    }
                    else
                    {
                        intValue = int.Parse(Value);
                    }

                Create:
                    FilterRule intRule = CreateRule(paramId, Function, intValue);
                    return intRule;

                case StorageType.Double:
                    double doubleValue = double.Parse(Value);
                    FilterRule doubleRule = CreateRule(paramId, Function, doubleValue);
                    return doubleRule;

                case StorageType.ElementId:
                    int id = int.Parse(Value);
                    ElementId valueId = new ElementId(id);
                    FilterRule idRule = CreateRule(paramId, Function, valueId);
                    return idRule;
            }
            return null;
        }


        private static FilterRule CreateRule(ElementId ParameterId, string Function, string Value)
        {
            switch (Function)
            {
                case "Равно":
                    return ParameterFilterRuleFactory.CreateEqualsRule(ParameterId, Value, true);
                case "Не равно":
                    return ParameterFilterRuleFactory.CreateNotEqualsRule(ParameterId, Value, true);
                case "Больше":
                    return ParameterFilterRuleFactory.CreateGreaterRule(ParameterId, Value, true);
                case "Больше или равно":
                    return ParameterFilterRuleFactory.CreateLessOrEqualRule(ParameterId, Value, true);
                case "Меньше":
                    return ParameterFilterRuleFactory.CreateLessRule(ParameterId, Value, true);
                case "Меньше или равно":
                    return ParameterFilterRuleFactory.CreateLessOrEqualRule(ParameterId, Value, true);
                case "Содержит":
                    return ParameterFilterRuleFactory.CreateContainsRule(ParameterId, Value, true);
                case "Не содержит":
                    return ParameterFilterRuleFactory.CreateNotContainsRule(ParameterId, Value, true);
                case "Начинается с":
                    return ParameterFilterRuleFactory.CreateBeginsWithRule(ParameterId, Value, true);
                case "Не начинается с":
                    return ParameterFilterRuleFactory.CreateNotBeginsWithRule(ParameterId, Value, true);
                case "Заканчивается на":
                    return ParameterFilterRuleFactory.CreateEndsWithRule(ParameterId, Value, true);
                case "Не заканчивается на":
                    return ParameterFilterRuleFactory.CreateNotEndsWithRule(ParameterId, Value, true);
                case "Поддерживает":
                    return ParameterFilterRuleFactory.CreateSharedParameterApplicableRule(Value);

                default:
                    return null;
            }
        }

        private static FilterRule CreateRule(ElementId ParameterId, string Function, double Value)
        {
            switch (Function)
            {
                case "Равно":
                    return ParameterFilterRuleFactory.CreateEqualsRule(ParameterId, Value, 0.0001);
                case "Не равно":
                    return ParameterFilterRuleFactory.CreateNotEqualsRule(ParameterId, Value, 0.0001);
                case "Больше":
                    return ParameterFilterRuleFactory.CreateGreaterRule(ParameterId, Value, 0.0001);
                case "Больше или равно":
                    return ParameterFilterRuleFactory.CreateLessOrEqualRule(ParameterId, Value, 0.0001);
                case "Меньше":
                    return ParameterFilterRuleFactory.CreateLessRule(ParameterId, Value, 0.0001);
                case "Меньше или равно":
                    return ParameterFilterRuleFactory.CreateLessOrEqualRule(ParameterId, Value, 0.0001);
                default:
                    return null;
            }
        }


        private static FilterRule CreateRule(ElementId ParameterId, string Function, int Value)
        {
            switch (Function)
            {
                case "Равно":
                    return ParameterFilterRuleFactory.CreateEqualsRule(ParameterId, Value);
                case "Не равно":
                    return ParameterFilterRuleFactory.CreateNotEqualsRule(ParameterId, Value);
                case "Больше":
                    return ParameterFilterRuleFactory.CreateGreaterRule(ParameterId, Value);
                case "Больше или равно":
                    return ParameterFilterRuleFactory.CreateLessOrEqualRule(ParameterId, Value);
                case "Меньше":
                    return ParameterFilterRuleFactory.CreateLessRule(ParameterId, Value);
                case "Меньше или равно":
                    return ParameterFilterRuleFactory.CreateLessOrEqualRule(ParameterId, Value);
                default:
                    return null;
            }
        }

        private static FilterRule CreateRule(ElementId ParameterId, string Function, ElementId ValueId)
        {
            switch (Function)
            {
                case "Равно":
                    return ParameterFilterRuleFactory.CreateEqualsRule(ParameterId, ValueId);
                case "Не равно":
                    return ParameterFilterRuleFactory.CreateNotEqualsRule(ParameterId, ValueId);
                case "Больше":
                    return ParameterFilterRuleFactory.CreateGreaterRule(ParameterId, ValueId);
                case "Больше или равно":
                    return ParameterFilterRuleFactory.CreateLessOrEqualRule(ParameterId, ValueId);
                case "Меньше":
                    return ParameterFilterRuleFactory.CreateLessRule(ParameterId, ValueId);
                case "Меньше или равно":
                    return ParameterFilterRuleFactory.CreateLessOrEqualRule(ParameterId, ValueId);
                default:
                    return null;
            }

        }

    }
}
