using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.SubclassFormulaBug.Mappings;

namespace NHibernate.SubclassFormulaBug;

public static class SessionFactoryCreator
{
    public static ISessionFactory GetSessionFactory()
    {
        var config = GetCommonConfiguration();
        var mapper = new ModelMapper();
        mapper.AddMappings(new []{typeof(PersonMapping), typeof(AnimalMapping), typeof(DogMapping), typeof(BirdMapping)});
        var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
        config.AddMapping(mappings);
        return config.BuildSessionFactory();
    }

    static Configuration GetCommonConfiguration()
    {
        var config = new Configuration();
        config.DataBaseIntegration(x =>
        {
            x.Dialect<SQLiteDialect>();
            x.ConnectionString = "Data Source=:memory:";
        });
        return config;
    }
}