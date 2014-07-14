Tru.Repo
========

Experimental repository pattern, aiming to bundle ORM with SQL database patching, such
that creating a new repo in code will automatically patch the database if necessary. The
attempt to patch the database will only happen the first time the repo is instantiated.

To run the tests simply update the 2 connection strings in
`Tru.Repo.Test/Given_a_valid_sql_connection_string.cs` so they are pointing at an accessible
sql server instance.
