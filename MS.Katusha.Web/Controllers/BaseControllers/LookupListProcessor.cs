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

        private readonly string _propertyName;

        private readonly Func<TPropData, TEnum> _compiledItemEnumExpression;
        private readonly Func<TModelData, IList<TPropData>> _compiledListDataExpression;
        private  Func<TModel,IList<TProp>> _compiledListModelExpression;
        private AddEvent _add;
        private DeleteEvent _delete;

        public LookupListProcessor(
            Expression<Func<TModelData, IList<TPropData>>> listDataExpression,
            Expression<Func<TModel, IList<TProp>>> listModelExpression,
            Expression<Func<TPropData, TEnum>> itemEnumExpression,
            DeleteEvent delete,
            AddEvent add
            )
        {

            _compiledListDataExpression = listDataExpression.Compile();
            _compiledListModelExpression = listModelExpression.Compile();
            _compiledItemEnumExpression = itemEnumExpression.Compile();
            _propertyName = typeof (TProp).Name;
            _add = add;
            _delete = delete;
        }

        public void Process(HttpRequestBase request, ModelStateDictionary modelState, TModel viewModel, TModelData dataModel)
        {
            var listData = _compiledListDataExpression.Invoke(dataModel);
            var listModel = _compiledListModelExpression.Invoke(viewModel);
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
                foreach (var line in listData) {
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
            if (validationResults.Count <= 0) return;
            foreach (var item in validationResults)
                modelState.AddModelError(_propertyName, item);
            listModel.Clear();
            foreach (var ctv in listData)
                listModel.Add(Mapper.Map<TProp>(ctv));
        }

    }
}