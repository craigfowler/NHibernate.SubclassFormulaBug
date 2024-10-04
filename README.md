# Querying formula-mapped properties in a joined-subclass

This is a reproduction case for an NHibernate bug in which incorrect SQL is produced when using a Linq query which makes use of a property which is mapped using a `formula`, when that property is a member of a `joined-subclass`.
As far as I can tell there appear to be two symptoms of this issue:

* The formula is modified from the one in the mapping with the addition of a table alias, but an incorrect table alias is added
* The formula appears to be used naïvely, as a single column when to query it correctly, it needs to be repeated for each subclass

## Versions affected

The test fails on NHibernate versions from **5.0.0** up to **5.5.2** (the latest at time of writing) and also fails on **5.4.9**.
I haven't found any 5.x version on which this passes.
If this issue is a dupe of NHibernate issue 776 (see below) then perhaps this has not worked _for a very long time_.

## The reproduction case

This reprository contains a reproduction case for this problem.
That's a small entity model, mappings, the infrastructure to scaffold an `ISession` with an in-memory SQLite DB, some DDL and sample data SQL and then a unit test to query that object model.
This reproduction case uses SQLite to provide a self-contained test, but we have also seen & reproduced this on MS-SQL, so I suspect SQLite is irrelevant to the issue.

_Please note that the reproduction case is rather contrived._
There would be better ways to accomplish this if our object/entity model were truly this simple.
It is the case that I have stripped out the value/purpose of the formula mapping as part of the effort to create a small self-contained repro case.

To run this reproduction case:

1. Clone the repository
2. Run the tests using .NET 8 or higher: `dotnet test`
    * Optional, to run the tests against a specific NHibernate version set the `NhVersion` property to the desired NHibernate version
    * For example: `dotnet test /p:NhVersion=5.4.9`

### Expected behaviour

The expectation would be that the unit test passes.

### Actual behaviour

The unit test fails with a crash error because of the first problem shown below: "Incorrect table alias".
However, I think that even if the crash issue were resolved in isolation (the correct table alias used with the formula), the test would likely still fail because of the "Naïve usage of the formula value" issue.

### Incorrect table alias

Looking at the SQL which is generated from the reproduction case included (reformatted for readability):

```sql
select
  'Bird, breed ' || pets1_1_.BirdBreedId || ', ' || pets1_1_.Color as col_0_0_
  -- ^^ This is the formula for Summary in BirdMapping.cs
  -- but notice how the table alias which is prepended to the column names
  -- is the alias used with the Dog table, not Bird
from
  Person person0_
  inner join Animal pets1_ on person0_.Id = pets1_.Owner
  left outer join Dog pets1_1_ on pets1_.Id = pets1_1_.dog_key
  --                  ^^^^^^^^ that table alias is declared here
  left outer join Bird pets1_2_ on pets1_.Id = pets1_2_.bird_key
where
  person0_.Name = @p0
```

This prepending of the wrong table alias can lead to a SQL error if, as is the case here, the column names used in the formula are unique to the table for the subclass.
In this instance, the `BirdBreedId` and `Color` columns are unique to the `Bird` table and don't exist on `Dog`.
So, the query raises a SQL column not found error.

### Naïve usage of the formula value

Whilst the misused table alias above can cause a SQL error, the overall approach does not seem correct, because it uses just a single result column and only one variant of the formula.
I think that in order to select correct results in this scenario, the generated SQL would need to look a little more like the following.

```sql
select
  case
    when pets1_1_.Id is not null
    then 'Dog, breed ' || pets1_1_.DogBreedId || ', ' || pets1_1_.WeightKg || ' KG'
    when pets1_2_.Id is not null
    then 'Bird, breed ' || pets1_1_.BirdBreedId || ', ' || pets1_1_.Color
    else null -- what should happen here? this shoudln't be possible!
  end as col_0_0_
from
  Person person0_
  inner join Animal pets1_ on person0_.Id = pets1_.Owner
  left outer join Dog pets1_1_ on pets1_.Id = pets1_1_.dog_key
  left outer join Bird pets1_2_ on pets1_.Id = pets1_2_.bird_key
where
  person0_.Name = @p0
```

That could alternatively be separated out as three separate result columns (for simplicity), one which determines which concrete class we're dealing with and then another column for each possible variant of the formula-mapped property value.
We'd only use the result from _one_ of the formula columns, dependent upon which concrete class we're dealing with.

## Possibly duplicates an old issue

I had a look through NHibernate's issue tracker and I wonder if this problem is a duplicate of [this very old **issue #776** from 2006 & the old Jira issue tracker](https://github.com/nhibernate/nhibernate-core/issues/776).
I'm not confident enough to say that it definitely is a dupe, but I'd like to point it out just in case.


