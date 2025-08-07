namespace Store.DTO.Result
{
    public class ListItem<T>
    {
        public List<T> Items { get; set; } = new List<T>();
    }

    public static class ListItemExtensions
    {
        public static ListItem<T> ToListItem<T>(this List<T> Items)
        {
            return new ListItem<T> { Items = Items };
        }
    }
}
