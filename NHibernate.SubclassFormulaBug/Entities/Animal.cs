namespace NHibernate.SubclassFormulaBug.Entities;

public abstract class Animal
{
    public virtual long Id { get; set; }
    
    public abstract string Summary { get; protected set; }

    public virtual Person Owner { get; set; }
}