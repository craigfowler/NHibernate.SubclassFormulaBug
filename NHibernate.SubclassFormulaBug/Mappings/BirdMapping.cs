using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.SubclassFormulaBug.Entities;

namespace NHibernate.SubclassFormulaBug.Mappings;

public class BirdMapping : JoinedSubclassMapping<Bird>
{
    public BirdMapping()
    {
        Property(x => x.BirdBreedId);
        
        Property(x => x.Color);
        
        Property(x => x.Summary, p =>
        {
            p.Formula("'Bird, breed ' || BirdBreedId || ', ' || Color");
        });
    }
}