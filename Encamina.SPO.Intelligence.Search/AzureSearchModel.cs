using Microsoft.Azure.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;


namespace Encamina.SPO.Intelligence.Search
{
    public class AzureSearchModel
    {
        [System.ComponentModel.DataAnnotations.Key]
        [IsFilterable]
        public string Id { get; set; }

        [IsRetrievable(true), IsSearchable, IsSortable]
        public string Name { get; set; }

        [IsRetrievable(true)]
        public string IdSharepoint { get; set; }

        [IsRetrievable(true), IsFacetable, IsFilterable, IsSearchable]
        public List<string> Tags { get; set; }
    }
    public class Result<T> where T : class
    {
        public long? Count { get; set; }
        public IEnumerable<T> Collection { get; set; }
        public FacetResults Facet { get; set; }
        public SearchContinuationToken ContinuationToken { get; set; }
    }

    public class FullResult
    {
        public string SearchResult { get; set; }
        public string SharepointResult { get; set; }
    }


}
