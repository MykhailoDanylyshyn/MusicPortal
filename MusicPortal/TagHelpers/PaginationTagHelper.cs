using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;
using MusicPortal;

[HtmlTargetElement("pagination")]
public class PaginationTagHelper : TagHelper
{
    private readonly IUrlHelperFactory _urlHelperFactory;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public PaginationTagHelper(
        IUrlHelperFactory urlHelperFactory,
        IStringLocalizer<SharedResource> localizer)
    {
        _urlHelperFactory = urlHelperFactory;
        _localizer = localizer;
    }

    [HtmlAttributeName("current-page")]
    public int CurrentPage { get; set; }

    [HtmlAttributeName("total-pages")]
    public int TotalPages { get; set; }

    [HtmlAttributeName("action")]
    public string Action { get; set; } = "Index";

    [HtmlAttributeName("controller")]
    public string? Controller { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (TotalPages <= 1)
        {
            output.SuppressOutput();
            return;
        }

        CurrentPage = Math.Clamp(CurrentPage, 1, TotalPages);

        var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
        var queryValues = new Dictionary<string, object?>();

        foreach (var item in ViewContext.HttpContext.Request.Query)
        {
            if (!string.Equals(item.Key, "page", StringComparison.OrdinalIgnoreCase))
                queryValues[item.Key] = item.Value.Count == 1 ? item.Value[0] : item.Value.ToArray();
        }

        output.TagName = "nav";
        output.Attributes.SetAttribute("aria-label", _localizer["Pagination_Label"]);
        output.Content.SetHtmlContent(BuildPagination(urlHelper, queryValues));
    }

    private string BuildPagination(IUrlHelper urlHelper, Dictionary<string, object?> queryValues)
    {
        var html = new System.Text.StringBuilder();
        html.Append("<ul class=\"pagination pagination-custom justify-content-center mt-4\">");

        AddPrevious(html, urlHelper, queryValues);

        var start = Math.Max(1, CurrentPage - 2);
        var end = Math.Min(TotalPages, CurrentPage + 2);

        if (start > 1)
        {
            AddPage(html, urlHelper, queryValues, 1);
            if (start > 2)
                html.Append("<li class=\"page-item disabled\"><span class=\"page-link\">…</span></li>");
        }

        for (var page = start; page <= end; page++)
            AddPage(html, urlHelper, queryValues, page);

        if (end < TotalPages)
        {
            if (end < TotalPages - 1)
                html.Append("<li class=\"page-item disabled\"><span class=\"page-link\">…</span></li>");
            AddPage(html, urlHelper, queryValues, TotalPages);
        }

        AddNext(html, urlHelper, queryValues);

        html.Append("</ul>");
        return html.ToString();
    }

    private void AddPrevious(StringBuilder html, IUrlHelper urlHelper, Dictionary<string, object?> queryValues)
    {
        var disabled = CurrentPage == 1 ? " disabled" : string.Empty;
        var link = CurrentPage == 1
            ? "#"
            : BuildUrl(urlHelper, queryValues, CurrentPage - 1);

        html.Append($"<li class=\"page-item{disabled}\"><a class=\"page-link\" href=\"{link}\">« {_localizer["Pagination_Previous"]}</a></li>");
    }

    private void AddNext(StringBuilder html, IUrlHelper urlHelper, Dictionary<string, object?> queryValues)
    {
        var disabled = CurrentPage == TotalPages ? " disabled" : string.Empty;
        var link = CurrentPage == TotalPages
            ? "#"
            : BuildUrl(urlHelper, queryValues, CurrentPage + 1);

        html.Append($"<li class=\"page-item{disabled}\"><a class=\"page-link\" href=\"{link}\">{_localizer["Pagination_Next"]} »</a></li>");
    }

    private void AddPage(StringBuilder html, IUrlHelper urlHelper, Dictionary<string, object?> queryValues, int page)
    {
        var active = page == CurrentPage ? " active" : string.Empty;
        var link = BuildUrl(urlHelper, queryValues, page);
        html.Append($"<li class=\"page-item{active}\"><a class=\"page-link\" href=\"{link}\">{page}</a></li>");
    }

    private string BuildUrl(IUrlHelper urlHelper, Dictionary<string, object?> queryValues, int page)
    {
        queryValues["page"] = page;
        return urlHelper.Action(Action, Controller, queryValues) ?? "#";
    }
}
