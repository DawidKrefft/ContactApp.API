namespace ContactApp.API.Models.Domain
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Contact> Contacts { get; set; }

        public List<Subcategory> Subcategories { get; set; }
    }
}
