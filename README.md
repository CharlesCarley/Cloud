# Cloud 
Is a client/server database system. Its purpose is to translate a class API into a database such that, the translation contains the code necessary to communicate between the two. The primary goal is to make the system easy to manage under two use cases. 
__Case 1:__ _It is being developed and changes to its structure happen frequently._
__Case 2:__ _After an idea has been developed and something new needs to be added to extend functionality._
The solution implemented here is to isolate a core API then generate the backend database functionality.
The diagram at the following [link](Content/CloudDoxy.svg) shows the overall idea.

It is composed of multiple class libraries which each serve a specific purpose.

- [Cloud](#cloud)
    - [Cloud.Common](#cloudcommon)
    - [Cloud.Generator](#cloudgenerator)
    - [Cloud.GeneratorApi](#cloudgeneratorapi)
      - [Cloud.Generator.ClientSQLite](#cloudgeneratorclientsqlite)
      - [Cloud.Generator.ServerMySQL](#cloudgeneratorservermysql)
      - [Cloud.Generator.ServerSQLite](#cloudgeneratorserversqlite)
    - [Cloud.ReflectionApi](#cloudreflectionapi)
    - [Cloud.Store](#cloudstore)
    - [Cloud.Transaction](#cloudtransaction)
  - [Basic testing](#basic-testing)
  - [Dependencies](#dependencies)


### Cloud.Common

Defines a class library that contains the code that is common to all other libraries in this system. 

### Cloud.Generator

Is responsible for sending a user-defined API to a backend code generator to output the database source. It defines a type switch so that different generators may be specified during the build stage. This is accomplished by embedding the [Cloud.Generator.targets](BuildTools/Cloud.Generator.targets) file into a project.  

__Example:__ This would be placed in a user-defined project for the specific database. IE the client or server.

```xml
<PropertyGroup>
    <CloudGeneratorInputAssembly>$(PathToAssembly.dll)</CloudGeneratorInputAssembly>
    <CloudGeneratorOutputFile>$(Result.Filename).cs</CloudGeneratorOutputFile>
    <CloudExtraOptions>Verbose</CloudExtraOptions>
    <CloudGeneratorType>ClientSQLite</CloudGeneratorType>
    <CloudGeneratorRootNamespace>$(Namespace)</CloudGeneratorRootNamespace>
</PropertyGroup>
<Import Project="Cloud.Generator.targets" />
```
Other projects can make use of the generated types by adding a reference to the user-defined project that contains this snippet. 


### Cloud.GeneratorApi

The generator program will search through the input assembly and collect any attributes defined in the reflection API.  It will then use what it finds to swap template variables with the replacements. 


#### Cloud.Generator.ClientSQLite

Is the backend generator to write the SQLite client source.

#### Cloud.Generator.ServerMySQL

Is the backend generator to write the MySQL server source.
_At the moment, only the SQLite back-ends are implemented._

#### Cloud.Generator.ServerSQLite

Is the backend generator to write the SQLite server source.


### Cloud.ReflectionApi

Defines attributes that are used to control the output of the generator.

### Cloud.Store

Defines common server functionality that should be shared with the generated server code.


### Cloud.Transaction

Is a utility library that allows communication between the client and server.

It uses the REST API routes in the generated server code to know where the class data should be sent.     

__The REST API__ is defined a bit differently. Rather than using plain English routes into the server. It uses randomly generated codes to represent the path to the data. Right now the codes are static, but the idea is to be able to cycle the codes in order to shift API access periodically.           

## Basic testing 

In the Visual Studio solution explorer, right click on the root of the solution tree and select Properties. Select Common Properties / Startup Project. From the list of projects enable BookStore.Store and BookStore.FrontEnd as the projects to start. Then Ctrl+F5 will launch both projects for testing. 

See also, [Running in Linux](Content/LinuxStore.md)


## Dependencies

This project makes use of the [sqlite-net](https://github.com/praeclarum/sqlite-net) project.


