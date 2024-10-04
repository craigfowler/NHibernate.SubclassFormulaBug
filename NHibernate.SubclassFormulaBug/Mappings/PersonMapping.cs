using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.SubclassFormulaBug.Entities;

namespace NHibernate.SubclassFormulaBug.Mappings;

public class PersonMapping : ClassMapping<Person>
{
    public PersonMapping()
    {
        Id(x => x.Id);
        
        Property(x => x.Name);
        
        Set(x => x.Pets, s =>
        {
            s.Inverse(true);
        },
            m => m.OneToMany());
    }
}