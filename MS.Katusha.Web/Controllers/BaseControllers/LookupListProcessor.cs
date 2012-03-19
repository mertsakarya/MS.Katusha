using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using AutoMapper;

namespace MS.Katusha.Web.Controllers.BaseControllers
{
    public class LookupListProcessor<TModel, TModelData, TProp, TPropData, TEnum> where TModelData : MS.Katusha.Domain.Entities.BaseEntities.BaseModel where TEnum : struct
    {

        public delegate void DeleteEvent(TModelData modelData, TEnum enumValue);

        public delegate void AddEvent(TModelData modelData, TEnum enumValue);

        public delegate void ModelAddEvent(TProp prop);

        public delegate void ModelClearEvent();

        private readonly Type _propType;
        private readonly string _propertyName;

        private readonly Func<TPropData, TEnum> _compiledItemEnumExpression;
        private readonly Func<TModelData, IList<TPropData>> _compiledListModelExpression;
        private AddEvent _add;
        private DeleteEvent _delete;
        private ModelAddEvent _modelAdd;
        private ModelClearEvent _modelClear;

        public LookupListProcessor(
            Expression<Func<TModelData, IList<TPropData>>> listModelExpression,
            Expression<Func<TPropData, TEnum>> itemEnumExpression,
            DeleteEvent delete,
            AddEvent add,
            ModelAddEvent modelAdd,
            ModelClearEvent modelClear
            )
        {

            _compiledListModelExpression = listModelExpression.Compile();
            _compiledItemEnumExpression = itemEnumExpression.Compile();
            _propType = typeof (TPropData);
            _propertyName = _propType.Name;
            _add = add;
            _delete = delete;
            _modelAdd = modelAdd;
            _modelClear = modelClear;
        }

        public bool Process(HttpRequestBase request, ModelStateDictionary ModelState, TModel viewModel, TModelData dataModel)
        {
            IList<TPropData> prop = _compiledListModelExpression.Invoke(dataModel);
            IList<string> validationResults = new List<string>();
            try {
                var formValue = request.Form[_propertyName + "Selection[]"];
                var setForm = new HashSet<TEnum>();
                if (!String.IsNullOrWhiteSpace(formValue)) {
                    var list = request.Form[_propertyName + "Selection[]"].Split(',');
                    foreach (var line in list) {
                        TEnum c;
                        if (Enum.TryParse(line, out c))
                            setForm.Add(c);
                        else
                            validationResults.Add(line + " Can't Parse");
                    }
                }

                var setData = new HashSet<TEnum>();
                foreach (var line in prop) {
                    var item = _compiledItemEnumExpression.Invoke(line);
                    setData.Add(item);
                    if (!setForm.Contains(item)) {
                        try {
                            _delete(dataModel, item);
                        } catch (Exception ex) {
                            validationResults.Add(item.ToString() + " Can't Delete");
                        }
                    }
                }
                foreach (var item in setForm) {
                    if (!setData.Contains(item)) {
                        try {
                            _add(dataModel, item);
                        } catch (Exception ex) {
                            validationResults.Add(item.ToString() + " Can't Add");
                        }
                    }
                }
            } catch (Exception ex) {
                validationResults.Add(ex.Message);
            }
            if (validationResults.Count > 0) {
                foreach (var item in validationResults)
                    ModelState.AddModelError(_propertyName, item);
                _modelClear();
                foreach (var ctv in prop) {
                    var ctvModel = Mapper.Map<TProp>(ctv);
                    _modelAdd(ctvModel);
                }
            }
            return (validationResults.Count == 0);
        }

    }
}