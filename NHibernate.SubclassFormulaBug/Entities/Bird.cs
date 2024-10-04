namespace NHibernate.SubclassFormulaBug.Entities;

public class Bird : Animal
{
    public virtual long BirdBreedId { get; set; }

    public virtual string Color { get; set; }

    // Typically I'd have some logic such that in-memory entities exhibit the same behaviour as the SQL formula,
    // but that's irrelevant to the issue being reported, so I omitted it for simplicity.
    public override string Summary { get; protected set; }
}