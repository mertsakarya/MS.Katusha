using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Domain.Entities
{
    public class Product : BaseFriendlyModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Amount { get; set; }
        public string Tax { get; set; }
        public string Notes { get; set; }
        public string RecurringInformation { get; set; }
        public string ExecutionData { get; set; }

        public ProductExecutionData GetProductExecutionData()
        {
            var productExecutionData = (ProductExecutionData)Newtonsoft.Json.JsonConvert.DeserializeObject(ExecutionData, typeof(ProductExecutionData));
            return productExecutionData;
        }
    }
}
