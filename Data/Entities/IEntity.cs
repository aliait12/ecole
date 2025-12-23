namespace SchoolManagementSystem.Data.Entities
{
    public interface IEntity
    {
        //This will be what will be common across all Entities
        int Id { get; set; }
    }
}
