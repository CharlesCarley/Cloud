# CustomCloud 

 
CustomCloud is a collection of tools and libraries that allows a 
movable cloud which can be set up on a local computer, virtual machine, or remote server.

_This project is still largely unfinished, and should be kept private. Not just because it shows how ambitiously mediocre I am, but i'm out of time to work on it. I'm making it public because it shows some database/REST experience. Even though primitive at best_    

## Architectural overview

The basic idea for this design is be able to define specific types and properties that should be persisted in both local and external databases. The primary goal is to make the database easy to manage under two use cases. __Case 1:__ _It is being developed and changes to it's structure happen frequently._ __Case 2:__ _After an idea has been developed and something new needs to be added in order to extend functionality._

The solution implemented here for both cases is to isolate a core data API then 
generate the backend database functionality. 

__Overview__

![](Content/CloudDoxy.svg)


## [Cloud.ReflectionApi](Source/Cloud..ReflectionApi)

In order to use this system, an API should be defined with Cloud.ReflectionApi, which contains the data structures that define how the API will be generated. 
The Cloud.Generator will use the user defined API to generate both the local, and the server backend database code. 

## [Cloud.Generator](Source/Cloud.Generator)

Is the program that will generate the code.
The generator can be set to automatically 
run during build by embedding [Cloud.Generator.targets](BuildTools/Cloud.Generator.targets) file.
into a project. 


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

![Overview](Content/Cloud.Database.svg)



The cloud database module is responsible for defining the reflection API. 
That API gets linked to a user module, which implements any desired data types.
After that, the output of the user module gets fed back into the generator build tool. The generator defines a type switch that determines the output database code.
The final output of the generator should be fed back into a project wishing to used the database types. 


## [Cloud.Store](Source/Cloud.Store)


## [Cloud.Transaction](Source/Cloud.Transaction)

The transaction layer communicates between the client and host.

![Overview](Content/Cloud.Transaction.svg)


## Utilities

[clean.sh](clean.sh) calls python [BuildTools/DeleteFiles.py](BuildTools/DeleteFiles.py) which loads the contents of .gitignore and deletes matches from the filesystem.




## References

+ [w3.org - Root](https://www.w3.org/Protocols/)
+ [w3.org - Semantics and Content](https://datatracker.ietf.org/doc/html/rfc7231#section-4.3)
