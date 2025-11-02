# Namespace BiemannT
The definitions of the individual namespaces are explained below.

## BiemannT.Common
This namespace contains classes for general use. All classes can be used independently of the current project.

## BiemannT.MUT
This namespace contains the "**M**atrix-**U**nit-**T**esting" namespaces.

### BiemannT.MUT.MsSql
This namespace contains the unit test-classes for Microsoft SQL-Databases.

#### BiemannT.MUT.MsSql.CLI
This namespace contains the command-line-interface application.

#### BiemannT.MUT.MsSql.Def
This namespace contains classes to represent the content of the test definition.

##### BiemannT.MUT.MsSql.Def.Base
This namespace contains abstract classes which should be inherit from all sub-namespaces.

##### BiemannT.MUT.MsSql.Def.JSON
This namespace contains the representation of test definitions in JSON-file format.

#### BiemannT.MUT.MsSql.Tests
This namespace contains the classes to execute the tests.
