using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JetBlack.MessageBus.Common.Json.Converters
{
    public abstract class StyledDataTableConverter : JsonConverter
    {
        public abstract DataTable ToDataTable(JToken token);
    }
}
