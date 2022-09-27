using System.Collections.Specialized;
using System.Web;

namespace BlazorWasm.Client.Utils;

public class UriBuilderExt
{
    private readonly UriBuilder _builder;
    private readonly NameValueCollection _collection;

    public UriBuilderExt(Uri uri)
    {
        _builder = new UriBuilder(uri);
        _collection = HttpUtility.ParseQueryString(string.Empty);
    }

    public Uri Uri
    {
        get
        {
            _builder.Query = _collection.ToString();
            return _builder.Uri;
        }
    }

    public UriBuilderExt AddParameter(string key, string value)
    {
        _collection.Add(key, value);
        return this;
    }
}