namespace NATSInternal.Helpers;

public static class PaginationHelperExtension
{
    public static PaginationRangeModel PaginationRange(
            this IHtmlHelper _,
            int pageCount,
            int currentPage,
            int maxButtonCount)
    {
        PaginationRangeModel model = new PaginationRangeModel();
        if (pageCount >= maxButtonCount)
        {
            if (currentPage - (int)Math.Floor((decimal)maxButtonCount / 2) <= 1)
            {
                model.StartingPage = 1;
                model.EndingPage = model.StartingPage + (maxButtonCount - 1);
            }
            else if (currentPage + Math.Floor((decimal)maxButtonCount / 2) > pageCount)
            {
                model.EndingPage = pageCount;
                model.StartingPage = model.EndingPage - (maxButtonCount - 1);
            }
            else
            {
                model.StartingPage = (int)Math.Ceiling(currentPage - (decimal)maxButtonCount / 2);
                model.EndingPage = (int)Math.Floor(currentPage + (decimal)maxButtonCount / 2);
            }
        }
        else
        {
            model.StartingPage = 1;
            model.EndingPage = pageCount;
        }
        return model;
    }
}
