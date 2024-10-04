using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace NHibernate.SubclassFormulaBug;

public static class AdoDbInitialiser
{
    const string SchemaSql = @"
CREATE TABLE Person (
    Id INTEGER NOT NULL PRIMARY KEY,
    Name TEXT NOT NULL
);

CREATE TABLE Animal (
    Id INTEGER NOT NULL PRIMARY KEY,
    OwnerId INTEGER NOT NULL
);

CREATE TABLE Dog (
    Id INTEGER NOT NULL PRIMARY KEY,
    DogBreedId INTEGER NOT NULL,
    WeightKg REAL NOT NULL
);

CREATE TABLE Bird (
    Id INTEGER NOT NULL PRIMARY KEY,
    BirdBreedId INTEGER NOT NULL,
    Color TEXT NOT NULL
);";

    const string DataSql = @"
INSERT INTO Person (Id, Name)
VALUES (1, 'Joe');
INSERT INTO Animal (Id, OwnerId)
VALUES(1, 1), (2, 1);
INSERT INTO Dog (Id, DogBreedId, WeightKg)
VALUES(1, 5, 5.6);
INSERT INTO Bird(Id, BirdBreedId, Color)
VALUES(2, 4, 'Red');";
    
    public static void CreateSchema(IDbConnection connection)
    {
        using var tran = connection.BeginTransaction();
        using var comm = connection.CreateCommand();
        comm.Transaction = tran;
        comm.CommandText = SchemaSql;
        comm.ExecuteNonQuery();
        tran.Commit();
    }
    
    public static void AddData(IDbConnection connection)
    {
        using var tran = connection.BeginTransaction();
        using var comm = connection.CreateCommand();
        comm.Transaction = tran;
        comm.CommandText = DataSql;
        comm.ExecuteNonQuery();
        tran.Commit();
    }

    public static DbConnection GetConnection() => new SQLiteConnection("Data Source=:memory:");
}