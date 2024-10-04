using System.Collections.Generic;

namespace NHibernate.SubclassFormulaBug.Entities;

public class Person
{
    public virtual long Id { get; set; }

    public virtual string Name { get; set; }

    public virtual ISet<Animal> Pets { get; protected set; } = new HashSet<Animal>();
}