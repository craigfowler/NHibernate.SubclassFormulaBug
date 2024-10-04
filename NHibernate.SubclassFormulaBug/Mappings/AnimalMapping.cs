using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.SubclassFormulaBug.Entities;

namespace NHibernate.SubclassFormulaBug.Mappings;

public class AnimalMapping : ClassMapping<Animal>
{
    public AnimalMapping()
    {
        Table("Animal");
        
        Id(x => x.Id, m =>
        {
            m.Generator(Generators.Identity);
        });
        
        ManyToOne(x => x.Owner);
    }
}