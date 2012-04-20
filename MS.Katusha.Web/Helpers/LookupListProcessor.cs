using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace MS.Katusha.Web.Helpers
{
    public class LookupListProcessor<TModel, TModelData, TProp, TPropData, TEnum> where TModelData : MS.Katusha.Domain.Entities.BaseEntities.BaseModel where TEnum : struct
    {

        public delegate void DeleteEvent(TModelData modelData, TEnum enumValue);
        public delegate void AddEvent(TModelData modelData, TEnum enumValue);

        private readonly string _propertyName;

        private readonly Func<TPropData, TEnum> _compiledItemDataEnumExpression;
        private readonly Func<TModelData, IList<TPropData>> _compiledListDataExpression;
        private readonly Func<TModel,IList<TProp>> _compiledListModelExpression;
        private readonly Func<TProp, TEnum> _compiledItemModelEnumExpression;
        private readonly AddEvent _add;
        private readonly DeleteEvent _delete;

        public LookupListProcessor(
            Expression<Func<TModelData, IList<TPropData>>> listDataExpression,
            Expression<Func<TModel, IList<TProp>>> listModelExpression,
            Expression<Func<TPropData, TEnum>> itemDataEnumExpression,
            Expression<Func<TProp, TEnum>> itemModelEnumExpression,
            DeleteEvent delete,
            AddEvent add
            )
        {

            _compiledListDataExpression = listDataExpression.Compile();
            _compiledListModelExpression = listModelExpression.Compile();
            _compiledItemDataEnumExpression = itemDataEnumExpression.Compile();
            _propertyName = typeof (TProp).Name;
            _compiledItemModelEnumExpression = itemModelEnumExpression.Compile();
            _add = add;
            _delete = delete;
        }

        public void Process(HttpRequestBase request, ModelStateDictionary modelState, TModel viewModel, TModelData dataModel, bool performDataOperation = true)
        {
            var listData = _compiledListDataExpression.Invoke(dataModel);
            var listModel = _compiledListModelExpression.Invoke(viewModel);
            IList<string> validationResults = new List<string>();
            try {
                var setForm = new HashSet<TEnum>();
                foreach(var item in listModel) setForm.Add(_compiledItemModelEnumExpression.Invoke(item));

                var setData = new HashSet<TEnum>();
                foreach (var line in listData) {
                    var item = _compiledItemDataEnumExpression.Invoke(line);
                    setData.Add(item);
                    if (!setForm.Contains(item)) {
                        try {
                            if(performDataOperation) _delete(dataModel, item);
                        } catch (Exception) {
                            validationResults.Add(item.ToString() + " Can't Delete");
                        }
                    }
                }
                foreach (var item in setForm) {
                    if (!setData.Contains(item)) {
                        try {
                            if (performDataOperation) _add(dataModel, item);
                        } catch (Exception) {
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
        }

    }
}