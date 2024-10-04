using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.SubclassFormulaBug.Entities;

namespace NHibernate.SubclassFormulaBug.Mappings;

public class DogMapping : JoinedSubclassMapping<Dog>
{
    public DogMapping()
    {
        Property(x => x.DogBreedId);
        
        Property(x => x.WeightKg);
        
        Property(x => x.Summary, p =>
        {
            p.Formula("'Dog, breed ' || DogBreedId || ', ' || WeightKg || ' KG'");
        });
    }
}